using RFQApp.Domain.Enums;

namespace RFQApp.Domain.Entities;

/// <summary>
/// Represents a supplier's quote submission for an RFQ.
/// </summary>
/// <remarks>
/// A quote contains the supplier's proposed prices for each line item,
/// along with lead time, validity period, and other commercial terms.
/// </remarks>
public class Quote : BaseEntity
{
    #region Properties

    /// <summary>
    /// Gets or sets the identifier of the RFQ this quote is for.
    /// </summary>
    public Guid RfqId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the supplier submitting this quote.
    /// </summary>
    public Guid SupplierId { get; set; }

    /// <summary>
    /// Gets or sets the current status of the quote.
    /// </summary>
    public QuoteStatus Status { get; set; } = QuoteStatus.Submitted;

    /// <summary>
    /// Gets or sets the lead time in days for delivery.
    /// </summary>
    public int LeadTimeDays { get; set; }

    /// <summary>
    /// Gets or sets the date until which this quote is valid (UTC).
    /// </summary>
    public DateTime ValidUntil { get; set; }

    /// <summary>
    /// Gets or sets the payment terms offered by the supplier.
    /// </summary>
    public string? PaymentTerms { get; set; }

    /// <summary>
    /// Gets or sets additional notes from the supplier.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets the total amount of the quote (sum of all line item totals).
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this quote has been awarded.
    /// </summary>
    public bool IsAwarded { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when the quote was submitted.
    /// </summary>
    public DateTime SubmittedAt { get; set; }

    /// <summary>
    /// Gets or sets the reason for withdrawal if the quote was withdrawn.
    /// </summary>
    public string? WithdrawalReason { get; set; }

    /// <summary>
    /// Gets or sets private notes from the buyer about this quote.
    /// </summary>
    public string? BuyerNotes { get; set; }

    #endregion

    #region Navigation Properties

    /// <summary>
    /// Gets or sets the RFQ this quote is for.
    /// </summary>
    public virtual Rfq Rfq { get; set; } = null!;

    /// <summary>
    /// Gets or sets the supplier who submitted this quote.
    /// </summary>
    public virtual Supplier Supplier { get; set; } = null!;

    /// <summary>
    /// Gets or sets the collection of line item prices in this quote.
    /// </summary>
    public virtual ICollection<QuoteLineItem> LineItems { get; set; } = new List<QuoteLineItem>();

    /// <summary>
    /// Gets or sets the collection of documents attached to this quote.
    /// </summary>
    public virtual ICollection<QuoteDocument> Documents { get; set; } = new List<QuoteDocument>();

    #endregion

    #region Methods

    /// <summary>
    /// Determines whether the quote can be edited by the supplier.
    /// </summary>
    /// <returns><c>true</c> if the quote is submitted and the RFQ deadline has not passed; otherwise, <c>false</c>.</returns>
    public bool CanBeEdited()
    {
        return Status == QuoteStatus.Submitted && DateTime.UtcNow < Rfq?.SubmissionDeadline;
    }

    /// <summary>
    /// Determines whether the quote can be withdrawn by the supplier.
    /// </summary>
    /// <returns><c>true</c> if the quote is submitted and not yet awarded; otherwise, <c>false</c>.</returns>
    public bool CanBeWithdrawn()
    {
        return Status == QuoteStatus.Submitted && !IsAwarded;
    }

    /// <summary>
    /// Determines whether the quote is still valid based on validity date.
    /// </summary>
    /// <returns><c>true</c> if the current date is before the validity date; otherwise, <c>false</c>.</returns>
    public bool IsValid()
    {
        return DateTime.UtcNow < ValidUntil;
    }

    /// <summary>
    /// Calculates and updates the total amount based on line items.
    /// </summary>
    public void CalculateTotalAmount()
    {
        TotalAmount = LineItems.Sum(lineItem => lineItem.TotalPrice);
    }

    #endregion
}
