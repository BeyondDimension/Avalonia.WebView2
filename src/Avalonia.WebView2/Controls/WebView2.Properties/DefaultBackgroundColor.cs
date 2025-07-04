#if ANDROID
using Android.Graphics.Drawables;
#endif
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using BD.Avalonia8.Media;
using System.Drawing;

namespace Avalonia.Controls;

partial class WebView2
{
    /// <summary>
    /// 默认背景颜色，白色
    /// </summary>
    static readonly ColorF _defaultBackgroundColorDefaultValue = System.Drawing.Color.White;

    ColorF _defaultBackgroundColor;

    /// <summary>
    /// <see cref="WebView2"/> 的默认背景颜色
    /// </summary>
    public ColorF DefaultBackgroundColor
    {
        get
        {
#if (!DISABLE_WEBVIEW2_CORE && (WINDOWS || NETFRAMEWORK)) || ANDROID || (IOS || MACCATALYST || (MACOS && !USE_DEPRECATED_WEBVIEW))
            var defaultBackgroundColor = GetDefaultBackgroundColor(this);
            if (defaultBackgroundColor.HasValue)
                return defaultBackgroundColor.Value;
#endif
            return _defaultBackgroundColor;
        }

        set
        {
            if (value == _defaultBackgroundColor)
            {
                return;
            }
            _defaultBackgroundColor = value;

#if (!DISABLE_WEBVIEW2_CORE && (WINDOWS || NETFRAMEWORK)) || ANDROID || (IOS || MACCATALYST || (MACOS && !USE_DEPRECATED_WEBVIEW))
            SetDefaultBackgroundColor(this, value);
#endif

            //        var avaColor =
            //global::Avalonia.Media.Color.FromArgb(value.A, value.R, value.G, value.B);
            //        if ((object?)this is Shape shape)
            //        {
            //            shape.Fill = new ImmutableSolidColorBrush(avaColor);
            //        }
        }
    }

    /// <summary>
    /// The <see cref="AvaloniaProperty" /> which backs the <see cref="DefaultBackgroundColor" /> property.
    /// </summary>
    public static readonly DirectProperty<WebView2, ColorF> DefaultBackgroundColorProperty = AvaloniaProperty.RegisterDirect<WebView2, ColorF>(nameof(DefaultBackgroundColor), x => x._defaultBackgroundColor, (x, y) => x.DefaultBackgroundColor = y);
}