namespace RFQApp.Domain.Entities;

/// <summary>
/// Represents a document attached to a supplier's quote.
/// </summary>
/// <remarks>
/// Quote documents can include certificates, datasheets, technical documentation,
/// or any supporting files for the supplier's quotation.
/// </remarks>
public class QuoteDocument : BaseEntity
{
    #region Properties

    /// <summary>
    /// Gets or sets the identifier of the quote this document belongs to.
    /// </summary>
    public Guid QuoteId { get; set; }

    /// <summary>
    /// Gets or sets the original file name.
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the stored file name (may be different from original for uniqueness).
    /// </summary>
    public string StoredFileName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the file content type (MIME type).
    /// </summary>
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the file size in bytes.
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// Gets or sets the storage path or URL for the file.
    /// </summary>
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a description of the document.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the document type (e.g., "Certificate", "Datasheet", "Proposal").
    /// </summary>
    public string? DocumentType { get; set; }

    /// <summary>
    /// Gets or sets the display order for sorting documents.
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when the document was uploaded.
    /// </summary>
    public DateTime UploadedAt { get; set; }

    #endregion

    #region Navigation Properties

    /// <summary>
    /// Gets or sets the quote this document belongs to.
    /// </summary>
    public virtual Quote Quote { get; set; } = null!;

    #endregion

    #region Methods

    /// <summary>
    /// Gets the file extension from the original file name.
    /// </summary>
    /// <returns>The file extension including the dot (e.g., ".pdf").</returns>
    public string GetFileExtension()
    {
        return Path.GetExtension(FileName);
    }

    /// <summary>
    /// Gets the file size formatted as a human-readable string.
    /// </summary>
    /// <returns>A formatted string (e.g., "1.5 MB").</returns>
    public string GetFormattedFileSize()
    {
        string[] sizes = ["B", "KB", "MB", "GB"];
        double fileSize = FileSize;
        int order = 0;

        while (fileSize >= 1024 && order < sizes.Length - 1)
        {
            order++;
            fileSize /= 1024;
        }

        return $"{fileSize:0.##} {sizes[order]}";
    }

    #endregion
}
