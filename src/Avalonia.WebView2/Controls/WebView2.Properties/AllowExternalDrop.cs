namespace Avalonia.Controls;

partial class WebView2
{
    bool _allowExternalDrop = true;

    /// <summary>
    /// Enable/disable external drop.
    /// 启用/禁用外部拖拽。
    /// </summary>
    public bool AllowExternalDrop
    {
        get
        {
#if (!DISABLE_WEBVIEW2_CORE && (WINDOWS || NETFRAMEWORK)) || ANDROID || (IOS || MACCATALYST || (MACOS && !USE_DEPRECATED_WEBVIEW))
            var allowExternalDrop = GetAllowExternalDrop(this);
            if (allowExternalDrop != null)
                return allowExternalDrop.Value;
#endif
            return _allowExternalDrop;
        }
        set
        {
            if (_allowExternalDrop == value)
            {
                return;
            }
            _allowExternalDrop = value;

#if (!DISABLE_WEBVIEW2_CORE && (WINDOWS || NETFRAMEWORK)) || ANDROID || (IOS || MACCATALYST || (MACOS && !USE_DEPRECATED_WEBVIEW))
            SetAllowExternalDrop(this, value);
#endif
        }
    }

    /// <summary>
    /// The <see cref="AvaloniaProperty" /> which backs the <see cref="AllowExternalDropProperty" /> property.
    /// </summary>
    public static readonly DirectProperty<WebView2, bool> AllowExternalDropProperty = AvaloniaProperty.RegisterDirect<WebView2, bool>(nameof(AllowExternalDrop), x => x._allowExternalDrop, (x, y) => x.AllowExternalDrop = y);
}