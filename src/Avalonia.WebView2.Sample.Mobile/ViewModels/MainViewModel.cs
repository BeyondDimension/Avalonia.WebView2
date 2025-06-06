#pragma warning disable CA1822 // Mark members as static

namespace Avalonia.WebView2.Sample.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    /// <summary>
    /// WebView 默认加载的 Url 地址源
    /// </summary>
    public string Source => "https://bing.com";

    /// <summary>
    /// 状态栏的外边距
    /// </summary>
    public Thickness StatusBarMargin => new(0, StatusBarMarginTop, 0, 0);

    /// <summary>
    /// 设置或获取状态栏的高度用于顶部控件的外边距顶部
    /// </summary>
    public static double StatusBarMarginTop { get; set; }
}
