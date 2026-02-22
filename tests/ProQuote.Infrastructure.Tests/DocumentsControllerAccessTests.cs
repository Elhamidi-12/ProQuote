using System.Security.Claims;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;

using ProQuote.Application.Interfaces;
using ProQuote.Domain.Entities;
using ProQuote.Domain.Enums;
using ProQuote.Infrastructure.Data;
using ProQuote.Infrastructure.Identity;
using ProQuote.Infrastructure.Repositories;
using ProQuote.Infrastructure.Tests.TestHelpers;
using ProQuote.Web.Controllers.Api.V1;

using Xunit;

namespace ProQuote.Infrastructure.Tests;

public sealed class DocumentsControllerAccessTests
{
    [Fact]
    public async Task DownloadRfqDocument_ShouldReturnFile_ForBuyerOwner()
    {
        using AppDbContext context = TestDbContextFactory.Create(Guid.NewGuid().ToString());
        string root = CreateTempRoot();
        try
        {
            SeedData seed = await SeedScenarioAsync(context, root);
            using UnitOfWork unitOfWork = new(context);

            DocumentsController controller = CreateController(unitOfWork, root, seed.Buyer.Id, ApplicationRoles.Buyer);
            IActionResult result = await controller.DownloadRfqDocument(seed.RfqDocument.Id);

            FileStreamResult file = Assert.IsType<FileStreamResult>(result);
            Assert.Equal("application/pdf", file.ContentType);
            file.FileStream.Dispose();
        }
        finally
        {
            CleanupTempRoot(root);
        }
    }

    [Fact]
    public async Task DownloadRfqDocument_ShouldForbid_ForUninvitedSupplier()
    {
        using AppDbContext context = TestDbContextFactory.Create(Guid.NewGuid().ToString());
        string root = CreateTempRoot();
        try
        {
            SeedData seed = await SeedScenarioAsync(context, root);
            using UnitOfWork unitOfWork = new(context);

            DocumentsController controller = CreateController(unitOfWork, root, seed.OtherSupplierUser.Id, ApplicationRoles.Supplier);
            IActionResult result = await controller.DownloadRfqDocument(seed.RfqDocument.Id);

            Assert.IsType<ForbidResult>(result);
        }
        finally
        {
            CleanupTempRoot(root);
        }
    }

    [Fact]
    public async Task DownloadQuoteDocument_ShouldReturnFile_ForQuoteSupplier()
    {
        using AppDbContext context = TestDbContextFactory.Create(Guid.NewGuid().ToString());
        string root = CreateTempRoot();
        try
        {
            SeedData seed = await SeedScenarioAsync(context, root);
            using UnitOfWork unitOfWork = new(context);

            DocumentsController controller = CreateController(unitOfWork, root, seed.SupplierUser.Id, ApplicationRoles.Supplier);
            IActionResult result = await controller.DownloadQuoteDocument(seed.QuoteDocument.Id);

            FileStreamResult file = Assert.IsType<FileStreamResult>(result);
            Assert.Equal("application/pdf", file.ContentType);
            file.FileStream.Dispose();
        }
        finally
        {
            CleanupTempRoot(root);
        }
    }

    [Fact]
    public async Task DownloadQuoteDocument_ShouldForbid_ForOtherSupplier()
    {
        using AppDbContext context = TestDbContextFactory.Create(Guid.NewGuid().ToString());
        string root = CreateTempRoot();
        try
        {
            SeedData seed = await SeedScenarioAsync(context, root);
            using UnitOfWork unitOfWork = new(context);

            DocumentsController controller = CreateController(unitOfWork, root, seed.OtherSupplierUser.Id, ApplicationRoles.Supplier);
            IActionResult result = await controller.DownloadQuoteDocument(seed.QuoteDocument.Id);

            Assert.IsType<ForbidResult>(result);
        }
        finally
        {
            CleanupTempRoot(root);
        }
    }

