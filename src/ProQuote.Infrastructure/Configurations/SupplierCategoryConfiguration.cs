using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using ProQuote.Domain.Entities;

namespace ProQuote.Infrastructure.Configurations;

/// <summary>
/// Entity configuration for <see cref="SupplierCategory"/>.
/// </summary>
public class SupplierCategoryConfiguration : IEntityTypeConfiguration<SupplierCategory>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<SupplierCategory> builder)
    {
        #region Table

        builder.ToTable("SupplierCategories");

        #endregion

        #region Primary Key (Composite)

        builder.HasKey(sc => new { sc.SupplierId, sc.CategoryId });

        #endregion

        #region Properties

        builder.Property(sc => sc.IsPrimary)
            .IsRequired()
            .HasDefaultValue(false);

        #endregion

        #region Indexes

        builder.HasIndex(sc => sc.SupplierId);

        builder.HasIndex(sc => sc.CategoryId);

        #endregion
    }
}
