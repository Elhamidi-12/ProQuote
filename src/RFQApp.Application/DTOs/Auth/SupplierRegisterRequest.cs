using System.ComponentModel.DataAnnotations;

namespace RFQApp.Application.DTOs.Auth;

/// <summary>
/// Request model for supplier registration.
/// </summary>
public class SupplierRegisterRequest : RegisterRequest
{
    /// <summary>
    /// Gets or sets the company name.
    /// </summary>
    [Required(ErrorMessage = "Company name is required")]
    [MaxLength(200, ErrorMessage = "Company name cannot exceed 200 characters")]
    public string CompanyName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the contact person name.
    /// </summary>
    [Required(ErrorMessage = "Contact name is required")]
    [MaxLength(100, ErrorMessage = "Contact name cannot exceed 100 characters")]
    public string ContactName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the company website.
    /// </summary>
    [Url(ErrorMessage = "Invalid website URL")]
    [MaxLength(500, ErrorMessage = "Website URL cannot exceed 500 characters")]
    public string? Website { get; set; }

    /// <summary>
    /// Gets or sets the company address.
    /// </summary>
    [MaxLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
    public string? Address { get; set; }

    /// <summary>
    /// Gets or sets the city.
    /// </summary>
    [MaxLength(100, ErrorMessage = "City cannot exceed 100 characters")]
    public string? City { get; set; }

    /// <summary>
    /// Gets or sets the country.
    /// </summary>
    [MaxLength(100, ErrorMessage = "Country cannot exceed 100 characters")]
    public string? Country { get; set; }

    /// <summary>
    /// Gets or sets the tax identification number.
    /// </summary>
    [MaxLength(50, ErrorMessage = "Tax ID cannot exceed 50 characters")]
    public string? TaxId { get; set; }

    /// <summary>
    /// Gets or sets the selected category IDs.
    /// </summary>
    public List<Guid> CategoryIds { get; set; } = [];
}
