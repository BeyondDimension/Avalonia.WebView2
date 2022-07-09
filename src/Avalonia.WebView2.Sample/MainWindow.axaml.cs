using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using _WebView2 = Avalonia.Controls.WebView2;

namespace Avalonia.WebView2.Sample
{
    public partial class MainWindow : Window
    {
        Button Button;
        _WebView2 WebView2;

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            WebView2 = this.FindControl<_WebView2>("WebView2");
            Button = this.FindControl<Button>("Button");
            Button.Click += Button_Click;
        }

        private void Button_Click(object? sender, Interactivity.RoutedEventArgs e)
        {
            WebView2.Test();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
