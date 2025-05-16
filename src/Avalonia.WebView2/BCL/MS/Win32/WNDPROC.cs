#if WINDOWS
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

namespace Windows.Win32.UI.WindowsAndMessaging;

/// <summary>
/// https://github.com/microsoft/CsWin32/issues/778
/// </summary>
internal unsafe readonly struct WNDPROC
{
    public readonly delegate* unmanaged<HWND, uint, WPARAM, LPARAM, LRESULT> Value;

    public WNDPROC(delegate* unmanaged<HWND, uint, WPARAM, LPARAM, LRESULT> value) => Value = value;

    public bool IsNull => Value is null;

    public static implicit operator WNDPROC(delegate* unmanaged<HWND, uint, WPARAM, LPARAM, LRESULT> value)
        => new(value);

    public static implicit operator delegate* unmanaged<HWND, uint, WPARAM, LPARAM, LRESULT>(WNDPROC value)
        => value.Value;
}
#endif
