namespace Avalonia.Platform.Storage;

/// <summary>
/// Web Storage 提供了访问特定域名下的会话存储或本地存储的功能，例如，可以添加、修改或删除存储的数据项。
/// <para>如果你想要操作一个域名的会话存储，可以使用 Window.sessionStorage；如果想要操作一个域名的本地存储，可以使用 Window.localStorage。</para>
/// </summary>
public interface IWebView2StorageService
{
    /// <summary>
    /// 根据请求地址获取存储数据
    /// </summary>
    /// <returns></returns>
    IEnumerable<KeyValuePair<(WebView2StorageItemType type, string key), WebView2StorageItemValue>>? GetStorages(string requestUri);
}
