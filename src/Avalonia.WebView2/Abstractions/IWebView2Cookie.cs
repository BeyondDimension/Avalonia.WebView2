using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.Controls;
internal interface IWebView2Cookie
{
#if (!DISABLE_WEBVIEW2_CORE && (WINDOWS || NETFRAMEWORK)) || ANDROID || (IOS || MACCATALYST || (MACOS && !USE_DEPRECATED_WEBVIEW))
    /// <summary>
    /// 将 Cookie 同步到当前系统平台的 WebView 中
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    Task SyncCookieToPlatformWebView(string url);

    /// <summary>
    /// 将当前系统平台的 Cookie 同步到 WebView2 中，并且会再次更新系统平台 WebView 的 Cookie
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    Task SyncPlatformCookiesToWebView2(string url);
#endif
}
