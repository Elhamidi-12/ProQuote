namespace ProQuote.UI.Services;

public class ToastService
{
    public sealed class ToastMessage
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public string Type { get; init; } = "info";
        public string Message { get; init; } = string.Empty;
        public string? Title { get; init; }
    }

    public event Action<ToastMessage>? OnToastAdded;
    public event Action<Guid>? OnToastRemoved;

    public void ShowSuccess(string message, string? title = null) => Add("success", message, title);
    public void ShowError(string message, string? title = null) => Add("error", message, title);
    public void ShowWarning(string message, string? title = null) => Add("warning", message, title);
    public void ShowInfo(string message, string? title = null) => Add("info", message, title);

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
