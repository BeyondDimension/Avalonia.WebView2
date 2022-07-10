using Avalonia.Markup.Xaml;
using AvaloniaWebView2 = Avalonia.Controls.WebView2;

namespace Avalonia.WebView2.Sample;

public partial class MainWindow : Window
{
    readonly Button Button;
    readonly AvaloniaWebView2 WebView2;
    readonly new Label Title;

    static string GetUserDataFolder()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "AppData", "WebView2", "UserData");
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        return path;
    }

    public CoreWebView2CreationProperties CreationProperties { get; } = new()
    {
        Language = "en",
        UserDataFolder = GetUserDataFolder(),
    };

    public CoreWebView2Environment Environment { get; }

    public MainWindow()
    {
        DataContext = this;
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
        Title = this.FindControl<Label>("Title");
        WebView2 = this.FindControl<AvaloniaWebView2>("WebView2");
        Button = this.FindControl<Button>("Button");

        Button.Click += Button_Click;
        Environment = CreationProperties.CreateEnvironmentAsync().GetAwaiter().GetResult();
        Environment.ProcessInfosChanged += Environment_ProcessInfosChanged;
        SetTitle(Environment.BrowserVersionString);
    }

    const Architecture Unknown = (Architecture)int.MinValue;

    static string GetTitle(string browserVersion, Architecture architecture = Unknown)
    {
        if (architecture != Unknown)
        {
            return $"Microsoft Edge WebView2 {browserVersion} {architecture} for Avalonia";
        }
        return $"Microsoft Edge WebView2 {browserVersion} for Avalonia";
    }

    void SetTitle(string browserVersion, Architecture architecture = Unknown) => Title.Content = base.Title = GetTitle(browserVersion, architecture);

    void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    void Environment_ProcessInfosChanged(object? sender, object e)
    {
        var processInfos = Environment.GetProcessInfos();
        foreach (var processInfo in processInfos)
        {
            try
            {
                var process = Process.GetProcessById(processInfo.ProcessId);
                if (IsWow64Process(process.Handle, out var wow64Process))
                {
                    Environment.ProcessInfosChanged -= Environment_ProcessInfosChanged;
                    SetTitle(Environment.BrowserVersionString, RuntimeInformation.ProcessArchitecture switch
                    {
                        Architecture.X86 or Architecture.X64 => wow64Process ? Architecture.X64 : Architecture.X86,
                        Architecture.Arm or Architecture.Arm64 => wow64Process ? Architecture.Arm64 : Architecture.Arm,
                        _ => Unknown,
                    });
                }
                return;
            }
            catch
            {

            }
        }
    }

    void Button_Click(object? sender, RoutedEventArgs e)
    {
#if DEBUG
        WebView2.Test();
#endif
    }

    [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool IsWow64Process([In] IntPtr processHandle, [Out, MarshalAs(UnmanagedType.Bool)] out bool wow64Process);
}
