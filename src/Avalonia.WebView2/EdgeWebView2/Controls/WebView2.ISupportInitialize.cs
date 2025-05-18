#if !DISABLE_WEBVIEW2_CORE && (WINDOWS || NETFRAMEWORK)
using System.ComponentModel;

namespace Avalonia.Controls;

partial class WebView2 : ISupportInitialize
{
    readonly ImplicitInitGate _implicitInitGate = new();

    /// <inheritdoc/>
    public override void BeginInit()
    {
        base.BeginInit();
        _implicitInitGate.BeginInit();
    }

    /// <inheritdoc/>
    public override void EndInit()
    {
        _implicitInitGate.EndInit();
        base.EndInit();
    }
}
#endif