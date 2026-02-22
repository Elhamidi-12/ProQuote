namespace ProQuote.Application.UseCases.AdminSuppliers.ReviewSupplier;

/// <summary>
/// Command payload for approving or rejecting supplier registration.
/// </summary>
/// <param name="AdminUserId">Current admin user identifier.</param>
/// <param name="SupplierId">Supplier identifier.</param>
/// <param name="Approve">True to approve; false to reject.</param>
/// <param name="Reason">Optional rejection reason.</param>
public sealed record ReviewSupplierRegistrationCommand(
    Guid AdminUserId,
    Guid SupplierId,
    bool Approve,
    string? Reason = null);
