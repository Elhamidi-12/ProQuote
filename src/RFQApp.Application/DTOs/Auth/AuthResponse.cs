namespace RFQApp.Application.DTOs.Auth;

/// <summary>
/// Response model for authentication operations.
/// </summary>
public class AuthResponse
{
    /// <summary>
    /// Gets or sets a value indicating whether the operation was successful.
    /// </summary>
    public bool Succeeded { get; set; }

    /// <summary>
    /// Gets or sets the JWT access token.
    /// </summary>
    public string? AccessToken { get; set; }

    /// <summary>
    /// Gets or sets the refresh token.
    /// </summary>
    public string? RefreshToken { get; set; }

    /// <summary>
    /// Gets or sets the access token expiration time (UTC).
    /// </summary>
    public DateTime? ExpiresAt { get; set; }

    /// <summary>
    /// Gets or sets the authenticated user information.
    /// </summary>
    public UserDto? User { get; set; }

    /// <summary>
    /// Gets or sets the error message if the operation failed.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Gets or sets the list of validation errors.
    /// </summary>
    public List<string> Errors { get; set; } = [];

    #region Factory Methods

    /// <summary>
    /// Creates a successful authentication response.
    /// </summary>
    /// <param name="accessToken">The access token.</param>
    /// <param name="refreshToken">The refresh token.</param>
    /// <param name="expiresAt">The token expiration time.</param>
    /// <param name="user">The user information.</param>
    /// <returns>A successful authentication response.</returns>
    public static AuthResponse Success(string accessToken, string refreshToken, DateTime expiresAt, UserDto user)
    {
        return new AuthResponse
        {
            Succeeded = true,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = expiresAt,
            User = user
        };
    }

    /// <summary>
    /// Creates a failed authentication response.
    /// </summary>
    /// <param name="errorMessage">The error message.</param>
    /// <returns>A failed authentication response.</returns>
    public static AuthResponse Failure(string errorMessage)
    {
        return new AuthResponse
        {
            Succeeded = false,
            ErrorMessage = errorMessage
        };
    }

    /// <summary>
    /// Creates a failed authentication response with multiple errors.
    /// </summary>
    /// <param name="errors">The list of errors.</param>
    /// <returns>A failed authentication response.</returns>
    public static AuthResponse Failure(IEnumerable<string> errors)
    {
        return new AuthResponse
        {
            Succeeded = false,
            Errors = errors.ToList(),
            ErrorMessage = errors.FirstOrDefault()
        };
    }

    #endregion
}
