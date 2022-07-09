namespace Avalonia.Controls;

/// <summary>
/// This class is a bundle of the most common parameters used to create <see cref="CoreWebView2Environment" /> and <see cref="CoreWebView2Controller" /> instances.
/// Its main purpose is to be set to <see cref="WebView2.CreationProperties" /> in order to customize the environment and/or controller used by a <see cref="WebView2" /> during implicit initialization.
/// It is also a nice Avalonia integration utility which allows commonly used environment/controller parameters to be dependency properties and be created and used in markup.
/// </summary>
/// <remarks>
/// This class isn't intended to contain all possible environment or controller customization options.
/// If you need complete control over the environment and/or controller used by a WebView2 control then you'll need to initialize the control explicitly by
/// creating your own environment (with <see cref="CoreWebView2Environment.CreateAsync(string,string,CoreWebView2EnvironmentOptions)" />) and/or controller options (with <see cref="CoreWebView2Environment.CreateCoreWebView2ControllerOptions" />) and passing them to <see cref="WebView2.EnsureCoreWebView2Async(CoreWebView2Environment,CoreWebView2ControllerOptions)" />
/// *before* you set the <see cref="WebView2.Source" /> property to anything.
/// See the <see cref="WebView2" /> class documentation for an initialization overview.
/// </remarks>
public class CoreWebView2CreationProperties : AvaloniaObject
{
    static CoreWebView2CreationProperties()
    {
        BrowserExecutableFolderProperty.Changed.Subscribe(EnvironmentPropertyChanged);
        UserDataFolderProperty.Changed.Subscribe(EnvironmentPropertyChanged);
        LanguageProperty.Changed.Subscribe(EnvironmentPropertyChanged);
    }

    /// <summary>
    /// The AvaloniaProperty which backs the <see cref="BrowserExecutableFolder" /> property.
    /// </summary>
    public static readonly StyledProperty<string> BrowserExecutableFolderProperty = AvaloniaProperty.Register<CoreWebView2CreationProperties, string>(nameof(BrowserExecutableFolder));

    /// <summary>
    /// The AvaloniaProperty which backs the <see cref="UserDataFolder" /> property.
    /// </summary>
    public static readonly StyledProperty<string> UserDataFolderProperty = AvaloniaProperty.Register<CoreWebView2CreationProperties, string>(nameof(UserDataFolder));

    /// <summary>
    /// The AvaloniaProperty which backs the <see cref="Language" /> property.
    /// </summary>
    public static readonly StyledProperty<string> LanguageProperty = AvaloniaProperty.Register<CoreWebView2CreationProperties, string>(nameof(Language));

    /// <summary>
    /// The AvaloniaProperty which backs the <see cref="ProfileName" /> property.
    /// </summary>
    public static readonly StyledProperty<string> ProfileNameProperty = AvaloniaProperty.Register<CoreWebView2CreationProperties, string>(nameof(ProfileName));

    /// <summary>
    /// The AvaloniaProperty which backs the <see cref="IsInPrivateModeEnabled" /> property.
    /// </summary>
    public static readonly StyledProperty<bool?> IsInPrivateModeEnabledProperty = AvaloniaProperty.Register<CoreWebView2CreationProperties, bool?>(nameof(IsInPrivateModeEnabled));

    Task<CoreWebView2Environment?>? _task;

    /// <summary>
    /// Gets or sets the value to pass as the browserExecutableFolder parameter of <see cref="CoreWebView2Environment.CreateAsync(string,string,CoreWebView2EnvironmentOptions)" /> when creating an environment with this instance.
    /// </summary>
    public string BrowserExecutableFolder
    {
        get => GetValue(BrowserExecutableFolderProperty);
        set => SetValue(BrowserExecutableFolderProperty, value);
    }

    /// <summary>
    /// Gets or sets the value to pass as the userDataFolder parameter of <see cref="CoreWebView2Environment.CreateAsync(string,string,CoreWebView2EnvironmentOptions)" /> when creating an environment with this instance.
    /// </summary>
    public string UserDataFolder
    {
        get => GetValue(UserDataFolderProperty);
        set => SetValue(UserDataFolderProperty, value);
    }

    /// <summary>
    /// Gets or sets the value to use for the Language property of the CoreWebView2EnvironmentOptions parameter passed to <see cref="CoreWebView2Environment.CreateAsync(string,string,CoreWebView2EnvironmentOptions)" /> when creating an environment with this instance.
    /// </summary>
    public string Language
    {
        get => GetValue(LanguageProperty);
        set => SetValue(LanguageProperty, value);
    }

    /// <summary>
    /// Gets or sets the value to use for the ProfileName property of the CoreWebView2ControllerOptions parameter passed to CreateCoreWebView2ControllerWithOptionsAsync when creating an controller with this instance.
    /// </summary>
    public string ProfileName
    {
        get => GetValue(ProfileNameProperty);
        set => SetValue(ProfileNameProperty, value);
    }

    /// <summary>
    /// Gets or sets the value to use for the IsInPrivateModeEnabled property of the CoreWebView2ControllerOptions parameter passed to CreateCoreWebView2ControllerWithOptionsAsync when creating an controller with this instance.
    /// </summary>
    public bool? IsInPrivateModeEnabled
    {
        get => GetValue(IsInPrivateModeEnabledProperty);
        set => SetValue(IsInPrivateModeEnabledProperty, value);
    }

    static void EnvironmentPropertyChanged(AvaloniaPropertyChangedEventArgs<string> e)
    {
        ((CoreWebView2CreationProperties)e.Sender)._task = null;
    }

    /// <summary>
    /// Create a <see cref="CoreWebView2Environment" /> using the current values of this instance's properties.
    /// </summary>
    /// <returns>A task which will provide the created environment on completion, or null if no environment-related options are set.</returns>
    /// <remarks>
    /// As long as no other properties on this instance are changed, repeated calls to this method will return the same task/environment as earlier calls.
    /// If some other property is changed then the next call to this method will return a different task/environment.
    /// </remarks>
    internal Task<CoreWebView2Environment?> CreateEnvironmentAsync()
    {
        if (_task == null && (BrowserExecutableFolder != null || UserDataFolder != null || Language != null))
            _task = CoreWebView2Environment.CreateAsync(BrowserExecutableFolder, UserDataFolder, new CoreWebView2EnvironmentOptions(language: Language));
        return _task ?? Task.FromResult<CoreWebView2Environment?>(null);
    }

    /// <summary>
    /// Create a <see cref="CoreWebView2ControllerOptions" /> using the current values of this instance's properties.
    /// </summary>
    /// <returns>A <see cref="CoreWebView2ControllerOptions" /> object or null if no controller-related properties are set.</returns>
    /// <exception cref="!:NullReferenceException">Thrown if the parameter environment is null.</exception>
    internal CoreWebView2ControllerOptions? CreateCoreWebView2ControllerOptions(CoreWebView2Environment environment)
    {
        CoreWebView2ControllerOptions? controllerOptions = null;
        if (ProfileName != null || IsInPrivateModeEnabled.HasValue)
        {
            controllerOptions = environment.CreateCoreWebView2ControllerOptions();
            controllerOptions.ProfileName = ProfileName;
            controllerOptions.IsInPrivateModeEnabled = IsInPrivateModeEnabled.GetValueOrDefault();
        }
        return controllerOptions;
    }
}