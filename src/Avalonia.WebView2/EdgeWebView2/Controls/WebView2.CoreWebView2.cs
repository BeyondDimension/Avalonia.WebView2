#if !DISABLE_WEBVIEW2_CORE && (WINDOWS || NETFRAMEWORK)
using Microsoft.Web.WebView2.Core;

namespace Avalonia.Controls;

partial class WebView2
{
    internal Task? _initTask;

    bool _isExplicitEnvironment;
    bool _isExplicitControllerOptions;
    CoreWebView2Environment? env;
    CoreWebView2ControllerOptions? opt;

    /// <summary>
    /// The underlying CoreWebView2. Use this property to perform more operations on the WebView2 content than is exposed
    /// on the WebView2. This value is null until it is initialized and the object itself has undefined behaviour once the control is disposed.
    /// You can force the underlying CoreWebView2 to
    /// initialize via the <see cref="EnsureCoreWebView2Async" /> method.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the calling thread isn't the thread which created this object (usually the UI thread). See InvokeRequired for more info.</exception>
    public CoreWebView2? CoreWebView2
    {
        get
        {
            try
            {
                return _coreWebView2Controller?.CoreWebView2;
            }
            catch (Exception ex)
            {
                if (!CheckAccess())
                {
                    throw new InvalidOperationException(
                        "CoreWebView2 can only be accessed from the UI thread.", ex);
                }
                throw;
            }
        }
    }

    /// <summary>
    /// Explicitly trigger initialization of the control's <see cref="Avalonia.Controls.WebView2.CoreWebView2" />.
    /// </summary>
    /// <param name="environment">
    /// A pre-created <see cref="Microsoft.Web.WebView2.Core.CoreWebView2Environment" /> that should be used to create the <see cref="Avalonia.Controls.WebView2.CoreWebView2" />.
    /// Creating your own environment gives you control over several options that affect how the <see cref="Avalonia.Controls.WebView2.CoreWebView2" /> is initialized.
    /// If you pass <c>null</c> (the default value) then a default environment will be created and used automatically.
    /// </param>
    /// <param name="controllerOptions">
    /// A pre-created <see cref="Microsoft.Web.WebView2.Core.CoreWebView2ControllerOptions" /> that should be used to create the <see cref="Microsoft.Web.WebView2.Core.CoreWebView2" />.
    /// Creating your own controller options gives you control over several options that affect how the <see cref="Microsoft.Web.WebView2.Core.CoreWebView2" /> is initialized.
    /// If you pass a controllerOptions to this method then it will override any settings specified on the <see cref="Avalonia.Controls.WebView2.CreationProperties" /> property.
    /// If you pass <c>null</c> (the default value) and no value has been set to <see cref="Avalonia.Controls.WebView2.CreationProperties" /> then a default controllerOptions will be created and used automatically.
    /// </param>
    /// <returns>
    /// A Task that represents the background initialization process.
    /// When the task completes then the <see cref="Avalonia.Controls.WebView2.CoreWebView2" /> property will be available for use (i.e. non-null).
    /// Note that the control's <see cref="Avalonia.Controls.WebView2.CoreWebView2InitializationCompleted" /> event will be invoked before the task completes
    /// or on exceptions.
    /// </returns>
    /// <remarks>
    /// Unless previous initialization has already failed, calling this method additional times with the same parameter will have no effect (any specified environment is ignored) and return the same Task as the first call.
    /// Unless previous initialization has already failed, calling this method after initialization has been implicitly triggered by setting the <see cref="Avalonia.Controls.WebView2.Source" /> property will have no effect if no environment is given
    /// and simply return a Task representing that initialization already in progress.
    /// Unless previous initialization has already failed, calling this method with a different environment after initialization has begun will result in an <see cref="System.ArgumentException" />. For example, this can happen if you begin initialization
    /// by setting the <see cref="Avalonia.Controls.WebView2.Source" /> property and then call this method with a new environment, if you begin initialization with <see cref="Avalonia.Controls.WebView2.CreationProperties" /> and then call this method with a new
    /// environment, or if you begin initialization with one environment and then call this method with no environment specified.
    /// When this method is called after previous initialization has failed, it will trigger initialization of the control's <see cref="Avalonia.Controls.WebView2.CoreWebView2" /> again.
    /// Note that even though this method is asynchronous and returns a Task, it still must be called on the UI thread like most public functionality of most UI controls.
    /// </remarks>
    /// <exception cref="System.ArgumentException">
    /// Thrown if this method is called with a different environment than when it was initialized. See Remarks for more info.
    /// </exception>
    /// <exception cref="System.InvalidOperationException">
    /// Thrown if this instance of <see cref="Avalonia.Controls.WebView2.CoreWebView2" /> is already disposed, or if the calling thread isn't the thread which created this object (usually the UI thread). See InvokeRequired for more info.
    /// May also be thrown if the browser process has crashed unexpectedly and left the control in an invalid state. We are considering throwing a different type of exception for this case in the future.
    /// </exception>
    public Task EnsureCoreWebView2Async(CoreWebView2Environment? environment = null, CoreWebView2ControllerOptions? controllerOptions = null)
    {
        if (IsInDesignMode || !IsSupported)
        {
            // 在设计器中或者没有安装 WebView2 运行时不工作
            return Task.CompletedTask;
        }

        VerifyNotClosedGuard();
        VerifyBrowserNotCrashedGuard();
        if (!CheckAccess())
        {
            // EnsureCoreWebView2Async 方法只能在用户界面线程中调用。
            throw new InvalidOperationException(
                "The method EnsureCoreWebView2Async can be invoked only from the UI thread.");
        }
        if (_initTask == null || _initTask.IsFaulted)
        {
            _initTask = InitCoreWebView2Async(environment, controllerOptions);
        }
        else
        {
            if ((!_isExplicitEnvironment && environment != null) || (_isExplicitEnvironment && environment != null && this.env != environment))
            {
                // WebView2 已使用不同的 CoreWebView2Environment 初始化。检查是否已经设置了源属性，或者之前调用 EnsureCoreWebView2Async 时使用了不同的值。
                throw new ArgumentException(
                    "WebView2 was already initialized with a different CoreWebView2Environment. Check to see if the Source property was already set or EnsureCoreWebView2Async was previously called with different values.");
            }
            if ((!_isExplicitControllerOptions && controllerOptions != null) || (_isExplicitControllerOptions && controllerOptions != null && opt != controllerOptions))
            {
                // WebView2 已用不同的 CoreWebView2ControllerOptions 初始化。检查是否已经设置了源属性，或者之前调用 EnsureCoreWebView2Async 时使用了不同的值。
                throw new ArgumentException(
                    "WebView2 was already initialized with a different CoreWebView2ControllerOptions. Check to see if the Source property was already set or EnsureCoreWebView2Async was previously called with different values.");
            }
        }
        return _initTask;
    }

