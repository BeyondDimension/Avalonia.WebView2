using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaWebView2 = Avalonia.Controls.WebView2;

namespace Avalonia.WebView2.Sample;

public partial class WebView2Compat : UserControl
{
    public readonly AvaloniaWebView2 WebView2;

    public WebView2Compat()
    {
        InitializeComponent();
        WebView2 = this.FindControl<AvaloniaWebView2>("WebView2");
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
