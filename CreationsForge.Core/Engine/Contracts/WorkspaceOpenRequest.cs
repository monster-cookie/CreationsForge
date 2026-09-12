using CreationsForge.Core.Enums;
using Mutagen.Bethesda;

namespace CreationsForge.Core.Engine.Contracts;

/// <summary>
/// Supplies every path and native release choice required to open a workspace without consulting an installed load order.
/// </summary>
public sealed class WorkspaceOpenRequest
{
    /// <summary>Initializes an explicit workspace-open request and snapshots all caller-owned path collections.</summary>
    /// <param name="workspaceId">The non-empty caller-assigned workspace identifier.</param>
    /// <param name="game">The supported CreationsForge game.</param>
    /// <param name="release">The exact native Mutagen release selection.</param>
    /// <param name="sourcePluginPath">The source plugin that remains read-only.</param>
    /// <param name="loadOrderPluginPaths">Every plugin path in explicit load-order order.</param>
    /// <param name="dataDirectoryPath">The data directory used to resolve native plugin resources.</param>
    /// <param name="stringDirectoryPaths">Explicit directories used for localized string resolution.</param>
    /// <param name="progress">An optional observer for coarse-grained open progress.</param>
    /// <exception cref="ArgumentNullException">Thrown when a required path collection is <see langword="null"/>.</exception>
    public WorkspaceOpenRequest(
        Guid workspaceId,
        SupportedGame game,
        GameRelease release,
        string sourcePluginPath,
        IReadOnlyList<string> loadOrderPluginPaths,
        string dataDirectoryPath,
        IReadOnlyList<string> stringDirectoryPaths,
        IProgress<WorkspaceOpenProgress>? progress = null)
    {
        ArgumentNullException.ThrowIfNull(loadOrderPluginPaths);
        ArgumentNullException.ThrowIfNull(stringDirectoryPaths);
        WorkspaceId = workspaceId;
        Game = game;
        Release = release;
        SourcePluginPath = sourcePluginPath;
        LoadOrderPluginPaths = Array.AsReadOnly(loadOrderPluginPaths.ToArray());
        DataDirectoryPath = dataDirectoryPath;
        StringDirectoryPaths = Array.AsReadOnly(stringDirectoryPaths.ToArray());
        Progress = progress;
    }

    /// <summary>Gets the caller-assigned workspace identifier.</summary>
    public Guid WorkspaceId { get; }

    /// <summary>Gets the selected CreationsForge game.</summary>
    public SupportedGame Game { get; }

    /// <summary>Gets the exact native Mutagen release selection.</summary>
    public GameRelease Release { get; }

    /// <summary>Gets the source plugin path, which the workspace must never modify.</summary>
    public string SourcePluginPath { get; }

    /// <summary>Gets an immutable snapshot of plugin paths in explicit load-order order.</summary>
    public IReadOnlyList<string> LoadOrderPluginPaths { get; }

    /// <summary>Gets the explicit data directory used for native resource resolution.</summary>
    public string DataDirectoryPath { get; }

    /// <summary>Gets an immutable snapshot of localized-string search directories.</summary>
    public IReadOnlyList<string> StringDirectoryPaths { get; }

    /// <summary>Gets the optional open-progress observer.</summary>
    public IProgress<WorkspaceOpenProgress>? Progress { get; }

    /// <summary>Creates an equivalent request with validated canonical paths.</summary>
    /// <param name="sourcePluginPath">The canonical source plugin path.</param>
    /// <param name="loadOrderPluginPaths">The canonical explicitly ordered load-order plugin paths.</param>
    /// <param name="dataDirectoryPath">The canonical game data directory.</param>
    /// <param name="stringDirectoryPaths">The canonical ordered localized string search directories.</param>
    /// <returns>A new immutable request containing the canonical paths.</returns>
    internal WorkspaceOpenRequest WithCanonicalPaths(
        string sourcePluginPath,
        IReadOnlyList<string> loadOrderPluginPaths,
        string dataDirectoryPath,
        IReadOnlyList<string> stringDirectoryPaths)
    {
        return new WorkspaceOpenRequest(
            WorkspaceId,
            Game,
            Release,
            sourcePluginPath,
            loadOrderPluginPaths,
            dataDirectoryPath,
            stringDirectoryPaths,
            Progress);
    }
}
