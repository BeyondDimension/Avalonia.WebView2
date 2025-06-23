#if !DISABLE_WEBVIEW2_CORE && (WINDOWS || NETFRAMEWORK)
using Microsoft.Web.WebView2.Core;
using System.Collections.Concurrent;

namespace Avalonia.Controls;

partial class WebView2
{
    // https://learn.microsoft.com/zh-cn/microsoft-edge/webview2/concepts/navigation-events

    readonly ConcurrentDictionary<ulong, string> dictNavigationStarting = new();

    public string? GetRequestUri(ulong navigationId)
    {
        if (dictNavigationStarting.TryGetValue(navigationId, out var value))
        {
            return value;
        }
        return null;
    }

    void CoreWebView2_NavigationStarting(object? sender, CoreWebView2NavigationStartingEventArgs e)
    {
        try
        {
            var navigationStarting = NavigationStarting;
            if (navigationStarting == null)
            {
                return;
            }
            navigationStarting(this, e);
        }
        finally
        {
            if (!e.Cancel)
            {
                dictNavigationStarting.TryAdd(e.NavigationId, e.Uri);
            }
        }
    }

    void CoreWebView2_NavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
    {
        try
        {
            var navigationCompleted = NavigationCompleted;
            if (navigationCompleted == null)
            {
                return;
            }
            navigationCompleted(this, e);
        }
        finally
        {
            if (_source != null)
            {
                SyncPlatformCookiesToWebView2(_source.OriginalString).FireAndForget();
            }

            dictNavigationStarting.TryRemove(e.NavigationId, out var _);

            SetAndRaise(CanGoBackProperty, ref _canGoBack, GetCanGoBack(this) ?? false);
            SetAndRaise(CanGoForwardProperty, ref _canGoForward, GetCanGoForward(this) ?? false);
        }
    }

    void CoreWebView2_WebResourceRequested(object? sender, CoreWebView2WebResourceRequestedEventArgs e)
    {
        var webResourceRequested = WebResourceRequested;
        if (webResourceRequested == null)
        {
            return;
        }
        webResourceRequested(this, e);
    }

    void CoreWebView2_WebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
    {
        var webMessageReceived = WebMessageReceived;
        if (webMessageReceived == null)
        {
            return;
        }
        webMessageReceived(this, e);
    }

    void CoreWebView2_SourceChanged(object? sender, CoreWebView2SourceChangedEventArgs e)
    {
        var source = CoreWebView2?.Source;
        if (_source == null || _source.AbsoluteUri != source)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                source = "about:blank";
            }
            _source = new Uri(source, UriKind.Absolute);
        }
        var sourceChanged = SourceChanged;
        if (sourceChanged == null)
        {
            return;
        }
        sourceChanged(this, e);
    }

    void CoreWebView2_ContentLoading(object? sender, CoreWebView2ContentLoadingEventArgs e)
    {
        try
        {
            var contentLoading = ContentLoading;
            if (contentLoading == null)
            {
                return;
            }
            contentLoading(this, e);
        }
        finally
        {
            if (!e.IsErrorPage && dictNavigationStarting.TryGetValue(e.NavigationId,
                out var requestUri))
            {
                if (!_browserCrashed && !disposedValue)
                {
                    var coreWebView2 = CoreWebView2;
                    if (coreWebView2 != null)
                    {
                        var js = HandlerStorageServiceGenerateJSString(StorageService, requestUri);
                        if (js != null)
                        {
                            coreWebView2.ExecuteScriptAsync(js);
                        }
                    }
                }
            }
        }
    }

    void CoreWebView2_DOMContentLoaded(object? sender, CoreWebView2DOMContentLoadedEventArgs e)
    {
        var contentLoading = DOMContentLoaded;
        if (contentLoading == null)
        {
            return;
        }
        contentLoading(this, e);
    }

    void CoreWebView2_DocumentTitleChanged(object? sender, object e)
    {
        var cocumentTitleChanged = DocumentTitleChanged;
        if (cocumentTitleChanged == null)
        {
            return;
        }
        cocumentTitleChanged(this, e);
    }

    void CoreWebView2_NewWindowRequested(object? sender, CoreWebView2NewWindowRequestedEventArgs args)
    {
        var newWindowRequested = NewWindowRequested;
        if (newWindowRequested == null)
        {
            if (sender is CoreWebView2 coreWebView2)
            {
                WebView2 webView2;
                var bounds = GetBoundsRectangle();
                var window = new Window()
                {
                    Width = bounds.Width,
                    Height = bounds.Height,
                    Content = webView2 = new WebView2()
                    {
                        Source = new Uri(args.Uri, UriKind.Absolute),
                        //EnabledDevTools = coreWebView2.Settings.AreDevToolsEnabled,
                        //Fill = Fill,
                        //UserDataFolder = coreWebView2.Environment.UserDataFolder,
                    },
                };
                webView2.DOMContentLoaded += (s, e) =>
                {
                    if (s is CoreWebView2 sender)
                    {
                        args.NewWindow = sender;
                    }
                };
                webView2.DocumentTitleChanged += async (s, e) =>
                {
                    window.Title = webView2.CoreWebView2!.DocumentTitle;
                    window.Icon = new WindowIcon(await webView2.CoreWebView2!.GetFaviconAsync(CoreWebView2FaviconImageFormat.Png));
                };
                window.Show();
                args.Handled = true;
            }
            return;
        }
        newWindowRequested(this, args);
    }

    void CoreWebView2_ProcessFailed(object? sender, CoreWebView2ProcessFailedEventArgs e)
    {
        if (e.ProcessFailedKind != CoreWebView2ProcessFailedKind.BrowserProcessExited)
        {
            return;
        }
        UnsubscribeHandlersAndCloseController(true);
    }

    void CoreWebView2Controller_ZoomFactorChanged(object? sender, object e)
    {
        if (_coreWebView2Controller != null)
        {
            _zoomFactor = _coreWebView2Controller.ZoomFactor;
        }
        var zoomFactorChanged = ZoomFactorChanged;
        if (zoomFactorChanged == null)
        {
            return;
        }
        zoomFactorChanged(this, EventArgs.Empty);
    }
}
#endif