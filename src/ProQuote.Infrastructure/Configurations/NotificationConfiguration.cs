using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using ProQuote.Domain.Entities;
using ProQuote.Infrastructure.Identity;

namespace ProQuote.Infrastructure.Configurations;

/// <summary>
/// Entity configuration for <see cref="Notification"/>.
/// </summary>
public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        #region Table

        builder.ToTable("Notifications");

        #endregion

        #region Primary Key

        builder.HasKey(n => n.Id);

        #endregion

        #region Properties

        builder.Property(n => n.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(n => n.Message)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(n => n.Type)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(n => n.ActionUrl)
            .HasMaxLength(500);

        builder.Property(n => n.RelatedEntityType)
            .HasMaxLength(100);

        builder.Property(n => n.IsRead)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(n => n.SentAt)
            .IsRequired();

        #endregion

        #region Indexes

        builder.HasIndex(n => n.UserId);

        builder.HasIndex(n => n.IsRead);

        builder.HasIndex(n => n.SentAt);

        builder.HasIndex(n => n.Type);

        builder.HasIndex(n => new { n.UserId, n.IsRead });

        #endregion

        #region Relationships

        builder.HasOne<ApplicationUserIdentity>()
            .WithMany(u => u.Notifications)
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        #endregion

        #region Ignore Domain Properties

        builder.Ignore(n => n.User);

        #endregion
    }
}
