using Microsoft.IO;
using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Utf8StringInterpolation;

namespace Avalonia.Platform.Storage;

/// <summary>
/// localStorage 或 sessionStorage 项的值类型
/// <para>生成示例：</para>
/// <list type="bullet">
/// <item>var dict = new Dictionary&lt;(WebView2StorageItemType type, string key), WebView2StorageItemValue&gt;</item>
/// <item>{</item>
/// <item>    { (WebView2StorageItemType.LocalStorage, "key1"), 2 },</item>
/// <item>    { (WebView2StorageItemType.LocalStorage, "key2"), "3" },</item>
/// <item>    { (WebView2StorageItemType.LocalStorage, "key3"), 4.5f },</item>
/// <item>    { (WebView2StorageItemType.LocalStorage, "key4"), EnvironmentVariableTarget.Machine },</item>
/// <item>    { (WebView2StorageItemType.LocalStorage, ""), LocalStorageItemOperate.RemoveItem },</item>
/// <item>    { (WebView2StorageItemType.LocalStorage, ""), LocalStorageItemOperate.ClearAll },</item>
/// <item>    { (WebView2StorageItemType.LocalStorage, "key5"), new WebView2StorageItemValue(jsonModelObj) },</item>
/// <item>};</item>
/// <item>var js = dict.ToJavaScriptString();</item>
/// </list>
/// </summary>
public partial class WebView2StorageItemValue
{
    /// <inheritdoc cref="WebView2StorageItemOperate"/>
    protected WebView2StorageItemOperate operate;

    /// <inheritdoc cref="TypeCode"/>
    protected TypeCode typeCode;

    string? stringValue;
    bool boolValue;
    char charValue;
    sbyte sbyteValue;
    byte byteValue;
    short int16Value;
    ushort uint16Value;
    int int32Value;
    uint uint32Value;
    long int64Value;
    ulong uint64Value;
    float floatValue;
    double doubleValue;
    decimal decimalValue;
    DateTime dateTimeValue;
    DateTimeOffset dateTimeOffsetValue;
#if NET6_0_OR_GREATER
    DateOnly dateOnlyValue;
#endif
    bool? dateType_IsTrue2Time_IsFalse2TimeOffset_IsNull2Only;

    protected virtual void Append(ref Utf8StringWriter<ArrayBufferWriter<byte>> writer)
    {
        switch (typeCode)
        {
            case TypeCode.String:
                if (stringValue == null)
                    writer.AppendFormatted("null"u8);
                else
                    AppendJavaScriptStringEncode(ref writer, stringValue);
                break;
            case TypeCode.Boolean:
                writer.AppendFormatted(boolValue ? "true"u8 : "false"u8);
                break;
            case TypeCode.Char:
                writer.Append(charValue);
                break;
            case TypeCode.SByte:
                writer.AppendFormatted(sbyteValue);
                break;
            case TypeCode.Byte:
                writer.AppendFormatted(byteValue);
                break;
            case TypeCode.Int16:
                writer.AppendFormatted(int16Value);
                break;
            case TypeCode.UInt16:
                writer.AppendFormatted(uint16Value);
                break;
            case TypeCode.Int32:
                writer.AppendFormatted(int32Value);
                break;
            case TypeCode.UInt32:
                writer.AppendFormatted(uint32Value);
                break;
            case TypeCode.Int64:
                writer.AppendFormatted(int64Value);
                break;
            case TypeCode.UInt64:
                writer.AppendFormatted(uint64Value);
                break;
            case TypeCode.Single:
                writer.AppendFormatted(floatValue);
                break;
            case TypeCode.Double:
                writer.AppendFormatted(doubleValue);
                break;
            case TypeCode.Decimal:
                writer.AppendFormatted(decimalValue);
                break;
            case TypeCode.DateTime:
                if (!dateType_IsTrue2Time_IsFalse2TimeOffset_IsNull2Only.HasValue)
                {
#if NET6_0_OR_GREATER
                    const int FormatLength = 10; // YYYY-MM-DD
#if NET8_0_OR_GREATER
                    // https://github.com/dotnet/runtime/blob/v10.0.0-preview.3.25171.5/src/libraries/System.Text.Json/src/System/Text/Json/Serialization/Converters/Value/DateOnlyConverter.cs#L58
                    Span<byte> buffer = stackalloc byte[FormatLength];
#else
                    Span<char> buffer = stackalloc char[FormatLength];
#endif
                    bool formattedSuccessfully = dateOnlyValue.TryFormat(buffer, out int charsWritten, "O", CultureInfo.InvariantCulture);
                    Debug.Assert(formattedSuccessfully && charsWritten == FormatLength);
                    writer.AppendFormatted(buffer);
#endif
                }
                else if (dateType_IsTrue2Time_IsFalse2TimeOffset_IsNull2Only.Value)
                {
                    WriteDateTimeTrimmed(writer, dateTimeValue);
                }
                else
                {
                    WriteDateTimeOffsetTrimmed(writer, dateTimeOffsetValue);
                }
                break;
        }
    }

