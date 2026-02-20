namespace RFQApp.Domain.Entities;

/// <summary>
/// Represents an in-app notification for a user.
/// </summary>
/// <remarks>
/// Notifications are created for various events like RFQ invitations,
/// quote submissions, deadline reminders, and award announcements.
/// </remarks>
public class Notification : BaseEntity
{
    #region Properties

    /// <summary>
    /// Gets or sets the identifier of the user receiving the notification.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the notification title.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the notification message.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the notification type (e.g., "RfqInvitation", "QuoteReceived", "Award").
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the URL to navigate to when the notification is clicked.
    /// </summary>
    public string? ActionUrl { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the related entity (e.g., RFQ ID, Quote ID).
    /// </summary>
    public Guid? RelatedEntityId { get; set; }

    /// <summary>
    /// Gets or sets the type of the related entity (e.g., "Rfq", "Quote").
    /// </summary>
    public string? RelatedEntityType { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the notification has been read.
    /// </summary>
    public bool IsRead { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when the notification was read.
    /// </summary>
    public DateTime? ReadAt { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when the notification was sent.
    /// </summary>
    public DateTime SentAt { get; set; }

    #endregion

    #region Navigation Properties

    /// <summary>
    /// Gets or sets the user receiving the notification.
    /// </summary>
    public virtual ApplicationUser User { get; set; } = null!;

    #endregion

    #region Methods

    /// <summary>
    /// Marks the notification as read and records the timestamp.
    /// </summary>
    public void MarkAsRead()
    {
        if (!IsRead)
        {
            IsRead = true;
            ReadAt = DateTime.UtcNow;
        }
    }

    #endregion
}
