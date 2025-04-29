#if !WINDOWS

using System;
using Avalonia.Controls;

namespace Avalonia
{
    public static class AvaloniaX11PlatformExtensions
    {
        public static AppBuilder UseX11(this AppBuilder builder)
        {
            throw new PlatformNotSupportedException();
        }
    }
}

#endif
