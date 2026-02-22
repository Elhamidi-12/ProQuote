using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using ProQuote.Infrastructure.Data;

namespace ProQuote.Infrastructure.Tests.TestHelpers;

public sealed class ApiWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"ProQuoteIntegration_{Guid.NewGuid():N}";
    private const string JwtSecret = "integration-tests-secret-key-32-characters-minimum!";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        Environment.SetEnvironmentVariable("JwtSettings__SecretKey", JwtSecret);
        Environment.SetEnvironmentVariable("JwtSettings__Issuer", "ProQuote.Tests");
        Environment.SetEnvironmentVariable("JwtSettings__Audience", "ProQuote.Tests.Client");
        Environment.SetEnvironmentVariable("Database__Provider", "InMemory");
        Environment.SetEnvironmentVariable("Database__InMemoryName", _databaseName);

        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            Dictionary<string, string?> settings = new()
            {
                ["JwtSettings:SecretKey"] = JwtSecret,
                ["JwtSettings:Issuer"] = "ProQuote.Tests",
                ["JwtSettings:Audience"] = "ProQuote.Tests.Client",
                ["JwtSettings:AccessTokenExpirationMinutes"] = "60",
                ["JwtSettings:RefreshTokenExpirationDays"] = "7",
                ["Database:Provider"] = "InMemory",
                ["Database:InMemoryName"] = _databaseName
            };

            config.AddInMemoryCollection(settings);
        });
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(_databaseName)
                .Options;

            using AppDbContext context = new(options);
            context.Database.EnsureDeleted();
        }

        Environment.SetEnvironmentVariable("JwtSettings__SecretKey", null);
        Environment.SetEnvironmentVariable("JwtSettings__Issuer", null);
        Environment.SetEnvironmentVariable("JwtSettings__Audience", null);
        Environment.SetEnvironmentVariable("Database__Provider", null);
        Environment.SetEnvironmentVariable("Database__InMemoryName", null);

        base.Dispose(disposing);
    }
}
