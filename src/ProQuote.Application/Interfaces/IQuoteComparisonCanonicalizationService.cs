using ProQuote.Application.DTOs.Quotes;

namespace ProQuote.Application.Interfaces;

/// <summary>
/// Service contract for canonical quote comparison normalization.
/// </summary>
public interface IQuoteComparisonCanonicalizationService
{
    /// <summary>
    /// Builds canonical normalized comparison payload.
    /// </summary>
    /// <param name="comparison">Raw comparison payload.</param>
    /// <returns>Canonical normalized comparison model.</returns>
    public CanonicalQuoteComparisonDto Build(BuyerQuoteComparisonDto comparison);
}
