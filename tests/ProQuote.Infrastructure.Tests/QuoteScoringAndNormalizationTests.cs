using ProQuote.Application.DTOs.Quotes;
using ProQuote.Domain.Entities;
using ProQuote.Domain.Enums;
using ProQuote.Infrastructure.Identity;
using ProQuote.Infrastructure.Services;
using ProQuote.Infrastructure.Tests.TestHelpers;
using Xunit;

namespace ProQuote.Infrastructure.Tests;

public class QuoteScoringAndNormalizationTests
{
    [Fact]
    public async Task SaveTemplateAsync_ShouldNormalizeWeightsToHundred()
    {
        string databaseName = Guid.NewGuid().ToString();
        using var context = TestDbContextFactory.Create(databaseName);

        ApplicationUserIdentity buyer = new()
        {
            Id = Guid.NewGuid(),
            UserName = "buyer-template@test.local",
            Email = "buyer-template@test.local",
            FirstName = "Buyer",
            LastName = "Template",
            CreatedAt = DateTime.UtcNow
        };

        Category category = new()
        {
            Id = Guid.NewGuid(),
            Name = "Template Category",
            DisplayOrder = 1,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        Rfq rfq = new()
        {
            Id = Guid.NewGuid(),
            ReferenceNumber = "RFQ-TEMPLATE-00001",
            Title = "Template Test RFQ",
            CategoryId = category.Id,
            Currency = "USD",
            SubmissionDeadline = DateTime.UtcNow.AddDays(7),
            Status = RfqStatus.QuotesReceived,
            BuyerId = buyer.Id,
            CreatedAt = DateTime.UtcNow
        };

        await context.Users.AddAsync(buyer);
        await context.Categories.AddAsync(category);
        await context.Rfqs.AddAsync(rfq);
        await context.SaveChangesAsync();

        QuoteScoringTemplateService service = new(context);

        QuoteScoringTemplateDto saved = await service.SaveTemplateAsync(
            buyer.Id,
            rfq.Id,
            new QuoteScoringTemplateDto
            {
                PriceWeight = 3m,
                LeadTimeWeight = 2m,
                CoverageWeight = 1m
            });

        decimal total = saved.PriceWeight + saved.LeadTimeWeight + saved.CoverageWeight;
        Assert.Equal(100m, Math.Round(total, 2));
        Assert.Equal(50m, saved.PriceWeight);
        Assert.Equal(33.33m, saved.LeadTimeWeight);
        Assert.Equal(16.67m, saved.CoverageWeight);

        using var verificationContext = TestDbContextFactory.Create(databaseName);
        QuoteScoringTemplateService verificationService = new(verificationContext);
        QuoteScoringTemplateDto loaded = await verificationService.GetTemplateAsync(buyer.Id, rfq.Id);

        Assert.Equal(saved.PriceWeight, loaded.PriceWeight);
        Assert.Equal(saved.LeadTimeWeight, loaded.LeadTimeWeight);
        Assert.Equal(saved.CoverageWeight, loaded.CoverageWeight);
    }

    [Fact]
    public void Canonicalization_ShouldComputeCoverageAndVariance()
    {
        BuyerQuoteComparisonDto comparison = new()
        {
            RfqId = Guid.NewGuid(),
            ReferenceNumber = "RFQ-CANON-00001",
            Title = "Canonical Test",
            Currency = "USD",
            RfqLineItems =
            [
                new BuyerQuoteComparisonLineItemDto { LineItemId = Guid.NewGuid(), Name = "Item A", Quantity = 1, UnitOfMeasure = "ea" },
                new BuyerQuoteComparisonLineItemDto { LineItemId = Guid.NewGuid(), Name = "Item B", Quantity = 1, UnitOfMeasure = "ea" }
            ]
        };

        Guid lineA = comparison.RfqLineItems[0].LineItemId;
        Guid lineB = comparison.RfqLineItems[1].LineItemId;

        comparison.Quotes.Add(new BuyerQuoteComparisonItemDto
        {
            QuoteId = Guid.NewGuid(),
            SupplierId = Guid.NewGuid(),
            SupplierName = "Supplier One",
            Status = QuoteStatus.Submitted,
            LeadTimeDays = 5,
            TotalAmount = 200m,
            Prices =
            [
                new BuyerQuoteComparisonPriceDto { LineItemId = lineA, UnitPrice = 100m, TotalPrice = 100m },
                new BuyerQuoteComparisonPriceDto { LineItemId = lineB, UnitPrice = 100m, TotalPrice = 100m }
            ]
        });

        comparison.Quotes.Add(new BuyerQuoteComparisonItemDto
        {
            QuoteId = Guid.NewGuid(),
            SupplierId = Guid.NewGuid(),
            SupplierName = "Supplier Two",
            Status = QuoteStatus.Submitted,
            LeadTimeDays = 6,
            TotalAmount = 90m,
            Prices =
            [
                new BuyerQuoteComparisonPriceDto { LineItemId = lineA, UnitPrice = 90m, TotalPrice = 90m }
            ]
        });

        QuoteComparisonCanonicalizationService service = new();
        CanonicalQuoteComparisonDto canonical = service.Build(comparison);

        Assert.Equal(2, canonical.LineBaselines.Count);
        Assert.Equal(2, canonical.Quotes.Count);

        CanonicalLineBaselineDto baselineA = Assert.Single(canonical.LineBaselines, x => x.LineItemId == lineA);
        Assert.Equal(95m, baselineA.MedianUnitPrice);

        CanonicalQuoteEntryDto quoteOne = Assert.Single(canonical.Quotes, x => x.QuoteId == comparison.Quotes[0].QuoteId);
        CanonicalQuoteEntryDto quoteTwo = Assert.Single(canonical.Quotes, x => x.QuoteId == comparison.Quotes[1].QuoteId);

        Assert.Equal(100m, quoteOne.CoveragePercent);
        Assert.Equal(50m, quoteTwo.CoveragePercent);

        CanonicalQuoteLineValueDto quoteTwoLineA = Assert.Single(quoteTwo.Lines, x => x.LineItemId == lineA);
        Assert.Equal(-5.26m, quoteTwoLineA.VarianceFromMedianPercent);
    }
}
