namespace RFQApp.Domain.Enums;

/// <summary>
/// Represents the status of a supplier's quote submission.
/// </summary>
public enum QuoteStatus
{
    /// <summary>
    /// Quote has been submitted by the supplier.
    /// </summary>
    Submitted = 0,

    /// <summary>
    /// Quote is currently being reviewed by the buyer.
    /// </summary>
    UnderReview = 1,

    /// <summary>
    /// Quote has been selected as the winning bid.
    /// </summary>
    Awarded = 2,

    /// <summary>
    /// Quote was not selected for the award.
    /// </summary>
    Rejected = 3,

    /// <summary>
    /// Supplier has withdrawn their quote.
    /// </summary>
    Withdrawn = 4
}
