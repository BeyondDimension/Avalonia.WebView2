#if IOS || MACCATALYST || (MACOS && !USE_DEPRECATED_WEBVIEW)
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebKit;

namespace Avalonia.Controls;
partial class WebView2
{
    public bool? GetAllowExternalDrop(IWebView2 webView2)
    {
        return null;
    }

    public bool? GetCanGoBack(IWebView2 webView2)
    {
        var platformWebView = webView2.PlatformWebView;
        if (platformWebView == null)
            return null;
        return platformWebView.CanGoBack;
    }

    public bool? GetCanGoForward(IWebView2 webView2)
    {
        var platformWebView = webView2.PlatformWebView;
        return platformWebView != null ? platformWebView.CanGoForward : null;
    }

    public Color? GetDefaultBackgroundColor(IWebView2 webView2)
    {
        var platformWebView = webView2?.PlatformWebView;
        if (platformWebView == null)
            return null;

#if IOS
        return platformWebView.BackgroundColor.AsColor();
#elif MACCATALYST || (MACOS && !USE_DEPRECATED_WEBVIEW)
        return platformWebView.UnderPageBackgroundColor.AsColor();
#endif

    }

    public string? GetHtmlSource(IWebView2 webView2)
    {
        return null;
    }

    public Uri? GetSource(IWebView2 webView2)
    {
        return null;
    }

    public double? GetZoomFactor(IWebView2 webView2)
    {
        var platformWebView = webView2?.PlatformWebView;
        return platformWebView?.PageZoom ?? null;
    }
}
#endif
