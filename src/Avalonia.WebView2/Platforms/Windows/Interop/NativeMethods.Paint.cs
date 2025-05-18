#if WINDOWS || NETFRAMEWORK
namespace MS.Win32;

partial class NativeMethods
{
#if DEBUG || (NO_CSWIN32 && (WINDOWS || NETFRAMEWORK))
    [global::System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
    internal static extern nint BeginPaint(nint hwnd, out PaintStruct lpPaint);

    [global::System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
    internal static extern bool EndPaint(nint hwnd, ref PaintStruct lpPaint);

    //[global::System.Runtime.InteropServices.DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    //public static extern IntPtr CreateWindowExW(WS_EX dwExStyle, [MarshalAs(UnmanagedType.LPWStr)] string? lpClassName, [MarshalAs(UnmanagedType.LPWStr)] string lpWindowName, WS dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);

    //[global::System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
    //[return: MarshalAs(UnmanagedType.Bool)]
    //public static extern bool DestroyWindow(IntPtr hwnd);

    //[global::System.Runtime.InteropServices.DllImport("user32.dll")]
    //public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    public struct Rect
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    public struct PaintStruct
    {
        public IntPtr hdc;
        public bool fErase;
        public Rect rcPaint;
        public bool fRestore;
        public bool fIncUpdate;
        public byte[] rgbReserved;
    }
#endif

    //[Flags]
    //public enum WS : uint
    //{
    //    None = 0,
    //    CLIPCHILDREN = 33554432, // 0x02000000
    //    VISIBLE = 268435456, // 0x10000000
    //    CHILD = 1073741824, // 0x40000000
    //}

    //[Flags]
    //public enum WS_EX : uint
    //{
    //    None = 0,
    //    TRANSPARENT = 32, // 0x00000020
    //}

    public enum WM : uint
    {
        /// <summary>
        /// https://learn.microsoft.com/zh-cn/windows/win32/inputdev/wm-setfocus
        /// </summary>
        SETFOCUS = 7,

        /// <summary>
        /// https://learn.microsoft.com/zh-cn/windows/win32/gdi/wm-paint
        /// </summary>
        PAINT = 15, // 0x0000000F

        WINDOWPOSCHANGING = 0x0046,
        GETOBJECT = 0x003D,
        SHOWWINDOW = 0x0018,
    }
}
#endif