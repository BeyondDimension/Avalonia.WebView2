#if !DISABLE_WEBVIEW2_CORE && (WINDOWS || NETFRAMEWORK)
using Microsoft.Web.WebView2.Core;

namespace Avalonia.Controls;

partial class WebView2
{
    /// <summary>
    /// 当前运行的设备是否安装了 WebView2 运行时，未安装 WebView2 运行时时，WebView2 控件将无法正常工作
    /// </summary>
    public static bool IsSupported => RuntimeInit.isSupported;

    /// <summary>
    /// 当前安装的 WebView2 运行时版本号
    /// </summary>
    public static string? VersionString => RuntimeInit.availableBrowserVersionString;

    /// <summary>
    /// 检查当前运行的设备是否安装了 WebView2 运行时，未安装 WebView2 运行时时可引导安装，安装完成后调用此函数刷新安装状态
    /// </summary>
    public static void RefreshIsSupported() => RuntimeInit.RefreshIsSupported();
}

file static class RuntimeInit
{
    internal static bool isSupported;
    internal static string? availableBrowserVersionString;

    internal static void RefreshIsSupported()
    {
        try
        {
            availableBrowserVersionString = CoreWebView2Environment.GetAvailableBrowserVersionString();
            if (!string.IsNullOrWhiteSpace(availableBrowserVersionString))
            {
                isSupported = true;
            }
        }
        catch (WebView2RuntimeNotFoundException)
        {
            // Exception Info: Microsoft.Web.WebView2.Core.WebView2RuntimeNotFoundException: Couldn't find a compatible Webview2 Runtime installation to host WebViews.
            // ---> System.IO.FileNotFoundException: 系统找不到指定的文件。 (0x80070002)
            // --- End of inner exception stack trace ---
            // at Microsoft.Web.WebView2.Core.CoreWebView2Environment.CreateAsync(String browserExecutableFolder, String userDataFolder, CoreWebView2EnvironmentOptions options)
        }
    }
}
#endif