#if IOS || MACCATALYST || (MACOS && !USE_DEPRECATED_WEBVIEW)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.Controls;

partial class WebView2
{
    public event EventHandler<string?>? NavigationStarting;

    public event EventHandler<string?>? NavigationCompleted;

    internal void PlatformWebView_NavigationStarting(object? sender, string? url)
    {
        var navigationStarting = NavigationStarting;
        if (navigationStarting != null)
        {
            navigationStarting(this, url);
        }
    }

    internal void PlatformWebView_NavigationCompleted(object? sender, string? url)
    {
        var navigationCompleted = NavigationCompleted;
        if (navigationCompleted != null)
        {
            navigationCompleted(this, url);
        }

        SetAndRaise(CanGoBackProperty, ref _canGoBack, GetCanGoBack(this) ?? false);
        SetAndRaise(CanGoForwardProperty, ref _canGoForward, GetCanGoForward(this) ?? false);
    }
}
#endif
