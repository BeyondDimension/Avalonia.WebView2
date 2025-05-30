using System.Buffers;
using System.Buffers.Text;
using System.Diagnostics;
using Utf8StringInterpolation;

namespace Avalonia.Controls;

partial class WebView2
{
    partial class StorageItemValue
    {
        // https://github.com/dotnet/runtime/blob/v10.0.0-preview.3.25171.5/src/libraries/System.Text.Json/src/System/Text/Json/Writer/JsonWriterHelper.Date.cs

        const int MaximumFormatDateTimeLength = 27;    // StandardFormat 'O', e.g. 2017-06-12T05:30:45.7680000
        const int MaximumFormatDateTimeOffsetLength = 33;  // StandardFormat 'O', e.g. 2017-06-12T05:30:45.7680000-07:00

        static readonly StandardFormat s_dateTimeStandardFormat = new('O');

        static void WriteDateTimeTrimmed(Utf8StringWriter<ArrayBufferWriter<byte>> writer, DateTime value)
        {
            Span<byte> tempSpan = stackalloc byte[MaximumFormatDateTimeOffsetLength];
            bool result = Utf8Formatter.TryFormat(value, tempSpan, out var bytesWritten, s_dateTimeStandardFormat);
            Debug.Assert(result);
            TrimDateTimeOffset(tempSpan[..bytesWritten], out bytesWritten);
            writer.AppendFormatted(tempSpan[..bytesWritten]);
        }

        static void WriteDateTimeOffsetTrimmed(Utf8StringWriter<ArrayBufferWriter<byte>> writer, DateTimeOffset value)
        {
            Span<byte> tempSpan = stackalloc byte[MaximumFormatDateTimeOffsetLength];
            bool result = Utf8Formatter.TryFormat(value, tempSpan, out var bytesWritten, s_dateTimeStandardFormat);
            Debug.Assert(result);
            TrimDateTimeOffset(tempSpan[..bytesWritten], out bytesWritten);
            writer.AppendFormatted(tempSpan[..bytesWritten]);
        }

        //
        // Trims roundtrippable DateTime(Offset) input.
        // If the milliseconds part of the date is zero, we omit the fraction part of the date,
        // else we write the fraction up to 7 decimal places with no trailing zeros. i.e. the output format is
        // YYYY-MM-DDThh:mm:ss[.s]TZD where TZD = Z or +-hh:mm.
        // e.g.
        //   ---------------------------------
        //   2017-06-12T05:30:45.768-07:00
        //   2017-06-12T05:30:45.00768Z           (Z is short for "+00:00" but also distinguishes DateTimeKind.Utc from DateTimeKind.Local)
        //   2017-06-12T05:30:45                  (interpreted as local time wrt to current time zone)
        static void TrimDateTimeOffset(Span<byte> buffer, out int bytesWritten)
        {
            const int maxDateTimeLength = MaximumFormatDateTimeLength;

            // Assert buffer is the right length for:
            // YYYY-MM-DDThh:mm:ss.fffffff (JsonConstants.MaximumFormatDateTimeLength)
            // YYYY-MM-DDThh:mm:ss.fffffffZ (JsonConstants.MaximumFormatDateTimeLength + 1)
            // YYYY-MM-DDThh:mm:ss.fffffff(+|-)hh:mm (JsonConstants.MaximumFormatDateTimeOffsetLength)
            Debug.Assert(buffer.Length == maxDateTimeLength ||
                buffer.Length == maxDateTimeLength + 1 ||
                buffer.Length == MaximumFormatDateTimeOffsetLength);

            // Find the last significant digit.
            int curIndex;
            if (buffer[maxDateTimeLength - 1] == '0')
                if (buffer[maxDateTimeLength - 2] == '0')
                    if (buffer[maxDateTimeLength - 3] == '0')
                        if (buffer[maxDateTimeLength - 4] == '0')
                            if (buffer[maxDateTimeLength - 5] == '0')
                                if (buffer[maxDateTimeLength - 6] == '0')
                                    if (buffer[maxDateTimeLength - 7] == '0')
                                    {
                                        // All decimal places are 0 so we can delete the decimal point too.
                                        curIndex = maxDateTimeLength - 7 - 1;
                                    }
                                    else { curIndex = maxDateTimeLength - 6; }
                                else { curIndex = maxDateTimeLength - 5; }
                            else { curIndex = maxDateTimeLength - 4; }
                        else { curIndex = maxDateTimeLength - 3; }
                    else { curIndex = maxDateTimeLength - 2; }
                else { curIndex = maxDateTimeLength - 1; }
            else
            {
                // There is nothing to trim.
                bytesWritten = buffer.Length;
                return;
            }

            // We are either trimming a DateTimeOffset, or a DateTime with
            // DateTimeKind.Local or DateTimeKind.Utc
            if (buffer.Length == maxDateTimeLength)
            {
                // There is no offset to copy.
                bytesWritten = curIndex;
            }
            else if (buffer.Length == MaximumFormatDateTimeOffsetLength)
            {
                // We have a non-UTC offset (+|-)hh:mm that are 6 characters to copy.
                buffer[curIndex] = buffer[maxDateTimeLength];
                buffer[curIndex + 1] = buffer[maxDateTimeLength + 1];
                buffer[curIndex + 2] = buffer[maxDateTimeLength + 2];
                buffer[curIndex + 3] = buffer[maxDateTimeLength + 3];
                buffer[curIndex + 4] = buffer[maxDateTimeLength + 4];
                buffer[curIndex + 5] = buffer[maxDateTimeLength + 5];
                bytesWritten = curIndex + 6;
            }
            else
            {
                // There is a single 'Z'. Just write it at the current index.
                Debug.Assert(buffer[maxDateTimeLength] == 'Z');

                buffer[curIndex] = (byte)'Z';
                bytesWritten = curIndex + 1;
            }
        }
    }

}