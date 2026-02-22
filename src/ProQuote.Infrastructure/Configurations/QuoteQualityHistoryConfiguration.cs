using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using ProQuote.Domain.Entities;

namespace ProQuote.Infrastructure.Configurations;

/// <summary>
/// Entity configuration for <see cref="QuoteQualityHistory"/>.
/// </summary>
public sealed class QuoteQualityHistoryConfiguration : IEntityTypeConfiguration<QuoteQualityHistory>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<QuoteQualityHistory> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("QuoteQualityHistory");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.OverallScore)
            .IsRequired();

        builder.Property(x => x.CompletenessScore)
            .IsRequired();

        builder.Property(x => x.LeadTimeScore)
            .IsRequired();

        builder.Property(x => x.CommercialScore)
            .IsRequired();

        builder.Property(x => x.EventType)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(x => x.ScoredAt)
            .IsRequired();

        builder.HasIndex(x => x.QuoteId);
        builder.HasIndex(x => x.ScoredAt);
    }
}
