// MIT License Copyright(c) 2020 CefNet
// https://github.com/CefNet/CefNet/blob/103.0.22181.155/CefNet.Avalonia/Internal/OffscreenGraphics.cs

namespace CefNet.Internal;

sealed class OffscreenGraphics
{
    class PixelBuffer : IDisposable
    {
        public byte[]? DIB;

        readonly List<CefRect> _dirtyRects = new();

        public int Width;

        public int Height;

        public WriteableBitmap? Surface;

        public PixelBuffer(int width, int height)
        {
            Width = width;
            Height = height;
            DIB = ArrayPool<byte>.Shared.Rent(width * height * 4);
        }

        ~PixelBuffer()
        {
            Dispose(false);
        }

        void Dispose(bool disposing)
        {
            var buffer = Interlocked.Exchange(ref DIB, null);
            if (buffer != null)
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public int Stride
        {
            get { return Width * 4; }
        }

        public int Size
        {
            get
            {
                return Width * Height * 4;
            }
        }

        public void AddDirtyRects(CefRect[] dirtyRects)
        {
            _dirtyRects.AddRange(dirtyRects);
        }

        //public Int32Rect GetDirtyRectangle()
        //{
        //	if (_dirtyRects.Count == 0)
        //		return new Int32Rect();
        //	CefRect r = _dirtyRects[0];
        //	Int32Rect dirtyRect = new Int32Rect(r.X, r.Y, r.Width, r.Height);
        //	for (int i = 1; i < _dirtyRects.Count; i++)
        //	{
        //		dirtyRect.Union(_dirtyRects[i]);
        //	}
        //	return dirtyRect;
        //}

        public void ClearDirtyRectangle()
        {
            _dirtyRects.Clear();
        }
    }

    PixelBuffer? ViewPixels;
    PixelBuffer? PopupPixels;

    readonly object _syncRoot;
    CefRect _bounds;
    CefRect _popupBounds;

    public OffscreenGraphics()
    {
        _syncRoot = new object();
        _bounds = new CefRect(0, 0, 1, 1);
    }

    public static DpiScale DpiScale { get; set; } = new DpiScale(1, 1);

    public void SetLocation(int x, int y)
    {
        _bounds.X = x;
        _bounds.Y = y;
    }

    public bool SetSize(int width, int height)
    {
        width = Math.Max(width, 1);
        height = Math.Max(height, 1);
        _bounds.Width = width;
        _bounds.Height = height;

        lock (_syncRoot)
        {
            return ViewPixels == null || ViewPixels.Width != width || ViewPixels.Height != height;
        }
    }

    public CefRect GetBounds()
    {
        return _bounds;
    }

    public void Draw(CefPaintEventArgs e)
    {
        lock (_syncRoot)
        {
            PixelBuffer pixelBuffer;
            if (e.PaintElementType == CefPaintElementType.View)
            {
                if (ViewPixels == null || ViewPixels.Width != e.Width || ViewPixels.Height != e.Height)
                {
                    if (ViewPixels != null)
                        ViewPixels.Dispose();

                    ViewPixels = new PixelBuffer(e.Width, e.Height);
                }
                pixelBuffer = ViewPixels!;
            }
            else if (e.PaintElementType == CefPaintElementType.Popup)
            {
                if (PopupPixels == null || PopupPixels.Width != e.Width || PopupPixels.Height != e.Height)
                {
                    if (PopupPixels != null)
                        PopupPixels.Dispose();
                    PopupPixels = new PixelBuffer(e.Width, e.Height);
                }
                pixelBuffer = PopupPixels!;
            }
            else
            {
                return;
            }

            Marshal.Copy(e.Buffer, pixelBuffer.DIB!, 0, pixelBuffer.Size);
            pixelBuffer.AddDirtyRects(e.DirtyRects);
        }
    }

    public unsafe void Render(DrawingContext drawingContext)
    {
        lock (_syncRoot)
        {
            if (ViewPixels != null)
            {
                PixelSize pixelSize;
                WriteableBitmap surface;

                surface = GetSurface(ViewPixels);
                pixelSize = surface.PixelSize;
                drawingContext.DrawImage(surface, new Rect(0, 0, pixelSize.Width, pixelSize.Height), new Rect(surface.Size));

                var pixelBuffer = PopupPixels;
                if (pixelBuffer == null)
                    return;

                surface = GetSurface(pixelBuffer);
                var size = surface.Size;
                pixelSize = surface.PixelSize;
                drawingContext.DrawImage(surface,
                    new Rect(0, 0, pixelSize.Width, pixelSize.Height),
                    new Rect(_popupBounds.X, _popupBounds.Y, size.Width, size.Height));
            }
        }
    }

    WriteableBitmap GetSurface(PixelBuffer pixelBuffer)
    {
        if (!Monitor.IsEntered(_syncRoot))
            throw new InvalidOperationException();

        var surface = pixelBuffer.Surface;
        if (surface is null || surface.PixelSize != new PixelSize(pixelBuffer.Width, pixelBuffer.Height))
        {
            surface?.Dispose();
            surface = new WriteableBitmap(new PixelSize(pixelBuffer.Width, pixelBuffer.Height), DpiScale.Dpi, PixelFormat.Bgra8888, AlphaFormat.Premul);
            pixelBuffer.Surface = surface;
        }

        using (ILockedFramebuffer frameBuffer = surface.Lock())
        {
            Marshal.Copy(pixelBuffer.DIB!, 0, frameBuffer.Address, pixelBuffer.Size);
            pixelBuffer.ClearDirtyRectangle();
        }
        return surface;
    }

    public void SetPopup(PopupShowEventArgs e)
    {
        if (e.Visible)
        {
            _popupBounds = e.Bounds;
        }
        else
        {
            lock (_syncRoot)
            {
                PopupPixels?.Dispose();
                PopupPixels = null;
            }
        }
    }
}