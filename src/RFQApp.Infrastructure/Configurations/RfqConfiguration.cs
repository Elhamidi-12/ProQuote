using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using RFQApp.Domain.Entities;
using RFQApp.Infrastructure.Identity;

namespace RFQApp.Infrastructure.Configurations;

/// <summary>
/// Entity configuration for <see cref="Rfq"/>.
/// </summary>
public class RfqConfiguration : IEntityTypeConfiguration<Rfq>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Rfq> builder)
    {
        #region Table

        builder.ToTable("Rfqs");

        #endregion

        #region Primary Key

        builder.HasKey(r => r.Id);

        #endregion

        #region Properties

        builder.Property(r => r.ReferenceNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(r => r.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(r => r.Description)
            .HasMaxLength(4000);

        builder.Property(r => r.Currency)
            .IsRequired()
            .HasMaxLength(10)
            .HasDefaultValue("USD");

        builder.Property(r => r.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(r => r.CancellationReason)
            .HasMaxLength(1000);

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        #endregion

        #region Indexes

        builder.HasIndex(r => r.ReferenceNumber)
            .IsUnique();

        builder.HasIndex(r => r.Status);

        builder.HasIndex(r => r.BuyerId);

        builder.HasIndex(r => r.SubmissionDeadline);

        #endregion

        #region Relationships

        builder.HasOne<ApplicationUserIdentity>()
            .WithMany(u => u.CreatedRfqs)
            .HasForeignKey(r => r.BuyerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Category)
            .WithMany(c => c.Rfqs)
            .HasForeignKey(r => r.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(r => r.LineItems)
            .WithOne(li => li.Rfq)
            .HasForeignKey(li => li.RfqId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(r => r.Invitations)
            .WithOne(i => i.Rfq)
            .HasForeignKey(i => i.RfqId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(r => r.Documents)
            .WithOne(d => d.Rfq)
            .HasForeignKey(d => d.RfqId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(r => r.Quotes)
            .WithOne(q => q.Rfq)
            .HasForeignKey(q => q.RfqId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(r => r.QaMessages)
            .WithOne(m => m.Rfq)
            .HasForeignKey(m => m.RfqId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(r => r.AuditLogs)
            .WithOne(a => a.Rfq)
            .HasForeignKey(a => a.RfqId)
            .OnDelete(DeleteBehavior.Cascade);

        #endregion

        #region Ignore Domain Properties

        builder.Ignore(r => r.Buyer);

        #endregion
    }
}
