namespace Avalonia.Controls;

partial class WebView2 : IDisposable
{
    bool disposedValue;
    readonly List<IDisposable> _disposables = [];

    /// <summary>
    /// 验证当前控件没有被释放，被释放后操作将抛出 <see cref="InvalidOperationException"/>
    /// </summary>
    /// <exception cref="InvalidOperationException">当前控件被释放后操作引发</exception>
    void VerifyNotClosedGuard()
    {
        if (disposedValue)
        {
            throw new InvalidOperationException("The instance of CoreWebView2 is disposed and unable to complete this operation.");
        }
    }

    /// <summary>
    /// 是否已释放
    /// </summary>
    protected bool DisposedValue => disposedValue;

    /// <inheritdoc cref="IDisposable.Dispose"/>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // 释放托管状态(托管对象)
#if !DISABLE_WEBVIEW2_CORE && (WINDOWS || NETFRAMEWORK)
                UnsubscribeHandlersAndCloseController();
#endif
#if !(WINDOWS || NETFRAMEWORK) && NET8_0_OR_GREATER && !ANDROID && !IOS && !MACOS && !MACCATALYST && !DISABLE_CEFGLUE
                CefGuleDispose(disposing);
#endif
                _disposables.ForEach(d => d.Dispose());
            }

            // 释放未托管的资源(未托管的对象)并重写终结器
            // 将大型字段设置为 null
            disposedValue = true;
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}