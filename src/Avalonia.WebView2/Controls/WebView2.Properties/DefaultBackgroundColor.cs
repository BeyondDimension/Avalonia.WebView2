using Avalonia.Controls.Shapes;
using Avalonia.Media.Immutable;
using System.Drawing;

namespace Avalonia.Controls;

partial class WebView2
{
    /// <summary>
    /// 默认背景颜色，白色
    /// </summary>
    static readonly Color _defaultBackgroundColorDefaultValue = Color.White;

    Color _defaultBackgroundColor;

    void SetDefaultBackgroundColor(Color value)
    {
        _defaultBackgroundColor = value;
#if !DISABLE_WEBVIEW2_CORE && WINDOWS || NETFRAMEWORK
        if (_coreWebView2Controller == null)
        {
        }
        else
        {
            _coreWebView2Controller.DefaultBackgroundColor = value;
        }
#else
        // CEF_TODO: 待实现 DefaultBackgroundColor
#endif
        var avaColor =
            global::Avalonia.Media.Color.FromArgb(value.A, value.R, value.G, value.B);
        if (this is Shape shape)
        {
            shape.Fill = new ImmutableSolidColorBrush(avaColor);
        }
    }

    /// <summary>
    /// <see cref="WebView2"/> 的默认背景颜色
    /// </summary>
    public Color DefaultBackgroundColor
    {
        get
        {
#if !DISABLE_WEBVIEW2_CORE && WINDOWS || NETFRAMEWORK
            if (_coreWebView2Controller != null)
            {
                return _coreWebView2Controller.DefaultBackgroundColor;
            }
#else
            // CEF_TODO: 待实现 DefaultBackgroundColor
#endif
            return _defaultBackgroundColor;
        }

        set
        {
            if (value == _defaultBackgroundColor)
            {
                return;
            }
            SetDefaultBackgroundColor(value);
        }
    }

    /// <summary>
    /// The <see cref="AvaloniaProperty" /> which backs the <see cref="DefaultBackgroundColor" /> property.
    /// </summary>
    public static readonly DirectProperty<WebView2, Color> DefaultBackgroundColorProperty = AvaloniaProperty.RegisterDirect<WebView2, Color>(nameof(DefaultBackgroundColor), x => x._defaultBackgroundColor, (x, y) => x.DefaultBackgroundColor = y);
}