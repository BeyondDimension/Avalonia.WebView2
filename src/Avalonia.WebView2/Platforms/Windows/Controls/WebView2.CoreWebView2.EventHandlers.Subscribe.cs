#if WINDOWS || NETFRAMEWORK
using Microsoft.Web.WebView2.Core;

namespace Avalonia.Controls;

partial class WebView2
{
    /// <summary>
    /// 订阅 WebView2 控件的事件处理程序
    /// </summary>
    void SubscribeHandlers()
    {
        if (_coreWebView2Controller != null)
        {
            //_coreWebView2Controller.MoveFocusRequested += new EventHandler<CoreWebView2MoveFocusRequestedEventArgs>(CoreWebView2Controller_MoveFocusRequested);
            _coreWebView2Controller.AcceleratorKeyPressed += new EventHandler<CoreWebView2AcceleratorKeyPressedEventArgs>(CoreWebView2Controller_AcceleratorKeyPressed);
            _coreWebView2Controller.ZoomFactorChanged += new EventHandler<object>(CoreWebView2Controller_ZoomFactorChanged);
            _coreWebView2Controller.CoreWebView2.NavigationCompleted += new EventHandler<CoreWebView2NavigationCompletedEventArgs>(CoreWebView2_NavigationCompleted);
            _coreWebView2Controller.CoreWebView2.NavigationStarting += new EventHandler<CoreWebView2NavigationStartingEventArgs>(CoreWebView2_NavigationStarting);
            _coreWebView2Controller.CoreWebView2.SourceChanged += new EventHandler<CoreWebView2SourceChangedEventArgs>(CoreWebView2_SourceChanged);
            _coreWebView2Controller.CoreWebView2.WebMessageReceived += new EventHandler<CoreWebView2WebMessageReceivedEventArgs>(CoreWebView2_WebMessageReceived);
            _coreWebView2Controller.CoreWebView2.ContentLoading += new EventHandler<CoreWebView2ContentLoadingEventArgs>(CoreWebView2_ContentLoading);
            _coreWebView2Controller.CoreWebView2.DOMContentLoaded += new EventHandler<CoreWebView2DOMContentLoadedEventArgs>(CoreWebView2_DOMContentLoaded);
            _coreWebView2Controller.CoreWebView2.ProcessFailed += new EventHandler<CoreWebView2ProcessFailedEventArgs>(CoreWebView2_ProcessFailed);
            _coreWebView2Controller.CoreWebView2.WebResourceRequested += new EventHandler<CoreWebView2WebResourceRequestedEventArgs>(CoreWebView2_WebResourceRequested);
            _coreWebView2Controller.CoreWebView2.DocumentTitleChanged += new EventHandler<object>(CoreWebView2_DocumentTitleChanged);
            _coreWebView2Controller.CoreWebView2.NewWindowRequested += new EventHandler<CoreWebView2NewWindowRequestedEventArgs>(CoreWebView2_NewWindowRequested);
        }
    }

    /// <summary>
    /// 取消订阅 WebView2 控件的事件处理程序
    /// </summary>
    void UnsubscribeHandlers()
    {
        if (CoreWebView2 != null)
        {
            CoreWebView2.NavigationCompleted -= new EventHandler<CoreWebView2NavigationCompletedEventArgs>(CoreWebView2_NavigationCompleted);
            CoreWebView2.NavigationStarting -= new EventHandler<CoreWebView2NavigationStartingEventArgs>(CoreWebView2_NavigationStarting);
            CoreWebView2.SourceChanged -= new EventHandler<CoreWebView2SourceChangedEventArgs>(CoreWebView2_SourceChanged);
            CoreWebView2.WebMessageReceived -= new EventHandler<CoreWebView2WebMessageReceivedEventArgs>(CoreWebView2_WebMessageReceived);
            CoreWebView2.ContentLoading -= new EventHandler<CoreWebView2ContentLoadingEventArgs>(CoreWebView2_ContentLoading);
            CoreWebView2.DOMContentLoaded -= new EventHandler<CoreWebView2DOMContentLoadedEventArgs>(CoreWebView2_DOMContentLoaded);
            CoreWebView2.ProcessFailed -= new EventHandler<CoreWebView2ProcessFailedEventArgs>(CoreWebView2_ProcessFailed);
            CoreWebView2.WebResourceRequested -= new EventHandler<CoreWebView2WebResourceRequestedEventArgs>(CoreWebView2_WebResourceRequested);
            CoreWebView2.DocumentTitleChanged -= new EventHandler<object>(CoreWebView2_DocumentTitleChanged);
            CoreWebView2.NewWindowRequested -= new EventHandler<CoreWebView2NewWindowRequestedEventArgs>(CoreWebView2_NewWindowRequested);
        }
        if (_coreWebView2Controller != null)
        {
            _coreWebView2Controller.ZoomFactorChanged -= new EventHandler<object>(CoreWebView2Controller_ZoomFactorChanged);
            //_coreWebView2Controller.MoveFocusRequested -= new EventHandler<CoreWebView2MoveFocusRequestedEventArgs>(CoreWebView2Controller_MoveFocusRequested);
            _coreWebView2Controller.AcceleratorKeyPressed -= new EventHandler<CoreWebView2AcceleratorKeyPressedEventArgs>(CoreWebView2Controller_AcceleratorKeyPressed);
        }
    }
}
#endif