using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using static Avalonia.Controls.WebView2;

namespace Avalonia.WebView2.Sample.Views;

public partial class MainView : UserControl, IStorageService
{
    public MainView()
    {
        InitializeComponent();

        UrlTextBox.KeyDown += UrlTextBox_KeyDown;

        WV.StorageService = this;
        //WV.Fill = new SolidColorBrush(Colors.Purple);
    }

    IEnumerable<KeyValuePair<(StorageItemType type, string key), StorageItemValue>>? IStorageService.GetStorages(string requestUri)
    {
        var result = SampleHelper.GetStorages(requestUri);
        return result;
    }

    void UrlTextBox_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            var text = UrlTextBox.Text;
            SampleHelper.Navigate(WV, text);
        }
    }
}