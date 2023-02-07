namespace Avalonia.WebView2.Sample;

public sealed partial class WebView2Compat : UserControl
{
    public readonly AvaloniaWebView2 WebView2;
    public readonly TextBlock TextBlock;

    public WebView2Compat()
    {
        InitializeComponent();
        WebView2 = this.FindControl<AvaloniaWebView2>("WebView2");
        WebView2.IsVisible = false;
        TextBlock = this.FindControl<TextBlock>("TextBlock");
        if (!AvaloniaWebView2.IsSupported)
        {
            TextBlock.Text = "Couldn't find a compatible Webview2 Runtime installation to host WebViews.";
        }
        else
        {
            WebView2.DOMContentLoaded += WebView2_DOMContentLoaded;
        }
    }

    void WebView2_DOMContentLoaded(object? sender, CoreWebView2DOMContentLoadedEventArgs e)
    {
        WebView2.IsVisible = true;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
