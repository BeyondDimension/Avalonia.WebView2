using BD.Avalonia8.Media;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.Controls;
internal interface IWebView2PropertiesGetValue
{
#if (!DISABLE_WEBVIEW2_CORE && (WINDOWS || NETFRAMEWORK)) || (IOS || MACCATALYST || (MACOS && !USE_DEPRECATED_WEBVIEW)) || ANDROID
    bool? GetAllowExternalDrop(IWebView2 webView2);

    bool? GetCanGoBack(IWebView2 webView2);

    bool? GetCanGoForward(IWebView2 webView2);

    ColorF? GetDefaultBackgroundColor(IWebView2 webView2);

    string? GetHtmlSource(IWebView2 webView2);

    Uri? GetSource(IWebView2 webView2);

    double? GetZoomFactor(IWebView2 webView2);
#endif
}
