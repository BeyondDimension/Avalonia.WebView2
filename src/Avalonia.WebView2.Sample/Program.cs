using Microsoft.Win32;
using System.Windows;


namespace Avalonia.WebView2.Sample;

static class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
#if WINDOWS
        // If urn:schemas-microsoft-com:compatibility.v1 supportedOS exists in the app.manifest file, the control will not display normally, but it is normal in WPF
        if (IsProgramInCompatibilityMode())
        {
            // It's strange that the control running in compatibility mode can't display normally, but it works in WPF
            MessageBox.Show("Windows Program Compatibility mode is on. Turn it off and then try again", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
#endif

        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

#if WINDOWS
    static bool IsProgramInCompatibilityMode()
    {
        try
        {
            foreach (var item in new[] { Registry.CurrentUser, Registry.LocalMachine })
            {
                using var layers = item.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\AppCompatFlags\Layers");
                var value = layers?.GetValue(Environment.ProcessPath)?.ToString();
                if (value != null)
                {
                    if (value.Contains("WIN8RTM", StringComparison.OrdinalIgnoreCase)) return true;
                    if (value.Contains("WIN7RTM", StringComparison.OrdinalIgnoreCase)) return true;
                }
            }
        }
        catch
        {
        }
        return false;
    }
#endif

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
    {
        var b = AppBuilder.Configure<App>()
#if WINDOWS
             .UseWin32()
#elif MACOS
             .UseAvaloniaNative()
#elif LINUX
             .UseX11()
             //.With(new X11PlatformOptions())
#endif
             .UseSkia()
             .LogToTrace()
#if !(WINDOWS || NETFRAMEWORK) && NET8_0_OR_GREATER && !ANDROID && !IOS
             //.AfterSetup(_ => CefRuntimeLoader.Initialize(new CefSettings()
             //{
             //    RootCachePath = GetCachePath(),
             //    WindowlessRenderingEnabled = false,
             //    LogSeverity = CefLogSeverity.Verbose,
             //}))
#endif
             ;
        return b;
    }

#if !(WINDOWS || NETFRAMEWORK) && NET8_0_OR_GREATER && !ANDROID && !IOS
    static string GetCachePath() => Path.Combine(Path.GetTempPath(), "CefGlue_" + Guid.NewGuid().ToString().Replace("-", null));
#endif

}