    /// <summary>
    /// 将多个 localStorage 键值对转换为 JavaScript 字符串
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        var result = ToJavaScriptString(new KeyValuePair<(WebView2StorageItemType type, string key), WebView2StorageItemValue>((WebView2StorageItemType.LocalStorage, ""), this));
        return result;
    }
}

partial class WebView2StorageItemValue // implicit operator
{
    public static implicit operator WebView2StorageItemValue(WebView2StorageItemOperate operate)
        => new() { operate = operate };

    public static implicit operator WebView2StorageItemValue(string value)
        => new() { stringValue = value, typeCode = TypeCode.String, operate = value == null ? WebView2StorageItemOperate.RemoveItem : WebView2StorageItemOperate.SetItem, };

    public static implicit operator WebView2StorageItemValue(bool value)
        => new() { boolValue = value, typeCode = TypeCode.Boolean, operate = WebView2StorageItemOperate.SetItem, };

    public static implicit operator WebView2StorageItemValue(char value)
        => new() { charValue = value, typeCode = TypeCode.Char, operate = WebView2StorageItemOperate.SetItem, };

    public static implicit operator WebView2StorageItemValue(sbyte value)
        => new() { sbyteValue = value, typeCode = TypeCode.SByte, operate = WebView2StorageItemOperate.SetItem, };

    public static implicit operator WebView2StorageItemValue(byte value)
        => new() { byteValue = value, typeCode = TypeCode.Byte, operate = WebView2StorageItemOperate.SetItem, };

    public static implicit operator WebView2StorageItemValue(short value)
        => new() { int16Value = value, typeCode = TypeCode.Int16, operate = WebView2StorageItemOperate.SetItem, };

    public static implicit operator WebView2StorageItemValue(ushort value)
        => new() { uint16Value = value, typeCode = TypeCode.UInt16, operate = WebView2StorageItemOperate.SetItem, };

    public static implicit operator WebView2StorageItemValue(int value)
        => new() { int32Value = value, typeCode = TypeCode.Int32, operate = WebView2StorageItemOperate.SetItem, };

    public static implicit operator WebView2StorageItemValue(uint value)
        => new() { uint32Value = value, typeCode = TypeCode.UInt32, operate = WebView2StorageItemOperate.SetItem, };

    public static implicit operator WebView2StorageItemValue(long value)
        => new() { int64Value = value, typeCode = TypeCode.Int64, operate = WebView2StorageItemOperate.SetItem, };

    public static implicit operator WebView2StorageItemValue(ulong value)
        => new() { uint64Value = value, typeCode = TypeCode.UInt64, operate = WebView2StorageItemOperate.SetItem, };

    public static implicit operator WebView2StorageItemValue(nint value)
        => new() { int64Value = value, typeCode = TypeCode.Int64, operate = WebView2StorageItemOperate.SetItem, };

    public static implicit operator WebView2StorageItemValue(nuint value)
        => new() { uint64Value = value, typeCode = TypeCode.UInt64, operate = WebView2StorageItemOperate.SetItem, };

    public static implicit operator WebView2StorageItemValue(float value)
        => new() { floatValue = value, typeCode = TypeCode.Single, operate = WebView2StorageItemOperate.SetItem, };

    public static implicit operator WebView2StorageItemValue(double value)
        => new() { doubleValue = value, typeCode = TypeCode.Double, operate = WebView2StorageItemOperate.SetItem, };

    public static implicit operator WebView2StorageItemValue(decimal value)
        => new() { decimalValue = value, typeCode = TypeCode.Decimal, operate = WebView2StorageItemOperate.SetItem, };

    public static implicit operator WebView2StorageItemValue(DateTime value)
        => new() { dateTimeValue = value, typeCode = TypeCode.DateTime, operate = WebView2StorageItemOperate.SetItem, dateType_IsTrue2Time_IsFalse2TimeOffset_IsNull2Only = true, };

    public static implicit operator WebView2StorageItemValue(DateTimeOffset value)
        => new() { dateTimeOffsetValue = value, typeCode = TypeCode.DateTime, operate = WebView2StorageItemOperate.SetItem, dateType_IsTrue2Time_IsFalse2TimeOffset_IsNull2Only = false, };

