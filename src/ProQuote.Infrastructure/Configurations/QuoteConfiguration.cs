using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using ProQuote.Domain.Entities;

namespace ProQuote.Infrastructure.Configurations;

/// <summary>
/// Entity configuration for <see cref="Quote"/>.
/// </summary>
public class QuoteConfiguration : IEntityTypeConfiguration<Quote>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Quote> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        #region Table

        builder.ToTable("Quotes");

        #endregion

        #region Primary Key

        builder.HasKey(q => q.Id);

        #endregion

        #region Properties

        builder.Property(q => q.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(q => q.LeadTimeDays)
            .IsRequired();

        builder.Property(q => q.ValidUntil)
            .IsRequired();

        builder.Property(q => q.PaymentTerms)
            .HasMaxLength(500);

        builder.Property(q => q.Notes)
            .HasMaxLength(4000);

        builder.Property(q => q.TotalAmount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(q => q.SubmittedAt)
            .IsRequired();

        builder.Property(q => q.WithdrawalReason)
            .HasMaxLength(1000);

        builder.Property(q => q.BuyerNotes)
            .HasMaxLength(2000);

        builder.Property(q => q.SubmissionQualityScore);

        builder.Property(q => q.SubmissionCompletenessScore);

        builder.Property(q => q.SubmissionLeadTimeScore);

        builder.Property(q => q.SubmissionCommercialScore);

        builder.Property(q => q.SubmissionQualityScoredAt);

        #endregion

        #region Indexes

        builder.HasIndex(q => q.RfqId);

        builder.HasIndex(q => q.SupplierId);

        builder.HasIndex(q => new { q.RfqId, q.SupplierId })
            .IsUnique();

        builder.HasIndex(q => q.Status);

        builder.HasIndex(q => q.IsAwarded);

        #endregion

        #region Relationships

        builder.HasMany(q => q.LineItems)
            .WithOne(qli => qli.Quote)
            .HasForeignKey(qli => qli.QuoteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(q => q.Documents)
            .WithOne(d => d.Quote)
            .HasForeignKey(d => d.QuoteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(q => q.QualityHistory)
            .WithOne(h => h.Quote)
            .HasForeignKey(h => h.QuoteId)
            .OnDelete(DeleteBehavior.Cascade);

        #endregion
    }
}
