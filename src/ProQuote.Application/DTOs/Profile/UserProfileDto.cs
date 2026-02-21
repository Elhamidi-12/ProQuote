using ProQuote.Domain.Enums;

namespace ProQuote.Application.DTOs.Profile;

/// <summary>
/// Data transfer object for the authenticated user's profile.
/// </summary>
public class UserProfileDto
{
    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    public Guid UserId { get; set; }

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
    /// Gets or sets the phone number.
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Gets or sets the user's job title.
    /// </summary>
    public string? JobTitle { get; set; }

    /// <summary>
    /// Gets or sets the user's department.
    /// </summary>
    public string? Department { get; set; }

    /// <summary>
    /// Gets or sets the user's profile picture URL.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1056:URI-like properties should not be strings", Justification = "Stored as string URL/path for transport compatibility.")]
    public string? ProfilePictureUrl { get; set; }

    /// <summary>
    /// Gets or sets the preferred timezone identifier.
    /// </summary>
    public string? TimeZoneId { get; set; }

    /// <summary>
    /// Gets or sets the preferred language/locale.
    /// </summary>
    public string? Locale { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this user has a supplier profile.
    /// </summary>
    public bool IsSupplier { get; set; }

    /// <summary>
    /// Gets or sets the supplier profile details when user is supplier.
    /// </summary>
    public SupplierProfileDto? Supplier { get; set; }
}

/// <summary>
/// Supplier profile details for the authenticated supplier user.
/// </summary>
public class SupplierProfileDto
{
    /// <summary>
    /// Gets or sets the supplier identifier.
    /// </summary>
    public Guid SupplierId { get; set; }

    /// <summary>
    /// Gets or sets the company name.
    /// </summary>
    public string CompanyName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the primary contact name.
    /// </summary>
    public string ContactName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the supplier profile email.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the supplier phone number.
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Gets or sets the company website URL.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1056:URI-like properties should not be strings", Justification = "Stored as string URL/path for transport compatibility.")]
    public string? Website { get; set; }

    /// <summary>
    /// Gets or sets the company address.
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Gets or sets the city.
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// Gets or sets the country.
    /// </summary>
    public string? Country { get; set; }

    /// <summary>
    /// Gets or sets the tax ID.
    /// </summary>
    public string? TaxId { get; set; }

    /// <summary>
    /// Gets or sets the supplier status.
    /// </summary>
    public SupplierStatus Status { get; set; }
}
