namespace Avalonia.Controls;

#if !DISABLE_WEBVIEW2_CORE
partial class WebView2
{
    #region Events

    /// <summary>
    /// This event is triggered either 1) when the control's <see cref="P:Avalonia.Controls.WebView2.CoreWebView2" /> has finished being initialized (regardless of how it was triggered or whether it succeeded) but before it is used for anything
    /// OR 2) the initialization failed.
    /// You should handle this event if you need to perform one time setup operations on the CoreWebView2 which you want to affect all of its usages
    /// (e.g. adding event handlers, configuring settings, installing document creation scripts, adding host objects).
    /// </summary>
    /// <remarks>
    /// This sender will be the WebView2 control, whose CoreWebView2 property will now be valid (i.e. non-null) for the first time
    /// if <see cref="P:Microsoft.Web.WebView2.Core.CoreWebView2InitializationCompletedEventArgs.IsSuccess" /> is true.
    /// Unlikely this event can fire second time (after reporting initialization success first)
    /// if the initialization is followed by navigation which fails.
    /// </remarks>
    public event EventHandler<CoreWebView2InitializationCompletedEventArgs>? CoreWebView2InitializationCompleted;

    /// <summary>
    /// NavigationStarting dispatches before a new navigate starts for the top
    /// level document of the <see cref="T:Avalonia.Controls.WebView2" />.
    /// This is equivalent to the <see cref="E:Microsoft.Web.WebView2.Core.CoreWebView2.NavigationStarting" /> event.
    /// </summary>
    /// <seealso cref="E:Microsoft.Web.WebView2.Core.CoreWebView2.NavigationStarting" />
    public event EventHandler<CoreWebView2NavigationStartingEventArgs>? NavigationStarting;

    /// <summary>
    /// NavigationCompleted dispatches after a navigate of the top level
    /// document completes rendering either successfully or not.
    /// This is equivalent to the <see cref="E:Microsoft.Web.WebView2.Core.CoreWebView2.NavigationCompleted" /> event.
    /// </summary>
    /// <seealso cref="E:Microsoft.Web.WebView2.Core.CoreWebView2.NavigationCompleted" />
    public event EventHandler<CoreWebView2NavigationCompletedEventArgs>? NavigationCompleted;

    /// <summary>
    /// WebResourceRequested is raised when the WebView is performing a URL request to a matching URL and resource context filter that was added with <see cref="M:Microsoft.Web.WebView2.Core.CoreWebView2.AddWebResourceRequestedFilter(System.String,Microsoft.Web.WebView2.Core.CoreWebView2WebResourceContext,Microsoft.Web.WebView2.Core.CoreWebView2WebResourceRequestSourceKinds)" />.
    /// </summary><remarks>
    /// At least one filter must be added for the event to be raised.
    /// The web resource requested may be blocked until the event handler returns if a deferral is not taken on the event args. If a deferral is taken, then the web resource requested is blocked until the deferral is completed.
    ///
    /// If this event is subscribed in the <see cref="E:Microsoft.Web.WebView2.Core.CoreWebView2.NewWindowRequested" /> handler it should be called after the new window is set. For more details see <see cref="P:Microsoft.Web.WebView2.Core.CoreWebView2NewWindowRequestedEventArgs.NewWindow" />.
    ///
    /// This event is by default raised for file, http, and https URI schemes. This is also raised for registered custom URI schemes. See <see cref="T:Microsoft.Web.WebView2.Core.CoreWebView2CustomSchemeRegistration" /> for more details.
    /// </remarks><seealso cref="M:Microsoft.Web.WebView2.Core.CoreWebView2.AddWebResourceRequestedFilter(System.String,Microsoft.Web.WebView2.Core.CoreWebView2WebResourceContext,Microsoft.Web.WebView2.Core.CoreWebView2WebResourceRequestSourceKinds)" />
    public event EventHandler<CoreWebView2WebResourceRequestedEventArgs>? WebResourceRequested;

    /// <summary>
    /// WebMessageReceived dispatches after web content sends a message to the
    /// app host via <c>chrome.webview.postMessage</c>.
    /// This is equivalent to the <see cref="E:Microsoft.Web.WebView2.Core.CoreWebView2.WebMessageReceived" /> event.
    /// </summary>
    /// <seealso cref="E:Microsoft.Web.WebView2.Core.CoreWebView2.WebMessageReceived" />
    public event EventHandler<CoreWebView2WebMessageReceivedEventArgs>? WebMessageReceived;

