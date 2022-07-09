using Avalonia.Markup.Xaml;
using AvaloniaWebView2 = Avalonia.Controls.WebView2;

namespace Avalonia.WebView2.Sample;

public partial class MainWindow : Window
{
    readonly Button Button;
    readonly AvaloniaWebView2 WebView2;

    public MainWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
        WebView2 = this.FindControl<AvaloniaWebView2>("WebView2");
        Button = this.FindControl<Button>("Button");
        Button.Click += Button_Click;
    }

    private void Button_Click(object? sender, RoutedEventArgs e)
    {
        WebView2.Test();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
