namespace Avalonia.Controls;

partial class WebView2
{
#if MACOS || WINDOWS || LINUX || NETFRAMEWORK
    TaskCompletionSource<nint> _hwndTaskSource = new();

    /// <summary>
    /// 当前控件所在的窗口，如果没有，则为 <see langword="null"/>
    /// </summary>
    protected Window? Window { get; private set; }

    void Window_Closed(object? sender, EventArgs e)
    {
        // 当窗口关闭时，释放控件
        Dispose();
    }
#else
    protected TopLevel? TopLevel { get; private set; }
#endif

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
#if MACOS || WINDOWS || LINUX || NETFRAMEWORK
        if (e.Root is Window window)
        {
            var prevWindow = Window;
            var isSameWindow = prevWindow == window;
            if (prevWindow != null)
            {
                if (!isSameWindow)
                {
                    // 移除上一个窗口的 Closed 事件
                    prevWindow.Closed -= Window_Closed;
                }
            }
            if (!isSameWindow)
            {
                // Different windows cannot be reinitialized successfully
                Window = window;
                Window.Closed += Window_Closed;
                var hwnd = window.TryGetPlatformHandle()?.Handle;
                if (hwnd.HasValue && hwnd.Value != default)
                {
                    if (!_hwndTaskSource.TrySetResult(hwnd.Value))
                    {
                        TaskCompletionSource<nint> hwndTaskSource = new();
                        hwndTaskSource.SetResult(hwnd.Value);
                        _hwndTaskSource = hwndTaskSource;
                    }
                }
#if !DISABLE_WEBVIEW2_CORE && (WINDOWS || NETFRAMEWORK)
                _implicitInitGate.OnSynchronizationContextExists();
#endif
            }
        }
#if !DISABLE_WEBVIEW2_CORE && (WINDOWS || NETFRAMEWORK)
        if (IsVisible && _coreWebView2Controller != null)
        {
            if (!_coreWebView2Controller.IsVisible)
            {
                _coreWebView2Controller.IsVisible = true;
            }
        }
#endif
#else
        if (e.Root is TopLevel topLevel)
        {
            TopLevel = topLevel;
        }
#if ANDROID        
        if (viewHandler == null)
        {
            viewHandler = CreateViewHandler(this);
            var view = viewHandler.CreatePlatformView();
            SetInitValue(view);
            Child = viewHandler;
        }
#elif IOS || MACCATALYST || (MACOS && !USE_DEPRECATED_WEBVIEW)
        if (IsVisible)
        {
            var nwv = WKWebView;
            if (nwv != null)
            {
                if (nwv.Hidden)
                {
                    nwv.Hidden = false;
                }
            }
        }
#endif
#endif
        base.OnAttachedToVisualTree(e);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
#if !DISABLE_WEBVIEW2_CORE && (WINDOWS || NETFRAMEWORK)
        if (_coreWebView2Controller != null)
        {
            if (_coreWebView2Controller.IsVisible)
            {
                // 当控件在虚拟树中 Detached 时，隐藏 CoreWebView2
                _coreWebView2Controller.IsVisible = false;
            }
        }
#elif ANDROID
        var nwv = AWebView;
        if (nwv != null)
        {
            if (nwv.Visibility != global::Android.Views.ViewStates.Gone)
            {
                nwv.Visibility = global::Android.Views.ViewStates.Gone;
            }
        }
#elif IOS || MACCATALYST || (MACOS && !USE_DEPRECATED_WEBVIEW)
        var nwv = WKWebView;
        if (nwv != null)
        {
            if (!nwv.Hidden)
            {
                nwv.Hidden = true;
            }
        }
#endif
    }
}