using System.Net;
using System.Text;

namespace Avalonia.Controls;

partial class WebView2
{
    /// <summary>
    /// The <see cref="AvaloniaProperty" /> which backs the <see cref="HtmlSource" /> property.
    /// </summary>
    public static readonly DirectProperty<WebView2, CookieContainer?> CookiesProperty = AvaloniaProperty.RegisterDirect<WebView2, CookieContainer?>(nameof(HtmlSource), x => x.Cookies);

    public CookieContainer Cookies
    {
        get { return (CookieContainer)GetValue(CookiesProperty); }
        set { SetValue(CookiesProperty, value); }
    }

    readonly HashSet<string> _loadedCookies = new HashSet<string>();

    static Uri? CreateUriForCookies(string url)
    {
        if (url == null)
            return null;

        Uri? uri;

        if (url.Length > 2000)
            url = url[..2000];

        if (Uri.TryCreate(url, UriKind.Absolute, out uri))
        {
            if (string.IsNullOrWhiteSpace(uri.Host))
                return null;

            return uri;
        }

        return null;
    }

    internal bool HasCookiesToLoad(string url)
    {
        var uri = CreateUriForCookies(url);

        if (uri == null)
            return false;

        var myCookieJar = Cookies;

        if (myCookieJar == null)
            return false;

        var cookies = myCookieJar.GetCookies(uri);

        if (cookies == null)
            return false;

        return cookies.Count > 0;
    }

    static string GetCookieString(List<Cookie> existingCookies)
    {
        StringBuilder cookieBuilder = new StringBuilder();
        foreach (Cookie jCookie in existingCookies)
        {
            cookieBuilder.Append("document.cookie = '");
            cookieBuilder.Append(jCookie.Name);
            cookieBuilder.Append('=');

            if (jCookie.Expired)
            {
                cookieBuilder.Append($"; Max-Age=0");
                cookieBuilder.Append($"; expires=Sun, 31 Dec 2000 00:00:00 UTC");
            }
            else
            {
                cookieBuilder.Append(jCookie.Value);
                cookieBuilder.Append($"; Max-Age={jCookie.Expires.Subtract(DateTime.UtcNow).TotalSeconds}");
            }

            if (!String.IsNullOrWhiteSpace(jCookie.Domain))
            {
                cookieBuilder.Append($"; Domain={jCookie.Domain}");
            }
            if (!String.IsNullOrWhiteSpace(jCookie.Domain))
            {
                cookieBuilder.Append($"; Path={jCookie.Path}");
            }
            if (jCookie.Secure)
            {
                cookieBuilder.Append($"; Secure");
            }
            if (jCookie.HttpOnly)
            {
                cookieBuilder.Append($"; HttpOnly");
            }

            cookieBuilder.Append("';");
        }

        return cookieBuilder.ToString();
    }
}