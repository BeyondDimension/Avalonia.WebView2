#if ANDROID
using Android.Webkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.Controls;
partial class WebView2
{
    // https://github.com/dotnet/maui/blob/9.0.70/src/Core/src/Handlers/WebView/WebViewHandler.Android.cs

    public async Task SyncCookieToPlatformWebView(string url)
    {
        var uri = CreateUriForCookies(url);

        if (uri == null)
            return;

        var myCookieJar = Cookies;

        if (myCookieJar == null)
            return;

        InitialCookiePreloadIfNecessary(url);
        var cookies = myCookieJar.GetCookies(uri);

        if (cookies == null)
            return;

        var retrieveCurrentWebCookies = GetCookiesFromPlatformStore(url);

        if (retrieveCurrentWebCookies == null)
            return;

        var cookieManager = CookieManager.Instance;

        if (cookieManager == null)
            return;

        cookieManager.SetAcceptCookie(true);
        for (var i = 0; i < cookies.Count; i++)
        {
            var cookie = cookies[i];
            var cookieString = cookie.ToString();
            cookieManager.SetCookie(cookie.Domain, cookieString);
        }

        foreach (Cookie cookie in retrieveCurrentWebCookies)
        {
            if (cookies[cookie.Name] != null)
                continue;

            var cookieString = $"{cookie.Name}=; max-age=0;expires=Sun, 31 Dec 2017 00:00:00 UTC";
            cookieManager.SetCookie(cookie.Domain, cookieString);
        }

        await Task.CompletedTask;
    }

    public async Task SyncPlatformCookiesToWebView2(string url)
    {
        var myCookieJar = Cookies;

        if (myCookieJar == null)
            return;

        var uri = CreateUriForCookies(url);

        if (uri == null)
            return;

        var cookies = myCookieJar.GetCookies(uri);
        var retrieveCurrentWebCookies = GetCookiesFromPlatformStore(url);

        if (retrieveCurrentWebCookies == null)
            return;

        foreach (Cookie cookie in cookies)
        {
            var platformCookie = retrieveCurrentWebCookies[cookie.Name];

            if (platformCookie == null)
                cookie.Expired = true;
            else
                cookie.Value = platformCookie.Value;
        }

        await SyncCookieToPlatformWebView(url);
    }

    void InitialCookiePreloadIfNecessary(string url)
    {
        var myCookieJar = Cookies;

        if (myCookieJar == null)
            return;

        var uri = CreateUriForCookies(url);

        if (uri == null)
            return;

        if (!_loadedCookies.Add(uri.Host))
            return;

        var cookies = myCookieJar.GetCookies(uri);

        if (cookies != null)
        {
            var existingCookies = GetCookiesFromPlatformStore(url);

            if (existingCookies != null)
            {
                foreach (Cookie cookie in existingCookies)
                {
                    if (cookies[cookie.Name] == null)
                        myCookieJar.Add(cookie);
                }
            }
        }
    }

    static CookieCollection? GetCookiesFromPlatformStore(string url)
    {
        CookieContainer existingCookies = new CookieContainer();
        var cookieManager = CookieManager.Instance;

        if (cookieManager == null)
            return null;

        var currentCookies = cookieManager.GetCookie(url);

        var uri = CreateUriForCookies(url);

        if (uri == null)
            return null;

        if (currentCookies != null)
        {
            foreach (var cookie in currentCookies.Split(';'))
                existingCookies.SetCookies(uri, cookie);
        }

        return existingCookies.GetCookies(uri);
    }
}
#endif
