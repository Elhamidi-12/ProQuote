using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using ProQuote.Domain.Entities;
using ProQuote.Infrastructure.Identity;

namespace ProQuote.Infrastructure.Configurations;

/// <summary>
/// Entity configuration for <see cref="Supplier"/>.
/// </summary>
public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        #region Table

        builder.ToTable("Suppliers");

        #endregion

        #region Primary Key

        builder.HasKey(s => s.Id);

        #endregion

        #region Properties

        builder.Property(s => s.CompanyName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.ContactName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(s => s.Phone)
            .HasMaxLength(50);

        builder.Property(s => s.Website)
            .HasMaxLength(500);

        builder.Property(s => s.Address)
            .HasMaxLength(500);

        builder.Property(s => s.City)
            .HasMaxLength(100);

        builder.Property(s => s.Country)
            .HasMaxLength(100);

        builder.Property(s => s.TaxId)
            .HasMaxLength(50);

        builder.Property(s => s.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(s => s.AverageRating)
            .HasPrecision(3, 2);

        builder.Property(s => s.StatusReason)
            .HasMaxLength(1000);

        #endregion

        #region Indexes

        builder.HasIndex(s => s.UserId)
            .IsUnique();

        builder.HasIndex(s => s.Email);

        builder.HasIndex(s => s.CompanyName);

        builder.HasIndex(s => s.Status);

        #endregion

        #region Relationships

        builder.HasOne<ApplicationUserIdentity>()
            .WithOne(u => u.Supplier)
            .HasForeignKey<Supplier>(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.Categories)
            .WithOne(sc => sc.Supplier)
            .HasForeignKey(sc => sc.SupplierId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.Quotes)
            .WithOne(q => q.Supplier)
            .HasForeignKey(q => q.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(s => s.Invitations)
            .WithOne(i => i.Supplier)
            .HasForeignKey(i => i.SupplierId)
            .OnDelete(DeleteBehavior.Cascade);

        #endregion

        #region Ignore Domain Properties

        builder.Ignore(s => s.User);

        #endregion
    }
}
