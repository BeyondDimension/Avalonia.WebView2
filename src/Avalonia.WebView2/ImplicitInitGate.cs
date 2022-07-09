namespace Avalonia.Controls;

/// <summary>
/// Tracks the conditions which block implicit initialization and whether it has been requested or not.
/// The analogy is a set of gates which are either open (implicit init allowed) or closed (will have to wait).
/// All sub-gates must be open before implicit init can proceed.
/// If implicit init is requested while the gate is open then it happens immediately.
/// If it's requested while the gate is closed then it occurs when the gate becomes open.
/// </summary>
/// <remarks>
/// It should be reasonably straight-forward to expand this class in the future to:
/// * add new sub-gates to further restrict when implicit initialization can occur
/// * support storing and invoking multiple actions next time the gate is open instead of only one
/// </remarks>
internal sealed class ImplicitInitGate : ISupportInitialize
{
    /// <summary>
    /// Tracks whether a sub-gate regarding <see cref="BeginInit" />/<see cref="EndInit" /> is open or closed.
    /// This sub-gate is only closed after calls to `BeginInit` and before an equal number of calls to `EndInit`.
    /// </summary>
    /// <remarks>
    /// We don't want implicit initialization to occur in between those calls,
    /// because implicit initialization is a side effect of setting the Source property,
    /// and side effects of setting properties during that period are supposed to be delayed until `EndInit`.
    /// </remarks>
    bool ISupportInitializeOpen => ISupportInitializeCount == 0;

    /// <summary>
    /// How many times <see cref="BeginInit" /> has been called without <see cref="EndInit" /> being called.
    /// </summary>
    int ISupportInitializeCount { get; set; }

    /// <summary>
    /// Tracks whether a sub-gate regarding <see cref="SynchronizationContext.Current" /> is open or closed.
    /// This sub-gate is closed if `SynchronizationContext.Current == null`.
    /// </summary>
    /// <remarks>
    /// Initialization won't work without a `SynchronizationContext` because otherwise an `await` might resume on a different thread.
    /// As far as I know so far this only occurs before an event loop as started on the running thread.
    /// Once there's an event loop running the `SynchronizationContext` ensures that `await`s resume in the same event loop (i.e. same thread).
    /// Although it's a rare corner case, it's possible to create a `Window` w/ `WebView2` before an app's event loop starts.
    /// This sub-gate handles that corner case.
    /// </remarks>
    static bool SyncContextOpen => SynchronizationContext.Current != null;

    /// <summary>
    /// An action which will trigger initialization next time the gate is open (and only once).
    /// </summary>
    /// <remarks>
    /// This basically tracks whether or not implicit initialization has been requested while the gate is closed.
    /// If this is non-null then it should be a delegate that calls <see cref="WebView2.EnsureCoreWebView2Async(CoreWebView2Environment,CoreWebView2ControllerOptions)" />.
    /// </remarks>
    Action? PendingInitAction { get; set; }

    /// <summary>
    /// Closes the gate until <see cref="EndInit" /> is called an equal number of times.
    /// </summary>
    public void BeginInit()
    {
        ++ISupportInitializeCount;
        OnDataChanged();
    }

    /// <summary>
    /// Opens the gate closed by <see cref="BeginInit" /> after being called the same number of times.
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
    /// </summary>
    public void OnSynchronizationContextExists()
    {
        Trace.Assert(ImplicitInitGate.SyncContextOpen, "Expected UI thread to have a SynchronizationContext by the time this event fires.");
        OnDataChanged();
    }

    /// <summary>Run a given action when the gate is open.</summary>
    /// <remarks>
    /// If the gate is currently open then the action runs immediately.
    /// Otherwise the action runs next time the gate is discovered to be open.
    /// The action is only ever run once; it will not run again a second/subsequent time the gate opens.
    /// If the gate is closed and another action is already pending then the new action *overwrites* the current one (i.e. the currently stored action will never run).
    /// To "forget" a currently stored action, pass `null`.
    /// </remarks>
    /// <param name="initAction">Action to run when the gate is open, or null to clear a previously specified action.</param>
    public void RunWhenOpen(Action initAction)
    {
        PendingInitAction = initAction;
        OnDataChanged();
    }

    /// <summary>
    /// Examine our overall open/closed state and run any pending action if appropriate.
    /// </summary>
    void OnDataChanged()
    {
        if (PendingInitAction == null || !ISupportInitializeOpen || !SyncContextOpen)
            return;
        PendingInitAction();
        PendingInitAction = null;
    }
}
