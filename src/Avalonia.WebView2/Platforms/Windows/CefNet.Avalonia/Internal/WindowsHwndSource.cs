// MIT License Copyright(c) 2020 CefNet
// https://github.com/CefNet/CefNet/blob/103.0.22181.155/CefNet.Avalonia/Internal/WindowsHwndSource.cs
#if WINDOWS || NETFRAMEWORK
using System.ComponentModel;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace CefNet.Internal;

internal delegate nint WindowsWindowProcDelegate(nint hwnd, uint message, nint wParam, nint lParam, ref bool handled);

internal sealed class WindowsHwndSource : CriticalFinalizerObject, IDisposable
{
    delegate nint WindowProc(nint hwnd, uint message, nint wParam, nint lParam);

    bool _disposed;
    nint hWndProcHook;
    readonly WindowProc fnWndProcHook;

    public static WindowsHwndSource FromHwnd(nint hwnd)
    {
        uint tid;
#if NO_CSWIN32
        tid = global::MS.Win32.NativeMethods.GetWindowThreadProcessId(hwnd, default);
#else
        global::Windows.Win32.Foundation.HWND hwnd_ = new(hwnd);
        unsafe
        {
            tid = global::Windows.Win32.PInvoke.GetWindowThreadProcessId(hwnd_, (uint*)default);
        }
#endif
        if (tid == 0)
            throw new Win32Exception(Marshal.GetLastWin32Error());

        var source = new WindowsHwndSource(hwnd);

        nint dwNewLong = Marshal.GetFunctionPointerForDelegate(source.fnWndProcHook);
        const int GWLP_WNDPROC = -4;
        source.hWndProcHook = global::MS.Win32.NativeMethods.SetWindowLong(hwnd, GWLP_WNDPROC, dwNewLong);

        if (source.hWndProcHook == default)
            throw new Win32Exception(Marshal.GetLastWin32Error());

        return source;
    }

    WindowsHwndSource(nint hwnd)
    {
        hWndProcHook = default;
        Handle = hwnd;
        fnWndProcHook = new WindowProc(WndProcHook);
    }

    ~WindowsHwndSource()
    {
        // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        Dispose(disposing: false);
    }

    void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // 释放托管状态(托管对象)
            }

            // 释放未托管的资源(未托管的对象)并重写终结器
            // 将大型字段设置为 null
            if (hWndProcHook != default)
            {
                const int GWLP_WNDPROC = -4;
                global::MS.Win32.NativeMethods.SetWindowLong(Handle, GWLP_WNDPROC, hWndProcHook);
            }
            _disposed = true;
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public nint Handle { get; }

    public WindowsWindowProcDelegate? WndProcCallback { get; set; }

    nint WndProcHook(nint hwnd, uint msg, nint wParam, nint lParam)
    {
        nint r = default;
        var w = WndProcCallback;
        if (w != null)
        {
            bool handled = false;
            r = w(hwnd, msg, wParam, lParam, ref handled);
            if (handled)
            {
                return r;
            }
        }

        var result = global::MS.Win32.NativeMethods.CallWindowProc(hWndProcHook, hwnd, msg, wParam, lParam);
        return result;
    }
}
#endif