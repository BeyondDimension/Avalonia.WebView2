using WebView2BaseType = Avalonia.Controls.Shapes.Rectangle;
using Avalonia.Threading;

#if WINDOWS
using Windows.Win32;
using Windows.Win32.Foundation;
#endif

namespace Avalonia.Controls;

/// <summary>
/// The Microsoft Edge WebView2 control allows you to embed web technologies (HTML, CSS, and JavaScript) in your native apps. The WebView2 control uses Microsoft Edge as the rendering engine to display the web content in native apps.
/// With WebView2, you can embed web code in different parts of your native app, or build all of the native app within a single WebView2 instance.
/// </summary>
public partial class WebView2 : WebView2BaseType, ISupportInitialize, IDisposable
#if WINDOWS
    , IHwndHost
#endif
{
    public static bool IsSupported { get; private set; }

    public static string? VersionString { get; private set; }

    public static void RefreshIsSupported()
    {
#if !DISABLE_WEBVIEW2_CORE
#if !WINDOWS
        if (OperatingSystem.IsWindows())
#endif
        {
            try
            {
                VersionString = CoreWebView2Environment.GetAvailableBrowserVersionString();
                if (!string.IsNullOrEmpty(VersionString)) IsSupported = true;
            }
            catch (WebView2RuntimeNotFoundException)
            {
                // Exception Info: Microsoft.Web.WebView2.Core.WebView2RuntimeNotFoundException: Couldn't find a compatible Webview2 Runtime installation to host WebViews.
                // ---> System.IO.FileNotFoundException: 系统找不到指定的文件。 (0x80070002)
                // --- End of inner exception stack trace ---
                // at Microsoft.Web.WebView2.Core.CoreWebView2Environment.CreateAsync(String browserExecutableFolder, String userDataFolder, CoreWebView2EnvironmentOptions options)
            }
        }
#endif
    }

    static WebView2()
    {
        RefreshIsSupported();
    }

    /// <inheritdoc />
    public WebView2() : base()
    {
        if (IsInDesignMode)
        {
            return;
        }

        _disposables.Add(this.GetPropertyChangedObservable(IsVisibleProperty).AddClassHandler<WebView2>((t, args) => { IsVisibleChanged(args); }));

        DefaultBackgroundColor = _defaultBackgroundColorDefaultValue;

        _creationProperties = new()
        {
            Language = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName,
        };
    }

    /// <inheritdoc/>
    protected override void OnSizeChanged(SizeChangedEventArgs e)
    {
        base.OnSizeChanged(e);

        Dispatcher.UIThread.Post(async () =>
        {
            OnBoundsChanged(EventArgs.Empty);
        }, DispatcherPriority.Render);
    }

    protected Screen? Screen
    {
        get
        {
            var window = Window;
            if (window != null)
            {
                var screen = window.Screens.ScreenFromWindow(window);
                return screen;
            }
            return null;
        }
    }

    protected int ToSize(double d, Screen? screen)
    {
        if (screen != null)
        {
            d *= screen.Scaling;
        }
        if (double.IsNaN(d) || d <= 0D) return 0;
        return Convert.ToInt32(Math.Ceiling(d));
    }

    protected Rectangle GetBounds()
    {
        var bounds = base.Bounds;
        var screen = Screen;
        int x = ToSize(bounds.X, screen);
        int y = ToSize(bounds.Y, screen);
        int w = ToSize(bounds.Width, screen);
        int h = ToSize(bounds.Height, screen);
        return new(x, y, w, h);
    }

    public new Rectangle Bounds
    {
        get
        {
            if (Window != null)
            {
                var point = this.TranslatePoint(new(0, 0), Window);
                if (point.HasValue)
                {
                    var screen = Screen;
                    var pointValue = point.Value;
                    int x = ToSize(pointValue.X, screen);
                    int y = ToSize(pointValue.Y, screen);
                    var bounds = base.Bounds;
                    int w = ToSize(bounds.Width, screen);
                    int h = ToSize(bounds.Height, screen);
                    return new(x, y, w, h);
                }
            }
            return GetBounds();
        }
    }

    protected virtual void OnBoundsChanged(EventArgs e)
    {
#if !DISABLE_WEBVIEW2_CORE
        if (_coreWebView2Controller != null)
        {
            var bounds = Bounds;
            OnWindowPositionChanged(bounds);
        }
#endif
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();

#if WINDOWS
        GlobalHooks.Initialize(this);
#endif
    }

    /// <summary>
    /// True when we're in design mode and shouldn't create an underlying CoreWebView2.
    /// </summary>
    protected static bool IsInDesignMode => Design.IsDesignMode;

    //public static CoreWebView2CreationProperties? DefaultCreationProperties { get; set; }

    bool disposedValue;
    CoreWebView2CreationProperties? _creationProperties;
    internal Task? _initTask;
#if !DISABLE_WEBVIEW2_CORE
    bool _isExplicitEnvironment;
    bool _isExplicitControllerOptions;
    CoreWebView2MoveFocusReason _lastMoveFocusReason;
    CoreWebView2Controller? _coreWebView2Controller;
#endif
    bool _allowExternalDrop = true;
    double _zoomFactor = 1.0;
    static readonly Color _defaultBackgroundColorDefaultValue = Color.White;
    Color _defaultBackgroundColor;
    Uri? _source;
    string? _htmlSource;
    bool _browserCrashed;
    string _userDataFolder;
    bool _enabledDevTools;
    readonly ImplicitInitGate _implicitInitGate = new();
    List<IDisposable> _disposables = new();

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                UnsubscribeHandlersAndCloseController();
                _disposables.ForEach(d => d.Dispose());
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    void UnsubscribeHandlersAndCloseController(bool browserCrashed = false)
    {
        _browserCrashed = browserCrashed;
        if (!_browserCrashed)
        {
#if !DISABLE_WEBVIEW2_CORE
            UnsubscribeHandlers();
            if (_coreWebView2Controller != null)
            {
                _coreWebView2Controller.Close();
            }
#endif
        }
#if !DISABLE_WEBVIEW2_CORE
        _coreWebView2Controller = null;
#endif
    }

#if WINDOWS
    /// <summary>
    /// This is overridden from <see cref="NativeControlHost" /> and is called to provide us with Win32 messages that are sent to our hwnd.
    /// </summary>
    /// <param name="hwnd">Window receiving the message (should always match our <see cref="IPlatformHandle.Handle" />).</param>
    /// <param name="msg">Indicates the message being received.  See Win32 documentation for WM_* constant values.</param>
    /// <param name="wParam">The "wParam" data being provided with the message.  Meaning varies by message.</param>
    /// <param name="lParam">The "lParam" data being provided with the message.  Meaning varies by message.</param>
    /// <param name="handled">If true then the message will not be forwarded to any (more) <see cref="GlobalHooks" /> handlers.</param>
    /// <returns>Return value varies by message.</returns>
    LRESULT WndProc(HWND hwnd, uint msg, WPARAM wParam, LPARAM lParam, ref bool handled)
    {
        if (!IsInDesignMode)
        {
            switch ((NativeMethods.WM)msg)
            {
                //                case NativeMethods.WM.SETFOCUS:
                //#if !DISABLE_WEBVIEW2_CORE
                //                    if (_coreWebView2Controller != null)
                //                    {
                //                        _coreWebView2Controller.MoveFocus(CoreWebView2MoveFocusReason.Programmatic);
                //                    }
                //#endif
                //                    break;
                case NativeMethods.WM.PAINT:
                    PInvoke.BeginPaint(hwnd, out var lpPaint);
                    PInvoke.EndPaint(hwnd, ref lpPaint);
                    handled = true;
                    break;
                    //case NativeMethods.WM.WINDOWPOSCHANGING:
                    //    break;
                    //case NativeMethods.WM.GETOBJECT:
                    //    handled = true;
                    //    break;
            }
        }
        return new(IntPtr.Zero);
    }

    LRESULT IHwndHost.WndProc(HWND hwnd, uint msg, WPARAM wParam, LPARAM lParam, ref bool handled)
        => WndProc(hwnd, msg, wParam, lParam, ref handled);
#endif

    /// <summary>
    /// Gets or sets a bag of options which are used during initialization of the control's <see cref="P:Avalonia.Controls.WebView2.CoreWebView2" />.
    /// This property cannot be modified (an exception will be thrown) after initialization of the control's CoreWebView2 has started.
    /// </summary>
    /// <exception cref="T:System.InvalidOperationException">Thrown if initialization of the control's CoreWebView2 has already started.</exception>
    [Browsable(false)]
    public CoreWebView2CreationProperties? CreationProperties
    {
        get => _creationProperties;
        set
        {
            if (_initTask != null)
                throw new InvalidOperationException("CreationProperties cannot be modified after the initialization of CoreWebView2 has begun.");
            _creationProperties = value;
        }
    }

#if !DISABLE_WEBVIEW2_CORE
    CoreWebView2Environment? Environment { get; set; }

    CoreWebView2ControllerOptions? ControllerOptions { get; set; }

    /// <summary>
    /// Explicitly trigger initialization of the control's <see cref="P:Avalonia.Controls.WebView2.CoreWebView2" />.
    /// </summary>
    /// <param name="environment">
    /// A pre-created <see cref="T:Microsoft.Web.WebView2.Core.CoreWebView2Environment" /> that should be used to create the <see cref="P:Avalonia.Controls.WebView2.CoreWebView2" />.
    /// Creating your own environment gives you control over several options that affect how the <see cref="P:Avalonia.Controls.WebView2.CoreWebView2" /> is initialized.
    /// If you pass <c>null</c> (the default value) then a default environment will be created and used automatically.
    /// </param>
    /// <param name="controllerOptions">
    /// A pre-created <see cref="T:Microsoft.Web.WebView2.Core.CoreWebView2ControllerOptions" /> that should be used to create the <see cref="T:Microsoft.Web.WebView2.Core.CoreWebView2" />.
    /// Creating your own controller options gives you control over several options that affect how the <see cref="T:Microsoft.Web.WebView2.Core.CoreWebView2" /> is initialized.
    /// If you pass a controllerOptions to this method then it will override any settings specified on the <see cref="P:Avalonia.Controls.WebView2.CreationProperties" /> property.
    /// If you pass <c>null</c> (the default value) and no value has been set to <see cref="P:Avalonia.Controls.WebView2.CreationProperties" /> then a default controllerOptions will be created and used automatically.
    /// </param>
    /// <returns>
    /// A Task that represents the background initialization process.
    /// When the task completes then the <see cref="P:Avalonia.Controls.WebView2.CoreWebView2" /> property will be available for use (i.e. non-null).
    /// Note that the control's <see cref="E:Avalonia.Controls.WebView2.CoreWebView2InitializationCompleted" /> event will be invoked before the task completes
    /// or on exceptions.
    /// </returns>
    /// <remarks>
    /// Unless previous initialization has already failed, calling this method additional times with the same parameter will have no effect (any specified environment is ignored) and return the same Task as the first call.
    /// Unless previous initialization has already failed, calling this method after initialization has been implicitly triggered by setting the <see cref="P:Avalonia.Controls.WebView2.Source" /> property will have no effect if no environment is given
    /// and simply return a Task representing that initialization already in progress.
    /// Unless previous initialization has already failed, calling this method with a different environment after initialization has begun will result in an <see cref="T:System.ArgumentException" />. For example, this can happen if you begin initialization
    /// by setting the <see cref="P:Avalonia.Controls.WebView2.Source" /> property and then call this method with a new environment, if you begin initialization with <see cref="P:Avalonia.Controls.WebView2.CreationProperties" /> and then call this method with a new
    /// environment, or if you begin initialization with one environment and then call this method with no environment specified.
    /// When this method is called after previous initialization has failed, it will trigger initialization of the control's <see cref="P:Avalonia.Controls.WebView2.CoreWebView2" /> again.
    /// Note that even though this method is asynchronous and returns a Task, it still must be called on the UI thread like most public functionality of most UI controls.
    /// </remarks>
    /// <exception cref="T:System.ArgumentException">
    /// Thrown if this method is called with a different environment than when it was initialized. See Remarks for more info.
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// Thrown if this instance of <see cref="P:Avalonia.Controls.WebView2.CoreWebView2" /> is already disposed, or if the calling thread isn't the thread which created this object (usually the UI thread). See <see cref="P:System.Windows.Forms.Control.InvokeRequired" /> for more info.
    /// May also be thrown if the browser process has crashed unexpectedly and left the control in an invalid state. We are considering throwing a different type of exception for this case in the future.
    /// </exception>
    public Task EnsureCoreWebView2Async(CoreWebView2Environment? environment = null, CoreWebView2ControllerOptions? controllerOptions = null)
    {
        if (IsInDesignMode || !IsSupported)
            return Task.CompletedTask;
        VerifyNotClosedGuard();
        VerifyBrowserNotCrashedGuard();
        if (!CheckAccess())
            throw new InvalidOperationException("The method EnsureCoreWebView2Async can be invoked only from the UI thread.");
        if (_initTask == null || _initTask.IsFaulted)
        {
            _initTask = InitCoreWebView2Async(environment, controllerOptions);
        }
        else
        {
            if ((!_isExplicitEnvironment && environment != null) || (_isExplicitEnvironment && environment != null && Environment != environment))
                throw new ArgumentException("WebView2 was already initialized with a different CoreWebView2Environment. Check to see if the Source property was already set or EnsureCoreWebView2Async was previously called with different values.");
            if ((!_isExplicitControllerOptions && controllerOptions != null) || (_isExplicitControllerOptions && controllerOptions != null && ControllerOptions != controllerOptions))
                throw new ArgumentException("WebView2 was already initialized with a different CoreWebView2ControllerOptions. Check to see if the Source property was already set or EnsureCoreWebView2Async was previously called with different values.");
        }
        return _initTask;
    }

    /// <summary>
    /// This is the function which implements the actual background initialization task.
    /// Cannot be called if the control is already initialized or has been disposed.
    /// </summary>
    /// <param name="environment">
    /// The environment to use to create the <see cref="T:Microsoft.Web.WebView2.Core.CoreWebView2Controller" />.
    /// If that is null then a default environment is created with <see cref="M:Microsoft.Web.WebView2.Core.CoreWebView2Environment.CreateAsync(System.String,System.String,Microsoft.Web.WebView2.Core.CoreWebView2EnvironmentOptions)" /> and its default parameters.
    /// </param>
    /// <param name="controllerOptions">
    /// The controllerOptions to use to create the <see cref="T:Microsoft.Web.WebView2.Core.CoreWebView2Controller" />.
    /// If that is null then a default controllerOptions is created with its default parameters.
    /// </param>
    /// <returns>A task representing the background initialization process.</returns>
    /// <remarks>All the event handlers added here need to be removed in <see cref="M:Avalonia.Controls.WebView2.Dispose(System.Boolean)" />.</remarks>
    async Task InitCoreWebView2Async(CoreWebView2Environment? environment = null, CoreWebView2ControllerOptions? controllerOptions = null)
    {
        WebView2 sender = this;
        try
        {
            if (environment != null)
            {
                sender.Environment = environment;
                sender._isExplicitEnvironment = true;
            }
            else if (sender.CreationProperties != null)
            {
                var environmentAsync = await sender.CreationProperties.CreateEnvironmentAsync();
                sender.Environment = environmentAsync;
            }
            if (sender.Environment == null)
            {
                var async = await CoreWebView2Environment.CreateAsync(null, null, null);
                sender.Environment = async;
            }
            if (controllerOptions != null)
            {
                sender.ControllerOptions = controllerOptions;
                sender._isExplicitControllerOptions = true;
            }
            else if (sender.CreationProperties != null)
                sender.ControllerOptions = sender.CreationProperties.CreateCoreWebView2ControllerOptions(sender.Environment);
            if (sender._defaultBackgroundColor != _defaultBackgroundColorDefaultValue)
                System.Environment.SetEnvironmentVariable("WEBVIEW2_DEFAULT_BACKGROUND_COLOR", Color.FromArgb(sender.DefaultBackgroundColor.ToArgb()).Name);
            if (sender.ControllerOptions != null)
            {
                // ISSUE: explicit non-virtual call
                CoreWebView2Controller view2ControllerAsync = await sender.Environment.CreateCoreWebView2ControllerAsync(await _hwndTaskSource.Task, sender.ControllerOptions);
                sender._coreWebView2Controller = view2ControllerAsync;
            }
            else
            {
                // ISSUE: explicit non-virtual call
                CoreWebView2Controller view2ControllerAsync = await sender.Environment.CreateCoreWebView2ControllerAsync(await _hwndTaskSource.Task);
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

            SubscribeHandlers(sender);
            sender._coreWebView2Controller.CoreWebView2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.Document);
            if (sender.Focusable)
                sender._coreWebView2Controller.MoveFocus(CoreWebView2MoveFocusReason.Programmatic);

            sender._coreWebView2Controller.CoreWebView2.Settings.AreDevToolsEnabled = sender.CreationProperties?.EnabledDevTools ?? false;
            sender.CoreWebView2InitializationCompleted?.Invoke(sender, new CoreWebView2InitializationCompletedEventArgs());

            await InitJavaScriptOnDocumentCreatedAsync();

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
#endif

    void WebView2_HandleDestroyed(object? sender, EventArgs e)
    {
#if !DISABLE_WEBVIEW2_CORE
        if (_coreWebView2Controller != null)
        {
            _coreWebView2Controller.IsVisible = false;
            _coreWebView2Controller.ParentWindow = IntPtr.Zero;
        }
#endif
    }

    protected virtual RoutedEvent? PreviewKeyDownEvent => KeyDownEvent;

    protected virtual RoutedEvent? PreviewKeyUpEvent => KeyUpEvent;

#if !DISABLE_WEBVIEW2_CORE
    /// <summary>
    /// This is an event handler for our CoreWebView2Controller's AcceleratorKeyPressed event.
    /// This is called to inform us about key presses that are likely to have special behavior (e.g. esc, return, Function keys, letters with modifier keys).
    /// Avalonia can't detect this input because Windows sends it directly to the Win32 CoreWebView2Controller control.
    /// We implement this by generating standard Avalonia key input events, allowing callers to handle the input in the usual Avalonia way if they want.
    /// If nobody handles the Avalonia key events then we'll allow the default CoreWebView2Controller logic (if any) to handle it.
    /// Of the possible options, this implementation should provide the most flexibility to callers.
    /// </summary>
    void CoreWebView2Controller_AcceleratorKeyPressed(object? sender, CoreWebView2AcceleratorKeyPressedEventArgs e)
    {
        var eventArgs = new WebView2KeyEventArgs(KeyInterop.KeyFromVirtualKey((int)e.VirtualKey))
        {
            RoutedEvent = e.KeyEventKind == CoreWebView2KeyEventKind.KeyDown || e.KeyEventKind == CoreWebView2KeyEventKind.SystemKeyDown ? PreviewKeyDownEvent : PreviewKeyUpEvent,
        };
        RaiseEvent(eventArgs);
        e.Handled = eventArgs.Handled;
    }

    //void CoreWebView2Controller_MoveFocusRequested(object? sender,
    // CoreWebView2MoveFocusRequestedEventArgs e)
    //{
    //    bool forward = e.Reason == CoreWebView2MoveFocusReason.Next || e.Reason == CoreWebView2MoveFocusReason.Programmatic;
    //    Control control = (Control)this.FindForm() ?? this.Parent;
    //    e.Handled = control == null || control.SelectNextControl((Control)this, forward, true, true, true);
    //    if (this._lastMoveFocusReason == CoreWebView2MoveFocusReason.Programmatic)
    //        return;
    //    this._coreWebView2Controller.MoveFocus(this._lastMoveFocusReason);
    //    this._lastMoveFocusReason = CoreWebView2MoveFocusReason.Programmatic;
    //}
#endif

    /// <summary>
    /// This is a handler for our base UIElement's IsVisibleChanged event.
    /// It's predictably fired whenever IsVisible changes, and IsVisible reflects the actual current visibility status of the control.
    /// We just need to pass this info through to our CoreWebView2Controller so it can save some effort when the control isn't visible.
    /// </summary>
    protected virtual void IsVisibleChanged(EventArgs e)
    {
#if !DISABLE_WEBVIEW2_CORE
        if (_coreWebView2Controller == null)
            return;
        _coreWebView2Controller.IsVisible = IsVisible;
#endif
    }

    protected override void OnGotFocus(GotFocusEventArgs e)
    {
        base.OnGotFocus(e);
        if (_coreWebView2Controller != null)
        {
            if (!_browserCrashed)
            {
#if !DISABLE_WEBVIEW2_CORE
                try
                {
                    _coreWebView2Controller.MoveFocus(_lastMoveFocusReason);
                }
                catch (InvalidOperationException ex)
                {
                    if (ex.InnerException != null && ex.InnerException.HResult != -2147019873)
                        throw ex;
                }
#endif
            }
        }
#if !DISABLE_WEBVIEW2_CORE
        _lastMoveFocusReason = CoreWebView2MoveFocusReason.Programmatic;
#endif
    }

#if !DISABLE_WEBVIEW2_CORE
    /// <summary>
    /// The underlying CoreWebView2. Use this property to perform more operations on the WebView2 content than is exposed
    /// on the WebView2. This value is null until it is initialized and the object itself has undefined behaviour once the control is disposed.
    /// You can force the underlying CoreWebView2 to
    /// initialize via the <see cref="M:Avalonia.Controls.WebView2.EnsureCoreWebView2Async(Microsoft.Web.WebView2.Core.CoreWebView2Environment,Microsoft.Web.WebView2.Core.CoreWebView2ControllerOptions)" /> method.
    /// </summary>
    /// <exception cref="T:System.InvalidOperationException">Thrown if the calling thread isn't the thread which created this object (usually the UI thread). See <see cref="P:System.Windows.Forms.Control.InvokeRequired" /> for more info.</exception>
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
                    throw new InvalidOperationException("CoreWebView2 can only be accessed from the UI thread.", ex);
                throw;
            }
        }
    }
