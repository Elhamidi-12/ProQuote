namespace RFQApp.Application.Common;

/// <summary>
/// Configuration settings for JWT authentication.
/// </summary>
public class JwtSettings
{
    #region Constants

    /// <summary>
    /// The configuration section name.
    /// </summary>
    public const string SectionName = "JwtSettings";

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the secret key for signing tokens.
    /// </summary>
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the token issuer.
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the token audience.
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the access token expiration in minutes.
    /// </summary>
    public int AccessTokenExpirationMinutes { get; set; } = 15;

    /// <summary>
    /// Gets or sets the refresh token expiration in days.
    /// </summary>
    public int RefreshTokenExpirationDays { get; set; } = 7;

    #endregion
}
