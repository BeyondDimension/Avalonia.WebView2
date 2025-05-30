#if !DISABLE_WEBVIEW2_CORE && WINDOWS || NETFRAMEWORK
using System.ComponentModel;

namespace Avalonia.Controls;

partial class WebView2
{
    CoreWebView2CreationProperties? _creationProperties;

    /// <summary>
    /// Gets or sets a bag of options which are used during initialization of the control's WebView2.CoreWebView2.
    /// This property cannot be modified (an exception will be thrown) after initialization of the control's CoreWebView2 has started.
    /// 获取或设置在初始化控件 WebView2.CoreWebView2 时使用的一组选项。
    /// 在控件的 CoreWebView2 初始化开始后，该属性不可修改（会出现异常）。
    /// </summary>
    /// <exception cref="global::System.InvalidOperationException">
    /// Thrown if initialization of the control's CoreWebView2 has already started.
    /// 如果控件的 CoreWebView2 已开始初始化，则抛出该抛掷值。
    /// </exception>
    [Browsable(false)]
    public CoreWebView2CreationProperties? CreationProperties
    {
        get => _creationProperties;
        set
        {
#if !DISABLE_WEBVIEW2_CORE && WINDOWS || NETFRAMEWORK
            if (_initTask != null)
            {
                // 在 CoreWebView2 初始化开始后，不能修改 CreationProperties
                throw new InvalidOperationException(
                    "CreationProperties cannot be modified after the initialization of CoreWebView2 has begun.");
            }
#endif
            _creationProperties = value;
        }
    }

    /// <summary>
    /// The <see cref="AvaloniaProperty" /> which backs the <see cref="CreationProperties" /> property.
    /// </summary>
    /// <seealso cref="AvaloniaProperty" />
    public static readonly DirectProperty<WebView2, CoreWebView2CreationProperties?> CreationPropertiesProperty = AvaloniaProperty.RegisterDirect<WebView2, CoreWebView2CreationProperties?>(nameof(CreationProperties),
#if !DISABLE_WEBVIEW2_CORE && WINDOWS || NETFRAMEWORK
        x => x.env != null ? x._creationProperties : null,
#else
        x => x._creationProperties,
#endif
        (x, y) => x.CreationProperties = y);
}
#endif