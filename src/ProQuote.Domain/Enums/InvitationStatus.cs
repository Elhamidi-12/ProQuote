namespace ProQuote.Domain.Enums;

/// <summary>
/// Represents the status of an RFQ invitation sent to a supplier.
/// </summary>
public enum InvitationStatus
{
    /// <summary>
    /// Invitation email has been sent to the supplier.
    /// </summary>
    Sent = 0,

    /// <summary>
    /// Supplier has viewed the RFQ details.
    /// </summary>
    Viewed = 1,

    /// <summary>
    /// Supplier has submitted a quote for this RFQ.
    /// </summary>
    Quoted = 2,

    /// <summary>
    /// Supplier has declined to participate in this RFQ.
    /// </summary>
    Declined = 3
}
