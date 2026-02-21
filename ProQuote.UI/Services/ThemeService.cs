using Microsoft.JSInterop;

namespace ProQuote.UI.Services;

public class ThemeService
{
    private readonly IJSRuntime _jsRuntime;
    public bool IsDarkMode { get; private set; }

    public event Action? OnThemeChanged;

    public ThemeService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task InitializeAsync()
    {
        string theme = await _jsRuntime.InvokeAsync<string>("pqInterop.getTheme");
        IsDarkMode = string.Equals(theme, "dark", StringComparison.OrdinalIgnoreCase);
        await _jsRuntime.InvokeVoidAsync("pqInterop.setTheme", IsDarkMode);
        OnThemeChanged?.Invoke();
    }

    public async Task ToggleTheme()
    {
        IsDarkMode = !IsDarkMode;
        await _jsRuntime.InvokeVoidAsync("pqInterop.setTheme", IsDarkMode);
        OnThemeChanged?.Invoke();
    }
}
