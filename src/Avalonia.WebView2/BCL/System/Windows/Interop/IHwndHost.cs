#if WINDOWS
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

#endif

namespace System.Windows.Interop;

interface IHwndHost
{

    /// <summary>
    /// When overridden in a derived class, accesses the window process (handle) of the hosted child window.
    /// </summary>
    /// <param name="hwnd">The window handle of the hosted window.</param>
    /// <param name="msg">The message to act upon.</param>
    /// <param name="wParam">Information that may be relevant to handling the message. This is typically used to store small pieces of information, such as flags.</param>
    /// <param name="lParam">Information that may be relevant to handling the message. This is typically used to reference an object.</param>
    /// <param name="handled">Whether events resulting should be marked handled.</param>
    /// <returns>The window handle of the child window.</returns>
    //IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled);

#if WINDOWS
    LRESULT WndProc(HWND hwnd, uint msg, WPARAM wParam, LPARAM lParam, ref bool handled);
#endif
}