#endif

    /// <summary>The zoom factor for the WebView.</summary>
    public double ZoomFactor
#if !DISABLE_WEBVIEW2_CORE
    {
        get => _coreWebView2Controller != null ? _coreWebView2Controller.ZoomFactor : _zoomFactor;
        set
        {
            _zoomFactor = value;
            if (_coreWebView2Controller == null)
                return;
            _coreWebView2Controller.ZoomFactor = value;
        }
    }
#else
    { get; set; }
#endif

    /// <summary>Enable/disable external drop.</summary>
    public bool AllowExternalDrop
#if !DISABLE_WEBVIEW2_CORE
    {
        get => _coreWebView2Controller != null ? _coreWebView2Controller.AllowExternalDrop : _allowExternalDrop;
        set
        {
            _allowExternalDrop = value;
            if (_coreWebView2Controller == null)
                return;
            _coreWebView2Controller.AllowExternalDrop = value;
        }
    }
#else
    { get; set; }
#endif

    /// <summary>Enable/disable external drop.</summary>
    public string UserDataFolder
#if !DISABLE_WEBVIEW2_CORE
    {
        get => _creationProperties != null ? _creationProperties.UserDataFolder : _userDataFolder;
        set
        {
            _userDataFolder = value;

            if (_creationProperties == null)
                return;
            _creationProperties.UserDataFolder = value;
        }
    }
