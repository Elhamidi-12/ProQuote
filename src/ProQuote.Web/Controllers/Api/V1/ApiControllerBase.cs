using System.Security.Claims;

using Microsoft.AspNetCore.Mvc;

namespace ProQuote.Web.Controllers.Api.V1;

/// <summary>
/// Base controller for v1 API endpoints.
/// </summary>
[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    /// <summary>
    /// Gets current authenticated user identifier from claims.
    /// </summary>
    protected Guid? CurrentUserId
    {
        get
        {
            string? userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(userIdClaim, out Guid userId) ? userId : null;
        }
    }

    /// <summary>
    /// Gets best effort remote IP address.
    /// </summary>
    protected string? RemoteIpAddress => HttpContext.Connection.RemoteIpAddress?.ToString();
}
