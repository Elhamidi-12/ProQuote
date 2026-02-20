using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using RFQApp.Domain.Entities;
using RFQApp.Infrastructure.Identity;

namespace RFQApp.Infrastructure.Configurations;

/// <summary>
/// Entity configuration for <see cref="RefreshToken"/>.
/// </summary>
public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        #region Table

        builder.ToTable("RefreshTokens");

        #endregion

        #region Primary Key

        builder.HasKey(rt => rt.Id);

        #endregion

        #region Properties

        builder.Property(rt => rt.Token)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(rt => rt.JwtId)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(rt => rt.IsUsed)
            .IsRequired();

        builder.Property(rt => rt.IsRevoked)
            .IsRequired();

        builder.Property(rt => rt.ExpiresAt)
            .IsRequired();

        builder.Property(rt => rt.CreatedByIp)
            .HasMaxLength(50);

        builder.Property(rt => rt.RevokedByIp)
            .HasMaxLength(50);

        builder.Property(rt => rt.ReplacedByToken)
            .HasMaxLength(256);

        builder.Property(rt => rt.ReasonRevoked)
            .HasMaxLength(500);

        #endregion

        #region Indexes

        builder.HasIndex(rt => rt.Token);

        builder.HasIndex(rt => rt.UserId);

        builder.HasIndex(rt => rt.JwtId);

        #endregion

        #region Relationships

        builder.HasOne<ApplicationUserIdentity>()
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        #endregion

        #region Ignore Domain Properties

        builder.Ignore(rt => rt.User);
        builder.Ignore(rt => rt.IsExpired);
        builder.Ignore(rt => rt.IsActive);

        #endregion
    }
}
