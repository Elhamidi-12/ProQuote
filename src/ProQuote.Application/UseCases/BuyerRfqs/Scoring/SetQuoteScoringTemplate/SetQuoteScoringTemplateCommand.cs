namespace ProQuote.Application.UseCases.BuyerRfqs.Scoring.SetQuoteScoringTemplate;

/// <summary>
/// Command payload for saving quote scoring template for an RFQ.
/// </summary>
/// <param name="BuyerUserId">Buyer user identifier.</param>
/// <param name="RfqId">RFQ identifier.</param>
/// <param name="PriceWeight">Price weight (0-100).</param>
/// <param name="LeadTimeWeight">Lead-time weight (0-100).</param>
/// <param name="CoverageWeight">Coverage weight (0-100).</param>
public sealed record SetQuoteScoringTemplateCommand(
    Guid BuyerUserId,
    Guid RfqId,
    decimal PriceWeight,
    decimal LeadTimeWeight,
    decimal CoverageWeight);
