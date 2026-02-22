namespace ProQuote.Web.Auth;

/// <summary>
/// Stores a one-time post-login success message across internal navigation.
/// </summary>
public sealed class PostLoginNotificationStore
{
    private string? _pendingSuccessMessage;

    /// <summary>
    /// Stores the success message to be shown on the next rendered authenticated layout.
    /// </summary>
    /// <param name="message">Success message text.</param>
    public void SetSuccess(string message)
    {
        _pendingSuccessMessage = message;
    }

    /// <summary>
    /// Returns and clears the pending success message if one exists.
    /// </summary>
    /// <param name="message">Dequeued success message.</param>
    /// <returns><c>true</c> when a message was available; otherwise <c>false</c>.</returns>
    public bool TryDequeueSuccess(out string message)
    {
        if (string.IsNullOrWhiteSpace(_pendingSuccessMessage))
        {
            message = string.Empty;
            return false;
        }

        message = _pendingSuccessMessage;
        _pendingSuccessMessage = null;
        return true;
    }
}
