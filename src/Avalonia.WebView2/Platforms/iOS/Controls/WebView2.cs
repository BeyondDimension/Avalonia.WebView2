#if IOS
using Avalonia.iOS;
#endif
#if IOS || MACCATALYST || (MACOS && !USE_DEPRECATED_WEBVIEW)
using Avalonia.Controls.Platform;
using Avalonia.Platform;
using System.Diagnostics.CodeAnalysis;
using WebKit;

namespace Avalonia.Controls;

partial class WebView2 : global::Avalonia.Controls.NativeControlHost
{
    // https://github.com/dotnet/maui/blob/9.0.70/src/Controls/src/Core/HybridWebView/HybridWebView.cs
    // https://github.com/dotnet/maui/blob/9.0.70/src/Controls/src/Core/WebView/WebView.cs
    // https://github.com/dotnet/maui/blob/9.0.70/src/Core/src/Handlers/WebView/WebViewHandler.iOS.cs
    // https://github.com/dotnet/maui/blob/9.0.70/src/Compatibility/Core/src/MacOS/Renderers/WebViewRenderer.cs
    // https://github.com/dotnet/maui/blob/9.0.70/src/Compatibility/Core/src/iOS/Renderers/WkWebViewRenderer.cs
    // https://github.com/dotnet/maui/blob/9.0.70/src/Core/src/Handlers/WebView/WebViewHandler.cs
    // https://github.com/dotnet/maui/blob/9.0.70/src/Core/src/Platform/iOS/WebViewExtensions.cs
    // https://github.com/dotnet/maui/blob/9.0.70/src/Core/src/Handlers/HybridWebView/HybridWebViewHandler.iOS.cs
    // https://developer.apple.com/documentation/webkit/wkwebview

    [UnconditionalSuppressMessage("Memory", "MEM0002", Justification = "Used to persist cookies across WebView instances. Not a leak.")]
    static WKProcessPool? SharedPool;

    /// <summary>
    /// https://developer.apple.com/forums/thread/99674
    /// WKWebView and making sure cookies synchronize is really quirky
    /// The main workaround I've found for ensuring that cookies synchronize 
    /// is to share the Process Pool between all WkWebView instances.
    /// It also has to be shared at the point you call init
    /// https://github.com/dotnet/maui/blob/9.0.70/src/Core/src/Platform/iOS/MauiWKWebView.cs#L159-L181
    /// </summary>
    /// <returns></returns>
    public static WKWebViewConfiguration CreateConfiguration()
    {
        // By default, setting inline media playback to allowed, including autoplay
        // and picture in picture, since these things MUST be set during the webview
        // creation, and have no effect if set afterwards.
        // A custom handler factory delegate could be set to disable these defaults
        // but if we do not set them here, they cannot be changed once the
        // handler's platform view is created, so erring on the side of wanting this
        // capability by default.
        var config = new WKWebViewConfiguration();
#if !MACOS
#if IOS
        if (OperatingSystem.IsIOSVersionAtLeast(10))
#elif MACCATALYST
        if (OperatingSystem.IsMacCatalystVersionAtLeast(10))
#else
        if (OperatingSystem.IsMacCatalystVersionAtLeast(10) || OperatingSystem.IsIOSVersionAtLeast(10))
#endif
#endif
        {
#if !MACOS
            config.AllowsPictureInPictureMediaPlayback = true;
            config.AllowsInlineMediaPlayback = true;
#endif
            config.MediaTypesRequiringUserActionForPlayback = WKAudiovisualMediaTypes.None;
        }
        if (SharedPool == null)
            SharedPool = config.ProcessPool;
        else
            config.ProcessPool = SharedPool;

        return config;
    }


    protected virtual WKWebView CreatePlatformView()
    {
        var webView = new WKWebView(CGRect.Empty, CreateConfiguration())
        {
#if !MACOS
            BackgroundColor = UIColor.Clear,
            AutosizesSubviews = true,
#endif
        };
        //webView.NavigationDelegate = new WebView2NavigationDelegate(handler);
        return webView;
    }

    WKWebViewControlHandle? platformHandle;

    public WKWebView? WKWebView => platformHandle?.WebView;

    /// <inheritdoc/>
    protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
    {
        var webView = CreatePlatformView();
        platformHandle = new WKWebViewControlHandle(webView);
        return platformHandle;
    }
}

sealed class WKWebViewControlHandle : PlatformHandle, INativeControlHostDestroyableControlHandle
{
    bool disposedValue;
    WKWebView? webView;

    internal WKWebViewControlHandle(WKWebView webView) : base(webView.Handle, "WKWebView")
    {
        this.webView = webView;
    }

    public WKWebView? WebView => webView;

    void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // 释放托管状态(托管对象)
                webView?.Dispose();
            }

            // 释放未托管的资源(未托管的对象)并重写终结器
            // 将大型字段设置为 null
            webView = null;
            disposedValue = true;
        }
    }

    /// <inheritdoc/>
    public void Destroy()
    {
        // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        Dispose(disposing: true);
    }
}

// Use interface instead of base class here
// https://developer.apple.com/documentation/webkit/wknavigationdelegate/1455641-webview?language=objc#discussion
// The newer policy method is implemented in the base class
// and the doc remarks state the older policy method is not called
// if the newer one is implemented, but the new one is v13+
// so we'd like to stick with the older one for now
public class WebView2NavigationDelegate : NSObject, IWKNavigationDelegate
{
    // https://github.com/dotnet/maui/blob/9.0.70/src/Core/src/Platform/iOS/MauiWebViewNavigationDelegate.cs

    [Export("webView:didFinishNavigation:")]
    public void DidFinishNavigation(WKWebView webView, WKNavigation navigation)
    {
    }

    [Export("webView:didFailNavigation:withError:")]
    public void DidFailNavigation(WKWebView webView, WKNavigation navigation, NSError error)
    {
    }

    [Export("webView:didFailProvisionalNavigation:withError:")]
    public void DidFailProvisionalNavigation(WKWebView webView, WKNavigation navigation, NSError error)
    {
    }

    // https://stackoverflow.com/questions/37509990/migrating-from-uiwebview-to-wkwebview
    [Export("webView:decidePolicyForNavigationAction:decisionHandler:")]
    public void DecidePolicy(WKWebView webView, WKNavigationAction navigationAction, Action<WKNavigationActionPolicy> decisionHandler)
    {
    }

    //string GetCurrentUrl()
    //{
    //    return Handler?.PlatformView?.Url?.AbsoluteUrl?.ToString() ?? string.Empty;
    //}

    //internal WebNavigationEvent CurrentNavigationEvent
    //{
    //    get;
    //    set;
    //}
}
#endif