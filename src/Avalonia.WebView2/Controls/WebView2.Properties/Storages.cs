using Avalonia.Platform.Storage;

namespace Avalonia.Controls;

partial class WebView2
{
    /// <inheritdoc cref="IStorageService"/>
    public IStorageService? StorageService { get; set; }

    /// <summary>
    /// 处理注入 Web Storage 生成的 JS 内容
    /// </summary>
    protected static string? HandlerStorageServiceGenerateJSString(IStorageService? storageService, string requestUri)
    {
        if (storageService != null)
        {
            var value = storageService.GetStorages(requestUri);
            if (value != null)
            {
                var js = StorageItemValue.ToJavaScriptString(value);
                if (!string.IsNullOrWhiteSpace(js))
                {
                    return js;
                }
            }
        }
        return null;
    }
}
