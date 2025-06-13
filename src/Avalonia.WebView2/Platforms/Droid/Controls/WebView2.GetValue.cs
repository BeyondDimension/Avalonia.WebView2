#if ANDROID
using Android.Graphics.Drawables;
using Avalonia.Controls;
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
    public bool? GetAllowExternalDrop(IWebView2 webView2)
    {
        return null;
    }

    public bool? GetCanGoBack(IWebView2 webView2)
    {
        var platformWebView = webView2.PlatformWebView;
        return platformWebView?.CanGoBack() ?? null;
    }

    public bool? GetCanGoForward(IWebView2 webView2)
    {
        var platformWebView = webView2.PlatformWebView;
        return platformWebView?.CanGoForward() ?? null;
    }

    public ColorF? GetDefaultBackgroundColor(IWebView2 webView2)
    {
        var platformWebView = webView2.PlatformWebView;
        if (platformWebView != null && platformWebView.Background is ColorDrawable drawable)
        {
            return drawable.Color;
        }
        return null;
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
        var platformWebView = webView2.PlatformWebView;
        return platformWebView?.GetZ() ?? null;
    }
}
#endif
