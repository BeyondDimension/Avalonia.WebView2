#if !DISABLE_WEBVIEW2_CORE && (WINDOWS || NETFRAMEWORK)
#if !DISABLE_WEBVIEW2_CORE && (WINDOWS || NETFRAMEWORK)
using Microsoft.Web.WebView2.Core;
#endif

namespace Avalonia.Controls;

/// <summary>
/// This class is a bundle of the most common parameters used to create CoreWebView2Environment and CoreWebView2Controller instances.
/// Its main purpose is to be set to <see cref="WebView2.CreationProperties" /> in order to customize the environment and/or controller used by a <see cref="WebView2" /> during implicit initialization.
/// It is also a nice Avalonia integration utility which allows commonly used environment/controller parameters to be dependency properties and be created and used in markup.
/// 该类包含用于创建 CoreWebView2Environment 和 CoreWebView2Controller 实例的最常用参数。
/// 其主要用途是设置为 <see cref="WebView2.CreationProperties" />，以便在隐式初始化过程中自定义 <see cref="WebView2" /> 使用的环境和/或控制器。
/// 它也是一个不错的 Avalonia 集成工具，可将常用的环境/控制器参数作为依赖属性，并在标记中创建和使用。
/// </summary>
/// <remarks>
/// This class isn't intended to contain all possible environment or controller customization options.
/// If you need complete control over the environment and/or controller used by a WebView2 control then you'll need to initialize the control explicitly by
/// creating your own environment (with CoreWebView2Environment.CreateAsync) and/or controller options (with CoreWebView2Environment.CreateCoreWebView2ControllerOptions) and passing them to WebView2.EnsureCoreWebView2Async
/// *before* you set the <see cref="WebView2.Source" /> property to anything.
/// See the <see cref="WebView2" /> class documentation for an initialization overview.
/// 该类并不打算包含所有可能的环境或控制器自定义选项。
/// 如果您需要完全控制 WebView2 控件使用的环境和/或控制器，那么您需要通过创建自己的环境（使用 CoreWebView2Environment.CreateAsync）和/或控制器选项（使用 CoreWebView2Environment.CreateCoreWebView2ControllerOptions）并将其传递给 WebView2.EnsureCoreWebView2Async 来显式地初始化控件。
/// *在*将 <see cref="WebView2.Source" /> 属性设置为任何内容之前。
/// 有关初始化概述，请参阅 <see cref="WebView2" /> 类文档。
/// </remarks>
public partial class CoreWebView2CreationProperties : AvaloniaObject
{
    static CoreWebView2CreationProperties()
    {
        BrowserExecutableFolderProperty.Changed.AddClassHandler<CoreWebView2CreationProperties, string>((t, args) => { EnvironmentPropertyChanged(args); });
        UserDataFolderProperty.Changed.AddClassHandler<CoreWebView2CreationProperties, string>((t, args) => { EnvironmentPropertyChanged(args); });
        LanguageProperty.Changed.AddClassHandler<CoreWebView2CreationProperties, string>((t, args) => { EnvironmentPropertyChanged(args); });
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

    /// <summary>
    /// The AvaloniaProperty which backs the <see cref="EnabledDevTools" /> property.
    /// </summary>
    public static readonly StyledProperty<bool> EnabledDevToolsProperty = AvaloniaProperty.Register<CoreWebView2CreationProperties, bool>(nameof(EnabledDevTools));

#if !DISABLE_WEBVIEW2_CORE && (WINDOWS || NETFRAMEWORK)
    Task<CoreWebView2Environment>? _task;
#endif

    /// <summary>
    /// Gets or sets the value to pass as the browserExecutableFolder parameter of CoreWebView2Environment.CreateAsync when creating an environment with this instance.
    /// </summary>
    public string BrowserExecutableFolder
    {
        get => GetValue(BrowserExecutableFolderProperty);
        set => SetValue(BrowserExecutableFolderProperty, value);
    }

    /// <summary>
    /// Gets or sets the value to pass as the userDataFolder parameter of CoreWebView2Environment.CreateAsync when creating an environment with this instance.
    /// </summary>
    public string UserDataFolder
    {
        get => GetValue(UserDataFolderProperty);
        set => SetValue(UserDataFolderProperty, value);
    }

    /// <summary>
    /// Gets or sets the value to use for the Language property of the CoreWebView2EnvironmentOptions parameter passed to CoreWebView2Environment.CreateAsync when creating an environment with this instance.
    /// </summary>
    public string Language
    {
        get => GetValue(LanguageProperty);
        set => SetValue(LanguageProperty, value);
    }

    /// <summary>
    /// F12 Developer Tools are enabled for the WebView2 control.
    /// </summary>
    public bool EnabledDevTools
    {
        get => GetValue(EnabledDevToolsProperty);
        set => SetValue(EnabledDevToolsProperty, value);
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
#if !DISABLE_WEBVIEW2_CORE && (WINDOWS || NETFRAMEWORK)
        ((CoreWebView2CreationProperties)e.Sender)._task = null;
#endif
    }

#if !DISABLE_WEBVIEW2_CORE && (WINDOWS || NETFRAMEWORK)
    /// <summary>
    /// Create a <see cref="CoreWebView2Environment" /> using the current values of this instance's properties.
    /// </summary>
    /// <returns>A task which will provide the created environment on completion, or null if no environment-related options are set.</returns>
    /// <remarks>
    /// As long as no other properties on this instance are changed, repeated calls to this method will return the same task/environment as earlier calls.
    /// If some other property is changed then the next call to this method will return a different task/environment.
    /// </remarks>
    public virtual Task<CoreWebView2Environment> CreateEnvironmentAsync()
    {
        lock (this)
        {
            if (_task == null)
            {
                if (BrowserExecutableFolder != null || UserDataFolder != null || Language != null)
                    _task = CoreWebView2Environment.CreateAsync(BrowserExecutableFolder, UserDataFolder, new CoreWebView2EnvironmentOptions(language: Language));
                else
                    _task = CoreWebView2Environment.CreateAsync();
            }
            return _task;
        }
    }

    /// <summary>
    /// Create a <see cref="CoreWebView2ControllerOptions" /> using the current values of this instance's properties.
    /// </summary>
    /// <returns>A <see cref="CoreWebView2ControllerOptions" /> object or null if no controller-related properties are set.</returns>
    /// <exception cref="!:NullReferenceException">Thrown if the parameter environment is null.</exception>
    public virtual CoreWebView2ControllerOptions? CreateCoreWebView2ControllerOptions(CoreWebView2Environment environment)
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
#endif
}
#endif