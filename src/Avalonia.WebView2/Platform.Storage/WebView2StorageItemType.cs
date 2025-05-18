namespace Avalonia.Platform.Storage;

/// <summary>
/// storageItem 类型是 localStorage 还是 sessionStorage
/// </summary>
public enum WebView2StorageItemType : byte
{
    /// <summary>
    /// 只读的 localStorage 属性允许你访问一个 Document 源（origin）的对象 Storage；存储的数据将保存在浏览器会话中。localStorage 类似 sessionStorage，但其区别在于：存储在 localStorage 的数据可以长期保留；而当页面会话结束——也就是说，当页面被关闭时，存储在 sessionStorage 的数据会被清除。
    /// <para>https://developer.mozilla.org/zh-CN/docs/Web/API/Window/localStorage</para>
    /// </summary>
    LocalStorage = 1,

    /// <summary>
    /// sessionStorage 属性允许你访问一个，对应当前源的 session Storage 对象。它与 localStorage 相似，不同之处在于 localStorage 里面存储的数据没有过期时间设置，而存储在 sessionStorage 里面的数据在页面会话结束时会被清除。
    /// <para>https://developer.mozilla.org/zh-CN/docs/Web/API/Window/sessionStorage</para>
    /// </summary>
    SessionStorage,

    /// <summary>
    /// <see cref="LocalStorage"/> AND <see cref="SessionStorage"/>
    /// </summary>
    AllStorage,
}
