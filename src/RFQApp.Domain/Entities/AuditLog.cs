namespace RFQApp.Domain.Entities;

/// <summary>
/// Represents an audit log entry for tracking changes to RFQs.
/// </summary>
/// <remarks>
/// Every significant action on an RFQ is logged with the user, action,
/// timestamp, and old/new values for full traceability.
/// </remarks>
public class AuditLog : BaseEntity
{
    #region Properties

    /// <summary>
    /// Gets or sets the identifier of the RFQ this log entry relates to.
    /// </summary>
    public Guid RfqId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who performed the action.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the type of action performed (e.g., "Created", "StatusChanged", "QuoteSubmitted").
    /// </summary>
    public string Action { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the entity type that was affected (e.g., "RFQ", "Quote", "LineItem").
    /// </summary>
    public string EntityType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the identifier of the affected entity.
    /// </summary>
    public Guid? EntityId { get; set; }

    /// <summary>
    /// Gets or sets the value before the change (JSON serialized).
    /// </summary>
    public string? OldValue { get; set; }

    /// <summary>
    /// Gets or sets the value after the change (JSON serialized).
    /// </summary>
    public string? NewValue { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when the action was performed.
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the IP address of the user who performed the action.
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// Gets or sets additional details about the action.
    /// </summary>
    public string? Details { get; set; }

    #endregion

    #region Navigation Properties

    /// <summary>
    /// Gets or sets the RFQ this log entry relates to.
    /// </summary>
    public virtual Rfq Rfq { get; set; } = null!;

    /// <summary>
    /// Gets or sets the user who performed the action.
    /// </summary>
    public virtual ApplicationUser User { get; set; } = null!;

    #endregion
}
