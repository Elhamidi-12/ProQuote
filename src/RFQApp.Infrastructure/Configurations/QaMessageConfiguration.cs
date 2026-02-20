using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using RFQApp.Domain.Entities;
using RFQApp.Infrastructure.Identity;

namespace RFQApp.Infrastructure.Configurations;

/// <summary>
/// Entity configuration for <see cref="QaMessage"/>.
/// </summary>
public class QaMessageConfiguration : IEntityTypeConfiguration<QaMessage>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<QaMessage> builder)
    {
        #region Table

        builder.ToTable("QaMessages");

        #endregion

        #region Primary Key

        builder.HasKey(m => m.Id);

        #endregion

        #region Properties

        builder.Property(m => m.Message)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(m => m.SentAt)
            .IsRequired();

        builder.Property(m => m.IsFromBuyer)
            .IsRequired();

        builder.Property(m => m.IsRead)
            .IsRequired()
            .HasDefaultValue(false);

        #endregion

        #region Indexes

        builder.HasIndex(m => m.RfqId);

        builder.HasIndex(m => m.SenderId);

        builder.HasIndex(m => m.TargetSupplierId);

        builder.HasIndex(m => m.ParentMessageId);

        builder.HasIndex(m => m.SentAt);

        #endregion

        #region Relationships

        builder.HasOne<ApplicationUserIdentity>()
            .WithMany(u => u.QaMessages)
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.TargetSupplier)
            .WithMany()
            .HasForeignKey(m => m.TargetSupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.ParentMessage)
            .WithMany(m => m.Replies)
            .HasForeignKey(m => m.ParentMessageId)
            .OnDelete(DeleteBehavior.Restrict);

        #endregion

        #region Ignore Domain Properties

        builder.Ignore(m => m.Sender);

        #endregion
    }
}
