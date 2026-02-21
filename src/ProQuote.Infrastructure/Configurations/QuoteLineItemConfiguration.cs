using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using ProQuote.Domain.Entities;

namespace ProQuote.Infrastructure.Configurations;

/// <summary>
/// Entity configuration for <see cref="QuoteLineItem"/>.
/// </summary>
public class QuoteLineItemConfiguration : IEntityTypeConfiguration<QuoteLineItem>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<QuoteLineItem> builder)
    {
        #region Table

        builder.ToTable("QuoteLineItems");

        #endregion

        #region Primary Key

        builder.HasKey(qli => qli.Id);

        #endregion

        #region Properties

        builder.Property(qli => qli.UnitPrice)
            .IsRequired()
            .HasPrecision(18, 4);

        builder.Property(qli => qli.TotalPrice)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(qli => qli.Notes)
            .HasMaxLength(1000);

        #endregion

        #region Indexes

        builder.HasIndex(qli => qli.QuoteId);

        builder.HasIndex(qli => qli.LineItemId);

        builder.HasIndex(qli => new { qli.QuoteId, qli.LineItemId })
            .IsUnique();

        #endregion
    }
}
