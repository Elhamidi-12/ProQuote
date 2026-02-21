namespace ProQuote.Domain.Entities;

/// <summary>
/// Represents an application user with extended profile information.
/// </summary>
/// <remarks>
/// This entity extends the ASP.NET Core Identity user with additional
/// properties specific to the ProQuote.
/// The actual IdentityUser inheritance is in the Infrastructure layer.
/// </remarks>
public class ApplicationUser
{
    #region Properties

    /// <summary>
    /// Gets or sets the unique identifier for the user.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the user's email address.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's first name.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's last name.
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's phone number.
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
    /// Gets or sets the URL to the user's profile picture.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1056:URI-like properties should not be strings", Justification = "Persisted as string path/URL for storage and transport compatibility.")]
    public string? ProfilePictureUrl { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the user account is active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the UTC timestamp when the user was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp of the user's last login.
    /// </summary>
    public DateTime? LastLoginAt { get; set; }

    /// <summary>
    /// Gets or sets the user's preferred timezone identifier.
    /// </summary>
    public string? TimeZoneId { get; set; }

    /// <summary>
    /// Gets or sets the user's preferred language/locale.
    /// </summary>
    public string? Locale { get; set; }

    #endregion

    #region Computed Properties

    /// <summary>
    /// Gets the user's full name.
    /// </summary>
    public string FullName => $"{FirstName} {LastName}".Trim();

    /// <summary>
    /// Gets the user's initials.
    /// </summary>
    public string Initials
    {
        get
        {
            string firstInitial = !string.IsNullOrEmpty(FirstName) ? FirstName[0].ToString() : string.Empty;
            string lastInitial = !string.IsNullOrEmpty(LastName) ? LastName[0].ToString() : string.Empty;
            return $"{firstInitial}{lastInitial}".ToUpperInvariant();
        }
    }

    #endregion

    #region Navigation Properties

    /// <summary>
    /// Gets or sets the supplier profile if the user is a supplier.
    /// </summary>
    public virtual Supplier? Supplier { get; set; }

    /// <summary>
    /// Gets or sets the collection of RFQs created by this user (if buyer).
    /// </summary>
    public virtual ICollection<Rfq> CreatedRfqs { get; set; } = new List<Rfq>();

    /// <summary>
    /// Gets or sets the collection of audit logs for actions performed by this user.
    /// </summary>
    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    /// <summary>
    /// Gets or sets the collection of Q&amp;A messages sent by this user.
    /// </summary>
    public virtual ICollection<QaMessage> QaMessages { get; set; } = new List<QaMessage>();

    /// <summary>
    /// Gets or sets the collection of refresh tokens for this user.
    /// </summary>
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    #endregion
}
