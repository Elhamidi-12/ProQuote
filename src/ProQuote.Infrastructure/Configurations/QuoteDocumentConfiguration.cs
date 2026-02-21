using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using ProQuote.Domain.Entities;

namespace ProQuote.Infrastructure.Configurations;

/// <summary>
/// Entity configuration for <see cref="QuoteDocument"/>.
/// </summary>
public class QuoteDocumentConfiguration : IEntityTypeConfiguration<QuoteDocument>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<QuoteDocument> builder)
    {
        #region Table

        builder.ToTable("QuoteDocuments");

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

        builder.Property(d => d.DocumentType)
            .HasMaxLength(100);

        builder.Property(d => d.UploadedAt)
            .IsRequired();

        #endregion

        #region Indexes

        builder.HasIndex(d => d.QuoteId);

        #endregion
    }
}
