namespace ProQuote.Application.UseCases.AdminSuppliers.ReviewSupplier;

/// <summary>
/// Application use-case contract for supplier registration review.
/// </summary>
public interface IReviewSupplierRegistrationUseCase
{
    /// <summary>
    /// Executes approve/reject workflow for supplier registration.
    /// </summary>
    /// <param name="command">Review command payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Review response.</returns>
    Task<ReviewSupplierRegistrationResponse> ExecuteAsync(
        ReviewSupplierRegistrationCommand command,
        CancellationToken cancellationToken = default);
}
