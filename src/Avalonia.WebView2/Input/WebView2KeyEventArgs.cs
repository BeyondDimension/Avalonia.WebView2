using Avalonia.Controls;

namespace Avalonia.Input;

/// <summary>
/// <see cref="WebView2"/> 控件的键盘输入事件参数
/// </summary>
public partial class WebView2KeyEventArgs : KeyEventArgs
{
    public long Timestamp { get; } =
#if NETCOREAPP3_0_OR_GREATER
        Environment.TickCount64;
#else
        Environment.TickCount;
#endif

    public WebView2KeyEventArgs(Key key)
    {
        Key = key;
    }
}