#else
    { get; set; }
#endif

    /// <summary>Enable/disable external drop.</summary>
    public bool EnabledDevTools
#if !DISABLE_WEBVIEW2_CORE
    {
        get => _creationProperties != null ? _creationProperties.EnabledDevTools : _enabledDevTools;
        set
        {
            _enabledDevTools = value;

            if (_creationProperties == null)
                return;
            _creationProperties.EnabledDevTools = value;
        }
    }
#else
    { get; set; }
#endif

    public override void BeginInit()
    {
        base.BeginInit();
        _implicitInitGate.BeginInit();
    }

    public override void EndInit()
    {
        _implicitInitGate.EndInit();
        base.EndInit();
    }

    /// <summary>
    /// The Source property is the URI of the top level document of the
    /// WebView2. Setting the Source is equivalent to calling <see cref="M:Microsoft.Web.WebView2.Core.CoreWebView2.Navigate(System.String)" />.
    /// Setting the Source will trigger initialization of the <see cref="P:Avalonia.Controls.WebView2.CoreWebView2" />, if not already initialized.
    /// The default value of Source is <c>null</c>, indicating that the <see cref="P:Avalonia.Controls.WebView2.CoreWebView2" /> is not yet initialized.
    /// </summary>
    /// <exception cref="T:System.ArgumentException">Specified value is not an absolute <see cref="T:System.Uri" />.</exception>
    /// <exception cref="T:System.NotImplementedException">Specified value is <c>null</c> and the control is initialized.</exception>
    /// <seealso cref="M:Microsoft.Web.WebView2.Core.CoreWebView2.Navigate(System.String)" />
    [Browsable(true)]
    public Uri? Source
    {
        get => _source;
        set
        {
            if (value == null)
            {
                if (_source == null)
                {
                    return;
                }
                throw new NotImplementedException("The Source property cannot be set to null.");
            }
            else
            {
                if (!value.IsAbsoluteUri)
                {
                    throw new ArgumentException("Only absolute URI is allowed", "Source");
                }
                if (_source == null || _source.AbsoluteUri != value.AbsoluteUri)
                {
                    _htmlSource = null;
                    SetAndRaise(SourceProperty, ref _source, value);
#if !DISABLE_WEBVIEW2_CORE
                    if (CoreWebView2 != null) CoreWebView2.Navigate(value.AbsoluteUri);
#endif
                }
#if !DISABLE_WEBVIEW2_CORE
                _implicitInitGate.RunWhenOpen(() => EnsureCoreWebView2Async());
#endif
            }
        }
    }

    [Browsable(true)]
    public string? HtmlSource
    {
        get => _htmlSource;
        set
        {
            if (value == null)
            {
                if (_htmlSource == null)
                {
                    return;
                }
                throw new NotImplementedException("The HtmlSource property cannot be set to null.");
            }
            else
            {
                if (_htmlSource == null || _htmlSource != value)
                {
                    _source = null;
                    SetAndRaise(HtmlSourceProperty, ref _htmlSource, value);
#if !DISABLE_WEBVIEW2_CORE
                    if (CoreWebView2 != null) CoreWebView2.NavigateToString(value);
#endif
                }
#if !DISABLE_WEBVIEW2_CORE
                _implicitInitGate.RunWhenOpen(() => EnsureCoreWebView2Async());
#endif
            }
        }
    }

    /// <summary>
    /// Returns true if the webview can navigate to a next page in the
    /// navigation history via the <see cref="M:Avalonia.Controls.WebView2.GoForward" /> method.
    /// This is equivalent to the <see cref="P:Microsoft.Web.WebView2.Core.CoreWebView2.CanGoForward" />.
    /// If the underlying <see cref="P:Avalonia.Controls.WebView2.CoreWebView2" /> is not yet initialized, this property is <c>false</c>.
    /// </summary>
    /// <seealso cref="P:Microsoft.Web.WebView2.Core.CoreWebView2.CanGoForward" />
    [Browsable(false)]
    public bool CanGoForward
