namespace Avalonia.WebView2.Sample;

public partial class WebView2Compat : UserControl
{
    public readonly AvaloniaWebView2 WebView2;
    public readonly TextBlock TextBlock;

    public WebView2Compat()
    {
        InitializeComponent();
        WebView2 = this.FindControl<AvaloniaWebView2>("WebView2");
        TextBlock = this.FindControl<TextBlock>("TextBlock");
        if (!AvaloniaWebView2.IsSupported)
        {
            TextBlock.Text = "Couldn't find a compatible Webview2 Runtime installation to host WebViews.";
        }
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
