using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using ProQuote.Application.DTOs.Auth;
using ProQuote.Domain.Entities;
using ProQuote.Domain.Enums;
using ProQuote.Infrastructure.Data;
using ProQuote.Infrastructure.Tests.TestHelpers;

using Xunit;

namespace ProQuote.Infrastructure.Tests;

public sealed class ApiIntegrationTests : IClassFixture<ApiWebApplicationFactory>
{
    private readonly ApiWebApplicationFactory _factory;

    public ApiIntegrationTests(ApiWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task AuthEndpoints_ShouldRegisterLoginRefreshAndLogout()
    {
        using HttpClient client = _factory.CreateClient();
        string email = $"buyer.{Guid.NewGuid():N}@test.local";
        const string password = "Buyer#12345";

        HttpResponseMessage registerResponse = await client.PostAsJsonAsync("/api/v1/auth/register/buyer", new RegisterRequest
        {
            Email = email,
            Password = password,
            ConfirmPassword = password,
            FirstName = "Buyer",
            LastName = "User"
        });
        registerResponse.EnsureSuccessStatusCode();

        AuthResponse login = await LoginAsync(client, email, password);
        Assert.True(login.Succeeded);
        Assert.False(string.IsNullOrWhiteSpace(login.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(login.RefreshToken));

        HttpResponseMessage refreshResponse = await client.PostAsJsonAsync("/api/v1/auth/refresh", new RefreshTokenRequest
        {
            AccessToken = login.AccessToken!,
            RefreshToken = login.RefreshToken!
        });
        refreshResponse.EnsureSuccessStatusCode();
        AuthResponse refreshed = await ReadAsAsync<AuthResponse>(refreshResponse);
        Assert.True(refreshed.Succeeded);
        Assert.NotEqual(login.RefreshToken, refreshed.RefreshToken);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", refreshed.AccessToken);
        HttpResponseMessage logoutResponse = await client.PostAsJsonAsync("/api/v1/auth/logout", new { refreshToken = refreshed.RefreshToken });
        logoutResponse.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task DocumentEndpoints_ShouldReturn401_WhenNoToken()
    {
        using HttpClient client = _factory.CreateClient();
        HttpResponseMessage response = await client.GetAsync(new Uri($"/api/v1/documents/rfq/{Guid.NewGuid()}", UriKind.Relative));
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DocumentEndpoints_ShouldEnforceAccessControl_ForSuppliers()
    {
        using HttpClient client = _factory.CreateClient();

        string buyerEmail = $"buyer.{Guid.NewGuid():N}@test.local";
        string supplierEmail = $"supplier.{Guid.NewGuid():N}@test.local";
        string otherSupplierEmail = $"supplier.other.{Guid.NewGuid():N}@test.local";
        const string password = "Supplier#12345";

        await RegisterBuyerAsync(client, buyerEmail, password);
        await RegisterSupplierAsync(client, supplierEmail, password, $"Supplier-{Guid.NewGuid():N}");
        await RegisterSupplierAsync(client, otherSupplierEmail, password, $"Supplier-{Guid.NewGuid():N}");
        await ApproveSupplierAsync(supplierEmail);
        await ApproveSupplierAsync(otherSupplierEmail);

        AuthResponse buyerLogin = await LoginAsync(client, buyerEmail, password);
        AuthResponse supplierLogin = await LoginAsync(client, supplierEmail, password);
        AuthResponse otherSupplierLogin = await LoginAsync(client, otherSupplierEmail, password);

        SeededDocumentIds seeded = await SeedDocumentsAsync(
            buyerLogin.User!.Id,
            supplierLogin.User!.Id);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", supplierLogin.AccessToken);
        HttpResponseMessage supplierAccess = await client.GetAsync(new Uri($"/api/v1/documents/rfq/{seeded.RfqDocumentId}", UriKind.Relative));
        Assert.Equal(HttpStatusCode.OK, supplierAccess.StatusCode);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", otherSupplierLogin.AccessToken);
        HttpResponseMessage otherSupplierAccess = await client.GetAsync(new Uri($"/api/v1/documents/rfq/{seeded.RfqDocumentId}", UriKind.Relative));
        Assert.Equal(HttpStatusCode.Forbidden, otherSupplierAccess.StatusCode);
    }

    private async Task<SeededDocumentIds> SeedDocumentsAsync(Guid buyerUserId, Guid supplierUserId)
    {
        using IServiceScope scope = _factory.Services.CreateScope();
        AppDbContext context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        IWebHostEnvironment hostEnvironment = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

        Supplier supplier = await context.Suppliers.FirstAsync(s => s.UserId == supplierUserId);

        Category category = new()
        {
            Id = Guid.NewGuid(),
            Name = $"IT-{Guid.NewGuid():N}",
            Description = "Integration test category",
            DisplayOrder = 1,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        Rfq rfq = new()
        {
            Id = Guid.NewGuid(),
            ReferenceNumber = $"RFQ-2026-{Random.Shared.Next(10000, 99999)}",
            Title = "Integration RFQ",
            CategoryId = category.Id,
            Currency = "USD",
            SubmissionDeadline = DateTime.UtcNow.AddDays(5),
            Status = RfqStatus.Published,
            BuyerId = buyerUserId,
            CreatedAt = DateTime.UtcNow,
            PublishedAt = DateTime.UtcNow
        };

        Quote quote = new()
        {
            Id = Guid.NewGuid(),
            RfqId = rfq.Id,
            SupplierId = supplier.Id,
            Status = QuoteStatus.Submitted,
            LeadTimeDays = 10,
            ValidUntil = DateTime.UtcNow.AddDays(20),
            TotalAmount = 2500m,
            SubmittedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        RfqInvitation invited = new()
        {
            Id = Guid.NewGuid(),
            RfqId = rfq.Id,
            SupplierId = supplier.Id,
            SecureToken = Guid.NewGuid().ToString("N"),
            Status = InvitationStatus.Sent,
            SentAt = DateTime.UtcNow,
            TokenExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        };

        RfqDocument rfqDocument = new()
        {
            Id = Guid.NewGuid(),
            RfqId = rfq.Id,
            FileName = "rfq-test.pdf",
            StoredFileName = $"{Guid.NewGuid():N}.pdf",
            ContentType = "application/pdf",
            FileSize = 10,
            FilePath = $"/api/v1/documents/rfq/{Guid.NewGuid()}",
            DisplayOrder = 1,
            UploadedById = buyerUserId,
            UploadedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        await context.Categories.AddAsync(category);
        await context.Rfqs.AddAsync(rfq);
        await context.Quotes.AddAsync(quote);
        await context.RfqInvitations.AddAsync(invited);
        await context.RfqDocuments.AddAsync(rfqDocument);
        await context.SaveChangesAsync();

        string rfqDir = Path.Combine(hostEnvironment.ContentRootPath, "App_Data", "uploads", "rfqs", rfq.Id.ToString("N"));
        Directory.CreateDirectory(rfqDir);
        await File.WriteAllTextAsync(Path.Combine(rfqDir, rfqDocument.StoredFileName), "rfq-content");

        return new SeededDocumentIds(rfqDocument.Id);
    }

    private async Task ApproveSupplierAsync(string supplierEmail)
    {
        using IServiceScope scope = _factory.Services.CreateScope();
        AppDbContext context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Supplier? supplier = await context.Suppliers.FirstOrDefaultAsync(s => s.Email == supplierEmail);
        if (supplier == null)
        {
            throw new InvalidOperationException("Expected supplier profile to exist after registration.");
        }

        supplier.Status = SupplierStatus.Approved;
        supplier.ApprovedAt = DateTime.UtcNow;
        context.Suppliers.Update(supplier);
        await context.SaveChangesAsync();
    }

    private static async Task RegisterBuyerAsync(HttpClient client, string email, string password)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync("/api/v1/auth/register/buyer", new RegisterRequest
        {
            Email = email,
            Password = password,
            ConfirmPassword = password,
            FirstName = "Buyer",
            LastName = "User"
        });
        response.EnsureSuccessStatusCode();
    }

    private static async Task RegisterSupplierAsync(HttpClient client, string email, string password, string companyName)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync("/api/v1/auth/register/supplier", new SupplierRegisterRequest
        {
            Email = email,
            Password = password,
            ConfirmPassword = password,
            FirstName = "Supplier",
            LastName = "User",
            CompanyName = companyName,
            ContactName = "Supplier Contact"
        });
        response.EnsureSuccessStatusCode();
    }

    private static async Task<AuthResponse> LoginAsync(HttpClient client, string email, string password)
    {
        HttpResponseMessage loginResponse = await client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest
        {
            Email = email,
            Password = password
        });
        loginResponse.EnsureSuccessStatusCode();
        return await ReadAsAsync<AuthResponse>(loginResponse);
    }

    private static async Task<T> ReadAsAsync<T>(HttpResponseMessage response)
    {
        T? payload = await response.Content.ReadFromJsonAsync<T>(new System.Text.Json.JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        return payload ?? throw new InvalidOperationException("Response payload was null.");
    }

    private sealed record SeededDocumentIds(Guid RfqDocumentId);
}
