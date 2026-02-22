namespace ProQuote.Application.UseCases.Notifications.MarkNotificationAsRead;

/// <summary>
/// Command payload for marking a notification as read.
/// </summary>
/// <param name="UserId">Current user identifier.</param>
/// <param name="NotificationId">Notification identifier.</param>
public sealed record MarkNotificationAsReadCommand(
    Guid UserId,
    Guid NotificationId);
