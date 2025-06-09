using System.Net.Http;
using System.Threading.Tasks;

namespace Avalonia.Controls;

partial class WebView2
{
    /// <summary>
    /// Renders the provided HTML as the top level document of the WebView2.
    /// This is equivalent to CoreWebView2.NavigateToString.
    /// </summary>
    /// <exception cref="InvalidOperationException">The underlying WebView2.CoreWebView2 is not yet initialized.</exception>
    /// <exception cref="InvalidOperationException">Thrown when browser process has unexpectedly and left this control in an invalid state. We are considering throwing a different type of exception for this case in the future.</exception>
    /// <remarks>The htmlContent parameter may not be larger than 2 MB (2 * 1024 * 1024 bytes) in total size. The origin of the new page is about:blank.</remarks>
    public void NavigateToString(string htmlContent)
    {
#if !DISABLE_WEBVIEW2_CORE && (WINDOWS || NETFRAMEWORK)
        VerifyBrowserNotCrashedGuard();
        CoreWebView2?.NavigateToString(htmlContent);
#elif ANDROID
        var aWebView = AWebView;
        if (aWebView != null)
        {
            aWebView.LoadData(htmlContent, "text/html", "UTF-8");
        }
#elif IOS || MACOS || MACCATALYST
        var wkWebView = WKWebView;
        if (wkWebView != null)
        {
            wkWebView.LoadHtmlString(htmlContent, (NSUrl?)null!);
        }
#else
        // CEF_TODO: 待实现 NavigateToString
#endif
    }

    public void Navigate(string uri)
    {
        _source = null;
        _htmlSource = null;

#if !DISABLE_WEBVIEW2_CORE && (WINDOWS || NETFRAMEWORK)
        VerifyBrowserNotCrashedGuard();
        CoreWebView2?.Navigate(uri);
#elif ANDROID
        var aWebView = AWebView;
        aWebView?.LoadUrl(uri);
#elif IOS || MACOS || MACCATALYST
        var wkWebView = WKWebView;
        if (wkWebView != null)
        {
#if DEBUG
            Console.WriteLine($"Navigate: {uri}");
            Console.WriteLine($"width: {wkWebView.Bounds.Width}, height: {wkWebView.Bounds.Height}");
#endif
            NSUrl nsUrl = new(uri);
            NSUrlRequest nsUrlRequest = new(nsUrl);
            wkWebView.LoadRequest(nsUrlRequest);
        }
#else
        // CEF_TODO: 待实现 Navigate
#endif
    }

    public void Navigate(string uri, Stream? content, HttpMethod? method = null, string? headers = null)
    {
#if !DISABLE_WEBVIEW2_CORE && (WINDOWS || NETFRAMEWORK)
        VerifyBrowserNotCrashedGuard();
        if (CoreWebView2 != null)
        {
            var req = CoreWebView2.Environment.CreateWebResourceRequest(uri, WebResourceRequestUri.GetMethod(method), content, headers);
            CoreWebView2.NavigateWithWebResourceRequest(req);
        }
#elif LINUX
        // CEF_TODO: 待实现 Navigate
#elif IOS || MACOS || MACCATALYST

#elif ANDROID
#endif
    }

    /// <summary>
    /// Reloads the top level document of the <see cref="WebView2" />.
    /// This is equivalent to CoreWebView2.Reload.
    /// </summary>
    /// <exception cref="System.InvalidOperationException">The underlying WebView2.CoreWebView2 is not yet initialized.</exception>
    /// <exception cref="System.InvalidOperationException">Thrown when browser process has unexpectedly and left this control in an invalid state. We are considering throwing a different type of exception for this case in the future.</exception>
    public void Reload()
    {
#if !DISABLE_WEBVIEW2_CORE && (WINDOWS || NETFRAMEWORK)
        VerifyBrowserNotCrashedGuard();
        CoreWebView2?.Reload();
#elif LINUX
        // CEF_TODO: 待实现 Reload
#elif IOS || MACOS || MACCATALYST
        WKWebView?.Reload();
#elif ANDROID

#endif
    }

    /// <summary>
    /// Stops any in progress navigation in the <see cref="WebView2" />.
    /// This is equivalent to CoreWebView2.Stop.
    /// If the underlying WebView2.CoreWebView2 is not yet initialized, this method does nothing.
    /// </summary>
    public void Stop()
    {
#if !DISABLE_WEBVIEW2_CORE && (WINDOWS || NETFRAMEWORK)
        VerifyBrowserNotCrashedGuard();
        CoreWebView2?.Stop();
#elif LINUX
        // CEF_TODO: 待实现 Stop
#elif IOS || MACOS || MACCATALYST
        WKWebView?.StopLoading();
#elif ANDROID
#endif
    }

    /// <summary>
    /// Evaluates the script that is specified by script.
    /// </summary>
    /// <param name="script">A script to evaluate.</param>
    public async void Eval(string script)
    {
        // 兼容 .NET MAUI IWebView API
        await ExecuteScriptAsync(script);
    }

    /// <summary>
    /// On platforms that support JavaScript evaluation, evaluates script.
    /// </summary>
    /// <param name="script">The script to evaluate.</param>
    /// <returns>A task that contains the result of the evaluation as a string.</returns>
    public Task<string?> EvaluateJavaScriptAsync(string script)
    {
        // 兼容 .NET MAUI IWebView API
        var result = ExecuteScriptAsync(script);
        return result;
    }

    /// <summary>
    /// Executes the provided script in the top level document of the <see cref="WebView2" />.
    /// This is equivalent to CoreWebView2.ExecuteScriptAsync.
    /// </summary>
    /// <exception cref="InvalidOperationException">The underlying WebView2.CoreWebView2 is not yet initialized.</exception>
    /// <exception cref="InvalidOperationException">Thrown when browser process has unexpectedly and left this control in an invalid state. We are considering throwing a different type of exception for this case in the future.</exception>
    public async Task<string?> ExecuteScriptAsync(string script)
    {
#if !DISABLE_WEBVIEW2_CORE && (WINDOWS || NETFRAMEWORK)
        VerifyBrowserNotCrashedGuard();
        if (CoreWebView2 != null)
        {
            return await CoreWebView2.ExecuteScriptAsync(script);
        }
#elif LINUX
        // CEF_TODO: 待实现 ExecuteScriptAsync
#elif IOS || MACOS || MACCATALYST
        if (WKWebView != null)
        {
            var result = await WKWebView.EvaluateJavaScriptAsync(script);
            return result?.ToString();
        }
#endif
        return (string?)null;
    }

    ///// <summary>
    ///// For internal use by the .NET MAUI platform.
    ///// Raised after web navigation begins.
    ///// </summary>
    //public bool Navigating(WebNavigationEvent evnt, string url)
    //{
    //    // 兼容 .NET MAUI IWebView API
    //    throw new NotImplementedException();
    //}

    ///// <summary>
    ///// For internal use by the .NET MAUI platform.
    ///// Raised after web navigation completes.
    ///// </summary>
    //public void Navigated(WebNavigationEvent evnt, string url, WebNavigationResult result)
    //{
    //    // 兼容 .NET MAUI IWebView API
    //    throw new NotImplementedException();
    //}

    //public void ProcessTerminated(WebProcessTerminatedEventArgs args)
    //{
    //    // 兼容 .NET MAUI IWebView API
    //    throw new NotImplementedException();
    //}
}