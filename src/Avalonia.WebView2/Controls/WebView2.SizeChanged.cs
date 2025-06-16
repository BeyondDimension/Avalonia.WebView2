#if !DISABLE_WEBVIEW2_CORE && (WINDOWS || NETFRAMEWORK)
// 仅 Windows Edge WebView2 处理 OnSizeChanged 事件

#if !DISABLE_WEBVIEW2_CORE && (WINDOWS || NETFRAMEWORK)
using Avalonia.Controls.Platforms.Windows.Interop;
#endif
using Avalonia.Platform;
using Avalonia.Threading;
using Rectangle = System.Drawing.Rectangle;

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

#if !DISABLE_WEBVIEW2_CORE && (WINDOWS || NETFRAMEWORK)
        Dispatcher.UIThread.Post(() =>
        {
            OnBoundsChanged(EventArgs.Empty);
        }, DispatcherPriority.Render);
#else
        OnBoundsChanged(EventArgs.Empty);
#endif
    }

    /// <inheritdoc cref="OnSizeChanged"/>
    protected virtual void OnBoundsChanged(EventArgs e)
    {
#if ANDROID && DEBUG
        {
            var eBounds = e is AvaloniaPropertyChangedEventArgs<Rect> e2 ? e2.NewValue.GetValueOrDefault() : default;
            global::Android.Util.Log.Warn("WebView2",
$"""
OnBoundsChanged: x={Bounds.X}, y={Bounds.Y}, w={Bounds.Width}, h={Bounds.Height}
eBounds: x={eBounds.X}, y={eBounds.Y}, w={eBounds.Width}, h={eBounds.Height}
""");
        }
#endif

#if !DISABLE_WEBVIEW2_CORE && (WINDOWS || NETFRAMEWORK)
        if (_coreWebView2Controller != null)
#endif
        {
            Rectangle bounds;
            if (e is AvaloniaPropertyChangedEventArgs<Rect> e2)
            {
                bounds = GetBoundsRectangle(e2.NewValue.GetValueOrDefault());
            }
            else
            {
                bounds = GetBoundsRectangle();
            }
            OnWindowPositionChanged(bounds);
        }
    }

    /// <summary>
    /// <see cref="Visual.Bounds"/> 的 <see cref="Rectangle"/> 值
    /// </summary>
    protected virtual Rectangle GetBoundsRectangle(Rect? bounds = null)
    {
#if WINDOWS || LINUX || NETFRAMEWORK
        var window = Window;
        var screen = GetScreenFromWindow(window);
        Visual? relativeTo = window;
#else
        var topLevel = TopLevel;
        var screen = GetScreenFromTopLevel(topLevel);
        Visual? relativeTo = topLevel;
#endif
        var result = GetBoundsRectangle(bounds ?? Bounds, this, relativeTo, screen);
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
        if (double.IsNaN(d) || d <= 0D)
        {
            return 0;
        }
        else if (screen != null)
        {
            // 乘以 DPI 缩放
            d *= screen.Scaling;
        }
        var result = Math.Ceiling(d);
        return Convert.ToInt32(result);
    }

#if WINDOWS || LINUX || NETFRAMEWORK
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
    protected static Rectangle GetBoundsRectangle(Rect bounds, Screen? screen)
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
    protected static Rectangle GetBoundsRectangle(Rect bounds, Visual visual, Visual? relativeTo, Screen? screen)
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
    protected virtual void OnWindowPositionChanged(Rectangle rectangle)
    {
#if !DISABLE_WEBVIEW2_CORE && (WINDOWS || NETFRAMEWORK)
        if (_coreWebView2Controller != null)
        {
            _coreWebView2Controller.Bounds = rectangle;
            _coreWebView2Controller.NotifyParentWindowPositionChanged();
        }
#elif ANDROID
        //        var nwv = AWebView;
        //        if (nwv != null)
        //        {
        //            // https://github.com/AvaloniaUI/Avalonia/blob/11.3.1/src/Android/Avalonia.Android/AvaloniaView.cs
        //            // AvaloniaView 使用 FrameLayout 实现
        //            if (nwv.LayoutParameters is FrameLayout.LayoutParams layout)
        //            {
        //                layout.Height = rectangle.Height;
        //                layout.Width = rectangle.Width;
        //                layout.LeftMargin = rectangle.X;
        //                layout.TopMargin = rectangle.Y;
        //                nwv.LayoutParameters = layout; // 调用 java set 函数更新布局参数
        //                nwv.RequestLayout(); // 重新布局
        //#if ANDROID && DEBUG
        //                {
        //                    var isUIThread = Dispatcher.UIThread.CheckAccess();
        //                    var isUIThreadA = (int)global::Android.OS.Build.VERSION.SdkInt >= 23 ?
        //#pragma warning disable CA1416 // 验证平台兼容性
        //                        global::Android.OS.Looper.MainLooper!.IsCurrentThread :
        //#pragma warning restore CA1416 // 验证平台兼容性
        //                        (global::Android.OS.Looper.MainLooper!.Thread == global::Java.Lang.Thread.CurrentThread());
        //                    global::Android.Util.Log.Warn("WebView2",
        //    $"""
        //OnWindowPositionChanged: x={rectangle.X}, y={rectangle.Y}, w={rectangle.Width}, h={rectangle.Height}
        //isUIThread: {isUIThread}
        //isUIThreadA: {isUIThreadA}
        //""");
        //                }
        //#endif
        //            }
        //        }
#elif IOS || MACCATALYST || (MACOS && !USE_DEPRECATED_WEBVIEW)
        var nwv = WKWebView;
        // TODO: 将 xywh 矩阵值传递给本机控件，CGRect 使用浮点型，疑似逻辑值，而不是物理像素值，需要计算 DPI 缩放？
        // 增加 #if 在 iOS 上使用 global::System.Drawing.RectangleF 浮点单精度坐标系
        nwv?.Frame = new CGRect(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
#endif
    }
}
#endif