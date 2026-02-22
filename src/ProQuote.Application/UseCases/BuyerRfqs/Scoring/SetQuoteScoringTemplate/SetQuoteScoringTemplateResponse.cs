using ProQuote.Application.DTOs.Quotes;

namespace ProQuote.Application.UseCases.BuyerRfqs.Scoring.SetQuoteScoringTemplate;

/// <summary>
/// Result payload for setting quote scoring template.
/// </summary>
public sealed class SetQuoteScoringTemplateResponse
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
    /// Gets or sets saved scoring template.
    /// </summary>
    public QuoteScoringTemplateDto Template { get; set; } = new();

    /// <summary>
    /// Gets or sets optional error message when operation fails.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Creates successful response.
    /// </summary>
    /// <param name="rfqId">RFQ identifier.</param>
    /// <param name="template">Saved template.</param>
    /// <returns>Success response.</returns>
    public static SetQuoteScoringTemplateResponse Success(Guid rfqId, QuoteScoringTemplateDto template)
    {
        return new SetQuoteScoringTemplateResponse
        {
            Succeeded = true,
            RfqId = rfqId,
            Template = template
        };
    }

    /// <summary>
    /// Creates failed response.
    /// </summary>
    /// <param name="rfqId">RFQ identifier.</param>
    /// <param name="errorMessage">Failure message.</param>
    /// <returns>Failure response.</returns>
    public static SetQuoteScoringTemplateResponse Failure(Guid rfqId, string errorMessage)
    {
        return new SetQuoteScoringTemplateResponse
        {
            Succeeded = false,
            RfqId = rfqId,
            ErrorMessage = errorMessage
        };
    }
}
