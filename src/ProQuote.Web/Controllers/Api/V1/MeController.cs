using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using ProQuote.Application.DTOs.Auth;
using ProQuote.Application.DTOs.Profile;
using ProQuote.Application.Interfaces;

namespace ProQuote.Web.Controllers.Api.V1;

/// <summary>
/// Current user profile endpoints.
/// </summary>
[Route("api/v1/me")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class MeController : ApiControllerBase
{
    private readonly IAuthService _authService;

    /// <summary>
    /// Initializes a new instance of the <see cref="MeController"/> class.
    /// </summary>
    /// <param name="authService">Auth service.</param>
    public MeController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Gets current user profile.
    /// </summary>
    /// <returns>User profile payload.</returns>
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        if (!CurrentUserId.HasValue)
        {
            return Unauthorized();
        }

        UserProfileDto? profile = await _authService.GetUserProfileAsync(CurrentUserId.Value);
        return profile == null ? NotFound() : Ok(profile);
    }

    /// <summary>
    /// Updates current user profile.
    /// </summary>
    /// <param name="request">Update profile request.</param>
    /// <returns>Operation response.</returns>
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserProfileRequest request)
    {
        if (!CurrentUserId.HasValue)
        {
            return Unauthorized();
        }

        AuthResponse response = await _authService.UpdateUserProfileAsync(CurrentUserId.Value, request);
        return response.Succeeded ? Ok(response) : BadRequest(response);
    }

    /// <summary>
    /// Updates current user settings.
    /// </summary>
    /// <param name="request">Update settings request.</param>
    /// <returns>Operation response.</returns>
    [HttpPut("settings")]
    public async Task<IActionResult> UpdateSettings([FromBody] UpdateUserSettingsRequest request)
    {
        if (!CurrentUserId.HasValue)
        {
            return Unauthorized();
        }

        AuthResponse response = await _authService.UpdateUserSettingsAsync(CurrentUserId.Value, request);
        return response.Succeeded ? Ok(response) : BadRequest(response);
    }

    /// <summary>
    /// Updates supplier profile for current supplier user.
    /// </summary>
    /// <param name="request">Supplier profile request.</param>
    /// <returns>Operation response.</returns>
    [HttpPut("supplier-profile")]
    [Authorize(Roles = Infrastructure.Identity.ApplicationRoles.Supplier, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> UpdateSupplierProfile([FromBody] UpdateSupplierProfileRequest request)
    {
        if (!CurrentUserId.HasValue)
        {
            return Unauthorized();
        }

        AuthResponse response = await _authService.UpdateSupplierProfileAsync(CurrentUserId.Value, request);
        return response.Succeeded ? Ok(response) : BadRequest(response);
    }
}
