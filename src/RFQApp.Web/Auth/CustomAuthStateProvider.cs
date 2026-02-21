using System.Security.Claims;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

using RFQApp.Application.DTOs.Auth;
using RFQApp.Application.Interfaces;

namespace RFQApp.Web.Auth;

/// <summary>
/// Custom authentication state provider for Blazor Server.
/// </summary>
public class CustomAuthStateProvider : AuthenticationStateProvider
{
    #region Fields

    private readonly ProtectedLocalStorage _localStorage;
    private readonly IServiceProvider _serviceProvider;
    private readonly ClaimsPrincipal _anonymous = new(new ClaimsIdentity());
    private ClaimsPrincipal _currentUser;

    #endregion

    #region Constants

    private const string AuthTokenKey = "authToken";
    private const string RefreshTokenKey = "refreshToken";
    private const string UserDataKey = "userData";

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomAuthStateProvider"/> class.
    /// </summary>
    /// <param name="localStorage">The protected local storage.</param>
    /// <param name="serviceProvider">The service provider.</param>
    public CustomAuthStateProvider(
        ProtectedLocalStorage localStorage,
        IServiceProvider serviceProvider)
    {
        _localStorage = localStorage;
        _serviceProvider = serviceProvider;
        _currentUser = _anonymous;
    }

    #endregion

    #region Public Methods

    /// <inheritdoc />
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (_currentUser.Identity?.IsAuthenticated == true)
        {
            return new AuthenticationState(_currentUser);
        }

        try
        {
            ProtectedBrowserStorageResult<string> tokenResult = await _localStorage.GetAsync<string>(AuthTokenKey);
            ProtectedBrowserStorageResult<UserDto> userResult = await _localStorage.GetAsync<UserDto>(UserDataKey);

            if (!tokenResult.Success || string.IsNullOrEmpty(tokenResult.Value) || !userResult.Success || userResult.Value == null)
            {
                _currentUser = _anonymous;
                return new AuthenticationState(_anonymous);
            }

            ClaimsPrincipal user = CreateClaimsPrincipal(userResult.Value);
            _currentUser = user;
            return new AuthenticationState(user);
        }
        catch
        {
            // During prerender, browser storage isn't available. Preserve in-circuit auth state if present.
            if (_currentUser.Identity?.IsAuthenticated == true)
            {
                return new AuthenticationState(_currentUser);
            }

            _currentUser = _anonymous;
            return new AuthenticationState(_anonymous);
        }
    }

    /// <summary>
    /// Marks the user as authenticated with the given response.
    /// </summary>
    /// <param name="response">The authentication response.</param>
    public async Task MarkUserAsAuthenticatedAsync(AuthResponse response)
    {
        if (response.User == null || string.IsNullOrEmpty(response.AccessToken))
        {
            return;
        }

        await _localStorage.SetAsync(AuthTokenKey, response.AccessToken);
        await _localStorage.SetAsync(RefreshTokenKey, response.RefreshToken ?? string.Empty);
        await _localStorage.SetAsync(UserDataKey, response.User);

        ClaimsPrincipal user = CreateClaimsPrincipal(response.User);
        _currentUser = user;
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    /// <summary>
    /// Marks the user as logged out.
    /// </summary>
    public async Task MarkUserAsLoggedOutAsync()
    {
        try
        {
            // Get refresh token to revoke on server
            ProtectedBrowserStorageResult<string> refreshTokenResult = await _localStorage.GetAsync<string>(RefreshTokenKey);

            if (refreshTokenResult.Success && !string.IsNullOrEmpty(refreshTokenResult.Value))
            {
                using IServiceScope scope = _serviceProvider.CreateScope();
                IAuthService authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
                await authService.LogoutAsync(refreshTokenResult.Value);
            }
        }
        catch
        {
            // Continue with local logout even if server logout fails
        }

        await _localStorage.DeleteAsync(AuthTokenKey);
        await _localStorage.DeleteAsync(RefreshTokenKey);
        await _localStorage.DeleteAsync(UserDataKey);

        _currentUser = _anonymous;
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
    }

    /// <summary>
    /// Gets the current access token.
    /// </summary>
    /// <returns>The access token, or null if not authenticated.</returns>
    public async Task<string?> GetAccessTokenAsync()
    {
        try
        {
            ProtectedBrowserStorageResult<string> result = await _localStorage.GetAsync<string>(AuthTokenKey);
            return result.Success ? result.Value : null;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Gets the current refresh token.
    /// </summary>
    /// <returns>The refresh token, or null if not authenticated.</returns>
    public async Task<string?> GetRefreshTokenAsync()
    {
        try
        {
            ProtectedBrowserStorageResult<string> result = await _localStorage.GetAsync<string>(RefreshTokenKey);
            return result.Success ? result.Value : null;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Gets the current user data.
    /// </summary>
    /// <returns>The user data, or null if not authenticated.</returns>
    public async Task<UserDto?> GetUserDataAsync()
    {
        try
        {
            ProtectedBrowserStorageResult<UserDto> result = await _localStorage.GetAsync<UserDto>(UserDataKey);
            return result.Success ? result.Value : null;
        }
        catch
        {
            return null;
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Creates a ClaimsPrincipal from a UserDto.
    /// </summary>
    /// <param name="user">The user data.</param>
    /// <returns>The claims principal.</returns>
    private static ClaimsPrincipal CreateClaimsPrincipal(UserDto user)
    {
        List<Claim> claims =
        [
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim("FirstName", user.FirstName),
            new Claim("LastName", user.LastName)
        ];

        if (user.SupplierId.HasValue)
        {
            claims.Add(new Claim("SupplierId", user.SupplierId.Value.ToString()));
        }

        if (!string.IsNullOrEmpty(user.CompanyName))
        {
            claims.Add(new Claim("CompanyName", user.CompanyName));
        }

        claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

        ClaimsIdentity identity = new(claims, "jwt");
        return new ClaimsPrincipal(identity);
    }

    #endregion
}
