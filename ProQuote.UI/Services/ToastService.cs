namespace ProQuote.UI.Services;

/// <summary>
/// Publishes toast notifications for UI consumers.
/// </summary>
public class ToastService
{
    /// <summary>
    /// Occurs when a toast is added.
    /// </summary>
    public event Action<ToastMessage>? OnToastAdded;

    /// <summary>
    /// Occurs when a toast is removed.
    /// </summary>
    public event Action<Guid>? OnToastRemoved;

    /// <summary>
    /// Shows a success toast.
    /// </summary>
    public void ShowSuccess(string message, string? title = null) => Add("success", message, title);

    /// <summary>
    /// Shows an error toast.
    /// </summary>
    public void ShowError(string message, string? title = null) => Add("error", message, title);

    /// <summary>
    /// Shows a warning toast.
    /// </summary>
    public void ShowWarning(string message, string? title = null) => Add("warning", message, title);

    /// <summary>
    /// Shows an informational toast.
    /// </summary>
    public void ShowInfo(string message, string? title = null) => Add("info", message, title);

    /// <summary>
    /// Removes a toast by identifier.
    /// </summary>
    public void Remove(Guid id) => OnToastRemoved?.Invoke(id);

    private void Add(string type, string message, string? title)
    {
        ToastMessage toast = new() { Type = type, Message = message, Title = title };
        OnToastAdded?.Invoke(toast);
        _ = AutoRemoveAsync(toast.Id);
    }

    private async Task AutoRemoveAsync(Guid id)
    {
        await Task.Delay(4000);
        OnToastRemoved?.Invoke(id);
    }
}
