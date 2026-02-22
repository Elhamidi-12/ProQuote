using System.Text;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

using ProQuote.Application.Common;
using ProQuote.Infrastructure;
using ProQuote.UI.Services;
using ProQuote.Web.Auth;

namespace ProQuote.Web.Startup;

/// <summary>
/// Service registration extensions for the web application.
/// </summary>
public static class ServiceRegistrationExtensions
{
    /// <summary>
    /// Registers ProQuote web services and authentication.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">Application configuration.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddProQuoteWebServices(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddInfrastructure(configuration);

        services.AddScoped<CustomAuthStateProvider>();
        services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<CustomAuthStateProvider>());
        services.AddAuthorizationCore();
        services.AddAuthorization();
        services.AddCascadingAuthenticationState();
        services.AddControllers();

        services.AddScoped<ThemeService>();
        services.AddScoped<ToastService>();
        services.AddScoped<PostLoginNotificationStore>();

        JwtSettings jwtSettings = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>() ?? new JwtSettings();
        EnsureJwtSecretIsConfigured(jwtSettings);

        services.AddAuthentication()
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                    ClockSkew = TimeSpan.FromMinutes(1)
                };
            });

        services.AddRazorComponents()
            .AddInteractiveServerComponents();

        return services;
    }

    private static void EnsureJwtSecretIsConfigured(JwtSettings jwtSettings)
    {
        if (string.IsNullOrWhiteSpace(jwtSettings.SecretKey) ||
            jwtSettings.SecretKey.Contains("YourSuperSecretKeyThatIsAtLeast32CharactersLong", StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                "JWT secret key is not configured. Set JwtSettings:SecretKey via User Secrets or environment variables.");
        }
    }
}
