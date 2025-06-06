using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using static Avalonia.Controls.WebView2;

namespace Avalonia.WebView2.Sample.Views;

public partial class MainView : UserControl, IStorageService
{
    public MainView()
    {
        InitializeComponent();

        Button = this.FindControl<Button>("Button");
        Button.Click += ButtonClick;


        WebView2 = this.FindControl<Controls.WebView2>("WebView2")!;
        WebView2.StorageService = this;
    }

    void ButtonClick(object? sender, RoutedEventArgs e)
    {
        if (WebView2 is not null)
        {
            var url = UrlTextBox.Text;
            if (!IsHttpUrl(url)) url = $"{Prefix_HTTPS}{url}";
            WebView2?.Navigate(url);
        }
    }

    static bool IsHttpUrl([NotNullWhen(true)] string? url, bool httpsOnly = false) => url != null &&
   (url.StartsWith(Prefix_HTTPS, StringComparison.OrdinalIgnoreCase) ||
         (!httpsOnly && url.StartsWith(Prefix_HTTP, StringComparison.OrdinalIgnoreCase)));

    const string Prefix_HTTPS = "https://";
    const string Prefix_HTTP = "http://";


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