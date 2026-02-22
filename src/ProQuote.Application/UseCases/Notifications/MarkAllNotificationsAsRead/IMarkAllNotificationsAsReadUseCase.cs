namespace ProQuote.Application.UseCases.Notifications.MarkAllNotificationsAsRead;

/// <summary>
/// Application use-case contract for marking all notifications as read.
/// </summary>
public interface IMarkAllNotificationsAsReadUseCase
{
    /// <summary>
    /// Executes mark-all-as-read workflow.
    /// </summary>
    /// <param name="command">Mark-all command payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Mark-all response.</returns>
    Task<MarkAllNotificationsAsReadResponse> ExecuteAsync(
        MarkAllNotificationsAsReadCommand command,
        CancellationToken cancellationToken = default);
}
