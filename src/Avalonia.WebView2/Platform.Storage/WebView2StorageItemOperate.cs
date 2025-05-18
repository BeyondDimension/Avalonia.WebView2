namespace Avalonia.Platform.Storage;

/// <summary>
/// localStorage 或 sessionStorage 的操作类型
/// </summary>
public enum WebView2StorageItemOperate : byte
{
    /// <summary>
    /// 设置键值项
    /// </summary>
    SetItem = 1,

    /// <summary>
    /// 移除键值项
    /// </summary>
    RemoveItem,

    /// <summary>
    /// 清空所有键值项
    /// </summary>
    ClearAll,
}
