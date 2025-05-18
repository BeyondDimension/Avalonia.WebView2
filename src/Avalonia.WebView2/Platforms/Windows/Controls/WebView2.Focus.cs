#if WINDOWS || NETFRAMEWORK
using Microsoft.Web.WebView2.Core;

namespace Avalonia.Controls;

partial class WebView2
{
    CoreWebView2MoveFocusReason _lastMoveFocusReason;
}
#endif