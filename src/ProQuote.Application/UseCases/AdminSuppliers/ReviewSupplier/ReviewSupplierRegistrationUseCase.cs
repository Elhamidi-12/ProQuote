using ProQuote.Application.Interfaces;
using ProQuote.Domain.Enums;

namespace ProQuote.Application.UseCases.AdminSuppliers.ReviewSupplier;

/// <summary>
/// Application use-case implementation for supplier registration review.
/// </summary>
public sealed class ReviewSupplierRegistrationUseCase : IReviewSupplierRegistrationUseCase
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReviewSupplierRegistrationUseCase"/> class.
    /// </summary>
    /// <param name="unitOfWork">Unit of work.</param>
    public ReviewSupplierRegistrationUseCase(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<ReviewSupplierRegistrationResponse> ExecuteAsync(
        ReviewSupplierRegistrationCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (command.AdminUserId == Guid.Empty || command.SupplierId == Guid.Empty)
        {
            return ReviewSupplierRegistrationResponse.Failure(command.SupplierId, "Invalid supplier review request.");
        }

        Domain.Entities.Supplier? supplier = await _unitOfWork.Suppliers.GetByIdAsync(command.SupplierId, cancellationToken);
        if (supplier == null)
        {
            return ReviewSupplierRegistrationResponse.Failure(command.SupplierId, "Supplier not found.");
        }

        if (supplier.Status != SupplierStatus.Pending)
        {
            return ReviewSupplierRegistrationResponse.Failure(command.SupplierId, "Only pending suppliers can be reviewed.");
        }

        if (command.Approve)
        {
            supplier.Status = SupplierStatus.Approved;
            supplier.ApprovedAt = DateTime.UtcNow;
            supplier.StatusReason = null;
        }
        else
        {
            supplier.Status = SupplierStatus.Rejected;
            supplier.ApprovedAt = null;
            supplier.StatusReason = string.IsNullOrWhiteSpace(command.Reason)
                ? "Rejected by admin."
                : command.Reason.Trim();
        }

        _unitOfWork.Suppliers.Update(supplier);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ReviewSupplierRegistrationResponse.Success(supplier.Id, supplier.Status);
    }
}
