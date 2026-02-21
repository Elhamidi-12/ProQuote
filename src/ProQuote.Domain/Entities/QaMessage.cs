namespace ProQuote.Domain.Entities;

/// <summary>
/// Represents a Q&amp;A message in an RFQ thread between buyers and suppliers.
/// </summary>
/// <remarks>
/// Messages can be public (visible to all invited suppliers) or private
/// (visible only to a specific supplier). This enables clarifications
/// during the quotation period.
/// </remarks>
public class QaMessage : BaseEntity
{
    #region Properties

    /// <summary>
    /// Gets or sets the identifier of the RFQ this message belongs to.
    /// </summary>
    public Guid RfqId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who sent the message.
    /// </summary>
    public Guid SenderId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the target supplier for private messages.
    /// Null means the message is visible to all invited suppliers.
    /// </summary>
    public Guid? TargetSupplierId { get; set; }

    /// <summary>
    /// Gets or sets the message content.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the UTC timestamp when the message was sent.
    /// </summary>
    public DateTime SentAt { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the message was sent by the buyer.
    /// </summary>
    public bool IsFromBuyer { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the parent message if this is a reply.
    /// </summary>
    public Guid? ParentMessageId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the message has been read.
    /// </summary>
    public bool IsRead { get; set; }

    #endregion

    #region Navigation Properties

    /// <summary>
    /// Gets or sets the RFQ this message belongs to.
    /// </summary>
    public virtual Rfq Rfq { get; set; } = null!;

    /// <summary>
    /// Gets or sets the user who sent the message.
    /// </summary>
    public virtual ApplicationUser Sender { get; set; } = null!;

    /// <summary>
    /// Gets or sets the target supplier for private messages.
    /// </summary>
    public virtual Supplier? TargetSupplier { get; set; }

    /// <summary>
    /// Gets or sets the parent message if this is a reply.
    /// </summary>
    public virtual QaMessage? ParentMessage { get; set; }

    /// <summary>
    /// Gets or sets the collection of replies to this message.
    /// </summary>
    public virtual ICollection<QaMessage> Replies { get; set; } = new List<QaMessage>();

    #endregion

    #region Methods

    /// <summary>
    /// Determines whether the message is private (targeted to a specific supplier).
    /// </summary>
    /// <returns><c>true</c> if the message has a target supplier; otherwise, <c>false</c>.</returns>
    public bool IsPrivate()
    {
        return TargetSupplierId.HasValue;
    }

    /// <summary>
    /// Determines whether a supplier can view this message.
    /// </summary>
    /// <param name="supplierId">The supplier identifier to check.</param>
    /// <returns><c>true</c> if the message is public or targeted to the supplier; otherwise, <c>false</c>.</returns>
    public bool CanBeViewedBySupplier(Guid supplierId)
    {
        return !TargetSupplierId.HasValue || TargetSupplierId == supplierId;
    }

    #endregion
}
