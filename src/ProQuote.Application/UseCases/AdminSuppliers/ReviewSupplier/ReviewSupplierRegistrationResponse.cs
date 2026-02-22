using ProQuote.Domain.Enums;

namespace ProQuote.Application.UseCases.AdminSuppliers.ReviewSupplier;

/// <summary>
/// Result payload for supplier registration review workflow.
/// </summary>
public sealed class ReviewSupplierRegistrationResponse
{
    /// <summary>
    /// Gets or sets a value indicating whether workflow succeeded.
    /// </summary>
    public bool Succeeded { get; set; }

    /// <summary>
    /// Gets or sets supplier identifier.
    /// </summary>
    public Guid SupplierId { get; set; }

    /// <summary>
    /// Gets or sets resulting supplier status when successful.
    /// </summary>
    public SupplierStatus? Status { get; set; }

    /// <summary>
    /// Gets or sets optional error message when operation fails.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Creates successful review response.
    /// </summary>
    /// <param name="supplierId">Supplier identifier.</param>
    /// <param name="status">Updated status.</param>
    /// <returns>Success response.</returns>
    public static ReviewSupplierRegistrationResponse Success(Guid supplierId, SupplierStatus status)
    {
        return new ReviewSupplierRegistrationResponse
        {
            Succeeded = true,
            SupplierId = supplierId,
            Status = status
        };
    }

    /// <summary>
    /// Creates failed review response.
    /// </summary>
    /// <param name="supplierId">Supplier identifier.</param>
    /// <param name="errorMessage">Failure reason.</param>
    /// <returns>Failure response.</returns>
    public static ReviewSupplierRegistrationResponse Failure(Guid supplierId, string errorMessage)
    {
        return new ReviewSupplierRegistrationResponse
        {
            Succeeded = false,
            SupplierId = supplierId,
            ErrorMessage = errorMessage
        };
    }
}
