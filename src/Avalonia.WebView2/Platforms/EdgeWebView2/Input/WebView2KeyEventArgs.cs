#if !DISABLE_WEBVIEW2_CORE && (WINDOWS || NETFRAMEWORK)
namespace Avalonia.Input;

partial class WebView2KeyEventArgs
{
    public WebView2KeyEventArgs(int virtualKey) : this(KeyInterop.KeyFromVirtualKey(virtualKey))
    {

    }

    public WebView2KeyEventArgs(uint virtualKey) : this(unchecked((int)virtualKey))
    {

    }

    /// <summary>
    /// Convert a Win32 VirtualKey into our Key enum.
    /// </summary>
    /// <param name="virtualKey"></param>
    /// <returns></returns>
    public static Key KeyFromVirtualKey(int virtualKey) => KeyInterop.KeyFromVirtualKey(virtualKey);
}
#endif