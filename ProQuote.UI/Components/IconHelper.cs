namespace ProQuote.UI.Components;

internal static class IconHelper
{
    public static string? NormalizeBootstrapIconName(string? icon)
    {
        if (string.IsNullOrWhiteSpace(icon))
        {
            return null;
        }

        icon = icon.Trim();

        return icon.StartsWith("bi-", StringComparison.OrdinalIgnoreCase)
            ? icon[3..]
            : icon;
    }
}
