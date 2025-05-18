using System.ComponentModel;

namespace Avalonia.Controls;

partial class WebView2
{
    /// <summary>
    /// Returns <see langword="true"/> if the <see cref="WebView2"/> can navigate to a next page in the
    /// navigation history via the <see cref="GoForward"/> method.
    /// This is equivalent to the CoreWebView2.CanGoForward.
    /// If the underlying CoreWebView2 is not yet initialized, this property is <c>false</c>.
    /// 如果 <see cref="WebView2"/> 可以通过 <see cref="GoForward"/> 方法导航到导航历史记录中的下一页，则返回 <see langword="true"/>。
    /// 这等同于 CoreWebView2.CanGoForward 方法。
    /// 如果底层 CoreWebView2 尚未初始化，则此属性为 <see langword="false"/>
    /// </summary>
    [Browsable(false)]
    public bool CanGoForward
    {
        get
        {
#if !DISABLE_WEBVIEW2_CORE && WINDOWS || NETFRAMEWORK
            var coreWebView2 = CoreWebView2;
            if (coreWebView2 != null)
            {
                return coreWebView2.CanGoForward;
            }
#elif ANDROID
#elif IOS
#else
            // CEF_TODO: 待实现 CanGoForward
#endif
            return false;
        }
    }

    /// <summary>
    /// The <see cref="AvaloniaProperty" /> which backs the <see cref="CanGoForward" /> property.
    /// </summary>
    public static readonly DirectProperty<WebView2, bool> CanGoForwardProperty = AvaloniaProperty.RegisterDirect<WebView2, bool>(nameof(CanGoForward), x => x.CanGoForward);

    /// <summary>
    /// Navigates to the next page in navigation history.
    /// This is equivalent to CoreWebView2.GoForward.
    /// If the underlying WebView2.CoreWebView2 is not yet initialized, this method does nothing.
    /// </summary>
    /// <seealso cref="M:Microsoft.Web.WebView2.Core.CoreWebView2.GoForward" />
    public void GoForward()
    {
#if !DISABLE_WEBVIEW2_CORE && WINDOWS || NETFRAMEWORK
        CoreWebView2?.GoForward();
#elif ANDROID
#elif IOS
#else
        // CEF_TODO: 待实现 GoForward
#endif
    }
}