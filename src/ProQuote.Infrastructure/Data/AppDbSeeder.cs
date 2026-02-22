using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using ProQuote.Domain.Entities;
using ProQuote.Domain.Enums;
using ProQuote.Infrastructure.Identity;

namespace ProQuote.Infrastructure.Data;

/// <summary>
/// Database seeder for initializing the database with required and sample data.
/// </summary>
public partial class AppDbSeeder
{
    #region Fields

    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUserIdentity> _userManager;
    private readonly RoleManager<ApplicationRoleIdentity> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AppDbSeeder> _logger;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="AppDbSeeder"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="userManager">The user manager.</param>
    /// <param name="roleManager">The role manager.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="logger">The logger.</param>
    public AppDbSeeder(
        AppDbContext context,
        UserManager<ApplicationUserIdentity> userManager,
        RoleManager<ApplicationRoleIdentity> roleManager,
        IConfiguration configuration,
        ILogger<AppDbSeeder> logger)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _logger = logger;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Seeds the database with initial data.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task SeedAsync(bool includeSampleData = false, bool includeDefaultAdmin = false)
    {
        try
        {
            await _context.Database.MigrateAsync();

            await SeedRolesAsync();
            if (includeDefaultAdmin)
            {
                await SeedAdminUserAsync();
            }

            await SeedCategoriesAsync();

            if (includeSampleData)
            {
                await SeedSampleDataAsync();
            }

            LogDatabaseSeedingCompleted(_logger);
        }
        catch (Exception ex)
        {
            LogDatabaseSeedingFailed(_logger, ex);
            throw;
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Seeds the application roles.
    /// </summary>
    private async Task SeedRolesAsync()
    {
        string[] roles = ApplicationRoles.GetAllRoles();

        foreach (string roleName in roles)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                ApplicationRoleIdentity role = new(roleName)
                {
                    Description = GetRoleDescription(roleName),
                    CreatedAt = DateTime.UtcNow,
                    IsSystemRole = true
                };

                IdentityResult result = await _roleManager.CreateAsync(role);

                if (result.Succeeded)
                {
                    LogRoleCreated(_logger, roleName);
                }
                else
                {
                    LogRoleCreationFailed(_logger, roleName, string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }

    /// <summary>
    /// Seeds the admin user.
    /// </summary>
    private async Task SeedAdminUserAsync()
    {
        string? adminEmail = _configuration["Seed:AdminEmail"];
        string? adminPassword = _configuration["Seed:AdminPassword"];

        if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
        {
            LogAdminUserSeedSkipped(_logger);
            return;
        }

        ApplicationUserIdentity? existingAdmin = await _userManager.FindByEmailAsync(adminEmail);

        if (existingAdmin == null)
        {
            ApplicationUserIdentity adminUser = new()
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                FirstName = "System",
                LastName = "Administrator",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            IdentityResult result = await _userManager.CreateAsync(adminUser, adminPassword);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(adminUser, ApplicationRoles.Admin);
                LogAdminUserCreated(_logger, adminEmail);
            }
            else
            {
                LogAdminUserCreationFailed(_logger, string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }

    /// <summary>
    /// Seeds the categories.
    /// </summary>
    private async Task SeedCategoriesAsync()
    {
        if (await _context.Categories.AnyAsync())
        {
            return;
        }

        List<Category> categories =
        [
            // Top-level categories
            new Category { Id = Guid.NewGuid(), Name = "IT & Technology", Description = "Information technology and software services", DisplayOrder = 1, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Category { Id = Guid.NewGuid(), Name = "Office Supplies", Description = "Office equipment and supplies", DisplayOrder = 2, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Category { Id = Guid.NewGuid(), Name = "Professional Services", Description = "Consulting and professional services", DisplayOrder = 3, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Category { Id = Guid.NewGuid(), Name = "Manufacturing & Industrial", Description = "Industrial equipment and materials", DisplayOrder = 4, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Category { Id = Guid.NewGuid(), Name = "Logistics & Transportation", Description = "Shipping, freight, and logistics services", DisplayOrder = 5, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Category { Id = Guid.NewGuid(), Name = "Facilities & Maintenance", Description = "Facility management and maintenance services", DisplayOrder = 6, IsActive = true, CreatedAt = DateTime.UtcNow }
        ];

        // Add subcategories for IT & Technology
        Guid itCategoryId = categories[0].Id;
        categories.AddRange(
        [
            new Category { Id = Guid.NewGuid(), Name = "Software Development", ParentCategoryId = itCategoryId, DisplayOrder = 1, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Category { Id = Guid.NewGuid(), Name = "Hardware & Equipment", ParentCategoryId = itCategoryId, DisplayOrder = 2, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Category { Id = Guid.NewGuid(), Name = "Cloud Services", ParentCategoryId = itCategoryId, DisplayOrder = 3, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Category { Id = Guid.NewGuid(), Name = "Cybersecurity", ParentCategoryId = itCategoryId, DisplayOrder = 4, IsActive = true, CreatedAt = DateTime.UtcNow }
        ]);

        // Add subcategories for Office Supplies
        Guid officeCategoryId = categories[1].Id;
        categories.AddRange(
        [
            new Category { Id = Guid.NewGuid(), Name = "Furniture", ParentCategoryId = officeCategoryId, DisplayOrder = 1, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Category { Id = Guid.NewGuid(), Name = "Stationery", ParentCategoryId = officeCategoryId, DisplayOrder = 2, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Category { Id = Guid.NewGuid(), Name = "Electronics", ParentCategoryId = officeCategoryId, DisplayOrder = 3, IsActive = true, CreatedAt = DateTime.UtcNow }
        ]);

        await _context.Categories.AddRangeAsync(categories);
        await _context.SaveChangesAsync();

        LogCategoriesSeeded(_logger, categories.Count);
    }

    /// <summary>
    /// Seeds sample data for development/testing.
    /// </summary>
    private async Task SeedSampleDataAsync()
    {
        // Only seed sample data if no RFQs exist
        if (await _context.Rfqs.AnyAsync())
        {
            return;
        }

        // Create sample buyer
        const string buyerEmail = "buyer@example.com";
        ApplicationUserIdentity? buyer = await _userManager.FindByEmailAsync(buyerEmail);

        if (buyer == null)
        {
            buyer = new ApplicationUserIdentity
            {
                UserName = buyerEmail,
                Email = buyerEmail,
                EmailConfirmed = true,
                FirstName = "John",
                LastName = "Buyer",
                JobTitle = "Procurement Manager",
                Department = "Purchasing",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            IdentityResult result = await _userManager.CreateAsync(buyer, "Buyer@123456");
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(buyer, ApplicationRoles.Buyer);
                LogSampleBuyerCreated(_logger, buyerEmail);
            }
        }

        // Create sample suppliers
        List<(string Email, string CompanyName, string ContactName)> supplierData =
        [
            ("supplier1@acmecorp.com", "ACME Corporation", "Alice Smith"),
            ("supplier2@techpro.com", "TechPro Solutions", "Bob Johnson"),
            ("supplier3@globalparts.com", "Global Parts Inc.", "Charlie Brown")
        ];

        List<Supplier> suppliers = [];

        foreach ((string email, string companyName, string contactName) in supplierData)
        {
            ApplicationUserIdentity? supplierUser = await _userManager.FindByEmailAsync(email);

            if (supplierUser == null)
            {
                supplierUser = new ApplicationUserIdentity
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    FirstName = contactName.Split(' ')[0],
                    LastName = contactName.Split(' ').Length > 1 ? contactName.Split(' ')[1] : string.Empty,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                IdentityResult result = await _userManager.CreateAsync(supplierUser, "Supplier@123456");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(supplierUser, ApplicationRoles.Supplier);

                    Supplier supplier = new()
                    {
                        Id = Guid.NewGuid(),
                        UserId = supplierUser.Id,
                        CompanyName = companyName,
                        ContactName = contactName,
                        Email = email,
                        Status = SupplierStatus.Approved,
                        RegisteredAt = DateTime.UtcNow,
                        ApprovedAt = DateTime.UtcNow,
                        CreatedAt = DateTime.UtcNow
                    };

                    suppliers.Add(supplier);
                    LogSampleSupplierCreated(_logger, email);
                }
            }
        }

        if (suppliers.Count > 0)
        {
            await _context.Suppliers.AddRangeAsync(suppliers);
            await _context.SaveChangesAsync();

            // Associate suppliers with categories
            Category? itCategory = await _context.Categories.FirstOrDefaultAsync(c => c.Name == "IT & Technology");
            if (itCategory != null)
            {
                List<SupplierCategory> supplierCategories = suppliers.Select(s => new SupplierCategory
                {
                    SupplierId = s.Id,
                    CategoryId = itCategory.Id,
                    IsPrimary = true
                }).ToList();

                await _context.SupplierCategories.AddRangeAsync(supplierCategories);
            }
        }

        // Create sample RFQ
        Category? category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == "IT & Technology");

        if (buyer != null && category != null)
        {
            Rfq sampleRfq = new()
            {
                Id = Guid.NewGuid(),
                ReferenceNumber = "RFQ-2025-00001",
                Title = "Enterprise Software Development Services",
                Description = "Looking for a qualified vendor to develop a custom enterprise resource planning (ERP) system. The system should include modules for inventory management, HR, and financial reporting.",
                CategoryId = category.Id,
                Currency = "USD",
                SubmissionDeadline = DateTime.UtcNow.AddDays(14),
                Status = RfqStatus.Draft,
                BuyerId = buyer.Id,
                CreatedAt = DateTime.UtcNow
            };

            // Add line items
            sampleRfq.LineItems.Add(new LineItem
            {
                Id = Guid.NewGuid(),
                RfqId = sampleRfq.Id,
                Name = "Requirements Analysis & Design",
                Description = "Complete analysis of business requirements and system design documentation",
                Quantity = 1,
                UnitOfMeasure = "Project",
                TechnicalSpecs = "Deliverables: Business requirements document, System design document, Database schema",
                DisplayOrder = 1,
                CreatedAt = DateTime.UtcNow
            });

            sampleRfq.LineItems.Add(new LineItem
            {
                Id = Guid.NewGuid(),
                RfqId = sampleRfq.Id,
                Name = "Development Phase",
                Description = "Full development of the ERP system including all modules",
                Quantity = 1,
                UnitOfMeasure = "Project",
                TechnicalSpecs = "Modules: Inventory, HR, Finance. Technology: .NET Core, SQL Server",
                DisplayOrder = 2,
                CreatedAt = DateTime.UtcNow
            });

            sampleRfq.LineItems.Add(new LineItem
            {
                Id = Guid.NewGuid(),
                RfqId = sampleRfq.Id,
                Name = "Training & Documentation",
                Description = "End-user training and complete system documentation",
                Quantity = 40,
                UnitOfMeasure = "Hours",
                TechnicalSpecs = "On-site training for up to 20 users, admin guide, user manual",
                DisplayOrder = 3,
                CreatedAt = DateTime.UtcNow
            });

            await _context.Rfqs.AddAsync(sampleRfq);
            await _context.SaveChangesAsync();

            LogSampleRfqCreated(_logger, sampleRfq.ReferenceNumber);
        }
    }

    /// <summary>
    /// Gets the description for a role.
    /// </summary>
    /// <param name="roleName">The role name.</param>
    /// <returns>The role description.</returns>
    private static string GetRoleDescription(string roleName)
    {
        return roleName switch
        {
            ApplicationRoles.Admin => "Full platform access and management capabilities",
            ApplicationRoles.Buyer => "Can create RFQs, invite suppliers, and award contracts",
            ApplicationRoles.Supplier => "Can view RFQ invitations and submit quotes",
            _ => string.Empty
        };
    }

    [LoggerMessage(EventId = 1000, Level = LogLevel.Information, Message = "Database seeding completed successfully")]
    private static partial void LogDatabaseSeedingCompleted(ILogger logger);

    [LoggerMessage(EventId = 1001, Level = LogLevel.Error, Message = "An error occurred while seeding the database")]
    private static partial void LogDatabaseSeedingFailed(ILogger logger, Exception exception);

    [LoggerMessage(EventId = 1002, Level = LogLevel.Information, Message = "Created role: {RoleName}")]
    private static partial void LogRoleCreated(ILogger logger, string roleName);

    [LoggerMessage(EventId = 1003, Level = LogLevel.Warning, Message = "Failed to create role {RoleName}: {Errors}")]
    private static partial void LogRoleCreationFailed(ILogger logger, string roleName, string errors);

    [LoggerMessage(EventId = 1004, Level = LogLevel.Information, Message = "Created admin user: {Email}")]
    private static partial void LogAdminUserCreated(ILogger logger, string email);

    [LoggerMessage(EventId = 1005, Level = LogLevel.Warning, Message = "Failed to create admin user: {Errors}")]
    private static partial void LogAdminUserCreationFailed(ILogger logger, string errors);

    [LoggerMessage(EventId = 1010, Level = LogLevel.Information, Message = "Skipping admin user seed because Seed:AdminEmail/Seed:AdminPassword are not configured")]
    private static partial void LogAdminUserSeedSkipped(ILogger logger);

    [LoggerMessage(EventId = 1006, Level = LogLevel.Information, Message = "Seeded {Count} categories")]
    private static partial void LogCategoriesSeeded(ILogger logger, int count);

    [LoggerMessage(EventId = 1007, Level = LogLevel.Information, Message = "Created sample buyer: {Email}")]
    private static partial void LogSampleBuyerCreated(ILogger logger, string email);

    [LoggerMessage(EventId = 1008, Level = LogLevel.Information, Message = "Created sample supplier: {Email}")]
    private static partial void LogSampleSupplierCreated(ILogger logger, string email);

    [LoggerMessage(EventId = 1009, Level = LogLevel.Information, Message = "Created sample RFQ: {ReferenceNumber}")]
    private static partial void LogSampleRfqCreated(ILogger logger, string referenceNumber);

    #endregion
}
