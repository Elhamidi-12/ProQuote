using RFQApp.Application.DTOs.Auth;

namespace RFQApp.Application.Interfaces;

/// <summary>
/// Service interface for authentication operations.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Authenticates a user with email and password.
    /// </summary>
    /// <param name="request">The login request.</param>
    /// <param name="ipAddress">The client IP address.</param>
    /// <returns>The authentication response.</returns>
    Task<AuthResponse> LoginAsync(LoginRequest request, string? ipAddress = null);

    /// <summary>
    /// Registers a new buyer user.
    /// </summary>
    /// <param name="request">The registration request.</param>
    /// <returns>The authentication response.</returns>
    Task<AuthResponse> RegisterBuyerAsync(RegisterRequest request);

    /// <summary>
    /// Registers a new supplier user (pending approval).
    /// </summary>
    /// <param name="request">The supplier registration request.</param>
    /// <returns>The authentication response.</returns>
    Task<AuthResponse> RegisterSupplierAsync(SupplierRegisterRequest request);

    /// <summary>
    /// Refreshes an expired access token.
    /// </summary>
    /// <param name="request">The refresh token request.</param>
    /// <param name="ipAddress">The client IP address.</param>
    /// <returns>The authentication response.</returns>
    Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request, string? ipAddress = null);

    /// <summary>
    /// Logs out a user by revoking their refresh token.
    /// </summary>
    /// <param name="refreshToken">The refresh token to revoke.</param>
    /// <param name="ipAddress">The client IP address.</param>
    /// <returns>True if logout was successful.</returns>
    Task<bool> LogoutAsync(string refreshToken, string? ipAddress = null);

    /// <summary>
    /// Gets the current authenticated user information.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>The user information.</returns>
    Task<UserDto?> GetCurrentUserAsync(Guid userId);

    /// <summary>
    /// Revokes all refresh tokens for a user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="ipAddress">The client IP address.</param>
    /// <returns>True if revocation was successful.</returns>
    Task<bool> RevokeAllTokensAsync(Guid userId, string? ipAddress = null);
}
