using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using ProQuote.Application.Common;
using ProQuote.Application.DTOs.Auth;
using ProQuote.Infrastructure.Data;
using ProQuote.Infrastructure.Identity;
using ProQuote.Infrastructure.Services;

using Xunit;

namespace ProQuote.Infrastructure.Tests;

public class AuthServiceTests
{
    [Fact]
    public async Task RefreshTokenAsync_ShouldRotateRefreshToken_WhenRequestIsValid()
    {
        using ServiceProvider provider = BuildServiceProvider(Guid.NewGuid().ToString());
        UserManager<ApplicationUserIdentity> userManager = provider.GetRequiredService<UserManager<ApplicationUserIdentity>>();
        RoleManager<ApplicationRoleIdentity> roleManager = provider.GetRequiredService<RoleManager<ApplicationRoleIdentity>>();
        AppDbContext context = provider.GetRequiredService<AppDbContext>();

        await EnsureRoleExistsAsync(roleManager, ApplicationRoles.Buyer);

        ApplicationUserIdentity user = new()
        {
            Id = Guid.NewGuid(),
            UserName = "buyer.refresh@test.local",
            Email = "buyer.refresh@test.local",
            FirstName = "Buyer",
            LastName = "Refresh",
            IsActive = true
        };

        IdentityResult createResult = await userManager.CreateAsync(user, "Buyer#12345");
        Assert.True(createResult.Succeeded);
        await userManager.AddToRoleAsync(user, ApplicationRoles.Buyer);

        AuthService service = CreateService(provider);
        LoginRequest loginRequest = new()
        {
            Email = user.Email!,
            Password = "Buyer#12345"
        };

        AuthResponse login = await service.LoginAsync(loginRequest, "127.0.0.1");
        Assert.True(login.Succeeded);
        Assert.False(string.IsNullOrWhiteSpace(login.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(login.RefreshToken));

        RefreshTokenRequest refreshRequest = new()
        {
            AccessToken = login.AccessToken,
            RefreshToken = login.RefreshToken
        };

        AuthResponse refreshed = await service.RefreshTokenAsync(refreshRequest, "127.0.0.1");

        Assert.True(refreshed.Succeeded);
        Assert.False(string.IsNullOrWhiteSpace(refreshed.AccessToken));
        Assert.NotEqual(login.RefreshToken, refreshed.RefreshToken);

        Domain.Entities.RefreshToken? originalToken = await context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == login.RefreshToken && rt.UserId == user.Id);
        Domain.Entities.RefreshToken? replacementToken = await context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshed.RefreshToken && rt.UserId == user.Id);

        Assert.NotNull(originalToken);
        Assert.NotNull(replacementToken);
        Assert.True(originalToken!.IsUsed);
        Assert.Equal(refreshed.RefreshToken, originalToken.ReplacedByToken);
        Assert.False(replacementToken!.IsUsed);
        Assert.False(replacementToken.IsRevoked);
    }

    [Fact]
    public async Task RefreshTokenAsync_ShouldRejectReuseOfUsedRefreshToken()
    {
        using ServiceProvider provider = BuildServiceProvider(Guid.NewGuid().ToString());
        UserManager<ApplicationUserIdentity> userManager = provider.GetRequiredService<UserManager<ApplicationUserIdentity>>();
        RoleManager<ApplicationRoleIdentity> roleManager = provider.GetRequiredService<RoleManager<ApplicationRoleIdentity>>();
        AppDbContext context = provider.GetRequiredService<AppDbContext>();

        await EnsureRoleExistsAsync(roleManager, ApplicationRoles.Buyer);

        ApplicationUserIdentity user = new()
        {
            Id = Guid.NewGuid(),
            UserName = "buyer.reuse@test.local",
            Email = "buyer.reuse@test.local",
            FirstName = "Buyer",
            LastName = "Reuse",
            IsActive = true
        };

        IdentityResult createResult = await userManager.CreateAsync(user, "Buyer#12345");
        Assert.True(createResult.Succeeded);
        await userManager.AddToRoleAsync(user, ApplicationRoles.Buyer);

        AuthService service = CreateService(provider);
        AuthResponse login = await service.LoginAsync(
            new LoginRequest { Email = user.Email!, Password = "Buyer#12345" },
            "127.0.0.1");
        Assert.True(login.Succeeded);
        Assert.False(string.IsNullOrWhiteSpace(login.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(login.RefreshToken));

        RefreshTokenRequest firstRefresh = new()
        {
            AccessToken = login.AccessToken!,
            RefreshToken = login.RefreshToken!
        };

        AuthResponse refreshed = await service.RefreshTokenAsync(firstRefresh, "127.0.0.1");
        Assert.True(refreshed.Succeeded);

        AuthResponse reuseAttempt = await service.RefreshTokenAsync(firstRefresh, "127.0.0.1");

        Assert.False(reuseAttempt.Succeeded);
        Assert.Contains("used", reuseAttempt.ErrorMessage ?? string.Empty, StringComparison.OrdinalIgnoreCase);

        int activeTokenCount = await context.RefreshTokens
            .CountAsync(rt => rt.UserId == user.Id && !rt.IsUsed && !rt.IsRevoked);
        Assert.Equal(1, activeTokenCount);
    }

    private static ServiceProvider BuildServiceProvider(string databaseName)
    {
        ServiceCollection services = new();

        services.AddLogging();
        services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase(databaseName));
        services
            .AddIdentity<ApplicationUserIdentity, ApplicationRoleIdentity>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        return services.BuildServiceProvider();
    }

    private static AuthService CreateService(ServiceProvider provider)
    {
        JwtSettings settings = new()
        {
            SecretKey = "this-is-a-test-secret-key-with-at-least-32-chars",
            Issuer = "ProQuote.Tests",
            Audience = "ProQuote.Tests.Client",
            AccessTokenExpirationMinutes = 15,
            RefreshTokenExpirationDays = 7
        };

        return new AuthService(
            provider.GetRequiredService<UserManager<ApplicationUserIdentity>>(),
            provider.GetRequiredService<SignInManager<ApplicationUserIdentity>>(),
            provider.GetRequiredService<AppDbContext>(),
            Options.Create(settings));
    }

    private static async Task EnsureRoleExistsAsync(RoleManager<ApplicationRoleIdentity> roleManager, string roleName)
    {
        if (await roleManager.RoleExistsAsync(roleName))
        {
            return;
        }

        ApplicationRoleIdentity role = new(roleName)
        {
            Id = Guid.NewGuid(),
            Description = $"{roleName} role for tests",
            CreatedAt = DateTime.UtcNow,
            IsSystemRole = true
        };

        IdentityResult roleResult = await roleManager.CreateAsync(role);
        Assert.True(roleResult.Succeeded);
    }
}
