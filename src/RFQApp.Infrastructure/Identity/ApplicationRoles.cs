namespace RFQApp.Infrastructure.Identity;

/// <summary>
/// Contains constants for application role names.
/// </summary>
public static class ApplicationRoles
{
    #region Role Names

    /// <summary>
    /// The admin role name. Full platform access and management.
    /// </summary>
    public const string Admin = "Admin";

    /// <summary>
    /// The buyer role name. Can create RFQs, invite suppliers, and award contracts.
    /// </summary>
    public const string Buyer = "Buyer";

    /// <summary>
    /// The supplier role name. Can view RFQ invitations and submit quotes.
    /// </summary>
    public const string Supplier = "Supplier";

    #endregion

    #region Role Combinations

    /// <summary>
    /// Admin or Buyer roles (for RFQ management access).
    /// </summary>
    public const string AdminOrBuyer = $"{Admin},{Buyer}";

    /// <summary>
    /// All roles.
    /// </summary>
    public const string All = $"{Admin},{Buyer},{Supplier}";

    #endregion

    #region Methods

    /// <summary>
    /// Gets all role names as an array.
    /// </summary>
    /// <returns>An array containing all role names.</returns>
    public static string[] GetAllRoles()
    {
        return [Admin, Buyer, Supplier];
    }

    /// <summary>
    /// Determines whether the specified role name is valid.
    /// </summary>
    /// <param name="roleName">The role name to validate.</param>
    /// <returns><c>true</c> if the role name is valid; otherwise, <c>false</c>.</returns>
    public static bool IsValidRole(string roleName)
    {
        return roleName == Admin || roleName == Buyer || roleName == Supplier;
    }

    #endregion
}
