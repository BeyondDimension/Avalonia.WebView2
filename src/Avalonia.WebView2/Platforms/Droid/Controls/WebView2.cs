#if ANDROID
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using AndroidX.Core.View;
using AndroidX.WebKit;
using Avalonia.Android;
using Avalonia.Controls.Platform;
using Avalonia.Platform;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;
using AUri = Android.Net.Uri;
using AWebView = Android.Webkit.WebView;

namespace Avalonia.Controls;

partial class WebView2
{
    /// <summary>
    /// https://github.com/dotnet/maui/blob/9.0.70/src/Core/src/Handlers/WebView/WebViewHandler.Android.cs
    /// </summary>
    partial class Handler
    {
        protected virtual WebViewClientCompat CreateWebViewClient() => new WebViewClientCompat2(this);

        protected virtual WebChromeClient CreateWebChromeClient() => new WebChromeClient2(this);

        /// <inheritdoc cref="WebSettings.JavaScriptEnabled"/>
        protected virtual bool JavaScriptEnabled => true;

        /// <inheritdoc cref="WebSettings.DomStorageEnabled"/>
        protected virtual bool DomStorageEnabled => true;

        /// <inheritdoc cref="WebSettings.SetSupportMultipleWindows"/>
        protected virtual bool SupportMultipleWindows => true;

        /// <inheritdoc cref="WebSettings.SetSupportZoom"/>
        protected virtual bool SupportZoom => true;

        protected virtual void ConfigureSettings(WebSettings settings)
        {
            settings.JavaScriptEnabled = JavaScriptEnabled;
            settings.DomStorageEnabled = DomStorageEnabled;
            settings.SetSupportMultipleWindows(SupportMultipleWindows);
            settings.SetSupportZoom(SupportZoom);
        }

        /// <summary>
        /// https://developer.android.google.cn/develop/ui/views/layout/webapps/managing-webview?hl=zh-cn
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual AWebView CreatePlatformView(Context? context = null)
        {
            context ??= global::Android.App.Application.Context;
            var platformView = new AWebView(context)
            {
                LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent),
            };

            ConfigureSettings(platformView.Settings);

            platformView.SetWebViewClient(CreateWebViewClient());
            platformView.SetWebChromeClient(CreateWebChromeClient());

#if DEBUG
            platformView.Background = new ColorDrawable(Color.Purple);
#endif

            platformView.Settings.JavaScriptEnabled = true;
            platformView.Settings.DomStorageEnabled = true;
            platformView.Settings.SetSupportMultipleWindows(true);

            return webView = platformView;
        }

