using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using ProQuote.Domain.Entities;

namespace ProQuote.Infrastructure.Configurations;

/// <summary>
/// Entity configuration for <see cref="QuoteScoringTemplate" />.
/// </summary>
public sealed class QuoteScoringTemplateConfiguration : IEntityTypeConfiguration<QuoteScoringTemplate>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<QuoteScoringTemplate> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        #region Table

        builder.ToTable("QuoteScoringTemplates");

        #endregion

        #region Primary Key

        builder.HasKey(x => x.Id);

        #endregion

        #region Properties

        builder.Property(x => x.PriceWeight)
            .HasPrecision(5, 2)
            .IsRequired();

        builder.Property(x => x.LeadTimeWeight)
            .HasPrecision(5, 2)
            .IsRequired();

        builder.Property(x => x.CoverageWeight)
            .HasPrecision(5, 2)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        #endregion

        #region Indexes

        builder.HasIndex(x => x.RfqId)
            .IsUnique();

        #endregion

        #region Relationships

        builder.HasOne(x => x.Rfq)
            .WithOne()
            .HasForeignKey<QuoteScoringTemplate>(x => x.RfqId)
            .OnDelete(DeleteBehavior.Cascade);

        #endregion
    }
}
