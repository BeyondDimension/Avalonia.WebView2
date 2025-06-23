#if IOS || MACOS || MACCATALYST
using WebKit;

namespace Avalonia.Controls;

partial class WebView2
{
    /// <summary>
    /// Use interface instead of base class here
    /// https://developer.apple.com/documentation/webkit/wknavigationdelegate/1455641-webview?language=objc#discussion
    /// The newer policy method is implemented in the base class
    /// and the doc remarks state the older policy method is not called
    /// if the newer one is implemented, but the new one is v13+
    /// so we'd like to stick with the older one for now
    /// https://github.com/dotnet/maui/blob/9.0.70/src/Core/src/Platform/iOS/MauiWebViewNavigationDelegate.cs#L14
    /// </summary>
    public partial class WKNavigationDelegate(Handler handler) : NSObject, IWKNavigationDelegate
    {
        readonly WeakReference<Handler> handler = new(handler);
        //WebView2NavigationEvent _lastEvent;

        /// <summary>
        /// 注入 JS 脚本
        /// </summary>
        /// <param name="platformView"></param>
        /// <param name="virtualView"></param>
        /// <param name="url"></param>
        protected virtual void InjectJavaScript(WKWebView platformView, WebView2 virtualView, string url)
        {
            var js = HandlerStorageServiceGenerateJSString(virtualView.StorageService, url);
#if DEBUG
            Console.WriteLine($"NavigationDelegate_DidFinishNavigationEvent js: {js}");
#endif
            if (js != null)
            {
                WKJavascriptEvaluationResult completionHandler = null!;
                platformView.EvaluateJavaScript(js, completionHandler);
            }
        }

        [Export("webView:didCommitNavigation:")]
        public virtual void DidCommitNavigation(WKWebView webView, WKNavigation navigation)
        {
            var handler = Handler;

            if (handler is null)
                return;

            var url = webView.GetCurrentUrl();
            handler.VirtualView.PlatformWebView_NavigationStarting(webView, url);
        }

        [Export("webView:didFinishNavigation:")]
        public virtual void DidFinishNavigation(WKWebView webView, WKNavigation navigation)
        {
            var handler = Handler;

            if (handler is null /*|| !handler.IsConnected()*/)
                return;

            var platformView = handler?.PlatformView;
            var virtualView = handler?.VirtualView;

            if (platformView is null || virtualView is null)
                return;

            //platformView.UpdateCanGoBackForward(virtualView);

            if (webView.IsLoading)
                return;

            var url = webView.GetCurrentUrl();

            virtualView.SyncPlatformCookiesToWebView2(url).FireAndForget();
            //virtualView.Navigated(_lastEvent, url, WebView2NavigationResult.Success);

#if DEBUG
            Console.WriteLine($"WebView2 DidFinishNavigation: {url}");
#endif
            if (!string.IsNullOrWhiteSpace(url))
            {
                InjectJavaScript(platformView, virtualView, url);
            }

            virtualView.PlatformWebView_NavigationStarting(webView, url);
        }

        [Export("webView:didFailNavigation:withError:")]
        public virtual void DidFailNavigation(WKWebView webView, WKNavigation navigation, NSError error)
        {
            //var handler = Handler;

            //if (handler is null || !handler.IsConnected())
            //    return;

            //var platformView = handler?.PlatformView;
            //var virtualView = handler?.VirtualView;

            //if (platformView is null || virtualView is null)
            //    return;

            //var url = GetCurrentUrl();

            //virtualView.Navigated(_lastEvent, url, WebNavigationResult.Failure);

            //platformView.UpdateCanGoBackForward(virtualView);
        }

        [Export("webView:didFailProvisionalNavigation:withError:")]
        public void DidFailProvisionalNavigation(WKWebView webView, WKNavigation navigation, NSError error)
        {
            //var handler = Handler;

            //if (handler is null || !handler.IsConnected())
            //    return;

            //var platformView = handler?.PlatformView;
            //var virtualView = handler?.VirtualView;

            //if (platformView is null || virtualView is null)
            //    return;

            //var url = GetCurrentUrl();

            //virtualView.Navigated(_lastEvent, url, WebNavigationResult.Failure);

            //platformView.UpdateCanGoBackForward(virtualView);
        }

        //// https://stackoverflow.com/questions/37509990/migrating-from-uiwebview-to-wkwebview
        //[Export("webView:decidePolicyForNavigationAction:decisionHandler:")]
        //public void DecidePolicy(WKWebView webView, WKNavigationAction navigationAction, Action<WKNavigationActionPolicy> decisionHandler)
        //{
        //    var handler = Handler;

        //    if (handler is null || !handler.IsConnected())
        //    {
        //        decisionHandler.Invoke(WKNavigationActionPolicy.Cancel);
        //        return;
        //    }

        //    var platformView = handler?.PlatformView;
        //    var virtualView = handler?.VirtualView;

        //    if (platformView is null || virtualView is null)
        //    {
        //        decisionHandler.Invoke(WKNavigationActionPolicy.Cancel);
        //        return;
        //    }

        //    var navEvent = WebNavigationEvent.NewPage;
        //    var navigationType = navigationAction.NavigationType;

        //    switch (navigationType)
        //    {
        //        case WKNavigationType.LinkActivated:
        //            navEvent = WebNavigationEvent.NewPage;

        //            if (navigationAction.TargetFrame == null)
        //                webView?.LoadRequest(navigationAction.Request);

        //            break;
        //        case WKNavigationType.FormSubmitted:
        //            navEvent = WebNavigationEvent.NewPage;
        //            break;
        //        case WKNavigationType.BackForward:
        //            navEvent = CurrentNavigationEvent;
        //            break;
        //        case WKNavigationType.Reload:
        //            navEvent = WebNavigationEvent.Refresh;
        //            break;
        //        case WKNavigationType.FormResubmitted:
        //            navEvent = WebNavigationEvent.NewPage;
        //            break;
        //        case WKNavigationType.Other:
        //            navEvent = WebNavigationEvent.NewPage;
        //            break;
        //    }

        //    _lastEvent = navEvent;

        //    var request = navigationAction.Request;
        //    var lastUrl = request.Url.ToString();

        //    bool cancel = virtualView.Navigating(navEvent, lastUrl);
        //    platformView.UpdateCanGoBackForward(virtualView);
        //    decisionHandler(cancel ? WKNavigationActionPolicy.Cancel : WKNavigationActionPolicy.Allow);
        //}

        protected Handler? Handler
        {
            get
            {
                if (handler.TryGetTarget(out var h))
                {
                    return h;
                }

                return null;
            }
        }
    }
}
#endif