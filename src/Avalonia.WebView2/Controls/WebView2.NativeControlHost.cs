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

    Handler? viewHandler;

    static unsafe Handler CreateViewHandler(WebView2 wv2)
    {
        var createViewHandler = CreateViewHandlerDelegate;
        if (createViewHandler == default)
        {
            return new Handler(wv2);
        }
        else
        {
            return = createViewHandler(wv2);
        }
    }

    ///// <summary>
    ///// For internal use by the .NET MAUI platform.
    ///// Raised after web navigation completes.
    ///// </summary>
    //public void Navigated(WebView2NavigationEvent evnt, string url, WebView2NavigationResult result)
    //{

    //}

    public partial class Handler : NativeControlHost
    {
        readonly WebView2 wv2;

        public WebView2 VirtualView => wv2;

        internal Handler(WebView2 webView2)
        {
            wv2 = webView2;
        }

        /// <inheritdoc/>
        protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
        {
#if ANDROID
            var parentContext = GetContext(parent);
            var view = CreatePlatformView(parentContext);
            wv2.SetValue(view);
            return this;
#elif IOS || MACCATALYST || (MACOS && !USE_DEPRECATED_WEBVIEW)
            var view = CreatePlatformView();
            wv2.SetValue(view);
            return wv2.platformHandle = new WKWebViewControlHandle(view);
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
