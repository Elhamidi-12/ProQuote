namespace ProQuote.Application.UseCases.AdminRfqs.MoveToEvaluation;

/// <summary>
/// Result payload for admin RFQ move-to-evaluation workflow.
/// </summary>
public sealed class MoveAdminRfqToEvaluationResponse
{
    /// <summary>
    /// Gets or sets a value indicating whether operation succeeded.
    /// </summary>
    public bool Succeeded { get; set; }

    /// <summary>
    /// Gets or sets RFQ identifier.
    /// </summary>
    public Guid RfqId { get; set; }

    /// <summary>
    /// Gets or sets optional error message when operation fails.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Creates successful response.
    /// </summary>
    /// <param name="rfqId">RFQ identifier.</param>
    /// <returns>Success response.</returns>
    public static MoveAdminRfqToEvaluationResponse Success(Guid rfqId)
    {
        return new MoveAdminRfqToEvaluationResponse
        {
            Succeeded = true,
            RfqId = rfqId
        };
    }

    /// <summary>
    /// Creates failed response.
    /// </summary>
    /// <param name="rfqId">RFQ identifier.</param>
    /// <param name="errorMessage">Failure reason.</param>
    /// <returns>Failure response.</returns>
    public static MoveAdminRfqToEvaluationResponse Failure(Guid rfqId, string errorMessage)
    {
        return new MoveAdminRfqToEvaluationResponse
        {
            Succeeded = false,
            RfqId = rfqId,
            ErrorMessage = errorMessage
        };
    }
}
