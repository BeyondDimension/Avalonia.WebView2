#if !DISABLE_WEBVIEW2_CORE && (WINDOWS || NETFRAMEWORK)
using Microsoft.Web.WebView2.Core;
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
        var coreWebViewController = webView2?.CoreWebView2Controller;
        return coreWebViewController?.AllowExternalDrop;
    }

    public bool? GetCanGoBack(IWebView2 webView2)
    {
        var coreWebView2 = webView2.PlatformWebView;
        if (coreWebView2 != null)
        {
            return coreWebView2.CanGoBack;
        }

        return null;
    }

    public bool? GetCanGoForward(IWebView2 webView2)
    {
        var coreWebView2 = webView2.PlatformWebView;
        if (coreWebView2 != null)
        {
            return coreWebView2.CanGoForward;
        }

        return null;
    }

    public Color? GetDefaultBackgroundColor(IWebView2 webView2)
    {
        var coreWebViewController = webView2?.CoreWebView2Controller;
        return coreWebViewController?.DefaultBackgroundColor;
    }

    public string? GetHtmlSource(IWebView2 webView2)
    {
        return null;
    }

    public Uri? GetSource(IWebView2 webView2)
    {
        var coreWebView2 = webView2.PlatformWebView;
        if (coreWebView2 != null)
        {
            return new Uri(coreWebView2.Source);
        }
        return null;
    }

    public double? GetZoomFactor(IWebView2 webView2)
    {
        var coreWebViewController = webView2?.CoreWebView2Controller;
        return coreWebViewController?.ZoomFactor;
    }
}
#endif
