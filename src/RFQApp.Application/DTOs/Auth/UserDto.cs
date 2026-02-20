namespace RFQApp.Application.DTOs.Auth;

/// <summary>
/// Data transfer object for user information.
/// </summary>
public class UserDto
{
    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the email address.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the first name.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the last name.
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the full name.
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the phone number.
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Gets or sets the profile picture URL.
    /// </summary>
    public string? ProfilePictureUrl { get; set; }

    /// <summary>
    /// Gets or sets the user's roles.
    /// </summary>
    public List<string> Roles { get; set; } = [];

    /// <summary>
    /// Gets or sets the supplier identifier if the user is a supplier.
    /// </summary>
    public Guid? SupplierId { get; set; }

    /// <summary>
    /// Gets or sets the company name if the user is a supplier.
    /// </summary>
    public string? CompanyName { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the user is active.
    /// </summary>
    public bool IsActive { get; set; }
}
