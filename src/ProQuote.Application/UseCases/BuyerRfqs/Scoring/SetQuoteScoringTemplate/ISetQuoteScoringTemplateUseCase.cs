namespace ProQuote.Application.UseCases.BuyerRfqs.Scoring.SetQuoteScoringTemplate;

/// <summary>
/// Application use-case contract for setting RFQ quote scoring template.
/// </summary>
public interface ISetQuoteScoringTemplateUseCase
{
    /// <summary>
    /// Executes save-template workflow.
    /// </summary>
    /// <param name="command">Save-template command payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Save-template response.</returns>
    Task<SetQuoteScoringTemplateResponse> ExecuteAsync(
        SetQuoteScoringTemplateCommand command,
        CancellationToken cancellationToken = default);
}
