namespace ProQuote.Application.UseCases.Notifications.MarkNotificationAsRead;

/// <summary>
/// Application use-case contract for marking a notification as read.
/// </summary>
public interface IMarkNotificationAsReadUseCase
{
    /// <summary>
    /// Executes mark-as-read workflow.
    /// </summary>
    /// <param name="command">Mark-as-read command payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Mark-as-read response.</returns>
    Task<MarkNotificationAsReadResponse> ExecuteAsync(
        MarkNotificationAsReadCommand command,
        CancellationToken cancellationToken = default);
}
