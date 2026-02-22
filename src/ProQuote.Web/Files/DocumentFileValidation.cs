namespace ProQuote.Web.Files;

/// <summary>
/// Validates supported uploaded document types and maps to safe MIME types.
/// </summary>
public static class DocumentFileValidation
{
    private static readonly Dictionary<string, string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        [".pdf"] = "application/pdf",
        [".doc"] = "application/msword",
        [".docx"] = "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        [".xls"] = "application/vnd.ms-excel",
        [".xlsx"] = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        [".ppt"] = "application/vnd.ms-powerpoint",
        [".pptx"] = "application/vnd.openxmlformats-officedocument.presentationml.presentation",
        [".txt"] = "text/plain",
        [".csv"] = "text/csv",
        [".jpg"] = "image/jpeg",
        [".jpeg"] = "image/jpeg",
        [".png"] = "image/png"
    };

    /// <summary>
    /// Determines whether a file name has an allowed extension and returns a safe MIME type.
    /// </summary>
    /// <param name="fileName">The uploaded file name.</param>
    /// <param name="safeContentType">The mapped MIME type when allowed.</param>
    /// <returns>True when extension is allowed.</returns>
    public static bool TryGetAllowedContentType(string fileName, out string safeContentType)
    {
        string extension = Path.GetExtension(fileName);
        if (string.IsNullOrWhiteSpace(extension))
        {
            safeContentType = string.Empty;
            return false;
        }

        return AllowedExtensions.TryGetValue(extension, out safeContentType!);
    }

    /// <summary>
    /// Normalizes file name for download header usage.
    /// </summary>
    /// <param name="fileName">File name value.</param>
    /// <returns>Sanitized file name.</returns>
    public static string SanitizeFileName(string fileName)
    {
        return Path.GetFileName(string.IsNullOrWhiteSpace(fileName) ? "document" : fileName);
    }
}
