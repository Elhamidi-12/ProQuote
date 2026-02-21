using Microsoft.JSInterop;

namespace ProQuote.UI.Services;

/// <summary>
/// Manages the active UI theme state.
/// </summary>
public class ThemeService
{
    private readonly IJSRuntime _jsRuntime;

    /// <summary>
    /// Gets a value indicating whether the dark theme is enabled.
    /// </summary>
    public bool IsDarkMode { get; private set; }

    /// <summary>
    /// Occurs when the theme changes.
    /// </summary>
    public event Action? OnThemeChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="ThemeService"/> class.
    /// </summary>
    /// <param name="jsRuntime">The JavaScript runtime.</param>
    public ThemeService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    /// <summary>
    /// Loads the persisted theme and applies it.
    /// </summary>
    public async Task InitializeAsync()
    {
        string theme = await _jsRuntime.InvokeAsync<string>("pqInterop.getTheme");
        IsDarkMode = string.Equals(theme, "dark", StringComparison.OrdinalIgnoreCase);
        await _jsRuntime.InvokeVoidAsync("pqInterop.setTheme", IsDarkMode);
        OnThemeChanged?.Invoke();
    }

    /// <summary>
    /// Toggles between light and dark themes.
    /// </summary>
    public async Task ToggleThemeAsync()
    {
        IsDarkMode = !IsDarkMode;
        await _jsRuntime.InvokeVoidAsync("pqInterop.setTheme", IsDarkMode);
        OnThemeChanged?.Invoke();
    }
}
