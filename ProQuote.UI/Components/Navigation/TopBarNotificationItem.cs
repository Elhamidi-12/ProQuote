namespace ProQuote.UI.Components.Navigation;

/// <summary>
/// Notification item displayed by the top bar dropdown.
/// </summary>
public sealed class TopBarNotificationItem
{
    /// <summary>
    /// Gets or sets notification identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets notification title.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets notification message.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets notification type.
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets target route when clicking the item.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1056:URI-like properties should not be strings", Justification = "Route is stored as application-relative string.")]
    public string? ActionUrl { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the notification is already read.
    /// </summary>
    public bool IsRead { get; set; }

    /// <summary>
    /// Gets or sets sent timestamp in UTC.
    /// </summary>
    public DateTime SentAt { get; set; }
}
