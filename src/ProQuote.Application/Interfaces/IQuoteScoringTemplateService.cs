using ProQuote.Application.DTOs.Quotes;

namespace ProQuote.Application.Interfaces;

/// <summary>
/// Service contract for RFQ quote scoring template management.
/// </summary>
public interface IQuoteScoringTemplateService
{
    /// <summary>
    /// Gets scoring template for a buyer RFQ.
    /// </summary>
    /// <param name="buyerUserId">Buyer user identifier.</param>
    /// <param name="rfqId">RFQ identifier.</param>
    /// <returns>Scoring template.</returns>
    public Task<QuoteScoringTemplateDto> GetTemplateAsync(Guid buyerUserId, Guid rfqId);

    /// <summary>
    /// Saves scoring template for a buyer RFQ.
    /// </summary>
    /// <param name="buyerUserId">Buyer user identifier.</param>
    /// <param name="rfqId">RFQ identifier.</param>
    /// <param name="scoringTemplate">Template payload.</param>
    /// <returns>Saved template.</returns>
    public Task<QuoteScoringTemplateDto> SaveTemplateAsync(Guid buyerUserId, Guid rfqId, QuoteScoringTemplateDto scoringTemplate);
}
