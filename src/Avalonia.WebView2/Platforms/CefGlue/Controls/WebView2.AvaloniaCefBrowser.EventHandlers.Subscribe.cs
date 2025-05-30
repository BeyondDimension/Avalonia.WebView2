#if !(WINDOWS || NETFRAMEWORK) && NET8_0_OR_GREATER && !ANDROID && !IOS && !MACOS

namespace Avalonia.Controls;

partial class WebView2
{
    /// <summary>
    /// 订阅 WebView2 控件的事件处理程序
    /// </summary>
    void SubscribeHandlers()
    {
        this.LoadStart += OnBrowserLoadStart;
    }

    /// <summary>
    /// 取消订阅 WebView2 控件的事件处理程序
    /// </summary>
    void UnsubscribeHandlers()
    {
        this.LoadStart -= OnBrowserLoadStart;
    }
}
#endif