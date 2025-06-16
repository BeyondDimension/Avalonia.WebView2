using Android.Runtime;
using Android.Webkit;
using WV2 = global::Avalonia.Controls.WebView2;

namespace Avalonia.WebView2.Sample;

[Application(
    Debuggable =
#if DEBUG
    true,
#else
    false,
#endif
    UsesCleartextTraffic = WV2.UsesCleartextTraffic)]
sealed class MainApplication : global::Android.App.Application
{
    internal MainApplication(nint javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
    {
    }

    public override void OnCreate()
    {
        base.OnCreate();

        // 开启远程调试 WebView https://developer.chrome.google.cn/docs/devtools/remote-debugging/webviews
        WebView.SetWebContentsDebuggingEnabled(true);
    }
}
