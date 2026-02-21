using Microsoft.EntityFrameworkCore;

using ProQuote.Domain.Entities;
using ProQuote.Domain.Enums;
using ProQuote.Infrastructure.Identity;
using ProQuote.Infrastructure.Services;
using ProQuote.Infrastructure.Tests.TestHelpers;
using Xunit;

namespace ProQuote.Infrastructure.Tests;

public class BuyerQuoteManagementServiceTests
{
    [Fact]
    public async Task AwardQuoteAsync_ShouldMarkWinner_RejectOthers_AndAwardRfq()
    {
        using var context = TestDbContextFactory.Create(Guid.NewGuid().ToString());

        (ApplicationUserIdentity buyer, Rfq rfq, Quote winningQuote, Quote losingQuote) = await SeedAwardScenarioAsync(context);
        AuditLogService auditLogService = new(context);
        BuyerQuoteManagementService service = new(context, auditLogService);

        var response = await service.AwardQuoteAsync(buyer.Id, rfq.Id, winningQuote.Id, "Best total and lead time");

        Quote? updatedWinner = await context.Quotes.FirstOrDefaultAsync(q => q.Id == winningQuote.Id);
        Quote? updatedLoser = await context.Quotes.FirstOrDefaultAsync(q => q.Id == losingQuote.Id);
        Rfq? updatedRfq = await context.Rfqs.FirstOrDefaultAsync(r => r.Id == rfq.Id);
        AuditLog? audit = await context.AuditLogs.FirstOrDefaultAsync(a => a.RfqId == rfq.Id && a.Action == "QuoteAwarded");

        Assert.True(response.Succeeded);
        Assert.NotNull(updatedWinner);
        Assert.NotNull(updatedLoser);
        Assert.NotNull(updatedRfq);
        Assert.True(updatedWinner!.IsAwarded);
        Assert.Equal(QuoteStatus.Awarded, updatedWinner.Status);
        Assert.Equal("Best total and lead time", updatedWinner.BuyerNotes);
        Assert.False(updatedLoser!.IsAwarded);
        Assert.Equal(QuoteStatus.Rejected, updatedLoser.Status);
        Assert.Equal(RfqStatus.Awarded, updatedRfq!.Status);
        Assert.NotNull(updatedRfq.AwardedAt);
        Assert.NotNull(audit);
    }

    [Fact]
    public async Task AwardQuoteAsync_ShouldFail_ForDifferentBuyer()
    {
        using var context = TestDbContextFactory.Create(Guid.NewGuid().ToString());

        (ApplicationUserIdentity buyer, Rfq rfq, Quote winningQuote, _) = await SeedAwardScenarioAsync(context);
        AuditLogService auditLogService = new(context);
        BuyerQuoteManagementService service = new(context, auditLogService);

        var response = await service.AwardQuoteAsync(Guid.NewGuid(), rfq.Id, winningQuote.Id);

        Assert.False(response.Succeeded);
        Assert.Contains("not found", response.ErrorMessage ?? string.Empty, StringComparison.OrdinalIgnoreCase);

        Quote? unchangedQuote = await context.Quotes.FirstOrDefaultAsync(q => q.Id == winningQuote.Id);
        Rfq? unchangedRfq = await context.Rfqs.FirstOrDefaultAsync(r => r.Id == rfq.Id);
        Assert.NotNull(unchangedQuote);
        Assert.NotNull(unchangedRfq);
        Assert.Equal(QuoteStatus.Submitted, unchangedQuote!.Status);
        Assert.Equal(RfqStatus.QuotesReceived, unchangedRfq!.Status);
    }

