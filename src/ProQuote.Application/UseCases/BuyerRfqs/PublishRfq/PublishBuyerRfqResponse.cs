namespace ProQuote.Application.UseCases.BuyerRfqs.PublishRfq;

/// <summary>
/// Result payload for RFQ publish workflow.
/// </summary>
public sealed class PublishBuyerRfqResponse
{
    /// <summary>
    /// Gets or sets a value indicating whether publish succeeded.
    /// </summary>
    public bool Succeeded { get; set; }

    /// <summary>
    /// Gets or sets RFQ identifier.
    /// </summary>
    public Guid RfqId { get; set; }

    /// <summary>
    /// Gets or sets publish timestamp when successful.
    /// </summary>
    public DateTime? PublishedAt { get; set; }

    /// <summary>
    /// Gets or sets optional error message when publish fails.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Creates successful publish response.
    /// </summary>
    /// <param name="rfqId">RFQ identifier.</param>
    /// <param name="publishedAt">Published timestamp.</param>
    /// <returns>Success response.</returns>
    public static PublishBuyerRfqResponse Success(Guid rfqId, DateTime? publishedAt)
    {
        return new PublishBuyerRfqResponse
        {
            Succeeded = true,
            RfqId = rfqId,
            PublishedAt = publishedAt
        };
    }

    /// <summary>
    /// Creates failed publish response.
    /// </summary>
    /// <param name="rfqId">RFQ identifier.</param>
    /// <param name="errorMessage">Failure reason.</param>
    /// <returns>Failure response.</returns>
    public static PublishBuyerRfqResponse Failure(Guid rfqId, string errorMessage)
    {
        return new PublishBuyerRfqResponse
        {
            Succeeded = false,
            RfqId = rfqId,
            ErrorMessage = errorMessage
        };
    }
}