    /// <summary>
    /// SourceChanged dispatches after the <see cref="P:Avalonia.Controls.WebView2.Source" /> property changes. This may happen
    /// during a navigation or if otherwise the script in the page changes the
    /// URI of the document.
    /// This is equivalent to the <see cref="E:Microsoft.Web.WebView2.Core.CoreWebView2.SourceChanged" /> event.
    /// </summary>
    /// <seealso cref="E:Microsoft.Web.WebView2.Core.CoreWebView2.SourceChanged" />
    public event EventHandler<CoreWebView2SourceChangedEventArgs>? SourceChanged;

    /// <summary>
    /// ContentLoading dispatches after a navigation begins to a new URI and the
    /// content of that URI begins to render.
    /// This is equivalent to the <see cref="E:Microsoft.Web.WebView2.Core.CoreWebView2.ContentLoading" /> event.
    /// </summary>
    /// <seealso cref="E:Microsoft.Web.WebView2.Core.CoreWebView2.ContentLoading" />
    public event EventHandler<CoreWebView2ContentLoadingEventArgs>? ContentLoading;

    /// <summary>
    /// DOMContentLoaded is raised when the initial HTML document has been parsed.
    /// </summary><remarks>
    /// This aligns with the the document's DOMContentLoaded event in HTML.
    /// </remarks>
    public event EventHandler<CoreWebView2DOMContentLoadedEventArgs>? DOMContentLoaded;

    /// <summary>
    /// DocumentTitleChanged is raised when the <see cref="P:Microsoft.Web.WebView2.Core.CoreWebView2.DocumentTitle" /> property changes and may be raised before or after the <see cref="E:Microsoft.Web.WebView2.Core.CoreWebView2.NavigationCompleted" /> event.
    /// </summary><seealso cref="P:Microsoft.Web.WebView2.Core.CoreWebView2.DocumentTitle" />
    public event EventHandler<object>? DocumentTitleChanged;

    /// <summary>
    /// NewWindowRequested is raised when content inside the WebView requests to open a new window, such as through <c>window.open()</c>.
    /// </summary><remarks>
    /// The app can pass a target WebView that is considered the opened window or mark the event as <see cref="P:Microsoft.Web.WebView2.Core.CoreWebView2NewWindowRequestedEventArgs.Handled" />, in which case WebView2 does not open a window.
    /// If either <c>Handled</c> or <see cref="P:Microsoft.Web.WebView2.Core.CoreWebView2NewWindowRequestedEventArgs.NewWindow" /> properties are not set, the target content will be opened on a popup window.
    /// If a deferral is not taken on the event args, scripts that resulted in the new window that are requested are blocked until the event handler returns. If a deferral is taken, then scripts are blocked until the deferral is completed.
    ///
    /// On Hololens 2, if the <see cref="P:Microsoft.Web.WebView2.Core.CoreWebView2NewWindowRequestedEventArgs.NewWindow" /> property is not set and the <see cref="P:Microsoft.Web.WebView2.Core.CoreWebView2NewWindowRequestedEventArgs.Handled" /> property is not set to <c>true</c>, the WebView2 will navigate to the <see cref="P:Microsoft.Web.WebView2.Core.CoreWebView2NewWindowRequestedEventArgs.Uri" />.
    /// If either of these properties are set, the WebView2 will not navigate to the <see cref="P:Microsoft.Web.WebView2.Core.CoreWebView2NewWindowRequestedEventArgs.Uri" /> and the the <see cref="E:Microsoft.Web.WebView2.Core.CoreWebView2.NewWindowRequested" /> event will continue as normal.
    /// </remarks>
    public event EventHandler<CoreWebView2NewWindowRequestedEventArgs>? NewWindowRequested;

    /// <summary>
    /// ZoomFactorChanged dispatches when the <see cref="P:Avalonia.Controls.WebView2.ZoomFactor" /> property changes.
    /// This is equivalent to the <see cref="E:Microsoft.Web.WebView2.Core.CoreWebView2Controller.ZoomFactorChanged" /> event.
    /// </summary>
    /// <seealso cref="E:Microsoft.Web.WebView2.Core.CoreWebView2Controller.ZoomFactorChanged" />
    public event EventHandler<EventArgs>? ZoomFactorChanged;
    #endregion

    #region EventHandlers

    void CoreWebView2_NavigationStarting(object? sender, CoreWebView2NavigationStartingEventArgs e)
    {
        var navigationStarting = NavigationStarting;
        if (navigationStarting == null)
            return;
        navigationStarting(this, e);
    }

