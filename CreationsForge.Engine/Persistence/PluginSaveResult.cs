namespace CreationsForge.Engine.Persistence;

/// <summary>Reports the complete bounded result of one save request.</summary>
public sealed class PluginSaveResult
{
    private readonly IReadOnlyList<string> _publishedPaths;
    private readonly IReadOnlyList<string> _backupPaths;

    /// <summary>Initializes a normalized save result.</summary>
    internal PluginSaveResult(
        PluginSaveStatus status,
        PluginPublicationState publicationState,
        PluginSavePhase terminalPhase,
        ulong revision,
        string destinationPath,
        IEnumerable<string> publishedPaths,
        IEnumerable<string> backupPaths,
        TimeSpan duration,
        string diagnostic,
        bool requiresWorkspaceReopen)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(destinationPath);
        ArgumentNullException.ThrowIfNull(publishedPaths);
        ArgumentNullException.ThrowIfNull(backupPaths);
        ArgumentException.ThrowIfNullOrWhiteSpace(diagnostic);

        Status = status;
        PublicationState = publicationState;
        TerminalPhase = terminalPhase;
        Revision = revision;
        DestinationPath = destinationPath;
        _publishedPaths = publishedPaths.Distinct(PathComparer).OrderBy(path => path, PathComparer).ToArray();
        _backupPaths = backupPaths.Distinct(PathComparer).OrderBy(path => path, PathComparer).ToArray();
        Duration = duration;
        Diagnostic = diagnostic;
        RequiresWorkspaceReopen = requiresWorkspaceReopen;
    }

    /// <summary>Gets the terminal save status.</summary>
    public PluginSaveStatus Status { get; }

    /// <summary>Gets the observed destination publication state.</summary>
    public PluginPublicationState PublicationState { get; }

    /// <summary>Gets the phase in which the operation finished or failed.</summary>
    public PluginSavePhase TerminalPhase { get; }

    /// <summary>Gets the exact workspace revision captured by the save.</summary>
    public ulong Revision { get; }

    /// <summary>Gets the absolute output plugin path.</summary>
    public string DestinationPath { get; }

    /// <summary>Gets destination paths known to have been published by this request.</summary>
    public IReadOnlyList<string> PublishedPaths => _publishedPaths;

    /// <summary>Gets retained backup paths available for diagnosis or manual recovery.</summary>
    public IReadOnlyList<string> BackupPaths => _backupPaths;

    /// <summary>Gets the complete operation duration.</summary>
    public TimeSpan Duration { get; }

    /// <summary>Gets a bounded human-readable terminal diagnostic.</summary>
    public string Diagnostic { get; }

    /// <summary>Gets whether further mutation, save, or discard must wait for a fresh workspace reopen.</summary>
    public bool RequiresWorkspaceReopen { get; }

    private static StringComparer PathComparer => OperatingSystem.IsWindows()
        ? StringComparer.OrdinalIgnoreCase
        : StringComparer.Ordinal;
}
