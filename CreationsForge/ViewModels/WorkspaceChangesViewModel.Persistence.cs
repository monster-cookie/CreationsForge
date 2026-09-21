using CreationsForge.Core.Engine.Contracts;

namespace CreationsForge.ViewModels;

/// <content>Owns exact save and discard requests, persistence-result mapping, and resumable post-success presentation refresh.</content>
public sealed partial class WorkspaceChangesViewModel
{
    /// <summary>Saves every freshly previewed staged workspace change or materializes a selected new output under the active owned dialog transition.</summary>
    /// <param name="cancellationToken">A token honored before destination mutation; terminal result mapping always drains.</param>
    /// <returns>A task that completes after the exact save result and required presentation refresh are mapped.</returns>
    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (!CanSaveChanges)
        {
            return Task.CompletedTask;
        }

        return RunOperationAsync(
            WorkspaceChangesOperationState.Saving,
            cancellationToken,
            SaveChangesCoreAsync);
    }

    /// <summary>Explicitly discards request-local input or every freshly previewed staged workspace change under the active transition.</summary>
    /// <param name="cancellationToken">A token that cancels before reopened workspace state is published.</param>
    /// <returns>A task that completes after the discard result and required presentation refresh are mapped.</returns>
    public Task DiscardChangesAsync(CancellationToken cancellationToken = default)
    {
        if (!CanDiscardChanges)
        {
            return Task.CompletedTask;
        }

        return RunOperationAsync(
            WorkspaceChangesOperationState.Discarding,
            cancellationToken,
            DiscardChangesCoreAsync);
    }

    /// <summary>Retries only the first incomplete presentation step after a known successful Core transition.</summary>
    /// <param name="cancellationToken">A token that cancels this refresh attempt without replaying persistence.</param>
    /// <returns>A task that completes after the state read or browser refresh attempt is mapped.</returns>
    public Task RetryCommittedRefreshAsync(CancellationToken cancellationToken = default)
    {
        if (!CanRetryCommittedRefresh)
        {
            return Task.CompletedTask;
        }

        return RunOperationAsync(
            WorkspaceChangesOperationState.Refreshing,
            cancellationToken,
            RetryCommittedRefreshCoreAsync);
    }

    /// <summary>Performs a fresh state and preview capture, constructs one immutable save request, and maps its exact result.</summary>
    /// <param name="cancellationToken">The linked operation token.</param>
    /// <returns>A task that completes after persistence and terminal presentation mapping.</returns>
    private async Task SaveChangesCoreAsync(CancellationToken cancellationToken)
    {
        EngineResult<WorkspaceSaveBorrowOutcome> borrowResult;
        try
        {
            borrowResult = await WorkspaceCoordinator.ExecuteAsync(
                async (workspace, token) =>
                {
                    var captureResult = await CaptureReviewWithinBorrowAsync(
                        workspace,
                        allowBlockedSynchronization: false,
                        token).ConfigureAwait(false);
                    if (!captureResult.Succeeded || captureResult.Value is null)
                    {
                        return EngineResult<WorkspaceSaveBorrowOutcome>.Failure(
                            captureResult.Error ?? UnexpectedFailure("The fresh save review returned no value or failure reason."),
                            workspace.WorkspaceId,
                            baseRevision: captureResult.BaseRevision,
                            resultRevision: captureResult.ResultRevision,
                            warnings: captureResult.Warnings);
                    }

                    if (!HasSaveableChanges(captureResult.Value))
                    {
                        return EngineResult<WorkspaceSaveBorrowOutcome>.Failure(
                            new EngineError(EngineErrorCode.InvalidRequest, "No staged workspace changes or new output are available to save."),
                            workspace.WorkspaceId,
                            baseRevision: captureResult.Value.State.Revision,
                            resultRevision: captureResult.Value.State.Revision,
                            warnings: captureResult.Value.Warnings);
                    }

                    var state = captureResult.Value.State;
                    var request = new SaveRequest(Guid.NewGuid(), state.Revision, state.OutputBaseline!);
                    var envelope = new WorkspaceSaveEnvelope(
                        workspace.WorkspaceId,
                        state.Game,
                        state.Release,
                        state.Output!,
                        captureResult.Value.Preview,
                        request);
                    var saveResult = await workspace.SaveAsync(request, token).ConfigureAwait(false);
                    var postSaveState = await ReadPostResultStateAsync(workspace).ConfigureAwait(false);
                    return EngineResult<WorkspaceSaveBorrowOutcome>.Success(
                        new WorkspaceSaveBorrowOutcome(envelope, saveResult, postSaveState),
                        workspace.WorkspaceId,
                        request.OperationId,
                        request.ExpectedRevision,
                        saveResult.ResultRevision,
                        CombineWarnings(captureResult.Value.Warnings, saveResult.Warnings));
                },
                cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            await PublishSuccessAsync("Save stopped before destination files changed.", WarningsValue).ConfigureAwait(false);
            return;
        }

        if (!borrowResult.Succeeded || borrowResult.Value is null)
        {
            await PublishFailureAsync(
                borrowResult.Error ?? UnexpectedFailure("The workspace coordinator returned no save outcome or failure reason."),
                MapPreSaveFailureStatus(borrowResult.Error),
                borrowResult.Warnings).ConfigureAwait(false);
            return;
        }

        var outcome = borrowResult.Value;
        LastSaveResultValue = outcome.Result;
        if (!SaveResultMatchesEnvelope(outcome.Envelope, outcome.Result))
        {
            await PublishFailureAsync(
                UnexpectedFailure("The save returned an inconsistent status, identity, revision, baseline, or recovery-evidence envelope."),
                "Save outcome unknown—recover this save before editing, discarding, or closing.",
                borrowResult.Warnings).ConfigureAwait(false);
            await UiDispatcher.InvokeAsync(() =>
            {
                OutputSynchronizationStatusValue = global::CreationsForge.Core.Engine.Contracts.OutputSynchronizationStatus.RecoveryRequired;
                IsReviewStaleValue = CurrentReviewValue is not null;
                RaiseAllBoundState();
            }).ConfigureAwait(false);
            return;
        }

        var warnings = CombineWarnings(borrowResult.Warnings, outcome.PostSaveState.Warnings);
        switch (outcome.Result.Status)
        {
            case SaveCommitStatus.Committed:
                await MapCommittedSaveAsync(outcome, warnings).ConfigureAwait(false);
                break;
            case SaveCommitStatus.NotCommitted:
                await MapNotCommittedSaveAsync(outcome, warnings).ConfigureAwait(false);
                break;
            case SaveCommitStatus.CommitOutcomeUnknown:
                await MapUnknownSaveAsync(outcome, warnings).ConfigureAwait(false);
                break;
            case SaveCommitStatus.CommittedButReopenFailed:
                await MapCommittedReopenFailureAsync(outcome, warnings).ConfigureAwait(false);
                break;
            default:
                await PublishFailureAsync(
                    UnexpectedFailure("The save returned an undefined commit status."),
                    "Save outcome unknown—recover this save before editing, discarding, or closing.",
                    warnings).ConfigureAwait(false);
                break;
        }
    }

    /// <summary>Gets whether a fresh capture contains record changes or a selected plugin that has not been materialized.</summary>
    /// <param name="capture">The exact state and preview captured at one revision.</param>
    /// <returns><see langword="true"/> when save has work to perform.</returns>
    private static bool HasSaveableChanges(WorkspaceReviewCapture capture)
    {
        return capture.Preview.HasStagedChanges
            || !capture.State.OutputBaseline!.Artifacts
                .Single(artifact => artifact.Role == PluginArtifactRole.Plugin)
                .Fingerprint.Exists;
    }

    /// <summary>Performs a fresh preview and either discards local-only input or calls Core with one exact discard request.</summary>
    /// <param name="cancellationToken">The linked operation token.</param>
    /// <returns>A task that completes after exact discard result mapping.</returns>
    private async Task DiscardChangesCoreAsync(CancellationToken cancellationToken)
    {
        EngineResult<WorkspaceDiscardBorrowOutcome> borrowResult;
        try
        {
            borrowResult = await WorkspaceCoordinator.ExecuteAsync(
                async (workspace, token) =>
                {
                    var captureResult = await CaptureReviewWithinBorrowAsync(
                        workspace,
                        allowBlockedSynchronization: false,
                        token).ConfigureAwait(false);
                    if (!captureResult.Succeeded || captureResult.Value is null)
                    {
                        return EngineResult<WorkspaceDiscardBorrowOutcome>.Failure(
                            captureResult.Error ?? UnexpectedFailure("The fresh discard review returned no value or failure reason."),
                            workspace.WorkspaceId,
                            baseRevision: captureResult.BaseRevision,
                            resultRevision: captureResult.ResultRevision,
                            warnings: captureResult.Warnings);
                    }

                    var capture = captureResult.Value;
                    if (!capture.Preview.HasStagedChanges)
                    {
                        return EngineResult<WorkspaceDiscardBorrowOutcome>.Success(
                            new WorkspaceDiscardBorrowOutcome(capture, envelope: null, result: null),
                            workspace.WorkspaceId,
                            baseRevision: capture.State.Revision,
                            resultRevision: capture.State.Revision,
                            warnings: capture.Warnings);
                    }

                    var request = new DiscardChangesRequest(
                        Guid.NewGuid(),
                        capture.State.Revision,
                        capture.State.OutputBaseline!);
                    var envelope = new WorkspaceDiscardEnvelope(
                        workspace.WorkspaceId,
                        capture.State.Output!,
                        capture.Preview,
                        request);
                    var result = await workspace.DiscardChangesAsync(request, token).ConfigureAwait(false);
                    return EngineResult<WorkspaceDiscardBorrowOutcome>.Success(
                        new WorkspaceDiscardBorrowOutcome(capture, envelope, result),
                        workspace.WorkspaceId,
                        request.OperationId,
                        request.ExpectedRevision,
                        result.ResultRevision,
                        CombineWarnings(capture.Warnings, result.Warnings));
                },
                cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            await PublishSuccessAsync("Discard stopped before reopened workspace state was published.", WarningsValue).ConfigureAwait(false);
            return;
        }

        if (!borrowResult.Succeeded || borrowResult.Value is null)
        {
            await PublishFailureAsync(
                borrowResult.Error ?? UnexpectedFailure("The workspace coordinator returned no discard outcome or failure reason."),
                "Staged workspace changes could not be discarded.",
                borrowResult.Warnings).ConfigureAwait(false);
            return;
        }

        var outcome = borrowResult.Value;
        if (outcome.Envelope is null || outcome.Result is null)
        {
            await UiDispatcher.InvokeAsync(() =>
            {
                if (HasDraftChanges)
                {
                    EditParticipant.DiscardRequestLocalFormChanges();
                }
            }).ConfigureAwait(false);

            await PublishAcceptedReviewCaptureAsync(outcome.Capture, "No unsaved changes").ConfigureAwait(false);
            return;
        }

        if (!DiscardResultMatchesEnvelope(outcome.Envelope, outcome.Result))
        {
            await PublishFailureAsync(
                UnexpectedFailure("The discard returned an inconsistent workspace, operation, or revision envelope."),
                "Staged workspace changes could not be discarded.",
                borrowResult.Warnings).ConfigureAwait(false);
            return;
        }

        if (!outcome.Result.Succeeded || outcome.Result.Value is null)
        {
            if (outcome.Result.Error?.Code == EngineErrorCode.ExternalChangeDetected)
            {
                AbandonmentEnvelopeValue = new WorkspaceAbandonmentEnvelope(
                    outcome.Envelope.WorkspaceId,
                    outcome.Envelope.Output);
            }

            await PublishFailureAsync(
                outcome.Result.Error ?? UnexpectedFailure("The discard returned no receipt or failure reason."),
                MapDiscardFailureStatus(outcome.Result.Error),
                borrowResult.Warnings).ConfigureAwait(false);
            return;
        }

        var refreshEnvelope = new WorkspaceRefreshEnvelope(
            WorkspaceRefreshKind.Discard,
            outcome.Envelope.WorkspaceId,
            outcome.Result.Value.Revision,
            outcome.Envelope.Request.ExpectedBaseline,
            WorkspaceRefreshStep.RefreshParticipant,
            acceptedState: null,
            saveResult: null);
        await RefreshKnownTransitionAsync(
            refreshEnvelope,
            borrowResult.Warnings,
            "Discarded staged changes and reopened the selected output.").ConfigureAwait(false);
    }

    /// <summary>Maps a known committed and reopened save without allowing persistence replay.</summary>
    /// <param name="outcome">The exact save and post-state outcome.</param>
    /// <param name="warnings">All warnings accumulated through the save.</param>
    /// <returns>A task that completes after refresh success or a resumable refresh failure is published.</returns>
    private async Task MapCommittedSaveAsync(
        WorkspaceSaveBorrowOutcome outcome,
        IReadOnlyList<EngineWarning> warnings)
    {
        var result = outcome.Result;
        var stateError = ValidatePostResultState(
            outcome.Envelope.WorkspaceId,
            result.ResultRevision,
            result.CommittedBaseline!,
            global::CreationsForge.Core.Engine.Contracts.OutputSynchronizationStatus.Ready,
            outcome.PostSaveState);
        if (stateError is not null || outcome.PostSaveState.Value is null)
        {
            PendingRefreshEnvelopeValue = new WorkspaceRefreshEnvelope(
                WorkspaceRefreshKind.Save,
                outcome.Envelope.WorkspaceId,
                result.ResultRevision,
                result.CommittedBaseline!,
                WorkspaceRefreshStep.ReadState,
                acceptedState: null,
                result);
            await PublishFailureAsync(
                stateError ?? UnexpectedFailure("The committed save state could not be read."),
                "Saved, but the refreshed workspace state could not be displayed.",
                warnings).ConfigureAwait(false);
            return;
        }

        var refreshEnvelope = new WorkspaceRefreshEnvelope(
            WorkspaceRefreshKind.Save,
            outcome.Envelope.WorkspaceId,
            result.ResultRevision,
            result.CommittedBaseline!,
            WorkspaceRefreshStep.RefreshParticipant,
            outcome.PostSaveState.Value,
            result);
        await RefreshKnownTransitionAsync(refreshEnvelope, warnings, "Saved and reopened.").ConfigureAwait(false);
    }

    /// <summary>Maps a definitive save rejection while preserving staged workspace and editor state.</summary>
    /// <param name="outcome">The exact save and post-state outcome.</param>
    /// <param name="warnings">All warnings accumulated through the save.</param>
    /// <returns>A task that completes after the failure is published.</returns>
    private async Task MapNotCommittedSaveAsync(
        WorkspaceSaveBorrowOutcome outcome,
        IReadOnlyList<EngineWarning> warnings)
    {
        if (outcome.Result.Error?.Code == EngineErrorCode.ExternalChangeDetected)
        {
            AbandonmentEnvelopeValue = new WorkspaceAbandonmentEnvelope(
                outcome.Envelope.WorkspaceId,
                outcome.Envelope.Output);
        }
        else
        {
            AbandonmentEnvelopeValue = null;
        }

        if (outcome.PostSaveState.Succeeded && outcome.PostSaveState.Value is not null)
        {
            OutputSynchronizationStatusValue = outcome.PostSaveState.Value.OutputSynchronization.Status;
        }

        await PublishFailureAsync(
            outcome.Result.Error ?? new EngineError(EngineErrorCode.ValidationFailed, "The save stopped before destination mutation."),
            MapNotCommittedStatus(outcome.Result.Error),
            warnings).ConfigureAwait(false);
    }

    /// <summary>Maps an unknown save outcome and latches the original immutable save identity for recovery.</summary>
    /// <param name="outcome">The exact save and post-state outcome.</param>
    /// <param name="warnings">All warnings accumulated through the save.</param>
    /// <returns>A task that completes after recovery-required state is published.</returns>
    private async Task MapUnknownSaveAsync(
        WorkspaceSaveBorrowOutcome outcome,
        IReadOnlyList<EngineWarning> warnings)
    {
        await UiDispatcher.InvokeAsync(() =>
        {
            OutputSynchronizationStatusValue = global::CreationsForge.Core.Engine.Contracts.OutputSynchronizationStatus.RecoveryRequired;
            IsReviewStaleValue = CurrentReviewValue is not null;
            RaiseAllBoundState();
        }).ConfigureAwait(false);
        await PublishFailureAsync(
            outcome.Result.Error ?? new EngineError(EngineErrorCode.CommitOutcomeUnknown, "The destination save outcome remains unknown."),
            "Save outcome unknown—recover this save before editing, discarding, or closing.",
            warnings).ConfigureAwait(false);
    }

    /// <summary>Maps a committed output that the workspace could not reopen and retains exact evidence for adoption.</summary>
    /// <param name="outcome">The exact save and post-state outcome.</param>
    /// <param name="warnings">All warnings accumulated through the save.</param>
    /// <returns>A task that completes after reopen-required state is published.</returns>
    private async Task MapCommittedReopenFailureAsync(
        WorkspaceSaveBorrowOutcome outcome,
        IReadOnlyList<EngineWarning> warnings)
    {
        var result = outcome.Result;
        var state = outcome.PostSaveState.Value;
        if (state is not null && result.ResolvedEvidence is not null)
        {
            var recoveryResult = new RecoverSaveResult(
                result.WorkspaceId,
                result.OperationId,
                RecoverSaveStatus.Committed,
                result.BaseRevision,
                repairRequired: false,
                result.RecoveryEvidenceToken,
                result.ResolvedEvidence,
                result.Error);
            RecoveryEnvelopeValue = new WorkspaceRecoveryEnvelope(
                outcome.Envelope.WorkspaceId,
                state,
                recoveryResult,
                repairResult: null,
                canResumeStaged: false);
        }

        await UiDispatcher.InvokeAsync(() =>
        {
            OutputSynchronizationStatusValue = global::CreationsForge.Core.Engine.Contracts.OutputSynchronizationStatus.ReopenRequired;
            IsReviewStaleValue = CurrentReviewValue is not null;
            RaiseAllBoundState();
        }).ConfigureAwait(false);
        await PublishFailureAsync(
            result.Error ?? new EngineError(EngineErrorCode.OutputOpenFailed, "The committed output could not be reopened."),
            "Files saved, but the workspace could not reopen them.",
            warnings).ConfigureAwait(false);
    }

    /// <summary>Retries the incomplete state-read or participant-refresh step without calling Core persistence again.</summary>
    /// <param name="cancellationToken">The linked refresh token.</param>
    /// <returns>A task that completes after the retry attempt is mapped.</returns>
    private async Task RetryCommittedRefreshCoreAsync(CancellationToken cancellationToken)
    {
        var envelope = PendingRefreshEnvelopeValue;
        if (envelope is null)
        {
            return;
        }

        if (envelope.Step == WorkspaceRefreshStep.ReadState)
        {
            var stateResult = await ReadCurrentStateAsync(cancellationToken).ConfigureAwait(false);
            var stateError = ValidatePostResultState(
                envelope.WorkspaceId,
                envelope.Revision,
                envelope.Baseline,
                global::CreationsForge.Core.Engine.Contracts.OutputSynchronizationStatus.Ready,
                stateResult);
            if (stateError is not null || stateResult.Value is null)
            {
                await PublishFailureAsync(
                    stateError ?? UnexpectedFailure("The persisted workspace state could not be read."),
                    GetRefreshFailureStatus(envelope.Kind),
                    CombineWarnings(WarningsValue, stateResult.Warnings)).ConfigureAwait(false);
                return;
            }

            envelope = envelope.WithAcceptedState(stateResult.Value);
            PendingRefreshEnvelopeValue = envelope;
        }

        await RefreshKnownTransitionAsync(
            envelope,
            WarningsValue,
            GetRefreshSuccessStatus(envelope.Kind)).ConfigureAwait(false);
    }

    /// <summary>Calls only the participant refresh for a known successful Core transition.</summary>
    /// <param name="envelope">The exact refresh-resume envelope.</param>
    /// <param name="warnings">Warnings retained from persistence and state validation.</param>
    /// <param name="successStatus">The exact status published on complete refresh.</param>
    /// <returns>A task that completes after success or a retained retryable refresh failure.</returns>
    private async Task RefreshKnownTransitionAsync(
        WorkspaceRefreshEnvelope envelope,
        IReadOnlyList<EngineWarning> warnings,
        string successStatus)
    {
        PendingRefreshEnvelopeValue = envelope;
        await PublishOperationStateAsync(
            WorkspaceChangesOperationState.Refreshing,
            IsCancelRequested,
            "Refreshing workspace...").ConfigureAwait(false);
        var refreshResult = await EditParticipant.RefreshAfterWorkspacePersistenceAsync(
            envelope.WorkspaceId,
            envelope.Revision,
            CancellationToken.None).ConfigureAwait(false);
        if (!refreshResult.Succeeded
            || refreshResult.Value is null
            || refreshResult.WorkspaceId != envelope.WorkspaceId
            || refreshResult.ResultRevision != envelope.Revision
            || refreshResult.Value.Revision != envelope.Revision)
        {
            PendingRefreshEnvelopeValue = envelope;
            await PublishFailureAsync(
                refreshResult.Error ?? UnexpectedFailure("The browser refresh did not publish the exact persisted workspace revision."),
                GetRefreshFailureStatus(envelope.Kind),
                CombineWarnings(warnings, refreshResult.Warnings)).ConfigureAwait(false);
            return;
        }

        PendingRefreshEnvelopeValue = null;
        RecoveryEnvelopeValue = null;
        PendingRepairEnvelopeValue = null;
        AbandonmentEnvelopeValue = null;
        Interlocked.Increment(ref CompletedPersistenceVersion);
        await UiDispatcher.InvokeAsync(() =>
        {
            OutputSynchronizationStatusValue = global::CreationsForge.Core.Engine.Contracts.OutputSynchronizationStatus.Ready;
            IsReviewStaleValue = CurrentReviewValue is not null;
            RaiseAllBoundState();
        }).ConfigureAwait(false);
        await PublishSuccessAsync(successStatus, CombineWarnings(warnings, refreshResult.Warnings)).ConfigureAwait(false);
    }

    /// <summary>Reads the current atomic workspace state through the sole coordinator borrow boundary.</summary>
    /// <param name="cancellationToken">A token that cancels waiting or state read.</param>
    /// <returns>The exact state result or typed coordinator failure.</returns>
    private ValueTask<EngineResult<WorkspaceState>> ReadCurrentStateAsync(CancellationToken cancellationToken)
    {
        return WorkspaceCoordinator.ExecuteAsync(
            async (workspace, token) => await workspace.ReadStateAsync(token).ConfigureAwait(false),
            cancellationToken);
    }

    /// <summary>Reads post-mutation state with a non-user-cancelable drain token and detaches unexpected failure.</summary>
    /// <param name="workspace">The still-borrowed live workspace.</param>
    /// <returns>The exact state result or a typed detached failure.</returns>
    private static async ValueTask<EngineResult<WorkspaceState>> ReadPostResultStateAsync(IPluginWorkspace workspace)
    {
        try
        {
            return await workspace.ReadStateAsync(CancellationToken.None).ConfigureAwait(false);
        }
        catch (Exception)
        {
            return EngineResult<WorkspaceState>.Failure(
                UnexpectedFailure("The workspace state could not be read after persistence."),
                workspace.WorkspaceId,
                resultRevision: workspace.Revision);
        }
    }

    /// <summary>Validates the exact workspace identity, revision, baseline, and synchronization after a known result.</summary>
    /// <param name="workspaceId">The expected workspace identity.</param>
    /// <param name="revision">The expected resulting revision.</param>
    /// <param name="baseline">The expected complete output baseline.</param>
    /// <param name="synchronizationStatus">The expected synchronization status.</param>
    /// <param name="result">The post-result state read.</param>
    /// <returns>A typed validation failure, or <see langword="null"/> when every field agrees.</returns>
    private static EngineError? ValidatePostResultState(
        Guid workspaceId,
        WorkspaceRevision revision,
        OutputArtifactSetBaseline baseline,
        OutputSynchronizationStatus synchronizationStatus,
        EngineResult<WorkspaceState> result)
    {
        if (!result.Succeeded || result.Value is null)
        {
            return result.Error ?? UnexpectedFailure("The post-persistence state read returned no state or failure reason.");
        }

        if (result.WorkspaceId != workspaceId
            || result.BaseRevision != revision
            || result.ResultRevision != revision
            || result.Value.Revision != revision
            || result.Value.OutputBaseline is null
            || !OutputBaselinesMatch(baseline, result.Value.OutputBaseline)
            || result.Value.OutputSynchronization.Status != synchronizationStatus)
        {
            return UnexpectedFailure("The post-persistence state did not match the exact accepted workspace, revision, baseline, or synchronization status.");
        }

        return null;
    }

    /// <summary>Validates every status-dependent field returned for one exact save request.</summary>
    /// <param name="envelope">The exact request envelope.</param>
    /// <param name="result">The returned save result.</param>
    /// <returns><see langword="true"/> when identity and status-dependent evidence are consistent.</returns>
    private static bool SaveResultMatchesEnvelope(WorkspaceSaveEnvelope envelope, SaveResult result)
    {
        if (result.WorkspaceId != envelope.WorkspaceId
            || result.OperationId != envelope.Request.OperationId
            || result.BaseRevision != envelope.Request.ExpectedRevision)
        {
            return false;
        }

        return result.Status switch
        {
            SaveCommitStatus.Committed => result.CommittedBaseline is not null
                && result.Error is null
                && result.RecoveryEvidenceToken is null
                && result.ResolvedEvidence is null,
            SaveCommitStatus.NotCommitted => result.ResultRevision == result.BaseRevision
                && result.CommittedBaseline is null
                && result.RecoveryEvidenceToken is null
                && result.ResolvedEvidence is null,
            SaveCommitStatus.CommitOutcomeUnknown => result.ResultRevision == result.BaseRevision
                && result.CommittedBaseline is null
                && result.ResolvedEvidence is null,
            SaveCommitStatus.CommittedButReopenFailed => result.ResultRevision == result.BaseRevision
                && result.CommittedBaseline is not null
                && result.RecoveryEvidenceToken is not null
                && result.ResolvedEvidence is not null
                && result.ResolvedEvidence.Status == RecoverSaveStatus.Committed
                && result.ResolvedEvidence.OriginalWorkspaceId == result.WorkspaceId
                && result.ResolvedEvidence.SaveOperationId == result.OperationId
                && result.ResolvedEvidence.SaveBaseRevision == result.BaseRevision
                && result.ResolvedEvidence.EvidenceToken.Equals(result.RecoveryEvidenceToken)
                && OutputAssociationsMatch(result.ResolvedEvidence.Output, envelope.Output)
                && OutputBaselinesMatch(result.ResolvedEvidence.ResolvedOutputBaseline, result.CommittedBaseline)
                && result.Error is not null,
            _ => false,
        };
    }

    /// <summary>Validates a discard result against its exact immutable request envelope.</summary>
    /// <param name="envelope">The exact discard request envelope.</param>
    /// <param name="result">The exact Core discard result.</param>
    /// <returns><see langword="true"/> when all required result identity fields agree.</returns>
    private static bool DiscardResultMatchesEnvelope(
        WorkspaceDiscardEnvelope envelope,
        EngineResult<OperationReceipt> result)
    {
        if (result.WorkspaceId != envelope.WorkspaceId
            || result.OperationId != envelope.Request.OperationId
            || result.BaseRevision != envelope.Request.ExpectedRevision
            || result.ResultRevision is null)
        {
            return false;
        }

        return result.Succeeded
            ? result.Value is not null
                && result.Value.OperationId == envelope.Request.OperationId
                && result.Value.Revision == result.ResultRevision
            : result.Value is null && result.Error is not null;
    }

    /// <summary>Publishes a newly captured empty review after request-local input alone was discarded.</summary>
    /// <param name="capture">The exact fresh empty review capture.</param>
    /// <param name="status">The exact success status.</param>
    /// <returns>A task that completes after detached projection state is published.</returns>
    private async Task PublishAcceptedReviewCaptureAsync(WorkspaceReviewCapture capture, string status)
    {
        var review = new WorkspaceChangeReview(
            capture.WorkspaceId,
            capture.State.Game,
            capture.State.Release,
            capture.State.Output!,
            capture.State.OutputBaseline!,
            capture.State.Revision,
            capture.Preview.Comparisons,
            Array.Empty<WorkspaceChangeItemViewModel>(),
            capture.Preview.UnresolvedReferenceCount,
            capture.Warnings,
            capture.Preview.MajorRecordComparisons);
        Interlocked.Increment(ref CompletedPersistenceVersion);
        await UiDispatcher.InvokeAsync(() =>
        {
            CurrentReviewValue = review;
            IsReviewStaleValue = false;
            OutputSynchronizationStatusValue = global::CreationsForge.Core.Engine.Contracts.OutputSynchronizationStatus.Ready;
            StatusTextValue = status;
            ErrorCodeValue = null;
            ErrorMessageValue = null;
            WarningsValue = review.Warnings;
            RaiseAllBoundState();
        }).ConfigureAwait(false);
    }

    /// <summary>Maps a coordinator or fresh-review failure before a save request is constructed.</summary>
    /// <param name="error">The typed failure, when available.</param>
    /// <returns>The exact user-visible failure category.</returns>
    private static string MapPreSaveFailureStatus(EngineError? error)
    {
        return error?.Code switch
        {
            EngineErrorCode.ExternalChangeDetected => "The output changed outside CreationsForge. No files were overwritten.",
            EngineErrorCode.OutputDirectoryBusy => "The output folder is in use. No files were changed.",
            _ => "The current workspace could not be prepared for saving.",
        };
    }

    /// <summary>Maps a definitive not-committed save result by typed error code.</summary>
    /// <param name="error">The exact typed save failure.</param>
    /// <returns>The required user-visible status.</returns>
    private static string MapNotCommittedStatus(EngineError? error)
    {
        return error?.Code switch
        {
            EngineErrorCode.ExternalChangeDetected => "The output changed outside CreationsForge. No files were overwritten.",
            EngineErrorCode.OutputDirectoryBusy => "The output folder is in use. No files were changed.",
            EngineErrorCode.OutputOpenFailed => "Save stopped before destination files changed. The selected output could not be opened.",
            _ => "Save stopped before destination files changed.",
        };
    }

    /// <summary>Maps a failed discard by typed error code without parsing error text.</summary>
    /// <param name="error">The exact typed discard failure.</param>
    /// <returns>The required user-visible status.</returns>
    private static string MapDiscardFailureStatus(EngineError? error)
    {
        return error?.Code switch
        {
            EngineErrorCode.ExternalChangeDetected => "The output changed outside CreationsForge. No files were overwritten.",
            EngineErrorCode.OutputDirectoryBusy => "The output folder is in use. No files were changed.",
            _ => "Staged workspace changes could not be discarded.",
        };
    }

    /// <summary>Gets the exact success message for a completed presentation refresh.</summary>
    /// <param name="kind">The known Core transition.</param>
    /// <returns>The exact user-visible success status.</returns>
    private static string GetRefreshSuccessStatus(WorkspaceRefreshKind kind)
    {
        return kind switch
        {
            WorkspaceRefreshKind.Save => "Saved and reopened.",
            WorkspaceRefreshKind.Discard => "Discarded staged changes and reopened the selected output.",
            WorkspaceRefreshKind.RecoveryResume => "Unsaved changes resumed.",
            WorkspaceRefreshKind.RecoveryReopen => "Recovered output reopened.",
            _ => "Workspace refreshed.",
        };
    }

    /// <summary>Gets the exact failure message for an incomplete presentation refresh after a known Core transition.</summary>
    /// <param name="kind">The known Core transition.</param>
    /// <returns>The user-visible refresh failure status.</returns>
    private static string GetRefreshFailureStatus(WorkspaceRefreshKind kind)
    {
        return kind switch
        {
            WorkspaceRefreshKind.Save => "Saved, but the refreshed workspace state could not be displayed.",
            WorkspaceRefreshKind.Discard => "Discarded, but the refreshed workspace state could not be displayed.",
            WorkspaceRefreshKind.RecoveryResume or WorkspaceRefreshKind.RecoveryReopen =>
                "Recovered, but the refreshed workspace state could not be displayed.",
            _ => "The refreshed workspace state could not be displayed.",
        };
    }
}
