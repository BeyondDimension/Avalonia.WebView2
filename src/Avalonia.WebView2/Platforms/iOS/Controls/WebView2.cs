#if IOS
using Avalonia.iOS;
#endif
#if IOS || MACCATALYST || (MACOS && !USE_DEPRECATED_WEBVIEW)
using Avalonia.Controls.Platform;
using Avalonia.Platform;
using System.Diagnostics.CodeAnalysis;
using WebKit;
using Avalonia.LogicalTree;
using Avalonia.Controls.Presenters;
using Avalonia.Metadata;
using System.Threading.Tasks;
using System.Drawing;
using Avalonia.Logging;

namespace Avalonia.Controls;

partial class WebView2
{
    /// <summary>
    /// https://github.com/dotnet/maui/blob/9.0.70/src/Core/src/Handlers/WebView/WebViewHandler.iOS.cs
    /// </summary>
    partial class Handler
    {
        public WKWebView? PlatformView => wv2.platformHandle?.WebView;

        protected virtual float MinimumSize => 44f;

        protected virtual IWKNavigationDelegate CreateWKNavigationDelegate()
            => new WKNavigationDelegate(this);

        /// <summary>
        /// https://developer.apple.com/documentation/webkit/wkwebview
        /// </summary>
        /// <returns></returns>
        public virtual WKWebView CreatePlatformView()
        {
            var config = WKWebView2.CreateConfiguration();
#if DEBUG
            config.Preferences.SetValueForKey(NSObject.FromObject(true), new NSString("developerExtrasEnabled"));
#endif
            var webView = new WKWebView2(RectangleF.Empty, this, config)
            {
                NavigationDelegate = CreateWKNavigationDelegate(),
            };
#if DEBUG
            if (OperatingSystem.IsIOSVersionAtLeast(16, 4) || OperatingSystem.IsMacCatalystVersionAtLeast(16, 6))
            {
                // Enable Developer Extras for iOS builds for 16.4+ and Mac Catalyst builds for 16.6 (macOS 13.5)+
                webView.SetValueForKey(NSObject.FromObject(true), new NSString("inspectable"));
            }
#endif
            return webView;
        }

#if !MACOS
        /// <summary>
        /// https://github.com/dotnet/maui/blob/9.0.70/src/Core/src/Handlers/ViewHandlerExtensions.iOS.cs#L66
        /// </summary>
        protected Size GetDesiredSizeFromHandler(WKWebView platformView, WebView2 virtualView, double widthConstraint, double heightConstraint)
        {
            if (platformView == null || virtualView == null)
            {
                return new Size(widthConstraint, heightConstraint);
            }

            // The measurements ran in SizeThatFits percolate down to child views
            // So if MaximumWidth/Height are not taken into account for constraints, the children may have wrong dimensions
            widthConstraint = Math.Min(widthConstraint, virtualView.MaxWidth);
            heightConstraint = Math.Min(heightConstraint, virtualView.MaxHeight);

            CGSize sizeThatFits;

            // Calling SizeThatFits on an ImageView always returns the image's dimensions, so we need to call the extension method
            // This also affects ImageButtons
            //if (platformView is UIImageView imageView)
            //{
            //    widthConstraint = IsExplicitSet(virtualView.Width) ? virtualView.Width : widthConstraint;
            //    heightConstraint = IsExplicitSet(virtualView.Height) ? virtualView.Height : heightConstraint;

            //    sizeThatFits = imageView.SizeThatFitsImage(new CGSize((float)widthConstraint, (float)heightConstraint));
            //}
            //else if (platformView is WrapperView wrapper)
            //{
            //    sizeThatFits = wrapper.SizeThatFitsWrapper(new CGSize((float)widthConstraint, (float)heightConstraint), virtualView.Width, virtualView.Height, virtualView);
            //}
            //else if (platformView is UIButton imageButton && imageButton.ImageView?.Image is not null && imageButton.CurrentTitle is null)
            //{
            //    widthConstraint = IsExplicitSet(virtualView.Width) ? virtualView.Width : widthConstraint;
            //    heightConstraint = IsExplicitSet(virtualView.Height) ? virtualView.Height : heightConstraint;

            //    sizeThatFits = imageButton.ImageView.SizeThatFitsImage(new CGSize((float)widthConstraint, (float)heightConstraint));
            //}
            //else
            {
                sizeThatFits = platformView.SizeThatFits(new CGSize((float)widthConstraint, (float)heightConstraint));
            }

            var size = new Size(
                sizeThatFits.Width == float.PositiveInfinity ? double.PositiveInfinity : sizeThatFits.Width,
                sizeThatFits.Height == float.PositiveInfinity ? double.PositiveInfinity : sizeThatFits.Height);

            if (double.IsInfinity(size.Width) || double.IsInfinity(size.Height))
            {
                platformView.SizeToFit();
                size = new Size(platformView.Frame.Width, platformView.Frame.Height);
            }

            var finalWidth = ResolveConstraints(size.Width, virtualView.Width, virtualView.MinWidth, virtualView.MaxWidth);
            var finalHeight = ResolveConstraints(size.Height, virtualView.Height, virtualView.MinHeight, virtualView.MaxHeight);

            return new Size(finalWidth, finalHeight);
        }

