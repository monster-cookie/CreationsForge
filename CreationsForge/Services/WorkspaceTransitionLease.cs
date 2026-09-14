namespace CreationsForge.Services;

/// <summary>Owns one exclusive workspace-transition reservation independently of any presentation scope.</summary>
public sealed class WorkspaceTransitionLease : IDisposable
{
    /// <summary>The callback that atomically releases this lease from its arbiter.</summary>
    private readonly Action<WorkspaceTransitionLease> Release;

    /// <summary>Tracks idempotent lease release.</summary>
    private int IsDisposedValue;

    /// <summary>Initializes one arbiter-owned workspace-transition lease.</summary>
    /// <param name="release">The callback that releases this exact lease.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="release"/> is <see langword="null"/>.</exception>
    internal WorkspaceTransitionLease(Action<WorkspaceTransitionLease> release)
    {
        ArgumentNullException.ThrowIfNull(release);
        Release = release;
    }

    /// <summary>Gets whether this lease still owns exclusive workspace-transition admission.</summary>
    public bool IsActive => Volatile.Read(ref IsDisposedValue) == 0;

    /// <summary>Releases exclusive transition admission exactly once.</summary>
    public void Dispose()
    {
        if (Interlocked.Exchange(ref IsDisposedValue, 1) != 0)
        {
            return;
        }

        Release(this);
    }
}
