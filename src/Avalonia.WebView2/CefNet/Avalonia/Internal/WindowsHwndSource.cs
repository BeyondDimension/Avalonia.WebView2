// MIT License Copyright(c) 2020 CefNet
// https://github.com/CefNet/CefNet/blob/103.0.22181.155/CefNet.Avalonia/Internal/WindowsHwndSource.cs

namespace CefNet.Internal;

#if WINDOWS
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

internal delegate LRESULT WindowsWindowProcDelegate(HWND hwnd, uint message, WPARAM wParam, LPARAM lParam, ref bool handled);

internal sealed class WindowsHwndSource : CriticalFinalizerObject, IDisposable
{
    delegate LRESULT WindowProc(HWND hwnd, uint message, WPARAM wParam, LPARAM lParam);

    bool _disposed;
    WNDPROC hWndProcHook;
    readonly WindowProc fnWndProcHook;

    internal unsafe static uint GetWindowThreadProcessId(HWND hwnd, out uint lpdwProcessId)
    {
        fixed (uint* alpdwProcessId = &lpdwProcessId)
            return PInvoke.GetWindowThreadProcessId(hwnd, alpdwProcessId);
    }

    public unsafe static WindowsHwndSource FromHwnd(HWND hwnd)
    {
        uint tid = GetWindowThreadProcessId(hwnd, out _);
        if (tid == 0)
            throw new Win32Exception(Marshal.GetLastWin32Error());

        var source = new WindowsHwndSource(hwnd);

        IntPtr dwNewLong = Marshal.GetFunctionPointerForDelegate(source.fnWndProcHook);
        source.hWndProcHook = SetWindowLong(hwnd, dwNewLong);

        if (source.hWndProcHook.IsNull)
            throw new Win32Exception(Marshal.GetLastWin32Error());

        return source;
    }

    WindowsHwndSource(HWND hwnd)
    {
        hWndProcHook = null;
        Handle = hwnd;
        fnWndProcHook = new WindowProc(WndProcHook);
    }

    ~WindowsHwndSource()
    {
        Dispose(false);
    }

    void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (!hWndProcHook.IsNull)
        {
            SetWindowLong(Handle, Marshal.GetFunctionPointerForDelegate(hWndProcHook));
        }
        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public HWND Handle { get; }

    public WindowsWindowProcDelegate? WndProcCallback { get; set; }

    unsafe LRESULT WndProcHook(HWND hwnd, uint msg, WPARAM wParam, LPARAM lParam)
    {
        IntPtr retval = IntPtr.Zero;
        var wndproc = WndProcCallback;
        if (wndproc != null)
        {
            bool handled = false;
            retval = wndproc(hwnd, msg, wParam, lParam, ref handled);
            if (handled)
                return new(retval);
        }

        var result = PInvoke.CallWindowProcNotW(hWndProcHook.Value, hwnd, msg, wParam, lParam);
        return new LRESULT(result);
    }

    IntPtr oldhWndProcHook;

    static unsafe WNDPROC SetWindowLong(HWND hWnd, IntPtr dwNewLong)
    {
        IntPtr oldProcPtr = IntPtr.Zero;
#if X86
        oldProcPtr = new IntPtr(PInvoke.SetWindowLong(hWnd, WINDOW_LONG_PTR_INDEX.GWLP_WNDPROC, dwNewLong.ToInt32()));
#endif

#if X64
        oldProcPtr = PInvoke.SetWindowLongPtr(hWnd, WINDOW_LONG_PTR_INDEX.GWLP_WNDPROC, dwNewLong);
#endif
        return new((delegate* unmanaged<HWND, uint, WPARAM, LPARAM, LRESULT>)oldProcPtr);
    }
}
#endif