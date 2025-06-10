#if ANDROID
using Android.Views;
#endif
using Avalonia.Input;

namespace Avalonia.Controls;

/// <summary>
/// Microsoft Edge WebView2 控件允许您在本地应用程序中嵌入网络技术（HTML、CSS 和 JavaScript）。WebView2 控件使用 Microsoft Edge 作为渲染引擎，在本地应用程序中显示网页内容。使用 WebView2，您可以在本地应用程序的不同部分嵌入网页代码，或者在一个 WebView2 实例中构建所有本地应用程序。
/// </summary>
public partial class WebView2 : IWebView2, IWebView2PropertiesSetValue, IWebView2PropertiesGetValue
{
    static WebView2()
    {
#if WINDOWS && !DISABLE_WEBVIEW2_CORE
        RefreshIsSupported();
#endif

#if ANDROID || IOS || MACCATALYST || (MACOS && !USE_DEPRECATED_WEBVIEW)
        ChildProperty.Changed.AddClassHandler<WebView2, Control?>(static (x, e) => x.ChildChanged(e));
#endif
        //BoundsProperty.Changed.AddClassHandler<WebView2, Rect>(static (x, e) => x.OnBoundsChanged(e));
    }

    /// <inheritdoc />
    public WebView2()
    {
        if (IsInDesignMode)
        {
            // 在设计器中不工作
            return;
        }
        // 添加控件显示隐藏切时通知 CoreWebView2Controller
        _disposables.Add(this.GetPropertyChangedObservable(IsVisibleProperty).AddClassHandler<WebView2>(static (s, e) => { s.IsVisibleChanged(e); }));
        DefaultBackgroundColor = _defaultBackgroundColorDefaultValue;

#if !(WINDOWS || NETFRAMEWORK) && NET8_0_OR_GREATER && !ANDROID && !IOS && !MACOS && !MACCATALYST && !DISABLE_CEFGLUE
        //CefGuleInitialize();
#endif
    }

#if IOS || MACCATALYST || (MACOS && !USE_DEPRECATED_WEBVIEW) || ANDROID
    /// <summary>
    /// 自定义创建 ViewHandler 的函数指针
    /// </summary>
    public static unsafe delegate* managed<WebView2, Handler> CreateViewHandlerDelegate { private get; set; }
#endif

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();

#if WINDOWS || NETFRAMEWORK
        global::CefNet.Internal.GlobalHooks.Initialize(this);
#endif
    }

    /// <summary>
    /// This is a handler for our base UIElement's IsVisibleChanged event.
    /// It's predictably fired whenever IsVisible changes, and IsVisible reflects the actual current visibility status of the control.
    /// We just need to pass this info through to our CoreWebView2Controller so it can save some effort when the control isn't visible.
    /// </summary>
    protected virtual void IsVisibleChanged(EventArgs e)
    {
        // 本机控件不在自绘层，在移动端自绘层通常是本机画布控件单独绘制，例如 Android 的 SurfaceView
        // 自绘层在父元素下，本机控件将在父元素下方，Z 轴会大于自绘层，这将导致 AXaml 控件无法遮盖住本机控件
        // 在 WebView2 的占位符层，即 Rectangle 矩形控件上，监听 Visual.IsVisible 属性传递值至本机控件
        // 还需要重写 Visual.OnAttachedToVisualTree/OnDetachedFromVisualTree 处理虚拟树的显示与隐藏

#if !DISABLE_WEBVIEW2_CORE && (WINDOWS || NETFRAMEWORK)
        if (_coreWebView2Controller == null)
            return;
        _coreWebView2Controller.IsVisible = IsVisible;
#elif ANDROID
        var nwv = AWebView;
        nwv?.Visibility = IsVisible ? ViewStates.Visible : ViewStates.Gone;
#elif IOS || MACCATALYST || (MACOS && !USE_DEPRECATED_WEBVIEW)
        var nwv = WKWebView;
        nwv?.Hidden = !IsVisible;
#endif
    }

    /// <inheritdoc />
    protected override void OnGotFocus(GotFocusEventArgs e)
    {
        base.OnGotFocus(e);
#if !DISABLE_WEBVIEW2_CORE && (WINDOWS || NETFRAMEWORK)
        if (_coreWebView2Controller != null)
        {
            if (!_browserCrashed)
            {
                try
                {
                    _coreWebView2Controller.MoveFocus(_lastMoveFocusReason);
                }
                catch (InvalidOperationException ex)
                {
                    if (ex.InnerException != null && ex.InnerException.HResult != -2147019873)
                    {
                        throw;
                    }
                }
            }
        }
        _lastMoveFocusReason = global::Microsoft.Web.WebView2.Core.CoreWebView2MoveFocusReason.Programmatic;
#endif
    }

    /// <summary>
    /// True when we're in design mode and shouldn't create an underlying CoreWebView2.
    /// 为 <see langword="true"/> 时，我们处于设计模式，不应创建底层 CoreWebView2
    /// </summary>
    protected static bool IsInDesignMode => Design.IsDesignMode;

    List<Action<IWebView2>>? _setCommonPropertiesValueActions;

    private void SetCommonPropertiesValue(IWebView2 webView2)
    {
        _setCommonPropertiesValueActions?.ForEach(x => x.Invoke(webView2));
    }


    protected virtual void SetValue(IWebView2 webView)
    {
        SetCommonPropertiesValue(webView);
    }

}

partial class WebView2 : global::Avalonia.Controls.Shapes.Rectangle
{
    // 使用矩形控件作为占位符，监听布局矩阵坐标传递值给本机控件
    // 处理显示隐藏属性
    // 转发功能函数逻辑
    // 订阅事件
    // 释放时销毁资源
}