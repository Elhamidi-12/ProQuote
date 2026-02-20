using Microsoft.AspNetCore.Identity;

namespace RFQApp.Infrastructure.Identity;

/// <summary>
/// Represents an application role for ASP.NET Core Identity.
/// </summary>
/// <remarks>
/// This class extends IdentityRole with additional properties for role management.
/// The three main roles are: Admin, Buyer, and Supplier.
/// </remarks>
public class ApplicationRoleIdentity : IdentityRole<Guid>
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationRoleIdentity"/> class.
    /// </summary>
    public ApplicationRoleIdentity() : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationRoleIdentity"/> class.
    /// </summary>
    /// <param name="roleName">The role name.</param>
    public ApplicationRoleIdentity(string roleName) : base(roleName)
    {
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the description of the role.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when the role was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this is a system role that cannot be deleted.
    /// </summary>
    public bool IsSystemRole { get; set; }

    #endregion
}
