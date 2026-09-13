using System.Diagnostics;
using CreationsForge.Core.Engine.Contracts;
using Serilog;

namespace CreationsForge.Core.Engine;

/// <summary>Logs workspace-open progress synchronously before forwarding it to an optional caller observer.</summary>
internal sealed class LoggingWorkspaceOpenProgress : IProgress<WorkspaceOpenProgress>
{
    /// <summary>The structured logger shared by the headless engine.</summary>
    private readonly ILogger Logger;

    /// <summary>The optional caller observer that receives the same ordered progress updates.</summary>
    private readonly IProgress<WorkspaceOpenProgress>? Observer;

    /// <summary>The timestamp captured before workspace validation began.</summary>
    private readonly long StartedTimestamp = Stopwatch.GetTimestamp();

    /// <summary>The workspace identity included in every diagnostic event.</summary>
    private readonly Guid? WorkspaceId;

    /// <summary>Protects the phase snapshot shared by adapter and slow-operation threads.</summary>
    private readonly object PhaseSync = new();

    /// <summary>The most recently entered open stage for slow-operation diagnostics.</summary>
    private WorkspaceOpenStage CurrentStageValue = WorkspaceOpenStage.Validating;

    /// <summary>The most recent progress description for slow-operation diagnostics.</summary>
    private string CurrentMessageValue = "Workspace opening has started.";

    /// <summary>Initializes a synchronous logging and forwarding progress observer.</summary>
    /// <param name="logger">The shared structured logger.</param>
    /// <param name="workspaceId">The requested workspace identity when available.</param>
    /// <param name="observer">The optional caller observer.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="logger"/> is <see langword="null"/>.</exception>
    public LoggingWorkspaceOpenProgress(
        ILogger logger,
        Guid? workspaceId,
        IProgress<WorkspaceOpenProgress>? observer)
    {
        ArgumentNullException.ThrowIfNull(logger);
        Logger = logger;
        WorkspaceId = workspaceId;
        Observer = observer;
    }

    /// <summary>Gets the elapsed time since workspace opening began.</summary>
    public TimeSpan Elapsed => Stopwatch.GetElapsedTime(StartedTimestamp);

    /// <summary>Logs and forwards one workspace-open progress update.</summary>
    /// <param name="value">The non-null stage update to publish.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is <see langword="null"/>.</exception>
    public void Report(WorkspaceOpenProgress value)
    {
        ArgumentNullException.ThrowIfNull(value);
        lock (PhaseSync)
        {
            CurrentStageValue = value.Stage;
            CurrentMessageValue = value.Message;
        }

        Logger.Information(
            "Workspace {WorkspaceId} open stage {OpenStage} after {ElapsedMilliseconds} ms: {ProgressMessage}",
            WorkspaceId,
            value.Stage,
            Elapsed.TotalMilliseconds,
            value.Message);
        Observer?.Report(value);
    }

    /// <summary>Writes a warning that plugin source acquisition remains incomplete at the most recently reported phase.</summary>
    public void LogStillRunning()
    {
        var (stage, message) = GetCurrentPhase();
        Logger.Warning(
            "Workspace {WorkspaceId} is still opening during {OpenStage} after {ElapsedMilliseconds} ms: {ProgressMessage}",
            WorkspaceId,
            stage,
            Elapsed.TotalMilliseconds,
            message);
    }

    /// <summary>Writes the terminal cancellation phase and elapsed time.</summary>
    public void LogCancellation()
    {
        var (stage, message) = GetCurrentPhase();
        Logger.Warning(
            "Workspace {WorkspaceId} opening was canceled during {OpenStage} after {ElapsedMilliseconds} ms: {ProgressMessage}",
            WorkspaceId,
            stage,
            Elapsed.TotalMilliseconds,
            message);
    }

    /// <summary>Returns one consistent snapshot of the most recently reported phase.</summary>
    /// <returns>The current stage and diagnostic description.</returns>
    private (WorkspaceOpenStage Stage, string Message) GetCurrentPhase()
    {
        lock (PhaseSync)
        {
            return (CurrentStageValue, CurrentMessageValue);
        }
    }
}
