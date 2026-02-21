using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using ProQuote.Domain.Entities;
using ProQuote.Infrastructure.Identity;

namespace ProQuote.Infrastructure.Configurations;

/// <summary>
/// Entity configuration for <see cref="AuditLog"/>.
/// </summary>
public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        #region Table

        builder.ToTable("AuditLogs");

        #endregion

        #region Primary Key

        builder.HasKey(a => a.Id);

        #endregion

        #region Properties

        builder.Property(a => a.Action)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.EntityType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.OldValue)
            .HasColumnType("nvarchar(max)");

        builder.Property(a => a.NewValue)
            .HasColumnType("nvarchar(max)");

        builder.Property(a => a.Timestamp)
            .IsRequired();

        builder.Property(a => a.IpAddress)
            .HasMaxLength(50);

        builder.Property(a => a.Details)
            .HasMaxLength(2000);

        #endregion

        #region Indexes

        builder.HasIndex(a => a.RfqId);

        builder.HasIndex(a => a.UserId);

        builder.HasIndex(a => a.Action);

        builder.HasIndex(a => a.Timestamp);

        builder.HasIndex(a => a.EntityType);

        #endregion

        #region Relationships

        builder.HasOne<ApplicationUserIdentity>()
            .WithMany(u => u.AuditLogs)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        #endregion

        #region Ignore Domain Properties

        builder.Ignore(a => a.User);

        #endregion
    }
}
