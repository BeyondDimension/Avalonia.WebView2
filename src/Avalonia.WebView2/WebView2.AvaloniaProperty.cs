namespace Avalonia.Controls;

partial class WebView2
{
#if !DISABLE_WEBVIEW2_CORE
    /// <summary>
    /// The <see cref="AvaloniaProperty" /> which backs the <see cref="CreationProperties" /> property.
    /// </summary>
    /// <seealso cref="AvaloniaProperty" />
    public static readonly DirectProperty<WebView2, CoreWebView2CreationProperties?> CreationPropertiesProperty = AvaloniaProperty.RegisterDirect<WebView2, CoreWebView2CreationProperties?>(nameof(CreationProperties),
        x => x.Environment != null ? x._creationProperties : null,
        (x, y) => x.CreationProperties = y);
#endif

    /// <summary>
    /// The <see cref="AvaloniaProperty" /> which backs the <see cref="Source" /> property.
    /// </summary>
    public static readonly DirectProperty<WebView2, Uri?> SourceProperty = AvaloniaProperty.RegisterDirect<WebView2, Uri?>(nameof(Source), x => x._source, (x, y) => x.Source = y);

    public static readonly DirectProperty<WebView2, string?> HtmlSourceProperty = AvaloniaProperty.RegisterDirect<WebView2, string?>(nameof(HtmlSource), x => x._htmlSource, (x, y) => x.HtmlSource = y);

    /// <summary>
    /// The <see cref="AvaloniaProperty" /> which backs the <see cref="CanGoBack" /> property.
    /// </summary>
    public static readonly DirectProperty<WebView2, bool> CanGoBackProperty = AvaloniaProperty.RegisterDirect<WebView2, bool>(nameof(CanGoBack), x => x.CanGoBack);

    /// <summary>
    /// The <see cref="AvaloniaProperty" /> which backs the <see cref="CanGoForward" /> property.
    /// </summary>
    public static readonly DirectProperty<WebView2, bool> CanGoForwardProperty = AvaloniaProperty.RegisterDirect<WebView2, bool>(nameof(CanGoForward), x => x.CanGoForward);

    /// <summary>
    /// The <see cref="AvaloniaProperty" /> which backs the <see cref="ZoomFactor" /> property.
    /// </summary>
    public static readonly DirectProperty<WebView2, double> ZoomFactorProperty = AvaloniaProperty.RegisterDirect<WebView2, double>(nameof(ZoomFactor), x => x._zoomFactor, (x, y) => x.ZoomFactor = y);

    /// <summary>
    /// The <see cref="AvaloniaProperty" /> which backs the <see cref="DefaultBackgroundColor" /> property.
    /// </summary>
    public static readonly DirectProperty<WebView2, Color> DefaultBackgroundColorProperty = AvaloniaProperty.RegisterDirect<WebView2, Color>(nameof(DefaultBackgroundColor), x => x._defaultBackgroundColor, (x, y) => x.DefaultBackgroundColor = y);

    /// <summary>
    /// The <see cref="AvaloniaProperty" /> which backs the <see cref="AllowExternalDropProperty" /> property.
    /// </summary>
    public static readonly DirectProperty<WebView2, bool> AllowExternalDropProperty = AvaloniaProperty.RegisterDirect<WebView2, bool>(nameof(AllowExternalDrop), x => x._allowExternalDrop, (x, y) => x.AllowExternalDrop = y);
}