        public virtual void DisconnectHandler(AWebView platformView)
        {
            platformView.SetWebViewClient(null!);
            platformView.SetWebChromeClient(null);

            // 停止网页加载
            platformView.StopLoading();
        }
    }

    partial class Handler : IPlatformHandle, INativeControlHostDestroyableControlHandle
    {
        nint IPlatformHandle.Handle => webView == default ? default : webView.Handle;

        string? IPlatformHandle.HandleDescriptor => "android.view.View";

        bool disposedValue;
        protected AWebView? webView;

        protected bool DisposedValue => disposedValue;

        public AWebView? PlatformView => webView;

        void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // 释放托管状态(托管对象)
                    if (webView != null)
                    {
                        DisconnectHandler(webView);
                        webView.Destroy(); // 销毁平台控件
                        webView.Dispose();
                    }
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


    public static Context? GetContext(IPlatformHandle platformHandle)
    {
        if (platformHandle is AndroidViewControlHandle viewControlHandle)
        {
            return viewControlHandle.View?.Context;
        }
        return null;
    }

    public AWebView? PlatformWebView => AWebView;

    public AWebView? AWebView
    {
        get
        {
            var result = viewHandler?.PlatformView;
            if (result.IsAlive())
            {
                return result;
            }
            return null;
        }
    }

    protected virtual void SetInitValue(AWebView webView)
    {
        var visibility = IsVisible ? ViewStates.Visible : ViewStates.Gone;
        if (webView.Visibility != visibility)
        {
            webView.Visibility = visibility;
        }

        if (_source != null)
        {
            webView.SetSource(_source);
        }

        // TODO: other properties
    }

    /// <summary>
    /// 指示应用是否打算使用明文网络流量，如明文 HTTP
    /// </summary>
    public const bool UsesCleartextTraffic =
#if DEBUG
        true;
#else
        false;
#endif

    /// <summary>
    /// https://github.com/dotnet/maui/blob/9.0.70/src/Core/src/Platform/Android/MauiWebViewClient.cs
    /// <para>Android\android-sdk\sources\android-34\android\webkit\WebViewClient.java</para>
    /// </summary>
    /// <param name="handler"></param>
    public partial class WebViewClientCompat2(Handler handler) : WebViewClientCompat
    {
        readonly WeakReference<Handler> handler = new(handler);

        /// <summary>
        /// 获取允许的 URL 协议列表，当非标准协议时，WebView 会显示 net::ERR_UNKNOWN_URL_SCHEME 错误页，设置白名单以屏蔽错误
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<string>? GetAllowedUrlSchemes()
        {
            yield return "https";
            if (UsesCleartextTraffic)
            {
                yield return "http";
            }
        }

        /// <summary>
        /// 当 URL 即将加载到当前 WebView 时，给主机应用程序一个控制的机会，加载到当前 WebView 时，让主机应用程序有机会进行控制，返回 <see langword="true"/> 则取消当前加载，否则返回 <see langword="false"/>
        /// </summary>
        public virtual bool ShouldOverrideUrlLoading(AWebView? view, IWebResourceRequest? request, string? url)
        {
            var allowedUrlSchemes = GetAllowedUrlSchemes();
            if (allowedUrlSchemes != null)
            {
                if (request != null)
                {
                    var urlScheme = request.Url?.Scheme;
                    if (allowedUrlSchemes.Any(
                        x => string.Equals(x, urlScheme, StringComparison.OrdinalIgnoreCase)))
                    {
                        return false;
                    }
                    else
                    {
                        return true; // 阻止加载非白名单协议的 URL
                    }
                }
                else if (url != null)
                {
                    var urlSpan = url.AsSpan();
                    foreach (var it in allowedUrlSchemes)
                    {
                        if (urlSpan.StartsWith(it, StringComparison.OrdinalIgnoreCase))
                        {
                            if (urlSpan[..it.Length].StartsWith("://"))
                            {
                                return false;
                            }
                        }
                    }
                    return true; // 阻止加载非白名单协议的 URL
                }
            }
            return false;
        }

        [SupportedOSPlatform("android24.0")]
        public override bool ShouldOverrideUrlLoading(AWebView? view, IWebResourceRequest? request)
        {
            return ShouldOverrideUrlLoading(view, request, null);
        }

        [ObsoletedOSPlatform("android24.0")]
        public override bool ShouldOverrideUrlLoading(AWebView? view, string? url)
        {
            return ShouldOverrideUrlLoading(view, null, url);
        }

        public override void OnPageStarted(AWebView? view, string? url, Bitmap? favicon)
        {
            var handler = Handler;
            if (!string.IsNullOrWhiteSpace(url))
            {
                handler.VirtualView.SyncPlatformCookiesToWebView2(url).FireAndForget();
            }

            if (view != null)
            {
                if (!string.IsNullOrWhiteSpace(url))
                {
                    var js = HandlerStorageServiceGenerateJSString(handler.VirtualView.StorageService, url);
                    if (js != null)
                    {
                        view.EvaluateJavascript(js, null);
                    }
                }
            }

            base.OnPageStarted(view, url, favicon);
        }

        public override void OnPageFinished(AWebView? view, string? url)
        {
            var handler = Handler;
            if (!string.IsNullOrWhiteSpace(url))
            {
                handler.VirtualView.SyncPlatformCookiesToWebView2(url).FireAndForget();
            }

            base.OnPageFinished(view, url);
        }

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


    /// <summary>
    /// https://github.com/dotnet/maui/blob/9.0.70/src/Core/src/Platform/Android/MauiWebChromeClient.cs
    /// </summary>
    /// <param name="handler"></param>
    public partial class WebChromeClient2(Handler handler) : WebChromeClient
    {
    }

    partial class WebResourceRequestUri
    {
        public void LoadDataWithBaseURL(AWebView webView)
        {
            var content = StringContent;
            //if (content == null && Content != null)
            //{
            //    Encoding
            //}
            webView.LoadDataWithBaseURL(uriString, content ?? "", "text/html", "UTF-8", null);
        }
    }
}

static class JavaObjectExtensions
{
    public static bool IsDisposed(this global::Java.Lang.Object obj)
    {
        return obj.Handle == IntPtr.Zero;
    }

    public static bool IsDisposed(this global::Android.Runtime.IJavaObject obj)
    {
        return obj.Handle == IntPtr.Zero;
    }

    public static bool IsAlive([NotNullWhen(true)] this global::Java.Lang.Object? obj)
    {
        if (obj == null)
            return false;

        return !obj.IsDisposed();
    }

    public static bool IsAlive([NotNullWhen(true)] this global::Android.Runtime.IJavaObject? obj)
    {
        if (obj == null)
            return false;

        return !obj.IsDisposed();
    }
}

static partial class AWebViewExtensions
{
    public static void SetSource(this AWebView webView, Uri? value)
    {
        if (value is WebView2.WebResourceRequestUri webResourceRequest)
        {
            webView.SetSource(webResourceRequest);
        }
        else
        {
            webView.SetSource(value?.AbsoluteUri);
        }
    }

    public static void SetSource(this AWebView webView, string? value)
        => webView.LoadUrl(string.IsNullOrWhiteSpace(value) ? "about:blank" : value);

    public static void SetSource(this AWebView webView, WebView2.WebResourceRequestUri value) => value.LoadDataWithBaseURL(webView);
}
#endif

// TODO
// https://github.com/dotnet/maui/blob/9.0.70/src/Controls/src/Core/HybridWebView/HybridWebView.cs
// https://github.com/dotnet/maui/blob/9.0.70/src/Controls/src/Core/WebView/WebView.cs
// https://github.com/dotnet/maui/blob/9.0.70/src/Core/src/Handlers/HybridWebView/HybridWebViewHandler.Android.cs
// https://github.com/dotnet/maui/blob/9.0.70/src/Core/src/Platform/Android/MauiHybridWebView.cs