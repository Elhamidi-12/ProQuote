namespace ProQuote.Application.UseCases.Notifications.MarkAllNotificationsAsRead;

/// <summary>
/// Command payload for marking all notifications as read for a user.
/// </summary>
/// <param name="UserId">Current user identifier.</param>
public sealed record MarkAllNotificationsAsReadCommand(Guid UserId);
