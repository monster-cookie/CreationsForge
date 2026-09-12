namespace CreationsForge.Core.Engine.Contracts;

/// <summary>
/// Carries deterministic progress for a workspace-open operation.
/// </summary>
public sealed class WorkspaceOpenProgress
{
    /// <summary>Initializes a workspace-open progress update.</summary>
    /// <param name="stage">The current open stage.</param>
    /// <param name="message">A non-empty diagnostic description of the stage.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="message"/> is empty or whitespace.</exception>
    public WorkspaceOpenProgress(WorkspaceOpenStage stage, string message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message);
        Stage = stage;
        Message = message;
    }

    /// <summary>Gets the current open stage.</summary>
    public WorkspaceOpenStage Stage { get; }

    /// <summary>Gets the diagnostic description of the stage.</summary>
    public string Message { get; }
}
