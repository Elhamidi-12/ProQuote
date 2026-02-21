using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using ProQuote.Application.DTOs.Auth;
using ProQuote.Application.Interfaces;

namespace ProQuote.Web.Controllers.Api.V1;

/// <summary>
/// Authentication endpoints for API clients.
/// </summary>
[Route("api/v1/auth")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "Request type is scoped to controller endpoint usage.")]
public class AuthController : ApiControllerBase
{
    private readonly IAuthService _authService;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthController"/> class.
    /// </summary>
    /// <param name="authService">Auth service.</param>
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Authenticates a user.
    /// </summary>
    /// <param name="request">Login request.</param>
    /// <returns>Authentication response.</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        AuthResponse response = await _authService.LoginAsync(request, RemoteIpAddress);
        return response.Succeeded ? Ok(response) : Unauthorized(response);
    }

    /// <summary>
    /// Registers a buyer user.
    /// </summary>
    /// <param name="request">Register request.</param>
    /// <returns>Registration result.</returns>
    [HttpPost("register/buyer")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterBuyer([FromBody] RegisterRequest request)
    {
        AuthResponse response = await _authService.RegisterBuyerAsync(request);
        return response.Succeeded ? Ok(response) : BadRequest(response);
    }

    /// <summary>
    /// Registers a supplier user.
    /// </summary>
    /// <param name="request">Supplier registration request.</param>
    /// <returns>Registration result.</returns>
    [HttpPost("register/supplier")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterSupplier([FromBody] SupplierRegisterRequest request)
    {
        AuthResponse response = await _authService.RegisterSupplierAsync(request);
        return response.Succeeded ? Ok(response) : BadRequest(response);
    }

    /// <summary>
    /// Refreshes JWT tokens.
    /// </summary>
    /// <param name="request">Refresh request.</param>
    /// <returns>Authentication response.</returns>
    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
    {
        AuthResponse response = await _authService.RefreshTokenAsync(request, RemoteIpAddress);
        return response.Succeeded ? Ok(response) : Unauthorized(response);
    }

    /// <summary>
    /// Revokes refresh token and logs out session.
    /// </summary>
    /// <param name="request">Logout request.</param>
    /// <returns>Operation result.</returns>
    [HttpPost("logout")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> Logout([FromBody] LogoutApiRequest request)
    {
        if (request == null)
        {
            return BadRequest(new { message = "Request body is required." });
        }

        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return BadRequest(new { message = "Refresh token is required." });
        }

        bool revoked = await _authService.LogoutAsync(request.RefreshToken, RemoteIpAddress);
        return revoked ? Ok(new { succeeded = true }) : NotFound(new { succeeded = false, message = "Token not found." });
    }

    /// <summary>
    /// Revoke all refresh tokens for current user.
    /// </summary>
    /// <returns>Operation result.</returns>
    [HttpPost("revoke-all")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> RevokeAll()
    {
        if (!CurrentUserId.HasValue)
        {
            return Unauthorized();
        }

        bool revoked = await _authService.RevokeAllTokensAsync(CurrentUserId.Value, RemoteIpAddress);
        return Ok(new { succeeded = revoked });
    }

    /// <summary>
    /// Logout request.
    /// </summary>
    public sealed class LogoutApiRequest
    {
        /// <summary>
        /// Gets or sets refresh token.
        /// </summary>
        public string RefreshToken { get; set; } = string.Empty;
    }
}
