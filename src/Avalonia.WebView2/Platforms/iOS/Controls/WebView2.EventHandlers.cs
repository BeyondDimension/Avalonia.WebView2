#if IOS || MACOS || MACCATALYST
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebKit;

namespace Avalonia.Controls;

partial class WebView2
{
    internal async void NavigationDelegate_DidFinishNavigation(object? sender, string? e)
    {
        try
        {
            var didFinishNavigation = DidFinishNavigationEvent;
            if (didFinishNavigation is null)
            {
                return;
            }
            didFinishNavigation(this, e);
        }
        finally
        {
#if DEBUG
            Console.WriteLine($"NavigationDelegate_DidFinishNavigationEvent {e}");
#endif
            if (!string.IsNullOrEmpty(e))
            {
                var js = HandlerStorageServiceGenerateJSString(StorageService, e);
#if DEBUG
                Console.WriteLine($"NavigationDelegate_DidFinishNavigationEvent js: {js}");
#endif
                if (js != null)
                {
                    await ExecuteScriptAsync(js);
                }
            }
        }
    }
}
#endif