#if !DISABLE_WEBVIEW2_CORE
    {
        get
        {
            var coreWebView2 = CoreWebView2;
            return coreWebView2 != null && coreWebView2.CanGoForward;
        }
    }
#else
    { get; }
#endif

    /// <summary>
    /// Returns <c>true</c> if the webview can navigate to a previous page in the
    /// navigation history via the <see cref="M:Avalonia.Controls.WebView2.GoBack" /> method.
    /// This is equivalent to the <see cref="P:Microsoft.Web.WebView2.Core.CoreWebView2.CanGoBack" />.
    /// If the underlying <see cref="P:Avalonia.Controls.WebView2.CoreWebView2" /> is not yet initialized, this property is <c>false</c>.
    /// </summary>
    /// <seealso cref="P:Microsoft.Web.WebView2.Core.CoreWebView2.CanGoBack" />
    [Browsable(false)]
    public bool CanGoBack
#if !DISABLE_WEBVIEW2_CORE
    {
        get
        {
            var coreWebView2 = CoreWebView2;
            return coreWebView2 != null && coreWebView2.CanGoBack;
        }
    }
#else
    { get; }
#endif

    /// <summary>The default background color for the WebView.</summary>
    public Color DefaultBackgroundColor
#if !DISABLE_WEBVIEW2_CORE
    {
        get => _coreWebView2Controller != null ? _coreWebView2Controller.DefaultBackgroundColor : _defaultBackgroundColor;
        set
        {
            if (_coreWebView2Controller != null)
                _coreWebView2Controller.DefaultBackgroundColor = value;
            else
                _defaultBackgroundColor = value;
            Fill = new ImmutableSolidColorBrush(AvaloniaColor.FromArgb(_defaultBackgroundColor.A, _defaultBackgroundColor.R, _defaultBackgroundColor.G, _defaultBackgroundColor.B));
        }
    }
