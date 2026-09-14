namespace CreationsForge.Core.Engine.Contracts;

/// <summary>
/// Reports a guarded save attempt, including commit knowledge that remains meaningful after response loss.
/// </summary>
public sealed class SaveResult
{
    /// <summary>Initializes a guarded save result.</summary>
    /// <param name="workspaceId">The workspace that initiated the save.</param>
    /// <param name="operationId">The save operation identifier.</param>
    /// <param name="baseRevision">The revision against which the save ran.</param>
    /// <param name="resultRevision">The resulting workspace revision.</param>
    /// <param name="status">What is known about destination commitment.</param>
    /// <param name="committedBaseline">The complete committed baseline when known.</param>
    /// <param name="recoveryEvidenceToken">The opaque evidence token when recovery inspection or repair is available.</param>
    /// <param name="resolvedEvidence">Terminal recognized evidence for explicit workspace adoption when reopen remains required.</param>
    /// <param name="error">The typed failure, or <see langword="null"/> for a fully committed and reopened save.</param>
    /// <param name="warnings">Non-fatal save and reopen warnings.</param>
    /// <exception cref="ArgumentException">Thrown when the workspace or operation identifier is empty or resolved evidence disagrees with the save outcome.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="warnings"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="status"/> is undefined.</exception>
    public SaveResult(
        Guid workspaceId,
        Guid operationId,
        WorkspaceRevision baseRevision,
        WorkspaceRevision resultRevision,
        SaveCommitStatus status,
        OutputArtifactSetBaseline? committedBaseline,
        RecoveryEvidenceToken? recoveryEvidenceToken,
        ResolvedOutputEvidence? resolvedEvidence,
        EngineError? error,
        IReadOnlyList<EngineWarning> warnings)
    {
        if (workspaceId == Guid.Empty)
        {
            throw new ArgumentException("A save result requires a non-empty workspace identifier.", nameof(workspaceId));
        }

        if (operationId == Guid.Empty)
        {
            throw new ArgumentException("A save result requires a non-empty operation identifier.", nameof(operationId));
        }

        if (!Enum.IsDefined(status))
        {
            throw new ArgumentOutOfRangeException(nameof(status));
        }

        if (status == SaveCommitStatus.CommittedButReopenFailed && resolvedEvidence is null)
        {
            throw new ArgumentException("A committed save whose reopen failed requires terminal resolved evidence.", nameof(resolvedEvidence));
        }

        if (resolvedEvidence is not null
            && (status is not SaveCommitStatus.Committed and not SaveCommitStatus.CommittedButReopenFailed
                || resolvedEvidence.OriginalWorkspaceId != workspaceId
                || resolvedEvidence.SaveOperationId != operationId
                || resolvedEvidence.SaveBaseRevision != baseRevision
                || resolvedEvidence.Status != RecoverSaveStatus.Committed
                || recoveryEvidenceToken is null
                || !resolvedEvidence.EvidenceToken.Equals(recoveryEvidenceToken)
                || committedBaseline is null
                || resolvedEvidence.ResolvedOutputBaseline.BaselineId != committedBaseline.BaselineId))
        {
            throw new ArgumentException("Resolved save evidence must match the save identity, base revision, committed status, and committed baseline.", nameof(resolvedEvidence));
        }

        ArgumentNullException.ThrowIfNull(warnings);
        WorkspaceId = workspaceId;
        OperationId = operationId;
        BaseRevision = baseRevision;
        ResultRevision = resultRevision;
        Status = status;
        CommittedBaseline = committedBaseline;
        RecoveryEvidenceToken = recoveryEvidenceToken;
        ResolvedEvidence = resolvedEvidence;
        Error = error;
        Warnings = Array.AsReadOnly(warnings.ToArray());
    }

    /// <summary>Gets the workspace that initiated the save.</summary>
    public Guid WorkspaceId { get; }

    /// <summary>Gets the save operation identifier.</summary>
    public Guid OperationId { get; }

    /// <summary>Gets the revision against which the save ran.</summary>
    public WorkspaceRevision BaseRevision { get; }

    /// <summary>Gets the resulting workspace revision.</summary>
    public WorkspaceRevision ResultRevision { get; }

    /// <summary>Gets what is known about destination commitment.</summary>
    public SaveCommitStatus Status { get; }

    /// <summary>Gets the complete committed output baseline when known.</summary>
    public OutputArtifactSetBaseline? CommittedBaseline { get; }

    /// <summary>Gets the evidence token for recovery inspection or repair, when available.</summary>
    public RecoveryEvidenceToken? RecoveryEvidenceToken { get; }

    /// <summary>Gets terminal recognized evidence for explicit workspace adoption when reopen remains required.</summary>
    public ResolvedOutputEvidence? ResolvedEvidence { get; }

    /// <summary>Gets the typed failure or incomplete-result detail, when applicable.</summary>
    public EngineError? Error { get; }

    /// <summary>Gets immutable save and reopen warnings.</summary>
    public IReadOnlyList<EngineWarning> Warnings { get; }
}
