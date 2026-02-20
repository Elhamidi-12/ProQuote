namespace RFQApp.Domain.Enums;

/// <summary>
/// Represents the lifecycle status of an RFQ.
/// </summary>
/// <remarks>
/// RFQ Status Flow: Draft → Published → QuotesReceived → UnderEvaluation → Awarded → Closed
/// Additional terminal states: Cancelled, Expired
/// </remarks>
public enum RfqStatus
{
    /// <summary>
    /// RFQ is being prepared by the buyer, not visible to suppliers.
    /// </summary>
    Draft = 0,

    /// <summary>
    /// RFQ has been sent to selected suppliers, deadline countdown has started.
    /// </summary>
    Published = 1,

    /// <summary>
    /// At least one supplier has submitted a quote.
    /// </summary>
    QuotesReceived = 2,

    /// <summary>
    /// Buyer is actively comparing quotes, no new submissions allowed.
    /// </summary>
    UnderEvaluation = 3,

    /// <summary>
    /// Buyer has selected a winning supplier.
    /// </summary>
    Awarded = 4,

    /// <summary>
    /// RFQ process is complete, all parties have been notified.
    /// </summary>
    Closed = 5,

    /// <summary>
    /// RFQ was cancelled by the buyer or admin.
    /// </summary>
    Cancelled = 6,

    /// <summary>
    /// Submission deadline passed with no quotes received.
    /// </summary>
    Expired = 7
}
