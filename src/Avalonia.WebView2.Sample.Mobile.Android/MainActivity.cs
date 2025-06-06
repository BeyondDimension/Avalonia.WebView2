using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Views;
using AndroidX.Core.View;
using Avalonia;
using Avalonia.Android;
using Avalonia.Android.Platform.Specific;
using Avalonia.Controls.Platform;
using Avalonia.Platform;
using Avalonia.ReactiveUI;
using Avalonia.WebView2.Sample.ViewModels;

namespace Avalonia.WebView2.Sample;

[Activity(
    Label = "@string/app_name",
    Theme = "@style/Theme.WebView2.Sample.NoActionBar",
    Icon = "@mipmap/ic_launcher",
    RoundIcon = "@mipmap/ic_launcher_round",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public sealed class MainActivity : AvaloniaMainActivity<App>
{
    static int GetStatusBarHeight(Context ctx)
    {
        var resId = ctx.Resources!.GetIdentifier("status_bar_height", "dimen", "android");
        if (resId > 0)
        {
            var h = ctx.Resources.GetDimensionPixelSize(resId);
            return h;
        }
        return default;
    }

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        MainViewModel.StatusBarMarginTop = GetStatusBarHeight(this);

        base.OnCreate(savedInstanceState);
    }

    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        return base.CustomizeAppBuilder(builder)
            .WithInterFont()
            .UseReactiveUI();
    }
}
