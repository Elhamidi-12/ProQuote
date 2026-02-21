namespace ProQuote.Application.DTOs.Communication;

/// <summary>
/// Represents a user notification item.
/// </summary>
public class NotificationItemDto
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
    /// Gets or sets action URL.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1056:URI-like properties should not be strings", Justification = "Stored as route string for UI navigation compatibility.")]
    public string? ActionUrl { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the notification is read.
    /// </summary>
    public bool IsRead { get; set; }

    /// <summary>
    /// Gets or sets sent timestamp in UTC.
    /// </summary>
    public DateTime SentAt { get; set; }
}

/// <summary>
/// Represents an RFQ option for message filtering/composing.
/// </summary>
public class MessagingRfqOptionDto
{
    /// <summary>
    /// Gets or sets RFQ identifier.
    /// </summary>
    public Guid RfqId { get; set; }

    /// <summary>
    /// Gets or sets RFQ reference number.
    /// </summary>
    public string ReferenceNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets RFQ title.
    /// </summary>
    public string Title { get; set; } = string.Empty;
}

/// <summary>
/// Represents a message item in an RFQ thread.
/// </summary>
public class MessageThreadItemDto
{
    /// <summary>
    /// Gets or sets message identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets RFQ identifier.
    /// </summary>
    public Guid RfqId { get; set; }

    /// <summary>
    /// Gets or sets RFQ reference number.
    /// </summary>
    public string RfqReferenceNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets RFQ title.
    /// </summary>
    public string RfqTitle { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets sender full name.
    /// </summary>
    public string SenderName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets message body.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether sender is buyer.
    /// </summary>
    public bool IsFromBuyer { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether message is private.
    /// </summary>
    public bool IsPrivate { get; set; }

    /// <summary>
    /// Gets or sets sent timestamp in UTC.
    /// </summary>
    public DateTime SentAt { get; set; }
}

/// <summary>
/// Represents a request to post a message in an RFQ thread.
/// </summary>
public class SendMessageRequest
{
    /// <summary>
    /// Gets or sets RFQ identifier.
    /// </summary>
    public Guid RfqId { get; set; }

    /// <summary>
    /// Gets or sets message body.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets optional private target supplier identifier.
    /// Only used for buyer-originated messages.
    /// </summary>
    public Guid? TargetSupplierId { get; set; }
}
