#if WINDOWS
using Microsoft.Web.WebView2.Core;
#endif
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices;
using WV2 = Avalonia.Controls.WebView2;

namespace Avalonia.WebView2.Sample;

public sealed partial class MainWindow : Window
{
    WV2? WebView => WebView2Compat?.WebView2;

    //public CoreWebView2Environment? Environment { get; }

    public MainWindow()
    {
        DataContext = this;
        InitializeComponent();

        var webView2AssemblyVersion =
#if WINDOWS
            $"Microsoft.Web.WebView2.Core: {GetVersion(typeof(CoreWebView2).Assembly)}";
#else
            $"TODO: ";
#endif

        AboutTextBlock.Text = $"Runtime: {Environment.Version}{Environment.NewLine}OSArchitecture: {RuntimeInformation.OSArchitecture}{Environment.NewLine}ProcessArchitecture: {RuntimeInformation.ProcessArchitecture}{Environment.NewLine}Avalonia: {GetVersion(typeof(Window).Assembly)}{Environment.NewLine}Avalonia.WebView2: {GetVersion(typeof(WV2).Assembly)}{Environment.NewLine}{webView2AssemblyVersion}";
        Button.Click += Button_Click;
        UrlTextBox.KeyDown += UrlTextBox_KeyDown;
#if WINDOWS
        if (Controls.WebView2.IsSupported)
        {
            //Environment = WebView.CreationProperties!.CreateEnvironmentAsync().GetAwaiter().GetResult();
            //Environment.ProcessInfosChanged += Environment_ProcessInfosChanged;
            //SetTitle(Environment.BrowserVersionString);
            SetTitle(Controls.WebView2.VersionString);
        }
        else
#endif
        {
            SetTitle(null);
        }
    }

    static string? GetVersion(Assembly assembly)
    {
        try
        {
            var attr = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            if (attr != null)
                return attr.InformationalVersion;
            var attr2 = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>();
            if (attr2 != null)
                return attr2.Version;
            var attr3 = assembly.GetCustomAttribute<AssemblyVersionAttribute>();
            if (attr3 != null)
                return attr3.Version;
            return null;
        }
        catch (Exception e)
        {
            return e.ToString();
        }
    }

    void UrlTextBox_KeyDown(object? sender, KeyEventArgs e)
    {
#if WINDOWS
        if (Controls.WebView2.IsSupported)
#endif
        {
            if (e.Key == Key.Enter)
            {
                var wv = WebView;
                if (wv != null)
                {
                    var url = UrlTextBox.Text;
                    SampleHelper.Navigate(wv, url);
                }
            }
        }
    }

    const Architecture Unknown = (Architecture)int.MinValue;

    static string GetTitle(string? browserVersion, Architecture architecture = Unknown) => $"Microsoft Edge WebView2{(string.IsNullOrEmpty(browserVersion) ? null : $" {browserVersion}")}{(architecture == Unknown ? null : $" {architecture}")} for Avalonia on {Environment.OSVersion.VersionString}";

    void SetTitle(string? browserVersion, Architecture architecture = Unknown)
        => Title.Content = base.Title = GetTitle(browserVersion, architecture);

    //void Environment_ProcessInfosChanged(object? sender, object e)
    //{
    //    var processInfos = Environment!.GetProcessInfos();
    //    foreach (var processInfo in processInfos)
    //    {
    //        try
    //        {
    //            var process = Process.GetProcessById(processInfo.ProcessId);
    //            if (IsWow64Process(process.Handle, out var wow64Process))
    //            {
    //                Environment.ProcessInfosChanged -= Environment_ProcessInfosChanged;
    //                SetTitle(Environment.BrowserVersionString, RuntimeInformation.ProcessArchitecture switch
    //                {
    //                    Architecture.X86 or Architecture.X64 => wow64Process ? Architecture.X64 : Architecture.X86,
    //                    Architecture.Arm or Architecture.Arm64 => wow64Process ? Architecture.Arm64 : Architecture.Arm,
    //                    _ => Unknown,
    //                });
    //            }
    //            return;
    //        }
    //        catch
    //        {

    //        }
    //    }
    //}

    void Button_Click(object? sender, RoutedEventArgs e)
    {
#if WINDOWS
        WebView?.CoreWebView2?.OpenDevToolsWindow();
#elif LINUX
        //WebView?.ShowDeveloperTools();
#elif IOS || MACCATALYST || MACOS

#elif ANDROID

#endif
    }

    //[DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
    //[return: MarshalAs(UnmanagedType.Bool)]
    //static extern bool IsWow64Process([In] IntPtr processHandle, [Out, MarshalAs(UnmanagedType.Bool)] out bool wow64Process);
}
