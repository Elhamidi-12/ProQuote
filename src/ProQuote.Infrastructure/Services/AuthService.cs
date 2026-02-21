using System.Collections.ObjectModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using ProQuote.Application.Common;
using ProQuote.Application.DTOs.Auth;
using ProQuote.Application.Interfaces;
using ProQuote.Domain.Entities;
using ProQuote.Domain.Enums;
using ProQuote.Infrastructure.Data;
using ProQuote.Infrastructure.Identity;

namespace ProQuote.Infrastructure.Services;

/// <summary>
/// Implementation of the authentication service.
/// </summary>
public class AuthService : IAuthService
{
    #region Fields

    private readonly UserManager<ApplicationUserIdentity> _userManager;
    private readonly SignInManager<ApplicationUserIdentity> _signInManager;
    private readonly AppDbContext _context;
    private readonly JwtSettings _jwtSettings;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthService"/> class.
    /// </summary>
    /// <param name="userManager">The user manager.</param>
    /// <param name="signInManager">The sign-in manager.</param>
    /// <param name="context">The database context.</param>
    /// <param name="jwtSettings">The JWT settings.</param>
    public AuthService(
        UserManager<ApplicationUserIdentity> userManager,
        SignInManager<ApplicationUserIdentity> signInManager,
        AppDbContext context,
        IOptions<JwtSettings> jwtSettings)
    {
        ArgumentNullException.ThrowIfNull(userManager);
        ArgumentNullException.ThrowIfNull(signInManager);
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(jwtSettings);

        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
        _jwtSettings = jwtSettings.Value;
    }

    #endregion

    #region Public Methods

    /// <inheritdoc />
    public async Task<AuthResponse> LoginAsync(LoginRequest request, string? ipAddress = null)
    {
        ArgumentNullException.ThrowIfNull(request);

        ApplicationUserIdentity? user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
        {
            return AuthResponse.Failure("Invalid email or password.");
        }

        if (!user.IsActive)
        {
            return AuthResponse.Failure("Your account has been deactivated. Please contact support.");
        }

        SignInResult result = await _signInManager.CheckPasswordSignInAsync(
            user, request.Password, lockoutOnFailure: true);

        if (result.IsLockedOut)
        {
            return AuthResponse.Failure("Your account has been locked due to multiple failed login attempts. Please try again later.");
        }

        if (!result.Succeeded)
        {
            return AuthResponse.Failure("Invalid email or password.");
        }

        // Check if supplier is approved
        Supplier? supplier = await _context.Suppliers.FirstOrDefaultAsync(s => s.UserId == user.Id);
        if (supplier != null && supplier.Status != SupplierStatus.Approved)
        {
            string statusMessage = supplier.Status switch
            {
                SupplierStatus.Pending => "Your supplier account is pending approval.",
                SupplierStatus.Rejected => "Your supplier registration has been rejected.",
                SupplierStatus.Suspended => "Your supplier account has been suspended.",
                _ => "Your account is not active."
            };
            return AuthResponse.Failure(statusMessage);
        }

        // Update last login
        user.LastLoginAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        // Generate tokens
        IList<string> roles = await _userManager.GetRolesAsync(user);
        string accessToken = GenerateAccessToken(user, roles);
        RefreshToken refreshToken = GenerateRefreshToken(user.Id, ipAddress);

        // Save refresh token
        await _context.RefreshTokens.AddAsync(refreshToken);
        await _context.SaveChangesAsync();

        UserDto userDto = await CreateUserDtoAsync(user, roles);
        DateTime expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes);

