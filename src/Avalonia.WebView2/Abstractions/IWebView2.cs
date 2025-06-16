using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
#if (!DISABLE_WEBVIEW2_CORE && (WINDOWS || NETFRAMEWORK))
using Microsoft.Web.WebView2.Core;
using PlatformWebViewType = Microsoft.Web.WebView2.Core.CoreWebView2;
#elif ANDROID
using PlatformWebViewType = Android.Webkit.WebView;
#elif IOS || MACCATALYST || (MACOS && !USE_DEPRECATED_WEBVIEW)
using PlatformWebViewType = WebKit.WKWebView;
#endif

namespace Avalonia.Controls;

public interface IWebView2 : IWebView2Properties
{
#if (!DISABLE_WEBVIEW2_CORE && (WINDOWS || NETFRAMEWORK)) || ANDROID || (IOS || MACCATALYST || (MACOS && !USE_DEPRECATED_WEBVIEW))
    PlatformWebViewType? PlatformWebView { get; }
#endif

#if (!DISABLE_WEBVIEW2_CORE && (WINDOWS || NETFRAMEWORK))
    CoreWebView2Controller? CoreWebView2Controller { get; }
#endif

    CookieContainer Cookies { get; }
}