    /// <summary>
    /// This is the function which implements the actual background initialization task.
    /// Cannot be called if the control is already initialized or has been disposed.
    /// </summary>
    /// <param name="environment">
    /// The environment to use to create the <see cref="Microsoft.Web.WebView2.Core.CoreWebView2Controller" />.
    /// If that is null then a default environment is created with <see cref="Microsoft.Web.WebView2.Core.CoreWebView2Environment.CreateAsync(System.String,System.String,Microsoft.Web.WebView2.Core.CoreWebView2EnvironmentOptions)" /> and its default parameters.
    /// </param>
    /// <param name="controllerOptions">
    /// The controllerOptions to use to create the <see cref="Microsoft.Web.WebView2.Core.CoreWebView2Controller" />.
    /// If that is null then a default controllerOptions is created with its default parameters.
    /// </param>
    /// <returns>A task representing the background initialization process.</returns>
    /// <remarks>All the event handlers added here need to be removed in <see cref="Avalonia.Controls.WebView2.Dispose(System.Boolean)" />.</remarks>
    async Task InitCoreWebView2Async(CoreWebView2Environment? environment = null, CoreWebView2ControllerOptions? controllerOptions = null)
    {
        WebView2 sender = this;
        try
        {
            if (environment != null)
            {
                sender.env = environment;
                sender._isExplicitEnvironment = true;
            }
            else if (sender.CreationProperties != null)
            {
                var environmentAsync = await sender.CreationProperties.CreateEnvironmentAsync();
                sender.env = environmentAsync;
            }
            if (sender.env == null)
            {
                var async = await CoreWebView2Environment.CreateAsync(null, null, null);
                sender.env = async;
            }
            if (controllerOptions != null)
            {
                sender.opt = controllerOptions;
                sender._isExplicitControllerOptions = true;
            }
            else if (sender.CreationProperties != null)
            {
                sender.opt = sender.CreationProperties.CreateCoreWebView2ControllerOptions(sender.env);
            }
            if (sender._defaultBackgroundColor != _defaultBackgroundColorDefaultValue)
            {
                global::System.Environment.SetEnvironmentVariable("WEBVIEW2_DEFAULT_BACKGROUND_COLOR", global::System.Drawing.Color.FromArgb(sender.DefaultBackgroundColor.ToArgb()).Name);
            }

            var hwnd = await _hwndTaskSource.Task;
            if (hwnd == default)
            {
                // 窗口句柄无效，tcs 应在控件附加到窗口时设置完成
                throw new InvalidOperationException("The HWND is not valid.");
            }
            if (sender.opt != null)
            {
                CoreWebView2Controller view2ControllerAsync = await sender.env.CreateCoreWebView2ControllerAsync(hwnd, sender.opt);
                sender._coreWebView2Controller = view2ControllerAsync;
            }
            else
            {
                CoreWebView2Controller view2ControllerAsync = await sender.env.CreateCoreWebView2ControllerAsync(hwnd);
                sender._coreWebView2Controller = view2ControllerAsync;
            }
            sender._coreWebView2Controller.ZoomFactor = sender._zoomFactor;
            sender._coreWebView2Controller.DefaultBackgroundColor = sender._defaultBackgroundColor;
            OnBoundsChanged(EventArgs.Empty);
            sender._coreWebView2Controller.IsVisible = IsVisible;
            try
            {
                sender._coreWebView2Controller.AllowExternalDrop = sender._allowExternalDrop;
            }
            catch (NotImplementedException)
            {
            }

            sender.SubscribeHandlers();
            sender._coreWebView2Controller.CoreWebView2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.Document);
            if (sender.Focusable)
                sender._coreWebView2Controller.MoveFocus(CoreWebView2MoveFocusReason.Programmatic);

            sender._coreWebView2Controller.CoreWebView2.Settings.AreDevToolsEnabled = sender.CreationProperties?.EnabledDevTools ?? false;
            sender.CoreWebView2InitializationCompleted?.Invoke(sender, new CoreWebView2InitializationCompletedEventArgs());

            //await InitJavaScriptOnDocumentCreatedAsync();

            if (sender._source != null)
            {
                sender._coreWebView2Controller.CoreWebView2.Navigate(sender._source.AbsoluteUri);
            }
            else if (sender._htmlSource != null)
            {
                sender._coreWebView2Controller.CoreWebView2.NavigateToString(sender._htmlSource);
            }
        }
        catch (Exception ex)
        {
            sender.CoreWebView2InitializationCompleted?.Invoke(sender, new CoreWebView2InitializationCompletedEventArgs(ex));
            throw;
        }
    }
}
#endif