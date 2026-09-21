using CreationsForge.Core.Engine.Contracts;

namespace CreationsForge.Core.Engine.PluginInputs;

/// <summary>Owns read locks for every existing physical artifact in one plugin source lifetime.</summary>
internal sealed class PluginSourceFileLockSet : IAsyncDisposable
{
    /// <summary>The retained source handles. Each permits readers and denies writers and deletion.</summary>
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
        if (observation.Stream is not null)
        {
            Streams.Add(observation.Stream);
        }

        return observation.Artifact;
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
            try
            {
                Streams[index].Dispose();
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
}
