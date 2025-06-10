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
    public void SetAllowExternalDrop(IWebView2 webView2, bool allowExternalDrop)
    {
        // Not supported on iOS/macOS/MACCATALYST WebView2
    }

    public void SetDefaultBackgroundColor(IWebView2 webView2, Color defaultBackgroundColor)
    {
        var wkWebView = webView2?.PlatformWebView;
        if (wkWebView != null)
        {
#if IOS
            wkWebView.BackgroundColor = defaultBackgroundColor.ToPlatform();
#elif MACCATALYST || (MACOS && !USE_DEPRECATED_WEBVIEW)
            wkWebView.UnderPageBackgroundColor = defaultBackgroundColor.ToPlatform();
#endif
        }
    }

    public void SetHtmlSource(IWebView2 webView2, string? htmlSource)
    {
        var wkWebView = webView2.PlatformWebView;
        if (wkWebView != null && !string.IsNullOrEmpty(htmlSource))
        {
            wkWebView.LoadHtmlString(htmlSource, (NSUrl?)null!);
        }
    }

    public void SetSource(IWebView2 webView2, Uri? source)
    {
        var wkWebView = webView2.PlatformWebView;
        wkWebView?.SetSource(source);
    }

    public void SetZoomFactor(IWebView2 webView2, double zoomFactor)
    {
        var wkWebView = webView2.PlatformWebView;
        wkWebView?.PageZoom = ((float)zoomFactor);
    }
}
#endif