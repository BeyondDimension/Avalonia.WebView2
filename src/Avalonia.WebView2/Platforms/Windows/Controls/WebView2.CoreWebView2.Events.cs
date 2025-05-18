#if WINDOWS || NETFRAMEWORK
using Microsoft.Web.WebView2.Core;

namespace Avalonia.Controls;

partial class WebView2
{
    /// <summary>
    /// This event is triggered either 1) when the control's <see cref="Avalonia.Controls.WebView2.CoreWebView2" /> has finished being initialized (regardless of how it was triggered or whether it succeeded) but before it is used for anything
    /// OR 2) the initialization failed.
    /// You should handle this event if you need to perform one time setup operations on the CoreWebView2 which you want to affect all of its usages
    /// (e.g. adding event handlers, configuring settings, installing document creation scripts, adding host objects).
    /// </summary>
    /// <remarks>
    /// This sender will be the WebView2 control, whose CoreWebView2 property will now be valid (i.e. non-null) for the first time
    /// if <see cref="Microsoft.Web.WebView2.Core.CoreWebView2InitializationCompletedEventArgs.IsSuccess" /> is true.
    /// Unlikely this event can fire second time (after reporting initialization success first)
    /// if the initialization is followed by navigation which fails.
    /// </remarks>
    public event EventHandler<CoreWebView2InitializationCompletedEventArgs>? CoreWebView2InitializationCompleted;

    /// <summary>
    /// NavigationStarting dispatches before a new navigate starts for the top
    /// level document of the <see cref="Avalonia.Controls.WebView2" />.
    /// This is equivalent to the <see cref="Microsoft.Web.WebView2.Core.CoreWebView2.NavigationStarting" /> event.
    /// </summary>
    /// <seealso cref="Microsoft.Web.WebView2.Core.CoreWebView2.NavigationStarting" />
    public event EventHandler<CoreWebView2NavigationStartingEventArgs>? NavigationStarting;

    /// <summary>
    /// NavigationCompleted dispatches after a navigate of the top level
    /// document completes rendering either successfully or not.
    /// This is equivalent to the <see cref="Microsoft.Web.WebView2.Core.CoreWebView2.NavigationCompleted" /> event.
    /// </summary>
    /// <seealso cref="Microsoft.Web.WebView2.Core.CoreWebView2.NavigationCompleted" />
    public event EventHandler<CoreWebView2NavigationCompletedEventArgs>? NavigationCompleted;

    /// <summary>
    /// WebResourceRequested is raised when the WebView is performing a URL request to a matching URL and resource context filter that was added with <see cref="M:Microsoft.Web.WebView2.Core.CoreWebView2.AddWebResourceRequestedFilter(System.String,Microsoft.Web.WebView2.Core.CoreWebView2WebResourceContext,Microsoft.Web.WebView2.Core.CoreWebView2WebResourceRequestSourceKinds)" />.
    /// </summary><remarks>
    /// At least one filter must be added for the event to be raised.
    /// The web resource requested may be blocked until the event handler returns if a deferral is not taken on the event args. If a deferral is taken, then the web resource requested is blocked until the deferral is completed.
    ///
    /// If this event is subscribed in the <see cref="Microsoft.Web.WebView2.Core.CoreWebView2.NewWindowRequested" /> handler it should be called after the new window is set. For more details see <see cref="Microsoft.Web.WebView2.Core.CoreWebView2NewWindowRequestedEventArgs.NewWindow" />.
    ///
    /// This event is by default raised for file, http, and https URI schemes. This is also raised for registered custom URI schemes. See <see cref="Microsoft.Web.WebView2.Core.CoreWebView2CustomSchemeRegistration" /> for more details.
    /// </remarks><seealso cref="M:Microsoft.Web.WebView2.Core.CoreWebView2.AddWebResourceRequestedFilter(System.String,Microsoft.Web.WebView2.Core.CoreWebView2WebResourceContext,Microsoft.Web.WebView2.Core.CoreWebView2WebResourceRequestSourceKinds)" />
    public event EventHandler<CoreWebView2WebResourceRequestedEventArgs>? WebResourceRequested;

    /// <summary>
    /// WebMessageReceived dispatches after web content sends a message to the
    /// app host via <c>chrome.webview.postMessage</c>.
    /// This is equivalent to the <see cref="Microsoft.Web.WebView2.Core.CoreWebView2.WebMessageReceived" /> event.
    /// </summary>
    /// <seealso cref="Microsoft.Web.WebView2.Core.CoreWebView2.WebMessageReceived" />
    public event EventHandler<CoreWebView2WebMessageReceivedEventArgs>? WebMessageReceived;