#else
    { get; set; }
#endif

    /// <summary>
    /// Executes the provided script in the top level document of the <see cref="T:Avalonia.Controls.WebView2" />.
    /// This is equivalent to <see cref="M:Microsoft.Web.WebView2.Core.CoreWebView2.ExecuteScriptAsync(System.String)" />.
    /// </summary>
    /// <exception cref="T:System.InvalidOperationException">The underlying <see cref="P:Avalonia.Controls.WebView2.CoreWebView2" /> is not yet initialized.</exception>
    /// <exception cref="T:System.InvalidOperationException">Thrown when browser process has unexpectedly and left this control in an invalid state. We are considering throwing a different type of exception for this case in the future.</exception>
    /// <seealso cref="M:Microsoft.Web.WebView2.Core.CoreWebView2.ExecuteScriptAsync(System.String)" />
    public async Task<string> ExecuteScriptAsync(string script)
    {
        VerifyBrowserNotCrashedGuard();
#if !DISABLE_WEBVIEW2_CORE
        if (CoreWebView2 != null)
        {
            return await CoreWebView2.ExecuteScriptAsync(script);
        }
#endif
        return await Task.FromResult(string.Empty);
    }

    /// <summary>
    /// Reloads the top level document of the <see cref="T:Avalonia.Controls.WebView2" />.
    /// This is equivalent to <see cref="M:Microsoft.Web.WebView2.Core.CoreWebView2.Reload" />.
    /// </summary>
    /// <exception cref="T:System.InvalidOperationException">The underlying <see cref="P:Avalonia.Controls.WebView2.CoreWebView2" /> is not yet initialized.</exception>
    /// <exception cref="T:System.InvalidOperationException">Thrown when browser process has unexpectedly and left this control in an invalid state. We are considering throwing a different type of exception for this case in the future.</exception>
    /// <seealso cref="M:Microsoft.Web.WebView2.Core.CoreWebView2.Reload" />
    public void Reload()
    {
        VerifyBrowserNotCrashedGuard();
#if !DISABLE_WEBVIEW2_CORE
        if (CoreWebView2 != null)
        {
            CoreWebView2.Reload();
        }
#endif
    }

    /// <summary>
    /// Navigates to the next page in navigation history.
    /// This is equivalent to <see cref="M:Microsoft.Web.WebView2.Core.CoreWebView2.GoForward" />.
    /// If the underlying <see cref="P:Avalonia.Controls.WebView2.CoreWebView2" /> is not yet initialized, this method does nothing.
    /// </summary>
    /// <seealso cref="M:Microsoft.Web.WebView2.Core.CoreWebView2.GoForward" />
    public void GoForward()
