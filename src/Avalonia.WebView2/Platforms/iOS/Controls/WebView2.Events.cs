#if IOS || MACOS || MACCATALYST
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.Controls;
partial class WebView2
{
    public event EventHandler<string?>? DidFinishNavigationEvent;
}
#endif