    void CoreWebView2_NavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
    {
        var navigationCompleted = NavigationCompleted;
        if (navigationCompleted == null)
            return;
        navigationCompleted(this, e);
    }

    void CoreWebView2_WebResourceRequested(object? sender, CoreWebView2WebResourceRequestedEventArgs e)
    {
        var webResourceRequested = WebResourceRequested;
        if (webResourceRequested == null)
            return;
        webResourceRequested(this, e);
    }

    void CoreWebView2_WebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
    {
        var webMessageReceived = WebMessageReceived;
        if (webMessageReceived == null)
            return;
        webMessageReceived(this, e);
    }

    void CoreWebView2_SourceChanged(object? sender, CoreWebView2SourceChangedEventArgs e)
    {
        var source = CoreWebView2?.Source;
        if (_source == null || _source.AbsoluteUri != source)
            _source = new Uri(source);
        var sourceChanged = SourceChanged;
        if (sourceChanged == null)
            return;
        sourceChanged(this, e);
    }

    void CoreWebView2_ContentLoading(object? sender, CoreWebView2ContentLoadingEventArgs e)
    {
        var contentLoading = ContentLoading;
        if (contentLoading == null)
            return;
        contentLoading(this, e);
    }

    void CoreWebView2_DOMContentLoaded(object? sender, CoreWebView2DOMContentLoadedEventArgs e)
    {
        var contentLoading = DOMContentLoaded;
        if (contentLoading == null)
            return;
        contentLoading(this, e);
    }

    void CoreWebView2_DocumentTitleChanged(object? sender, object e)
    {
        var cocumentTitleChanged = DocumentTitleChanged;
        if (cocumentTitleChanged == null)
            return;
        cocumentTitleChanged(this, e);
    }

    void CoreWebView2_NewWindowRequested(object? sender, CoreWebView2NewWindowRequestedEventArgs args)
    {
        var newWindowRequested = NewWindowRequested;
        if (newWindowRequested == null)
        {
            if (sender is CoreWebView2 parent)
            {
                args.Handled = true;
                WebView2 coreWebView2;
                var window = new Window()
                {
                    Width = Bounds.Width,
                    Height = Bounds.Height,
                    Content = coreWebView2 = new WebView2()
                    {
                        Source = new Uri(args.Uri),
                        EnabledDevTools = parent.Settings.AreDevToolsEnabled,
                        Fill = Fill,
                        UserDataFolder = parent.Environment.UserDataFolder,
                    },
                };
                coreWebView2.DOMContentLoaded += (s, e) =>
                {
                    if (s is CoreWebView2 sender)
                    {
                        args.NewWindow = sender;
                    }
                };
                coreWebView2.DocumentTitleChanged += async (s, e) =>
                {
                    window.Title = coreWebView2.CoreWebView2!.DocumentTitle;
                    window.Icon = new WindowIcon(await coreWebView2.CoreWebView2!.GetFaviconAsync(CoreWebView2FaviconImageFormat.Png));
                };

                window.Show();
            }

            return;
        }
        newWindowRequested(this, args);
    }

    void CoreWebView2_ProcessFailed(object? sender, CoreWebView2ProcessFailedEventArgs e)
    {
        if (e.ProcessFailedKind != CoreWebView2ProcessFailedKind.BrowserProcessExited)
            return;
        UnsubscribeHandlersAndCloseController(true);
    }

    void CoreWebView2Controller_ZoomFactorChanged(object? sender, object e)
    {
        if (_coreWebView2Controller != null)
            _zoomFactor = _coreWebView2Controller.ZoomFactor;
        var zoomFactorChanged = ZoomFactorChanged;
        if (zoomFactorChanged == null)
            return;
        zoomFactorChanged(this, EventArgs.Empty);
    }
    #endregion

