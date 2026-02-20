namespace RFQApp.Domain.Enums;

/// <summary>
/// Represents the registration and approval status of a supplier.
/// </summary>
public enum SupplierStatus
{
    /// <summary>
    /// Supplier registration is pending admin approval.
    /// </summary>
    Pending = 0,

    /// <summary>
    /// Supplier has been approved and can participate in RFQs.
    /// </summary>
    Approved = 1,

    /// <summary>
    /// Supplier registration was rejected by admin.
    /// </summary>
    Rejected = 2,

    /// <summary>
    /// Supplier account has been suspended by admin.
    /// </summary>
    Suspended = 3
}