#if !DISABLE_WEBVIEW2_CORE
        => CoreWebView2?.GoForward();
#else
    { }
#endif

    /// <summary>
    /// Navigates to the previous page in navigation history.
    /// This is equivalent to <see cref="M:Microsoft.Web.WebView2.Core.CoreWebView2.GoBack" />.
    /// If the underlying <see cref="P:Avalonia.Controls.WebView2.CoreWebView2" /> is not yet initialized, this method does nothing.
    /// </summary>
    /// <seealso cref="M:Microsoft.Web.WebView2.Core.CoreWebView2.GoBack" />
    public void GoBack()
#if !DISABLE_WEBVIEW2_CORE
        => CoreWebView2?.GoBack();
#else
    { }
#endif

    /// <summary>
    /// Renders the provided HTML as the top level document of the <see cref="T:Avalonia.Controls.WebView2" />.
    /// This is equivalent to <see cref="M:Microsoft.Web.WebView2.Core.CoreWebView2.NavigateToString(System.String)" />.
    /// </summary>
    /// <exception cref="T:System.InvalidOperationException">The underlying <see cref="P:Avalonia.Controls.WebView2.CoreWebView2" /> is not yet initialized.</exception>
    /// <exception cref="T:System.InvalidOperationException">Thrown when browser process has unexpectedly and left this control in an invalid state. We are considering throwing a different type of exception for this case in the future.</exception>
    /// <remarks>The <c>htmlContent</c> parameter may not be larger than 2 MB (2 * 1024 * 1024 bytes) in total size. The origin of the new page is <c>about:blank</c>.</remarks>
    /// <seealso cref="M:Microsoft.Web.WebView2.Core.CoreWebView2.NavigateToString(System.String)" />
    public void NavigateToString(string htmlContent)
    {
        VerifyBrowserNotCrashedGuard();
#if !DISABLE_WEBVIEW2_CORE
        if (CoreWebView2 != null)
        {
            CoreWebView2.NavigateToString(htmlContent);
        }
#endif
    }

    /// <summary>
    /// Stops any in progress navigation in the <see cref="T:Avalonia.Controls.WebView2" />.
    /// This is equivalent to <see cref="M:Microsoft.Web.WebView2.Core.CoreWebView2.Stop" />.
    /// If the underlying <see cref="P:Avalonia.Controls.WebView2.CoreWebView2" /> is not yet initialized, this method does nothing.
    /// </summary>
    /// <seealso cref="M:Microsoft.Web.WebView2.Core.CoreWebView2.Stop" />
    public void Stop()