    void SubscribeHandlers(WebView2 sender)
    {
        if (sender._coreWebView2Controller != null)
        {
            //sender._coreWebView2Controller.MoveFocusRequested += new EventHandler<CoreWebView2MoveFocusRequestedEventArgs>(sender.CoreWebView2Controller_MoveFocusRequested);
            sender._coreWebView2Controller.AcceleratorKeyPressed += new EventHandler<CoreWebView2AcceleratorKeyPressedEventArgs>(sender.CoreWebView2Controller_AcceleratorKeyPressed);
            sender._coreWebView2Controller.ZoomFactorChanged += new EventHandler<object>(sender.CoreWebView2Controller_ZoomFactorChanged);
            sender._coreWebView2Controller.CoreWebView2.NavigationCompleted += new EventHandler<CoreWebView2NavigationCompletedEventArgs>(sender.CoreWebView2_NavigationCompleted);
            sender._coreWebView2Controller.CoreWebView2.NavigationStarting += new EventHandler<CoreWebView2NavigationStartingEventArgs>(sender.CoreWebView2_NavigationStarting);
            sender._coreWebView2Controller.CoreWebView2.SourceChanged += new EventHandler<CoreWebView2SourceChangedEventArgs>(sender.CoreWebView2_SourceChanged);
            sender._coreWebView2Controller.CoreWebView2.WebMessageReceived += new EventHandler<CoreWebView2WebMessageReceivedEventArgs>(sender.CoreWebView2_WebMessageReceived);
            sender._coreWebView2Controller.CoreWebView2.ContentLoading += new EventHandler<CoreWebView2ContentLoadingEventArgs>(sender.CoreWebView2_ContentLoading);
            sender._coreWebView2Controller.CoreWebView2.DOMContentLoaded += new EventHandler<CoreWebView2DOMContentLoadedEventArgs>(sender.CoreWebView2_DOMContentLoaded);
            sender._coreWebView2Controller.CoreWebView2.ProcessFailed += new EventHandler<CoreWebView2ProcessFailedEventArgs>(sender.CoreWebView2_ProcessFailed);
            sender._coreWebView2Controller.CoreWebView2.WebResourceRequested += new EventHandler<CoreWebView2WebResourceRequestedEventArgs>(sender.CoreWebView2_WebResourceRequested);
            sender._coreWebView2Controller.CoreWebView2.DocumentTitleChanged += new EventHandler<object>(sender.CoreWebView2_DocumentTitleChanged);
            sender._coreWebView2Controller.CoreWebView2.NewWindowRequested += new EventHandler<CoreWebView2NewWindowRequestedEventArgs>(sender.CoreWebView2_NewWindowRequested);
        }
    }

    void UnsubscribeHandlers()
    {
        if (CoreWebView2 != null)
        {
            CoreWebView2.NavigationCompleted -= new EventHandler<CoreWebView2NavigationCompletedEventArgs>(CoreWebView2_NavigationCompleted);
            CoreWebView2.NavigationStarting -= new EventHandler<CoreWebView2NavigationStartingEventArgs>(CoreWebView2_NavigationStarting);
            CoreWebView2.SourceChanged -= new EventHandler<CoreWebView2SourceChangedEventArgs>(CoreWebView2_SourceChanged);
            CoreWebView2.WebMessageReceived -= new EventHandler<CoreWebView2WebMessageReceivedEventArgs>(CoreWebView2_WebMessageReceived);
            CoreWebView2.ContentLoading -= new EventHandler<CoreWebView2ContentLoadingEventArgs>(CoreWebView2_ContentLoading);
            CoreWebView2.DOMContentLoaded -= new EventHandler<CoreWebView2DOMContentLoadedEventArgs>(CoreWebView2_DOMContentLoaded);
            CoreWebView2.ProcessFailed -= new EventHandler<CoreWebView2ProcessFailedEventArgs>(CoreWebView2_ProcessFailed);
            CoreWebView2.WebResourceRequested -= new EventHandler<CoreWebView2WebResourceRequestedEventArgs>(CoreWebView2_WebResourceRequested);
            CoreWebView2.DocumentTitleChanged -= new EventHandler<object>(CoreWebView2_DocumentTitleChanged);
            CoreWebView2.NewWindowRequested -= new EventHandler<CoreWebView2NewWindowRequestedEventArgs>(CoreWebView2_NewWindowRequested);
        }
        if (_coreWebView2Controller != null)
        {
            _coreWebView2Controller.ZoomFactorChanged -= new EventHandler<object>(CoreWebView2Controller_ZoomFactorChanged);
            //_coreWebView2Controller.MoveFocusRequested -= new EventHandler<CoreWebView2MoveFocusRequestedEventArgs>(CoreWebView2Controller_MoveFocusRequested);
            _coreWebView2Controller.AcceleratorKeyPressed -= new EventHandler<CoreWebView2AcceleratorKeyPressedEventArgs>(CoreWebView2Controller_AcceleratorKeyPressed);
        }
    }
}
#endif
