using System.ComponentModel;

namespace Avalonia.Controls;

partial class WebView2
{
    string? _htmlSource;

    /// <summary>
    /// HtmlSource 属性是 WebView2 顶层文档的 <see cref="string"/> HTML 内容字符串
    /// 总大小不得超过 2 MB（2 * 1024 * 1024 字节）
    /// <para>设置源相当于调用 CoreWebView2.Navigate 设置源将触发 CoreWebView2 的初始化（如果尚未初始化）</para>
    /// <para>HtmlSource 的默认值为 <see langword="null"/> ，表示 CoreWebView2 尚未初始化</para>
    /// </summary>
    [Browsable(true)]
    public string? HtmlSource
    {
        get => _htmlSource;
        set
        {
            if (value == null)
            {
                if (_htmlSource == null)
                {
                    return;
                }
                else
                {
                    // 行为更变：源 WebView2 Wpf 中会抛出 new NotImplementedException("The HtmlSource property cannot be set to null.")，改为空字符串避免 catch
                    value = string.Empty;
                }
            }
            else
            {
                if (_htmlSource == null || _htmlSource != value)
                {
                    _source = null;
                    SetAndRaise(HtmlSourceProperty, ref _htmlSource, value);
#if !DISABLE_WEBVIEW2_CORE && WINDOWS || NETFRAMEWORK
                    CoreWebView2?.NavigateToString(value);
#elif ANDROID
#elif IOS
#else
                    // CEF_TODO: 待实现 NavigateToString
#endif
                }
#if !DISABLE_WEBVIEW2_CORE && WINDOWS || NETFRAMEWORK
                _implicitInitGate.RunWhenOpen(() => EnsureCoreWebView2Async());
#endif
            }
        }
    }

    /// <summary>
    /// The <see cref="AvaloniaProperty" /> which backs the <see cref="HtmlSource" /> property.
    /// </summary>
    public static readonly DirectProperty<WebView2, string?> HtmlSourceProperty = AvaloniaProperty.RegisterDirect<WebView2, string?>(nameof(HtmlSource), x => x._htmlSource, (x, y) => x.HtmlSource = y);
}