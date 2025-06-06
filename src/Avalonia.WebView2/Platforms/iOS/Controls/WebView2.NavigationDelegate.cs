#if IOS || MACOS || MACCATALYST

using WebKit;

namespace Avalonia.Controls;


// Use interface instead of base class here
// https://developer.apple.com/documentation/webkit/wknavigationdelegate/1455641-webview?language=objc#discussion
// The newer policy method is implemented in the base class
// and the doc remarks state the older policy method is not called
// if the newer one is implemented, but the new one is v13+
// so we'd like to stick with the older one for now
public class WebView2NavigationDelegate : WKNavigationDelegate
{
    readonly WeakReference<WebView2> _webView2;

    public WebView2NavigationDelegate(WebView2 webView2)
    {
        _ = webView2 ?? throw new ArgumentNullException(nameof(webView2));
        _webView2 = new WeakReference<WebView2>(webView2);
    }

    // https://github.com/dotnet/maui/blob/9.0.70/src/Core/src/Platform/iOS/MauiWebViewNavigationDelegate.cs
    [Export("webView:didFinishNavigation:")]
    public override void DidFinishNavigation(WKWebView webView, WKNavigation navigation)
    {
        var webView2 = WebView2;
        var requestUri = GetCurrentUrl();
#if DEBUG
        Console.WriteLine($"WebView2 DidFinishNavigation: {requestUri}");
#endif
        if (webView2 is not null)
        {
            webView2.NavigationDelegate_DidFinishNavigation(null, requestUri);
        }
    }

    [Export("webView:didFailNavigation:withError:")]
    public override void DidFailNavigation(WKWebView webView, WKNavigation navigation, NSError error)
    {
        base.DidFailNavigation(webView, navigation, error);
    }

    [Export("webView:didFailProvisionalNavigation:withError:")]
    public override void DidFailProvisionalNavigation(WKWebView webView, WKNavigation navigation, NSError error)
    {
        base.DidFailProvisionalNavigation(webView, navigation, error);
    }

    //// https://stackoverflow.com/questions/37509990/migrating-from-uiwebview-to-wkwebview
    //[Export("webView:decidePolicyForNavigationAction:decisionHandler:")]
    //public override void DecidePolicy(WKWebView webView, WKNavigationAction navigationAction, Action<WKNavigationActionPolicy> decisionHandler)
    //{
    //    base.DecidePolicy(webView, navigationAction, decisionHandler);
    //}

    string GetCurrentUrl()
    {
        return WebView2?.WKWebView?.Url?.AbsoluteUrl?.ToString() ?? string.Empty;
    }

    //internal WebNavigationEvent CurrentNavigationEvent
    //{
    //    get;
    //    set;
    //}


    WebView2? WebView2
    {
        get
        {
            if (_webView2.TryGetTarget(out var webView2))
            {
                return webView2;
            }

            return null;
        }
    }
}
#endif