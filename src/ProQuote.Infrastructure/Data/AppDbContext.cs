using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using ProQuote.Domain.Entities;
using ProQuote.Infrastructure.Identity;

namespace ProQuote.Infrastructure.Data;

/// <summary>
/// The application database context.
/// </summary>
/// <remarks>
/// This context inherits from IdentityDbContext to support ASP.NET Core Identity
/// and includes all domain entities for the ProQuote.
/// </remarks>
public class AppDbContext : IdentityDbContext<ApplicationUserIdentity, ApplicationRoleIdentity, Guid>
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="AppDbContext"/> class.
    /// </summary>
    /// <param name="options">The database context options.</param>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    #endregion

    #region DbSets

    /// <summary>
    /// Gets or sets the RFQs table.
    /// </summary>
    public DbSet<Rfq> Rfqs => Set<Rfq>();

    /// <summary>
    /// Gets or sets the LineItems table.
    /// </summary>
    public DbSet<LineItem> LineItems => Set<LineItem>();

    /// <summary>
    /// Gets or sets the Quotes table.
    /// </summary>
    public DbSet<Quote> Quotes => Set<Quote>();

    /// <summary>
    /// Gets or sets the QuoteLineItems table.
    /// </summary>
    public DbSet<QuoteLineItem> QuoteLineItems => Set<QuoteLineItem>();

    /// <summary>
    /// Gets or sets the Suppliers table.
    /// </summary>
    public DbSet<Supplier> Suppliers => Set<Supplier>();

    /// <summary>
    /// Gets or sets the RfqInvitations table.
    /// </summary>
    public DbSet<RfqInvitation> RfqInvitations => Set<RfqInvitation>();

    /// <summary>
    /// Gets or sets the QaMessages table.
    /// </summary>
    public DbSet<QaMessage> QaMessages => Set<QaMessage>();

    /// <summary>
    /// Gets or sets the AuditLogs table.
    /// </summary>
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    /// <summary>
    /// Gets or sets the Categories table.
    /// </summary>
    public DbSet<Category> Categories => Set<Category>();

    /// <summary>
    /// Gets or sets the SupplierCategories table.
    /// </summary>
    public DbSet<SupplierCategory> SupplierCategories => Set<SupplierCategory>();

    /// <summary>
    /// Gets or sets the RfqDocuments table.
    /// </summary>
    public DbSet<RfqDocument> RfqDocuments => Set<RfqDocument>();

    /// <summary>
    /// Gets or sets the QuoteDocuments table.
    /// </summary>
    public DbSet<QuoteDocument> QuoteDocuments => Set<QuoteDocument>();

    /// <summary>
    /// Gets or sets the RefreshTokens table.
    /// </summary>
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    /// <summary>
    /// Gets or sets the Notifications table.
    /// </summary>
    public DbSet<Notification> Notifications => Set<Notification>();

    /// <summary>
    /// Gets or sets the quote scoring templates table.
    /// </summary>
    public DbSet<QuoteScoringTemplate> QuoteScoringTemplates => Set<QuoteScoringTemplate>();

    /// <summary>
    /// Gets or sets historical quote quality snapshots.
    /// </summary>
    public DbSet<QuoteQualityHistory> QuoteQualityHistory => Set<QuoteQualityHistory>();

    #endregion

    #region Methods

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        base.OnModelCreating(builder);

        // Apply all configurations from the current assembly
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Rename Identity tables with custom prefix
        builder.Entity<ApplicationUserIdentity>(entity =>
        {
            entity.ToTable("Users");
        });

        builder.Entity<ApplicationRoleIdentity>(entity =>
        {
            entity.ToTable("Roles");
        });

        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserRole<Guid>>(entity =>
        {
            entity.ToTable("UserRoles");
        });

        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserClaim<Guid>>(entity =>
        {
            entity.ToTable("UserClaims");
        });

        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserLogin<Guid>>(entity =>
        {
            entity.ToTable("UserLogins");
        });

        builder.Entity<Microsoft.AspNetCore.Identity.IdentityRoleClaim<Guid>>(entity =>
        {
            entity.ToTable("RoleClaims");
        });

        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserToken<Guid>>(entity =>
        {
            entity.ToTable("UserTokens");
        });
    }

    /// <inheritdoc />
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    /// <summary>
    /// Updates CreatedAt and ModifiedAt timestamps for entities.
    /// </summary>
    private void UpdateTimestamps()
    {
        IEnumerable<Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<BaseEntity>> entries = ChangeTracker
            .Entries<BaseEntity>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<BaseEntity> entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
            }

            entry.Entity.ModifiedAt = DateTime.UtcNow;
        }
    }

    #endregion
}
