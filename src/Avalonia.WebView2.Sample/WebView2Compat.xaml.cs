#if WINDOWS
using Microsoft.Web.WebView2.Core;
#endif
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using System.Net;
using Avalonia.Media;

namespace Avalonia.WebView2.Sample;

public sealed partial class WebView2Compat : UserControl, IWebView2StorageService
{
    public WebView2Compat()
    {
        InitializeComponent();
        WebView2 = this.FindControl<global::Avalonia.Controls.WebView2>("WebView2")!;
        WebView2.IsVisible = false;
        TextBlock = this.FindControl<TextBlock>("TextBlock")!;
#if WINDOWS
        // 设置背景色透明
        WebView2.Fill = new SolidColorBrush(Colors.Transparent);
        if (!global::Avalonia.Controls.WebView2.IsSupported)
        {
            TextBlock.Text = "Couldn't find a compatible Webview2 Runtime installation to host WebViews.";
        }
        else
        {
            WebView2.DOMContentLoaded += WebView2_DOMContentLoaded;
            //WebView2.DocumentCreatedLoader = new Controls.WebView2.WebView2OnDocumentCreatedLoader
            //{
            //    LocalStorage = (options) => { },
            //    SessionStorage = (options) => { },
            //};
        }
        WebView2.StorageService = this;
#endif
    }

#if WINDOWS
    void WebView2_DOMContentLoaded(object? sender, CoreWebView2DOMContentLoadedEventArgs e)
    {
        WebView2.IsVisible = true;
    }
#endif

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    readonly DomainPattern bingComDomainPattern = new("*bing.com");

    IEnumerable<KeyValuePair<(WebView2StorageItemType type, string key), WebView2StorageItemValue>>? IWebView2StorageService.GetStorages(string requestUri)
    {
        // 测试 localStorage 注入
        var now = DateTime.Now;

        var dict = new Dictionary<(WebView2StorageItemType type, string key), WebView2StorageItemValue>()
        {
            { (WebView2StorageItemType.LocalStorage, "global_test"), 2 },
            { (WebView2StorageItemType.SessionStorage, "global_test_s"), 7.5 },
            { (WebView2StorageItemType.LocalStorage, "global_test_now"), now },
            { (WebView2StorageItemType.AllStorage, "global_test_now_str"), now.ToString("yyyy-MM-dd HH:mm:ss.fffffff") },
        };

        foreach (var it in dict)
        {
            yield return it;
        }

        if (bingComDomainPattern.IsMatchOnlyDomain(requestUri))
        {
            var dict2 = new Dictionary<(WebView2StorageItemType type, string key), WebView2StorageItemValue>()
            {
                { (WebView2StorageItemType.LocalStorage, "bing.com"), 4.5f },
                { (WebView2StorageItemType.LocalStorage, "bing"), "key4" },
                { (WebView2StorageItemType.LocalStorage, "bing3"), now },
                { (WebView2StorageItemType.LocalStorage, "bing4"), now.ToString("yyyy-MM-dd HH:mm:ss.fffffff") },
            };

            foreach (var it in dict2)
            {
                yield return it;
            }
        }
    }
}
