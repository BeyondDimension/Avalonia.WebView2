#if !DISABLE_WEBVIEW2_CORE && (WINDOWS || NETFRAMEWORK)
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.Controls;
partial class WebView2
{
    // https://github.com/dotnet/maui/blob/9.0.70/src/Core/src/Handlers/WebView/WebViewHandler.Windows.cs

    public async Task SyncCookieToPlatformWebView(string url)
    {
        var uri = CreateUriForCookies(url);

        if (uri is null)
            return;

        var myCookieJar = Cookies;

        if (myCookieJar is null)
            return;

        if (PlatformWebView is null)
        {
            return;
        }

        await InitialCookiePreloadIfNecessary(url);
        var cookies = myCookieJar.GetCookies(uri);

        if (cookies is null)
            return;

        var retrieveCurrentWebCookies = await GetCookiesFromPlatformStore(url);

        foreach (Cookie cookie in cookies)
        {
            var createdCookie = PlatformWebView.CookieManager.CreateCookie(cookie.Name, cookie.Value, cookie.Domain, cookie.Path);
            PlatformWebView.CookieManager.AddOrUpdateCookie(createdCookie);
        }

        foreach (CoreWebView2Cookie cookie in retrieveCurrentWebCookies)
        {
            if (cookies[cookie.Name] is not null)
                continue;

            PlatformWebView.CookieManager.DeleteCookie(cookie);
        }
    }

    public async Task SyncPlatformCookiesToWebView2(string url)
    {
        if (PlatformWebView is null)
            return;

        var myCookieJar = Cookies;

        if (myCookieJar is null)
            return;

        var uri = CreateUriForCookies(url);

        if (uri is null)
            return;

        var cookies = myCookieJar.GetCookies(uri);
        var retrieveCurrentWebCookies = await GetCookiesFromPlatformStore(url);

        var platformCookies = await PlatformWebView.CookieManager.GetCookiesAsync(uri.AbsoluteUri);

        foreach (Cookie cookie in cookies)
        {
            var httpCookie = platformCookies
                .FirstOrDefault(x => x.Name == cookie.Name);

            if (httpCookie is null)
                cookie.Expired = true;
            else
                cookie.Value = httpCookie.Value;
        }

        await SyncCookieToPlatformWebView(url);
    }

    async Task InitialCookiePreloadIfNecessary(string url)
    {
        var myCookieJar = Cookies;

        if (myCookieJar is null)
            return;

        var uri = new Uri(url);

        if (!_loadedCookies.Add(uri.Host))
            return;

        var cookies = myCookieJar.GetCookies(uri);

        if (cookies is not null)
        {
            var existingCookies = await GetCookiesFromPlatformStore(url);

            if (existingCookies.Count == 0)
                return;

            foreach (CoreWebView2Cookie cookie in existingCookies)
            {
                // TODO Ideally we use cookie.ToSystemNetCookie() here, but it's not available for some reason check back later
                if (cookies[cookie.Name] is null)
                    myCookieJar.SetCookies(uri,
                        new Cookie(cookie.Name, cookie.Value, cookie.Path, cookie.Domain)
                        {
                            Expires = cookie.Expires,
                            HttpOnly = cookie.IsHttpOnly,
                            Secure = cookie.IsSecure,
                        }.ToString());
            }
        }
    }

    async Task<IReadOnlyList<CoreWebView2Cookie>> GetCookiesFromPlatformStore(string url)
    {
        if (PlatformWebView is null)
            return Array.Empty<CoreWebView2Cookie>();

        return await PlatformWebView.CookieManager.GetCookiesAsync(url);
    }
}
#endif