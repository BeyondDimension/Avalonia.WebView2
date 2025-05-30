// MIT License Copyright(c) 2020 CefNet
// https://github.com/CefNet/CefNet/blob/103.0.22181.155/CefNet.Avalonia/WinApi/NativeMethods.cs

#if !DISABLE_WEBVIEW2_CORE && (WINDOWS || NETFRAMEWORK)
namespace MS.Win32;

partial class NativeMethods
{
#if DEBUG || (NO_CSWIN32 && (WINDOWS || NETFRAMEWORK))
    [global::System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
    public static extern uint GetWindowThreadProcessId(nint hWnd, nint pPid);
#endif

    public static nint SetWindowLong(nint hWnd, int nIndex, nint dwNewLong)
    {
        if (IntPtr.Size == 8)
            return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
        else
            return SetWindowLong32(hWnd, nIndex, unchecked((int)dwNewLong));
    }

    [global::System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "SetWindowLong", CharSet = global::System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
    static extern int SetWindowLong32(nint hWnd, int nIndex, int dwNewLong);

    [global::System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", CharSet = global::System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
    static extern nint SetWindowLongPtr64(nint hWnd, int nIndex, nint dwNewLong);

    [global::System.Runtime.InteropServices.DllImport("user32.dll", CharSet = global::System.Runtime.InteropServices.CharSet.Auto)]
    public static extern nint CallWindowProc(nint lpPrevWndFunc, nint hWnd, uint msg, nint wParam, nint lParam); // CsWin32 生成的标注 ExactSpelling = true 会闪退
}
#endif