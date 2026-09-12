namespace CreationsForge.Core.Engine.Persistence;

/// <summary>Serializes output-directory lease acquisition across every provider instance in this process.</summary>
internal static class OutputDirectoryInProcessGate
{
    /// <summary>Protects the process-wide gate table and every entry reference count.</summary>
    private static readonly object SyncRoot = new();

    /// <summary>Tracks canonical output directories with an active owner or waiter.</summary>
    private static readonly Dictionary<string, Entry> Entries = new(
        OperatingSystem.IsWindows() ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);

    /// <summary>Waits asynchronously for exclusive in-process ownership of one canonical output directory.</summary>
    /// <param name="canonicalDirectoryPath">The canonical existing output directory.</param>
    /// <param name="cancellationToken">The token that cancels waiting without leaking a gate entry.</param>
    /// <returns>An idempotent ownership handle that releases the directory gate.</returns>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is requested before ownership is acquired.</exception>
    internal static async ValueTask<Releaser> EnterAsync(
        string canonicalDirectoryPath,
        CancellationToken cancellationToken)
    {
        Entry entry;
        lock (SyncRoot)
        {
            if (!Entries.TryGetValue(canonicalDirectoryPath, out entry!))
            {
                entry = new Entry();
                Entries.Add(canonicalDirectoryPath, entry);
            }

            entry.ReferenceCount++;
        }

        try
        {
            await entry.Semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
            return new Releaser(() => ReleaseReference(
                canonicalDirectoryPath,
                entry,
                releaseSemaphore: true));
        }
        catch
        {
            ReleaseReference(canonicalDirectoryPath, entry, releaseSemaphore: false);
            throw;
        }
    }

    /// <summary>Releases an owner or canceled waiter reference and removes unused gate state.</summary>
    /// <param name="canonicalDirectoryPath">The gate table key.</param>
    /// <param name="entry">The referenced gate entry.</param>
    /// <param name="releaseSemaphore">Whether the caller currently owns the semaphore.</param>
    private static void ReleaseReference(
        string canonicalDirectoryPath,
        Entry entry,
        bool releaseSemaphore)
    {
        lock (SyncRoot)
        {
            if (releaseSemaphore)
            {
                entry.Semaphore.Release();
            }

            entry.ReferenceCount--;
            if (entry.ReferenceCount == 0
                && Entries.TryGetValue(canonicalDirectoryPath, out var current)
                && ReferenceEquals(current, entry))
            {
                Entries.Remove(canonicalDirectoryPath);
                entry.Semaphore.Dispose();
            }
        }
    }

    /// <summary>Owns one semaphore and its table reference.</summary>
    private sealed class Entry
    {
        /// <summary>Gets the exclusive asynchronous semaphore.</summary>
        internal SemaphoreSlim Semaphore { get; } = new(1, 1);

        /// <summary>Gets or sets the number of current owners and waiters.</summary>
        internal int ReferenceCount { get; set; }
    }

    /// <summary>Releases one acquired process-wide directory gate exactly once.</summary>
    internal sealed class Releaser : IDisposable
    {
        /// <summary>The captured release operation, cleared after its first invocation.</summary>
        private Action? release;

        /// <summary>Initializes an acquired gate ownership handle.</summary>
        /// <param name="release">The private gate release operation supplied by the enclosing gate.</param>
        internal Releaser(Action release)
        {
            this.release = release;
        }

        /// <summary>Releases the acquired semaphore and table reference exactly once.</summary>
        public void Dispose()
        {
            Interlocked.Exchange(ref release, null)?.Invoke();
        }
    }
}
