#if !WINDOWS

using System;
using Avalonia.Controls;

namespace Avalonia
{
    public static class AvaloniaNativePlatformExtensions
    {
        public static AppBuilder UseAvaloniaNative(this AppBuilder builder)
        {
            throw new PlatformNotSupportedException();
        }
    }
}

#endif