#if !DISABLE_WEBVIEW2_CORE
        => CoreWebView2?.Stop();
#else
    { }
#endif

#if !DISABLE_WEBVIEW2_CORE
    void VerifyNotClosedGuard()
    {
        if (disposedValue)
            throw new InvalidOperationException("The instance of CoreWebView2 is disposed and unable to complete this operation.");
    }
#endif

    void VerifyBrowserNotCrashedGuard()
    {
        if (_browserCrashed)
            throw new InvalidOperationException("The instance of CoreWebView2 is no longer valid because the browser process crashed.To work around this, please listen for the ProcessFailed event to explicitly manage the lifetime of the WebView2 control in the event of a browser failure.https://docs.microsoft.com/en-us/dotnet/api/microsoft.web.webview2.core.corewebview2.processfailed");
    }

    readonly TaskCompletionSource<IntPtr> _hwndTaskSource = new();

    protected Window? Window { get; set; }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        if (e.Root is Window window)
        {
            var prevWindow = Window;
            var isSameWindow = prevWindow == window;
            if (prevWindow != null)
            {
                if (!isSameWindow)
                {
                    prevWindow.Closed -= Window_Closed;
                }
            }
            if (!isSameWindow)
            {
                // Different windows cannot be reinitialized successfully
                Window = window;
                Window.Closed += Window_Closed;

                var handle = window.TryGetPlatformHandle();
                _hwndTaskSource.TrySetResult(handle.Handle);
                _implicitInitGate.OnSynchronizationContextExists();
            }
        }
