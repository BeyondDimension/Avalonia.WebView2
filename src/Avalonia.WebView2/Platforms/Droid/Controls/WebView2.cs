#if ANDROID
using Android.Content;
using Android.Views;
using Android.Webkit;
using Avalonia.Android;
using Avalonia.Controls.Platform;
using Avalonia.Platform;
using System.Diagnostics.CodeAnalysis;
using AUri = Android.Net.Uri;
using AWebView = Android.Webkit.WebView;

namespace Avalonia.Controls;

partial class WebView2 : global::Avalonia.Controls.NativeControlHost
{
    // https://github.com/dotnet/maui/blob/9.0.70/src/Controls/src/Core/HybridWebView/HybridWebView.cs
    // https://github.com/dotnet/maui/blob/9.0.70/src/Controls/src/Core/WebView/WebView.cs
    // https://github.com/dotnet/maui/blob/9.0.70/src/Core/src/Handlers/HybridWebView/HybridWebViewHandler.Android.cs
    // https://github.com/dotnet/maui/blob/9.0.70/src/Core/src/Handlers/WebView/WebViewHandler.Android.cs
    // https://github.com/dotnet/maui/blob/9.0.70/src/Core/src/Platform/Android/MauiHybridWebView.cs
    // https://developer.android.google.cn/develop/ui/views/layout/webapps/managing-webview?hl=zh-cn

    public static Context? GetContext(IPlatformHandle platformHandle)
    {
        if (platformHandle is AndroidViewControlHandle viewControlHandle)
        {
            return viewControlHandle.View?.Context;
        }
        else if (platformHandle is AndroidWebViewControlHandle webView2ControlHandle)
        {
            return webView2ControlHandle.WebView?.Context;
        }
        return null;
    }

    protected virtual AWebView CreatePlatformView(Context? context = null)
    {
        context ??= global::Android.App.Application.Context;
        var platformView = new AWebView(context)
        {
            LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent),
        };

        platformView.Settings.JavaScriptEnabled = true;
        platformView.Settings.DomStorageEnabled = true;
        platformView.Settings.SetSupportMultipleWindows(true);

        return platformView;
    }

    AndroidWebViewControlHandle? platformHandle;

    public AWebView? AWebView
    {
        get
        {
            var result = platformHandle?.WebView;
            if (result.IsAlive())
            {
                return result;
            }
            return null;
        }
    }

    /// <inheritdoc/>
    protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
    {
        var parentContext = GetContext(parent);
        var view = CreatePlatformView(parentContext);
        platformHandle = new AndroidWebViewControlHandle(view);
        return platformHandle;
    }
}

sealed class AndroidWebViewControlHandle : PlatformHandle, INativeControlHostDestroyableControlHandle
{
    bool disposedValue;
    AWebView? webView;

    internal AndroidWebViewControlHandle(AWebView webView) : base(webView.Handle, "android.webkit.WebView")
    {
        this.webView = webView;
    }

    public AWebView? WebView => webView;

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
#endif