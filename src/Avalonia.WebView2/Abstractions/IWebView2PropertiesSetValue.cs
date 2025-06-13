using BD.Avalonia8.Media;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.Controls;
internal interface IWebView2PropertiesSetValue
{
#if (!DISABLE_WEBVIEW2_CORE && (WINDOWS || NETFRAMEWORK)) || (IOS || MACCATALYST || (MACOS && !USE_DEPRECATED_WEBVIEW)) || ANDROID
    void SetAllowExternalDrop(IWebView2 webView2, bool allowExternalDrop);

    void SetDefaultBackgroundColor(IWebView2 webView2, ColorF defaultBackgroundColor);

    void SetHtmlSource(IWebView2 webView2, string? htmlSource);

    void SetSource(IWebView2 webView2, Uri? source);

    void SetZoomFactor(IWebView2 webView2, double zoomFactor);
#endif
}
