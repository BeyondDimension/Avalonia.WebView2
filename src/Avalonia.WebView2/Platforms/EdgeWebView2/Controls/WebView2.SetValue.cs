#if !DISABLE_WEBVIEW2_CORE && (WINDOWS || NETFRAMEWORK)
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
        var coreWebViewController = webView2?.CoreWebView2Controller;
        if (coreWebViewController != null)
        {
            coreWebViewController.AllowExternalDrop = allowExternalDrop;
        }
    }

    public void SetDefaultBackgroundColor(IWebView2 webView2, Color defaultBackgroundColor)
    {
        var coreWebViewController = webView2?.CoreWebView2Controller;
        if (coreWebViewController != null)
        {
            coreWebViewController.DefaultBackgroundColor = defaultBackgroundColor;
        }
    }

    public void SetHtmlSource(IWebView2 webView2, string? htmlSource)
    {
        var coreWebView2 = webView2?.PlatformWebView;
        if (coreWebView2 != null && !string.IsNullOrEmpty(htmlSource))
        {
            coreWebView2.NavigateToString(htmlSource);
        }
    }

    public void SetSource(IWebView2 webView2, Uri? source)
    {
        var coreWebView2 = webView2?.PlatformWebView;
        if (coreWebView2 != null)
        {
            if (source is WebResourceRequestUri webResourceRequest)
            {
                coreWebView2.NavigateWithWebResourceRequest(webResourceRequest.ToRequest(coreWebView2.Environment));
            }
            else
            {
                coreWebView2.Navigate(source!.AbsoluteUri);
            }
        }
    }

    public void SetZoomFactor(IWebView2 webView2, double zoomFactor)
    {
        var coreWebViewController = webView2?.CoreWebView2Controller;
        if (coreWebViewController != null)
        {
            coreWebViewController.ZoomFactor = zoomFactor;
        }
    }
}
#endif