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

        QuoteSubmissionService service = new(context);

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
        RfqInvitation? updatedInvitation = await context.RfqInvitations.FirstOrDefaultAsync(i => i.RfqId == rfq.Id && i.SupplierId == supplier.Id);
        Rfq? updatedRfq = await context.Rfqs.FirstOrDefaultAsync(r => r.Id == rfq.Id);

        Assert.True(response.Succeeded);
        Assert.NotNull(savedQuote);
        Assert.Equal(120m * lineItem.Quantity, savedQuote!.TotalAmount);
        Assert.Equal(InvitationStatus.Quoted, updatedInvitation!.Status);
        Assert.Equal(RfqStatus.QuotesReceived, updatedRfq!.Status);
    }

    [Fact]
    public async Task SaveQuoteAsync_ShouldFail_WhenDeadlineHasPassed()
    {
        using var context = TestDbContextFactory.Create(Guid.NewGuid().ToString());

        (_, ApplicationUserIdentity supplierUser, _, Rfq rfq, _, LineItem lineItem) =
            await SeedQuoteScenarioAsync(context, DateTime.UtcNow.AddMinutes(-5));

        QuoteSubmissionService service = new(context);

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
