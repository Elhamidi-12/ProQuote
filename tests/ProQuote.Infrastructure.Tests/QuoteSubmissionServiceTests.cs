using Microsoft.EntityFrameworkCore;

using ProQuote.Application.DTOs.Quotes;
using ProQuote.Domain.Entities;
using ProQuote.Domain.Enums;
using ProQuote.Infrastructure.Identity;
using ProQuote.Infrastructure.Services;
using ProQuote.Infrastructure.Tests.TestHelpers;
using Xunit;

namespace ProQuote.Infrastructure.Tests;

public class QuoteSubmissionServiceTests
{
    [Fact]
    public async Task SaveQuoteAsync_ShouldCreateQuote_AndMarkInvitationQuoted()
    {
        using var context = TestDbContextFactory.Create(Guid.NewGuid().ToString());

        (ApplicationUserIdentity buyer, ApplicationUserIdentity supplierUser, Supplier supplier, Rfq rfq, RfqInvitation invitation, LineItem lineItem) =
            await SeedQuoteScenarioAsync(context, DateTime.UtcNow.AddDays(3));

        AuditLogService auditLogService = new(context);
        QuoteSubmissionService service = new(context, auditLogService);

        SaveQuoteRequest request = new()
        {
            RfqId = rfq.Id,
            LeadTimeDays = 10,
            ValidUntil = DateTime.UtcNow.AddDays(15),
            PaymentTerms = "Net 30",
            Notes = "Test quote",
            LineItems =
            [
                new SaveQuoteLineItemRequest
                {
                    LineItemId = lineItem.Id,
                    UnitPrice = 120m
                }
            ]
        };

        QuoteSaveResponse response = await service.SaveQuoteAsync(supplierUser.Id, request);

        Quote? savedQuote = await context.Quotes.FirstOrDefaultAsync(q => q.RfqId == rfq.Id && q.SupplierId == supplier.Id);
        List<QuoteQualityHistory> qualityHistory = [];
        if (savedQuote is not null)
        {
            qualityHistory = await context.QuoteQualityHistory
                .Where(h => h.QuoteId == savedQuote.Id)
                .OrderBy(h => h.ScoredAt)
                .ToListAsync();
        }
        RfqInvitation? updatedInvitation = await context.RfqInvitations.FirstOrDefaultAsync(i => i.RfqId == rfq.Id && i.SupplierId == supplier.Id);
        Rfq? updatedRfq = await context.Rfqs.FirstOrDefaultAsync(r => r.Id == rfq.Id);
        AuditLog? audit = await context.AuditLogs.FirstOrDefaultAsync(a => a.RfqId == rfq.Id && a.Action == "QuoteSubmitted");

        Assert.True(response.Succeeded);
        Assert.NotNull(savedQuote);
        Assert.Equal(120m * lineItem.Quantity, savedQuote!.TotalAmount);
        Assert.Equal(89, savedQuote.SubmissionQualityScore);
        Assert.Equal(100, savedQuote.SubmissionCompletenessScore);
        Assert.Equal(85, savedQuote.SubmissionLeadTimeScore);
        Assert.Equal(70, savedQuote.SubmissionCommercialScore);
        Assert.NotNull(savedQuote.SubmissionQualityScoredAt);
        Assert.Single(qualityHistory);
        Assert.Equal("Submitted", qualityHistory[0].EventType);
        Assert.Equal(savedQuote.SubmissionQualityScore, qualityHistory[0].OverallScore);
        Assert.Equal(InvitationStatus.Quoted, updatedInvitation!.Status);
        Assert.Equal(RfqStatus.QuotesReceived, updatedRfq!.Status);
        Assert.NotNull(audit);
    }

