using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using ReactiveUI;

namespace Avalonia.WebView2.Sample.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    MainViewModel()
    {
    }

    public static MainViewModel Instance { get; } = new();

    /// <summary>
    /// WebView 默认加载的 Url 地址源
    /// </summary>
    public string Source => "https://bing.com";

    /// <summary>
    /// 状态栏的外边距
    /// </summary>
    public Thickness StatusBarMargin
    {
        get => field;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>
    /// 设置状态栏的高度用于顶部控件的外边距
    /// </summary>
    public static void SetStatusBarMargin(double left = 0, double top = 0, double right = 0, double bottom = 0, double scaling = 1)
    {
        Instance.StatusBarMargin =
                        new Thickness(GetValue(left), GetValue(top), GetValue(right), GetValue(bottom));

        int GetValue(double d)
        {
            if (double.IsNaN(d) || d <= 0D)
            {
                return 0;
            }
            else
            {
                var d2 = d / scaling;
                var d3 = Math.Ceiling(d2);
                var d4 = Convert.ToInt32(d3);
                return d4;
            }
        }
    }
}
