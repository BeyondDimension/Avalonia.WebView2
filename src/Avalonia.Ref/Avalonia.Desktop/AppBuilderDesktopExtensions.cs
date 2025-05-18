using Avalonia.Controls;
using Avalonia.Platform;

// https://github.com/AvaloniaUI/Avalonia/blob/0.10.13/src/Avalonia.Desktop/AppBuilderDesktopExtensions.cs

namespace Avalonia
{
    public static class AppBuilderDesktopExtensions
    {
        public static AppBuilder UsePlatformDetect(this AppBuilder builder)
        {
#if WINDOWS
            builder.UseWin32();
#else

            // We don't have the ability to load every assembly right now, so we are
            // stuck with manual configuration  here
            // Helpers are extracted to separate methods to take the advantage of the fact
            // that CLR doesn't try to load dependencies before referencing method is jitted
            // Additionally, by having a hard reference to each assembly,
            // we verify that the assemblies are in the final .deps.json file
            //  so .NET Core knows where to load the assemblies from,.

            if (OperatingSystem.IsWindows())
            {
                builder.UseWin32();
            }
            else if (OperatingSystem.IsMacOS())
            {
                builder.UseAvaloniaNative();
            }
            else
            {
                builder.UseX11();
            }
#endif
            builder.UseSkia();
            return builder;
        }
    }
}