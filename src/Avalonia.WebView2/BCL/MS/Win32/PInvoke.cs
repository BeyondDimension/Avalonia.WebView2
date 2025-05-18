using System;
using System.Collections.Generic;
using System.Text;
#if WINDOWS
using winmdroot = global::Windows.Win32;

namespace Windows.Win32;

partial class PInvoke
{
    /// <summary>Passes message information to the specified window procedure. (Unicode)</summary>
    /// <param name="lpPrevWndFunc">
    /// <para>Type: <b>WNDPROC</b> The previous window procedure. If this value is obtained by calling the <a href="https://docs.microsoft.com/windows/desktop/api/winuser/nf-winuser-getwindowlonga">GetWindowLong</a> function with the <i>nIndex</i> parameter set to <b>GWL_WNDPROC</b> or <b>DWL_DLGPROC</b>, it is actually either the address of a window or dialog box procedure, or a special internal value meaningful only to <b>CallWindowProc</b>.</para>
    /// <para><see href="https://learn.microsoft.com/windows/win32/api/winuser/nf-winuser-callwindowprocw#parameters">Read more on docs.microsoft.com</see>.</para>
    /// </param>
    /// <param name="hWnd">
    /// <para>Type: <b>HWND</b> A handle to the window procedure to receive the message.</para>
    /// <para><see href="https://learn.microsoft.com/windows/win32/api/winuser/nf-winuser-callwindowprocw#parameters">Read more on docs.microsoft.com</see>.</para>
    /// </param>
    /// <param name="Msg">
    /// <para>Type: <b>UINT</b> The message.</para>
    /// <para><see href="https://learn.microsoft.com/windows/win32/api/winuser/nf-winuser-callwindowprocw#parameters">Read more on docs.microsoft.com</see>.</para>
    /// </param>
    /// <param name="wParam">
    /// <para>Type: <b>WPARAM</b> Additional message-specific information. The contents of this parameter depend on the value of the <i>Msg</i> parameter.</para>
    /// <para><see href="https://learn.microsoft.com/windows/win32/api/winuser/nf-winuser-callwindowprocw#parameters">Read more on docs.microsoft.com</see>.</para>
    /// </param>
    /// <param name="lParam">
    /// <para>Type: <b>LPARAM</b> Additional message-specific information. The contents of this parameter depend on the value of the <i>Msg</i> parameter.</para>
    /// <para><see href="https://learn.microsoft.com/windows/win32/api/winuser/nf-winuser-callwindowprocw#parameters">Read more on docs.microsoft.com</see>.</para>
    /// </param>
    /// <returns>
    /// <para>Type: <b>LRESULT</b> The return value specifies the result of the message processing and depends on the message sent.</para>
    /// </returns>
    /// <remarks>
    /// <para>Use the <b>CallWindowProc</b> function for window subclassing. Usually, all windows with the same class share one window procedure. A subclass is a window or set of windows with the same class whose messages are intercepted and processed by another window procedure (or procedures) before being passed to the window procedure of the class. The <a href="https://docs.microsoft.com/windows/desktop/api/winuser/nf-winuser-setwindowlonga">SetWindowLong</a> function creates the subclass by changing the window procedure associated with a particular window, causing the system to call the new window procedure instead of the previous one. An application must pass any messages not processed by the new window procedure to the previous window procedure by calling <b>CallWindowProc</b>. This allows the application to create a chain of window procedures. If <b>STRICT</b> is defined, the <i>lpPrevWndFunc</i> parameter has the data type <b>WNDPROC</b>. The <b>WNDPROC</b> type is declared as follows:</para>
    /// <para></para>
    /// <para>This doc was truncated.</para>
    /// <para><see href="https://learn.microsoft.com/windows/win32/api/winuser/nf-winuser-callwindowprocw#">Read more on docs.microsoft.com</see>.</para>
    /// </remarks>
    [DllImport("USER32.dll", ExactSpelling = false, EntryPoint = "CallWindowProc"), DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    [SupportedOSPlatform("windows5.0")]
    internal static extern winmdroot.Foundation.LRESULT CallWindowProcNotW(winmdroot.UI.WindowsAndMessaging.WNDPROC lpPrevWndFunc, winmdroot.Foundation.HWND hWnd, uint Msg, winmdroot.Foundation.WPARAM wParam, winmdroot.Foundation.LPARAM lParam);
}

#endif
