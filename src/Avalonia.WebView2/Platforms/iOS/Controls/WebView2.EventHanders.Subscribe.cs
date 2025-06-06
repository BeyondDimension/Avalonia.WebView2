#if IOS || MACOS || MACCATALYST
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.Controls;
partial class WebView2
{
    /// <summary>
    /// 订阅 WebView2 控件的事件处理程序
    /// </summary>
    void SubscribeHandlers()
    {
        //this.DidFinishNavigationEvent += OnDidFinishNavigation;
    }

    /// <summary>
    /// 取消订阅 WebView2 控件的事件处理程序
    /// </summary>
    void UnsubscribeHandlers()
    {
        //this.DidFinishNavigation -= OnDidFinishNavigation;
    }
}
#endif
