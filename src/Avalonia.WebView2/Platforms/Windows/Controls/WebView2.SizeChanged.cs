#if WINDOWS || NETFRAMEWORK
using Avalonia.Controls.Platforms.Windows.Interop;
using Avalonia.Platform;
using Avalonia.Threading;

namespace Avalonia.Controls;

partial class WebView2
{
    /// <inheritdoc/>
    protected override void OnSizeChanged(SizeChangedEventArgs e)
    {
        base.OnSizeChanged(e);

        Dispatcher.UIThread.Post(() =>
        {
            OnBoundsChanged(EventArgs.Empty);
        }, DispatcherPriority.Render);
    }

    /// <inheritdoc cref="OnSizeChanged"/>
    protected virtual void OnBoundsChanged(EventArgs e)
    {
        if (_coreWebView2Controller != null)
        {
            var bounds = GetBoundsRectangle();
            OnWindowPositionChanged(bounds);
        }
    }

    /// <summary>
    /// <see cref="Visual.Bounds"/> 的 <see cref="global::System.Drawing.Rectangle"/> 值
    /// </summary>
    /// <returns></returns>
    public global::System.Drawing.Rectangle GetBoundsRectangle()
    {
        var result = GetBoundsRectangle(Bounds, this, Window);
        return result;
    }

    /// <summary>
    /// 将宽高双精度值与屏幕 DPI 缩放相乘并转换为像素点整数值
    /// </summary>
    /// <param name="d"></param>
    /// <param name="screen"></param>
    /// <returns></returns>
    static int ToPxSize(double d, Screen? screen)
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

    /// <summary>
    /// 根据窗口获取屏幕
    /// </summary>
    /// <param name="window"></param>
    /// <returns></returns>
    static Screen? GetScreenFromWindow(WindowBase? window)
    {
        if (window != null)
        {
            var result = window.Screens.ScreenFromWindow(window);
            return result;
        }
        return null;
    }

    /// <summary>
    /// 根据 Avalonia 控件的边界与屏幕获取像素点的边界
    /// </summary>
    /// <param name="bounds"></param>
    /// <param name="screen"></param>
    /// <returns></returns>
    static global::System.Drawing.Rectangle GetBoundsRectangle(Rect bounds, Screen? screen)
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
    /// <param name="bounds"></param>
    /// <param name="visual"></param>
    /// <param name="window"></param>
    /// <returns></returns>
    static global::System.Drawing.Rectangle GetBoundsRectangle(Rect bounds, Visual visual, WindowBase? window)
    {
        var screen = GetScreenFromWindow(window);
        if (window != null)
        {
            var point = visual.TranslatePoint(new(0, 0), window);
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
    /// This is overridden from <see cref="IHwndHost" /> and called when our control's location has changed.
    /// The HwndHost takes care of updating the HWND we created.
    /// What we need to do is move our CoreWebView2 to match the new location.
    /// </summary>
    protected virtual void OnWindowPositionChanged(global::System.Drawing.Rectangle rectangle)
    {
        if (_coreWebView2Controller != null)
        {
            _coreWebView2Controller.Bounds = rectangle;
            _coreWebView2Controller.NotifyParentWindowPositionChanged();
        }
    }
}
#endif