using ProQuote.Application.DTOs.Quotes;

namespace ProQuote.Application.Interfaces;

/// <summary>
/// Service contract for buyer quote comparison and award workflows.
/// </summary>
public interface IBuyerQuoteManagementService
{
    /// <summary>
    /// Gets RFQ-level list items that have buyer quotes.
    /// </summary>
    /// <param name="buyerUserId">Buyer user identifier.</param>
    /// <returns>RFQ quote list.</returns>
    public Task<IReadOnlyList<BuyerQuoteRfqListItemDto>> GetBuyerQuoteRfqsAsync(Guid buyerUserId);

    /// <summary>
    /// Gets quote comparison payload for a buyer RFQ.
    /// </summary>
    /// <param name="buyerUserId">Buyer user identifier.</param>
    /// <param name="rfqId">RFQ identifier.</param>
    /// <returns>Comparison payload, or null if inaccessible.</returns>
    public Task<BuyerQuoteComparisonDto?> GetComparisonAsync(Guid buyerUserId, Guid rfqId);

    /// <summary>
    /// Awards a quote for the buyer RFQ and rejects remaining submitted quotes.
    /// </summary>
    /// <param name="buyerUserId">Buyer user identifier.</param>
    /// <param name="rfqId">RFQ identifier.</param>
    /// <param name="quoteId">Selected quote identifier.</param>
    /// <param name="buyerNotes">Optional buyer notes for awarded quote.</param>
    /// <returns>Award result.</returns>
    public Task<AwardQuoteResponse> AwardQuoteAsync(Guid buyerUserId, Guid rfqId, Guid quoteId, string? buyerNotes = null);
}
