using ProQuote.Application.DTOs.Quotes;
using ProQuote.Application.Interfaces;

namespace ProQuote.Application.UseCases.BuyerRfqs.Scoring.SetQuoteScoringTemplate;

/// <summary>
/// Application use-case implementation for setting RFQ quote scoring template.
/// </summary>
public sealed class SetQuoteScoringTemplateUseCase : ISetQuoteScoringTemplateUseCase
{
    private readonly IQuoteScoringTemplateService _quoteScoringTemplateService;

    /// <summary>
    /// Initializes a new instance of the <see cref="SetQuoteScoringTemplateUseCase"/> class.
    /// </summary>
    /// <param name="quoteScoringTemplateService">Quote scoring template service.</param>
    public SetQuoteScoringTemplateUseCase(IQuoteScoringTemplateService quoteScoringTemplateService)
    {
        _quoteScoringTemplateService = quoteScoringTemplateService;
    }

    /// <inheritdoc />
    public async Task<SetQuoteScoringTemplateResponse> ExecuteAsync(
        SetQuoteScoringTemplateCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        cancellationToken.ThrowIfCancellationRequested();

        if (command.BuyerUserId == Guid.Empty || command.RfqId == Guid.Empty)
        {
            return SetQuoteScoringTemplateResponse.Failure(command.RfqId, "Invalid scoring template request.");
        }

        try
        {
            QuoteScoringTemplateDto savedTemplate = await _quoteScoringTemplateService.SaveTemplateAsync(
                command.BuyerUserId,
                command.RfqId,
                new QuoteScoringTemplateDto
                {
                    PriceWeight = command.PriceWeight,
                    LeadTimeWeight = command.LeadTimeWeight,
                    CoverageWeight = command.CoverageWeight
                });

            return SetQuoteScoringTemplateResponse.Success(command.RfqId, savedTemplate);
        }
        catch (InvalidOperationException ex)
        {
            return SetQuoteScoringTemplateResponse.Failure(command.RfqId, ex.Message);
        }
    }
}
