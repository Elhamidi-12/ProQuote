using ProQuote.Application.DTOs.Quotes;

namespace ProQuote.Application.UseCases.BuyerRfqs.AwardQuote;

/// <summary>
/// Command payload for awarding a quote for a buyer RFQ.
/// </summary>
/// <param name="BuyerUserId">Current buyer user identifier.</param>
/// <param name="RfqId">RFQ identifier.</param>
/// <param name="QuoteId">Selected quote identifier.</param>
/// <param name="BuyerNotes">Optional buyer notes attached to award decision.</param>
public sealed record AwardBuyerQuoteCommand(
    Guid BuyerUserId,
    Guid RfqId,
    Guid QuoteId,
    string? BuyerNotes);
