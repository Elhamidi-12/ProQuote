using ProQuote.Application.Interfaces;

namespace ProQuote.Application.UseCases.Notifications.MarkNotificationAsRead;

/// <summary>
/// Application use-case implementation for marking a notification as read.
/// </summary>
public sealed class MarkNotificationAsReadUseCase : IMarkNotificationAsReadUseCase
{
    private readonly ICommunicationService _communicationService;

    /// <summary>
    /// Initializes a new instance of the <see cref="MarkNotificationAsReadUseCase"/> class.
    /// </summary>
    /// <param name="communicationService">Communication service.</param>
    public MarkNotificationAsReadUseCase(ICommunicationService communicationService)
    {
        _communicationService = communicationService;
    }

    /// <inheritdoc />
    public async Task<MarkNotificationAsReadResponse> ExecuteAsync(
        MarkNotificationAsReadCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        cancellationToken.ThrowIfCancellationRequested();

        if (command.UserId == Guid.Empty || command.NotificationId == Guid.Empty)
        {
            return MarkNotificationAsReadResponse.Failure(command.NotificationId, "Invalid mark-as-read request.");
        }

        bool updated = await _communicationService.MarkNotificationAsReadAsync(command.UserId, command.NotificationId);
        return updated
            ? MarkNotificationAsReadResponse.Success(command.NotificationId)
            : MarkNotificationAsReadResponse.Failure(command.NotificationId, "Notification not found.");
    }
}