    private static DocumentsController CreateController(IUnitOfWork unitOfWork, string contentRoot, Guid userId, string role)
    {
        TestWebHostEnvironment hostEnvironment = new(contentRoot);
        DocumentsController controller = new(unitOfWork, hostEnvironment);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = CreatePrincipal(userId, role)
            }
        };

        return controller;
    }

    private static ClaimsPrincipal CreatePrincipal(Guid userId, string role)
    {
        ClaimsIdentity identity = new(
        [
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Role, role)
        ], "TestAuth");

        return new ClaimsPrincipal(identity);
    }

    private static async Task<SeedData> SeedScenarioAsync(AppDbContext context, string root)
    {
        ApplicationUserIdentity buyer = new()
        {
            Id = Guid.NewGuid(),
            UserName = "buyer@test.local",
            Email = "buyer@test.local",
            FirstName = "Buyer",
            LastName = "User",
            CreatedAt = DateTime.UtcNow
        };

        ApplicationUserIdentity supplierUser = new()
        {
            Id = Guid.NewGuid(),
            UserName = "supplier@test.local",
            Email = "supplier@test.local",
            FirstName = "Supplier",
            LastName = "User",
            CreatedAt = DateTime.UtcNow
        };

        ApplicationUserIdentity otherSupplierUser = new()
        {
            Id = Guid.NewGuid(),
            UserName = "supplier.other@test.local",
            Email = "supplier.other@test.local",
            FirstName = "Other",
            LastName = "Supplier",
            CreatedAt = DateTime.UtcNow
        };

        Category category = new()
        {
            Id = Guid.NewGuid(),
            Name = "IT",
            Description = "IT category",
            DisplayOrder = 1,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        Supplier supplier = new()
        {
            Id = Guid.NewGuid(),
            UserId = supplierUser.Id,
            CompanyName = "Supplier Inc",
            ContactName = "Supplier User",
            Email = supplierUser.Email!,
            Status = SupplierStatus.Approved,
            RegisteredAt = DateTime.UtcNow,
            ApprovedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        Supplier otherSupplier = new()
        {
            Id = Guid.NewGuid(),
            UserId = otherSupplierUser.Id,
            CompanyName = "Other Supplier Inc",
            ContactName = "Other Supplier",
            Email = otherSupplierUser.Email!,
            Status = SupplierStatus.Approved,
            RegisteredAt = DateTime.UtcNow,
            ApprovedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        Rfq rfq = new()
        {
            Id = Guid.NewGuid(),
            ReferenceNumber = "RFQ-2026-00001",
            Title = "Test RFQ",
            CategoryId = category.Id,
            Currency = "USD",
            SubmissionDeadline = DateTime.UtcNow.AddDays(7),
            Status = RfqStatus.Published,
            BuyerId = buyer.Id,
            CreatedAt = DateTime.UtcNow,
            PublishedAt = DateTime.UtcNow
        };

        RfqInvitation invitation = new()
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

        Quote quote = new()
        {
            Id = Guid.NewGuid(),
            RfqId = rfq.Id,
            SupplierId = supplier.Id,
            Status = QuoteStatus.Submitted,
            LeadTimeDays = 7,
            ValidUntil = DateTime.UtcNow.AddDays(14),
            TotalAmount = 1000m,
            SubmittedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        RfqDocument rfqDocument = new()
        {
            Id = Guid.NewGuid(),
            RfqId = rfq.Id,
            FileName = "rfq-spec.pdf",
            StoredFileName = "rfq-spec.pdf",
            ContentType = "application/pdf",
            FileSize = 5,
            FilePath = $"/api/v1/documents/rfq/{Guid.NewGuid()}",
            DisplayOrder = 1,
            UploadedById = buyer.Id,
            UploadedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        QuoteDocument quoteDocument = new()
        {
            Id = Guid.NewGuid(),
            QuoteId = quote.Id,
            FileName = "quote.pdf",
            StoredFileName = "quote.pdf",
            ContentType = "application/pdf",
            FileSize = 5,
            FilePath = $"/api/v1/documents/quote/{Guid.NewGuid()}",
            DisplayOrder = 1,
            UploadedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        await context.Users.AddRangeAsync(buyer, supplierUser, otherSupplierUser);
        await context.Categories.AddAsync(category);
        await context.Suppliers.AddRangeAsync(supplier, otherSupplier);
        await context.Rfqs.AddAsync(rfq);
        await context.RfqInvitations.AddAsync(invitation);
        await context.Quotes.AddAsync(quote);
        await context.RfqDocuments.AddAsync(rfqDocument);
        await context.QuoteDocuments.AddAsync(quoteDocument);
        await context.SaveChangesAsync();

        string rfqDir = Path.Combine(root, "App_Data", "uploads", "rfqs", rfq.Id.ToString("N"));
        Directory.CreateDirectory(rfqDir);
        await File.WriteAllTextAsync(Path.Combine(rfqDir, rfqDocument.StoredFileName), "rfq");

        string quoteDir = Path.Combine(root, "App_Data", "uploads", "quotes", quote.Id.ToString("N"));
        Directory.CreateDirectory(quoteDir);
        await File.WriteAllTextAsync(Path.Combine(quoteDir, quoteDocument.StoredFileName), "quote");

        return new SeedData(
            buyer,
            supplierUser,
            otherSupplierUser,
            rfqDocument,
            quoteDocument);
    }

    private static string CreateTempRoot()
    {
        string root = Path.Combine(Path.GetTempPath(), "ProQuoteTests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        return root;
    }

    private static void CleanupTempRoot(string root)
    {
        if (Directory.Exists(root))
        {
            Directory.Delete(root, true);
        }
    }

    private sealed record SeedData(
        ApplicationUserIdentity Buyer,
        ApplicationUserIdentity SupplierUser,
        ApplicationUserIdentity OtherSupplierUser,
        RfqDocument RfqDocument,
        QuoteDocument QuoteDocument);

    private sealed class TestWebHostEnvironment : IWebHostEnvironment
    {
        public TestWebHostEnvironment(string contentRootPath)
        {
            ContentRootPath = contentRootPath;
            ContentRootFileProvider = new PhysicalFileProvider(contentRootPath);
            WebRootPath = Path.Combine(contentRootPath, "wwwroot");
            WebRootFileProvider = new PhysicalFileProvider(contentRootPath);
            ApplicationName = "ProQuote.Tests";
            EnvironmentName = "Development";
        }

        public string ApplicationName { get; set; }
        public IFileProvider WebRootFileProvider { get; set; }
        public string WebRootPath { get; set; }
        public string EnvironmentName { get; set; }
        public string ContentRootPath { get; set; }
        public IFileProvider ContentRootFileProvider { get; set; }
    }
}
