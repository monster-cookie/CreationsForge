using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.PluginInputs;

namespace CreationsForge.Core.Engine;

/// <summary>
/// Owns explicit evidence-validated output recovery adoption for one isolated workspace.
/// </summary>
public sealed partial class FormListWorkspace
{
    /// <inheritdoc />
    public async ValueTask<EngineResult<OutputSelectionReceipt>> ResolveOutputRecoveryAsync(
        ResolveOutputRecoveryRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        var fingerprint = FingerprintFactory.Create(WorkspaceId, request);
        await OperationGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (TryReplay(request.OperationId, fingerprint, out EngineResult<OutputSelectionReceipt>? replay, out var conflict, out var expired))
            {
                return replay!;
            }

            if (expired)
            {
                return CreateOperationReplayExpiredFailure<OutputSelectionReceipt>(request.OperationId, request.ExpectedRevision);
            }

            if (conflict)
            {
                return CreateReuseFailure<OutputSelectionReceipt>(request.OperationId, request.ExpectedRevision);
            }

            if (!CanStoreFinalizationOperation(request.OperationId))
            {
                return CreateOperationCapacityFailure<OutputSelectionReceipt>(request.OperationId, request.ExpectedRevision);
            }

            var guardFailure = ValidateRecoveryMutation(request.OperationId, request.ExpectedRevision);
            if (guardFailure is not null)
            {
                return StoreRecoveryFailure(request, fingerprint, guardFailure);
            }

            var compatibilityFailure = ValidateRecoveryCompatibility(request.Mode, request.Evidence);
            if (compatibilityFailure is not null)
            {
                return StoreRecoveryFailure(request, fingerprint, compatibilityFailure);
            }

            var leaseResult = await AcquireOutputDirectoryLeaseAsync(
                request.Evidence.Output,
                cancellationToken).ConfigureAwait(false);
            if (!leaseResult.Succeeded || leaseResult.Value is null)
            {
                return StoreRecoveryFailure(
                    request,
                    fingerprint,
                    leaseResult.Error ?? new EngineError(EngineErrorCode.UnexpectedFailure, "The output-directory lease provider returned no lease."),
                    leaseResult.Warnings);
            }

            await using var outputLease = leaseResult.Value;
            EngineResult<ResolvedOutputEvidence> validationResult;
            try
            {
                validationResult = await SaveCoordinator.ValidateResolvedEvidenceAsync(
                    outputLease,
                    request.Evidence,
                    cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                Logger.Error(
                    exception,
                    "Failed to validate resolved output evidence in workspace {WorkspaceId} for operation {OperationId}",
                    WorkspaceId,
                    request.OperationId);
                return StoreRecoveryFailure(
                    request,
                    fingerprint,
                    new EngineError(EngineErrorCode.UnexpectedFailure, "Resolved output evidence could not be validated."),
                    leaseResult.Warnings);
            }

            var warnings = CombineWarnings(leaseResult.Warnings, validationResult.Warnings);
            if (!validationResult.Succeeded || validationResult.Value is null)
            {
                return StoreRecoveryFailure(
                    request,
                    fingerprint,
                    validationResult.Error ?? new EngineError(EngineErrorCode.NoRecoveryEvidence, "Resolved output evidence validation returned no result."),
                    warnings);
            }

            var validatedEvidence = validationResult.Value;
            if (!ResolvedEvidenceClaimsMatch(request.Evidence, validatedEvidence))
            {
                return StoreRecoveryFailure(
                    request,
                    fingerprint,
                    new EngineError(EngineErrorCode.NoRecoveryEvidence, "Validated recovery evidence differs from the caller-supplied terminal identity."),
                    warnings);
            }

            compatibilityFailure = ValidateRecoveryCompatibility(request.Mode, validatedEvidence);
            if (compatibilityFailure is not null)
            {
                return StoreRecoveryFailure(request, fingerprint, compatibilityFailure, warnings);
            }

            var sourceVerification = await VerifySourcesUnchangedAsync(cancellationToken).ConfigureAwait(false);
            warnings = CombineWarnings(warnings, sourceVerification.Warnings);
            if (!sourceVerification.Succeeded
                || sourceVerification.Value is null
                || !SourceBaselinesMatch(Sources!.Baseline, sourceVerification.Value)
                || !SourceBaselinesMatch(validatedEvidence.SourceBaseline, sourceVerification.Value))
            {
                return StoreRecoveryFailure(
                    request,
                    fingerprint,
                    sourceVerification.Error ?? new EngineError(EngineErrorCode.ExternalChangeDetected, "The live plugin sources do not match the resolved save evidence."),
                    warnings);
            }

            return request.Mode == OutputRecoveryAdoptionMode.ResumeStagedAfterNotCommitted
                ? await ResumeStagedOutputAsync(request, fingerprint, outputLease, validatedEvidence, warnings, cancellationToken).ConfigureAwait(false)
                : await ReopenResolvedOutputAsync(request, fingerprint, outputLease, validatedEvidence, warnings, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            OperationGate.Release();
        }
    }

    /// <summary>Validates mutation identity and revision while intentionally permitting a recovery latch.</summary>
    /// <param name="operationId">The required recovery-adoption operation identifier.</param>
    /// <param name="expectedRevision">The exact caller-observed live revision.</param>
    /// <returns>A typed guard failure, or <see langword="null"/> when recovery may proceed.</returns>
    private EngineError? ValidateRecoveryMutation(Guid operationId, WorkspaceRevision expectedRevision)
    {
        if (Disposed)
        {
            return new EngineError(EngineErrorCode.WorkspaceDisposed, "The workspace has already been disposed.");
        }

        if (operationId == Guid.Empty)
        {
            return new EngineError(EngineErrorCode.InvalidRequest, "Recovery adoption requires a non-empty operation identifier.");
        }

        if (expectedRevision != CurrentRevision)
        {
            return new EngineError(EngineErrorCode.RevisionConflict, "The workspace revision differs from the caller's expected recovery revision.");
        }

        return null;
    }

    /// <summary>Validates workspace, pending-save, source, and output compatibility before trusting terminal evidence.</summary>
    /// <param name="mode">The requested recovery adoption behavior.</param>
    /// <param name="evidence">The caller-supplied or coordinator-validated evidence claims.</param>
    /// <returns>A typed compatibility failure, or <see langword="null"/> when evidence may be validated or adopted.</returns>
    private EngineError? ValidateRecoveryCompatibility(
        OutputRecoveryAdoptionMode mode,
        ResolvedOutputEvidence evidence)
    {
        if (evidence.Game != Request.Game || evidence.Release != Request.Release)
        {
            return new EngineError(EngineErrorCode.InvalidRequest, "Resolved output evidence targets a different game or engine release.");
        }

        if (!SourceBaselinesMatch(evidence.SourceBaseline, Sources!.Baseline))
        {
            return new EngineError(EngineErrorCode.ExternalChangeDetected, "Resolved output evidence targets a different plugin source baseline.");
        }

        var pendingSave = CurrentOutputSynchronization.PendingSave;
        if (pendingSave is not null && !ResolvedEvidenceMatchesPendingSave(evidence, pendingSave))
        {
            return new EngineError(EngineErrorCode.InvalidRequest, "Resolved output evidence does not match the workspace's pending save identity.");
        }

        if (SelectedOutput is not null && !OutputAssociationsMatch(evidence.Output, SelectedOutput))
        {
            return new EngineError(EngineErrorCode.InvalidRequest, "Resolved output evidence targets a different selected output.");
        }

        if (mode == OutputRecoveryAdoptionMode.ResumeStagedAfterNotCommitted)
        {
            if (evidence.Status != RecoverSaveStatus.NotCommitted)
            {
                return new EngineError(EngineErrorCode.InvalidRequest, "Staged output can resume only after a terminal not-committed recovery result.");
            }

            if (CurrentOutputSynchronization.Status != OutputSynchronizationStatus.RecoveryRequired
                || pendingSave is null
                || pendingSave.OriginalWorkspaceId != WorkspaceId
                || Output is null
                || SelectedOutput is null
                || SelectedOutputBaseline is null)
            {
                return new EngineError(EngineErrorCode.InvalidRequest, "Staged output can resume only in the original latched workspace with its pending candidate.");
            }

            return null;
        }

        if (mode != OutputRecoveryAdoptionMode.ReopenResolvedOutput)
        {
            return new EngineError(EngineErrorCode.InvalidRequest, "The recovery adoption mode is invalid.");
        }

        return null;
    }

    /// <summary>Retains the original staged candidate after proving the destination remained logically unchanged.</summary>
    /// <param name="request">The guarded recovery adoption request.</param>
    /// <param name="fingerprint">The complete canonical request fingerprint.</param>
    /// <param name="lease">The caller-owned output-directory lease.</param>
    /// <param name="evidence">The coordinator-validated not-committed evidence.</param>
    /// <param name="warnings">Warnings accumulated before engine reopen proof.</param>
    /// <param name="cancellationToken">A token that cancels before publication.</param>
    /// <returns>The resumed output receipt or a typed failure retaining the latch.</returns>
    private async ValueTask<EngineResult<OutputSelectionReceipt>> ResumeStagedOutputAsync(
        ResolveOutputRecoveryRequest request,
        OperationFingerprint fingerprint,
        IOutputDirectoryLease lease,
        ResolvedOutputEvidence evidence,
        IReadOnlyList<EngineWarning> warnings,
        CancellationToken cancellationToken)
    {
        _ = lease;
        var proofResult = await ReopenRecoveryEvidenceAsync(evidence, cancellationToken).ConfigureAwait(false);
        warnings = CombineWarnings(warnings, proofResult.Warnings);
        if (!proofResult.Succeeded || proofResult.Value is null)
        {
            return StoreRecoveryFailure(
                request,
                fingerprint,
                proofResult.Error ?? new EngineError(EngineErrorCode.OutputOpenFailed, "The restored output could not be reopened for engine proof."),
                warnings);
        }

        await DisposeOpenedOutputIfCanceledAsync(proofResult.Value, cancellationToken).ConfigureAwait(false);
        if (!OutputAssociationsMatch(evidence.Output, proofResult.Value.Association)
            || !BaselinesMatch(evidence.ResolvedOutputBaseline, proofResult.Value.Baseline))
        {
            await DisposeRejectedOpenedOutputAsync(proofResult.Value, "recovery resume proof validation").ConfigureAwait(false);
            return StoreRecoveryFailure(
                request,
                fingerprint,
                new EngineError(EngineErrorCode.ExternalChangeDetected, "The restored output did not match the exact resolved association and baseline."),
                warnings);
        }

        await DisposeRejectedOpenedOutputAsync(proofResult.Value, "completed recovery resume proof").ConfigureAwait(false);
        cancellationToken.ThrowIfCancellationRequested();
        var baseRevision = CurrentRevision;
        var resultRevision = CreateOutputRevision(evidence.Output, evidence.ResolvedOutputBaseline);
        SelectedOutputBaseline = evidence.ResolvedOutputBaseline;
        SetRevision(resultRevision);
        SetOutputSynchronization(OutputSynchronizationStatus.Ready, null);
        var receipt = new OutputSelectionReceipt(SelectedOutput!, SelectedOutputBaseline, resultRevision);
        return StoreFinalization(request.OperationId, fingerprint, EngineResult<OutputSelectionReceipt>.Success(
            receipt,
            WorkspaceId,
            request.OperationId,
            baseRevision,
            resultRevision,
            warnings));
    }

    /// <summary>Directly reopens terminal resolved output state and replaces any selected candidate atomically.</summary>
    /// <param name="request">The guarded recovery adoption request.</param>
    /// <param name="fingerprint">The complete canonical request fingerprint.</param>
    /// <param name="lease">The caller-owned output-directory lease.</param>
    /// <param name="evidence">The coordinator-validated terminal evidence.</param>
    /// <param name="warnings">Warnings accumulated before engine reopening.</param>
    /// <param name="cancellationToken">A token that cancels before publication.</param>
    /// <returns>The reopened output receipt or a typed failure preserving existing state.</returns>
    private async ValueTask<EngineResult<OutputSelectionReceipt>> ReopenResolvedOutputAsync(
        ResolveOutputRecoveryRequest request,
        OperationFingerprint fingerprint,
        IOutputDirectoryLease lease,
        ResolvedOutputEvidence evidence,
        IReadOnlyList<EngineWarning> warnings,
        CancellationToken cancellationToken)
    {
        _ = lease;
        var openResult = await ReopenRecoveryEvidenceAsync(evidence, cancellationToken).ConfigureAwait(false);
        warnings = CombineWarnings(warnings, openResult.Warnings);
        if (!openResult.Succeeded || openResult.Value is null)
        {
            return StoreRecoveryFailure(
                request,
                fingerprint,
                openResult.Error ?? new EngineError(EngineErrorCode.OutputOpenFailed, "The resolved output could not be reopened."),
                warnings);
        }

        await DisposeOpenedOutputIfCanceledAsync(openResult.Value, cancellationToken).ConfigureAwait(false);
        if (!OutputAssociationsMatch(evidence.Output, openResult.Value.Association)
            || !BaselinesMatch(evidence.ResolvedOutputBaseline, openResult.Value.Baseline))
        {
            await DisposeRejectedOpenedOutputAsync(openResult.Value, "resolved recovery output validation").ConfigureAwait(false);
            return StoreRecoveryFailure(
                request,
                fingerprint,
                new EngineError(EngineErrorCode.ExternalChangeDetected, "The reopened output did not match the exact resolved association and baseline."),
                warnings);
        }

        var baseRevision = CurrentRevision;
        var resultRevision = CreateOutputRevision(openResult.Value.Association, openResult.Value.Baseline);
        var disposalWarnings = await PublishOutputAsync(openResult.Value, true, resultRevision).ConfigureAwait(false);
        SetOutputSynchronization(OutputSynchronizationStatus.Ready, null);
        var receipt = new OutputSelectionReceipt(SelectedOutput!, SelectedOutputBaseline!, resultRevision);
        return StoreFinalization(request.OperationId, fingerprint, EngineResult<OutputSelectionReceipt>.Success(
            receipt,
            WorkspaceId,
            request.OperationId,
            baseRevision,
            resultRevision,
            CombineWarnings(warnings, disposalWarnings)));
    }

    /// <summary>Asks the adapter to reopen the exact terminal output baseline for engine proof.</summary>
    /// <param name="evidence">The coordinator-validated terminal evidence.</param>
    /// <param name="cancellationToken">A token that cancels before engine state is returned.</param>
    /// <returns>The independently owned reopened engine state or a typed adapter failure.</returns>
    private async ValueTask<EngineResult<PluginOutputOpenResult>> ReopenRecoveryEvidenceAsync(
        ResolvedOutputEvidence evidence,
        CancellationToken cancellationToken)
    {
        try
        {
            return await Task.Run(
                async () => await Adapter.ReopenOutputAsync(
                    Sources!,
                    evidence.Output,
                    evidence.ResolvedOutputBaseline,
                    cancellationToken).ConfigureAwait(false)).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            Logger.Error(
                exception,
                "Failed to reopen resolved plugin output in workspace {WorkspaceId}",
                WorkspaceId);
            return EngineResult<PluginOutputOpenResult>.Failure(new EngineError(
                EngineErrorCode.OutputOpenFailed,
                "The resolved plugin output could not be reopened."));
        }
    }

    /// <summary>Stores a typed recovery adoption failure with the unchanged live revision.</summary>
    /// <param name="request">The guarded recovery request.</param>
    /// <param name="fingerprint">The complete canonical request fingerprint.</param>
    /// <param name="error">The typed failure.</param>
    /// <param name="warnings">Optional accumulated warnings.</param>
    /// <returns>The stored replayable failure.</returns>
    private EngineResult<OutputSelectionReceipt> StoreRecoveryFailure(
        ResolveOutputRecoveryRequest request,
        OperationFingerprint fingerprint,
        EngineError error,
        IReadOnlyList<EngineWarning>? warnings = null)
    {
        return StoreFinalization(request.OperationId, fingerprint, EngineResult<OutputSelectionReceipt>.Failure(
            error,
            WorkspaceId,
            request.OperationId,
            request.ExpectedRevision,
            CurrentRevision,
            warnings));
    }

    /// <summary>Determines whether coordinator validation preserved every caller-supplied evidence claim.</summary>
    /// <param name="expected">The untrusted caller-supplied evidence.</param>
    /// <param name="actual">The coordinator-validated evidence.</param>
    /// <returns><see langword="true"/> when every identity and complete baseline field matches.</returns>
    private static bool ResolvedEvidenceClaimsMatch(
        ResolvedOutputEvidence expected,
        ResolvedOutputEvidence actual)
    {
        return expected.EvidenceToken.Equals(actual.EvidenceToken)
            && expected.Game == actual.Game
            && expected.Release == actual.Release
            && expected.OriginalWorkspaceId == actual.OriginalWorkspaceId
            && expected.SaveOperationId == actual.SaveOperationId
            && expected.SaveBaseRevision == actual.SaveBaseRevision
            && SourceBaselinesMatch(expected.SourceBaseline, actual.SourceBaseline)
            && OutputAssociationsMatch(expected.Output, actual.Output)
            && BaselinesMatch(expected.ResolvedOutputBaseline, actual.ResolvedOutputBaseline)
            && expected.Status == actual.Status;
    }

    /// <summary>Determines whether terminal evidence matches the workspace's exact pending save.</summary>
    /// <param name="evidence">The terminal evidence.</param>
    /// <param name="pendingSave">The workspace's pending save identity.</param>
    /// <returns><see langword="true"/> when every shared identity field matches.</returns>
    private static bool ResolvedEvidenceMatchesPendingSave(
        ResolvedOutputEvidence evidence,
        PendingSaveIdentity pendingSave)
    {
        return evidence.OriginalWorkspaceId == pendingSave.OriginalWorkspaceId
            && evidence.SaveOperationId == pendingSave.SaveOperationId
            && evidence.SaveBaseRevision == pendingSave.SaveBaseRevision
            && evidence.Game == pendingSave.Game
            && evidence.Release == pendingSave.Release
            && OutputAssociationsMatch(evidence.Output, pendingSave.Output);
    }

    /// <summary>Determines whether terminal evidence matches every coordinator save-envelope field.</summary>
    /// <param name="evidence">The terminal evidence.</param>
    /// <param name="pendingSave">The immutable original save identity.</param>
    /// <param name="sourceBaseline">The exact source baseline lent to the coordinator.</param>
    /// <param name="resolvedBaseline">The exact terminal output baseline.</param>
    /// <param name="evidenceToken">The coordinator-issued evidence token.</param>
    /// <param name="status">The required terminal recovery status.</param>
    /// <returns><see langword="true"/> when every identity and complete baseline field matches.</returns>
    private static bool ResolvedEvidenceMatches(
        ResolvedOutputEvidence evidence,
        PendingSaveIdentity pendingSave,
        PluginSourceInputBaseline sourceBaseline,
        OutputArtifactSetBaseline resolvedBaseline,
        RecoveryEvidenceToken evidenceToken,
        RecoverSaveStatus status)
    {
        return evidence.EvidenceToken.Equals(evidenceToken)
            && ResolvedEvidenceMatchesPendingSave(evidence, pendingSave)
            && SourceBaselinesMatch(evidence.SourceBaseline, sourceBaseline)
            && BaselinesMatch(evidence.ResolvedOutputBaseline, resolvedBaseline)
            && evidence.Status == status;
    }
}
