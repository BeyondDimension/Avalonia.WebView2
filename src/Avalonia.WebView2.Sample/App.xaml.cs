using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace Avalonia.WebView2.Sample;

public sealed class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        InitWebView2();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }

    static void InitWebView2()
    {
#if WINDOWS
        if (global::Avalonia.Controls.WebView2.IsSupported)
        {
            //AvaloniaWebView2.DefaultCreationProperties = new()
            //{
            //    Language = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName,
            //    UserDataFolder = GetUserDataFolder(),
            //};

            //static string GetUserDataFolder()
            //{
            //    var path = Path.Combine(AppContext.BaseDirectory, "AppData", "WebView2", "UserData");
            //    if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            //    return path;
            //}
        }
#elif !(WINDOWS || NETFRAMEWORK) && NET8_0_OR_GREATER
#endif
    }
}
