namespace CreationsForge.Core.Engine.Contracts;

/// <summary>
/// Coordinates guarded recoverable multi-file saves, read-only recovery inspection, and explicit repair.
/// </summary>
public interface IWorkspaceSaveCoordinator
{
    /// <summary>Inspects stable save metadata while the caller holds the output-directory lease, before plugin output opening.</summary>
    /// <param name="lease">The caller-owned lease for the request's output directory.</param>
    /// <param name="request">The exact workspace, source, game, release, and output identity seeking admission.</param>
    /// <param name="cancellationToken">A token that cancels metadata inspection.</param>
    /// <returns>Ready admission or the original unresolved save that requires recovery.</returns>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is requested.</exception>
    ValueTask<EngineResult<OutputAdmissionResult>> InspectOutputAdmissionAsync(
        IOutputDirectoryLease lease,
        OutputAdmissionRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>Stages, reopens through the game adapter, and guardedly commits a complete plugin-and-strings output set.</summary>
    /// <param name="context">Borrowed live workspace state held under the workspace operation gate.</param>
    /// <param name="request">The guarded save request.</param>
    /// <param name="cancellationToken">A token that may cancel before the first destination mutation.</param>
    /// <returns>The exact known or unknown destination commit outcome.</returns>
    ValueTask<SaveResult> SaveAsync(
        WorkspaceSaveContext context,
        SaveRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>Inspects a prior save without mutating destination, staging, backup, or journal files.</summary>
    /// <param name="request">The original workspace, save operation, and output identity.</param>
    /// <param name="cancellationToken">A token that cancels read-only recovery inspection.</param>
    /// <returns>The observed commitment state and repair evidence when applicable.</returns>
    ValueTask<RecoverSaveResult> RecoverAsync(
        RecoverSaveRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>Mutates only a recognized incomplete transaction after rechecking reviewed recovery evidence.</summary>
    /// <param name="request">The explicit complete-or-restore repair decision with a new operation identifier.</param>
    /// <param name="cancellationToken">A token that may cancel before the first repair destination mutation.</param>
    /// <returns>The repair result, including unknown outcomes after the mutation boundary.</returns>
    ValueTask<RepairSaveResult> RepairAsync(
        RepairSaveRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>Rechecks terminal recovery evidence and durably finalizes a physically resolved nonterminal journal while the caller holds the output-directory lease.</summary>
    /// <param name="lease">The caller-owned lease for the evidence output directory.</param>
    /// <param name="evidence">The terminal evidence whose token, identity, revisions, and baselines must still match.</param>
    /// <param name="cancellationToken">A token that cancels evidence validation or the terminal journal write before that write starts.</param>
    /// <returns>The same validated evidence after any required terminal journal projection is durable, or a typed stale, mismatched, unavailable-evidence, or persistence failure.</returns>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is requested.</exception>
    ValueTask<EngineResult<ResolvedOutputEvidence>> ValidateResolvedEvidenceAsync(
        IOutputDirectoryLease lease,
        ResolvedOutputEvidence evidence,
        CancellationToken cancellationToken = default);
}
