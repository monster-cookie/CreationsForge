using System.ComponentModel;
using System.Runtime.InteropServices;

namespace CreationsForge.Engine.Workspaces;

/// <summary>Owns the operating-system file handles and advisory locks held for one workspace lifetime.</summary>
internal sealed class WorkspaceFileLockSet : IDisposable
{
    private readonly List<WorkspaceFileLock> _locks;
    private bool _disposed;

    private WorkspaceFileLockSet(List<WorkspaceFileLock> locks)
    {
        _locks = locks;
    }

    /// <summary>Acquires all requested paths in deterministic absolute-path order.</summary>
    /// <param name="requests">The complete source and output lock set.</param>
    /// <returns>The acquired lifetime lock set.</returns>
    public static WorkspaceFileLockSet Acquire(IEnumerable<WorkspaceFileLockRequest> requests)
    {
        ArgumentNullException.ThrowIfNull(requests);

        var comparer = OperatingSystem.IsWindows() ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;
        var requestArray = requests.ToArray();
        foreach (var request in requestArray.Where(request => request.CreateIfMissing))
        {
            if (File.Exists(request.Path))
            {
                continue;
            }

            try
            {
                using var created = new FileStream(
                    request.Path,
                    FileMode.OpenOrCreate,
                    FileAccess.ReadWrite,
                    FileShare.ReadWrite | FileShare.Delete);
            }
            catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
            {
                throw new PluginWorkspaceLockException(
                    $"Could not prepare {request.Purpose} ownership for '{request.Path}'. Close the other process using this plugin and try again.",
                    exception);
            }
        }

        var orderedRequests = requestArray
            .OrderBy(request => request.Path, comparer)
            .ThenBy(request => request.Purpose, StringComparer.Ordinal)
            .ToArray();
        var acquired = new List<WorkspaceFileLock>(orderedRequests.Length);

        try
        {
            foreach (var request in orderedRequests)
            {
                acquired.Add(WorkspaceFileLock.Acquire(request));
            }

            return new WorkspaceFileLockSet(acquired);
        }
        catch
        {
            for (var index = acquired.Count - 1; index >= 0; index--)
            {
                try
                {
                    acquired[index].Dispose();
                }
                catch
                {
                    // Preserve the acquisition failure while attempting every remaining cleanup step.
                }
            }

            throw;
        }
    }

    /// <summary>Acquires an additional deterministic lock phase and transfers it into this lifetime set.</summary>
    /// <param name="requests">The additional lock requests.</param>
    public void AcquireAdditional(IEnumerable<WorkspaceFileLockRequest> requests)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        var additional = Acquire(requests);
        _locks.AddRange(additional._locks);
        additional._locks.Clear();
        additional._disposed = true;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        var exceptions = new List<Exception>();
        for (var index = _locks.Count - 1; index >= 0; index--)
        {
            try
            {
                _locks[index].Dispose();
            }
            catch (Exception exception)
            {
                exceptions.Add(exception);
            }
        }

        if (exceptions.Count > 0)
        {
            throw new AggregateException("One or more workspace file locks failed to close.", exceptions);
        }
    }
}

/// <summary>Describes one lifetime lock request.</summary>
internal sealed class WorkspaceFileLockRequest
{
    /// <summary>Initializes a file-lock request.</summary>
    public WorkspaceFileLockRequest(string path, string purpose, bool isShared, bool createIfMissing)
    {
        Path = path;
        Purpose = purpose;
        IsShared = isShared;
        CreateIfMissing = createIfMissing;
    }

    /// <summary>Gets the absolute target path.</summary>
    public string Path { get; }

    /// <summary>Gets the diagnostic purpose.</summary>
    public string Purpose { get; }

    /// <summary>Gets whether Unix should acquire a shared rather than exclusive advisory lock.</summary>
    public bool IsShared { get; }

    /// <summary>Gets whether an absent target should be created.</summary>
    public bool CreateIfMissing { get; }
}

/// <summary>Owns one open handle and its Unix advisory lock, when applicable.</summary>
internal sealed class WorkspaceFileLock : IDisposable
{
    private const int LockShared = 1;
    private const int LockExclusive = 2;
    private const int LockNonBlocking = 4;
    private const int LockUnlock = 8;

    private readonly FileStream _stream;
    private readonly bool _hasUnixLock;
    private bool _disposed;

    private WorkspaceFileLock(FileStream stream, bool hasUnixLock)
    {
        _stream = stream;
        _hasUnixLock = hasUnixLock;
    }

    /// <summary>Acquires one file handle and its platform-specific ownership lock without waiting.</summary>
    public static WorkspaceFileLock Acquire(WorkspaceFileLockRequest request)
    {
        FileStream? stream = null;
        try
        {
            const FileMode mode = FileMode.Open;
            var access = request.IsShared ? FileAccess.Read : FileAccess.ReadWrite;
            var share = OperatingSystem.IsWindows()
                ? request.IsShared ? FileShare.Read : FileShare.None
                : FileShare.ReadWrite | FileShare.Delete;
            stream = new FileStream(request.Path, mode, access, share);

            if (!OperatingSystem.IsWindows())
            {
                var operation = (request.IsShared ? LockShared : LockExclusive) | LockNonBlocking;
                if (UnixFileLockApi.Flock(stream.SafeFileHandle.DangerousGetHandle().ToInt32(), operation) != 0)
                {
                    throw new IOException(new Win32Exception(Marshal.GetLastPInvokeError()).Message);
                }
            }

            return new WorkspaceFileLock(stream, !OperatingSystem.IsWindows());
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            stream?.Dispose();
            throw new PluginWorkspaceLockException(
                $"Could not acquire {request.Purpose} ownership for '{request.Path}'. Close the other process using this plugin and try again.",
                exception);
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        if (_hasUnixLock)
        {
            _ = UnixFileLockApi.Flock(_stream.SafeFileHandle.DangerousGetHandle().ToInt32(), LockUnlock);
        }

        _stream.Dispose();
    }

    private static class UnixFileLockApi
    {
        [DllImport("libc", EntryPoint = "flock", SetLastError = true)]
        internal static extern int Flock(int fileDescriptor, int operation);
    }
}
