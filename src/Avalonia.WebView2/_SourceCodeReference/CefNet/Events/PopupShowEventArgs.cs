// MIT License Copyright(c) 2020 CefNet
// https://github.com/CefNet/CefNet/blob/103.0.22181.155/CefNet/Events/PopupShowEventArgs.cs

namespace CefNet;

sealed class PopupShowEventArgs : EventArgs
{
    public PopupShowEventArgs()
    {
        Visible = false;
    }

    public PopupShowEventArgs(CefRect rect)
    {
        Visible = (rect.Width | rect.Height) != 0;
        Bounds = rect;
    }

    public bool Visible { get; }

    public CefRect Bounds { get; }
}