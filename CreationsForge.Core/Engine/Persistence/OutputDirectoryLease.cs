using CreationsForge.Core.Engine.Contracts;

namespace CreationsForge.Core.Engine.Persistence;

/// <summary>Owns the stable guard stream, retained directory identity, and process-wide gate for one output directory.</summary>
internal sealed class OutputDirectoryLease : IOutputDirectoryLease
{
    /// <summary>The atomically owned resource set, cleared by the first disposal.</summary>
    private LeaseResources? resources;

    /// <summary>Initializes ownership of every acquired lease resource.</summary>
    /// <param name="outputDirectoryPath">The canonical protected output directory.</param>
    /// <param name="guardStream">The exclusively opened stable guard stream.</param>
    /// <param name="directoryHandle">The retained native directory identity.</param>
    /// <param name="processGate">The acquired process-wide directory gate.</param>
    internal OutputDirectoryLease(
        string outputDirectoryPath,
        FileStream guardStream,
        OutputDirectoryIdentityHandle directoryHandle,
        OutputDirectoryInProcessGate.Releaser processGate)
    {
        OutputDirectoryPath = outputDirectoryPath;
        resources = new LeaseResources(guardStream, directoryHandle, processGate);
    }

    /// <inheritdoc />
    public string OutputDirectoryPath { get; }

    /// <summary>Releases the guard stream first, then the retained directory handle and in-process gate.</summary>
    /// <returns>A value task that completes after asynchronous guard-stream disposal.</returns>
    public async ValueTask DisposeAsync()
    {
        var ownedResources = Interlocked.Exchange(ref resources, null);
        if (ownedResources is null)
        {
            return;
        }

        try
        {
            await ownedResources.GuardStream.DisposeAsync().ConfigureAwait(false);
        }
        finally
        {
            try
            {
                ownedResources.DirectoryHandle.Dispose();
            }
            finally
            {
                ownedResources.ProcessGate.Dispose();
            }
        }
    }

    /// <summary>Groups all lease resources for one atomic transfer into disposal.</summary>
    private sealed class LeaseResources
    {
        /// <summary>Initializes ownership of every acquired lease resource.</summary>
        /// <param name="guardStream">The exclusively opened stable guard stream.</param>
        /// <param name="directoryHandle">The retained native directory identity.</param>
        /// <param name="processGate">The acquired process-wide directory gate.</param>
        internal LeaseResources(
            FileStream guardStream,
            OutputDirectoryIdentityHandle directoryHandle,
            OutputDirectoryInProcessGate.Releaser processGate)
        {
            GuardStream = guardStream;
            DirectoryHandle = directoryHandle;
            ProcessGate = processGate;
        }

        /// <summary>Gets the exclusively opened stable guard stream.</summary>
        internal FileStream GuardStream { get; }

        /// <summary>Gets the retained native output-directory identity.</summary>
        internal OutputDirectoryIdentityHandle DirectoryHandle { get; }

        /// <summary>Gets the acquired process-wide directory gate.</summary>
        internal OutputDirectoryInProcessGate.Releaser ProcessGate { get; }
    }
}