        /// <summary>
        /// https://github.com/dotnet/maui/blob/9.0.70/src/Core/src/Handlers/ViewHandlerExtensions.iOS.cs#L155
        /// </summary>
        /// <param name="measured"></param>
        /// <param name="exact"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        protected static double ResolveConstraints(double measured, double exact, double min, double max)
        {
            var resolved = measured;

            min = Dimension.ResolveMinimum(min);

            if (Dimension.IsExplicitSet(exact))
            {
                // If an exact value has been specified, try to use that
                resolved = exact;
            }

            if (resolved > max)
            {
                // Apply the max value constraint (if any)
                // If the exact value is in conflict with the max value, the max value should win
                resolved = max;
            }

            if (resolved < min)
            {
                // Apply the min value constraint (if any)
                // If the exact or max value is in conflict with the min value, the min value should win
                resolved = min;
            }

            return resolved;
        }

        /// <summary>
        /// https://github.com/dotnet/maui/blob/9.0.70/src/Core/src/Primitives/Dimension.cs
        /// </summary>
        static partial class Dimension
        {
            public const double Minimum = 0;
            public const double Unset = double.NaN;
            public const double Maximum = double.PositiveInfinity;

            public static bool IsExplicitSet(double value)
            {
                return !double.IsNaN(value);
            }

            public static bool IsMaximumSet(double value)
            {
                return !double.IsPositiveInfinity(value);
            }

            public static bool IsMinimumSet(double value)
            {
                return !double.IsNaN(value);
            }

            public static double ResolveMinimum(double value)
            {
                if (IsMinimumSet(value))
                {
                    return value;
                }

                return Minimum;
            }
        }


        public virtual Size GetDesiredSize(WKWebView platformView, WebView2 virtualView, double widthConstraint, double heightConstraint)
        {
            var size = GetDesiredSizeFromHandler(platformView, virtualView, widthConstraint, heightConstraint);

            var set = false;

            var width = widthConstraint;
            var height = heightConstraint;

            if (size.Width == 0)
            {
                if (widthConstraint <= 0 || double.IsInfinity(widthConstraint))
                {
                    width = MinimumSize;
                    set = true;
                }
            }

            if (size.Height == 0)
            {
                if (heightConstraint <= 0 || double.IsInfinity(heightConstraint))
                {
                    height = MinimumSize;
                    set = true;
                }
            }

            if (set)
                size = new Size(width, height);

            return size;
        }
#endif

    }

    public WKWebView? WKWebView => platformHandle?.WebView;

    WKWebViewControlHandle? platformHandle;

    protected virtual void SetValue(WKWebView webView)
    {
        if (_source != null)
        {
            webView?.SetSource(_source);
        }

        // TODO: other properties
    }
}

/// <summary>
/// https://github.com/dotnet/maui/blob/9.0.70/src/Core/src/Platform/iOS/MauiWKWebView.cs
/// </summary>
public class WKWebView2 : WKWebView
{
    [UnconditionalSuppressMessage("Memory", "MEM0002", Justification = "Used to persist cookies across WebView instances. Not a leak.")]
    static WKProcessPool? SharedPool;

    //string? _pendingUrl;
    readonly WeakReference<WebView2.Handler> _handler;

    //public WKWebView2(WebView2.Handler handler)
    //    : this(RectangleF.Empty, handler)
    //{
    //}

    //public WKWebView2(CGRect frame, WebView2.Handler handler)
    //    : this(frame, handler, CreateConfiguration())
    //{
    //}

    public WKWebView2(CGRect frame, WebView2.Handler handler, WKWebViewConfiguration configuration)
            : base(frame, configuration)
    {
        _ = handler ?? throw new ArgumentNullException(nameof(handler));
        _handler = new WeakReference<WebView2.Handler>(handler);

#if !MACOS
        BackgroundColor = UIColor.Clear;
        AutosizesSubviews = true;
#endif
    }

    //public string? CurrentUrl => Url?.AbsoluteUrl?.ToString();

