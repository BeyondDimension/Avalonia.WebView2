using CefNet.Internal;
using System.Windows.Interop;

namespace Avalonia.Controls;

public class WebView2 : NativeControlHost, IHwndHost, IDisposable
{
    protected override void OnInitialized()
    {
        base.OnInitialized();
        GlobalHooks.Initialize(this);
    }

    static WebView2()
    {
        SourceProperty.Changed.Subscribe(SourcePropertyChanged);
        ZoomFactorProperty.Changed.Subscribe(ZoomFactorPropertyChanged);
        DefaultBackgroundColorProperty.Changed.Subscribe(DefaultBackgroundColorPropertyChanged);
        AllowExternalDropProperty.Changed.Subscribe(AllowExternalDropPropertyChanged);
        IsVisibleProperty.Changed.Subscribe(IsVisibleChanged);
    }

    protected Window? Window { get; private set; }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        if (e.Root is Window window)
        {
            Window = window;
        }
        base.OnAttachedToVisualTree(e);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        Window = null;
        Dispose();
    }

    /// <summary>
    /// The <see cref="AvaloniaProperty" /> which backs the <see cref="CreationProperties" /> property.
    /// </summary>
    /// <seealso cref="AvaloniaProperty" />
    public static readonly StyledProperty<CoreWebView2CreationProperties> CreationPropertiesProperty = AvaloniaProperty.Register<WebView2, CoreWebView2CreationProperties>(nameof(CreationProperties), coerce: CoerceCreationPropertiesProperty);

    readonly TaskCompletionSource<IntPtr> _hwndTaskSource = new();
    Task? _initTask;
    bool _isExplicitEnvironment;
    bool _isExplicitControllerOptions;
    bool _browserCrashed;
    bool disposedValue;

    /// <summary>
    /// This is a "gate" which controls whether or not implicit initialization can occur.
    /// If implicit initialization is triggered while the gate is closed,
    /// then the initialization should be delayed until the gate opens.
    /// When we want to trigger implicit initialization we route the call through this gate.
    /// If the gate is open then the initialization will proceed.
    /// If the gate is closed then it will remember to trigger the initialization when it opens.
    /// </summary>
    readonly ImplicitInitGate _implicitInitGate = new();
    AvaloniaProperty? _propertyChangingFromCore;

    /// <summary>
    /// The <see cref="AvaloniaProperty" /> which backs the <see cref="Source" /> property.
    /// </summary>
    public static readonly DirectProperty<WebView2, Uri?> SourceProperty = AvaloniaProperty.RegisterDirect<WebView2, Uri?>(nameof(Source), x => x._Source, (x, y) => x.Source = y);

    /// <summary>
    /// The <see cref="AvaloniaProperty" /> which backs the <see cref="CanGoBack" /> property.
    /// </summary>
    public static readonly DirectProperty<WebView2, bool> CanGoBackProperty = AvaloniaProperty.RegisterDirect<WebView2, bool>(nameof(CanGoBack), x => x._CanGoBack);

    /// <summary>
    /// The <see cref="AvaloniaProperty" /> which backs the <see cref="CanGoForward" /> property.
    /// </summary>
    public static readonly DirectProperty<WebView2, bool> CanGoForwardProperty = AvaloniaProperty.RegisterDirect<WebView2, bool>(nameof(CanGoForward), x => x._CanGoForward);

    /// <summary>
    /// The <see cref="AvaloniaProperty" /> which backs the <see cref="ZoomFactor" /> property.
    /// </summary>
    public static readonly DirectProperty<WebView2, double> ZoomFactorProperty = AvaloniaProperty.RegisterDirect<WebView2, double>(nameof(ZoomFactor), x => x._ZoomFactor, (x, y) => x.ZoomFactor = y);

    /// <summary>
    /// The <see cref="AvaloniaProperty" /> which backs the <see cref="DefaultBackgroundColor" /> property.
    /// </summary>
    public static readonly StyledProperty<Color> DefaultBackgroundColorProperty = AvaloniaProperty.Register<WebView2, Color>(nameof(DefaultBackgroundColor), Color.White);

    /// <summary>
    /// The <see cref="AvaloniaProperty" /> which backs the <see cref="AllowExternalDropProperty" /> property.
    /// </summary>
    public static readonly StyledProperty<bool> AllowExternalDropProperty = AvaloniaProperty.Register<WebView2, bool>(nameof(AllowExternalDrop), true);

    /// <summary>
    /// The <see cref="AvaloniaProperty" /> which backs the <see cref="DesignModeForegroundColor" /> property.
    /// </summary>
    public static readonly StyledProperty<Color> DesignModeForegroundColorProperty = AvaloniaProperty.Register<WebView2, Color>(nameof(DesignModeForegroundColor), Color.Black);

    /// <summary>
    /// Creates a new instance of a WebView2 control.
    /// Note that the control's <see cref="CoreWebView2" /> will be null until initialized.
    /// See the <see cref="WebView2" /> class documentation for an initialization overview.
    /// </summary>
    public WebView2()
    {

    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        OnWindowPositionChanged(Bounds);
        _implicitInitGate.OnSynchronizationContextExists();
    }

    /// <summary>
    /// Gets or sets a bag of options which are used during initialization of the control's <see cref="CoreWebView2" />.
    /// Setting this property will not work after initialization of the control's <see cref="CoreWebView2" /> has started (the old value will be retained).
    /// See the <see cref="WebView2" /> class documentation for an initialization overview.
    /// </summary>
    /// <seealso cref="WebView2" />
    [Category("Common")]
    public CoreWebView2CreationProperties CreationProperties
    {
        get => GetValue(CreationPropertiesProperty);
        set => SetValue(CreationPropertiesProperty, value);
    }

    static CoreWebView2CreationProperties CoerceCreationPropertiesProperty(IAvaloniaObject d, CoreWebView2CreationProperties value) => d is WebView2 webView2 && webView2.Environment != null ? value : null!;

    protected CoreWebView2Environment? Environment { get; set; }

    protected IPlatformHandle? PlatformHandle { get; private set; }

    public IntPtr Handle
    {
        get
        {
            var platformHandle = PlatformHandle;
            if (platformHandle != null) return platformHandle.Handle;
            return IntPtr.Zero;
        }
    }

    public void Test()
    {

    }

    /// <summary>
    /// This is overridden from <see cref="IPlatformHandle" /> and is called to instruct us to create our HWND.
    /// </summary>
    /// <param name="parent">The HWND that we should use as the parent of the one we create.</param>
    /// <returns>The HWND that we created.</returns>
    /// <seealso cref="NativeControlHost.CreateNativeControlCore(IPlatformHandle)" />
    protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
    {
        if (OperatingSystem.IsWindows())
        {
            var hwnd = Window!.PlatformImpl.Handle.Handle;
            IntPtr windowExW = NativeMethods.CreateWindowExW(NativeMethods.WS_EX.TRANSPARENT, "static", string.Empty, NativeMethods.WS.CLIPCHILDREN | NativeMethods.WS.VISIBLE | NativeMethods.WS.CHILD, 0, 0, 0, 0, hwnd, IntPtr.Zero, Marshal.GetHINSTANCE(typeof(NativeMethods).Module), IntPtr.Zero);
            if (CoreWebView2Controller != null)
                ReparentController(windowExW);
            if (!_hwndTaskSource.Task.IsCompleted)
                _hwndTaskSource.SetResult(windowExW);
            return PlatformHandle = new PlatformHandle(windowExW, "HWND");
        }
        return base.CreateNativeControlCore(parent);
    }

    /// <summary>
    /// This is overridden from <see cref="IPlatformHandle" /> and is called to instruct us to destroy our HWND.
    /// </summary>
    /// <param name="control">Our HWND that we need to destroy.</param>
    /// <seealso cref="NativeControlHost.DestroyNativeControlCore(IPlatformHandle)" />
    protected override void DestroyNativeControlCore(IPlatformHandle control)
    {
        PlatformHandle = null;
        if (OperatingSystem.IsWindows())
        {
            if (CoreWebView2Controller != null)
                ReparentController(IntPtr.Zero);
            NativeMethods.DestroyWindow(control.Handle);
        }
        base.DestroyNativeControlCore(control);
    }

    /// <summary>
    /// This is overridden from <see cref="NativeControlHost" /> and is called to provide us with Win32 messages that are sent to our hwnd.
    /// </summary>
    /// <param name="hwnd">Window receiving the message (should always match our <see cref="IPlatformHandle.Handle" />).</param>
    /// <param name="msg">Indicates the message being received.  See Win32 documentation for WM_* constant values.</param>
    /// <param name="wParam">The "wParam" data being provided with the message.  Meaning varies by message.</param>
    /// <param name="lParam">The "lParam" data being provided with the message.  Meaning varies by message.</param>
    /// <param name="handled">If true then the message will not be forwarded to any (more) <see cref="GlobalHooks" /> handlers.</param>
    /// <returns>Return value varies by message.</returns>
    protected virtual IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        switch ((NativeMethods.WM)msg)
        {
            case NativeMethods.WM.SETFOCUS:
                var webView2Controller = CoreWebView2Controller;
                if (webView2Controller != null)
                {
                    //webView2Controller.MoveFocus(CoreWebView2MoveFocusReason.Programmatic);
                    break;
                }
                break;
            case NativeMethods.WM.PAINT:
                if (!IsInDesignMode)
                {
                    NativeMethods.BeginPaint(hwnd, out var lpPaint);
                    NativeMethods.EndPaint(hwnd, ref lpPaint);
                    handled = true;
                    return IntPtr.Zero;
                }
                break;
                //case NativeMethods.WM.WINDOWPOSCHANGING:
                //    break;
                //case NativeMethods.WM.GETOBJECT:
                //    handled = true;
                //    break;
        }
        return IntPtr.Zero;
    }

    IntPtr IHwndHost.WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        => WndProc(hwnd, msg, wParam, lParam, ref handled);

    protected CoreWebView2Controller? CoreWebView2Controller { get; set; }

    protected CoreWebView2ControllerOptions? ControllerOptions { get; set; }

    /// <summary>
    /// Accesses the complete functionality of the underlying <see cref="CoreWebView2" /> COM API.
    /// Returns <c>null</c> until initialization has completed.
    /// See the <see cref="WebView2" /> class documentation for an initialization overview.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the calling thread isn't the thread which created this object (usually the UI thread). See <see cref="AvaloniaObject.VerifyAccess" /> for more info.
    /// May also be thrown if the browser process has crashed unexpectedly and left the control in an invalid state. We are considering throwing a different type of exception for this case in the future.
    /// </exception>
    /// <exception cref="ObjectDisposedException">Thrown if <see cref="WebView2.Dispose(bool)" /> has already been called on the control.</exception>
    /// <seealso cref="AvaloniaObject.VerifyAccess" />
    /// <seealso cref="WebView2" />
    [Browsable(false)]
    public CoreWebView2? CoreWebView2
    {
        get
        {
            VerifyAccess();
            VerifyNotDisposed();
            VerifyBrowserNotCrashed();
            return CoreWebView2Controller?.CoreWebView2;
        }
    }

    CoreWebView2 VerifyCoreWebView2()
    {
        var coreWebView2 = CoreWebView2;
        if (coreWebView2 == null)
            throw new InvalidOperationException("Attempted to use WebView2 functionality which requires its CoreWebView2 prior to the CoreWebView2 being initialized.  Call EnsureCoreWebView2Async or set the Source property first.");
        return coreWebView2;
    }

    /// <summary>
    /// This event is triggered either
    /// 1) when the control's <see cref="CoreWebView2" /> has finished being initialized (regardless of how initialization was triggered) but before it is used for anything, or
    /// 2) if the initialization failed.
    /// You should handle this event if you need to perform one time setup operations on the <see cref="CoreWebView2" /> which you want to affect all of its usages.
    /// (e.g. adding event handlers, configuring settings, installing document creation scripts, adding host objects).
    /// See the <see cref="WebView2" /> class documentation for an initialization overview.
    /// </summary>
    /// <remarks>
    /// This sender will be the <see cref="WebView2" /> control, whose <see cref="CoreWebView2" /> property will now be valid (i.e. non-null) for the first time
    /// if <see cref="CoreWebView2InitializationCompletedEventArgs.IsSuccess" /> is <c>true</c>.
    /// Unlikely this event can fire second time (after reporting initialization success first)
    /// if the initialization is followed by navigation which fails.
    /// </remarks>
    /// <seealso cref="WebView2" />
    public event EventHandler<CoreWebView2InitializationCompletedEventArgs>? CoreWebView2InitializationCompleted;

    /// <summary>
    /// Explicitly triggers initialization of the control's <see cref="CoreWebView2" />.
    /// See the <see cref="WebView2" /> class documentation for an initialization overview.
    /// </summary>
    /// <param name="environment">
    /// A pre-created <see cref="CoreWebView2Environment" /> that should be used to create the <see cref="CoreWebView2" />.
    /// Creating your own environment gives you control over several options that affect how the <see cref="CoreWebView2" /> is initialized.
    /// If you pass an environment to this method then it will override any settings specified on the <see cref="CreationProperties" /> property.
    /// If you pass <c>null</c> (the default value) and no value has been set to <see cref="CreationProperties" /> then a default environment will be created and used automatically.
    /// </param>
    /// <param name="controllerOptions">
    /// A pre-created <see cref="CoreWebView2ControllerOptions" /> that should be used to create the <see cref="CoreWebView2" />.
    /// Creating your own controller options gives you control over several options that affect how the <see cref="CoreWebView2" /> is initialized.
    /// If you pass a controllerOptions to this method then it will override any settings specified on the <see cref="CreationProperties" /> property.
    /// If you pass <c>null</c> (the default value) and no value has been set to <see cref="CreationProperties" /> then a default controllerOptions will be created and used automatically.
    /// </param>
    /// <returns>
    /// A Task that represents the background initialization process.
    /// When the task completes then the <see cref="CoreWebView2" /> property will be available for use (i.e. non-null).
    /// Note that the control's <see cref="CoreWebView2InitializationCompleted" /> event will be invoked before the task completes.
    /// </returns>
    /// <remarks>
    /// Unless previous initialization has already failed, calling this method additional times with the same parameter will have no effect (any specified environment is ignored) and return the same Task as the first call.
    /// Unless previous initialization has already failed, calling this method after initialization has been implicitly triggered by setting the <see cref=Source" /> property will have no effect if no environment is given
    /// and simply return a Task representing that initialization already in progress, unless previous initialization has already failed.
    /// Unless previous initialization has already failed, calling this method with a different environment after initialization has begun will result in an <see cref="ArgumentException" />. For example, this can happen if you begin initialization
    /// by setting the <see cref="Source" /> property and then call this method with a new environment, if you begin initialization with <see cref="CreationProperties" /> and then call this method with a new
    /// environment, or if you begin initialization with one environment and then call this method with no environment specified.
    /// When this method is called after previous initialization has failed, it will trigger initialization of the control's <see cref="CoreWebView2" /> again.
    /// Note that even though this method is asynchronous and returns a Task, it still must be called on the UI thread like most public functionality of most UI controls.
    /// </remarks>
    /// <exception cref="ArgumentException">
    /// Thrown if this method is called with a different environment than when it was initialized. See Remarks for more info.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the calling thread isn't the thread which created this object (usually the UI thread). See <see cref="AvaloniaObject.VerifyAccess" /> for more info.
    /// May also be thrown if <see cref="SynchronizationContext.Current" /> is null, which probably indicates that the application's event loop hasn't started yet.
    /// May also be thrown if the browser process has crashed unexpectedly and left the control in an invalid state. We are considering throwing a different type of exception for this case in the future.
    /// </exception>
    /// <exception cref="T:System.ObjectDisposedException">Thrown if <see cref="Dispose(bool)" /> has already been called on the control.</exception>
    /// <seealso cref="AvaloniaObject.VerifyAccess" />
    /// <seealso cref="WebView2" />
    public Task EnsureCoreWebView2Async(CoreWebView2Environment? environment = null, CoreWebView2ControllerOptions? controllerOptions = null)
    {
        if (IsInDesignMode)
            return Task.FromResult(0);
        VerifyAccess();
        VerifyNotDisposed();
        VerifyBrowserNotCrashed();
        if (SynchronizationContext.Current == null)
            throw new InvalidOperationException("EnsureCoreWebView2Async cannot be used before the application's event loop has started running.");
        if (_initTask == null || _initTask.IsFaulted)
        {
            _initTask = Init();
        }
        else
        {
            if ((!_isExplicitEnvironment && environment != null) || (_isExplicitEnvironment && environment != null && Environment != environment))
                throw new ArgumentException("WebView2 was already initialized with a different CoreWebView2Environment. Check to see if the Source property was already set or EnsureCoreWebView2Async was previously called with different values.");
            if ((!_isExplicitControllerOptions && controllerOptions != null) || (_isExplicitControllerOptions && controllerOptions != null && ControllerOptions != controllerOptions))
                throw new ArgumentException("WebView2 was already initialized with a different CoreWebView2ControllerOptions. Check to see if the Source property was already set or EnsureCoreWebView2Async was previously called with different values.");
        }
        return _initTask;

        async Task Init()
        {
            try
            {
                if (environment != null)
                {
                    Environment = environment;
                    _isExplicitEnvironment = true;
                }
                else if (CreationProperties != null)
                    Environment = await CreationProperties.CreateEnvironmentAsync();
                if (Environment == null)
                    Environment = await CoreWebView2Environment.CreateAsync(null, null, null);
                if (controllerOptions != null)
                {
                    ControllerOptions = controllerOptions;
                    _isExplicitControllerOptions = true;
                }
                else if (CreationProperties != null)
                    ControllerOptions = CreationProperties.CreateCoreWebView2ControllerOptions(Environment);
                if (DefaultBackgroundColor != DefaultBackgroundColorProperty.GetDefaultValue(typeof(WebView2)))
                    System.Environment.SetEnvironmentVariable("WEBVIEW2_DEFAULT_BACKGROUND_COLOR", Color.FromArgb(DefaultBackgroundColor.ToArgb()).Name);
                CoreWebView2Environment? view2Environment;
                if (ControllerOptions != null)
                {
                    view2Environment = Environment;
                    CoreWebView2Controller view2ControllerAsync = await view2Environment.CreateCoreWebView2ControllerAsync(await _hwndTaskSource.Task, ControllerOptions);
                    view2Environment = null;
                    CoreWebView2Controller = view2ControllerAsync;
                }
                else
                {
                    view2Environment = Environment;
                    CoreWebView2Controller view2ControllerAsync = await view2Environment.CreateCoreWebView2ControllerAsync(await _hwndTaskSource.Task);
                    view2Environment = null;
                    CoreWebView2Controller = view2ControllerAsync;
                }
                CoreWebView2Controller.AcceleratorKeyPressed += new EventHandler<CoreWebView2AcceleratorKeyPressedEventArgs>(CoreWebView2Controller_AcceleratorKeyPressed);
                CoreWebView2Controller.GotFocus += new EventHandler<object>(CoreWebView2Controller_GotFocus);
                CoreWebView2Controller.LostFocus += new EventHandler<object>(CoreWebView2Controller_LostFocus);
                CoreWebView2Controller.MoveFocusRequested += new EventHandler<CoreWebView2MoveFocusRequestedEventArgs>(CoreWebView2Controller_MoveFocusRequested);
                CoreWebView2Controller.ZoomFactorChanged += new EventHandler<object>(CoreWebView2Controller_ZoomFactorChanged);
                CoreWebView2!.ContentLoading += new EventHandler<CoreWebView2ContentLoadingEventArgs>(CoreWebView2_ContentLoading);
                CoreWebView2.HistoryChanged += new EventHandler<object>(CoreWebView2_HistoryChanged);
                CoreWebView2.NavigationCompleted += new EventHandler<CoreWebView2NavigationCompletedEventArgs>(CoreWebView2_NavigationCompleted);
                CoreWebView2.NavigationStarting += new EventHandler<CoreWebView2NavigationStartingEventArgs>(CoreWebView2_NavigationStarting);
                CoreWebView2.ProcessFailed += new EventHandler<CoreWebView2ProcessFailedEventArgs>(CoreWebView2_ProcessFailed);
                CoreWebView2.SourceChanged += new EventHandler<CoreWebView2SourceChangedEventArgs>(CoreWebView2_SourceChanged);
                CoreWebView2.WebMessageReceived += new EventHandler<CoreWebView2WebMessageReceivedEventArgs>(CoreWebView2_WebMessageReceived);
                //this.IsVisibleChanged += new DependencyPropertyChangedEventHandler(this.UIElement_IsVisibleChanged);
                if (CoreWebView2Controller.ParentWindow != Handle)
                    ReparentController(Handle, false);
                if (CoreWebView2Controller.ParentWindow != IntPtr.Zero)
                    SyncControllerWithParentWindow();
                bool flag = Source != null;
                if (Source == null)
                    SetCurrentValueFromCore(SourceProperty, new(CoreWebView2.Source));
                if (ZoomFactor != DefaultZoomFactor)
                    CoreWebView2Controller.ZoomFactor = ZoomFactor;
                if (DefaultBackgroundColor != DefaultBackgroundColorProperty.GetDefaultValue(typeof(WebView2)))
                    CoreWebView2Controller.DefaultBackgroundColor = DefaultBackgroundColor;
                if (AllowExternalDrop != AllowExternalDropProperty.GetDefaultValue(typeof(WebView2)))
                {
                    try
                    {
                        CoreWebView2Controller.AllowExternalDrop = AllowExternalDrop;
                    }
                    catch (NotImplementedException)
                    {
                    }
                }
                CoreWebView2InitializationCompleted?.Invoke(this, new CoreWebView2InitializationCompletedEventArgs(null));
                if (!flag)
                    return;
                CoreWebView2.Navigate(Source?.AbsoluteUri);
            }
            catch (Exception ex)
            {
                CoreWebView2InitializationCompleted?.Invoke(this, new CoreWebView2InitializationCompletedEventArgs(ex));
                throw;
            }
        }
    }

    /// <summary>
    /// This is called by our base class according to the typical implementation of the <see cref="IDisposable" /> pattern.
    /// We implement it by releasing all of our underlying COM resources, including our <see cref="CoreWebView2" />.
    /// </summary>
    /// <param name="disposing">True if a caller is explicitly calling Dispose, false if we're being finalized.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: 释放托管状态(托管对象)
                Uninitialize();
            }

            // TODO: 释放未托管的资源(未托管的对象)并重写终结器
            // TODO: 将大型字段设置为 null
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    void VerifyNotDisposed()
    {
        if (disposedValue) throw new ObjectDisposedException(Name);
    }

    void Uninitialize(bool browserCrashed = false)
    {
        _browserCrashed = browserCrashed;
        if (CoreWebView2Controller != null)
        {
            CoreWebView2Controller webView2Controller = CoreWebView2Controller;
            CoreWebView2Controller = null;
            //this.IsVisible -= new DependencyPropertyChangedEventHandler(this.UIElement_IsVisibleChanged);
            if (!_browserCrashed)
            {
                webView2Controller.CoreWebView2.ContentLoading -= new EventHandler<CoreWebView2ContentLoadingEventArgs>(CoreWebView2_ContentLoading);
                webView2Controller.CoreWebView2.HistoryChanged -= new EventHandler<object>(CoreWebView2_HistoryChanged);
                webView2Controller.CoreWebView2.NavigationCompleted -= new EventHandler<CoreWebView2NavigationCompletedEventArgs>(CoreWebView2_NavigationCompleted);
                webView2Controller.CoreWebView2.NavigationStarting -= new EventHandler<CoreWebView2NavigationStartingEventArgs>(CoreWebView2_NavigationStarting);
                webView2Controller.CoreWebView2.ProcessFailed -= new EventHandler<CoreWebView2ProcessFailedEventArgs>(CoreWebView2_ProcessFailed);
                webView2Controller.CoreWebView2.SourceChanged -= new EventHandler<CoreWebView2SourceChangedEventArgs>(CoreWebView2_SourceChanged);
                webView2Controller.CoreWebView2.WebMessageReceived -= new EventHandler<CoreWebView2WebMessageReceivedEventArgs>(CoreWebView2_WebMessageReceived);
                webView2Controller.AcceleratorKeyPressed -= new EventHandler<CoreWebView2AcceleratorKeyPressedEventArgs>(CoreWebView2Controller_AcceleratorKeyPressed);
                webView2Controller.GotFocus -= new EventHandler<object>(CoreWebView2Controller_GotFocus);
                webView2Controller.LostFocus -= new EventHandler<object>(CoreWebView2Controller_LostFocus);
                webView2Controller.MoveFocusRequested -= new EventHandler<CoreWebView2MoveFocusRequestedEventArgs>(CoreWebView2Controller_MoveFocusRequested);
                webView2Controller.ZoomFactorChanged -= new EventHandler<object>(CoreWebView2Controller_ZoomFactorChanged);
                webView2Controller.Close();
            }
        }
        Environment = null;
    }

    /// <summary>
    /// This is an event handler for our CoreWebView2's ProcessFailedEvent
    /// </summary>
    void CoreWebView2_ProcessFailed(object? sender, CoreWebView2ProcessFailedEventArgs e)
    {
        if (e.ProcessFailedKind != CoreWebView2ProcessFailedKind.BrowserProcessExited)
            return;
        Uninitialize(true);
    }

    void VerifyBrowserNotCrashed()
    {
        if (_browserCrashed)
            throw new InvalidOperationException("The WebView control is no longer valid because the browser process crashed.To work around this, please listen for the CoreWebView2.ProcessFailed event to explicitly manage the lifetime of the WebView2 control in the event of a browser failure.https://docs.microsoft.com/en-us/dotnet/api/microsoft.web.webview2.core.corewebview2.processfailed");
    }

    /// <summary>
    /// Implementation of the ISupportInitialize pattern.
    /// Prevents the control from implicitly initializing its <see cref="CoreWebView2" /> until <see cref="EndInit" /> is called.
    /// Does *not* prevent explicit initialization of the CoreWebView2 (i.e. <see cref="EnsureCoreWebView2Async(CoreWebView2Environment,CoreWebView2ControllerOptions)" />).
    /// Mainly intended for use by interactive UI designers.
    /// </summary>
    /// <remarks>
    /// Note that the "Initialize" in ISupportInitialize and the "Init" in BeginInit/EndInit mean
    /// something different and more general than this control's specific concept of initializing
    /// its CoreWebView2 (explicitly or implicitly).  This ISupportInitialize pattern is a general
    /// way to set batches of properties on the control to their initial values without triggering
    /// any dependent side effects until all of the values are set (i.e. until EndInit is called).
    /// In the case of this control, a specific side effect to be avoided is triggering implicit
    /// initialization of the CoreWebView2 when setting the Source property.
    /// For example, normally if you set <see cref="CreationProperties" /> after you've already set Source,
    /// the data set to CreationProperties is ignored because implicit initialization has already started.
    /// However, if you set the two properties (in the same order) in between calls to BeginInit and
    /// EndInit then the implicit initialization of the CoreWebView2 is delayed until EndInit, so the data
    /// set to CreationProperties is still used even though it was set after Source.
    /// </remarks>
    public override void BeginInit()
    {
        base.BeginInit();
        _implicitInitGate.BeginInit();
    }

    /// <summary>
    /// Implementation of the ISupportInitialize pattern.
    /// Invokes any functionality that has been delayed since the corresponding call to <see cref="BeginInit" />.
    /// Mainly intended for use by interactive UI designers.
    /// </summary>
    /// <remarks>
    /// See the documentation of <see cref="BeginInit" /> for more information.
    /// </remarks>
    public override void EndInit()
    {
        _implicitInitGate.EndInit();
        base.EndInit();
    }

    /// <summary>
    /// Sets the value of a dependency property without changing its value source.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="property">The identifier of the dependency property to set.</param>
    /// <param name="value">The new local value.</param>
    public void SetCurrentValue<T>(AvaloniaProperty<T> property, T value)
    {
        if (property == SourceProperty)
        {
            _Source = value is Uri value2 ? value2 : null;
        }
        else if (property == ZoomFactorProperty)
        {
            _ZoomFactor = ConvertibleHelper.Convert<double, T>(value);
        }
    }

    /// <summary>
    /// Updates one of our dependency properties to match a new value from the <see cref="CoreWebView2" />.
    /// It both sets the value and remembers (in _propertyChangingFromCore) that it came from the CoreWebView2 rather than the caller,
    /// allowing the property's "on changed" handler to alter its behavior based on where the new value came from.
    /// It's only intended to be called in a CoreWebView2 event handler that's informing us of a new property value.
    /// It's basically just a wrapper around the inherited SetCurrentValue which also maintains _propertyChangingFromCore.
    /// See the comments on <see cref="_propertyChangingFromCore" /> for additional background info.
    /// One more thing worth explicitly stating is that it wraps SetCurrentValue rather than SetValue,
    /// in order to avoid overwriting any OneWay bindings that are set on the specified properties.
    /// Check the link https://stackoverflow.com/q/4230698 for more information about the difference between SetValue and SetCurrentValue.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="property">The property to change due to an equivalent change in the CoreWebView2.</param>
    /// <param name="value">The new value from the CoreWebView2.</param>
    void SetCurrentValueFromCore<T>(AvaloniaProperty<T> property, T value)
    {
        Trace.Assert(_propertyChangingFromCore == null);
        _propertyChangingFromCore = property;
        SetCurrentValue(property, value);
        _propertyChangingFromCore = null;
    }

    /// <summary>
    /// Checks if a given property is currently being updated to match an equivalent change in the <see cref="CoreWebView2" />.
    /// This method should only be called from a property's "on changed" handler; it has no meaning at any other time.
    /// It is used to determine if the property is changing to match the CoreWebView2 or because the caller set it.
    /// Usually this is used in order to decide if the new value needs to be propagated down to the CoreWebView2.
    /// See the comments on <see cref="_propertyChangingFromCore" /> for additional background info.
    /// </summary>
    /// <param name="property">The property to check.</param>
    /// <returns>True if the property is changing to match the CoreWebView2, or false if the property was changed by the caller.</returns>
    bool IsPropertyChangingFromCore(AvaloniaProperty property)
    {
        if (property == null)
            throw new ArgumentNullException(nameof(property));
        return property == _propertyChangingFromCore;
    }

    /// <summary>
    /// Changes our controller's ParentWindow to the given HWND, along with any other necessary associated work.
    /// </summary>
    /// <param name="hwnd">The new HWND to set as the controller's parent.  IntPtr.Zero means that the controller will have no parent and the CoreWebView2 will be hidden.</param>
    /// <param name="sync">Whether or not to call <see cref="SyncControllerWithParentWindow" /> as required.  Defaults to true.  If you pass false then you should call it yourself if required.</param>
    /// <remarks>
    /// Reparenting the controller isn't necessarily as simple as changing its ParentWindow property,
    /// and this method exists to ensure that any other work that needs to be done at the same time gets done.
    /// The reason that SyncControllerWithParentWindow isn't baked directly into this method is because
    /// sometimes we want to call the Sync functionality without necessarily reparenting (e.g. during initialization).
    /// </remarks>
    void ReparentController(IntPtr hwnd, bool sync = true)
    {
        if (hwnd == IntPtr.Zero)
        {
            if (CoreWebView2Controller != null)
            {
                CoreWebView2Controller.IsVisible = false;
                CoreWebView2Controller.ParentWindow = IntPtr.Zero;
            }
        }
        else
        {
            if (CoreWebView2Controller != null)
            {
                CoreWebView2Controller.ParentWindow = hwnd;
            }
            if (!sync)
                return;
            SyncControllerWithParentWindow();
        }
    }

    /// <summary>
    /// Updates the child window's size, visibility, and position to reflect the current state of the element.
    /// </summary>
    public virtual void UpdateWindowPos()
    {
        if (disposedValue) return;
        HandleRef handle = new(null, PlatformHandle!.Handle);
        NativeMethods.ShowWindowAsync(handle, 5);
    }

    /// <summary>
    /// Syncs visual/windowing information between the controller and its parent HWND.
    /// This should be called any time a new, non-null HWND is set as the controller's parent,
    /// including when the controller is first created.
    /// </summary>
    void SyncControllerWithParentWindow()
    {
        UpdateWindowPos();
        if (CoreWebView2Controller != null)
        {
            if (KeyboardDevice.Instance?.FocusedElement == this)
                CoreWebView2Controller.MoveFocus(CoreWebView2MoveFocusReason.Programmatic);
            CoreWebView2Controller.IsVisible = IsVisible;
        }
    }

    /// <summary>
    /// This is a handler for our base UIElement's IsVisibleChanged event.
    /// It's predictably fired whenever IsVisible changes, and IsVisible reflects the actual current visibility status of the control.
    /// We just need to pass this info through to our CoreWebView2Controller so it can save some effort when the control isn't visible.
    /// </summary>
    static void IsVisibleChanged(AvaloniaPropertyChangedEventArgs<bool> e)
    {
        if (e.Sender is WebView2 webView)
        {
            if (webView.CoreWebView2Controller == null)
                return;
            webView.CoreWebView2Controller.IsVisible = e.NewValue.HasValue && e.NewValue.Value;
        }
    }

    /// <summary>
    /// This is overridden from <see cref="T:System.Windows.Interop.HwndHost" /> and called when our control's location has changed.
    /// The HwndHost takes care of updating the HWND we created.
    /// What we need to do is move our CoreWebView2 to match the new location.
    /// </summary>
    protected void OnWindowPositionChanged(Rect rcBoundingBox)
    {
        if (CoreWebView2Controller != null)
        {
            CoreWebView2Controller webView2Controller = CoreWebView2Controller;
            Size size = rcBoundingBox.Size;
            int int32_1 = Convert.ToInt32(size.Width);
            size = rcBoundingBox.Size;
            int int32_2 = Convert.ToInt32(size.Height);
            Rectangle rectangle = new Rectangle(0, 0, int32_1, int32_2);
            webView2Controller.Bounds = rectangle;
            webView2Controller.NotifyParentWindowPositionChanged();
        }
    }

    Uri? _Source;

    /// <summary>
    /// The top-level <see cref="Uri" /> which the WebView is currently displaying (or will display once initialization of its <see cref="CoreWebView2" /> is finished).
    /// Generally speaking, getting this property is equivalent to getting the <see cref="CoreWebView2.Source" /> property and setting this property (to a different value) is equivalent to calling the <see cref="CoreWebView2.Navigate(string)" /> method.
    /// </summary>
    /// <remarks>
    /// Getting this property before the <see cref="CoreWebView2" /> has been initialized will retrieve the last Uri which was set to it, or null (the default) if none has been.
    /// Setting this property before the <see cref="CoreWebView2" /> has been initialized will cause initialization to start in the background (if not already in progress), after which the <see cref="WebView2" /> will navigate to the specified <see cref="Uri" />.
    /// This property can never be set back to null or to a relative <see cref="Uri" />.
    /// See the <see cref="WebView2" /> class documentation for an initialization overview.
    /// </remarks>
    /// <exception cref="ObjectDisposedException">Thrown if <see cref="Dispose(bool)" /> has already been called on the control.</exception>
    /// <exception cref="NotImplementedException">Thrown if the property is set to <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown if the property is set to a relative <see cref="Uri" /> (i.e. a <see cref="Uri" /> whose <see cref="Uri.IsAbsoluteUri" /> property is <c>false</c>).</exception>
    /// <seealso cref="WebView2" />
    [Category("Common")]
    public Uri? Source
    {
        get => _Source;
        set
        {
            if (SourcePropertyValid(value))
                SetAndRaise(SourceProperty, ref _Source, value);
        }
    }

    /// <summary>
    /// This is a callback that Avalonia calls to validate a potential new Source value.
    /// </summary>
    /// <returns>
    /// True if the value is valid, false if it is not.
    /// If we return false then Avalonia should respond by throwing an <see cref="ArgumentException" />.
    /// </returns>
    /// <remarks>
    /// Note that we unfortunately can't treat null as invalid here because null is valid prior to initialization.
    /// </remarks>
    static bool SourcePropertyValid(Uri? uri)
    {
        return uri == null || uri.IsAbsoluteUri;
    }

    /// <summary>
    /// This is a callback that Avalonia calls when the Avalonia Source property's value changes.
    /// This might have been triggered by either:
    /// 1) The caller set Source to programmatically trigger a navigation.
    /// 2) The CoreWebView changed its own source and we're just updating the dependency property to match.
    /// We use <see cref="IsPropertyChangingFromCore(AvaloniaProperty)" /> to distinguish the two cases.
    /// </summary>
    static void SourcePropertyChanged(AvaloniaPropertyChangedEventArgs<Uri?> e)
    {
        if (e.Sender is WebView2 control)
        {
            if (control.IsPropertyChangingFromCore(SourceProperty))
                return;
            if (!e.NewValue.HasValue)
                throw new NotImplementedException("The Source property cannot be set to null.");
            if (control.CoreWebView2 != null && (!e.OldValue.HasValue || e.OldValue.Value!.AbsoluteUri != e.NewValue.Value!.AbsoluteUri))
                control.CoreWebView2.Navigate(e.NewValue.Value!.AbsoluteUri);
            control._implicitInitGate.RunWhenOpen(() => control.EnsureCoreWebView2Async());
        }
    }

    /// <summary>
    /// A wrapper around the <see cref="CoreWebView2.SourceChanged" />.
    /// The only difference between this event and <see cref="CoreWebView2.SourceChanged" /> is the first parameter that's passed to handlers.
    /// Handlers of this event will receive the <see cref="WebView2" /> control, whereas handlers of <see cref="CoreWebView2.SourceChanged" /> will receive the <see cref="T:Microsoft.Web.WebView2.Core.CoreWebView2" /> instance.
    /// </summary>
    /// <see cref="CoreWebView2.SourceChanged" />
    public event EventHandler<CoreWebView2SourceChangedEventArgs>? SourceChanged;

    /// <summary>
    /// This is an event handler for our CoreWebView2's SourceChanged event.
    /// Unsurprisingly, it fires when the CoreWebView2's source URI has been changed.
    /// Note that there are two distinct triggers for this:
    /// 1) The CoreWebView2 was told to navigate programmatically (potentially by us, see SourcePropertyChanged).
    /// 2) The user interacted with the CoreWebView2, e.g. clicked a link.
    /// In either of the above cases, this event might trigger several times due to e.g. redirection.
    /// Aside from propagating to our own event, we just need to update our WPF Source property to match the CoreWebView2's.
    /// </summary>
    void CoreWebView2_SourceChanged(object? sender, CoreWebView2SourceChangedEventArgs e)
    {
        var source = CoreWebView2?.Source;
        if (!string.IsNullOrWhiteSpace(source))
        {
            SetCurrentValueFromCore(SourceProperty, new(source));
            var sourceChanged = SourceChanged;
            if (sourceChanged == null)
                return;
            sourceChanged(this, e);
        }
    }

    /// <summary>
    /// A wrapper around the <see cref="CoreWebView2.NavigationStarting" />.
    /// The only difference between this event and <see cref="CoreWebView2.NavigationStarting" /> is the first parameter that's passed to handlers.
    /// Handlers of this event will receive the <see cref="WebView2" /> control, whereas handlers of <see cref="CoreWebView2.NavigationStarting" /> will receive the <see cref="T:Microsoft.Web.WebView2.Core.CoreWebView2" /> instance.
    /// </summary>
    /// <seealso cref="CoreWebView2.NavigationStarting" />
    public event EventHandler<CoreWebView2NavigationStartingEventArgs>? NavigationStarting;

    /// <summary>
    /// This is an event handler for our CoreWebView2's NavigationStarting event.
    /// We just need to propagate the event to WPF.
    /// </summary>
    void CoreWebView2_NavigationStarting(object? sender, CoreWebView2NavigationStartingEventArgs e)
    {
        var navigationStarting = NavigationStarting;
        if (navigationStarting == null)
            return;
        navigationStarting(this, e);
    }

    /// <summary>
    /// A wrapper around the <see cref="CoreWebView2.NavigationCompleted" />.
    /// The only difference between this event and <see cref="CoreWebView2.NavigationCompleted" /> is the first parameter that's passed to handlers.
    /// Handlers of this event will receive the <see cref="WebView2" /> control, whereas handlers of <see cref="CoreWebView2.NavigationCompleted" /> will receive the <see cref="T:Microsoft.Web.WebView2.Core.CoreWebView2" /> instance.
    /// </summary>
    /// <seealso cref="CoreWebView2.NavigationCompleted" />
    public event EventHandler<CoreWebView2NavigationCompletedEventArgs>? NavigationCompleted;

    /// <summary>
    /// This is an event handler for our CoreWebView2's NavigationCompleted event.
    /// We just need to propagate the event to WPF.
    /// </summary>
    void CoreWebView2_NavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
    {
        var navigationCompleted = NavigationCompleted;
        if (navigationCompleted == null)
            return;
        navigationCompleted(this, e);
    }

    /// <summary>
    /// This is an event handler for our CoreWebView2's HistoryChanged event.
    /// We're handling it in order to update our WPF CanGoBack and CanGoForward properties.
    /// </summary>
    void CoreWebView2_HistoryChanged(object? sender, object e)
    {
        var coreWebView2 = CoreWebView2;
        if (coreWebView2 != null)
        {
            CanGoBack = coreWebView2.CanGoBack;
            CanGoForward = coreWebView2.CanGoForward;
        }
    }

    bool _CanGoBack;

    /// <summary>
    /// Returns <c>true</c> if the WebView can navigate to a previous page in the navigation history.
    /// Wrapper around the <see cref="CoreWebView2.CanGoBack" /> property of <see cref="CoreWebView2" />.
    /// If <see cref="CoreWebView2" /> isn't initialized yet then returns <c>false</c>.
    /// </summary>
    /// <seealso cref="CoreWebView2.CanGoBack" />
    [Browsable(false)]
    public bool CanGoBack
    {
        get => _CanGoBack;
        private set => SetAndRaise(CanGoBackProperty, ref _CanGoBack, value);
    }

    bool _CanGoForward;

    /// <summary>
    /// Returns <c>true</c> if the WebView can navigate to a next page in the navigation history.
    /// Wrapper around the <see cref="CoreWebView2.CanGoForward" /> property of <see cref="CoreWebView2" />.
    /// If <see cref="CoreWebView2" /> isn't initialized yet then returns <c>false</c>.
    /// </summary>
    /// <seealso cref="CoreWebView2.CanGoForward" />
    [Browsable(false)]
    public bool CanGoForward
    {
        get => _CanGoForward;
        private set => SetAndRaise(CanGoForwardProperty, ref _CanGoForward, value);
    }

    /// <summary>
    /// This is overridden from <see cref="T:System.Windows.Interop.IHwndHost" /> and is called to inform us that tabbing has caused the focus to move into our control/window.
    /// Since WPF can't manage the transition of focus to a non-WPF HWND, it delegates the transition to us here.
    /// So our job is just to place the focus in our external HWND.
    /// </summary>
    /// <param name="focusNavigationDirection">Information about how the focus is moving.</param>
    /// <returns><c>true</c> to indicate that we handled the navigation, or <c>false</c> to indicate that we didn't.</returns>
    protected bool TabIntoCore(string focusNavigationDirection)
    {
        if (CoreWebView2 != null)
        {
            switch (focusNavigationDirection)
            {
                case "Next":
                case "First":
                    CoreWebView2Controller?.MoveFocus(CoreWebView2MoveFocusReason.Next);
                    return true;
                case "Previous":
                case "Last":
                    CoreWebView2Controller?.MoveFocus(CoreWebView2MoveFocusReason.Previous);
                    return true;
            }
        }
        return default;
    }

    /// <summary>
    /// This is overridden from <see cref="Control" /> and is called to inform us when we receive the keyboard focus.
    /// We handle this by passing the keyboard focus on to the underlying <see cref="T:Microsoft.Web.WebView2.Core.CoreWebView2" />.
    /// We never want to land in a state where our window (this.Handle) actually has the keyboard focus.
    /// </summary>
    /// <param name="e">Arguments from the underlying GotKeyboardFocus event.</param>
    /// <remarks>
    /// Note that it's actually possible for us to receive keyboard focus without this method being called.
    /// One known case where that happens is when our parent window is deactivated while we have focus, then reactivated.
    /// We handle that case in <see cref="WndProc(IntPtr, int, IntPtr, IntPtr, ref bool)" />.
    /// </remarks>
    /// <seealso cref="WndProc(IntPtr, int, IntPtr, IntPtr, ref bool)" />
    protected void OnGotKeyboardFocus()
    {
        CoreWebView2Controller?.MoveFocus(CoreWebView2MoveFocusReason.Programmatic);
    }

    /// <summary>
    /// Moves the keyboard focus away from this element and to another element in a provided traversal direction.
    /// </summary>
    /// <param name="focusNavigationDirection">The direction that focus is to be moved, as a value of the enumeration.</param>
    public virtual void MoveFocus(string focusNavigationDirection)
    {

    }

    /// <summary>
    /// This is an event handler for our CoreWebView2Controller's MoveFocusRequested event.
    /// It fires when the CoreWebView2Controller has focus but wants to move it elsewhere in the app.
    /// E.g. this happens when the user tabs past the last item in the CoreWebView2 and focus needs to return to some other app control.
    /// So our job is just to tell WPF to move the focus on to the next control.
    /// Note that we don't propagate this event outward as a standard WPF routed event because we've implemented its purpose here.
    /// If users of the control want to track focus shifting in/out of the control, they should use standard WPF events.
    /// </summary>
    void CoreWebView2Controller_MoveFocusRequested(object? sender, CoreWebView2MoveFocusRequestedEventArgs e)
    {
        switch (e.Reason)
        {
            case CoreWebView2MoveFocusReason.Programmatic:
            case CoreWebView2MoveFocusReason.Next:
                MoveFocus("Next");
                break;
            case CoreWebView2MoveFocusReason.Previous:
                MoveFocus("Previous");
                break;
        }
        e.Handled = true;
    }

    /// <summary>
    /// This is an event handler for our CoreWebView2Controller's GotFocus event.
    /// We just need to propagate the event to WPF.
    /// </summary>
    void CoreWebView2Controller_GotFocus(object? sender, object e)
        => RaiseEvent(new RoutedEventArgs(GotFocusEvent));

    /// <summary>
    /// This is an event handler for our CoreWebView2Controller's LostFocus event.
    /// We just need to propagate the event to WPF.
    /// </summary>
    void CoreWebView2Controller_LostFocus(object? sender, object e)
        => RaiseEvent(new RoutedEventArgs(LostFocusEvent));

    protected virtual RoutedEvent? PreviewKeyDownEvent => KeyDownEvent;

    protected virtual RoutedEvent? PreviewKeyUpEvent => KeyUpEvent;

    /// <summary>
    /// This is an event handler for our CoreWebView2Controller's AcceleratorKeyPressed event.
    /// This is called to inform us about key presses that are likely to have special behavior (e.g. esc, return, Function keys, letters with modifier keys).
    /// WPF can't detect this input because Windows sends it directly to the Win32 CoreWebView2Controller control.
    /// We implement this by generating standard WPF key input events, allowing callers to handle the input in the usual WPF way if they want.
    /// If nobody handles the WPF key events then we'll allow the default CoreWebView2Controller logic (if any) to handle it.
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

    const double DefaultZoomFactor = 1.0D;

    double _ZoomFactor = DefaultZoomFactor;

    /// <summary>
    /// The zoom factor for the WebView.
    /// This property directly exposes <see cref="CoreWebView2Controller.ZoomFactor" />, see its documentation for more info.
    /// Getting this property before the <see cref="T:Microsoft.Web.WebView2.Core.CoreWebView2" /> has been initialized will retrieve the last value which was set to it, or <c>1.0</c> (the default) if none has been.
    /// The most recent value set to this property before the CoreWebView2 has been initialized will be set on it after initialization.
    /// </summary>
    /// <seealso cref="CoreWebView2Controller.ZoomFactor" />
    [Category("Common")]
    public double ZoomFactor
    {
        get => _ZoomFactor;
        set => SetAndRaise(ZoomFactorProperty, ref _ZoomFactor, value);
    }

    /// <summary>
    /// This is a callback that WPF calls when our WPF ZoomFactor property's value changes.
    /// This might have been triggered by either:
    /// 1) The caller set ZoomFactor to change the zoom of the CoreWebView2.
    /// 2) The CoreWebView2 changed its own ZoomFactor and we're just updating the dependency property to match.
    /// We use <see cref="IsPropertyChangingFromCore(AvaloniaProperty)" /> to distinguish the two cases.
    /// </summary>
    static void ZoomFactorPropertyChanged(AvaloniaPropertyChangedEventArgs<double> e)
    {
        if (e.Sender is WebView2 webView2)
        {
            if (!e.NewValue.HasValue || webView2.CoreWebView2 == null || webView2.CoreWebView2Controller == null || webView2.IsPropertyChangingFromCore(ZoomFactorProperty))
                return;
            webView2.CoreWebView2Controller.ZoomFactor = e.NewValue.Value;
        }
    }

    /// <summary>
    /// The event is raised when the <see cref="ZoomFactor" /> property changes.
    /// This event directly exposes <see cref="CoreWebView2Controller.ZoomFactorChanged" />.
    /// </summary>
    /// <seealso cref="ZoomFactor" />
    /// <seealso cref="CoreWebView2Controller.ZoomFactorChanged" />
    public event EventHandler<EventArgs>? ZoomFactorChanged;

    /// <summary>
    /// This is an event handler for our CoreWebView2Controller's ZoomFactorChanged event.
    /// Unsurprisingly, it fires when the CoreWebView2Controller's ZoomFactor has been changed.
    /// Note that there are two distinct triggers for this:
    /// 1) The value was changed programmatically (potentially by us, see ZoomFactorPropertyChanged).
    /// 2) The user interacted with the CoreWebView2, e.g. CTRL + Mouse Wheel.
    /// Aside from propagating to our own event, we just need to update our WPF ZoomFactor property to match the CoreWebView2Controller's.
    /// </summary>
    void CoreWebView2Controller_ZoomFactorChanged(object? sender, object e)
    {
        if (CoreWebView2Controller == null) return;
        SetCurrentValueFromCore(ZoomFactorProperty, CoreWebView2Controller.ZoomFactor);
        var zoomFactorChanged = ZoomFactorChanged;
        if (zoomFactorChanged == null)
            return;
        zoomFactorChanged(this, EventArgs.Empty);
    }

    /// <summary>
    /// The default background color for the WebView.
    /// This property directly exposes <see cref="CoreWebView2Controller.DefaultBackgroundColor" />, see its documentation for more info.
    /// Getting this property before the <see cref="T:Microsoft.Web.WebView2.Core.CoreWebView2Controller" /> has been initialized will retrieve the last value which was
    /// set to it, or <c>Color.White</c> (the default) if none has been.
    /// The most recent value set to this property before CoreWebView2Controller has been initialized will be set on it after initialization.
    /// </summary>
    [Category("Common")]
    public Color DefaultBackgroundColor
    {
        get => GetValue(DefaultBackgroundColorProperty);
        set => SetValue(DefaultBackgroundColorProperty, value);
    }

    /// <summary>
    /// This is a callback that WPF calls when our WPF DefaultBackgroundColor property's value changes.
    /// Since CoreWebView2Controller does not update this property itself, this is only triggered by the
    /// caller setting DefaultBackgroundColor.
    /// </summary>
    static void DefaultBackgroundColorPropertyChanged(AvaloniaPropertyChangedEventArgs<Color> e)
    {
        if (e.Sender is WebView2 webView2)
        {
            if (webView2.CoreWebView2Controller == null || !e.NewValue.HasValue)
                return;
            webView2.CoreWebView2Controller.DefaultBackgroundColor = e.NewValue.Value;
        }
    }

    /// <summary>
    /// The AllowExternalDrop property for the WebView.
    /// This property directly exposes <see cref="CoreWebView2Controller.AllowExternalDrop" />, see its documentation for more info.
    /// Getting this property before the <see cref="T:Microsoft.Web.WebView2.Core.CoreWebView2Controller" /> has been initialized will retrieve the last value which was
    /// set to it, or <c>true</c> (the default) if none has been.
    /// The most recent value set to this property before CoreWebView2Controller has been initialized will be set on it after initialization.
    /// </summary>
    [Category("Common")]
    public bool AllowExternalDrop
    {
        get => GetValue(AllowExternalDropProperty);
        set => SetValue(AllowExternalDropProperty, value);
    }

    /// <summary>
    /// This is a callback that WPF calls when our WPF AllowExternalDrop property's value changes.
    /// Since CoreWebView2Controller does not update this property itself, this is only triggered by the
    /// caller setting AllowExternalDrop.
    /// </summary>
    static void AllowExternalDropPropertyChanged(AvaloniaPropertyChangedEventArgs<bool> e)
    {
        if (e.Sender is WebView2 webView2)
        {
            if (webView2.CoreWebView2Controller == null || !e.NewValue.HasValue)
                return;
            try
            {
                webView2.CoreWebView2Controller.AllowExternalDrop = e.NewValue.Value;
            }
            catch (NotImplementedException)
            {
            }
        }
    }

    /// <summary>The foreground color to be used in design mode.</summary>
    [Category("Common")]
    public Color DesignModeForegroundColor
    {
        get => GetValue(DesignModeForegroundColorProperty);
        set => SetValue(DesignModeForegroundColorProperty, (object)value);
    }

    /// <summary>
    /// Navigates the WebView to the previous page in the navigation history.
    /// Equivalent to calling <see cref="CoreWebView2.GoBack" />
    /// If <see cref="T:Microsoft.Web.WebView2.Core.CoreWebView2" /> hasn't been initialized yet then does nothing.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the calling thread isn't the thread which created this object (usually the UI thread). See <see cref="AvaloniaObject.VerifyAccess" /> for more info.
    /// May also be thrown if the browser process has crashed unexpectedly and left the control in an invalid state. We are considering throwing a different type of exception for this case in the future.
    /// </exception>
    /// <exception cref="ObjectDisposedException">Thrown if <see cref="Dispose(bool)" /> has already been called on the control.</exception>
    /// <seealso cref="AvaloniaObject.VerifyAccess" />
    /// <seealso cref="CoreWebView2.CanGoBack" />
    public void GoBack()
    {
        if (CoreWebView2 == null)
            return;
        CoreWebView2.GoBack();
    }

    /// <summary>
    /// Navigates the WebView to the next page in the navigation history.
    /// Equivalent to calling <see cref="CoreWebView2.GoForward" />.
    /// If <see cref="T:Microsoft.Web.WebView2.Core.CoreWebView2" /> hasn't been initialized yet then does nothing.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the calling thread isn't the thread which created this object (usually the UI thread). See <see cref="AvaloniaObject.VerifyAccess" /> for more info.
    /// May also be thrown if the browser process has crashed unexpectedly and left the control in an invalid state. We are considering throwing a different type of exception for this case in the future.
    /// </exception>
    /// <exception cref="ObjectDisposedException">Thrown if <see cref="Dispose(bool)" /> has already been called on the control.</exception>
    /// <seealso cref="AvaloniaObject.VerifyAccess" />
    /// <seealso cref="CoreWebView2.GoForward" />
    public void GoForward()
    {
        if (CoreWebView2 == null)
            return;
        CoreWebView2.GoForward();
    }

    /// <summary>
    /// Reloads the current page.
    /// Equivalent to calling <see cref="CoreWebView2.Reload" />.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if <see cref="CoreWebView2" /> hasn't been initialized yet, or if the calling thread isn't the thread which created this object (usually the UI thread). See <see cref="AvaloniaObject.VerifyAccess" /> for more info.
    /// May also be thrown if the browser process has crashed unexpectedly and left the control in an invalid state. We are considering throwing a different type of exception for this case in the future.
    /// </exception>
    /// <exception cref="ObjectDisposedException">Thrown if <see cref="Dispose(bool)" /> has already been called on the control.</exception>
    /// <seealso cref="AvaloniaObject.VerifyAccess" />
    /// <seealso cref="CoreWebView2.Reload" />
    public void Reload()
    {
        VerifyCoreWebView2().Reload();
    }

    /// <summary>
    /// Stops all navigations and pending resource fetches.
    /// Equivalent to calling <see cref="CoreWebView2.Stop" />.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if <see cref="CoreWebView2" /> hasn't been initialized yet, or if the calling thread isn't the thread which created this object (usually the UI thread). See <see cref="AvaloniaObject.VerifyAccess" /> for more info.
    /// May also be thrown if the browser process has crashed unexpectedly and left the control in an invalid state. We are considering throwing a different type of exception for this case in the future.
    /// </exception>
    /// <exception cref="ObjectDisposedException">Thrown if <see cref=Dispose(bool)" /> has already been called on the control.</exception>
    /// <seealso cref="AvaloniaObject.VerifyAccess" />
    /// <seealso cref="CoreWebView2.Stop" />
    public void Stop()
    {
        VerifyCoreWebView2().Stop();
    }

    /// <summary>
    /// Initiates a navigation to htmlContent as source HTML of a new document.
    /// Equivalent to calling <see cref="CoreWebView2.NavigateToString(string)" />.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if <see cref="CoreWebView2" /> hasn't been initialized yet, or if the calling thread isn't the thread which created this object (usually the UI thread). See <see cref="AvaloniaObject.VerifyAccess" /> for more info.
    /// May also be thrown if the browser process has crashed unexpectedly and left the control in an invalid state. We are considering throwing a different type of exception for this case in the future.
    /// </exception>
    /// <exception cref="ObjectDisposedException">Thrown if <see cref="Dispose(bool)" /> has already been called on the control.</exception>
    /// <remarks>The <c>htmlContent</c> parameter may not be larger than 2 MB (2 * 1024 * 1024 bytes) in total size. The origin of the new page is <c>about:blank</c>.</remarks>
    /// <seealso cref="AvaloniaObject.VerifyAccess" />
    /// <seealso cref="CoreWebView2.NavigateToString(string)" />
    public void NavigateToString(string htmlContent)
    {
        VerifyCoreWebView2().NavigateToString(htmlContent);
    }

    /// <summary>
    /// A wrapper around the <see cref="CoreWebView2.ContentLoading" />.
    /// The only difference between this event and <see cref="CoreWebView2.ContentLoading" /> is the first parameter that's passed to handlers.
    /// Handlers of this event will receive the <see cref="WebView2" /> control, whereas handlers of <see cref="CoreWebView2.ContentLoading" /> will receive the <see cref="T:Microsoft.Web.WebView2.Core.CoreWebView2" /> instance.
    /// </summary>
    /// <seealso cref="CoreWebView2.ContentLoading" />
    public event EventHandler<CoreWebView2ContentLoadingEventArgs>? ContentLoading;

    /// <summary>
    /// This is an event handler for our CoreWebView2's ContentLoading event.
    /// We just need to propagate the event to WPF.
    /// </summary>
    void CoreWebView2_ContentLoading(object? sender, CoreWebView2ContentLoadingEventArgs e)
    {
        var contentLoading = ContentLoading;
        if (contentLoading == null)
            return;
        contentLoading(this, e);
    }

    /// <summary>
    /// Executes JavaScript code from the javaScript parameter in the current top level document rendered in the WebView.
    /// Equivalent to calling <see cref="CoreWebView2.ExecuteScriptAsync(string)" />.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if <see cref="CoreWebView2" /> hasn't been initialized yet, or if the calling thread isn't the thread which created this object (usually the UI thread). See <see cref="AvaloniaObject.VerifyAccess" /> for more info.
    /// May also be thrown if the browser process has crashed unexpectedly and left the control in an invalid state. We are considering throwing a different type of exception for this case in the future.
    /// </exception>
    /// <exception cref="ObjectDisposedException">Thrown if <see cref="Dispose(bool)" /> has already been called on the control.</exception>
    /// <seealso cref="AvaloniaObject.VerifyAccess" />
    /// <seealso cref="CoreWebView2.ExecuteScriptAsync(string)" />
    public Task<string> ExecuteScriptAsync(string javaScript)
    {
        return VerifyCoreWebView2().ExecuteScriptAsync(javaScript);
    }

    /// <summary>
    /// A wrapper around the <see cref="CoreWebView2.WebMessageReceived" />.
    /// The only difference between this event and <see cref="CoreWebView2.WebMessageReceived" /> is the first parameter that's passed to handlers.
    /// Handlers of this event will receive the <see cref="WebView2" /> control, whereas handlers of <see cref="CoreWebView2.WebMessageReceived" /> will receive the <see cref="T:Microsoft.Web.WebView2.Core.CoreWebView2" /> instance.
    /// </summary>
    /// <seealso cref="CoreWebView2.WebMessageReceived" />
    public event EventHandler<CoreWebView2WebMessageReceivedEventArgs>? WebMessageReceived;

    /// <summary>
    /// This is an event handler for our CoreWebView2's WebMessageReceived event.
    /// We just need to propagate the event to WPF.
    /// </summary>
    void CoreWebView2_WebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
    {
        var webMessageReceived = WebMessageReceived;
        if (webMessageReceived == null)
            return;
        webMessageReceived(this, e);
    }

    /// <summary>
    /// True when we're in design mode and shouldn't create an underlying CoreWebView2.
    /// </summary>
    protected static bool IsInDesignMode => Design.IsDesignMode;

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new IBrush? OpacityMask => base.OpacityMask;

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new double Opacity => base.Opacity;

    //[Browsable(false)]
    //[EditorBrowsable(EditorBrowsableState.Never)]
    //public new Effect Effect => base.Effect;

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new ContextMenu? ContextMenu => base.ContextMenu;

    //[Browsable(false)]
    //[EditorBrowsable(EditorBrowsableState.Never)]
    //public new Style FocusVisualStyle => base.FocusVisualStyle;

    //[Browsable(false)]
    //[EditorBrowsable(EditorBrowsableState.Never)]
    //public new InputScope InputScope => base.InputScope;

    static class NativeMethods
    {
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr BeginPaint(IntPtr hwnd, out PaintStruct lpPaint);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool EndPaint(IntPtr hwnd, ref PaintStruct lpPaint);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr CreateWindowExW(WS_EX dwExStyle, [MarshalAs(UnmanagedType.LPWStr)] string lpClassName, [MarshalAs(UnmanagedType.LPWStr)] string lpWindowName, WS dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DestroyWindow(IntPtr hwnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool ShowWindowAsync(HandleRef hWnd, int nCmdShow);

        [Flags]
        public enum WS : uint
        {
            None = 0,
            CLIPCHILDREN = 33554432, // 0x02000000
            VISIBLE = 268435456, // 0x10000000
            CHILD = 1073741824, // 0x40000000
        }

        [Flags]
        public enum WS_EX : uint
        {
            None = 0,
            TRANSPARENT = 32, // 0x00000020
        }

        public enum WM : uint
        {
            SETFOCUS = 7,
            PAINT = 15, // 0x0000000F

            WINDOWPOSCHANGING = 0x0046,
            GETOBJECT = 0x003D,
        }

        public struct Rect
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        public struct PaintStruct
        {
            public IntPtr hdc;
            public bool fErase;
            public Rect rcPaint;
            public bool fRestore;
            public bool fIncUpdate;
            public byte[] rgbReserved;
        }
    }
}
