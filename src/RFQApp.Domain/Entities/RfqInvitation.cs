using RFQApp.Domain.Enums;

namespace RFQApp.Domain.Entities;

/// <summary>
/// Represents an invitation sent to a supplier to participate in an RFQ.
/// </summary>
/// <remarks>
/// Invitations are created when a buyer publishes an RFQ and selects suppliers.
/// Each invitation contains a secure token for email-based access.
/// </remarks>
public class RfqInvitation : BaseEntity
{
    #region Properties

    /// <summary>
    /// Gets or sets the identifier of the RFQ.
    /// </summary>
    public Guid RfqId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the invited supplier.
    /// </summary>
    public Guid SupplierId { get; set; }

    /// <summary>
    /// Gets or sets the secure token for email-based RFQ access.
    /// </summary>
    public string SecureToken { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the current status of the invitation.
    /// </summary>
    public InvitationStatus Status { get; set; } = InvitationStatus.Sent;

    /// <summary>
    /// Gets or sets the UTC timestamp when the invitation was sent.
    /// </summary>
    public DateTime SentAt { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when the supplier first viewed the RFQ.
    /// </summary>
    public DateTime? ViewedAt { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when the supplier submitted a quote.
    /// </summary>
    public DateTime? QuotedAt { get; set; }

    /// <summary>
    /// Gets or sets the reason for declining if the supplier declined.
    /// </summary>
    public string? DeclineReason { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when the token expires.
    /// </summary>
    public DateTime TokenExpiresAt { get; set; }

    #endregion

    #region Navigation Properties

    /// <summary>
    /// Gets or sets the RFQ associated with this invitation.
    /// </summary>
    public virtual Rfq Rfq { get; set; } = null!;

    /// <summary>
    /// Gets or sets the invited supplier.
    /// </summary>
    public virtual Supplier Supplier { get; set; } = null!;

    #endregion

    #region Methods

    /// <summary>
    /// Determines whether the secure token is still valid.
    /// </summary>
    /// <returns><c>true</c> if the token has not expired; otherwise, <c>false</c>.</returns>
    public bool IsTokenValid()
    {
        return DateTime.UtcNow < TokenExpiresAt;
    }

    /// <summary>
    /// Marks the invitation as viewed and records the timestamp.
    /// </summary>
    public void MarkAsViewed()
    {
        if (Status == InvitationStatus.Sent)
        {
            Status = InvitationStatus.Viewed;
            ViewedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Marks the invitation as quoted and records the timestamp.
    /// </summary>
    public void MarkAsQuoted()
    {
        Status = InvitationStatus.Quoted;
        QuotedAt = DateTime.UtcNow;
    }

    #endregion
}
