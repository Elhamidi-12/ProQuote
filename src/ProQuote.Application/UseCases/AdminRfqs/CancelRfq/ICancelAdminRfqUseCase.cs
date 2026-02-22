namespace ProQuote.Application.UseCases.AdminRfqs.CancelRfq;

/// <summary>
/// Application use-case contract for cancelling RFQs as admin.
/// </summary>
public interface ICancelAdminRfqUseCase
{
    /// <summary>
    /// Executes RFQ cancel workflow.
    /// </summary>
    /// <param name="command">Cancel command payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Cancel response.</returns>
    Task<CancelAdminRfqResponse> ExecuteAsync(
        CancelAdminRfqCommand command,
        CancellationToken cancellationToken = default);
}
