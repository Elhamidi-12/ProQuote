using RFQApp.Domain.Enums;

namespace RFQApp.Domain.Entities;

/// <summary>
/// Represents a Request for Quotation (RFQ) created by a buyer.
/// </summary>
/// <remarks>
/// An RFQ is the core entity of the system. It contains line items that suppliers
/// will quote on, and goes through a defined lifecycle from Draft to Closed.
/// </remarks>
public class Rfq : BaseEntity
{
    #region Properties

    /// <summary>
    /// Gets or sets the auto-generated reference number (e.g., RFQ-2025-00142).
    /// </summary>
    public string ReferenceNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the title of the RFQ.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the detailed description of the RFQ requirements.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the category identifier for this RFQ.
    /// </summary>
    public Guid CategoryId { get; set; }

    /// <summary>
    /// Gets or sets the currency code for all monetary values (e.g., USD, EUR).
    /// </summary>
    public string Currency { get; set; } = "USD";

    /// <summary>
    /// Gets or sets the deadline for suppliers to submit their quotes (UTC).
    /// </summary>
    public DateTime SubmissionDeadline { get; set; }

    /// <summary>
    /// Gets or sets the current status of the RFQ in its lifecycle.
    /// </summary>
    public RfqStatus Status { get; set; } = RfqStatus.Draft;

    /// <summary>
    /// Gets or sets the identifier of the buyer who created this RFQ.
    /// </summary>
    public Guid BuyerId { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when the RFQ was published to suppliers.
    /// </summary>
    public DateTime? PublishedAt { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when the RFQ was awarded.
    /// </summary>
    public DateTime? AwardedAt { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when the RFQ was closed.
    /// </summary>
    public DateTime? ClosedAt { get; set; }

    /// <summary>
    /// Gets or sets the reason for cancellation if the RFQ was cancelled.
    /// </summary>
    public string? CancellationReason { get; set; }

    #endregion

    #region Navigation Properties

    /// <summary>
    /// Gets or sets the buyer who created this RFQ.
    /// </summary>
    public virtual ApplicationUser Buyer { get; set; } = null!;

    /// <summary>
    /// Gets or sets the category of this RFQ.
    /// </summary>
    public virtual Category Category { get; set; } = null!;

    /// <summary>
    /// Gets or sets the collection of line items for this RFQ.
    /// </summary>
    public virtual ICollection<LineItem> LineItems { get; set; } = new List<LineItem>();

    /// <summary>
    /// Gets or sets the collection of supplier invitations for this RFQ.
    /// </summary>
    public virtual ICollection<RfqInvitation> Invitations { get; set; } = new List<RfqInvitation>();

    /// <summary>
    /// Gets or sets the collection of documents attached to this RFQ.
    /// </summary>
    public virtual ICollection<RfqDocument> Documents { get; set; } = new List<RfqDocument>();

    /// <summary>
    /// Gets or sets the collection of quotes received for this RFQ.
    /// </summary>
    public virtual ICollection<Quote> Quotes { get; set; } = new List<Quote>();

    /// <summary>
    /// Gets or sets the collection of Q&amp;A messages for this RFQ.
    /// </summary>
    public virtual ICollection<QaMessage> QaMessages { get; set; } = new List<QaMessage>();

    /// <summary>
    /// Gets or sets the collection of audit log entries for this RFQ.
    /// </summary>
    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    #endregion

    #region Methods

    /// <summary>
    /// Determines whether the RFQ can be edited.
    /// </summary>
    /// <returns><c>true</c> if the RFQ is in Draft status; otherwise, <c>false</c>.</returns>
    public bool CanBeEdited()
    {
        return Status == RfqStatus.Draft;
    }

    /// <summary>
    /// Determines whether the RFQ can be published.
    /// </summary>
    /// <returns><c>true</c> if the RFQ is in Draft status and has at least one line item; otherwise, <c>false</c>.</returns>
    public bool CanBePublished()
    {
        return Status == RfqStatus.Draft && LineItems.Count > 0;
    }

    /// <summary>
    /// Determines whether the RFQ can accept new quotes.
    /// </summary>
    /// <returns><c>true</c> if the RFQ is Published or QuotesReceived and deadline has not passed; otherwise, <c>false</c>.</returns>
    public bool CanAcceptQuotes()
    {
        bool isValidStatus = Status == RfqStatus.Published || Status == RfqStatus.QuotesReceived;
        bool isBeforeDeadline = DateTime.UtcNow < SubmissionDeadline;
        return isValidStatus && isBeforeDeadline;
    }

    /// <summary>
    /// Determines whether the RFQ can be awarded.
    /// </summary>
    /// <returns><c>true</c> if the RFQ has received quotes and is under evaluation; otherwise, <c>false</c>.</returns>
    public bool CanBeAwarded()
    {
        return Status == RfqStatus.UnderEvaluation && Quotes.Count > 0;
    }

    /// <summary>
    /// Determines whether the RFQ can be cancelled.
    /// </summary>
    /// <returns><c>true</c> if the RFQ is not already closed, cancelled, or awarded; otherwise, <c>false</c>.</returns>
    public bool CanBeCancelled()
    {
        return Status != RfqStatus.Closed &&
               Status != RfqStatus.Cancelled &&
               Status != RfqStatus.Awarded;
    }

    #endregion
}