    private static async Task<(ApplicationUserIdentity Buyer, Rfq Rfq, Quote WinningQuote, Quote LosingQuote)>
        SeedAwardScenarioAsync(Infrastructure.Data.AppDbContext context)
    {
        ApplicationUserIdentity buyer = new()
        {
            Id = Guid.NewGuid(),
            UserName = "buyer-award@test.local",
            Email = "buyer-award@test.local",
            FirstName = "Buyer",
            LastName = "Award",
            CreatedAt = DateTime.UtcNow
        };

        ApplicationUserIdentity supplierUserA = new()
        {
            Id = Guid.NewGuid(),
            UserName = "supplier-a@test.local",
            Email = "supplier-a@test.local",
            FirstName = "Supplier",
            LastName = "A",
            CreatedAt = DateTime.UtcNow
        };

        ApplicationUserIdentity supplierUserB = new()
        {
            Id = Guid.NewGuid(),
            UserName = "supplier-b@test.local",
            Email = "supplier-b@test.local",
            FirstName = "Supplier",
            LastName = "B",
            CreatedAt = DateTime.UtcNow
        };

        Category category = new()
        {
            Id = Guid.NewGuid(),
            Name = "Facilities",
            Description = "Facilities",
            DisplayOrder = 1,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        Supplier supplierA = new()
        {
            Id = Guid.NewGuid(),
            UserId = supplierUserA.Id,
            CompanyName = "Alpha Supplies",
            ContactName = "Supplier A",
            Email = supplierUserA.Email!,
            Status = SupplierStatus.Approved,
            RegisteredAt = DateTime.UtcNow,
            ApprovedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        Supplier supplierB = new()
        {
            Id = Guid.NewGuid(),
            UserId = supplierUserB.Id,
            CompanyName = "Beta Supplies",
            ContactName = "Supplier B",
            Email = supplierUserB.Email!,
            Status = SupplierStatus.Approved,
            RegisteredAt = DateTime.UtcNow,
            ApprovedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        Rfq rfq = new()
        {
            Id = Guid.NewGuid(),
            ReferenceNumber = $"RFQ-{DateTime.UtcNow.Year}-88888",
            Title = "Award RFQ Test",
            Description = "Award test RFQ",
            CategoryId = category.Id,
            Currency = "USD",
            SubmissionDeadline = DateTime.UtcNow.AddDays(-1),
            Status = RfqStatus.QuotesReceived,
            BuyerId = buyer.Id,
            CreatedAt = DateTime.UtcNow,
            PublishedAt = DateTime.UtcNow.AddDays(-7)
        };

        LineItem lineItem = new()
        {
            Id = Guid.NewGuid(),
            RfqId = rfq.Id,
            Name = "Labor",
            Quantity = 10,
            UnitOfMeasure = "hrs",
            DisplayOrder = 1,
            CreatedAt = DateTime.UtcNow
        };

        Quote winningQuote = new()
        {
            Id = Guid.NewGuid(),
            RfqId = rfq.Id,
            SupplierId = supplierA.Id,
            Status = QuoteStatus.Submitted,
            LeadTimeDays = 5,
            ValidUntil = DateTime.UtcNow.AddDays(10),
            TotalAmount = 1000m,
            SubmittedAt = DateTime.UtcNow.AddDays(-2),
            CreatedAt = DateTime.UtcNow.AddDays(-2)
        };
        winningQuote.LineItems.Add(new QuoteLineItem
        {
            Id = Guid.NewGuid(),
            QuoteId = winningQuote.Id,
            LineItemId = lineItem.Id,
            UnitPrice = 100m,
            TotalPrice = 1000m,
            CreatedAt = DateTime.UtcNow.AddDays(-2)
        });

        Quote losingQuote = new()
        {
            Id = Guid.NewGuid(),
            RfqId = rfq.Id,
            SupplierId = supplierB.Id,
            Status = QuoteStatus.Submitted,
            LeadTimeDays = 6,
            ValidUntil = DateTime.UtcNow.AddDays(10),
            TotalAmount = 1100m,
            SubmittedAt = DateTime.UtcNow.AddDays(-2),
            CreatedAt = DateTime.UtcNow.AddDays(-2)
        };
        losingQuote.LineItems.Add(new QuoteLineItem
        {
            Id = Guid.NewGuid(),
            QuoteId = losingQuote.Id,
            LineItemId = lineItem.Id,
            UnitPrice = 110m,
            TotalPrice = 1100m,
            CreatedAt = DateTime.UtcNow.AddDays(-2)
        });

        await context.Users.AddRangeAsync(buyer, supplierUserA, supplierUserB);
        await context.Categories.AddAsync(category);
        await context.Suppliers.AddRangeAsync(supplierA, supplierB);
        await context.Rfqs.AddAsync(rfq);
        await context.LineItems.AddAsync(lineItem);
        await context.Quotes.AddRangeAsync(winningQuote, losingQuote);
        await context.SaveChangesAsync();

        return (buyer, rfq, winningQuote, losingQuote);
    }
}
