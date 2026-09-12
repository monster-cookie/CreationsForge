using CreationsForge.Core.Engine.Contracts;

namespace CreationsForge.ViewModels;

/// <content>Owns read-only save inspection, explicit repair, exact recovery adoption, and terminal outcome mapping.</content>
public sealed partial class NativeWorkspaceChangesViewModel
{
    /// <summary>Inspects the exact pending save identity without mutating destination, staging, backup, or journal files.</summary>
    /// <param name="cancellationToken">A token that cancels read-only state and recovery inspection.</param>
    /// <returns>A task that completes after the exact recovery result is published.</returns>
    public Task InspectSaveOutcomeAsync(CancellationToken cancellationToken = default)
    {
        if (!CanInspectSaveOutcome)
        {
            return Task.CompletedTask;
        }

        return RunOperationAsync(
            NativeWorkspaceChangesOperationState.InspectingRecovery,
            cancellationToken,
            InspectSaveOutcomeCoreAsync);
    }

    /// <summary>Explicitly completes the reviewed prepared output set with a fresh repair identity.</summary>
    /// <param name="cancellationToken">A token honored before the first repair destination mutation.</param>
    /// <returns>A task that completes after the exact repair result is published.</returns>
    public Task CompletePreparedSaveAsync(CancellationToken cancellationToken = default)
    {
        return RepairSaveAsync(RepairSaveDirection.CompletePrepared, cancellationToken);
    }

    /// <summary>Explicitly restores the reviewed pre-save output set with a fresh repair identity.</summary>
    /// <param name="cancellationToken">A token honored before the first repair destination mutation.</param>
    /// <returns>A task that completes after the exact repair result is published.</returns>
    public Task RestorePreviousOutputAsync(CancellationToken cancellationToken = default)
    {
        return RepairSaveAsync(RepairSaveDirection.RestoreBaseline, cancellationToken);
    }

    /// <summary>Adopts not-committed evidence by retaining the original live workspace's staged candidate.</summary>
    /// <param name="cancellationToken">A token that cancels before recovered state is published.</param>
    /// <returns>A task that completes after exact adoption and required browser refresh.</returns>
    public Task ResumeUnsavedChangesAsync(CancellationToken cancellationToken = default)
    {
        if (!CanResumeUnsavedChanges)
        {
            return Task.CompletedTask;
        }

        return RunOperationAsync(
            NativeWorkspaceChangesOperationState.AdoptingRecovery,
            cancellationToken,
            token => AdoptRecoveryAsync(OutputRecoveryAdoptionMode.ResumeStagedAfterNotCommitted, token));
    }

    /// <summary>Adopts terminal recovery evidence by reopening the resolved output and replacing staged state.</summary>
    /// <param name="cancellationToken">A token that cancels before reopened state is published.</param>
    /// <returns>A task that completes after exact adoption and required browser refresh.</returns>
    public Task ReopenResolvedOutputAsync(CancellationToken cancellationToken = default)
    {
        if (!CanReopenResolvedOutput)
        {
            return Task.CompletedTask;
        }

        return RunOperationAsync(
            NativeWorkspaceChangesOperationState.AdoptingRecovery,
            cancellationToken,
            token => AdoptRecoveryAsync(OutputRecoveryAdoptionMode.ReopenResolvedOutput, token));
    }