#if !DISABLE_WEBVIEW2_CORE
        if (_coreWebView2Controller != null)
        {
            if (!_coreWebView2Controller.IsVisible)
                _coreWebView2Controller.IsVisible = true;
        }
#endif
        base.OnAttachedToVisualTree(e);
    }

    void Window_Closed(object? sender, EventArgs e)
    {
        Dispose();
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
#if !DISABLE_WEBVIEW2_CORE
        if (_coreWebView2Controller != null)
        {
            if (_coreWebView2Controller.IsVisible)
                _coreWebView2Controller.IsVisible = false;
        }
#endif
    }

    /// <summary>
    /// This is overridden from <see cref="IHwndHost" /> and called when our control's location has changed.
    /// The HwndHost takes care of updating the HWND we created.
    /// What we need to do is move our CoreWebView2 to match the new location.
    /// </summary>
    protected virtual void OnWindowPositionChanged(Rectangle rectangle)
    {
#if !DISABLE_WEBVIEW2_CORE
        if (_coreWebView2Controller != null)
        {
            _coreWebView2Controller.Bounds = rectangle;
            _coreWebView2Controller.NotifyParentWindowPositionChanged();
        }
#endif
    }

#if DEBUG
    public void Test()
    {
#if !DISABLE_WEBVIEW2_CORE
        if (_coreWebView2Controller != null)
        {
            _coreWebView2Controller.CoreWebView2.OpenDevToolsWindow();
        }
#endif
    }
#endif
}