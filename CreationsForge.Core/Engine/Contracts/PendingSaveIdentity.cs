using CreationsForge.Core.Enums;
using Mutagen.Bethesda;

namespace CreationsForge.Core.Engine.Contracts;

/// <summary>Identifies the original save that blocks ordinary output operations until explicitly resolved.</summary>
public sealed class PendingSaveIdentity
{
    /// <summary>Initializes an immutable pending-save identity.</summary>
    /// <param name="originalWorkspaceId">The workspace that initiated the save.</param>
    /// <param name="saveOperationId">The original save operation identifier.</param>
    /// <param name="saveBaseRevision">The workspace revision journaled before the save.</param>
    /// <param name="game">The exact CreationsForge game recorded by the save.</param>
    /// <param name="release">The exact native release recorded by the save.</param>
    /// <param name="output">The exact output association recorded by the save.</param>
    /// <exception cref="ArgumentException">Thrown when a workspace or operation identifier is empty.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="output"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="game"/> or <paramref name="release"/> is undefined.</exception>
    public PendingSaveIdentity(
        Guid originalWorkspaceId,
        Guid saveOperationId,
        WorkspaceRevision saveBaseRevision,
        SupportedGame game,
        GameRelease release,
        OutputAssociation output)
    {
        if (originalWorkspaceId == Guid.Empty)
        {
            throw new ArgumentException("A pending save requires a non-empty original workspace identifier.", nameof(originalWorkspaceId));
        }

        if (saveOperationId == Guid.Empty)
        {
            throw new ArgumentException("A pending save requires a non-empty save operation identifier.", nameof(saveOperationId));
        }

        if (!Enum.IsDefined(game))
        {
            throw new ArgumentOutOfRangeException(nameof(game));
        }

        if (!Enum.IsDefined(release))
        {
            throw new ArgumentOutOfRangeException(nameof(release));
        }

        ArgumentNullException.ThrowIfNull(output);
        OriginalWorkspaceId = originalWorkspaceId;
        SaveOperationId = saveOperationId;
        SaveBaseRevision = saveBaseRevision;
        Game = game;
        Release = release;
        Output = output;
    }

    /// <summary>Gets the workspace that initiated the pending save.</summary>
    public Guid OriginalWorkspaceId { get; }

    /// <summary>Gets the original save operation identifier.</summary>
    public Guid SaveOperationId { get; }

    /// <summary>Gets the workspace revision journaled before the save.</summary>
    public WorkspaceRevision SaveBaseRevision { get; }

    /// <summary>Gets the exact CreationsForge game recorded by the save.</summary>
    public SupportedGame Game { get; }

    /// <summary>Gets the exact native release recorded by the save.</summary>
    public GameRelease Release { get; }

    /// <summary>Gets the exact output association recorded by the save.</summary>
    public OutputAssociation Output { get; }
}
