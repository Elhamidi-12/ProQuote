namespace ProQuote.Application.UseCases.AdminRfqs.CloseRfq;

/// <summary>
/// Application use-case contract for closing RFQs as admin.
/// </summary>
public interface ICloseAdminRfqUseCase
{
    /// <summary>
    /// Executes RFQ close workflow.
    /// </summary>
    /// <param name="command">Close command payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Close response.</returns>
    Task<CloseAdminRfqResponse> ExecuteAsync(
        CloseAdminRfqCommand command,
        CancellationToken cancellationToken = default);
}
