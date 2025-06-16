//#if MACOS && USE_DEPRECATED_WEBVIEW
//using Avalonia.Controls.Platform;
//using Avalonia.Platform;
//using System.Diagnostics.CodeAnalysis;
//using WebKit;
//using macOSWebView = WebKit.WebView;

//namespace Avalonia.Controls;

//partial class WebView2
//{
//    protected virtual macOSWebView CreatePlatformView()
//    {
//#pragma warning disable CA1422 // 验证平台兼容性
//        var webView = new macOSWebView();
//#pragma warning restore CA1422 // 验证平台兼容性
//        //webView.NavigationDelegate = new WebView2NavigationDelegate(handler);
//        return webView;
//    }

//    MacOSWebViewControlHandle? platformHandle;

//    public macOSWebView? NativeWebView => platformHandle?.WebView;

//    /// <inheritdoc/>
//    protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
//    {
//        var webView = CreatePlatformView();
//        platformHandle = new MacOSWebViewControlHandle(webView);
//        return platformHandle;
//    }
//}

//sealed class MacOSWebViewControlHandle : PlatformHandle, INativeControlHostDestroyableControlHandle
//{
//    bool disposedValue;
//    macOSWebView? webView;

//    internal MacOSWebViewControlHandle(macOSWebView webView) : base(webView.Handle, "WebView")
//    {
//        this.webView = webView;
//    }

//    /// <summary>
//    /// https://developer.apple.com/documentation/webkit/webview
//    /// </summary>
//    public macOSWebView? WebView => webView;

//    void Dispose(bool disposing)
//    {
//        if (!disposedValue)
//        {
//            if (disposing)
//            {
//                // 释放托管状态(托管对象)
//                webView?.Dispose();
//            }

//            // 释放未托管的资源(未托管的对象)并重写终结器
//            // 将大型字段设置为 null
//            webView = null;
//            disposedValue = true;
//        }
//    }

//    /// <inheritdoc/>
//    public void Destroy()
//    {
//        // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
//        Dispose(disposing: true);
//    }
//}

////// Use interface instead of base class here
////// https://developer.apple.com/documentation/webkit/wknavigationdelegate/1455641-webview?language=objc#discussion
////// The newer policy method is implemented in the base class
////// and the doc remarks state the older policy method is not called
////// if the newer one is implemented, but the new one is v13+
////// so we'd like to stick with the older one for now
////public class WebView2NavigationDelegate : NSObject, IWKNavigationDelegate
////{
////    // https://github.com/dotnet/maui/blob/9.0.70/src/Core/src/Platform/iOS/MauiWebViewNavigationDelegate.cs

////    [Export("webView:didFinishNavigation:")]
////    public void DidFinishNavigation(WKWebView webView, WKNavigation navigation)
////    {
////    }

////    [Export("webView:didFailNavigation:withError:")]
////    public void DidFailNavigation(WKWebView webView, WKNavigation navigation, NSError error)
////    {
////    }

////    [Export("webView:didFailProvisionalNavigation:withError:")]
////    public void DidFailProvisionalNavigation(WKWebView webView, WKNavigation navigation, NSError error)
////    {
////    }

////    // https://stackoverflow.com/questions/37509990/migrating-from-uiwebview-to-wkwebview
////    [Export("webView:decidePolicyForNavigationAction:decisionHandler:")]
////    public void DecidePolicy(WKWebView webView, WKNavigationAction navigationAction, Action<WKNavigationActionPolicy> decisionHandler)
////    {
////    }

////    //string GetCurrentUrl()
////    //{
////    //    return Handler?.PlatformView?.Url?.AbsoluteUrl?.ToString() ?? string.Empty;
////    //}

////    //internal WebNavigationEvent CurrentNavigationEvent
////    //{
////    //    get;
////    //    set;
////    //}
////}
//#endif