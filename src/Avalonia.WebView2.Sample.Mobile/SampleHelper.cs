using Avalonia.Input;
using System.Net;
using static Avalonia.Controls.WebView2;
using WV2 = Avalonia.Controls.WebView2;

namespace Avalonia.WebView2.Sample;

static partial class SampleHelper
{
    internal static void Navigate(WV2 wv2, string? url)
    {
        if (!string.IsNullOrWhiteSpace(url))
        {
            if (string.Equals("t", url, StringComparison.OrdinalIgnoreCase) ||
                string.Equals("test", url, StringComparison.OrdinalIgnoreCase) ||
                url.All(static x => x == ' ' || x == '\t' || x == '\r' || x == '\n' || x == default || char.ToLowerInvariant(x) == 't'))
            {
                wv2.Source = new WebResourceRequestUri("https://wv2.bing.com", null)
                {
                    StringContent = IndexHtml,
                };
                return;
            }
            else if (url.Contains('.'))
            {
                if (!url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    url = $"https://{url}";
                }
                wv2.Navigate(url);
            }
        }
    }

    const string IndexHtml = @"
<!DOCTYPE html>
<html>
<head>
    <title>WebView2 存储注入测试</title>
    <style>
        body { font-family: Arial, sans-serif; margin: 20px; }
        h2 { color: #333; }
        table { border-collapse: collapse; width: 100%; }
        th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }
        th { background-color: #f2f2f2; }
    </style>
</head>
<body>
    <h1>WebView2 存储注入测试</h1>
    
    <h2>localStorage 内容:</h2>
    <table id='localStorageTable'>
        <tr><th>键</th><th>值</th><th>类型</th></tr>
    </table>
    
    <h2>sessionStorage 内容:</h2>
    <table id='sessionStorageTable'>
        <tr><th>键</th><th>值</th><th>类型</th></tr>
    </table>
    
    <script>
        function displayStorage(storageType, tableId) {
            const table = document.getElementById(tableId);
            const storage = storageType === 'local' ? localStorage : sessionStorage;

            if (storageType === 'session')
            {
                var b = sessionStorage.getItem('global_test_s');
                alert(b);
            }

            for(let i = 0; i < storage.length; i++) {
                const key = storage.key(i);
                const value = storage.getItem(key);
                
                const row = table.insertRow();
                const keyCell = row.insertCell(0);
                const valueCell = row.insertCell(1);
                const typeCell = row.insertCell(2);
                
                keyCell.textContent = key;
                valueCell.textContent = value;
                typeCell.textContent = typeof value;
            }
            
            if(storage.length === 0) {
                const row = table.insertRow();
                const cell = row.insertCell(0);
                cell.colSpan = 3;
                cell.textContent = '没有存储项';
            }
        }
        
        // 页面加载后显示存储内容
        window.onload = function() {
            try
            {
                displayStorage('local', 'localStorageTable');
                displayStorage('session', 'sessionStorageTable');
            }
            catch (error)
            {
                alert(error);
            }
        };
    </script>
</body>
</html>
";

    static readonly DomainPattern bingComDomainPattern = new("*bing.com");

    internal static IEnumerable<KeyValuePair<(StorageItemType type, string key), StorageItemValue>>? GetStorages(string requestUri)
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
