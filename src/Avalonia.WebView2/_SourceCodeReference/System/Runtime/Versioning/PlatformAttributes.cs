// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// https://github.com/dotnet/runtime/blob/v6.0.0/src/libraries/System.Private.CoreLib/src/System/Runtime/Versioning/PlatformAttributes.cs

namespace System.Runtime.Versioning;

#if !NET5_0_OR_GREATER || !NET6_0_OR_GREATER
abstract class OSPlatformCompatAttribute : Attribute
{
    private protected OSPlatformCompatAttribute(string platformName)
    {
        PlatformName = platformName;
    }

    public string PlatformName { get; }
}
#endif

#if !NET5_0_OR_GREATER
/// <summary>
/// Records the operating system (and minimum version) that supports an API. Multiple attributes can be
/// applied to indicate support on multiple operating systems.
/// </summary>
/// <remarks>
/// Callers can apply a <see cref="System.Runtime.Versioning.SupportedOSPlatformAttribute " />
/// or use guards to prevent calls to APIs on unsupported operating systems.
///
/// A given platform should only be specified once.
/// </remarks>
[AttributeUsage(AttributeTargets.Assembly |
                AttributeTargets.Class |
                AttributeTargets.Constructor |
                AttributeTargets.Enum |
                AttributeTargets.Event |
                AttributeTargets.Field |
                AttributeTargets.Interface |
                AttributeTargets.Method |
                AttributeTargets.Module |
                AttributeTargets.Property |
                AttributeTargets.Struct,
                AllowMultiple = true, Inherited = false)]
sealed class SupportedOSPlatformAttribute : OSPlatformCompatAttribute
{
    public SupportedOSPlatformAttribute(string platformName) : base(platformName)
    {
    }
}

///// <summary>
///// Marks APIs that were removed in a given operating system version.
///// </summary>
///// <remarks>
///// Primarily used by OS bindings to indicate APIs that are only available in
///// earlier versions.
///// </remarks>
//[AttributeUsage(AttributeTargets.Assembly |
//                AttributeTargets.Class |
//                AttributeTargets.Constructor |
//                AttributeTargets.Enum |
//                AttributeTargets.Event |
//                AttributeTargets.Field |
//                AttributeTargets.Interface |
//                AttributeTargets.Method |
//                AttributeTargets.Module |
//                AttributeTargets.Property |
//                AttributeTargets.Struct,
//                AllowMultiple = true, Inherited = false)]
//sealed class UnsupportedOSPlatformAttribute : OSPlatformCompatAttribute
//{
//    public UnsupportedOSPlatformAttribute(string platformName) : base(platformName)
//    {
//    }
//}
#endif

#if !NET6_0_OR_GREATER
/// <summary>
/// Annotates a custom guard field, property or method with a supported platform name and optional version.
/// Multiple attributes can be applied to indicate guard for multiple supported platforms.
/// </summary>
/// <remarks>
/// Callers can apply a <see cref="System.Runtime.Versioning.SupportedOSPlatformGuardAttribute " /> to a field, property or method
/// and use that field, property or method in a conditional or assert statements in order to safely call platform specific APIs.
///
/// The type of the field or property should be boolean, the method return type should be boolean in order to be used as platform guard.
/// </remarks>
[AttributeUsage(AttributeTargets.Field |
                AttributeTargets.Method |
                AttributeTargets.Property,
                AllowMultiple = true, Inherited = false)]
sealed class SupportedOSPlatformGuardAttribute : OSPlatformCompatAttribute
{
    public SupportedOSPlatformGuardAttribute(string platformName) : base(platformName)
    {
    }
}
#endif
