using ProQuote.Application.Interfaces;

namespace ProQuote.Application.UseCases.Notifications.MarkAllNotificationsAsRead;

/// <summary>
/// Application use-case implementation for marking all notifications as read.
/// </summary>
public sealed class MarkAllNotificationsAsReadUseCase : IMarkAllNotificationsAsReadUseCase
{
    private readonly ICommunicationService _communicationService;

    /// <summary>
    /// Initializes a new instance of the <see cref="MarkAllNotificationsAsReadUseCase"/> class.
    /// </summary>
    /// <param name="communicationService">Communication service.</param>
    public MarkAllNotificationsAsReadUseCase(ICommunicationService communicationService)
    {
        _communicationService = communicationService;
    }

    /// <inheritdoc />
    public async Task<MarkAllNotificationsAsReadResponse> ExecuteAsync(
        MarkAllNotificationsAsReadCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        cancellationToken.ThrowIfCancellationRequested();

        if (command.UserId == Guid.Empty)
        {
            return MarkAllNotificationsAsReadResponse.Failure("Invalid mark-all-as-read request.");
        }

        int updatedCount = await _communicationService.MarkAllNotificationsAsReadAsync(command.UserId);
        return MarkAllNotificationsAsReadResponse.Success(updatedCount);
    }
}