        return AuthResponse.Success(accessToken, refreshToken.Token, expiresAt, userDto);
    }

    /// <inheritdoc />
    public async Task<AuthResponse> RegisterBuyerAsync(RegisterRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        ApplicationUserIdentity? existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return AuthResponse.Failure("An account with this email already exists.");
        }

        ApplicationUserIdentity user = new()
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        IdentityResult result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            return AuthResponse.Failure(result.Errors.Select(e => e.Description));
        }

        await _userManager.AddToRoleAsync(user, ApplicationRoles.Buyer);

        return AuthResponse.Success(
            string.Empty,
            string.Empty,
            DateTime.UtcNow,
            await CreateUserDtoAsync(user, [ApplicationRoles.Buyer]));
    }

    /// <inheritdoc />
    public async Task<AuthResponse> RegisterSupplierAsync(SupplierRegisterRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        ApplicationUserIdentity? existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return AuthResponse.Failure("An account with this email already exists.");
        }

        // Check if company name already exists
        bool companyExists = await _context.Suppliers.AnyAsync(s => s.CompanyName == request.CompanyName);

        if (companyExists)
        {
            return AuthResponse.Failure("A supplier with this company name already exists.");
        }

        using Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // Create user
            ApplicationUserIdentity user = new()
            {
                UserName = request.Email,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            IdentityResult result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                return AuthResponse.Failure(result.Errors.Select(e => e.Description));
            }

            await _userManager.AddToRoleAsync(user, ApplicationRoles.Supplier);

            // Create supplier profile (pending approval)
            Supplier supplier = new()
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                CompanyName = request.CompanyName,
                ContactName = request.ContactName,
                Email = request.Email,
                Phone = request.PhoneNumber,
                Website = request.Website,
                Address = request.Address,
                City = request.City,
                Country = request.Country,
                TaxId = request.TaxId,
                Status = SupplierStatus.Pending,
                RegisteredAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Suppliers.AddAsync(supplier);

            // Add supplier categories
            if (request.CategoryIds.Count > 0)
            {
                List<SupplierCategory> supplierCategories = request.CategoryIds.Select((categoryId, index) => new SupplierCategory
                {
                    SupplierId = supplier.Id,
                    CategoryId = categoryId,
                    IsPrimary = index == 0
                }).ToList();

                await _context.SupplierCategories.AddRangeAsync(supplierCategories);
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            UserDto userDto = await CreateUserDtoAsync(user, [ApplicationRoles.Supplier]);
            userDto.SupplierId = supplier.Id;
            userDto.CompanyName = supplier.CompanyName;

            return new AuthResponse
            {
                Succeeded = true,
                User = userDto,
                ErrorMessage = "Registration successful! Your account is pending admin approval."
            };
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request, string? ipAddress = null)
    {
        ArgumentNullException.ThrowIfNull(request);

        ClaimsPrincipal? principal = GetPrincipalFromExpiredToken(request.AccessToken);
        if (principal == null)
        {
            return AuthResponse.Failure("Invalid access token.");
        }

        string? userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out Guid userId))
        {
            return AuthResponse.Failure("Invalid access token.");
        }

        RefreshToken? storedToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken && rt.UserId == userId);

        if (storedToken == null)
        {
            return AuthResponse.Failure("Invalid refresh token.");
        }

        if (storedToken.IsRevoked)
        {
            // Token was revoked - revoke all descendant tokens
            await RevokeDescendantRefreshTokensAsync(storedToken, userId, ipAddress, "Attempted reuse of revoked token");
            return AuthResponse.Failure("Refresh token has been revoked.");
        }

        if (storedToken.IsUsed)
        {
            return AuthResponse.Failure("Refresh token has already been used.");
        }

        if (storedToken.IsExpired)
        {
            return AuthResponse.Failure("Refresh token has expired.");
        }

        ApplicationUserIdentity? user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null || !user.IsActive)
        {
            return AuthResponse.Failure("User not found or inactive.");
        }

        // Rotate refresh token
        RefreshToken newRefreshToken = GenerateRefreshToken(userId, ipAddress);
        storedToken.IsUsed = true;
        storedToken.ReplacedByToken = newRefreshToken.Token;

        await _context.RefreshTokens.AddAsync(newRefreshToken);
        await _context.SaveChangesAsync();

        // Generate new access token
        IList<string> roles = await _userManager.GetRolesAsync(user);
        string accessToken = GenerateAccessToken(user, roles);

        UserDto userDto = await CreateUserDtoAsync(user, roles);
        DateTime expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes);

        return AuthResponse.Success(accessToken, newRefreshToken.Token, expiresAt, userDto);
    }

    /// <inheritdoc />
    public async Task<bool> LogoutAsync(string refreshToken, string? ipAddress = null)
    {
        RefreshToken? storedToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        if (storedToken == null)
        {
            return false;
        }

        storedToken.IsRevoked = true;
        storedToken.RevokedAt = DateTime.UtcNow;
        storedToken.RevokedByIp = ipAddress;
        storedToken.ReasonRevoked = "Logged out";

        await _context.SaveChangesAsync();
        return true;
    }

    /// <inheritdoc />
    public async Task<UserDto?> GetCurrentUserAsync(Guid userId)
    {
        ApplicationUserIdentity? user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return null;
        }

        IList<string> roles = await _userManager.GetRolesAsync(user);
        return await CreateUserDtoAsync(user, roles);
    }

    /// <inheritdoc />
    public async Task<bool> RevokeAllTokensAsync(Guid userId, string? ipAddress = null)
    {
        List<RefreshToken> tokens = await _context.RefreshTokens
            .Where(rt => rt.UserId == userId && !rt.IsRevoked && !rt.IsUsed)
            .ToListAsync();

        foreach (RefreshToken token in tokens)
        {
            token.IsRevoked = true;
            token.RevokedAt = DateTime.UtcNow;
            token.RevokedByIp = ipAddress;
            token.ReasonRevoked = "Revoked by user";
        }

        await _context.SaveChangesAsync();
        return true;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Generates a JWT access token for the user.
    /// </summary>
    private string GenerateAccessToken(ApplicationUserIdentity user, IList<string> roles)
    {
        List<Claim> claims =
        [
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        ];

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        SigningCredentials credentials = new(key, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken token = new(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Generates a refresh token.
    /// </summary>
    private RefreshToken GenerateRefreshToken(Guid userId, string? ipAddress)
    {
        byte[] randomBytes = new byte[64];
        using RandomNumberGenerator rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);

        return new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = Convert.ToBase64String(randomBytes),
            JwtId = Guid.NewGuid().ToString(),
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
            CreatedAt = DateTime.UtcNow,
            CreatedByIp = ipAddress
        };
    }

    /// <summary>
    /// Gets the principal from an expired access token.
    /// </summary>
    private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        TokenValidationParameters tokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false, // Allow expired tokens
            ValidateIssuerSigningKey = true,
            ValidIssuer = _jwtSettings.Issuer,
            ValidAudience = _jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey))
        };

        JwtSecurityTokenHandler tokenHandler = new();

        try
        {
            ClaimsPrincipal principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            return principal;
        }
        catch (Exception ex) when (ex is SecurityTokenException or ArgumentException)
        {
            return null;
        }
    }

    /// <summary>
    /// Revokes all descendant refresh tokens.
    /// </summary>
    private async Task RevokeDescendantRefreshTokensAsync(
        RefreshToken token,
        Guid userId,
        string? ipAddress,
        string reason)
    {
        if (!string.IsNullOrEmpty(token.ReplacedByToken))
        {
            RefreshToken? childToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == token.ReplacedByToken && rt.UserId == userId);

            if (childToken != null)
            {
                if (!childToken.IsRevoked && !childToken.IsUsed)
                {
                    childToken.IsRevoked = true;
                    childToken.RevokedAt = DateTime.UtcNow;
                    childToken.RevokedByIp = ipAddress;
                    childToken.ReasonRevoked = reason;
                }

                await RevokeDescendantRefreshTokensAsync(childToken, userId, ipAddress, reason);
            }
        }

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Creates a UserDto from an ApplicationUserIdentity.
    /// </summary>
    private async Task<UserDto> CreateUserDtoAsync(ApplicationUserIdentity user, IList<string> roles)
    {
        UserDto dto = new()
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            FirstName = user.FirstName,
            LastName = user.LastName,
            FullName = user.FullName,
            PhoneNumber = user.PhoneNumber,
            ProfilePictureUrl = user.ProfilePictureUrl,
            Roles = new Collection<string>(roles.ToList()),
            IsActive = user.IsActive
        };

        // Check if user is a supplier
        Supplier? supplier = await _context.Suppliers.FirstOrDefaultAsync(s => s.UserId == user.Id);
        if (supplier != null)
        {
            dto.SupplierId = supplier.Id;
            dto.CompanyName = supplier.CompanyName;
        }

        return dto;
    }

    #endregion
}
