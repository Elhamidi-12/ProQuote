namespace ProQuote.Application.UseCases.AdminRfqs.CancelRfq;

/// <summary>
/// Result payload for admin RFQ cancel workflow.
/// </summary>
public sealed class CancelAdminRfqResponse
{
    /// <summary>
    /// Gets or sets a value indicating whether cancel succeeded.
    /// </summary>
    public bool Succeeded { get; set; }

    /// <summary>
    /// Gets or sets RFQ identifier.
    /// </summary>
    public Guid RfqId { get; set; }

    /// <summary>
    /// Gets or sets cancellation reason when successful.
    /// </summary>
    public string? CancellationReason { get; set; }

    /// <summary>
    /// Gets or sets optional error message when cancel fails.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Creates successful cancel response.
    /// </summary>
    /// <param name="rfqId">RFQ identifier.</param>
    /// <param name="cancellationReason">Cancellation reason.</param>
    /// <returns>Success response.</returns>
    public static CancelAdminRfqResponse Success(Guid rfqId, string cancellationReason)
    {
        return new CancelAdminRfqResponse
        {
            Succeeded = true,
            RfqId = rfqId,
            CancellationReason = cancellationReason
        };
    }

    /// <summary>
    /// Creates failed cancel response.
    /// </summary>
    /// <param name="rfqId">RFQ identifier.</param>
    /// <param name="errorMessage">Failure reason.</param>
    /// <returns>Failure response.</returns>
    public static CancelAdminRfqResponse Failure(Guid rfqId, string errorMessage)
    {
        return new CancelAdminRfqResponse
        {
            Succeeded = false,
            RfqId = rfqId,
            ErrorMessage = errorMessage
        };
    }
}
