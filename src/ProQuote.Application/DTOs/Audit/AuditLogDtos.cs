namespace ProQuote.Application.DTOs.Audit;

/// <summary>
/// Audit log list item payload.
/// </summary>
public sealed class AuditLogListItemDto
{
    /// <summary>
    /// Gets or sets audit log identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets related RFQ identifier.
    /// </summary>
    public Guid RfqId { get; set; }

    /// <summary>
    /// Gets or sets RFQ reference number.
    /// </summary>
    public string RfqReferenceNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets actor user identifier.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets actor display name.
    /// </summary>
    public string UserDisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets action name.
    /// </summary>
    public string Action { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets entity type.
    /// </summary>
    public string EntityType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets entity identifier.
    /// </summary>
    public Guid? EntityId { get; set; }

    /// <summary>
    /// Gets or sets details text.
    /// </summary>
    public string? Details { get; set; }

    /// <summary>
    /// Gets or sets timestamp in UTC.
    /// </summary>
    public DateTime Timestamp { get; set; }
}
