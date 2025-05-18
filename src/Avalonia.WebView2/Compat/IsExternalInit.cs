// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// https://github.com/dotnet/runtime/blob/main/src/libraries/System.Private.CoreLib/src/System/Runtime/CompilerServices/IsExternalInit.cs
// https://stackoverflow.com/questions/64749385/predefined-type-system-runtime-compilerservices-isexternalinit-is-not-defined
// https://stackoverflow.com/questions/62648189/testing-c-sharp-9-0-in-vs2019-cs0518-isexternalinit-is-not-defined-or-imported
// https://github.com/dotnet/roslyn/issues/45510
// https://docs.microsoft.com/zh-cn/dotnet/api/system.runtime.compilerservices.isexternalinit?view=net-6.0
#if !NETCOREAPP
using System.ComponentModel;

namespace System.Runtime.CompilerServices;

/// <summary>
/// 保留供编译器用于跟踪元数据。 开发人员不应在源代码中使用此类。
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
class IsExternalInit
{
}
#endif