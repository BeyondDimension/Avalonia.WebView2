namespace MS.Win32;

static partial class NativeMethods
{
    [DllImport("user32.dll", SetLastError = true)]
    internal static extern IntPtr BeginPaint(IntPtr hwnd, out PaintStruct lpPaint);

    [DllImport("user32.dll", SetLastError = true)]
    internal static extern bool EndPaint(IntPtr hwnd, ref PaintStruct lpPaint);

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern IntPtr CreateWindowExW(WS_EX dwExStyle, [MarshalAs(UnmanagedType.LPWStr)] string? lpClassName, [MarshalAs(UnmanagedType.LPWStr)] string lpWindowName, WS dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool DestroyWindow(IntPtr hwnd);

    [Flags]
    public enum WS : uint
    {
        None = 0,
        CLIPCHILDREN = 33554432, // 0x02000000
        VISIBLE = 268435456, // 0x10000000
        CHILD = 1073741824, // 0x40000000
    }

    [Flags]
    public enum WS_EX : uint
    {
        None = 0,
        TRANSPARENT = 32, // 0x00000020
    }

    public enum WM : uint
    {
        SETFOCUS = 7,
        PAINT = 15, // 0x0000000F

        WINDOWPOSCHANGING = 0x0046,
        GETOBJECT = 0x003D,
    }

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
}