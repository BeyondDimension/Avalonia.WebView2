#if IOS || MACCATALYST || (MACOS && !USE_DEPRECATED_WEBVIEW)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebKit;

namespace Avalonia.Controls;
partial class WebView2
{
    // https://github.com/dotnet/maui/blob/9.0.70/src/Core/src/Handlers/WebView/WebViewHandler.iOS.cs

    public async Task SyncCookieToPlatformWebView(string url)
    {
        var uri = CreateUriForCookies(url);

        if (uri == null)
            return;

        var myCookieJar = Cookies;

        if (myCookieJar == null)
            return;

        await InitialCookiePreloadIfNecessary(url);
        var cookies = myCookieJar.GetCookies(uri);
        if (cookies == null)
            return;

        var retrieveCurrentWebCookies = await GetCookiesFromPlatformStore(url);

        List<NSHttpCookie> deleteCookies = new List<NSHttpCookie>();
        foreach (var cookie in retrieveCurrentWebCookies)
        {
            if (cookies[cookie.Name] != null)
                continue;

            deleteCookies.Add(cookie);
        }

        List<Cookie> cookiesToSet = new List<Cookie>();
        foreach (Cookie cookie in cookies)
        {
            bool changeCookie = true;

            // This code is used to only push updates to cookies that have changed.
            // This doesn't quite work on on iOS 10 if we have to delete any cookies.
            // I haven't found a way on iOS 10 to remove individual cookies. 
            // The trick we use on Android with writing a cookie that expires doesn't work
            // So on iOS10 if the user wants to remove any cookies we just delete 
            // the cookie for the entire domain inside of DeleteCookies and then rewrite
            // all the cookies
            if (OperatingSystem.IsIOSVersionAtLeast(11) || OperatingSystem.IsTvOSVersionAtLeast(11) || deleteCookies.Count == 0)
            {
                foreach (var nsCookie in retrieveCurrentWebCookies)
                {
                    // if the cookie value hasn't changed don't set it again
                    if (nsCookie.Domain == cookie.Domain &&
                        nsCookie.Name == cookie.Name &&
                        nsCookie.Value == cookie.Value)
                    {
                        changeCookie = false;
                        break;
                    }
                }
            }

            if (changeCookie)
                cookiesToSet.Add(cookie);
        }

        await SetCookie(cookiesToSet);
        await DeleteCookies(deleteCookies);
    }

    public async Task SyncPlatformCookiesToWebView2(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return;

        var myCookieJar = Cookies;

        if (myCookieJar == null)
            return;

        var uri = CreateUriForCookies(url);

        if (uri == null)
            return;

        var cookies = myCookieJar.GetCookies(uri);
        var retrieveCurrentWebCookies = await GetCookiesFromPlatformStore(url);

        foreach (var nscookie in retrieveCurrentWebCookies)
        {
            if (cookies[nscookie.Name] == null)
            {
                string cookieH = $"{nscookie.Name}={nscookie.Value}; domain={nscookie.Domain}; path={nscookie.Path}";

                myCookieJar.SetCookies(uri, cookieH);
            }
        }

        foreach (Cookie cookie in cookies)
        {
            NSHttpCookie? nSHttpCookie = null;

            foreach (var findCookie in retrieveCurrentWebCookies)
            {
                if (findCookie.Name == cookie.Name)
                {
                    nSHttpCookie = findCookie;
                    break;
                }
            }

            if (nSHttpCookie == null)
                cookie.Expired = true;
            else
                cookie.Value = nSHttpCookie.Value;
        }

        await SyncCookieToPlatformWebView(url);
    }

