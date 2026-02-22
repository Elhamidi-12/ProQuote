using ProQuote.Application.DTOs.Quotes;

namespace ProQuote.Application.UseCases.BuyerRfqs.AwardQuote;

/// <summary>
/// Application use-case contract for awarding buyer RFQ quotes.
/// </summary>
public interface IAwardBuyerQuoteUseCase
{
    /// <summary>
    /// Executes quote award workflow.
    /// </summary>
    /// <param name="command">Award command payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Award response.</returns>
    Task<AwardQuoteResponse> ExecuteAsync(AwardBuyerQuoteCommand command, CancellationToken cancellationToken = default);
}
