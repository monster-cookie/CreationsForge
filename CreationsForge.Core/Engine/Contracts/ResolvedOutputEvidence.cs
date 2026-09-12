using CreationsForge.Core.Engine.NativeInputs;
using CreationsForge.Core.Enums;
using Mutagen.Bethesda;

namespace CreationsForge.Core.Engine.Contracts;

/// <summary>
/// Carries terminal recovery evidence without exposing native record state; a workspace must revalidate it before adoption.
/// </summary>
public sealed class ResolvedOutputEvidence
{
    /// <summary>Initializes immutable terminal evidence for one recognized save journal.</summary>
    /// <param name="evidenceToken">The opaque token binding this evidence to the verified journal state.</param>
    /// <param name="game">The exact CreationsForge game recorded by the save.</param>
    /// <param name="release">The exact native release recorded by the save.</param>
    /// <param name="originalWorkspaceId">The workspace that initiated the save.</param>
    /// <param name="saveOperationId">The original save operation identifier.</param>
    /// <param name="saveBaseRevision">The workspace revision journaled before the save.</param>
    /// <param name="sourceBaseline">The complete source baseline recorded by the save.</param>
    /// <param name="output">The exact output association recorded by the save.</param>
    /// <param name="resolvedOutputBaseline">The complete terminal destination baseline.</param>
    /// <param name="status">Whether the prepared set committed or the original baseline remains committed.</param>
    /// <exception cref="ArgumentException">Thrown when a workspace or operation identifier is empty.</exception>
    /// <exception cref="ArgumentNullException">Thrown when a required evidence component is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when a game, release, or non-terminal status is supplied.</exception>
    public ResolvedOutputEvidence(
        RecoveryEvidenceToken evidenceToken,
        SupportedGame game,
        GameRelease release,
        Guid originalWorkspaceId,
        Guid saveOperationId,
        WorkspaceRevision saveBaseRevision,
        NativeSourceInputBaseline sourceBaseline,
        OutputAssociation output,
        OutputArtifactSetBaseline resolvedOutputBaseline,
        RecoverSaveStatus status)
    {
        if (originalWorkspaceId == Guid.Empty)
        {
            throw new ArgumentException("Resolved output evidence requires a non-empty original workspace identifier.", nameof(originalWorkspaceId));
        }

        if (saveOperationId == Guid.Empty)
        {
            throw new ArgumentException("Resolved output evidence requires a non-empty save operation identifier.", nameof(saveOperationId));
        }

        if (!Enum.IsDefined(game))
        {
            throw new ArgumentOutOfRangeException(nameof(game));
        }

        if (!Enum.IsDefined(release))
        {
            throw new ArgumentOutOfRangeException(nameof(release));
        }

        if (status is not RecoverSaveStatus.Committed and not RecoverSaveStatus.NotCommitted)
        {
            throw new ArgumentOutOfRangeException(nameof(status), "Resolved output evidence requires a terminal recovery status.");
        }

        ArgumentNullException.ThrowIfNull(evidenceToken);
        ArgumentNullException.ThrowIfNull(sourceBaseline);
        ArgumentNullException.ThrowIfNull(output);
        ArgumentNullException.ThrowIfNull(resolvedOutputBaseline);
        EvidenceToken = evidenceToken;
        Game = game;
        Release = release;
        OriginalWorkspaceId = originalWorkspaceId;
        SaveOperationId = saveOperationId;
        SaveBaseRevision = saveBaseRevision;
        SourceBaseline = sourceBaseline;
        Output = output;
        ResolvedOutputBaseline = resolvedOutputBaseline;
        Status = status;
    }

    /// <summary>Gets the opaque token binding this evidence to the verified journal state.</summary>
    public RecoveryEvidenceToken EvidenceToken { get; }

    /// <summary>Gets the exact CreationsForge game recorded by the save.</summary>
    public SupportedGame Game { get; }

    /// <summary>Gets the exact native release recorded by the save.</summary>
    public GameRelease Release { get; }

    /// <summary>Gets the workspace that initiated the save.</summary>
    public Guid OriginalWorkspaceId { get; }

    /// <summary>Gets the original save operation identifier.</summary>
    public Guid SaveOperationId { get; }

    /// <summary>Gets the workspace revision journaled before the save.</summary>
    public WorkspaceRevision SaveBaseRevision { get; }

    /// <summary>Gets the complete source baseline recorded by the save.</summary>
    public NativeSourceInputBaseline SourceBaseline { get; }

    /// <summary>Gets the exact output association recorded by the save.</summary>
    public OutputAssociation Output { get; }

    /// <summary>Gets the complete terminal destination baseline verified by the coordinator.</summary>
    public OutputArtifactSetBaseline ResolvedOutputBaseline { get; }

    /// <summary>Gets whether the prepared set committed or the original baseline remains committed.</summary>
    public RecoverSaveStatus Status { get; }
}
