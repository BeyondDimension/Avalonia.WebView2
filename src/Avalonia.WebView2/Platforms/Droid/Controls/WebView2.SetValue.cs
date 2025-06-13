#if ANDROID
using BD.Avalonia8.Media;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.Controls;
partial class WebView2
{
    public void SetAllowExternalDrop(IWebView2 webView2, bool allowExternalDrop)
    {
        // not supported on Android WebView
    }

    public void SetDefaultBackgroundColor(IWebView2 webView2, ColorF defaultBackgroundColor)
    {
        if (webView2.PlatformWebView != null)
        {
            webView2.PlatformWebView.SetBackgroundColor(defaultBackgroundColor);
        }
    }

    public void SetHtmlSource(IWebView2 webView2, string? htmlSource)
    {
        var aWebView = webView2.PlatformWebView;
        if (aWebView != null && !string.IsNullOrEmpty(htmlSource))
        {
            aWebView.LoadData(htmlSource, "text/html", "UTF-8");
        }
    }

    public void SetSource(IWebView2 webView2, Uri? source)
    {
        var aWebView = webView2.PlatformWebView;
        if (aWebView != null && source != null)
        {
            aWebView?.SetSource(source);
        }
    }

    public void SetZoomFactor(IWebView2 webView2, double zoomFactor)
    {
        if (zoomFactor >= 0.01d)
        {
            webView2.PlatformWebView?.ZoomBy((float)zoomFactor);
        }
    }
}
#endif