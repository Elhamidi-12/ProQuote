using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using ProQuote.Domain.Entities;

namespace ProQuote.Infrastructure.Configurations;

/// <summary>
/// Entity configuration for <see cref="LineItem"/>.
/// </summary>
public class LineItemConfiguration : IEntityTypeConfiguration<LineItem>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<LineItem> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        #region Table

        builder.ToTable("LineItems");

        #endregion

        #region Primary Key

        builder.HasKey(li => li.Id);

        #endregion

        #region Properties

        builder.Property(li => li.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(li => li.Description)
            .HasMaxLength(2000);

        builder.Property(li => li.Quantity)
            .IsRequired()
            .HasPrecision(18, 4);

        builder.Property(li => li.UnitOfMeasure)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(li => li.TechnicalSpecs)
            .HasMaxLength(4000);

        builder.Property(li => li.DisplayOrder)
            .IsRequired();

        #endregion

        #region Indexes

        builder.HasIndex(li => li.RfqId);

        builder.HasIndex(li => new { li.RfqId, li.DisplayOrder });

        #endregion

        #region Relationships

        builder.HasMany(li => li.QuoteLineItems)
            .WithOne(qli => qli.LineItem)
            .HasForeignKey(qli => qli.LineItemId)
            .OnDelete(DeleteBehavior.Restrict);

        #endregion
    }
}
