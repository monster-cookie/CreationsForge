using System.Collections.Concurrent;
using CreationsForge.Engine.Persistence;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;

namespace CreationsForge.UnitTests.Workspaces;

/// <summary>Verifies ordered save progress delivery outside the workspace operation gate.</summary>
public sealed partial class PluginWorkspaceTests
{
    /// <summary>Allows persistence to advance while a synchronous progress consumer is blocked.</summary>
    [Fact]
    public async Task ProgressCallbackDoesNotBlockSaveOperationGate()
    {
        using var directory = new TemporaryDirectory();
        var outputModKey = ModKey.FromNameAndExtension("Output.esp");
        var exportPause = new ExportPause();
        var backend = new InstrumentedPersistenceBackend { AfterExportAsync = exportPause.WaitAsync };
        using var workspace = CreateFactory(backend).Open(
            CreateNewRequest(directory.Path, GameRelease.Fallout4, [], outputModKey));
        var progress = new BlockingProgress();
        var saveTask = Task.Run(
            () => workspace.SaveAsync(progress, TestContext.Current.CancellationToken),
            TestContext.Current.CancellationToken);

        var progressTimeout = Task.Delay(TimeSpan.FromSeconds(2), TestContext.Current.CancellationToken);
        var progressCompleted = await Task.WhenAny(progress.Entered.Task, progressTimeout);
        var progressWasObserved = progressCompleted == progress.Entered.Task;
        var saveAdvancedWhileProgressWasBlocked = false;
        if (progressWasObserved)
        {
            var exportTimeout = Task.Delay(TimeSpan.FromSeconds(2), TestContext.Current.CancellationToken);
            var exportCompleted = await Task.WhenAny(exportPause.Entered.Task, exportTimeout);
            saveAdvancedWhileProgressWasBlocked = exportCompleted == exportPause.Entered.Task;
        }

        progress.Release.TrySetResult(true);
        exportPause.Release.TrySetResult(true);
        var result = await saveTask.WaitAsync(TestContext.Current.CancellationToken);

        Assert.True(progressWasObserved, "No save progress callback was delivered.");
        Assert.True(
            saveAdvancedWhileProgressWasBlocked,
            "Native export did not begin until the synchronous progress callback returned.");
        Assert.True(result.Status == PluginSaveStatus.Succeeded, result.Diagnostic);
        Assert.Equal(PluginSavePhase.Preflight, progress.Events.First().Phase);
        Assert.Equal(PluginSavePhase.Complete, progress.Events.Last().Phase);
    }

    /// <summary>Blocks its first report call until the test releases it while retaining every delivered event.</summary>
    private sealed class BlockingProgress : IProgress<PluginSaveProgress>
    {
        private int _blocked;

        /// <summary>Gets the events delivered to the consumer in report order.</summary>
        public ConcurrentQueue<PluginSaveProgress> Events { get; } = new();

        /// <summary>Gets the signal completed when the first report call begins.</summary>
        public TaskCompletionSource<bool> Entered { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        /// <summary>Gets the signal that releases the first report call.</summary>
        public TaskCompletionSource<bool> Release { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        /// <inheritdoc />
        public void Report(PluginSaveProgress value)
        {
            Events.Enqueue(value);
            if (Interlocked.CompareExchange(ref _blocked, 1, 0) != 0)
            {
                return;
            }

            Entered.TrySetResult(true);
            Release.Task.GetAwaiter().GetResult();
        }
    }
}
