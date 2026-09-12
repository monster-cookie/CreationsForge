namespace CreationsForge.Core.Engine.Contracts;

/// <summary>
/// Reports an explicit incomplete-save repair and any remaining recovery evidence.
/// </summary>
public sealed class RepairSaveResult
{
    /// <summary>Initializes an explicit save-repair result.</summary>
    /// <param name="workspaceId">The original caller workspace identifier.</param>
    /// <param name="saveOperationId">The original incomplete save operation identifier.</param>
    /// <param name="repairOperationId">The repair idempotency identifier.</param>
    /// <param name="status">The observed repair outcome.</param>
    /// <param name="resultingBaseline">The complete resulting baseline when known.</param>
    /// <param name="evidenceToken">Updated evidence for a still-incomplete transaction, when available.</param>
    /// <param name="resolvedEvidence">Terminal recognized evidence that a workspace may explicitly adopt, when available.</param>
    /// <param name="error">The typed failure or incomplete-result detail, when applicable.</param>
    /// <exception cref="ArgumentException">Thrown when any operation or workspace identifier is empty or resolved evidence disagrees with the repair outcome.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="status"/> is undefined.</exception>
    public RepairSaveResult(
        Guid workspaceId,
        Guid saveOperationId,
        Guid repairOperationId,
        RepairSaveStatus status,
        OutputArtifactSetBaseline? resultingBaseline,
        RecoveryEvidenceToken? evidenceToken,
        ResolvedOutputEvidence? resolvedEvidence,
        EngineError? error)
    {
        if (workspaceId == Guid.Empty || saveOperationId == Guid.Empty || repairOperationId == Guid.Empty)
        {
            throw new ArgumentException("A repair result requires non-empty workspace, save, and repair operation identifiers.");
        }

        if (!Enum.IsDefined(status))
        {
            throw new ArgumentOutOfRangeException(nameof(status));
        }

        var expectedRecoveryStatus = status switch
        {
            RepairSaveStatus.PreparedSetCompleted => RecoverSaveStatus.Committed,
            RepairSaveStatus.BaselineRestored => RecoverSaveStatus.NotCommitted,
            _ => (RecoverSaveStatus?)null
        };
        if (expectedRecoveryStatus is not null && (resultingBaseline is null || resolvedEvidence is null))
        {
            throw new ArgumentException("A completed repair requires its resulting baseline and terminal resolved evidence.", nameof(status));
        }

        if (expectedRecoveryStatus is null && resolvedEvidence is not null)
        {
            throw new ArgumentException("A non-terminal repair result cannot carry resolved evidence.", nameof(resolvedEvidence));
        }

        if (resolvedEvidence is not null
            && (resolvedEvidence.OriginalWorkspaceId != workspaceId
                || resolvedEvidence.SaveOperationId != saveOperationId
                || expectedRecoveryStatus is null
                || resolvedEvidence.Status != expectedRecoveryStatus
                || resultingBaseline is null
                || resolvedEvidence.ResolvedOutputBaseline.BaselineId != resultingBaseline.BaselineId))
        {
            throw new ArgumentException("Resolved repair evidence must match the original save identity, terminal repair outcome, and resulting baseline.", nameof(resolvedEvidence));
        }

        WorkspaceId = workspaceId;
        SaveOperationId = saveOperationId;
        RepairOperationId = repairOperationId;
        Status = status;
        ResultingBaseline = resultingBaseline;
        EvidenceToken = evidenceToken;
        ResolvedEvidence = resolvedEvidence;
        Error = error;
    }

    /// <summary>Gets the original caller workspace identifier.</summary>
    public Guid WorkspaceId { get; }

    /// <summary>Gets the original incomplete save operation identifier.</summary>
    public Guid SaveOperationId { get; }

    /// <summary>Gets the repair idempotency identifier.</summary>
    public Guid RepairOperationId { get; }

    /// <summary>Gets the observed repair outcome.</summary>
    public RepairSaveStatus Status { get; }

    /// <summary>Gets the complete resulting baseline when known.</summary>
    public OutputArtifactSetBaseline? ResultingBaseline { get; }

    /// <summary>Gets updated evidence for a still-incomplete transaction, when available.</summary>
    public RecoveryEvidenceToken? EvidenceToken { get; }

    /// <summary>Gets terminal recognized evidence that a workspace may explicitly adopt.</summary>
    public ResolvedOutputEvidence? ResolvedEvidence { get; }

    /// <summary>Gets the typed failure or incomplete-result detail, when applicable.</summary>
    public EngineError? Error { get; }
}
