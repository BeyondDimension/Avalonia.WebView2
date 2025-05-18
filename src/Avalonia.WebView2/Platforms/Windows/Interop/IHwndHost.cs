namespace Avalonia.Controls.Platforms.Windows.Interop;

/// <summary>
/// https://learn.microsoft.com/zh-cn/dotnet/api/system.windows.interop.hwndhost
/// </summary>
partial interface IHwndHost
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
    nint WndProc(nint hwnd, uint msg, nint wParam, nint lParam, ref bool handled);
}