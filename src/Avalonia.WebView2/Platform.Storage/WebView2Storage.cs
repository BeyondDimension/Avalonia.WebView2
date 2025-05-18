//using System.Net;

//namespace Avalonia.Platform.Storage;

///// <summary>
///// Web Storage API 的数据模型类
///// </summary>
///// <param name="domainPattern">要注入的域名匹配表达式</param>
///// <param name="values">要注入的数据</param>
//public sealed class WebView2Storage(string domainPattern, params IEnumerable<KeyValuePair<(WebView2StorageItemType type, string key), WebView2StorageItemValue>> values)
//{
//    internal DomainPattern DomainPattern { get; } = new(domainPattern);

//    internal IEnumerable<KeyValuePair<(WebView2StorageItemType type, string key), WebView2StorageItemValue>> Values => values;

//    /// <inheritdoc/>
//    public override string ToString()
//    {
//        var result = WebView2StorageItemValue.ToJavaScriptString(values);
//        return result;
//    }
//}
