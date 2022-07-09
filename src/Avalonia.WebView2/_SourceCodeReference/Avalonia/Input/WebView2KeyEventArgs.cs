namespace Avalonia.Input;

public class WebView2KeyEventArgs : KeyEventArgs
{
    public long Timestamp { get; } =
#if NETCOREAPP3_0_OR_GREATER
        Environment.TickCount64;
#else
        Environment.TickCount;
#endif

    public WebView2KeyEventArgs(Key key)
    {
        Device = KeyboardDevice.Instance;
        Key = key;
    }

    public WebView2KeyEventArgs(int virtualKey) : this(KeyInterop.KeyFromVirtualKey(virtualKey))
    {

    }

    /// <inheritdoc cref="KeyInterop.KeyFromVirtualKey(int)"/>
    public static Key KeyFromVirtualKey(int virtualKey) => KeyInterop.KeyFromVirtualKey(virtualKey);
}