using ProQuote.Application.DTOs.Quotes;
using ProQuote.Application.Interfaces;

namespace ProQuote.Infrastructure.Services;

/// <summary>
/// Canonical quote comparison normalization service.
/// </summary>
public sealed class QuoteComparisonCanonicalizationService : IQuoteComparisonCanonicalizationService
{
    /// <inheritdoc />
    public CanonicalQuoteComparisonDto Build(BuyerQuoteComparisonDto comparison)
    {
        ArgumentNullException.ThrowIfNull(comparison);

        CanonicalQuoteComparisonDto canonical = new();
        Dictionary<Guid, BuyerQuoteComparisonLineItemDto> lineItemMap = comparison.RfqLineItems
            .ToDictionary(li => li.LineItemId);

        foreach (BuyerQuoteComparisonLineItemDto lineItem in comparison.RfqLineItems)
        {
            List<decimal> unitPrices = comparison.Quotes
                .SelectMany(q => q.Prices)
                .Where(p => p.LineItemId == lineItem.LineItemId && p.UnitPrice > 0m)
                .Select(p => p.UnitPrice)
                .OrderBy(p => p)
                .ToList();

            canonical.LineBaselines.Add(new CanonicalLineBaselineDto
            {
                LineItemId = lineItem.LineItemId,
                Name = lineItem.Name,
                MedianUnitPrice = GetMedian(unitPrices),
                MinUnitPrice = unitPrices.Count == 0 ? null : unitPrices.Min(),
                MaxUnitPrice = unitPrices.Count == 0 ? null : unitPrices.Max()
            });
        }

        Dictionary<Guid, CanonicalLineBaselineDto> baselineByLineId = canonical.LineBaselines
            .ToDictionary(x => x.LineItemId);

        foreach (BuyerQuoteComparisonItemDto quote in comparison.Quotes)
        {
            CanonicalQuoteEntryDto quoteEntry = new()
            {
                QuoteId = quote.QuoteId
            };

            int pricedLines = 0;
            decimal normalizedTotal = 0m;

            foreach (BuyerQuoteComparisonLineItemDto lineItem in comparison.RfqLineItems)
            {
                BuyerQuoteComparisonPriceDto? price = quote.Prices.FirstOrDefault(p => p.LineItemId == lineItem.LineItemId);
                decimal? unitPrice = price is { UnitPrice: > 0m } ? price.UnitPrice : null;
                decimal? totalPrice = price is { TotalPrice: > 0m } ? price.TotalPrice : null;

                if (totalPrice.HasValue)
                {
                    normalizedTotal += totalPrice.Value;
                }

                if (unitPrice.HasValue)
                {
                    pricedLines++;
                }

                decimal? variancePct = null;
                if (unitPrice.HasValue &&
                    baselineByLineId.TryGetValue(lineItem.LineItemId, out CanonicalLineBaselineDto? baseline) &&
                    baseline.MedianUnitPrice.HasValue &&
                    baseline.MedianUnitPrice.Value > 0m)
                {
                    variancePct = Math.Round(
                        (unitPrice.Value - baseline.MedianUnitPrice.Value) * 100m / baseline.MedianUnitPrice.Value,
                        2);
                }

                quoteEntry.Lines.Add(new CanonicalQuoteLineValueDto
                {
                    LineItemId = lineItem.LineItemId,
                    UnitPrice = unitPrice,
                    TotalPrice = totalPrice,
                    VarianceFromMedianPercent = variancePct
                });
            }

            quoteEntry.CoveragePercent = lineItemMap.Count == 0
                ? 0m
                : Math.Round(pricedLines * 100m / lineItemMap.Count, 2);
            quoteEntry.NormalizedTotalAmount = normalizedTotal;

            canonical.Quotes.Add(quoteEntry);
        }

        return canonical;
    }

    private static decimal? GetMedian(List<decimal> values)
    {
        if (values.Count == 0)
        {
            return null;
        }

        int mid = values.Count / 2;
        if (values.Count % 2 == 0)
        {
            return Math.Round((values[mid - 1] + values[mid]) / 2m, 4);
        }

        return values[mid];
    }
}
