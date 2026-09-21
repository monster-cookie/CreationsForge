using System.ComponentModel;
using System.Runtime.InteropServices;
using CreationsForge.Core.Engine.Contracts;

namespace CreationsForge.Core.Engine.PluginInputs;

/// <summary>Owns read locks for every existing physical artifact in one plugin source lifetime.</summary>
internal sealed class PluginSourceFileLockSet : IAsyncDisposable
{
    /// <summary>The nonblocking shared lock operation used by Linux and macOS.</summary>
    private const int LockSharedNonBlocking = 0x01 | 0x04;

    /// <summary>The unlock operation used by Linux and macOS.</summary>
    private const int LockUnlock = 0x08;

    /// <summary>The retained source handles. Windows share modes deny writers and deletion; Unix handles also retain shared advisory locks.</summary>
    private readonly List<FileStream> Streams = [];

    /// <summary>Tracks whether the retained handles have been released.</summary>
    private int IsDisposed;

    /// <summary>Observes one source path and retains its read lock when the file exists.</summary>
    /// <param name="path">The canonical absolute artifact path.</param>
    /// <param name="role">The artifact role.</param>
    /// <param name="language">The sidecar language, or <see langword="null"/> for plugins and archives.</param>
    /// <param name="mustExist">Whether absence is a source-open failure.</param>
    /// <param name="cancellationToken">The token checked before acquiring the lock.</param>
    /// <returns>The metadata observed from the retained source handle, or an absent optional artifact.</returns>
    internal PluginArtifactAssociation ObserveAndLock(
        string path,
        PluginArtifactRole role,
        string? language,
        bool mustExist,
        CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(Volatile.Read(ref IsDisposed) != 0, this);
        var observation = PluginFileInspector.OpenReadLock(
            path,
            role,
            language,
            mustExist,
            cancellationToken);
        if (observation.Stream is null)
        {
            return observation.Artifact;
        }

        try
        {
            AcquireUnixSharedLock(observation.Stream, path);
            Streams.Add(observation.Stream);
            return observation.Artifact;
        }
        catch
        {
            observation.Stream.Dispose();
            throw;
        }
    }

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref IsDisposed, 1) != 0)
        {
            return ValueTask.CompletedTask;
        }

        List<Exception>? failures = null;
        for (var index = Streams.Count - 1; index >= 0; index--)
        {
            var stream = Streams[index];
            try
            {
                ReleaseUnixSharedLock(stream);
            }
            catch (Exception exception)
            {
                (failures ??= []).Add(exception);
            }

            try
            {
                stream.Dispose();
            }
            catch (Exception exception)
            {
                (failures ??= []).Add(exception);
            }
        }

        Streams.Clear();
        return failures is null
            ? ValueTask.CompletedTask
            : ValueTask.FromException(new AggregateException("One or more plugin source locks could not be released.", failures));
    }

    /// <summary>Acquires the host's shared advisory source lock when Windows share modes are not available.</summary>
    /// <param name="stream">The retained source stream.</param>
    /// <param name="path">The source path used in diagnostics.</param>
    /// <exception cref="PluginSourceInputException">Thrown when the shared lock cannot be acquired.</exception>
    private static void AcquireUnixSharedLock(FileStream stream, string path)
    {
        if (OperatingSystem.IsWindows())
        {
            return;
        }

        if (!OperatingSystem.IsLinux() && !OperatingSystem.IsMacOS())
        {
            throw new PluginSourceInputException(
                EngineErrorCode.UnsupportedInput,
                "Plugin source locking is currently supported only on Windows, Linux, and macOS.");
        }

        if (Flock(stream.SafeFileHandle.DangerousGetHandle().ToInt32(), LockSharedNonBlocking) != 0)
        {
            throw new PluginSourceInputException(
                EngineErrorCode.SourceOpenFailed,
                $"The plugin source is already locked for writing: '{path}'.",
                new Win32Exception(Marshal.GetLastPInvokeError()));
        }
    }

    /// <summary>Releases the host's shared advisory source lock before closing its retained handle.</summary>
    /// <param name="stream">The retained source stream.</param>
    /// <exception cref="IOException">Thrown when the host cannot release the advisory lock.</exception>
    private static void ReleaseUnixSharedLock(FileStream stream)
    {
        if (OperatingSystem.IsWindows())
        {
            return;
        }

        if (Flock(stream.SafeFileHandle.DangerousGetHandle().ToInt32(), LockUnlock) != 0)
        {
            throw new IOException(
                "The plugin source advisory lock could not be released.",
                new Win32Exception(Marshal.GetLastPInvokeError()));
        }
    }

    /// <summary>Applies or releases a BSD-style whole-file advisory lock on Linux and macOS.</summary>
    /// <param name="fileDescriptor">The open source file descriptor.</param>
    /// <param name="operation">The shared, exclusive, nonblocking, or unlock operation flags.</param>
    /// <returns>Zero on success or minus one on failure.</returns>
    [DllImport("libc", EntryPoint = "flock", SetLastError = true)]
    private static extern int Flock(int fileDescriptor, int operation);
}
