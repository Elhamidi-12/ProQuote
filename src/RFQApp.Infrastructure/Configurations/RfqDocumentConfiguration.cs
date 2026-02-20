using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using RFQApp.Domain.Entities;
using RFQApp.Infrastructure.Identity;

namespace RFQApp.Infrastructure.Configurations;

/// <summary>
/// Entity configuration for <see cref="RfqDocument"/>.
/// </summary>
public class RfqDocumentConfiguration : IEntityTypeConfiguration<RfqDocument>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<RfqDocument> builder)
    {
        #region Table

        builder.ToTable("RfqDocuments");

        #endregion

        #region Primary Key

        builder.HasKey(d => d.Id);

        #endregion

        #region Properties

        builder.Property(d => d.FileName)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(d => d.StoredFileName)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(d => d.ContentType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.FileSize)
            .IsRequired();

        builder.Property(d => d.FilePath)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(d => d.Description)
            .HasMaxLength(500);

        builder.Property(d => d.UploadedAt)
            .IsRequired();

        #endregion

        #region Indexes

        builder.HasIndex(d => d.RfqId);

        builder.HasIndex(d => d.UploadedById);

        #endregion

        #region Relationships

        builder.HasOne<ApplicationUserIdentity>()
            .WithMany(u => u.UploadedDocuments)
            .HasForeignKey(d => d.UploadedById)
            .OnDelete(DeleteBehavior.Restrict);

        #endregion

        #region Ignore Domain Properties

        builder.Ignore(d => d.UploadedBy);

        #endregion
    }
}
