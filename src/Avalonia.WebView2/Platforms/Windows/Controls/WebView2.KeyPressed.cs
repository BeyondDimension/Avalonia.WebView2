#if WINDOWS || NETFRAMEWORK
using Avalonia.Input;
using Avalonia.Interactivity;
using Microsoft.Web.WebView2.Core;

namespace Avalonia.Controls;

partial class WebView2
{
    /// <summary>
    /// 键盘按下事件的路由事件
    /// </summary>
    protected virtual RoutedEvent? PreviewKeyDownEvent => KeyDownEvent;

    /// <summary>
    /// 键盘抬起事件的路由事件
    /// </summary>
    protected virtual RoutedEvent? PreviewKeyUpEvent => KeyUpEvent;

    /// <summary>
    /// This is an event handler for our CoreWebView2Controller's AcceleratorKeyPressed event.
    /// This is called to inform us about key presses that are likely to have special behavior (e.g. esc, return, Function keys, letters with modifier keys).
    /// Avalonia can't detect this input because Windows sends it directly to the Win32 CoreWebView2Controller control.
    /// We implement this by generating standard Avalonia key input events, allowing callers to handle the input in the usual Avalonia way if they want.
    /// If nobody handles the Avalonia key events then we'll allow the default CoreWebView2Controller logic (if any) to handle it.
    /// Of the possible options, this implementation should provide the most flexibility to callers.
    /// 这是 CoreWebView2Controller 的 AcceleratorKeyPressed 事件的事件处理程序。
    /// 调用该事件是为了通知我们按下的按键可能会有特殊行为（如 ESC、return、功能键、带修改键的字母）。
    /// Avalonia 无法检测到这种输入，因为 Windows 会直接将其发送到 Win32 CoreWebView2Controller 控件。
    /// 我们通过生成标准的 Avalonia 键输入事件来实现这一点，允许调用者按自己的意愿以通常的 Avalonia 方式处理输入。
    /// 如果没有人处理 Avalonia 键事件，我们将允许 CoreWebView2Controller 的默认逻辑（如果有的话）来处理它。
    /// 在所有可能的选项中，这种实现方式应能为调用者提供最大的灵活性。
    /// </summary>
    void CoreWebView2Controller_AcceleratorKeyPressed(object? sender, CoreWebView2AcceleratorKeyPressedEventArgs e)
    {
        var virtualKey = e.VirtualKey;
        if (virtualKey >= 0 && virtualKey <= int.MaxValue)
        {
            var eventArgs = new WebView2KeyEventArgs(virtualKey)
            {
                RoutedEvent = e.KeyEventKind == CoreWebView2KeyEventKind.KeyDown || e.KeyEventKind == CoreWebView2KeyEventKind.SystemKeyDown ? PreviewKeyDownEvent : PreviewKeyUpEvent,
            };
            RaiseEvent(eventArgs);
            e.Handled = eventArgs.Handled;
        }
    }
}
#endif