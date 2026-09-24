namespace CreationsForge.Engine.Persistence;

/// <summary>Reports the bounded outcome of one per-file publication attempt.</summary>
internal sealed class PluginPublicationResult
{
    private PluginPublicationResult(
        bool succeeded,
        PluginPublicationState state,
        IReadOnlyList<string> publishedPaths,
        IReadOnlyList<string> backupPaths,
        string diagnostic)
    {
        Succeeded = succeeded;
        State = state;
        PublishedPaths = publishedPaths;
        BackupPaths = backupPaths;
        Diagnostic = diagnostic;
    }

    /// <summary>Gets whether every staged file was published.</summary>
    public bool Succeeded { get; }

    /// <summary>Gets the destination state established by publication or restoration.</summary>
    public PluginPublicationState State { get; }

    /// <summary>Gets destination paths known to have been published during the attempt.</summary>
    public IReadOnlyList<string> PublishedPaths { get; }

    /// <summary>Gets backup paths retained after the attempt.</summary>
    public IReadOnlyList<string> BackupPaths { get; }

    /// <summary>Gets the bounded publication diagnostic.</summary>
    public string Diagnostic { get; }

    /// <summary>Creates a successful publication result.</summary>
    public static PluginPublicationResult Success(
        IReadOnlyList<string> publishedPaths,
        IReadOnlyList<string> backupPaths) =>
        new(true, PluginPublicationState.Published, publishedPaths, backupPaths, "The staged file set was published.");

    /// <summary>Creates a failed publication or restoration result.</summary>
    public static PluginPublicationResult Failure(
        PluginPublicationState state,
        IReadOnlyList<string> publishedPaths,
        IReadOnlyList<string> backupPaths,
        string diagnostic) =>
        new(false, state, publishedPaths, backupPaths, diagnostic);
}
