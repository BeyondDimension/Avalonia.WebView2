namespace Avalonia.Controls;

partial class WebView2
{
    TaskCompletionSource<nint> _hwndTaskSource = new();

    /// <summary>
    /// 当前控件所在的窗口，如果没有，则为 <see langword="null"/>
    /// </summary>
    protected Window? Window { get; set; }

    void Window_Closed(object? sender, EventArgs e)
    {
        // 当窗口关闭时，释放控件
        Dispose();
    }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
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
        if (_coreWebView2Controller != null)
        {
            if (!_coreWebView2Controller.IsVisible)
            {
                _coreWebView2Controller.IsVisible = true;
            }
        }
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
#endif
    }
}