#if !(WINDOWS || NETFRAMEWORK) && NET8_0_OR_GREATER && !ANDROID && !IOS && !MACOS

using Xilium.CefGlue.Common.Events;

namespace Avalonia.Controls;
partial class WebView2
{
    /// <summary>
    /// DOMContentLoaded is raised when the initial HTML document has been parsed.
    /// </summary><remarks>
    /// This aligns with the the document's DOMContentLoaded event in HTML.
    /// </remarks>
    public event EventHandler<LoadStartEventArgs>? ContentLoading;
}
#endif
