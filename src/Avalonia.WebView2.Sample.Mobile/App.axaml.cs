using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.WebView2.Sample.ViewModels;
using Avalonia.WebView2.Sample.Views;

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
                DataContext = new MainViewModel()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}