    public static implicit operator WebView2StorageItemValue(Enum value)
        => Convert.ToInt64(value);

#if NET6_0_OR_GREATER
    public static implicit operator WebView2StorageItemValue(DateOnly value)
        => new() { dateOnlyValue = value, typeCode = TypeCode.DateTime, operate = WebView2StorageItemOperate.SetItem, dateType_IsTrue2Time_IsFalse2TimeOffset_IsNull2Only = null, };
#endif
}

partial class WebView2StorageItemValue // static
{
    protected static readonly RecyclableMemoryStreamManager m = new();

    static void AppendKey(ref Utf8StringWriter<ArrayBufferWriter<byte>> writer, string? key)
    {
        if (key == null)
        {
            writer.AppendFormatted("null"u8);
        }
        else
        {
            AppendJavaScriptStringEncode(ref writer, key);
        }
    }

    static void AppendValue(ref Utf8StringWriter<ArrayBufferWriter<byte>> writer, WebView2StorageItemValue? value)
    {
        if (value == null)
        {
            writer.AppendFormatted("null"u8);
        }
        else
        {
            value.Append(ref writer);
        }
    }
}

/// <inheritdoc cref="WebView2StorageItemValue"/>
public sealed class LocalStorageItemValue<TValue> : WebView2StorageItemValue where TValue : notnull
{
    readonly TValue value;
    readonly JsonSerializerOptions? options;
    readonly JsonTypeInfo<TValue>? jsonTypeInfo;

#if NET7_0_OR_GREATER
    /// <summary>
    /// https://github.com/dotnet/runtime/blob/v8.0.0-rc.1.23419.4/src/libraries/System.Text.Json/src/System/Text/Json/Serialization/JsonSerializer.Helpers.cs#L13
    /// 映射序列化未引用的代码消息
    /// </summary>
    internal const string SerializationUnreferencedCodeMessage = "JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.";

    /// <summary>
    /// 序列化要求动态代码消息
    /// </summary>
    internal const string SerializationRequiresDynamicCodeMessage = "JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.";
#endif

#if NET7_0_OR_GREATER
    [RequiresUnreferencedCode(SerializationUnreferencedCodeMessage)]
    [RequiresDynamicCode(SerializationRequiresDynamicCodeMessage)]
#endif
    public LocalStorageItemValue(TValue value) : this(value, JsonSerializerOptions.Web)
    {
    }

#if NET7_0_OR_GREATER
    [RequiresUnreferencedCode(SerializationUnreferencedCodeMessage)]
    [RequiresDynamicCode(SerializationRequiresDynamicCodeMessage)]
#endif
    public LocalStorageItemValue(TValue value, JsonSerializerOptions options)
    {
        this.value = value;
        this.options = options;
        typeCode = TypeCode.Object;
    }

    public LocalStorageItemValue(TValue value, JsonTypeInfo<TValue> jsonTypeInfo)
    {
        this.value = value;
        this.jsonTypeInfo = jsonTypeInfo;
        typeCode = TypeCode.Object;
    }

    protected override void Append(ref Utf8StringWriter<ArrayBufferWriter<byte>> writer)
    {
        if (value is null)
        {
            writer.AppendFormatted("null"u8);
            return;
        }

        using var stream = m.GetStream();
        if (jsonTypeInfo != null)
        {
            JsonSerializer.Serialize(stream, value, jsonTypeInfo);
        }
        else
        {
            // 已由构造函数设置 RequiresXYZCode attr
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
            JsonSerializer.Serialize(stream, value, options);
#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
        }

#if NETFRAMEWORK || (NETSTANDARD && !NETSTANDARD2_1_OR_GREATER)
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var jsonString = reader.ReadToEnd();
        AppendJavaScriptStringEncode(ref writer, jsonString);
#else
        var len = stream.Length;
        if (len > int.MaxValue)
        {
            return;
        }
        else
        {
            var charCount = Encoding.UTF8.GetMaxCharCount(unchecked((int)len));
            var temp = ArrayPool<char>.Shared.Rent(charCount);
            try
            {
                var tempSpan = temp.AsSpan();
#if NET8_0_OR_GREATER
                if (Encoding.UTF8.TryGetChars(stream.GetSpan(), tempSpan, out var charsWritten))
                {
                    tempSpan = tempSpan[..charsWritten];
                    AppendJavaScriptStringEncode(ref writer, tempSpan);
                }
#else
                var charsWritten = Encoding.UTF8.GetChars(stream.GetReadOnlySequence(), tempSpan);
                tempSpan = tempSpan[..charsWritten];
                AppendJavaScriptStringEncode(ref writer, new string(tempSpan));
#endif
            }
            finally
            {
                ArrayPool<char>.Shared.Return(temp);
            }
        }
#endif
    }
}