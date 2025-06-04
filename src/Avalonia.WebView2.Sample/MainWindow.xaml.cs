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

namespace Avalonia.WebView2.Sample;

public sealed partial class MainWindow : Window
{
    global::Avalonia.Controls.WebView2? WebView => WebView2Compat?.WebView2;

    //public CoreWebView2Environment? Environment { get; }

    public MainWindow()
    {
        DataContext = this;
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif

        Title = this.FindControl<Label>("Title");
        WebView2Compat = this.FindControl<WebView2Compat>("WebView2Compat");
        Button = this.FindControl<Button>("Button");
        UrlTextBox = this.FindControl<TextBox>("UrlTextBox");
        AboutTextBlock = this.FindControl<TextBlock>("AboutTextBlock");

        var webView2AssemblyVersion =
#if WINDOWS
            $"Microsoft.Web.WebView2.Core: {GetVersion(typeof(CoreWebView2).Assembly)}";
#elif !(WINDOWS || NETFRAMEWORK) && NET8_0_OR_GREATER
            $"Microsoft.Web.WebView2.Core: {GetVersion(typeof(global::Xilium.CefGlue.Avalonia.AvaloniaCefBrowser).Assembly)}";
#else
            $"Xilium.CefGlue.Avalonia: {GetVersion(typeof(CoreWebView2).Assembly)}";
#endif

        AboutTextBlock.Text = $"Runtime: {System.Environment.Version}{System.Environment.NewLine}OSArchitecture: {RuntimeInformation.OSArchitecture}{System.Environment.NewLine}ProcessArchitecture: {RuntimeInformation.ProcessArchitecture}{System.Environment.NewLine}Avalonia: {GetVersion(typeof(Window).Assembly)}{System.Environment.NewLine}Avalonia.WebView2: {GetVersion(typeof(global::Avalonia.Controls.WebView2).Assembly)}{System.Environment.NewLine}{webView2AssemblyVersion}";
        Button.Click += Button_Click;
        UrlTextBox.KeyDown += UrlTextBox_KeyDown;
#if WINDOWS
        if (global::Avalonia.Controls.WebView2.IsSupported)
        {
            //Environment = WebView.CreationProperties!.CreateEnvironmentAsync().GetAwaiter().GetResult();
            //Environment.ProcessInfosChanged += Environment_ProcessInfosChanged;
            //SetTitle(Environment.BrowserVersionString);
            SetTitle(global::Avalonia.Controls.WebView2.VersionString);
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
        if (global::Avalonia.Controls.WebView2.IsSupported)
        {
            if (e.Key == Key.Enter)
            {
                var url = UrlTextBox.Text;
                if (!IsHttpUrl(url)) url = $"{Prefix_HTTPS}{url}";
                WebView?.CoreWebView2?.Navigate(url);
            }
        }
#elif LINUX

#elif IOS || MACCATALYST || MACOS
        if (e.Key == Key.Enter && WebView is not null)
        {
            var url = UrlTextBox.Text;
            if (!IsHttpUrl(url)) url = $"{Prefix_HTTPS}{url}";
            WebView?.Navigate(url);
        }
#endif
    }

    const string Prefix_HTTPS = "https://";
    const string Prefix_HTTP = "http://";

    static bool IsHttpUrl([NotNullWhen(true)] string? url, bool httpsOnly = false) => url != null &&
       (url.StartsWith(Prefix_HTTPS, StringComparison.OrdinalIgnoreCase) ||
             (!httpsOnly && url.StartsWith(Prefix_HTTP, StringComparison.OrdinalIgnoreCase)));

    const Architecture Unknown = (Architecture)int.MinValue;

    static string GetTitle(string? browserVersion, Architecture architecture = Unknown) => $"Microsoft Edge WebView2{(string.IsNullOrEmpty(browserVersion) ? null : $" {browserVersion}")}{(architecture == Unknown ? null : $" {architecture}")} for Avalonia on {System.Environment.OSVersion.VersionString}";

    void SetTitle(string? browserVersion, Architecture architecture = Unknown) => Title.Content = base.Title = GetTitle(browserVersion, architecture);

    void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

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
        WebView?.ShowDeveloperTools();
#elif IOS || MACCATALYST || MACOS

#elif ANDROID

#endif
    }

    //[DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
    //[return: MarshalAs(UnmanagedType.Bool)]
    //static extern bool IsWow64Process([In] IntPtr processHandle, [Out, MarshalAs(UnmanagedType.Bool)] out bool wow64Process);
}
