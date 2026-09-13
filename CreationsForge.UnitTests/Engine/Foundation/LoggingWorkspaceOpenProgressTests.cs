using CreationsForge.Core.Engine;
using CreationsForge.Core.Engine.Contracts;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Shouldly;

namespace CreationsForge.UnitTests.Engine.Foundation;

/// <summary>Verifies native workspace progress is both forwarded and retained in structured diagnostics.</summary>
public sealed class LoggingWorkspaceOpenProgressTests
{
    /// <summary>Verifies phase, slow-open, and cancellation events identify the most recent plugin operation.</summary>
    [Fact]
    public void ProgressDiagnostics_IdentifyCurrentPluginAndForwardUpdate()
    {
        var sink = new CollectingLogSink();
        using var logger = new LoggerConfiguration().WriteTo.Sink(sink).CreateLogger();
        var observer = new RecordingProgress();
        var workspaceId = Guid.NewGuid();
        var progress = new LoggingWorkspaceOpenProgress(logger, workspaceId, observer);
        var update = new WorkspaceOpenProgress(
            WorkspaceOpenStage.ParsingPlugin,
            "Parsing Starfield plugin 4 of 6: 'Source.esm'.");

        progress.Report(update);
        progress.LogStillRunning();
        progress.LogCancellation();

        observer.Updates.ShouldBe([update]);
        sink.Events.Select(logEvent => logEvent.Level).ShouldBe([
            LogEventLevel.Information,
            LogEventLevel.Warning,
            LogEventLevel.Warning]);
        sink.Events.ShouldAllBe(logEvent =>
            logEvent.RenderMessage().Contains(workspaceId.ToString(), StringComparison.OrdinalIgnoreCase)
            && logEvent.RenderMessage().Contains("ParsingPlugin", StringComparison.Ordinal)
            && logEvent.RenderMessage().Contains("Source.esm", StringComparison.Ordinal));
        sink.Events[1].RenderMessage().ShouldContain("still opening");
        sink.Events[2].RenderMessage().ShouldContain("was canceled");
    }

    /// <summary>Collects structured log events synchronously for exact diagnostic assertions.</summary>
    private sealed class CollectingLogSink : ILogEventSink
    {
        /// <summary>Gets emitted log events in publication order.</summary>
        public IList<LogEvent> Events { get; } = [];

        /// <summary>Records one structured log event.</summary>
        /// <param name="logEvent">The event emitted by the logger.</param>
        public void Emit(LogEvent logEvent)
        {
            Events.Add(logEvent);
        }
    }

    /// <summary>Records forwarded native workspace progress synchronously.</summary>
    private sealed class RecordingProgress : IProgress<WorkspaceOpenProgress>
    {
        /// <summary>Gets forwarded updates in publication order.</summary>
        public IList<WorkspaceOpenProgress> Updates { get; } = [];

        /// <summary>Records one forwarded progress update.</summary>
        /// <param name="value">The update forwarded by the logging observer.</param>
        public void Report(WorkspaceOpenProgress value)
        {
            Updates.Add(value);
        }
    }
}
