#if IOS || MACCATALYST || (MACOS && !USE_DEPRECATED_WEBVIEW)
using Avalonia.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.Controls;
partial class WebView2
{
    public void LoadUrl(string? url)
    {
        LoadUrlAsync(url).FireAndForget();
    }

    async Task LoadUrlAsync(string? url)
    {
        if (PlatformWebView is null)
            return;

        try
        {
            var uri = new Uri(url ?? string.Empty);
            var safeHostUri = new Uri($"{uri.Scheme}://{uri.Authority}", UriKind.Absolute);
            var safeRelativeUri = new Uri($"{uri.PathAndQuery}{uri.Fragment}", UriKind.Relative);
            var safeFullUri = new Uri(safeHostUri, safeRelativeUri);
            NSUrlRequest request = new NSUrlRequest(new NSUrl(safeFullUri.AbsoluteUri));

            if (HasCookiesToLoad(safeFullUri.AbsoluteUri) &&
                !(OperatingSystem.IsIOSVersionAtLeast(11) || OperatingSystem.IsTvOSVersionAtLeast(11)))
            {
                return;
            }

            await SyncCookieToPlatformWebView(safeFullUri.AbsoluteUri);

            PlatformWebView.LoadRequest(request);
        }
        catch (UriFormatException formatException)
        {
            // If we got a format exception trying to parse the URI, it might be because
            // someone is passing in a local bundled file page. If we can find a better way
            // to detect that scenario, we should use it; until then, we'll fall back to 
            // local file loading here and see if that works:
            if (!string.IsNullOrEmpty(url))
            {
                if (!PlatformWebView.LoadFile(url))
                {
                    Logger.Sink?.Log(LogEventLevel.Warning, "WebView2", PlatformWebView,
                        $"Unable to Load Url {url}: {formatException}");
                }
            }
        }
        catch (Exception exc)
        {
            Logger.Sink?.Log(LogEventLevel.Warning, "WebView2", PlatformWebView,
                $"Unable to Load Url {url}: {exc}");
        }
    }
}
#endif
