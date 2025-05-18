#if !(WINDOWS || NETFRAMEWORK) && NET8_0_OR_GREATER && !ANDROID && !IOS

namespace Avalonia.Controls;

partial class WebView2 : global::Xilium.CefGlue.Avalonia.AvaloniaCefBrowser
{
}
#endif