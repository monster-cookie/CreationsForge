namespace CreationsForge.Services;

/// <summary>Owns one editor-operation admission and its workspace-transition cancellation signal.</summary>
public sealed class WorkspaceEditorOperationLease : IDisposable
{
    /// <summary>The callback that atomically releases this lease from its arbiter.</summary>
    private readonly Action<WorkspaceEditorOperationLease> Release;

    /// <summary>The cancellation source requested by a cancel-and-wait workspace transition.</summary>
    private readonly CancellationTokenSource CancellationSource = new();

    /// <summary>Signals that the editor completed terminal publication and released this lease.</summary>
    private readonly TaskCompletionSource DrainCompletion = new(TaskCreationOptions.RunContinuationsAsynchronously);

    /// <summary>The stable cancellation token retained after the source is disposed.</summary>
    private readonly CancellationToken OperationCancellationToken;

    /// <summary>Tracks idempotent lease release.</summary>
    private int IsDisposedValue;

    /// <summary>Initializes one arbiter-owned editor-operation lease.</summary>
    /// <param name="release">The callback that releases this exact lease.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="release"/> is <see langword="null"/>.</exception>
    internal WorkspaceEditorOperationLease(Action<WorkspaceEditorOperationLease> release)
    {
        ArgumentNullException.ThrowIfNull(release);
        Release = release;
        OperationCancellationToken = CancellationSource.Token;
    }

    /// <summary>Gets the token canceled when an exclusive transition requests cancellation and drain.</summary>
    public CancellationToken CancellationToken => OperationCancellationToken;

    /// <summary>Gets whether this lease still owns editor admission.</summary>
    public bool IsActive => Volatile.Read(ref IsDisposedValue) == 0;

    /// <summary>Gets the terminal-drain signal observed only by the owning arbiter.</summary>
    internal Task DrainTask => DrainCompletion.Task;

    /// <summary>Requests cooperative cancellation without releasing editor admission or allowing observer failures to corrupt arbiter state.</summary>
    internal void RequestCancellation()
    {
        if (!IsActive)
        {
            return;
        }

        try
        {
            CancellationSource.Cancel();
        }
        catch (ObjectDisposedException)
        {
        }
        catch (AggregateException)
        {
        }
    }

    /// <summary>Releases editor admission and publishes terminal drain exactly once.</summary>
    public void Dispose()
    {
        if (Interlocked.Exchange(ref IsDisposedValue, 1) != 0)
        {
            return;
        }

        try
        {
            Release(this);
        }
        finally
        {
            CancellationSource.Dispose();
            DrainCompletion.TrySetResult();
        }
    }
}
