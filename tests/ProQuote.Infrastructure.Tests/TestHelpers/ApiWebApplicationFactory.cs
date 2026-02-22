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
    private string ConnectionString =>
        $"Server=(localdb)\\mssqllocaldb;Database={_databaseName};Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        Environment.SetEnvironmentVariable("JwtSettings__SecretKey", JwtSecret);
        Environment.SetEnvironmentVariable("JwtSettings__Issuer", "ProQuote.Tests");
        Environment.SetEnvironmentVariable("JwtSettings__Audience", "ProQuote.Tests.Client");

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
                ["ConnectionStrings:DefaultConnection"] = ConnectionString
            };

            config.AddInMemoryCollection(settings);
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(ConnectionString)
                .Options;

            using AppDbContext context = new(options);
            context.Database.EnsureDeleted();
        }

        Environment.SetEnvironmentVariable("JwtSettings__SecretKey", null);
        Environment.SetEnvironmentVariable("JwtSettings__Issuer", null);
        Environment.SetEnvironmentVariable("JwtSettings__Audience", null);
    }
}