    //public override void MovedToWindow()
    //{
    //    base.MovedToWindow();

    //    if (!string.IsNullOrWhiteSpace(_pendingUrl))
    //    {
    //        var closure = _pendingUrl;
    //        _pendingUrl = null;

    //        // I realize this looks like the worst hack ever but iOS 11 and cookies are super quirky
    //        // and this is the only way I could figure out how to get iOS 11 to inject a cookie 
    //        // the first time a WkWebView is used in your app. This only has to run the first time a WkWebView is used 
    //        // anywhere in the application. All subsequents uses of WkWebView won't hit this hack
    //        // Even if it's a WkWebView on a new page.
    //        // read through this thread https://developer.apple.com/forums/thread/99674
    //        // Or Bing "WkWebView and Cookies" to see the myriad of hacks that exist
    //        // Most of them all came down to different variations of synching the cookies before or after the
    //        // WebView is added to the controller. This is the only one I was able to make work
    //        // I think if we could delay adding the WebView to the Controller until after ViewWillAppear fires that might also work
    //        // But we're not really setup for that
    //        // If you'd like to try your hand at cleaning this up then UI Test Issue12134 and Issue3262 are your final bosses
    //        InvokeOnMainThread(async () =>
    //        {
    //            await Task.Delay(500);
    //            if (_handler.TryGetTarget(out var handler))
    //                await handler.FirstLoadUrlAsync(closure);
    //        });
    //    }

    //    _movedToWindow?.Invoke(this, EventArgs.Empty);
    //}

    //[Obsolete("Use MauiWebViewNavigationDelegate.DidFinishNavigation instead.")]
    //public async void DidFinishNavigation(WKWebView webView, WKNavigation navigation)
    //{
    //    var url = CurrentUrl;

    //    if (url == null || url == $"file://{NSBundle.MainBundle.BundlePath}/")
    //        return;

    //    if (_handler.TryGetTarget(out var handler))
    //        await handler.ProcessNavigatedAsync(url);
    //}

    //[Export("webViewWebContentProcessDidTerminate:")]
    //public void ContentProcessDidTerminate(WKWebView webView)
    //{
    //    if (_handler.TryGetTarget(out var handler))
    //        handler.VirtualView.ProcessTerminated(new WebProcessTerminatedEventArgs(webView));
    //}

    // https://developer.apple.com/forums/thread/99674
    // WKWebView and making sure cookies synchronize is really quirky
    // The main workaround I've found for ensuring that cookies synchronize 
    // is to share the Process Pool between all WkWebView instances.
    // It also has to be shared at the point you call init
    public static WKWebViewConfiguration CreateConfiguration()
    {
        // By default, setting inline media playback to allowed, including autoplay
        // and picture in picture, since these things MUST be set during the webview
        // creation, and have no effect if set afterwards.
        // A custom handler factory delegate could be set to disable these defaults
        // but if we do not set them here, they cannot be changed once the
        // handler's platform view is created, so erring on the side of wanting this
        // capability by default.
        var config = new WKWebViewConfiguration();
#if !MACOS
#if IOS
        if (OperatingSystem.IsIOSVersionAtLeast(10))
#elif MACCATALYST
        if (OperatingSystem.IsMacCatalystVersionAtLeast(10))
#else
        if (OperatingSystem.IsMacCatalystVersionAtLeast(10) || OperatingSystem.IsIOSVersionAtLeast(10))
#endif
#endif
        {
#if !MACOS
            config.AllowsPictureInPictureMediaPlayback = true;
            config.AllowsInlineMediaPlayback = true;
#endif
            config.MediaTypesRequiringUserActionForPlayback = WKAudiovisualMediaTypes.None;
        }
        if (SharedPool == null)
            SharedPool = config.ProcessPool;
        else
            config.ProcessPool = SharedPool;

        return config;
    }

    //[UnconditionalSuppressMessage("Memory", "MEM0002", Justification = IUIViewLifeCycleEvents.UnconditionalSuppressMessage)]
    //EventHandler? _movedToWindow;
    //event EventHandler IUIViewLifeCycleEvents.MovedToWindow
    //{
    //    add => _movedToWindow += value;
    //    remove => _movedToWindow -= value;
    //}
}

sealed class WKWebViewControlHandle : PlatformHandle, INativeControlHostDestroyableControlHandle
{
    bool disposedValue;
    WKWebView? webView;

    internal WKWebViewControlHandle(WKWebView webView) : base(webView.Handle, "WKWebView")
    {
        this.webView = webView;
    }

    public WKWebView? WebView => webView;

