#if !DISABLE_WEBVIEW2_CORE && (WINDOWS || NETFRAMEWORK)
using System.ComponentModel;
using System.Diagnostics;

namespace Avalonia.Controls;

/// <summary>
/// Tracks the conditions which block implicit initialization and whether it has been requested or not.
/// The analogy is a set of gates which are either open (implicit init allowed) or closed (will have to wait).
/// All sub-gates must be open before implicit init can proceed.
/// If implicit init is requested while the gate is open then it happens immediately.
/// If it's requested while the gate is closed then it occurs when the gate becomes open.
/// 跟踪阻止隐式初始化的条件，以及是否已请求隐式初始化。
/// 这就好比一组大门，要么打开（允许隐式启动），要么关闭（必须等待）。
/// 在进行隐式启动前，所有子门都必须打开。
/// 如果在闸门打开时请求隐式启动，则会立即执行。
/// 如果在闸门关闭时提出请求，则在闸门打开时发生。
/// </summary>
/// <remarks>
/// It should be reasonably straight-forward to expand this class in the future to:
/// * add new sub-gates to further restrict when implicit initialization can occur
/// * support storing and invoking multiple actions next time the gate is open instead of only one
/// 今后，将这一类别扩展到以下方面应该是比较简单的：
/// * 增加新的子门，进一步限制何时可以进行隐式初始化
/// * 支持存储多个操作，并在下一次闸门打开时调用多个操作，而不是只调用一个操作
/// </remarks>
sealed class ImplicitInitGate : ISupportInitialize
{
    /// <summary>
    /// Tracks whether a sub-gate regarding <see cref="BeginInit" />/<see cref="EndInit" /> is open or closed.
    /// This sub-gate is only closed after calls to `BeginInit` and before an equal number of calls to `EndInit`.
    /// 跟踪有关 <see cref="BeginInit" />/<see cref="EndInit" /> 的子门是打开还是关闭。
    /// 只有在调用完 `BeginInit` 和相同次数的 `EndInit` 之前，该子网关才会关闭。
    /// </summary>
    /// <remarks>
    /// We don't want implicit initialization to occur in between those calls,
    /// because implicit initialization is a side effect of setting the Source property,
    /// and side effects of setting properties during that period are supposed to be delayed until `EndInit`.
    /// 我们不希望在这些调用之间发生隐式初始化，因为隐式初始化是设置源属性的副作用，
    /// 在此期间设置属性的副作用应该延迟到 `EndInit` 结束。
    /// </remarks>
    bool ISupportInitializeOpen => ISupportInitializeCount == 0;

    /// <summary>
    /// How many times <see cref="BeginInit" /> has been called without <see cref="EndInit" /> being called.
    /// 在未调用 <see cref="EndInit" /> 的情况下调用 <see cref="BeginInit" /> 的次数。
    /// </summary>
    int ISupportInitializeCount { get; set; }

    /// <summary>
    /// Tracks whether a sub-gate regarding <see cref="SynchronizationContext.Current" /> is open or closed.
    /// This sub-gate is closed if `SynchronizationContext.Current == null`.
    /// 跟踪关于 <see cref="SynchronizationContext.Current" /> 的子门是打开还是关闭。
    /// 如果 `SynchronizationContext.Current == null` 关闭此子网关。
    /// </summary>
    /// <remarks>
    /// Initialization won't work without a `SynchronizationContext` because otherwise an `await` might resume on a different thread.
    /// As far as I know so far this only occurs before an event loop as started on the running thread.
    /// Once there's an event loop running the `SynchronizationContext` ensures that `await`s resume in the same event loop (i.e. same thread).
    /// Although it's a rare corner case, it's possible to create a `Window` w/ `WebView2` before an app's event loop starts.
    /// This sub-gate handles that corner case.
    /// 如果没有 `SynchronizationContext`，初始化将无法进行，否则 `await` 可能会在不同的线程上继续。
    /// 据我所知，目前只有在运行线程启动事件循环之前才会出现这种情况。
    /// 一旦有一个事件循环在运行，`SynchronizationContext` 就会确保 `await` 在同一个事件循环（即同一个线程）中继续运行。
    /// 在应用程序的事件循环开始之前创建一个 `Window` w/ `WebView2` 是有可能的，但这种情况很少见。
    /// 这个分门可以处理这种边角情况。
    /// </remarks>
    static bool SyncContextOpen => SynchronizationContext.Current != null;

