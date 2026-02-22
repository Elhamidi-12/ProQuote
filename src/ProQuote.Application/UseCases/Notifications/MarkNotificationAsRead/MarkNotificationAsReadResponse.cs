namespace ProQuote.Application.UseCases.Notifications.MarkNotificationAsRead;

/// <summary>
/// Result payload for marking a notification as read.
/// </summary>
public sealed class MarkNotificationAsReadResponse
{
    /// <summary>
    /// Gets or sets a value indicating whether operation succeeded.
    /// </summary>
    public bool Succeeded { get; set; }

    /// <summary>
    /// Gets or sets notification identifier.
    /// </summary>
    public Guid NotificationId { get; set; }

    /// <summary>
    /// Gets or sets optional error message when operation fails.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Creates successful response.
    /// </summary>
    /// <param name="notificationId">Notification identifier.</param>
    /// <returns>Success response.</returns>
    public static MarkNotificationAsReadResponse Success(Guid notificationId)
    {
        return new MarkNotificationAsReadResponse
        {
            Succeeded = true,
            NotificationId = notificationId
        };
    }

    /// <summary>
    /// Creates failed response.
    /// </summary>
    /// <param name="notificationId">Notification identifier.</param>
    /// <param name="errorMessage">Failure reason.</param>
    /// <returns>Failure response.</returns>
    public static MarkNotificationAsReadResponse Failure(Guid notificationId, string errorMessage)
    {
        return new MarkNotificationAsReadResponse
        {
            Succeeded = false,
            NotificationId = notificationId,
            ErrorMessage = errorMessage
        };
    }
}
