namespace Avalonia.WebView2.Sample;

public partial class MainWindow : Window
{
    readonly Button Button;
    readonly WebView2Compat WebView2Compat;
    readonly new Label Title;
    readonly TextBox UrlTextBox;
    readonly TextBlock AboutTextBlock;

    AvaloniaWebView2? WebView => WebView2Compat?.WebView2;

    public CoreWebView2Environment? Environment { get; }

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

        AboutTextBlock.Text = $"OSArchitecture: {RuntimeInformation.OSArchitecture}{System.Environment.NewLine}ProcessArchitecture: {RuntimeInformation.ProcessArchitecture}{System.Environment.NewLine}Avalonia: {GetVersion(typeof(Window).Assembly)}{System.Environment.NewLine}Avalonia.WebView2: {GetVersion(typeof(AvaloniaWebView2).Assembly)}{System.Environment.NewLine}Microsoft.Web.WebView2.Core: {GetVersion(typeof(CoreWebView2).Assembly)}";
        Button.Click += Button_Click;
        if (AvaloniaWebView2.IsSupported)
        {
            UrlTextBox.KeyDown += UrlTextBox_KeyDown;
            Environment = AvaloniaWebView2.DefaultCreationProperties!.CreateEnvironmentAsync().GetAwaiter().GetResult();
            Environment.ProcessInfosChanged += Environment_ProcessInfosChanged;
            SetTitle(Environment.BrowserVersionString);
        }
        else
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
        if (e.Key == Key.Enter)
        {
            var url = UrlTextBox.Text;
            if (!IsHttpUrl(url)) url = $"{Prefix_HTTPS}{url}";
            WebView?.CoreWebView2?.Navigate(url);
        }
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

    void Environment_ProcessInfosChanged(object? sender, object e)
    {
        var processInfos = Environment!.GetProcessInfos();
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
        WebView?.Test();
#endif
    }

    [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool IsWow64Process([In] IntPtr processHandle, [Out, MarshalAs(UnmanagedType.Bool)] out bool wow64Process);
}
