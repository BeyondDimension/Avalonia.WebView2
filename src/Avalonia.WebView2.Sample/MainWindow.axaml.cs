using Avalonia.Markup.Xaml;
using AvaloniaWebView2 = Avalonia.Controls.WebView2;

namespace Avalonia.WebView2.Sample;

public partial class MainWindow : Window
{
    readonly Button Button;
    readonly AvaloniaWebView2 WebView2;
    readonly new Label Title;

    public CoreWebView2CreationProperties CreationProperties { get; } = new()
    {
        Language = "en",
    };

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
        InitializeControls();
    }

    void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    void InitializeControls()
    {
        Button.Click += Button_Click;
        var environment = CreationProperties.CreateEnvironmentAsync().GetAwaiter().GetResult()!;
        Title.Content = $"{Title.Content} {environment.BrowserVersionString}";
    }

    void Button_Click(object? sender, RoutedEventArgs e)
    {
        WebView2.Test();
    }
}