    /// <summary>
    /// SourceChanged dispatches after the <see cref="Avalonia.Controls.WebView2.Source" /> property changes. This may happen
    /// during a navigation or if otherwise the script in the page changes the
    /// URI of the document.
    /// This is equivalent to the <see cref="Microsoft.Web.WebView2.Core.CoreWebView2.SourceChanged" /> event.
    /// </summary>
    /// <seealso cref="Microsoft.Web.WebView2.Core.CoreWebView2.SourceChanged" />
    public event EventHandler<CoreWebView2SourceChangedEventArgs>? SourceChanged;

    /// <summary>
    /// ContentLoading dispatches after a navigation begins to a new URI and the
    /// content of that URI begins to render.
    /// This is equivalent to the <see cref="Microsoft.Web.WebView2.Core.CoreWebView2.ContentLoading" /> event.
    /// </summary>
    /// <seealso cref="Microsoft.Web.WebView2.Core.CoreWebView2.ContentLoading" />
    public event EventHandler<CoreWebView2ContentLoadingEventArgs>? ContentLoading;

    /// <summary>
    /// DOMContentLoaded is raised when the initial HTML document has been parsed.
    /// </summary><remarks>
    /// This aligns with the the document's DOMContentLoaded event in HTML.
    /// </remarks>
    public event EventHandler<CoreWebView2DOMContentLoadedEventArgs>? DOMContentLoaded;

    /// <summary>
    /// DocumentTitleChanged is raised when the <see cref="Microsoft.Web.WebView2.Core.CoreWebView2.DocumentTitle" /> property changes and may be raised before or after the <see cref="Microsoft.Web.WebView2.Core.CoreWebView2.NavigationCompleted" /> event.
    /// </summary><seealso cref="Microsoft.Web.WebView2.Core.CoreWebView2.DocumentTitle" />
    public event EventHandler<object>? DocumentTitleChanged;

    /// <summary>
    /// NewWindowRequested is raised when content inside the WebView requests to open a new window, such as through <c>window.open()</c>.
    /// </summary><remarks>
    /// The app can pass a target WebView that is considered the opened window or mark the event as <see cref="Microsoft.Web.WebView2.Core.CoreWebView2NewWindowRequestedEventArgs.Handled" />, in which case WebView2 does not open a window.
    /// If either <c>Handled</c> or <see cref="Microsoft.Web.WebView2.Core.CoreWebView2NewWindowRequestedEventArgs.NewWindow" /> properties are not set, the target content will be opened on a popup window.
    /// If a deferral is not taken on the event args, scripts that resulted in the new window that are requested are blocked until the event handler returns. If a deferral is taken, then scripts are blocked until the deferral is completed.
    ///
    /// On Hololens 2, if the <see cref="Microsoft.Web.WebView2.Core.CoreWebView2NewWindowRequestedEventArgs.NewWindow" /> property is not set and the <see cref="Microsoft.Web.WebView2.Core.CoreWebView2NewWindowRequestedEventArgs.Handled" /> property is not set to <c>true</c>, the WebView2 will navigate to the <see cref="Microsoft.Web.WebView2.Core.CoreWebView2NewWindowRequestedEventArgs.Uri" />.
    /// If either of these properties are set, the WebView2 will not navigate to the <see cref="Microsoft.Web.WebView2.Core.CoreWebView2NewWindowRequestedEventArgs.Uri" /> and the the <see cref="Microsoft.Web.WebView2.Core.CoreWebView2.NewWindowRequested" /> event will continue as normal.
    /// </remarks>
    public event EventHandler<CoreWebView2NewWindowRequestedEventArgs>? NewWindowRequested;

    /// <summary>
    /// ZoomFactorChanged dispatches when the <see cref="Avalonia.Controls.WebView2.ZoomFactor" /> property changes.
    /// This is equivalent to the <see cref="Microsoft.Web.WebView2.Core.CoreWebView2Controller.ZoomFactorChanged" /> event.
    /// </summary>
    /// <seealso cref="Microsoft.Web.WebView2.Core.CoreWebView2Controller.ZoomFactorChanged" />
    public event EventHandler<EventArgs>? ZoomFactorChanged;
}
#endif