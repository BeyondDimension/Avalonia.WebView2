using System.Buffers;
using System.Text;
using Utf8StringInterpolation;

namespace Avalonia.Controls;

partial class WebView2
{
    partial class StorageItemValue
    {
        internal static string ToJavaScriptString(
            params IEnumerable<KeyValuePair<(StorageItemType type, string key), StorageItemValue>> pairs)
        {
            using var buffer = Utf8String.CreateWriter(out var writer);
            int i = 0;
            foreach (var it in pairs)
            {
                static bool WriteItemByType(ref Utf8StringWriter<ArrayBufferWriter<byte>> writer, int i, KeyValuePair<(StorageItemType type, string key), StorageItemValue> it)
                {
                    switch (it.Key.type)
                    {
                        case StorageItemType.LocalStorage:
                            writer.AppendFormatted("local"u8);
                            return true;
                        case StorageItemType.SessionStorage:
                            writer.AppendFormatted("session"u8);
                            return true;
                        case StorageItemType.AllStorage:
                            switch (it.Value.operate)
                            {
                                case StorageItemOperate.SetItem:
                                    writer.AppendFormatted("let i"u8);
                                    writer.AppendFormatted(i);
                                    writer.AppendFormatted(" = \""u8);
                                    AppendValue(ref writer, it.Value);
                                    writer.AppendFormatted("\";"u8);
                                    writer.AppendFormatted("localStorage.setItem(\""u8);
                                    AppendKey(ref writer, it.Key.key);
                                    writer.AppendFormatted("\", i"u8);
                                    writer.AppendFormatted(i);
                                    writer.AppendFormatted(");"u8);
                                    writer.AppendFormatted("sessionStorage.setItem(\""u8);
                                    AppendKey(ref writer, it.Key.key);
                                    writer.AppendFormatted("\", i"u8);
                                    writer.AppendFormatted(i);
                                    writer.AppendFormatted(");"u8);
                                    break;
                                case StorageItemOperate.RemoveItem:
                                    writer.AppendFormatted("localStorage.removeItem(\""u8);
                                    AppendJavaScriptStringEncode(ref writer, it.Key.key);
                                    writer.AppendFormatted("\");"u8);
                                    writer.AppendFormatted("sessionStorage.removeItem(\""u8);
                                    AppendJavaScriptStringEncode(ref writer, it.Key.key);
                                    writer.AppendFormatted("\");"u8);
                                    AppendKey(ref writer, it.Key.key);
                                    break;
                                case StorageItemOperate.ClearAll:
                                    writer.AppendFormatted("localStorage.clear();sessionStorage.clear();"u8);
                                    break;
                            }
                            return false;
                    }
                    return false;
                }

                switch (it.Value.operate)
                {
                    case StorageItemOperate.SetItem:
                        if (!WriteItemByType(ref writer, i, it))
                        {
                            continue;
                        }
                        writer.AppendFormatted("Storage.setItem(\""u8);
                        AppendKey(ref writer, it.Key.key);
                        writer.AppendFormatted("\", \""u8);
                        AppendValue(ref writer, it.Value);
                        writer.AppendFormatted("\");"u8);
                        break;
                    case StorageItemOperate.RemoveItem:
                        if (!WriteItemByType(ref writer, i, it))
                        {
                            continue;
                        }
                        writer.AppendFormatted("Storage.removeItem(\""u8);
                        AppendJavaScriptStringEncode(ref writer, it.Key.key);
                        writer.AppendFormatted("\");"u8);
                        break;
                    case StorageItemOperate.ClearAll:
                        if (!WriteItemByType(ref writer, i, it))
                        {
                            continue;
                        }
                        writer.AppendFormatted("Storage.clear();"u8);
                        break;
                }
                i++;
            }
            writer.Flush();
#if NETFRAMEWORK || (NETSTANDARD && !NETSTANDARD2_1_OR_GREATER)
            var buffer2 = ArrayPool<byte>.Shared.Rent(buffer.WrittenSpan.Length);
            try
            {
                buffer.WrittenSpan.CopyTo(buffer2);
                var result = Encoding.UTF8.GetString(buffer2);
                return result;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer2);
            }
#else
            var result = Encoding.UTF8.GetString(buffer.WrittenSpan);
            return result;
#endif
        }
    }
}