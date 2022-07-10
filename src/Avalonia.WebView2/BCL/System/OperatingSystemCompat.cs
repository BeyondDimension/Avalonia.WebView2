#if !NET5_0_OR_GREATER

// https://github.com/BeyondDimension/OperatingSystem2/blob/2.0.0/OperatingSystem2.windows.cs

namespace System;

/// <inheritdoc cref="OperatingSystem"/>
public static class OperatingSystemCompat
{
    /// <summary>
    /// Indicates whether the current application is running on Windows.
    /// </summary>
    /// <returns><see langword="true"/> if the current application is running on Windows; <see langword="false"/> otherwise.</returns>
    [SupportedOSPlatformGuard("Windows")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsWindows() =>
#if WINDOWS
        true;
#elif NET5_0_OR_GREATER
        S_OperatingSystem.IsWindows();
#else
        RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
#endif
}
#endif
