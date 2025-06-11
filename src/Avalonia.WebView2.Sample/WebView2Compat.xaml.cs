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
#if WINDOWS
        WebView2.IsVisible = false;
        // 设置背景色透明
        //WebView2.Fill = new SolidColorBrush(Colors.Transparent);
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

    IEnumerable<KeyValuePair<(StorageItemType type, string key), StorageItemValue>>? IStorageService.GetStorages(string requestUri)
    {
        var result = SampleHelper.GetStorages(requestUri);
        return result;
    }
}
