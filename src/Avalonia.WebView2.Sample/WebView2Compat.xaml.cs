#if WINDOWS
using Microsoft.Web.WebView2.Core;
#endif
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Net;
using Avalonia.Media;
using IStorageService = Avalonia.Controls.WebView2.IStorageService;
using static Avalonia.Controls.WebView2;

namespace Avalonia.WebView2.Sample;

public sealed partial class WebView2Compat : UserControl, IStorageService
{
    public WebView2Compat()
    {
        InitializeComponent();
        WebView2 = this.FindControl<Controls.WebView2>("WebView2")!;
        TextBlock = this.FindControl<TextBlock>("TextBlock")!;
#if WINDOWS
        WebView2.IsVisible = false;
        // 设置背景色透明
        WebView2.Fill = new SolidColorBrush(Colors.Transparent);
        if (!IsSupported)
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

    IEnumerable<KeyValuePair<(StorageItemType type, string key), StorageItemValue>>? IStorageService.GetStorages(string requestUri)
    {
        // 测试 localStorage 注入
        var now = DateTime.Now;

        var dict = new Dictionary<(StorageItemType type, string key), StorageItemValue>()
        {
            { (StorageItemType.LocalStorage, "global_test"), 2 },
            { (StorageItemType.SessionStorage, "global_test_s"), 7.5 },
            { (StorageItemType.LocalStorage, "global_test_now"), now },
            { (StorageItemType.AllStorage, "global_test_now_str"), now.ToString("yyyy-MM-dd HH:mm:ss.fffffff") },
        };

        foreach (var it in dict)
        {
            yield return it;
        }

        if (bingComDomainPattern.IsMatchOnlyDomain(requestUri))
        {
            var dict2 = new Dictionary<(StorageItemType type, string key), StorageItemValue>()
            {
                { (StorageItemType.LocalStorage, "bing.com"), 4.5f },
                { (StorageItemType.LocalStorage, "bing"), "key4" },
                { (StorageItemType.LocalStorage, "bing3"), now },
                { (StorageItemType.LocalStorage, "bing4"), now.ToString("yyyy-MM-dd HH:mm:ss.fffffff") },
            };

            foreach (var it in dict2)
            {
                yield return it;
            }
        }
    }
}
