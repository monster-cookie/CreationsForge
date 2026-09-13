using CreationsForge.Core.Enums;
using Mutagen.Bethesda;

namespace CreationsForge.Core.Engine.Contracts;

/// <summary>
/// Borrows a workspace's selected plugin state while its operation gate remains held by the caller.
/// </summary>
public sealed class WorkspaceSaveContext
{
    /// <summary>Initializes a borrowed save context.</summary>
    /// <param name="workspaceId">The live workspace identifier.</param>
    /// <param name="revision">The live workspace revision.</param>
    /// <param name="game">The immutable CreationsForge game selected when the workspace opened.</param>
    /// <param name="release">The exact plugin release selected when the workspace opened.</param>
    /// <param name="adapter">The selected game adapter.</param>
    /// <param name="sources">The borrowed plugin source-set handle.</param>
    /// <param name="output">The borrowed plugin output-state handle.</param>
    /// <param name="outputAssociation">The selected output association.</param>
    /// <param name="outputBaseline">The complete selected-output baseline.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="workspaceId"/> is empty or the adapter does not match the selected game and release.</exception>
    /// <exception cref="ArgumentNullException">Thrown when any borrowed dependency, handle, association, or baseline is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="game"/> or <paramref name="release"/> is undefined.</exception>
    public WorkspaceSaveContext(
        Guid workspaceId,
        WorkspaceRevision revision,
        SupportedGame game,
        GameRelease release,
        IFormListGameAdapter adapter,
        IPluginSourceSet sources,
        IPluginOutputState output,
        OutputAssociation outputAssociation,
        OutputArtifactSetBaseline outputBaseline)
    {
        if (workspaceId == Guid.Empty)
        {
            throw new ArgumentException("A save context requires a non-empty workspace identifier.", nameof(workspaceId));
        }

        if (!Enum.IsDefined(game))
        {
            throw new ArgumentOutOfRangeException(nameof(game));
        }

        if (!Enum.IsDefined(release))
        {
            throw new ArgumentOutOfRangeException(nameof(release));
        }

        ArgumentNullException.ThrowIfNull(adapter);
        if (adapter.Game != game || !adapter.SupportsRelease(release))
        {
            throw new ArgumentException("The save context adapter must match the workspace's exact game and release.", nameof(adapter));
        }

        ArgumentNullException.ThrowIfNull(sources);
        ArgumentNullException.ThrowIfNull(output);
        ArgumentNullException.ThrowIfNull(outputAssociation);
        ArgumentNullException.ThrowIfNull(outputBaseline);
        WorkspaceId = workspaceId;
        Revision = revision;
        Game = game;
        Release = release;
        Adapter = adapter;
        Sources = sources;
        Output = output;
        OutputAssociation = outputAssociation;
        OutputBaseline = outputBaseline;
    }

    /// <summary>Gets the live workspace identifier.</summary>
    public Guid WorkspaceId { get; }

    /// <summary>Gets the live workspace revision.</summary>
    public WorkspaceRevision Revision { get; }

    /// <summary>Gets the immutable CreationsForge game selected when the workspace opened.</summary>
    public SupportedGame Game { get; }

    /// <summary>Gets the exact plugin release selected when the workspace opened.</summary>
    public GameRelease Release { get; }

    /// <summary>Gets the borrowed selected game adapter.</summary>
    public IFormListGameAdapter Adapter { get; }

    /// <summary>Gets the borrowed plugin source-set handle, which the save coordinator must not dispose.</summary>
    public IPluginSourceSet Sources { get; }

    /// <summary>Gets the borrowed plugin output-state handle, which the save coordinator must not dispose.</summary>
    public IPluginOutputState Output { get; }

    /// <summary>Gets the selected output association.</summary>
    public OutputAssociation OutputAssociation { get; }

    /// <summary>Gets the complete selected-output baseline.</summary>
    public OutputArtifactSetBaseline OutputBaseline { get; }
}
