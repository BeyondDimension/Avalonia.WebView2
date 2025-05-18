//using System;
//using System.Collections.Generic;
//using System.Collections.Specialized;
//using System.Text;
//using System.Web;

//namespace Avalonia.Controls;

//partial class WebView2
//{

//    WebView2OnDocumentCreatedLoader? _documentCreatedLoader;

//    /// <summary>
//    /// WebView2 DocumentCreated Loader
//    /// </summary>
//    public WebView2OnDocumentCreatedLoader? DocumentCreatedLoader { get => _documentCreatedLoader; set { _documentCreatedLoader = value; } }

//    /// <summary>
//    /// 添加 JavaScript 到 WebView2 的 DocumentCreated 事件中。
//    /// </summary>
//    /// <param name="js"></param>
//    /// <returns></returns>
//    /// <exception cref="ArgumentNullException"></exception>
//    public async Task<string> AddScriptToExecuteOnDocumentCreatedAsync(string js)
//    {
//#if !DISABLE_WEBVIEW2_CORE && (WINDOWS || NETFRAMEWORK)
//        if (_coreWebView2Controller != null)
//        {
//            return await _coreWebView2Controller.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(js);
//        }
//#endif
//        throw new ArgumentNullException();
//    }

//    /// <summary>
//    /// 加载需要载入的 JavaScript 到 WebView2 的 DocumentCreated 事件中。
//    /// </summary>
//    /// <returns></returns>
//    internal async Task InitJavaScriptOnDocumentCreatedAsync()
//    {
//        if (_documentCreatedLoader is not null)
//        {
//            if (_documentCreatedLoader.SessionStorage != null)
//            {
//                var sessionStorage = new NameValueCollection();
//                _documentCreatedLoader.SessionStorage(sessionStorage);
//                await AddSessionStorageOnDocumentCreatedAsync(sessionStorage);
//            }
//            if (_documentCreatedLoader.LocalStorage != null)
//            {
//                var localStorage = new NameValueCollection();
//                _documentCreatedLoader.LocalStorage(localStorage);
//                await AddLocalStorageOnDocumentCreatedAsync(localStorage);
//            }
//        }
//    }

//    /// <summary>
//    /// 添加 LocalStorage 到 WebView2 的 DocumentCreated 事件中。
//    /// </summary>
//    /// <param name="localStorage"></param>
//    /// <returns></returns>
//    internal async Task<bool> AddLocalStorageOnDocumentCreatedAsync(NameValueCollection localStorage)
//        => await SetStorageAsync(localStorageSetFormatter, localStorage);

//    internal async Task<bool> AddSessionStorageOnDocumentCreatedAsync(NameValueCollection sessionStorage)
//        => await SetStorageAsync(sessionStorageSetFormatter, sessionStorage);

//    static string localStorageSetFormatter = "localStorage.setItem('{0}', `{1}`);";
//    static string sessionStorageSetFormatter = "sessionStorage.setItem('{0}', `{1}`);";

//    private async Task<bool> SetStorageAsync(string stringFormatter, NameValueCollection collection)
//    {
//#if !DISABLE_WEBVIEW2_CORE && (WINDOWS || NETFRAMEWORK)
//        if (_coreWebView2Controller != null)
//        {

//            foreach (var key in collection.AllKeys)
//            {
//                var value = collection[key];
//                var js = string.Format(stringFormatter, key, value);
//                await _coreWebView2Controller.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(js);
//            }
//            return true;
//        }
//#endif
//        return false;
//    }

//    /// <summary>
//    /// WebView2 DocumentCreated Loader，for example SessionStorage 和 LocalStorage。
//    /// </summary>
//    public class WebView2OnDocumentCreatedLoader
//    {
//        /// <summary>
//        /// SessionStorage Load OnDocumentCreated.
//        /// </summary>
//        public Action<NameValueCollection>? SessionStorage { get; set; }

//        /// <summary>
//        /// LocalStorage Load OnDocumentCreated.
//        /// </summary>
//        public Action<NameValueCollection>? LocalStorage { get; set; }
//    }
//}
