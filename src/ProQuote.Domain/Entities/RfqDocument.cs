namespace ProQuote.Domain.Entities;

/// <summary>
/// Represents a document attached to an RFQ.
/// </summary>
/// <remarks>
/// RFQ documents can include technical specifications, drawings, requirements,
/// and other files that suppliers need to prepare their quotes.
/// </remarks>
public class RfqDocument : BaseEntity
{
    #region Properties

    /// <summary>
    /// Gets or sets the identifier of the RFQ this document belongs to.
    /// </summary>
    public Guid RfqId { get; set; }

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
    /// Gets or sets the display order for sorting documents.
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who uploaded the document.
    /// </summary>
    public Guid UploadedById { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when the document was uploaded.
    /// </summary>
    public DateTime UploadedAt { get; set; }

    #endregion

    #region Navigation Properties

    /// <summary>
    /// Gets or sets the RFQ this document belongs to.
    /// </summary>
    public virtual Rfq Rfq { get; set; } = null!;

    /// <summary>
    /// Gets or sets the user who uploaded the document.
    /// </summary>
    public virtual ApplicationUser UploadedBy { get; set; } = null!;

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
