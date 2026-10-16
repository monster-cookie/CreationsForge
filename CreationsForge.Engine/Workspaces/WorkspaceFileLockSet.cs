using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

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
    public WorkspaceFileLockRequest(string path, string purpose, bool isShared, bool createIfMissing, bool deleteOnRelease = false)
    {
        Path = path;
        Purpose = purpose;
        IsShared = isShared;
        CreateIfMissing = createIfMissing;
        DeleteOnRelease = deleteOnRelease;
    }

    /// <summary>Gets the absolute target path.</summary>
    public string Path { get; }

    /// <summary>Gets the diagnostic purpose.</summary>
    public string Purpose { get; }

    /// <summary>Gets whether Unix should acquire a shared rather than exclusive advisory lock.</summary>
    public bool IsShared { get; }

    /// <summary>Gets whether an absent target should be created.</summary>
    public bool CreateIfMissing { get; }

    /// <summary>Gets whether the target file is removed after its handle is closed.</summary>
    /// <remarks>Output identity sidecars use this so a closed workspace does not leave a lock file behind.</remarks>
    public bool DeleteOnRelease { get; }
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
    private readonly string? _deletePath;
    private bool _disposed;

    private WorkspaceFileLock(FileStream stream, bool hasUnixLock, string? deletePath)
    {
        _stream = stream;
        _hasUnixLock = hasUnixLock;
        _deletePath = deletePath;
    }

    /// <summary>Acquires one file handle and its platform-specific ownership lock without waiting.</summary>
    public static WorkspaceFileLock Acquire(WorkspaceFileLockRequest request)
    {
        const int maximumAttempts = 8;
        for (var attempt = 1; attempt <= maximumAttempts; attempt++)
        {
            FileStream? stream = null;
            try
            {
                stream = OpenHandle(request);
                if (!OperatingSystem.IsWindows())
                {
                    var operation = (request.IsShared ? LockShared : LockExclusive) | LockNonBlocking;
                    if (UnixFileLockApi.Flock(FileDescriptor(stream), operation) != 0)
                    {
                        throw new IOException(new Win32Exception(Marshal.GetLastPInvokeError()).Message);
                    }

                    if (request.DeleteOnRelease && !UnixFileLockApi.IsCurrentDirectoryEntry(stream, request.Path))
                    {
                        _ = UnixFileLockApi.Flock(FileDescriptor(stream), LockUnlock);
                        stream.Dispose();
                        stream = null;
                        if (attempt == maximumAttempts)
                        {
                            throw new IOException($"The output lock '{request.Path}' was replaced while ownership was being acquired.");
                        }

                        continue;
                    }
                }

                var owned = stream;
                stream = null;
                var deletePath = request.DeleteOnRelease && !OperatingSystem.IsWindows() ? request.Path : null;
                return new WorkspaceFileLock(owned, !OperatingSystem.IsWindows(), deletePath);
            }
            catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
            {
                stream?.Dispose();
                throw new PluginWorkspaceLockException(
                    $"Could not acquire {request.Purpose} ownership for '{request.Path}'. Close the other process using this plugin and try again.",
                    exception);
            }
        }

        throw new PluginWorkspaceLockException(
            $"Could not acquire {request.Purpose} ownership for '{request.Path}'. Close the other process using this plugin and try again.",
            new IOException($"The output lock '{request.Path}' was replaced while ownership was being acquired."));
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        if (_deletePath is not null)
        {
            try
            {
                File.Delete(_deletePath);
            }
            catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
            {
                // The advisory lock is still held. A sidecar that cannot be unlinked is removed when the handle closes if the platform allows it.
            }
        }

        if (_hasUnixLock)
        {
            _ = UnixFileLockApi.Flock(FileDescriptor(_stream), LockUnlock);
        }

        _stream.Dispose();
    }

    /// <summary>Opens the lock file with the share and deletion mode for this platform.</summary>
    /// <param name="request">The lock to open.</param>
    /// <returns>The open handle. The caller owns it.</returns>
    private static FileStream OpenHandle(WorkspaceFileLockRequest request)
    {
        var mode = request.CreateIfMissing ? FileMode.OpenOrCreate : FileMode.Open;
        var access = request.IsShared ? FileAccess.Read : FileAccess.ReadWrite;
        var share = OperatingSystem.IsWindows()
            ? request.IsShared ? FileShare.Read : FileShare.None
            : FileShare.ReadWrite | FileShare.Delete;
        var options = OperatingSystem.IsWindows() && request.DeleteOnRelease
            ? FileOptions.DeleteOnClose
            : FileOptions.None;
        return new FileStream(request.Path, mode, access, share, bufferSize: 1, options);
    }

    /// <summary>Reads the native descriptor used by <c>flock</c>.</summary>
    /// <param name="stream">The open lock file.</param>
    /// <returns>The descriptor value.</returns>
    private static int FileDescriptor(FileStream stream)
    {
        return stream.SafeFileHandle.DangerousGetHandle().ToInt32();
    }

    private static class UnixFileLockApi
    {
        [DllImport("libc", EntryPoint = "flock", SetLastError = true)]
        internal static extern int Flock(int fileDescriptor, int operation);

        /// <summary>Reports whether <paramref name="stream"/> still names <paramref name="path"/> after <c>flock</c>.</summary>
        /// <param name="stream">The locked handle.</param>
        /// <param name="path">The directory entry that should own the lock.</param>
        /// <returns><see langword="false"/> when a closer unlinked the entry before this lock was acquired.</returns>
        internal static bool IsCurrentDirectoryEntry(FileStream stream, string path)
        {
            if (OperatingSystem.IsLinux())
            {
                return LinuxFileIdentity.IsLinked(stream);
            }

            if (OperatingSystem.IsMacOS())
            {
                return DarwinFileIdentity.MatchesPath(stream, path);
            }

            return true;
        }
    }

    /// <summary>Detects a Linux lock file that was unlinked while another process still had it open.</summary>
    private static class LinuxFileIdentity
    {
        [DllImport("libc", EntryPoint = "readlink", SetLastError = true)]
        private static extern int ReadLink(string path, byte[] buffer, nuint bufferSize);

        /// <summary>Reports whether the kernel still links <paramref name="stream"/> to a directory entry.</summary>
        /// <param name="stream">The locked handle.</param>
        /// <returns><see langword="false"/> when <c>/proc/self/fd</c> reports the file as deleted.</returns>
        internal static bool IsLinked(FileStream stream)
        {
            var buffer = new byte[4096];
            var length = ReadLink("/proc/self/fd/" + FileDescriptor(stream).ToString(System.Globalization.CultureInfo.InvariantCulture), buffer, (nuint)buffer.Length);
            if (length <= 0)
            {
                return false;
            }

            if (length >= buffer.Length)
            {
                return false;
            }

            var text = Encoding.UTF8.GetString(buffer, 0, length);
            return !text.EndsWith(" (deleted)", StringComparison.Ordinal);
        }
    }

    /// <summary>Compares a macOS lock descriptor with the directory entry it is supposed to own.</summary>
    private static class DarwinFileIdentity
    {
        /// <summary>Matches the 144-byte Darwin <c>stat$INODE64</c> result so the call cannot write past this value.</summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct Inode64Stat
        {
            public int Device;
            public ushort Mode;
            public ushort LinkCount;
            public ulong Inode;
            private uint _userId;
            private uint _groupId;
            private int _representedDevice;
            private int _alignment;
            private long _accessSeconds;
            private long _accessNanoseconds;
            private long _modifySeconds;
            private long _modifyNanoseconds;
            private long _changeSeconds;
            private long _changeNanoseconds;
            private long _birthSeconds;
            private long _birthNanoseconds;
            private long _size;
            private long _blocks;
            private int _blockSize;
            private uint _flags;
            private uint _generation;
            private int _spare;
            private long _spare0;
            private long _spare1;
        }

        [DllImport("libc", EntryPoint = "fstat$INODE64", SetLastError = true)]
        private static extern int FStat(int fileDescriptor, out Inode64Stat stat);

        [DllImport("libc", EntryPoint = "stat$INODE64", SetLastError = true)]
        private static extern int Stat(string path, out Inode64Stat stat);

        /// <summary>Reports whether <paramref name="stream"/> and <paramref name="path"/> are the same linked inode.</summary>
        /// <param name="stream">The locked handle.</param>
        /// <param name="path">The directory entry that should own the lock.</param>
        /// <returns><see langword="false"/> when the path is missing or names a different inode.</returns>
        internal static bool MatchesPath(FileStream stream, string path)
        {
            if (FStat(FileDescriptor(stream), out var held) != 0 || Stat(path, out var current) != 0)
            {
                return false;
            }

            return held.Device == current.Device && held.Inode == current.Inode && held.LinkCount > 0;
        }
    }
}
