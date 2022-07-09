// MIT License Copyright(c) 2020 CefNet
// https://github.com/CefNet/CefNet/blob/103.0.22181.155/CefNet.Windows.Forms/WinApi/DWMWINDOWATTRIBUTE.cs

namespace CefNet.WinApi;

enum DWMWINDOWATTRIBUTE
{
    NCRenderingEnabled = 1,
    NCRenderingPolicy,
    TransitionsForceDisabled,
    AllowNCPaint,
    CaptionButtonBounds,
    NonClientRtlLayout,
    ForceIconicRepresentation,
    Flip3DPolicy,
    ExtendedFrameBounds,
    HasIconicBitmap,
    DisallowPeek,
    ExcludedFromPeek,
    ExceludedFromPeek,
    Cloak,
    Cloaked,
    FreezeRepresentation
}