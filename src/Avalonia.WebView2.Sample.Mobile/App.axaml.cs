using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.WebView2.Sample.ViewModels;
using Avalonia.WebView2.Sample.Views;
//using BD.Avalonia8.Fonts;

namespace Avalonia.WebView2.Sample;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        //if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        //{
        //    desktop.MainWindow = new MainWindow
        //    {
        //        DataContext = new MainViewModel()
        //    };
        //}
        //else
        // 此示例项目仅移动端
        if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = MainViewModel.Instance,
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    public static AppBuilder BuildAvaloniaApp(AppBuilder? builder = null)
    {
        //FontManagerOptions options = new()
        //{
        //    DefaultFamilyName = HarmonyOS_Sans_SC.Name,
        //    FontFallbacks =
        //    [
        //        new FontFallback { FontFamily = HarmonyOS_Sans_SC.Instance },
        //        new FontFallback { FontFamily = FontFamily.Default },
        //    ],
        //};
        builder ??= AppBuilder.Configure<App>();
        return builder/*.With(options)*/;
    }
}