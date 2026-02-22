namespace ProQuote.Application.UseCases.BuyerRfqs.PublishRfq;

/// <summary>
/// Application use-case contract for publishing buyer RFQs.
/// </summary>
public interface IPublishBuyerRfqUseCase
{
    /// <summary>
    /// Executes RFQ publish workflow.
    /// </summary>
    /// <param name="command">Publish command payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Publish response.</returns>
    Task<PublishBuyerRfqResponse> ExecuteAsync(PublishBuyerRfqCommand command, CancellationToken cancellationToken = default);
}