    /// <summary>
    /// An action which will trigger initialization next time the gate is open (and only once).
    /// 在下一次闸门打开时触发初始化的操作（仅一次）。
    /// </summary>
    /// <remarks>
    /// This basically tracks whether or not implicit initialization has been requested while the gate is closed.
    /// If this is non-null then it should be a delegate that calls <see cref="WebView2.EnsureCoreWebView2Async" />.
    /// 这主要是跟踪在门关闭时是否有隐式初始化请求。
    /// 如果它不是空值，那么它应该是一个调用 <see cref="WebView2.EnsureCoreWebView2Async"/>
    /// </remarks>
    Action? PendingInitAction { get; set; }

    /// <summary>
    /// Closes the gate until <see cref="EndInit" /> is called an equal number of times.
    /// 关闭栅极，直到 <see cref="EndInit" /> 被调用相同次数。
    /// </summary>
    public void BeginInit()
    {
        ++ISupportInitializeCount;
        OnDataChanged();
    }

    /// <summary>
    /// Opens the gate closed by <see cref="BeginInit" /> after being called the same number of times.
    /// 在被叫到相同次数后，打开 <see cref="BeginInit" /> 关闭的大门。
    /// </summary>
    public void EndInit()
    {
        Trace.Assert(ISupportInitializeCount > 0, "Called EndInit more times than BeginInit was called.");
        --ISupportInitializeCount;
        OnDataChanged();
    }

    /// <summary>
    /// A handler that should be attached to an event which indicates that <see cref="SynchronizationContext.Current" /> exists.
    /// The best one I know of right now is <see cref="Control.OnAttachedToVisualTreeCore(VisualTreeAttachmentEventArgs)" />.
    /// When the handler is called, the gate will re-evaluate its state and potentially allow any pending initialization action.
    /// 指示 <see cref="SynchronizationContext.Current" /> 存在的事件的处理程序。
    /// 据我所知，目前最好的方法是 <see cref="Control.OnAttachedToVisualTreeCore(VisualTreeAttachmentEventArgs)" />。
    /// 当处理程序被调用时，门将重新评估其状态，并可能允许任何待处理的初始化操作。
    /// </summary>
    public void OnSynchronizationContextExists()
    {
        Trace.Assert(SyncContextOpen, "Expected UI thread to have a SynchronizationContext by the time this event fires.");
        OnDataChanged();
    }

    /// <summary>
    /// Run a given action when the gate is open.
    /// 当门打开时，运行给定的操作。
    /// </summary>
    /// <remarks>
    /// If the gate is currently open then the action runs immediately.
    /// Otherwise the action runs next time the gate is discovered to be open.
    /// The action is only ever run once; it will not run again a second/subsequent time the gate opens.
    /// If the gate is closed and another action is already pending then the new action *overwrites* the current one (i.e. the currently stored action will never run).
    /// To "forget" a currently stored action, pass `null`.
    /// 如果闸门当前处于打开状态，则立即执行该操作。
    /// 否则，下一次发现闸门打开时，就会执行该操作。
    /// 该动作只运行一次，在第二次或其后闸门打开时不会再运行。
    /// 如果闸门已关闭，而另一个操作尚未执行，那么新操作将*覆盖*当前操作（即当前存储的操作永远不会执行）。
    /// 要 "遗忘" 当前存储的操作，请输入 `null`。
    /// </remarks>
    /// <param name="initAction">
    /// Action to run when the gate is open, or null to clear a previously specified action.
    /// 闸门打开时要执行的操作，或清空先前指定的操作。
    /// </param>
    public void RunWhenOpen(Action initAction)
    {
        PendingInitAction = initAction;
        OnDataChanged();
    }

    /// <summary>
    /// Examine our overall open/closed state and run any pending action if appropriate.
    /// 检查我们的整体打开/关闭状态，并酌情运行任何待执行的操作。
    /// </summary>
    void OnDataChanged()
    {
        if (PendingInitAction == null || !ISupportInitializeOpen || !SyncContextOpen)
            return;
        PendingInitAction();
        PendingInitAction = null;
    }
}
#endif