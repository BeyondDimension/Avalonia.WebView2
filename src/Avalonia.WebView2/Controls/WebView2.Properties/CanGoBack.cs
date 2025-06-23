using System.ComponentModel;

namespace Avalonia.Controls;

partial class WebView2
{
    bool _canGoBack;

    /// <summary>
    /// Returns <see langword="true"/> if the <see cref="WebView2"/> can navigate to a previous page in the
    /// navigation history via the <see cref="GoBack"/> method.
    /// This is equivalent to the CoreWebView2.CanGoBack.
    /// If the underlying CoreWebView2 is not yet initialized, this property is <see langword="false"/>.
    /// 如果 <see cref="WebView2"/> 可以通过 <see cref="GoBack"/> 方法导航到导航历史记录中的前一页，则返回 <see langword="true"/>
    /// 这等同于 CoreWebView2.CanGoBack
    /// 如果底层 CoreWebView2 尚未初始化，则此属性为 <see langword="false"/>
    /// </summary>
    [Browsable(false)]
    public bool CanGoBack
    {
        get
        {
#if (!DISABLE_WEBVIEW2_CORE && (WINDOWS || NETFRAMEWORK)) || ANDROID || (IOS || MACCATALYST || (MACOS && !USE_DEPRECATED_WEBVIEW))
            _canGoBack = GetCanGoBack(this) ?? false;
#endif
            return _canGoBack;
        }
    }

    /// <summary>
    /// The <see cref="AvaloniaProperty" /> which backs the <see cref="CanGoBack" /> property.
    /// </summary>
    public static readonly DirectProperty<WebView2, bool> CanGoBackProperty = AvaloniaProperty.RegisterDirect<WebView2, bool>(nameof(CanGoBack), x => x.CanGoBack);

    /// <summary>
    /// Navigates to the previous page in navigation history.
    /// This is equivalent to CoreWebView2.GoBack.
    /// If the underlying WebView2.CoreWebView2 is not yet initialized, this method does nothing.
    /// </summary>
    public void GoBack()
    {
#if !DISABLE_WEBVIEW2_CORE && WINDOWS || NETFRAMEWORK
        CoreWebView2?.GoBack();
#elif ANDROID
        var aWebView = AWebView;
        if (aWebView != null && aWebView.CanGoBack())
        {
            aWebView.GoBack();
        }
#elif IOS || MACOS || MACCATALYST
        var wkWebView = WKWebView;
        if (wkWebView != null && wkWebView.CanGoBack)
        {
            wkWebView.GoBack();
        }
#elif LINUX
        // CEF_TODO: 待实现 GoBack
#endif
    }
}