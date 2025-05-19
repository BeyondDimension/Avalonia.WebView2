#if !(WINDOWS || NETFRAMEWORK) && NET8_0_OR_GREATER && !ANDROID && !IOS

using Avalonia.Interactivity;

namespace Avalonia.Controls;

partial class WebView2 : global::Xilium.CefGlue.Avalonia.AvaloniaCefBrowser
{
    internal void CefGuleInitialize()
    {
        SubscribeHandlers();
    }

    internal void CefGuleDispose()
    {
        UnsubscribeHandlers();
    }
}
#endif