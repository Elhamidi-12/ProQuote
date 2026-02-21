using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using ProQuote.Domain.Entities;

namespace ProQuote.Infrastructure.Configurations;

/// <summary>
/// Entity configuration for <see cref="RfqInvitation"/>.
/// </summary>
public class RfqInvitationConfiguration : IEntityTypeConfiguration<RfqInvitation>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<RfqInvitation> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        #region Table

        builder.ToTable("RfqInvitations");

        #endregion

        #region Primary Key

        builder.HasKey(i => i.Id);

        #endregion

        #region Properties

        builder.Property(i => i.SecureToken)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(i => i.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(i => i.SentAt)
            .IsRequired();

        builder.Property(i => i.DeclineReason)
            .HasMaxLength(1000);

        builder.Property(i => i.TokenExpiresAt)
            .IsRequired();

        #endregion

        #region Indexes

        builder.HasIndex(i => i.SecureToken)
            .IsUnique();

        builder.HasIndex(i => i.RfqId);

        builder.HasIndex(i => i.SupplierId);

        builder.HasIndex(i => new { i.RfqId, i.SupplierId })
            .IsUnique();

        builder.HasIndex(i => i.Status);

        #endregion
    }
}
