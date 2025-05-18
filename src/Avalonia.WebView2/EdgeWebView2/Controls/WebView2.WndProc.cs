#if !DISABLE_WEBVIEW2_CORE && (WINDOWS || NETFRAMEWORK)
using Microsoft.Web.WebView2.Core;
using MS.Win32;

namespace Avalonia.Controls;

partial class WebView2 : global::Avalonia.Controls.Platforms.Windows.Interop.IHwndHost
{
    nint global::Avalonia.Controls.Platforms.Windows.Interop.IHwndHost.WndProc(nint hwnd, uint msg, nint wParam, nint lParam, ref bool handled)
    {
        var result = WndProc(hwnd, msg, wParam, lParam, ref handled);
        return result;
    }

    ///// <summary>
    ///// 是否启用在 WndProc 中处理 WM_SETFOCUS 消息
    ///// </summary>
    //protected virtual bool EnabledOnWndProcSetFocus => false;

    /// <summary>
    /// This is overridden from <see cref="NativeControlHost" /> and is called to provide us with Win32 messages that are sent to our hwnd.
    /// </summary>
    /// <param name="hwnd">Window receiving the message (should always match our <see cref="global::Avalonia.Platform.IPlatformHandle.Handle" />).</param>
    /// <param name="msg">Indicates the message being received.  See Win32 documentation for WM_* constant values.</param>
    /// <param name="wParam">The "wParam" data being provided with the message.  Meaning varies by message.</param>
    /// <param name="lParam">The "lParam" data being provided with the message.  Meaning varies by message.</param>
    /// <param name="handled">If true then the message will not be forwarded to any (more) <see cref="global::CefNet.Internal.GlobalHooks" /> handlers.</param>
    /// <returns>Return value varies by message.</returns>
    protected virtual nint WndProc(nint hwnd, uint msg, nint wParam, nint lParam, ref bool handled)
    {
        if (IsInDesignMode)
        {
            return default;
        }

        switch (msg)
        {
            //case (uint)NativeMethods.WM.SETFOCUS:
            //    {
            //        if (EnabledOnWndProcSetFocus)
            //        {
            //            _coreWebView2Controller?.MoveFocus(CoreWebView2MoveFocusReason.Programmatic);
            //        }
            //    }
            //    break;
            case (uint)NativeMethods.WM.PAINT:
                {
#if NO_CSWIN32
                    NativeMethods.BeginPaint(hwnd, out var lpPaint);
                    NativeMethods.EndPaint(hwnd, ref lpPaint);
#else
                    global::Windows.Win32.Foundation.HWND hWnd_ = new(hwnd);
                    global::Windows.Win32.PInvoke.BeginPaint(hWnd_, out var lpPaint);
                    global::Windows.Win32.PInvoke.EndPaint(hWnd_, in lpPaint);
                    handled = true;
#endif
                }
                break;
        }

        return default;
    }
}
#endif