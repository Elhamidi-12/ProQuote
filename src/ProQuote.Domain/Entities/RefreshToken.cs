namespace ProQuote.Domain.Entities;

/// <summary>
/// Represents a refresh token for JWT authentication.
/// </summary>
/// <remarks>
/// Refresh tokens allow users to obtain new access tokens without
/// re-authenticating, improving security and user experience.
/// </remarks>
public class RefreshToken : BaseEntity
{
    #region Properties

    /// <summary>
    /// Gets or sets the identifier of the user this token belongs to.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the refresh token value.
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the JWT ID that this refresh token is associated with.
    /// </summary>
    public string JwtId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the token has been used.
    /// </summary>
    public bool IsUsed { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the token has been revoked.
    /// </summary>
    public bool IsRevoked { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when the token expires.
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Gets or sets the IP address from which the token was created.
    /// </summary>
    public string? CreatedByIp { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when the token was revoked.
    /// </summary>
    public DateTime? RevokedAt { get; set; }

    /// <summary>
    /// Gets or sets the IP address from which the token was revoked.
    /// </summary>
    public string? RevokedByIp { get; set; }

    /// <summary>
    /// Gets or sets the replacement token if this token was rotated.
    /// </summary>
    public string? ReplacedByToken { get; set; }

    /// <summary>
    /// Gets or sets the reason for revocation.
    /// </summary>
    public string? ReasonRevoked { get; set; }

    #endregion

    #region Navigation Properties

    /// <summary>
    /// Gets or sets the user this token belongs to.
    /// </summary>
    public virtual ApplicationUser User { get; set; } = null!;

    #endregion

    #region Computed Properties

    /// <summary>
    /// Gets a value indicating whether the token is expired.
    /// </summary>
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

    /// <summary>
    /// Gets a value indicating whether the token is still active (not expired, used, or revoked).
    /// </summary>
    public bool IsActive => !IsRevoked && !IsUsed && !IsExpired;

    #endregion
}