    /// <summary>Reads the authoritative pending save and inspects its durable journal state.</summary>
    /// <param name="cancellationToken">The linked read-only operation token.</param>
    /// <returns>A task that completes after exact recovery mapping.</returns>
    private async Task InspectSaveOutcomeCoreAsync(CancellationToken cancellationToken)
    {
        var stateResult = await ReadCurrentStateAsync(cancellationToken).ConfigureAwait(false);
        if (!TryGetPendingState(stateResult, out var liveWorkspaceId, out var state, out var pending, out var stateError))
        {
            await PublishFailureAsync(
                stateError!,
                "Save outcome unknown—recover this save before editing, discarding, or closing.",
                stateResult.Warnings).ConfigureAwait(false);
            return;
        }

        RecoverSaveResult result;
        try
        {
            result = await SaveCoordinator.RecoverAsync(
                new RecoverSaveRequest(pending!.OriginalWorkspaceId, pending.SaveOperationId, pending.Output),
                cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            Logger.Error(exception, "Recovery inspection failed for original save {SaveOperationId}", pending!.SaveOperationId);
            await PublishFailureAsync(
                UnexpectedFailure("The original save outcome could not be inspected."),
                "Save outcome unknown—recover this save before editing, discarding, or closing.",
                stateResult.Warnings).ConfigureAwait(false);
            return;
        }

        if (!RecoveryResultMatchesPending(result, pending!))
        {
            await PublishFailureAsync(
                UnexpectedFailure("Recovery inspection returned an inconsistent original save identity, revision, or evidence envelope."),
                "Save outcome unknown—recover this save before editing, discarding, or closing.",
                stateResult.Warnings).ConfigureAwait(false);
            return;
        }

        var canResume = CanResumeOriginalStagedCandidate(liveWorkspaceId, state!, pending!, result.ResolvedEvidence);
        RecoveryEnvelopeValue = new NativeWorkspaceRecoveryEnvelope(
            liveWorkspaceId,
            state!,
            result,
            repairResult: null,
            canResume);
        PendingRepairEnvelopeValue = null;
        await PublishRecoveryResultAsync(result, stateResult.Warnings).ConfigureAwait(false);
    }

    /// <summary>Starts or exactly replays one explicitly selected repair request.</summary>
    /// <param name="direction">The explicit reviewed repair direction.</param>
    /// <param name="cancellationToken">A token honored before the first repair destination mutation.</param>
    /// <returns>A task that completes after exact repair mapping.</returns>
    private Task RepairSaveAsync(RepairSaveDirection direction, CancellationToken cancellationToken)
    {
        if (!CanRepair(direction))
        {
            return Task.CompletedTask;
        }

        return RunOperationAsync(
            NativeWorkspaceChangesOperationState.Repairing,
            cancellationToken,
            token => RepairSaveCoreAsync(direction, token));
    }

    /// <summary>Revalidates live pending identity, performs one exact repair, and retains request identity across response loss.</summary>
    /// <param name="direction">The explicit repair direction.</param>
    /// <param name="cancellationToken">The linked repair token.</param>
    /// <returns>A task that completes after exact repair mapping.</returns>
    private async Task RepairSaveCoreAsync(
        RepairSaveDirection direction,
        CancellationToken cancellationToken)
    {
        var recovery = RecoveryEnvelopeValue;
        if (recovery is null)
        {
            return;
        }

        var stateResult = await ReadCurrentStateAsync(cancellationToken).ConfigureAwait(false);
        if (!TryGetPendingState(stateResult, out var liveWorkspaceId, out var state, out var pending, out var stateError)
            || liveWorkspaceId != recovery.LiveWorkspaceId
            || pending!.OriginalWorkspaceId != recovery.Result.WorkspaceId
            || pending.SaveOperationId != recovery.Result.SaveOperationId)
        {
            await PublishFailureAsync(
                stateError ?? UnexpectedFailure("The live pending save changed after recovery inspection."),
                "Re-inspect the original save before choosing a repair.",
                stateResult.Warnings).ConfigureAwait(false);
            return;
        }

        var repairEnvelope = PendingRepairEnvelopeValue;
        if (repairEnvelope is not null && repairEnvelope.Request.Direction != direction)
        {
            await PublishFailureAsync(
                new EngineError(EngineErrorCode.InvalidRequest, "The unresolved repair request must be replayed with its original direction."),
                "Repair outcome unknown—inspect the original save before another decision.",
                stateResult.Warnings).ConfigureAwait(false);
            return;
        }

        if (repairEnvelope is null)
        {
            var result = recovery.Result;
            var request = new RepairSaveRequest(
                result.WorkspaceId,
                result.SaveOperationId,
                Guid.NewGuid(),
                result.SaveBaseRevision!.Value,
                pending.Output,
                result.EvidenceToken!,
                direction);
            repairEnvelope = new NativeWorkspaceRepairEnvelope(request, recovery);
            PendingRepairEnvelopeValue = repairEnvelope;
        }

        RepairSaveResult repairResult;
        try
        {
            repairResult = await SaveCoordinator.RepairAsync(repairEnvelope.Request, cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            PendingRepairEnvelopeValue = null;
            await PublishSuccessAsync("Repair stopped before destination files changed.", stateResult.Warnings).ConfigureAwait(false);
            return;
        }
        catch (Exception exception)
        {
            Logger.Error(
                exception,
                "Repair response was not received for repair {RepairOperationId} of save {SaveOperationId}",
                repairEnvelope.Request.RepairOperationId,
                repairEnvelope.Request.SaveOperationId);
            await PublishFailureAsync(
                new EngineError(EngineErrorCode.CommitOutcomeUnknown, "The repair response was not received; retry reuses the exact repair request."),
                "Repair outcome unknown—inspect the original save before another decision.",
                stateResult.Warnings).ConfigureAwait(false);
            return;
        }

        if (!RepairResultMatchesRequest(repairResult, repairEnvelope.Request))
        {
            await PublishFailureAsync(
                UnexpectedFailure("Repair returned an inconsistent original save, repair identity, baseline, or resolved-evidence envelope."),
                "Repair outcome unknown—inspect the original save before another decision.",
                stateResult.Warnings).ConfigureAwait(false);
            return;
        }

        PendingRepairEnvelopeValue = null;
        await MapRepairResultAsync(liveWorkspaceId, state!, repairResult, stateResult.Warnings).ConfigureAwait(false);
    }

    /// <summary>Constructs a fresh adoption request inside one workspace borrow and maps its exact result.</summary>
    /// <param name="mode">The explicitly selected recovery adoption mode.</param>
    /// <param name="cancellationToken">The linked adoption token.</param>
    /// <returns>A task that completes after adoption and presentation refresh.</returns>
    private async Task AdoptRecoveryAsync(
        OutputRecoveryAdoptionMode mode,
        CancellationToken cancellationToken)
    {
        var recovery = RecoveryEnvelopeValue;
        var evidence = recovery?.Result.ResolvedEvidence;
        if (recovery is null || evidence is null)
        {
            return;
        }

        EngineResult<NativeWorkspaceAdoptionBorrowOutcome> borrowResult;
        try
        {
            borrowResult = await WorkspaceCoordinator.ExecuteAsync(
                async (workspace, token) =>
                {
                    var stateResult = await workspace.ReadStateAsync(token).ConfigureAwait(false);
                    var stateError = ValidateStateResult(workspace, stateResult, requireOutput: true);
                    if (stateError is not null || stateResult.Value is null)
                    {
                        return EngineResult<NativeWorkspaceAdoptionBorrowOutcome>.Failure(
                            stateError ?? UnexpectedFailure("The live recovery state returned no value."),
                            workspace.WorkspaceId,
                            resultRevision: stateResult.ResultRevision,
                            warnings: stateResult.Warnings);
                    }

                    var state = stateResult.Value;
                    if (mode == OutputRecoveryAdoptionMode.ResumeStagedAfterNotCommitted
                        && !CanResumeOriginalStagedCandidate(
                            workspace.WorkspaceId,
                            state,
                            state.OutputSynchronization.PendingSave,
                            evidence))
                    {
                        return EngineResult<NativeWorkspaceAdoptionBorrowOutcome>.Failure(
                            new EngineError(EngineErrorCode.InvalidRequest, "Unsaved changes can resume only in the matching original recovery-required workspace."),
                            workspace.WorkspaceId,
                            baseRevision: state.Revision,
                            resultRevision: state.Revision,
                            warnings: stateResult.Warnings);
                    }

                    var request = new ResolveOutputRecoveryRequest(Guid.NewGuid(), state.Revision, mode, evidence);
                    var envelope = new NativeWorkspaceAdoptionEnvelope(workspace.WorkspaceId, request);
                    var adoptionResult = await workspace.ResolveOutputRecoveryAsync(request, token).ConfigureAwait(false);
                    var postState = adoptionResult.Succeeded
                        ? await ReadPostResultStateAsync(workspace).ConfigureAwait(false)
                        : stateResult;
                    return EngineResult<NativeWorkspaceAdoptionBorrowOutcome>.Success(
                        new NativeWorkspaceAdoptionBorrowOutcome(envelope, adoptionResult, postState),
                        workspace.WorkspaceId,
                        request.OperationId,
                        request.ExpectedRevision,
                        adoptionResult.ResultRevision,
                        CombineWarnings(stateResult.Warnings, adoptionResult.Warnings));
                },
                cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            await PublishSuccessAsync("Recovery adoption stopped before workspace state changed.", WarningsValue).ConfigureAwait(false);
            return;
        }

        if (!borrowResult.Succeeded || borrowResult.Value is null)
        {
            await PublishFailureAsync(
                borrowResult.Error ?? UnexpectedFailure("The workspace coordinator returned no recovery-adoption outcome."),
                "The resolved output could not be adopted by this workspace.",
                borrowResult.Warnings).ConfigureAwait(false);
            return;
        }

        var outcome = borrowResult.Value;
        if (!AdoptionResultMatchesRequest(outcome.Envelope, outcome.Result, evidence))
        {
            await PublishFailureAsync(
                UnexpectedFailure("Recovery adoption returned an inconsistent workspace, operation, revision, output, or baseline envelope."),
                "The resolved output could not be adopted by this workspace.",
                borrowResult.Warnings).ConfigureAwait(false);
            return;
        }

        if (!outcome.Result.Succeeded || outcome.Result.Value is null)
        {
            if (outcome.Result.Error?.Code == EngineErrorCode.ExternalChangeDetected)
            {
                AbandonmentEnvelopeValue = new NativeWorkspaceAbandonmentEnvelope(
                    outcome.Envelope.WorkspaceId,
                    evidence.Output);
            }

            await PublishFailureAsync(
                outcome.Result.Error ?? UnexpectedFailure("Recovery adoption returned no receipt or failure reason."),
                outcome.Result.Error?.Code == EngineErrorCode.ExternalChangeDetected
                    ? "The output changed outside CreationsForge. No files were overwritten."
                    : "The resolved output could not be adopted by this workspace.",
                borrowResult.Warnings).ConfigureAwait(false);
            return;
        }

        var receipt = outcome.Result.Value;
        var stateError = ValidatePostResultState(
            outcome.Envelope.WorkspaceId,
            receipt.Revision,
            receipt.Baseline,
            global::CreationsForge.Core.Engine.Contracts.OutputSynchronizationStatus.Ready,
            outcome.PostAdoptionState);
        var refreshEnvelope = new NativeWorkspaceRefreshEnvelope(
            mode == OutputRecoveryAdoptionMode.ResumeStagedAfterNotCommitted
                ? NativeWorkspaceRefreshKind.RecoveryResume
                : NativeWorkspaceRefreshKind.RecoveryReopen,
            outcome.Envelope.WorkspaceId,
            receipt.Revision,
            receipt.Baseline,
            stateError is null ? NativeWorkspaceRefreshStep.RefreshParticipant : NativeWorkspaceRefreshStep.ReadState,
            stateError is null ? outcome.PostAdoptionState.Value : null,
            saveResult: null);
        if (stateError is not null)
        {
            PendingRefreshEnvelopeValue = refreshEnvelope;
            await PublishFailureAsync(
                stateError,
                "Recovered, but the refreshed workspace state could not be displayed.",
                borrowResult.Warnings).ConfigureAwait(false);
            return;
        }

        var successStatus = mode == OutputRecoveryAdoptionMode.ResumeStagedAfterNotCommitted
            ? "Unsaved changes resumed."
            : "Recovered output reopened.";
        await RefreshKnownTransitionAsync(refreshEnvelope, borrowResult.Warnings, successStatus).ConfigureAwait(false);
    }

    /// <summary>Maps one exact repair result without automatically choosing a recovery-adoption mode.</summary>
    /// <param name="liveWorkspaceId">The current live workspace identity.</param>
    /// <param name="state">The exact blocked state retained during repair.</param>
    /// <param name="repairResult">The exact repair result.</param>
    /// <param name="warnings">Warnings retained from live-state validation.</param>
    /// <returns>A task that completes after repair state is published.</returns>
    private async Task MapRepairResultAsync(
        Guid liveWorkspaceId,
        WorkspaceState state,
        RepairSaveResult repairResult,
        IReadOnlyList<EngineWarning> warnings)
    {
        switch (repairResult.Status)
        {
            case RepairSaveStatus.PreparedSetCompleted:
            case RepairSaveStatus.BaselineRestored:
                var recoveryStatus = repairResult.Status == RepairSaveStatus.PreparedSetCompleted
                    ? RecoverSaveStatus.Committed
                    : RecoverSaveStatus.NotCommitted;
                var recoveryResult = new RecoverSaveResult(
                    repairResult.WorkspaceId,
                    repairResult.SaveOperationId,
                    recoveryStatus,
                    repairResult.ResolvedEvidence!.SaveBaseRevision,
                    repairRequired: false,
                    repairResult.ResolvedEvidence.EvidenceToken,
                    repairResult.ResolvedEvidence,
                    error: null);
                var pending = state.OutputSynchronization.PendingSave;
                var canResume = recoveryStatus == RecoverSaveStatus.NotCommitted
                    && CanResumeOriginalStagedCandidate(liveWorkspaceId, state, pending, repairResult.ResolvedEvidence);
                RecoveryEnvelopeValue = new NativeWorkspaceRecoveryEnvelope(
                    liveWorkspaceId,
                    state,
                    recoveryResult,
                    repairResult,
                    canResume);
                await PublishSuccessAsync(
                    recoveryStatus == RecoverSaveStatus.Committed
                        ? "The prepared complete output set was committed. Reopen the saved output."
                        : "The previous output is intact.",
                    warnings).ConfigureAwait(false);
                break;
            case RepairSaveStatus.BlockedByExternalChange:
                RecoveryEnvelopeValue = null;
                if (state.Output is not null)
                {
                    AbandonmentEnvelopeValue = new NativeWorkspaceAbandonmentEnvelope(liveWorkspaceId, state.Output);
                }

                await PublishFailureAsync(
                    repairResult.Error ?? new EngineError(EngineErrorCode.ExternalChangeDetected, "Repair refused to overwrite an unrecognized external change."),
                    "The output changed outside CreationsForge. No files were overwritten.",
                    warnings).ConfigureAwait(false);
                break;
            case RepairSaveStatus.NotStarted:
                RecoveryEnvelopeValue = null;
                await PublishFailureAsync(
                    repairResult.Error ?? new EngineError(EngineErrorCode.RepairRequired, "The repair did not begin and the original save remains unresolved."),
                    "Re-inspect the original save before choosing another repair.",
                    warnings).ConfigureAwait(false);
                break;
            case RepairSaveStatus.StillUnknown:
                RecoveryEnvelopeValue = null;
                await PublishFailureAsync(
                    repairResult.Error ?? new EngineError(EngineErrorCode.CommitOutcomeUnknown, "The repair outcome remains unknown."),
                    "Repair outcome unknown—inspect the original save before another decision.",
                    warnings).ConfigureAwait(false);
                break;
            default:
                await PublishFailureAsync(
                    UnexpectedFailure("Repair returned an undefined outcome."),
                    "Repair outcome unknown—inspect the original save before another decision.",
                    warnings).ConfigureAwait(false);
                break;
        }
    }

    /// <summary>Publishes a validated read-only recovery result and its precise available next action.</summary>
    /// <param name="result">The exact recovery result.</param>
    /// <param name="warnings">Warnings retained from workspace-state capture.</param>
    /// <returns>A task that completes after presentation publication.</returns>
    private Task PublishRecoveryResultAsync(RecoverSaveResult result, IReadOnlyList<EngineWarning> warnings)
    {
        return result.Status switch
        {
            RecoverSaveStatus.Committed when result.ResolvedEvidence is not null =>
                PublishSuccessAsync("The complete output was committed. Reopen the saved output.", warnings),
            RecoverSaveStatus.NotCommitted when result.ResolvedEvidence is not null =>
                PublishSuccessAsync("The previous output is intact.", warnings),
            RecoverSaveStatus.Committed or RecoverSaveStatus.NotCommitted =>
                PublishFailureAsync(
                    result.Error ?? UnexpectedFailure("Terminal recovery evidence is not currently adoptable."),
                    result.Status == RecoverSaveStatus.NotCommitted
                        ? "The previous output is intact, but it cannot currently be reopened."
                        : "The saved output is committed, but it cannot currently be reopened.",
                    warnings),
            RecoverSaveStatus.StillUnknown when result.RepairRequired =>
                PublishFailureAsync(
                    result.Error ?? new EngineError(EngineErrorCode.RepairRequired, "The incomplete save requires an explicit repair choice."),
                    "Choose whether to complete the prepared save or restore the previous output.",
                    warnings),
            _ => PublishFailureAsync(
                result.Error ?? new EngineError(EngineErrorCode.CommitOutcomeUnknown, "No recognized terminal or safely repairable save state is available."),
                "Save outcome unknown—recover this save before editing, discarding, or closing.",
                warnings),
        };
    }

    /// <summary>Extracts and validates the authoritative pending save from a fresh atomic state read.</summary>
    /// <param name="stateResult">The fresh coordinator-mediated state result.</param>
    /// <param name="liveWorkspaceId">Receives the current live workspace identity.</param>
    /// <param name="state">Receives the exact blocked state.</param>
    /// <param name="pending">Receives the authoritative pending save.</param>
    /// <param name="error">Receives a typed validation failure.</param>
    /// <returns><see langword="true"/> when every required blocked-state field is present and exact.</returns>
    private static bool TryGetPendingState(
        EngineResult<WorkspaceState> stateResult,
        out Guid liveWorkspaceId,
        out WorkspaceState? state,
        out PendingSaveIdentity? pending,
        out EngineError? error)
    {
        liveWorkspaceId = stateResult.WorkspaceId ?? Guid.Empty;
        state = stateResult.Value;
        pending = state?.OutputSynchronization.PendingSave;
        if (!stateResult.Succeeded || state is null)
        {
            error = stateResult.Error ?? UnexpectedFailure("The blocked workspace state returned no state or failure reason.");
            return false;
        }

        if (liveWorkspaceId == Guid.Empty
            || stateResult.BaseRevision != state.Revision
            || stateResult.ResultRevision != state.Revision
            || state.Output is null
            || state.OutputBaseline is null
            || state.OutputSynchronization.Status == global::CreationsForge.Core.Engine.Contracts.OutputSynchronizationStatus.Ready
            || pending is null
            || !OutputAssociationsMatch(state.Output, pending.Output))
        {
            error = UnexpectedFailure("The blocked workspace state omitted or contradicted its pending save identity, output, baseline, or revision.");
            return false;
        }

        error = null;
        return true;
    }

    /// <summary>Determines whether read-only recovery preserved the exact authoritative pending-save identity.</summary>
    /// <param name="result">The recovery result.</param>
    /// <param name="pending">The authoritative pending identity.</param>
    /// <returns><see langword="true"/> when every status-dependent identity and evidence field agrees.</returns>
    private static bool RecoveryResultMatchesPending(RecoverSaveResult result, PendingSaveIdentity pending)
    {
        if (result.WorkspaceId != pending.OriginalWorkspaceId
            || result.SaveOperationId != pending.SaveOperationId
            || result.SaveBaseRevision is not null && result.SaveBaseRevision != pending.SaveBaseRevision)
        {
            return false;
        }

        if (result.ResolvedEvidence is null)
        {
            return true;
        }

        return result.ResolvedEvidence.OriginalWorkspaceId == pending.OriginalWorkspaceId
            && result.ResolvedEvidence.SaveOperationId == pending.SaveOperationId
            && result.ResolvedEvidence.SaveBaseRevision == pending.SaveBaseRevision
            && result.ResolvedEvidence.Game == pending.Game
            && result.ResolvedEvidence.Release == pending.Release
            && OutputAssociationsMatch(result.ResolvedEvidence.Output, pending.Output);
    }

    /// <summary>Determines whether a live workspace is the exact original retained candidate eligible for staged resume.</summary>
    /// <param name="liveWorkspaceId">The current live workspace identity.</param>
    /// <param name="state">The exact current blocked workspace state.</param>
    /// <param name="pending">The exact pending save identity.</param>
    /// <param name="evidence">The exact terminal recovery evidence.</param>
    /// <returns><see langword="true"/> only when every original-workspace, pending-save, output, and not-committed precondition agrees.</returns>
    private static bool CanResumeOriginalStagedCandidate(
        Guid liveWorkspaceId,
        WorkspaceState state,
        PendingSaveIdentity? pending,
        ResolvedOutputEvidence? evidence)
    {
        return pending is not null
            && evidence is not null
            && evidence.Status == RecoverSaveStatus.NotCommitted
            && state.OutputSynchronization.Status == global::CreationsForge.Core.Engine.Contracts.OutputSynchronizationStatus.RecoveryRequired
            && liveWorkspaceId == pending.OriginalWorkspaceId
            && pending.OriginalWorkspaceId == evidence.OriginalWorkspaceId
            && pending.SaveOperationId == evidence.SaveOperationId
            && pending.SaveBaseRevision == evidence.SaveBaseRevision
            && state.Output is not null
            && state.OutputBaseline is not null
            && OutputAssociationsMatch(state.Output, pending.Output)
            && OutputAssociationsMatch(state.Output, evidence.Output);
    }

    /// <summary>Validates one exact repair result against its immutable request.</summary>
    /// <param name="result">The returned repair result.</param>
    /// <param name="request">The exact repair request.</param>
    /// <returns><see langword="true"/> when identities and terminal evidence agree.</returns>
    private static bool RepairResultMatchesRequest(RepairSaveResult result, RepairSaveRequest request)
    {
        if (result.WorkspaceId != request.WorkspaceId
            || result.SaveOperationId != request.SaveOperationId
            || result.RepairOperationId != request.RepairOperationId)
        {
            return false;
        }

        return result.Status switch
        {
            RepairSaveStatus.PreparedSetCompleted => result.ResultingBaseline is not null
                && result.ResolvedEvidence is not null
                && result.ResolvedEvidence.Status == RecoverSaveStatus.Committed
                && OutputBaselinesMatch(result.ResultingBaseline, result.ResolvedEvidence.ResolvedOutputBaseline),
            RepairSaveStatus.BaselineRestored => result.ResultingBaseline is not null
                && result.ResolvedEvidence is not null
                && result.ResolvedEvidence.Status == RecoverSaveStatus.NotCommitted
                && OutputBaselinesMatch(result.ResultingBaseline, result.ResolvedEvidence.ResolvedOutputBaseline),
            RepairSaveStatus.BlockedByExternalChange or RepairSaveStatus.NotStarted or RepairSaveStatus.StillUnknown =>
                result.ResolvedEvidence is null,
            _ => false,
        };
    }

    /// <summary>Validates one exact recovery-adoption result against its immutable request and evidence.</summary>
    /// <param name="envelope">The exact adoption request envelope.</param>
    /// <param name="result">The returned adoption result.</param>
    /// <param name="evidence">The exact reviewed terminal evidence.</param>
    /// <returns><see langword="true"/> when every required identity, output, baseline, and revision agrees.</returns>
    private static bool AdoptionResultMatchesRequest(
        NativeWorkspaceAdoptionEnvelope envelope,
        EngineResult<OutputSelectionReceipt> result,
        ResolvedOutputEvidence evidence)
    {
        if (result.WorkspaceId != envelope.WorkspaceId
            || result.OperationId != envelope.Request.OperationId
            || result.BaseRevision != envelope.Request.ExpectedRevision
            || result.ResultRevision is null)
        {
            return false;
        }

        if (!result.Succeeded)
        {
            return result.Value is null && result.Error is not null;
        }

        return result.Value is not null
            && result.Value.Revision == result.ResultRevision
            && OutputAssociationsMatch(result.Value.Output, evidence.Output)
            && OutputBaselinesMatch(result.Value.Baseline, evidence.ResolvedOutputBaseline);
    }
}
