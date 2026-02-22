namespace ProQuote.Application.UseCases.AdminRfqs.MoveToEvaluation;

/// <summary>
/// Application use-case contract for moving RFQs to under evaluation as admin.
/// </summary>
public interface IMoveAdminRfqToEvaluationUseCase
{
    /// <summary>
    /// Executes move-to-evaluation workflow.
    /// </summary>
    /// <param name="command">Move command payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Move response.</returns>
    Task<MoveAdminRfqToEvaluationResponse> ExecuteAsync(
        MoveAdminRfqToEvaluationCommand command,
        CancellationToken cancellationToken = default);
}
