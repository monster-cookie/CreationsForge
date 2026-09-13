using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Enums;
using Mutagen.Bethesda;

namespace CreationsForge.Services;

/// <summary>
/// Exposes immutable presentation-safe identity for the currently active workspace without transferring its ownership.
/// </summary>
public sealed class WorkspaceDescriptor
{
    /// <summary>Initializes a snapshot of an active workspace.</summary>
    /// <param name="workspaceId">The caller-assigned workspace identity.</param>
    /// <param name="game">The selected supported game.</param>
    /// <param name="release">The exact plugin game release.</param>
    /// <param name="sourcePluginPath">The read-only source plugin path.</param>
    /// <param name="loadOrderPluginPaths">The explicit plugin paths in load-order order.</param>
    /// <param name="output">The admitted output association, or <see langword="null"/> for a read-only workspace.</param>
    /// <param name="revision">The source revision, advanced when an output was selected.</param>
    /// <exception cref="ArgumentNullException">Thrown when a required reference is <see langword="null"/>.</exception>
    public WorkspaceDescriptor(
        Guid workspaceId,
        SupportedGame game,
        GameRelease release,
        string sourcePluginPath,
        IReadOnlyList<string> loadOrderPluginPaths,
        OutputAssociation? output,
        WorkspaceRevision revision)
    {
        ArgumentNullException.ThrowIfNull(loadOrderPluginPaths);
        WorkspaceId = workspaceId;
        Game = game;
        Release = release;
        SourcePluginPath = sourcePluginPath;
        LoadOrderPluginPaths = Array.AsReadOnly(loadOrderPluginPaths.ToArray());
        Output = output;
        Revision = revision;
    }

    /// <summary>Gets the caller-assigned workspace identity.</summary>
    public Guid WorkspaceId { get; }

    /// <summary>Gets the selected supported game.</summary>
    public SupportedGame Game { get; }

    /// <summary>Gets the exact plugin game release.</summary>
    public GameRelease Release { get; }

    /// <summary>Gets the read-only source plugin path.</summary>
    public string SourcePluginPath { get; }

    /// <summary>Gets the explicit plugin paths in load-order order.</summary>
    public IReadOnlyList<string> LoadOrderPluginPaths { get; }

    /// <summary>Gets the admitted output association, or <see langword="null"/> for a read-only workspace.</summary>
    public OutputAssociation? Output { get; }

    /// <summary>Gets the source revision, advanced when an output was selected.</summary>
    public WorkspaceRevision Revision { get; }
}
