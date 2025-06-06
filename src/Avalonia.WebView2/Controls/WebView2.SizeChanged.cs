#if !DISABLE_WEBVIEW2_CORE && (WINDOWS || NETFRAMEWORK)
using Avalonia.Controls.Platforms.Windows.Interop;
#endif
using Avalonia.Platform;
using Avalonia.Threading;

namespace Avalonia.Controls;

partial class WebView2
{
    /// <inheritdoc/>
    protected override void OnSizeChanged(SizeChangedEventArgs e)
    {
#if DEBUG
        Console.WriteLine($"WebView2 OnSizeChanged: {e.NewSize.Width}x{e.NewSize.Height}");
#endif

        base.OnSizeChanged(e);

        Dispatcher.UIThread.Post(() =>
        {
            OnBoundsChanged(EventArgs.Empty);
        }, DispatcherPriority.Render);
    }

    /// <inheritdoc cref="OnSizeChanged"/>
    protected virtual void OnBoundsChanged(EventArgs e)
    {
#if !DISABLE_WEBVIEW2_CORE && (WINDOWS || NETFRAMEWORK)
        if (_coreWebView2Controller != null)
#endif
        {
            var bounds = GetBoundsRectangle();
            OnWindowPositionChanged(bounds);
        }
    }

    /// <summary>
    /// <see cref="Visual.Bounds"/> 的 <see cref="global::System.Drawing.Rectangle"/> 值
    /// </summary>
    /// <returns></returns>
    protected virtual global::System.Drawing.Rectangle GetBoundsRectangle()
    {
#if MACOS || WINDOWS || LINUX || NETFRAMEWORK
        var window = Window;
        var screen = GetScreenFromWindow(window);
        Visual? relativeTo = window;
#else
        var topLevel = TopLevel;
        var screen = GetScreenFromTopLevel(topLevel);
        Visual? relativeTo = topLevel;
#endif
        var result = GetBoundsRectangle(Bounds, this, relativeTo, screen);
        return result;
    }

    /// <summary>
    /// 将宽高双精度值与屏幕 DPI 缩放相乘并转换为像素点整数值
    /// </summary>
    /// <param name="d"></param>
    /// <param name="screen"></param>
    /// <returns></returns>
    protected static int ToPxSize(double d, Screen? screen)
    {
        if (screen != null)
        {
            // 乘以 DPI 缩放
            d *= screen.Scaling;
        }
        if (double.IsNaN(d) || d <= 0D)
        {
            return 0;
        }
        var result = Math.Ceiling(d);
        return Convert.ToInt32(result);
    }

#if MACOS || WINDOWS || LINUX || NETFRAMEWORK
    /// <summary>
    /// 根据窗口获取屏幕
    /// </summary>
    /// <param name="window"></param>
    /// <returns></returns>
    protected static Screen? GetScreenFromWindow(WindowBase? window)
    {
        if (window != null)
        {
            var result = window.Screens.ScreenFromWindow(window);
            return result;
        }
        return null;
    }
#else
    protected static Screen? GetScreenFromTopLevel(TopLevel? topLevel)
    {
        if (topLevel != null)
        {
            var result = topLevel.Screens?.ScreenFromTopLevel(topLevel);
            return result;
        }
        return null;
    }
#endif

    /// <summary>
    /// 根据 Avalonia 控件的边界与屏幕获取像素点的边界
    /// </summary>
    /// <param name="bounds"></param>
    /// <param name="screen"></param>
    /// <returns></returns>
    protected static global::System.Drawing.Rectangle GetBoundsRectangle(Rect bounds, Screen? screen)
    {
        int x = ToPxSize(bounds.X, screen);
        int y = ToPxSize(bounds.Y, screen);
        int w = ToPxSize(bounds.Width, screen);
        int h = ToPxSize(bounds.Height, screen);
        return new(x, y, w, h);
    }

    /// <summary>
    /// 根据 Avalonia 控件的边界与当前控件所在的窗口以及屏幕获取像素点的边界
    /// </summary>
    protected static global::System.Drawing.Rectangle GetBoundsRectangle(Rect bounds, Visual visual, Visual? relativeTo, Screen? screen)
    {
        if (relativeTo != null)
        {
            var point = visual.TranslatePoint(new(0, 0), relativeTo);
            if (point.HasValue)
            {
                var pointValue = point.Value;
                int x = ToPxSize(pointValue.X, screen);
                int y = ToPxSize(pointValue.Y, screen);
                int w = ToPxSize(bounds.Width, screen);
                int h = ToPxSize(bounds.Height, screen);
                return new(x, y, w, h);
            }
        }
        return GetBoundsRectangle(bounds, screen);
    }

    /// <summary>
    /// This is overridden from IHwndHost and called when our control's location has changed.
    /// The HwndHost takes care of updating the HWND we created.
    /// What we need to do is move our CoreWebView2 to match the new location.
    /// </summary>
    protected virtual void OnWindowPositionChanged(global::System.Drawing.Rectangle rectangle)
    {
#if !DISABLE_WEBVIEW2_CORE && (WINDOWS || NETFRAMEWORK)
        if (_coreWebView2Controller != null)
        {
            _coreWebView2Controller.Bounds = rectangle;
            _coreWebView2Controller.NotifyParentWindowPositionChanged();
        }
#elif ANDROID
        var nwv = AWebView;
        if (nwv != null)
        {
            // https://github.com/AvaloniaUI/Avalonia/blob/11.3.1/src/Android/Avalonia.Android/AvaloniaView.cs
            // AvaloniaView 使用 FrameLayout 实现
            if (nwv.LayoutParameters is FrameLayout.LayoutParams layout)
            {
                layout.Height = rectangle.Height;
                layout.Width = rectangle.Width;
                layout.LeftMargin = rectangle.X;
                layout.TopMargin = rectangle.Y;
                nwv.LayoutParameters = layout; // 调用 java set 函数更新布局参数
            }
        }
#elif IOS || MACCATALYST || (MACOS && !USE_DEPRECATED_WEBVIEW)
        var nwv = WKWebView;
        if (nwv != null)
        {
            // TODO: 将 xywh 矩阵值传递给本机控件
            //nwv.LayoutGuides
        }
#endif
    }
}