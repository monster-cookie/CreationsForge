namespace CreationsForge.Engine.Persistence;

/// <summary>Serializes save progress report calls on the thread pool so consumers never execute under the workspace operation gate.</summary>
internal sealed class PluginSaveProgressDispatcher : IProgress<PluginSaveProgress>
{
    private readonly object _sync = new();
    private readonly IProgress<PluginSaveProgress>? _progress;
    private Task _tail = Task.CompletedTask;

    /// <summary>Initializes a dispatcher for the optional progress consumer.</summary>
    /// <param name="progress">The consumer whose report calls are queued in event order, or <see langword="null"/> to discard events.</param>
    public PluginSaveProgressDispatcher(IProgress<PluginSaveProgress>? progress)
    {
        _progress = progress;
    }

    /// <inheritdoc />
    public void Report(PluginSaveProgress value)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (_progress is null)
        {
            return;
        }

        lock (_sync)
        {
            _tail = _tail.ContinueWith(
                _ =>
                {
                    try
                    {
                        _progress.Report(value);
                    }
                    catch
                    {
                        // Diagnostics must not change the persistence outcome.
                    }
                },
                CancellationToken.None,
                TaskContinuationOptions.DenyChildAttach,
                TaskScheduler.Default);
        }
    }

    /// <summary>Waits for every report call queued before this method was invoked.</summary>
    /// <returns>A task that completes after the queued report calls return.</returns>
    public Task CompleteAsync()
    {
        lock (_sync)
        {
            return _tail;
        }
    }
}
