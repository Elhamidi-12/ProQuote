namespace ProQuote.UI.Services;

/// <summary>
/// Represents a toast notification payload.
/// </summary>
public sealed class ToastMessage
{
    /// <summary>
    /// Gets the unique toast identifier.
    /// </summary>
    public Guid Id { get; init; } = Guid.NewGuid();

    /// <summary>
    /// Gets the toast type (success, error, warning, info).
    /// </summary>
    public string Type { get; init; } = "info";

    /// <summary>
    /// Gets the toast message.
    /// </summary>
    public string Message { get; init; } = string.Empty;

    /// <summary>
    /// Gets the optional toast title.
    /// </summary>
    public string? Title { get; init; }
}