    [Fact]
    public async Task SaveQuoteAsync_ShouldFail_WhenDeadlineHasPassed()
    {
        using var context = TestDbContextFactory.Create(Guid.NewGuid().ToString());

        (_, ApplicationUserIdentity supplierUser, _, Rfq rfq, _, LineItem lineItem) =
            await SeedQuoteScenarioAsync(context, DateTime.UtcNow.AddMinutes(-5));

        AuditLogService auditLogService = new(context);
        QuoteSubmissionService service = new(context, auditLogService);

        SaveQuoteRequest request = new()
        {
            RfqId = rfq.Id,
            LeadTimeDays = 5,
            ValidUntil = DateTime.UtcNow.AddDays(10),
            LineItems =
            [
                new SaveQuoteLineItemRequest
                {
                    LineItemId = lineItem.Id,
                    UnitPrice = 50m
                }
            ]
        };

        QuoteSaveResponse response = await service.SaveQuoteAsync(supplierUser.Id, request);

        Assert.False(response.Succeeded);
        Assert.Contains("deadline", response.ErrorMessage ?? string.Empty, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task RecalculateQuoteQualitySnapshotAsync_ShouldIncludeSupportingDocuments()
    {
        using var context = TestDbContextFactory.Create(Guid.NewGuid().ToString());

        (_, ApplicationUserIdentity supplierUser, Supplier supplier, Rfq rfq, _, LineItem lineItem) =
            await SeedQuoteScenarioAsync(context, DateTime.UtcNow.AddDays(3));

        AuditLogService auditLogService = new(context);
        QuoteSubmissionService service = new(context, auditLogService);

        Quote quote = new()
        {
            Id = Guid.NewGuid(),
            RfqId = rfq.Id,
            SupplierId = supplier.Id,
            Status = QuoteStatus.Submitted,
            LeadTimeDays = 10,
            ValidUntil = DateTime.UtcNow.AddDays(10),
            PaymentTerms = "Net 30",
            Notes = "Quality recalc test",
            SubmittedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            LineItems =
            [
                new QuoteLineItem
                {
                    Id = Guid.NewGuid(),
                    QuoteId = Guid.Empty, // reassigned after quote attach
                    LineItemId = lineItem.Id,
                    UnitPrice = 10m,
                    TotalPrice = 20m,
                    CreatedAt = DateTime.UtcNow
                }
            ]
        };
        quote.LineItems.First().QuoteId = quote.Id;
        quote.CalculateTotalAmount();
        await context.Quotes.AddAsync(quote);
        await context.SaveChangesAsync();

        int? initialScore = quote.SubmissionQualityScore;

        await context.QuoteDocuments.AddAsync(new QuoteDocument
        {
            Id = Guid.NewGuid(),
            QuoteId = quote.Id,
            FileName = "proposal.pdf",
            StoredFileName = "proposal.pdf",
            ContentType = "application/pdf",
            FileSize = 1024,
            FilePath = "/api/v1/documents/quote/test",
            DisplayOrder = 1,
            UploadedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        });
        await context.SaveChangesAsync();

        await service.RecalculateQuoteQualitySnapshotAsync(supplierUser.Id, quote.Id);

        Quote? refreshed = await context.Quotes.FirstOrDefaultAsync(q => q.Id == quote.Id);
        List<QuoteQualityHistory> qualityHistory = await context.QuoteQualityHistory
            .Where(h => h.QuoteId == quote.Id)
            .OrderBy(h => h.ScoredAt)
            .ToListAsync();
        Assert.NotNull(refreshed);
        Assert.True(refreshed!.SubmissionQualityScore > (initialScore ?? 0));
        Assert.Equal(100, refreshed.SubmissionCommercialScore);
        Assert.Single(qualityHistory);
        Assert.Equal("Recalculated", qualityHistory[0].EventType);
    }

    private static async Task<(ApplicationUserIdentity Buyer, ApplicationUserIdentity SupplierUser, Supplier Supplier, Rfq Rfq, RfqInvitation Invitation, LineItem LineItem)>
        SeedQuoteScenarioAsync(Infrastructure.Data.AppDbContext context, DateTime submissionDeadlineUtc)
    {
        ApplicationUserIdentity buyer = new()
        {
            Id = Guid.NewGuid(),
            UserName = "buyer@test.local",
            Email = "buyer@test.local",
            FirstName = "Buyer",
            LastName = "User",
            CreatedAt = DateTime.UtcNow
        };

        ApplicationUserIdentity supplierUser = new()
        {
            Id = Guid.NewGuid(),
            UserName = "supplier@test.local",
            Email = "supplier@test.local",
            FirstName = "Supplier",
            LastName = "User",
            CreatedAt = DateTime.UtcNow
        };

        Category category = new()
        {
            Id = Guid.NewGuid(),
            Name = "IT",
            Description = "IT",
            DisplayOrder = 1,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        Supplier supplier = new()
        {
            Id = Guid.NewGuid(),
            UserId = supplierUser.Id,
            CompanyName = "Supplier Inc",
            ContactName = "Supplier User",
            Email = supplierUser.Email!,
            Status = SupplierStatus.Approved,
            RegisteredAt = DateTime.UtcNow,
            ApprovedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        Rfq rfq = new()
        {
            Id = Guid.NewGuid(),
            ReferenceNumber = $"RFQ-{DateTime.UtcNow.Year}-99999",
            Title = "Test RFQ",
            Description = "RFQ for tests",
            CategoryId = category.Id,
            Currency = "USD",
            SubmissionDeadline = submissionDeadlineUtc,
            Status = RfqStatus.Published,
            BuyerId = buyer.Id,
            CreatedAt = DateTime.UtcNow,
            PublishedAt = DateTime.UtcNow
        };

        LineItem lineItem = new()
        {
            Id = Guid.NewGuid(),
            RfqId = rfq.Id,
            Name = "Service",
            Quantity = 2,
            UnitOfMeasure = "hrs",
            DisplayOrder = 1,
            CreatedAt = DateTime.UtcNow
        };

        RfqInvitation invitation = new()
        {
            Id = Guid.NewGuid(),
            RfqId = rfq.Id,
            SupplierId = supplier.Id,
            SecureToken = Guid.NewGuid().ToString("N"),
            Status = InvitationStatus.Sent,
            SentAt = DateTime.UtcNow,
            TokenExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        };

        await context.Users.AddRangeAsync(buyer, supplierUser);
        await context.Categories.AddAsync(category);
        await context.Suppliers.AddAsync(supplier);
        await context.Rfqs.AddAsync(rfq);
        await context.LineItems.AddAsync(lineItem);
        await context.RfqInvitations.AddAsync(invitation);
        await context.SaveChangesAsync();

        return (buyer, supplierUser, supplier, rfq, invitation, lineItem);
    }
}
