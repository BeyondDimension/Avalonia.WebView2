namespace Avalonia.Controls;

partial class WebView2
{
    double _zoomFactor = 1.0;

    /// <summary>
    /// The zoom factor for the WebView.
    /// WebView 的缩放系数。
    /// </summary>
    public double ZoomFactor
    {
        get
        {
#if !DISABLE_WEBVIEW2_CORE && WINDOWS || NETFRAMEWORK
            if (_coreWebView2Controller != null)
            {
                return _coreWebView2Controller.ZoomFactor;
            }
#else
            // CEF_TODO: 待实现 ZoomFactor
#endif
            return _zoomFactor;
        }
        set
        {
            if (_zoomFactor == value)
            {
                return;
            }
            _zoomFactor = value;
#if !DISABLE_WEBVIEW2_CORE && WINDOWS || NETFRAMEWORK
            _coreWebView2Controller?.ZoomFactor = value;
#else
            // CEF_TODO: 待实现 ZoomFactor
#endif
        }
    }

    /// <summary>
    /// The <see cref="AvaloniaProperty" /> which backs the <see cref="ZoomFactor" /> property.
    /// </summary>
    public static readonly DirectProperty<WebView2, double> ZoomFactorProperty = AvaloniaProperty.RegisterDirect<WebView2, double>(nameof(ZoomFactor), x => x._zoomFactor, (x, y) => x.ZoomFactor = y);
}