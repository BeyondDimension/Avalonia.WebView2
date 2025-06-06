using Avalonia.Metadata;
using Avalonia.Platform;

namespace Avalonia.Controls;

partial class WebView2
{
#if ANDROID || IOS || MACCATALYST || (MACOS && !USE_DEPRECATED_WEBVIEW)
    [Content]
    Control? Child
    {
        get => GetValue(ChildProperty);
        set => SetValue(ChildProperty, value);
    }

    static readonly StyledProperty<Control?> ChildProperty =
            AvaloniaProperty.Register<WebView2, Control?>(nameof(Child));

    NativeWebViewControlHost? nativeControlHost;

    sealed partial class NativeWebViewControlHost : NativeControlHost
    {
        readonly WebView2 wv2;

        internal NativeWebViewControlHost(WebView2 webView2)
        {
            wv2 = webView2;
        }

        /// <inheritdoc/>
        protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
        {
#if ANDROID
            var parentContext = GetContext(parent);
            var view = wv2.CreatePlatformView(parentContext);
            return wv2.platformHandle = new AndroidWebViewControlHandle(view);
#elif IOS || MACCATALYST || (MACOS && !USE_DEPRECATED_WEBVIEW)
            var webView = wv2.CreatePlatformView();
            return wv2.platformHandle = new WKWebViewControlHandle(webView);
#endif
        }
    }

    void ChildChanged(AvaloniaPropertyChangedEventArgs e)
    {
        var oldChild = e.OldValue is Control o1 ? o1 : null;
        var newChild = e.NewValue is Control n1 ? n1 : null;

        if (oldChild != null)
        {
            if (oldChild is ISetLogicalParent p)
            {
                p.SetParent(null);
            }
            LogicalChildren.Clear();
            VisualChildren.Remove(oldChild);
        }

        if (newChild != null)
        {
            if (newChild is ISetLogicalParent p)
            {
                p.SetParent(null);
            }
            VisualChildren.Add(newChild);
            LogicalChildren.Add(newChild);
        }
    }
#endif
}
