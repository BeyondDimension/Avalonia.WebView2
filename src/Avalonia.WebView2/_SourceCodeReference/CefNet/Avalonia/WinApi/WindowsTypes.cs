// MIT License Copyright(c) 2020 CefNet
// https://github.com/CefNet/CefNet/blob/103.0.22181.155/CefNet/Windows/WindowsTypes.cs

namespace CefNet.WinApi;

/// <summary>
/// The <see cref="RECT"/> structure defines a rectangle by the coordinates of its upper-left and lower-right corners.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
struct RECT
{
    /// <summary>
    /// Specifies the x-coordinate of the upper-left corner of the rectangle.
    /// </summary>
    public int Left;

    /// <summary>
    /// Specifies the y-coordinate of the upper-left corner of the rectangle.
    /// </summary>
    public int Top;

    /// <summary>
    /// Specifies the x-coordinate of the lower-right corner of the rectangle.
    /// </summary>
    public int Right;

    /// <summary>
    /// Specifies the y-coordinate of the lower-right corner of the rectangle.
    /// </summary>
    public int Bottom;

    /// <summary>
    /// Converts a <see cref="RECT"/> structure to a <see cref="CefRect"/> structure. 
    /// </summary>
    /// <returns>A <see cref="CefRect"/> structure.</returns>
    public CefRect ToCefRect()
    {
        return new CefRect { X = Left, Y = Top, Width = Right - Left, Height = Bottom - Top };
    }

    /// <summary>
    /// Creates a <see cref="RECT"/> structure from the specified <see cref="CefRect"/> structure. 
    /// </summary>
    /// <param name="rect">The <see cref="CefRect"/> to be converted.</param>
    /// <returns>The new <see cref="RECT"/> that this method creates.</returns>
    public static RECT FromCefRect(ref CefRect rect)
    {
        return new RECT { Left = rect.X, Top = rect.Y, Right = rect.X + rect.Width, Bottom = rect.Y + rect.Height };
    }

    /// <summary>
    /// Creates a <see cref="RECT"/> structure from the specified <see cref="CefRect"/> structure. 
    /// </summary>
    /// <param name="rect">The <see cref="CefRect"/> to be converted.</param>
    /// <returns>The new <see cref="RECT"/> that this method creates.</returns>
    public static RECT FromCefRect(CefRect rect)
    {
        return new RECT { Left = rect.X, Top = rect.Y, Right = rect.X + rect.Width, Bottom = rect.Y + rect.Height };
    }
}