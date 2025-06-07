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
sealed class MainActivity : AvaloniaMainActivity<App>
{
    /// <summary>
    /// 获取状态栏的高度
    /// </summary>
    /// <param name="ctx"></param>
    /// <returns></returns>
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

    /// <summary>
    /// 获取 DPI 缩放比例
    /// </summary>
    /// <param name="ctx"></param>
    /// <returns></returns>
    static double GetScaling(Context ctx)
    {
        // https://github.com/AvaloniaUI/Avalonia/blob/a8fb53e92ae31eced207c56d4d2662467afa7d9e/src/Android/Avalonia.Android/Platform/AndroidScreens.cs#L41-L44
        if (ctx.Resources?.Configuration is { } config)
        {
            return config.DensityDpi / (double)global::Android.Util.DisplayMetricsDensity.Default;
        }
        return 1D;
    }

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        var statusBarHeight = GetStatusBarHeight(this);
        var scaling = GetScaling(this);
        MainViewModel.SetStatusBarMargin(top: statusBarHeight, scaling: scaling);

        base.OnCreate(savedInstanceState);
    }

    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        return App.BuildAvaloniaApp(base.CustomizeAppBuilder(builder))
            .UseReactiveUI();
    }
}
