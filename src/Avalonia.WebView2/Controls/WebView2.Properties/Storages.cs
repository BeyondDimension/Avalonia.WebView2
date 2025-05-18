using Avalonia.Platform.Storage;

namespace Avalonia.Controls;

partial class WebView2
{
    /// <inheritdoc cref="IWebView2StorageService"/>
    public IWebView2StorageService? StorageService { get; set; }

    /// <summary>
    /// 处理注入 Web Storage 生成的 JS 内容
    /// </summary>
    static string? HandlerStorageServiceGenerateJSString(IWebView2StorageService? storageService, string requestUri)
    {
        if (storageService != null)
        {
            var value = storageService.GetStorages(requestUri);
            if (value != null)
            {
                var js = WebView2StorageItemValue.ToJavaScriptString(value);
                if (!string.IsNullOrWhiteSpace(js))
                {
                    return js;
                }
            }
        }
        return null;
    }
}
