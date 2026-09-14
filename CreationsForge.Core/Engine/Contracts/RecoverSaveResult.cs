namespace CreationsForge.Core.Engine.Contracts;

/// <summary>
/// Reports durable commit knowledge separately from currently adoptable recovery evidence.
/// </summary>
public sealed class RecoverSaveResult
{
    /// <summary>Initializes a read-only save-recovery result.</summary>
    /// <param name="workspaceId">The original caller workspace identifier.</param>
    /// <param name="saveOperationId">The original save operation identifier.</param>
    /// <param name="status">The observed commitment state.</param>
    /// <param name="saveBaseRevision">The original revision recorded by a recognized journal, or <see langword="null"/> when no evidence exists.</param>
    /// <param name="repairRequired">Whether a recognized incomplete transaction requires explicit repair.</param>
    /// <param name="evidenceToken">The reviewed evidence token required for repair, when available.</param>
    /// <param name="resolvedEvidence">Terminal recognized evidence that a workspace may currently adopt, when available.</param>
    /// <param name="error">The typed recovery detail, when applicable.</param>
    /// <exception cref="ArgumentException">Thrown when an identifier is empty or the supplied revision, token, repair, and resolved-evidence state is inconsistent.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="status"/> is undefined.</exception>
    public RecoverSaveResult(
        Guid workspaceId,
        Guid saveOperationId,
        RecoverSaveStatus status,
        WorkspaceRevision? saveBaseRevision,
        bool repairRequired,
        RecoveryEvidenceToken? evidenceToken,
        ResolvedOutputEvidence? resolvedEvidence,
        EngineError? error)
    {
        if (workspaceId == Guid.Empty)
        {
            throw new ArgumentException("A recovery result requires a non-empty workspace identifier.", nameof(workspaceId));
        }

        if (saveOperationId == Guid.Empty)
        {
            throw new ArgumentException("A recovery result requires a non-empty save operation identifier.", nameof(saveOperationId));
        }

        if (!Enum.IsDefined(status))
        {
            throw new ArgumentOutOfRangeException(nameof(status));
        }

        if (saveBaseRevision is null && (repairRequired || evidenceToken is not null || resolvedEvidence is not null))
        {
            throw new ArgumentException("Recovery details cannot carry repair or resolved evidence without a recognized journal revision.", nameof(saveBaseRevision));
        }

        if (repairRequired && (status != RecoverSaveStatus.StillUnknown || evidenceToken is null))
        {
            throw new ArgumentException("Repair-required recovery must remain unknown and include its reviewed evidence token.", nameof(repairRequired));
        }

        if (status is RecoverSaveStatus.Committed or RecoverSaveStatus.NotCommitted
            && (saveBaseRevision is null || repairRequired))
        {
            throw new ArgumentException("A terminal recovery result requires its recognized journal revision and cannot require repair.", nameof(status));
        }

        if (status is RecoverSaveStatus.Committed or RecoverSaveStatus.NotCommitted
            && resolvedEvidence is null
            && error is null)
        {
            throw new ArgumentException("Terminal commit knowledge without adoptable evidence requires a typed current-conflict detail.", nameof(error));
        }

        if (status == RecoverSaveStatus.StillUnknown && resolvedEvidence is not null)
        {
            throw new ArgumentException("An unknown recovery result cannot carry terminal resolved evidence.", nameof(resolvedEvidence));
        }

        if (resolvedEvidence is not null
            && (resolvedEvidence.OriginalWorkspaceId != workspaceId
                || resolvedEvidence.SaveOperationId != saveOperationId
                || resolvedEvidence.SaveBaseRevision != saveBaseRevision
                || resolvedEvidence.Status != status
                || evidenceToken is null
                || !resolvedEvidence.EvidenceToken.Equals(evidenceToken)))
        {
            throw new ArgumentException("Resolved recovery evidence must match the result's original save identity, revision, status, and token.", nameof(resolvedEvidence));
        }

        WorkspaceId = workspaceId;
        SaveOperationId = saveOperationId;
        Status = status;
        SaveBaseRevision = saveBaseRevision;
        RepairRequired = repairRequired;
        EvidenceToken = evidenceToken;
        ResolvedEvidence = resolvedEvidence;
        Error = error;
    }

    /// <summary>Gets the original caller workspace identifier.</summary>
    public Guid WorkspaceId { get; }

    /// <summary>Gets the original save operation identifier.</summary>
    public Guid SaveOperationId { get; }

    /// <summary>Gets durable knowledge of whether the original save committed, independently of current adoptability.</summary>
    public RecoverSaveStatus Status { get; }

    /// <summary>Gets the original revision recorded by a recognized journal, or <see langword="null"/> when no evidence exists.</summary>
    public WorkspaceRevision? SaveBaseRevision { get; }

    /// <summary>Gets whether a recognized incomplete transaction requires explicit repair.</summary>
    public bool RepairRequired { get; }

    /// <summary>Gets the reviewed evidence token required for repair, when available.</summary>
    public RecoveryEvidenceToken? EvidenceToken { get; }

    /// <summary>Gets terminal recognized evidence that a workspace may currently adopt.</summary>
    public ResolvedOutputEvidence? ResolvedEvidence { get; }

    /// <summary>Gets the typed recovery detail, when applicable.</summary>
    public EngineError? Error { get; }
}
