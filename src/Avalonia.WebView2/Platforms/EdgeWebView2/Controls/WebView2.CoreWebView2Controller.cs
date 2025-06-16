#if !DISABLE_WEBVIEW2_CORE && (WINDOWS || NETFRAMEWORK)
using Microsoft.Web.WebView2.Core;

namespace Avalonia.Controls;

partial class WebView2
{
    CoreWebView2Controller? _coreWebView2Controller;
    bool _browserCrashed;

    public CoreWebView2Controller? CoreWebView2Controller => _coreWebView2Controller;

    /// <summary>
    /// 解除订阅和关闭 CoreWebView2 控制器
    /// </summary>
    /// <param name="browserCrashed"></param>
    void UnsubscribeHandlersAndCloseController(bool browserCrashed = false)
    {
        _browserCrashed = browserCrashed;
        if (!_browserCrashed)
        {
            UnsubscribeHandlers();
            _coreWebView2Controller?.Close();
        }
        _coreWebView2Controller = null;
    }

    /// <summary>
    /// 验证浏览器组件没有崩溃，当浏览器组件崩溃时，抛出 <see cref="InvalidOperationException"/>
    /// </summary>
    /// <exception cref="InvalidOperationException">当浏览器组件崩溃时</exception>
    void VerifyBrowserNotCrashedGuard()
    {
        if (_browserCrashed)
        {
            throw new InvalidOperationException("The instance of CoreWebView2 is no longer valid because the browser process crashed.To work around this, please listen for the ProcessFailed event to explicitly manage the lifetime of the WebView2 control in the event of a browser failure.https://docs.microsoft.com/en-us/dotnet/api/microsoft.web.webview2.core.corewebview2.processfailed");
        }
    }
}
#endif