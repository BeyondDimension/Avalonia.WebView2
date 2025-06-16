#if !DISABLE_WEBVIEW2_CORE && WINDOWS || NETFRAMEWORK
using Microsoft.Web.WebView2.Core;
#endif
using System.ComponentModel;
using System.Net.Http;

namespace Avalonia.Controls;

partial class WebView2
{
    Uri? _source;

    /// <summary>
    /// Source 属性是 WebView2 顶层文档的 <see cref="Uri"/>
    /// <para>设置源相当于调用 CoreWebView2.Navigate 设置源将触发 CoreWebView2 的初始化（如果尚未初始化）</para>
    /// <para>Source 的默认值为 <see langword="null"/>，表示 CoreWebView2 尚未初始化</para>
    /// </summary>
    /// <exception cref="ArgumentException">指定 <see cref="Uri" /> 值不是 <see cref="UriKind.Absolute"/></exception>
    [Browsable(true)]
    public Uri? Source
    {
        get
        {
#if (!DISABLE_WEBVIEW2_CORE && (WINDOWS || NETFRAMEWORK)) || ANDROID || (IOS || MACCATALYST || (MACOS && !USE_DEPRECATED_WEBVIEW))
            var source = GetSource(this);
            if (source != null)
            {
                return source;
            }
#endif
            return _source;
        }
        set
        {
            if (value == null)
            {
                if (_source == null)
                {
                    return;
                }
                else
                {
                    // 行为更变：源 WebView2 Wpf 中会抛出 new NotImplementedException("The Source property cannot be set to null.")，改为跳转空白页避免 catch
                    value = new Uri("about:blank", UriKind.Absolute);
                }
            }
            else if (!value.IsAbsoluteUri)
            {
                throw new ArgumentException("Only absolute URI is allowed", "Source");
            }
            else if (_source == null ||
                _source.GetType() != value.GetType() || // 允许 Uri 的派生类
                _source.AbsoluteUri != value.AbsoluteUri)
            {
                _htmlSource = null;
                SetAndRaise(SourceProperty, ref _source, value);
#if (!DISABLE_WEBVIEW2_CORE && (WINDOWS || NETFRAMEWORK)) || ANDROID || (IOS || MACCATALYST || (MACOS && !USE_DEPRECATED_WEBVIEW))
                SetSource(this, value);
#endif
            }
#if !DISABLE_WEBVIEW2_CORE && WINDOWS || NETFRAMEWORK
            _implicitInitGate.RunWhenOpen(() => EnsureCoreWebView2Async());
#endif
        }
    }

    /// <summary>
    /// 与 WebResourceRequested 事件一起使用的 HTTP 请求，继承自 <see cref="Uri"/>，可自定义响应内容并赋值给属性 <see cref="Source"/>，默认 <see cref="HttpMethod"/> 为 <see cref="HttpMethod.Get"/>
    /// <para>示例：https://learn.microsoft.com/en-us/dotnet/api/microsoft.web.webview2.core.corewebview2.navigatewithwebresourcerequest</para>
    /// </summary>
    public sealed partial class WebResourceRequestUri(string uriString, Stream? content, HttpMethod? method = null) : Uri(uriString, UriKind.Absolute)
    {
        readonly string uriString = uriString;

        /// <summary>
        /// 以流形式获取或设置 HTTP 请求消息正文
        /// </summary>
        public Stream? Content { get; set; } = content;

        public string? StringContent { get; set; }

        /// <summary>
        /// 获取或设置可变 HTTP 请求头
        /// <para>示例值：Content-Type: application/x-www-form-urlencoded\r\n</para>
        /// <para>https://learn.microsoft.com/en-us/dotnet/api/microsoft.web.webview2.core.corewebview2.navigatewithwebresourcerequest</para>
        /// </summary>
        public string? Headers { get; set; }

        /// <summary>
        /// HTTP 请求方法
        /// </summary>
        public HttpMethod? Method { get; set; } = method;

        internal static string GetMethod(HttpMethod? method) => (method ?? HttpMethod.Get).Method;

#if !DISABLE_WEBVIEW2_CORE && WINDOWS || NETFRAMEWORK
        internal CoreWebView2WebResourceRequest ToRequest(CoreWebView2Environment env)
        {
            var result = env.CreateWebResourceRequest(uriString,
                GetMethod(Method),
                Content,
                Headers);
            return result;
        }
#endif
    }

    /// <summary>
    /// The <see cref="AvaloniaProperty" /> which backs the <see cref="Source" /> property.
    /// </summary>
    public static readonly DirectProperty<WebView2, Uri?> SourceProperty = AvaloniaProperty.RegisterDirect<WebView2, Uri?>(nameof(Source), x => x._source, (x, y) => x.Source = y);
}