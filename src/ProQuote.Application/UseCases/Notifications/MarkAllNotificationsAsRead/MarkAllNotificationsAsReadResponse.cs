namespace ProQuote.Application.UseCases.Notifications.MarkAllNotificationsAsRead;

/// <summary>
/// Result payload for marking all notifications as read.
/// </summary>
public sealed class MarkAllNotificationsAsReadResponse
{
    /// <summary>
    /// Gets or sets a value indicating whether operation succeeded.
    /// </summary>
    public bool Succeeded { get; set; }

    /// <summary>
    /// Gets or sets number of updated notifications.
    /// </summary>
    public int UpdatedCount { get; set; }

    /// <summary>
    /// Gets or sets optional error message when operation fails.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Creates successful response.
    /// </summary>
    /// <param name="updatedCount">Updated item count.</param>
    /// <returns>Success response.</returns>
    public static MarkAllNotificationsAsReadResponse Success(int updatedCount)
    {
        return new MarkAllNotificationsAsReadResponse
        {
            Succeeded = true,
            UpdatedCount = updatedCount
        };
    }

    /// <summary>
    /// Creates failed response.
    /// </summary>
    /// <param name="errorMessage">Failure reason.</param>
    /// <returns>Failure response.</returns>
    public static MarkAllNotificationsAsReadResponse Failure(string errorMessage)
    {
        return new MarkAllNotificationsAsReadResponse
        {
            Succeeded = false,
            ErrorMessage = errorMessage
        };
    }
}