    void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // 释放托管状态(托管对象)
                webView?.Dispose();
            }

            // 释放未托管的资源(未托管的对象)并重写终结器
            // 将大型字段设置为 null
            webView = null;
            disposedValue = true;
        }
    }

    /// <inheritdoc/>
    public void Destroy()
    {
        // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        Dispose(disposing: true);
    }
}

static partial class WKWebView2Extensions
{
    public static string GetCurrentUrl(this WKWebView webView)
    {
        return webView?.Url?.AbsoluteUrl?.ToString() ?? string.Empty;
    }

    public static void LoadHtml(this WKWebView webView, string? html, string? baseUrl)
    {
        if (html != null)
            webView.LoadHtmlString(html, baseUrl == null ? new NSUrl(NSBundle.MainBundle.BundlePath, true) : new NSUrl(baseUrl, true));
    }

    static void LoadUrlAsync(this WKWebView webView, string? url)
    {
        try
        {
            var uri = new Uri(url ?? string.Empty);
            var safeHostUri = new Uri($"{uri.Scheme}://{uri.Authority}", UriKind.Absolute);
            var safeRelativeUri = new Uri($"{uri.PathAndQuery}{uri.Fragment}", UriKind.Relative);
            var safeFullUri = new Uri(safeHostUri, safeRelativeUri);
            var request = new NSUrlRequest(new NSUrl(safeFullUri.AbsoluteUri));

            //if (_handler.TryGetTarget(out var handler))
            //{
            //    if (handler.HasCookiesToLoad(safeFullUri.AbsoluteUri) &&
            //        !(OperatingSystem.IsIOSVersionAtLeast(11) || OperatingSystem.IsTvOSVersionAtLeast(11)))
            //    {
            //        return;
            //    }

            //    await handler.SyncPlatformCookiesAsync(safeFullUri.AbsoluteUri);
            //}

            webView.LoadRequest(request);
        }
        catch (UriFormatException formatException)
        {
            // If we got a format exception trying to parse the URI, it might be because
            // someone is passing in a local bundled file page. If we can find a better way
            // to detect that scenario, we should use it; until then, we'll fall back to 
            // local file loading here and see if that works:
            if (!string.IsNullOrEmpty(url))
            {
                if (!webView.LoadFile(url))
                {
                    Logger.Sink?.Log(LogEventLevel.Warning, "WebView2", webView,
                        $"Unable to Load Url {url}: {formatException}");
                }
            }
        }
        catch (Exception exc)
        {
            Logger.Sink?.Log(LogEventLevel.Warning, "WebView2", webView,
                $"Unable to Load Url {url}: {exc}");
        }
    }

    public static bool LoadFile(this WKWebView webView, string url)
    {
        try
        {
            var file = Path.GetFileNameWithoutExtension(url);
            var ext = Path.GetExtension(url);

            var nsUrl = NSBundle.MainBundle.GetUrlForResource(file, ext);

            if (nsUrl == null)
            {
                return false;
            }

            webView.LoadFileUrl(nsUrl, nsUrl);

            return true;
        }
        catch (Exception ex)
        {
            Logger.Sink?.Log(LogEventLevel.Warning, "WebView2", webView,
                $"Could not load {url} as local file: {ex}");
        }

        return false;
    }

    public static void LoadUrl(this WKWebView webView, string? url)
    {
        webView.LoadUrlAsync(url);
    }

    public static void SetSource(this WKWebView webView, Uri? value) => webView.LoadUrl(value?.AbsoluteUri);
}
#endif

// TODO
// https://github.com/dotnet/maui/blob/9.0.70/src/Controls/src/Core/HybridWebView/HybridWebView.cs
// https://github.com/dotnet/maui/blob/9.0.70/src/Controls/src/Core/WebView/WebView.cs
// https://github.com/dotnet/maui/blob/9.0.70/src/Core/src/Handlers/WebView/WebViewHandler.iOS.cs
// https://github.com/dotnet/maui/blob/9.0.70/src/Compatibility/Core/src/MacOS/Renderers/WebViewRenderer.cs
// https://github.com/dotnet/maui/blob/9.0.70/src/Compatibility/Core/src/iOS/Renderers/WkWebViewRenderer.cs
// https://github.com/dotnet/maui/blob/9.0.70/src/Core/src/Handlers/WebView/WebViewHandler.cs
// https://github.com/dotnet/maui/blob/9.0.70/src/Core/src/Platform/iOS/WebViewExtensions.cs
// https://github.com/dotnet/maui/blob/9.0.70/src/Core/src/Handlers/HybridWebView/HybridWebViewHandler.iOS.cs