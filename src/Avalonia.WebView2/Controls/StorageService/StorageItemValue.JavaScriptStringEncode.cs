using System.Buffers;
using Utf8StringInterpolation;

namespace Avalonia.Controls;

partial class WebView2
{
    partial class StorageItemValue
    {
#if NET8_0_OR_GREATER
        static readonly SearchValues<char> s_invalidJavaScriptChars = SearchValues.Create(
                   // Any Control, < 32 (' ')
                   "\u0000\u0001\u0002\u0003\u0004\u0005\u0006\u0007\u0008\u0009\u000A\u000B\u000C\u000D\u000E\u000F\u0010\u0011\u0012\u0013\u0014\u0015\u0016\u0017\u0018\u0019\u001A\u001B\u001C\u001D\u001E\u001F" +
                   // Chars which must be encoded per JSON spec / HTML-sensitive chars encoded for safety
                   "\"&'<>\\" +
                   // newline chars (see Unicode 6.2, Table 5-1 [http://www.unicode.org/versions/Unicode6.2.0/ch05.pdf]) have to be encoded
                   "\u0085\u2028\u2029");

        /// <summary>
        /// https://github.com/dotnet/runtime/blob/v10.0.0-preview.3.25171.5/src/libraries/System.Web.HttpUtility/src/System/Web/Util/HttpEncoder.cs#L129
        /// </summary>
        protected static void AppendJavaScriptStringEncode(ref Utf8StringWriter<ArrayBufferWriter<byte>> writer, ReadOnlySpan<char> value, bool addDoubleQuotes = false)
        {
            int i = value.IndexOfAny(s_invalidJavaScriptChars);
            if (i < 0)
            {
                if (addDoubleQuotes)
                {
                    writer.AppendFormatted("\""u8);
                    writer.AppendFormatted(value);
                    writer.AppendFormatted("\""u8);
                }
                else
                {
                    writer.AppendFormatted(value);
                }
                return;
            }

            // EncodeCore

            if (addDoubleQuotes)
            {
                writer.AppendFormatted("\""u8);
            }

            ReadOnlySpan<char> chars = value;
            do
            {
                writer.AppendFormatted(chars[..i]);
                char c = chars[i];
                chars = chars[(i + 1)..];
                switch (c)
                {
                    case '\r':
                        writer.AppendFormatted("\\r"u8);
                        break;
                    case '\t':
                        writer.AppendFormatted("\\t"u8);
                        break;
                    case '\"':
                        writer.AppendFormatted("\\\""u8);
                        break;
                    case '\\':
                        writer.AppendFormatted("\\\\"u8);
                        break;
                    case '\n':
                        writer.AppendFormatted("\\n"u8);
                        break;
                    case '\b':
                        writer.AppendFormatted("\\b"u8);
                        break;
                    case '\f':
                        writer.AppendFormatted("\\f"u8);
                        break;
                    default:
                        writer.AppendFormatted("\\u"u8);
                        writer.AppendFormatted((int)c, 0, "x4");
                        break;
                }

                i = chars.IndexOfAny(s_invalidJavaScriptChars);
            } while (i >= 0);

            writer.AppendFormatted(chars);

            if (addDoubleQuotes)
            {
                writer.AppendFormatted("\""u8);
            }
        }
#else
        protected static void AppendJavaScriptStringEncode(ref Utf8StringWriter<ArrayBufferWriter<byte>> writer, string? value)
        {
            if (value == null)
            {
                writer.AppendFormatted("null"u8);
            }
            else
            {
                var encodeValue = global::System.Web.HttpUtility.JavaScriptStringEncode(value);
                writer.Append(encodeValue);
            }
        }
#endif
    }

}