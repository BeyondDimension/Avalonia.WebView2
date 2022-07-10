using Avalonia.Markup.Xaml;
using AvaloniaWebView2 = Avalonia.Controls.WebView2;

namespace Avalonia.WebView2.Sample;

public class App : Application
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

    public static bool AvailableWebView2 { get; private set; }

    public static string? WebView2VersionString { get; private set; }

    static void InitWebView2()
    {
        try
        {
            WebView2VersionString = CoreWebView2Environment.GetAvailableBrowserVersionString();
            if (!string.IsNullOrEmpty(WebView2VersionString)) AvailableWebView2 = true;
        }
        catch (WebView2RuntimeNotFoundException)
        {

        }

        if (AvailableWebView2)
        {
            AvaloniaWebView2.DefaultCreationProperties = new()
            {
                Language = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName,
                UserDataFolder = GetUserDataFolder(),
            };

            static string GetUserDataFolder()
            {
                var path = Path.Combine(AppContext.BaseDirectory, "AppData", "WebView2", "UserData");
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                return path;
            }
        }
    }
}
