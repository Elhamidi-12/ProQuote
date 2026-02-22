namespace ProQuote.Application.UseCases.AdminRfqs.CloseRfq;

/// <summary>
/// Result payload for admin RFQ close workflow.
/// </summary>
public sealed class CloseAdminRfqResponse
{
    /// <summary>
    /// Gets or sets a value indicating whether close succeeded.
    /// </summary>
    public bool Succeeded { get; set; }

    /// <summary>
    /// Gets or sets RFQ identifier.
    /// </summary>
    public Guid RfqId { get; set; }

    /// <summary>
    /// Gets or sets close timestamp when successful.
    /// </summary>
    public DateTime? ClosedAt { get; set; }

    /// <summary>
    /// Gets or sets optional error message when close fails.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Creates successful close response.
    /// </summary>
    /// <param name="rfqId">RFQ identifier.</param>
    /// <param name="closedAt">Closed timestamp.</param>
    /// <returns>Success response.</returns>
    public static CloseAdminRfqResponse Success(Guid rfqId, DateTime? closedAt)
    {
        return new CloseAdminRfqResponse
        {
            Succeeded = true,
            RfqId = rfqId,
            ClosedAt = closedAt
        };
    }

    /// <summary>
    /// Creates failed close response.
    /// </summary>
    /// <param name="rfqId">RFQ identifier.</param>
    /// <param name="errorMessage">Failure reason.</param>
    /// <returns>Failure response.</returns>
    public static CloseAdminRfqResponse Failure(Guid rfqId, string errorMessage)
    {
        return new CloseAdminRfqResponse
        {
            Succeeded = false,
            RfqId = rfqId,
            ErrorMessage = errorMessage
        };
    }
}
