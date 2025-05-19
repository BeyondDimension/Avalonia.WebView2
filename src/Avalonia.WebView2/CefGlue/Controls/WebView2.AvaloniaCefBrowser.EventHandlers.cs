#if !(WINDOWS || NETFRAMEWORK) && NET8_0_OR_GREATER && !ANDROID && !IOS

using Xilium.CefGlue.Common.Events;

namespace Avalonia.Controls;

partial class WebView2
{
    void OnBrowserLoadStart(object sender, LoadStartEventArgs e)
    {
        try
        {
            var contentLoading = ContentLoading;
            if (contentLoading == null)
            {
                return;
            }
            contentLoading(this, e);
        }
        finally
        {
            var requestUri = e.Frame.Url;
            var js = HandlerStorageServiceGenerateJSString(StorageService, requestUri);
            if (js != null)
            {
                EvaluateJavaScript<object>(js);
            }
        }
    }
}
#endif