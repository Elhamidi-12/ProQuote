using Microsoft.AspNetCore.Identity;

using ProQuote.Domain.Entities;

namespace ProQuote.Infrastructure.Identity;

/// <summary>
/// Represents the application user for ASP.NET Core Identity.
/// </summary>
/// <remarks>
/// This class extends IdentityUser with additional properties from the domain layer.
/// It serves as the bridge between ASP.NET Core Identity and our domain model.
/// </remarks>
public class ApplicationUserIdentity : IdentityUser<Guid>
{
    #region Properties

    /// <summary>
    /// Gets or sets the user's first name.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's last name.
    /// </summary>
    public string LastName { get; set; } = string.Empty;

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

    /// <summary>
    /// Gets or sets the collection of notifications for this user.
    /// </summary>
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    /// <summary>
    /// Gets or sets the collection of RFQ documents uploaded by this user.
    /// </summary>
    public virtual ICollection<RfqDocument> UploadedDocuments { get; set; } = new List<RfqDocument>();

    #endregion

    #region Methods

    /// <summary>
    /// Converts this identity user to a domain ApplicationUser.
    /// </summary>
    /// <returns>A domain ApplicationUser instance.</returns>
    public ApplicationUser ToDomainUser()
    {
        return new ApplicationUser
        {
            Id = Id,
            Email = Email ?? string.Empty,
            FirstName = FirstName,
            LastName = LastName,
            PhoneNumber = PhoneNumber,
            JobTitle = JobTitle,
            Department = Department,
            ProfilePictureUrl = ProfilePictureUrl,
            IsActive = IsActive,
            CreatedAt = CreatedAt,
            LastLoginAt = LastLoginAt,
            TimeZoneId = TimeZoneId,
            Locale = Locale
        };
    }

    #endregion
}