    async Task InitialCookiePreloadIfNecessary(string url)
    {
        var myCookieJar = Cookies;

        if (myCookieJar == null)
            return;

        var uri = CreateUriForCookies(url);

        if (uri == null)
            return;

        if (!_loadedCookies.Add(uri.Host))
            return;

        // Pre ios 11 we sync cookies after navigated
        if (!(OperatingSystem.IsIOSVersionAtLeast(11) || OperatingSystem.IsTvOSVersionAtLeast(11)))
            return;

        var cookies = myCookieJar.GetCookies(uri);
        var existingCookies = await GetCookiesFromPlatformStore(url);
        foreach (var nscookie in existingCookies)
        {
            if (cookies[nscookie.Name] == null)
            {
                string cookieH = $"{nscookie.Name}={nscookie.Value}; domain={nscookie.Domain}; path={nscookie.Path}";
                myCookieJar.SetCookies(uri, cookieH);
            }
        }
    }


    async Task<List<NSHttpCookie>> GetCookiesFromPlatformStore(string url)
    {
        NSHttpCookie[]? _initialCookiesLoaded = null;

        if (OperatingSystem.IsIOSVersionAtLeast(11) && PlatformWebView != null)
        {
            _initialCookiesLoaded = await PlatformWebView.Configuration.WebsiteDataStore.HttpCookieStore.GetAllCookiesAsync();
        }
        else
        {
            // TODO: Implement EvaluateJavaScriptAsync.
        }

        _initialCookiesLoaded ??= Array.Empty<NSHttpCookie>();

        List<NSHttpCookie> existingCookies = new List<NSHttpCookie>();

        var uriForCookies = CreateUriForCookies(url);

        if (uriForCookies != null)
        {
            string domain = uriForCookies.Host;
            foreach (var cookie in _initialCookiesLoaded)
            {
                // we don't care that much about this being accurate
                // the cookie container will split the cookies up more correctly
                if (!cookie.Domain.Contains(domain, StringComparison.Ordinal) && !domain.Contains(cookie.Domain, StringComparison.Ordinal))
                    continue;

                existingCookies.Add(cookie);
            }
        }

        return existingCookies;
    }

    async Task SetCookie(List<Cookie> cookies)
    {
        if (PlatformWebView is null)
            return;

        if (OperatingSystem.IsIOSVersionAtLeast(11))
        {
            foreach (var cookie in cookies)
                await PlatformWebView.Configuration.WebsiteDataStore.HttpCookieStore.SetCookieAsync(new NSHttpCookie(cookie));
        }
        else
        {
            PlatformWebView.Configuration.UserContentController.RemoveAllUserScripts();

            if (cookies.Count > 0)
            {
                WKUserScript wKUserScript = new WKUserScript(new NSString(GetCookieString(cookies)), WKUserScriptInjectionTime.AtDocumentStart, false);

                PlatformWebView.Configuration.UserContentController.AddUserScript(wKUserScript);
            }
        }
    }

    async Task DeleteCookies(List<NSHttpCookie> cookies)
    {
        if (PlatformWebView is null)
            return;


        if (OperatingSystem.IsIOSVersionAtLeast(11))
        {
            foreach (var cookie in cookies)
                await PlatformWebView.Configuration.WebsiteDataStore.HttpCookieStore.DeleteCookieAsync(cookie);
        }
        else
        {
            var wKWebsiteDataStore = WKWebsiteDataStore.DefaultDataStore;

            // This is the only way I've found to delete cookies on pre ios 11
            // I tried to set an expired cookie but it doesn't delete the cookie
            // So, just deleting the whole domain is the best option I've found
            WKWebsiteDataStore
                .DefaultDataStore
                .FetchDataRecordsOfTypes(WKWebsiteDataStore.AllWebsiteDataTypes, (NSArray records) =>
                {
                    for (nuint i = 0; i < records.Count; i++)
                    {
                        var record = records.GetItem<WKWebsiteDataRecord>(i);

                        foreach (var deleteme in cookies)
                        {
                            if (record.DisplayName.Contains(deleteme.Domain, StringComparison.Ordinal) || deleteme.Domain.Contains(record.DisplayName, StringComparison.Ordinal))
                            {
                                WKWebsiteDataStore.DefaultDataStore.RemoveDataOfTypes(record.DataTypes,
                                      new[] { record }, () => { });

                                break;
                            }

                        }
                    }
                });
        }
    }
}
#endif
