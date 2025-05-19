using Avalonia.Input;

namespace Avalonia.Controls;

/// <summary>
/// Microsoft Edge WebView2 控件允许您在本地应用程序中嵌入网络技术（HTML、CSS 和 JavaScript）。WebView2 控件使用 Microsoft Edge 作为渲染引擎，在本地应用程序中显示网页内容。使用 WebView2，您可以在本地应用程序的不同部分嵌入网页代码，或者在一个 WebView2 实例中构建所有本地应用程序。
/// </summary>
public partial class WebView2
{
    static WebView2()
    {
#if WINDOWS && !DISABLE_WEBVIEW2_CORE
        RefreshIsSupported();
#endif
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
        _disposables.Add(this.GetPropertyChangedObservable(IsVisibleProperty).AddClassHandler<WebView2>((t, args) => { IsVisibleChanged(args); }));
        SetDefaultBackgroundColor(_defaultBackgroundColorDefaultValue);


#if !(WINDOWS || NETFRAMEWORK) && NET8_0_OR_GREATER && !ANDROID && !IOS
        CefGuleInitialize();
#endif
    }

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
#if !DISABLE_WEBVIEW2_CORE && (WINDOWS || NETFRAMEWORK)
        if (_coreWebView2Controller == null)
            return;
        _coreWebView2Controller.IsVisible = IsVisible;
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
}

#if !(WINDOWS || NETFRAMEWORK) && !NET8_0_OR_GREATER
partial class WebView2 : global::Avalonia.Controls.Shapes.Rectangle
{
}
#endif