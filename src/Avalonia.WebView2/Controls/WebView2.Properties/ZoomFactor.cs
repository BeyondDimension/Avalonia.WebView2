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
#if (!DISABLE_WEBVIEW2_CORE && (WINDOWS || NETFRAMEWORK)) || ANDROID || (IOS || MACCATALYST || (MACOS && !USE_DEPRECATED_WEBVIEW))
            var zoomFactor = GetZoomFactor(this);
            if (zoomFactor.HasValue)
            {
                return zoomFactor.Value;
            }
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

#if (!DISABLE_WEBVIEW2_CORE && (WINDOWS || NETFRAMEWORK)) || ANDROID || (IOS || MACCATALYST || (MACOS && !USE_DEPRECATED_WEBVIEW))
            SetZoomFactor(this, value);
#endif
        }
    }

    /// <summary>
    /// The <see cref="AvaloniaProperty" /> which backs the <see cref="ZoomFactor" /> property.
    /// </summary>
    public static readonly DirectProperty<WebView2, double> ZoomFactorProperty = AvaloniaProperty.RegisterDirect<WebView2, double>(nameof(ZoomFactor), x => x._zoomFactor, (x, y) => x.ZoomFactor = y);
}