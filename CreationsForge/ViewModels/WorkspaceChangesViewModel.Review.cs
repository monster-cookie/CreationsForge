using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Services;

namespace CreationsForge.ViewModels;

/// <content>Owns modal admission, fresh state and preview capture, detached projection, and exact identity checks.</content>
public sealed partial class WorkspaceChangesViewModel
{
    /// <summary>Opens the shared dialog with a fresh revision-consistent review.</summary>
    /// <param name="cancellationToken">A token that cancels transition admission or read-only review work.</param>
    /// <returns>A task that completes after the modal review session closes and releases its untransferred transition.</returns>
    public Task ReviewChangesAsync(CancellationToken cancellationToken = default)
    {
        return ShowDialogSessionAsync(WorkspaceChangesDialogRequest.ForReview(), cancellationToken);
    }

    /// <summary>Opens a save-purpose dialog after capturing a fresh revision-consistent review.</summary>
    /// <param name="cancellationToken">A token that cancels transition admission or read-only review work.</param>
    /// <returns>A task that completes after the modal session closes and releases its untransferred transition.</returns>
    public Task ShowSaveChangesDialogAsync(CancellationToken cancellationToken = default)
    {
        return ShowDialogSessionAsync(WorkspaceChangesDialogRequest.ForSave(), cancellationToken);
    }

    /// <summary>Opens a discard-purpose dialog after capturing a fresh revision-consistent review.</summary>
    /// <param name="cancellationToken">A token that cancels transition admission or read-only review work.</param>
    /// <returns>A task that completes after the modal session closes and releases its untransferred transition.</returns>
    public Task ShowDiscardChangesDialogAsync(CancellationToken cancellationToken = default)
    {
        return ShowDialogSessionAsync(WorkspaceChangesDialogRequest.ForDiscard(), cancellationToken);
    }

    /// <summary>Runs one top-level non-leave modal session under a single exact transition lease.</summary>
    /// <param name="request">The immutable closed dialog request.</param>
    /// <param name="cancellationToken">A token that cancels admission, review, or dialog waiting.</param>
    /// <returns>A task that completes after untransferred admission is released.</returns>
    private async Task ShowDialogSessionAsync(
        WorkspaceChangesDialogRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (!TryBeginDialogSession(request))
        {
            return;
        }

        try
        {
            await RunOperationAsync(
                WorkspaceChangesOperationState.ReservingTransition,
                cancellationToken,
                async token =>
                {
                    await AcquireTransitionAsync(WorkspaceTransitionDrainMode.WaitForCurrentOperation, token).ConfigureAwait(false);
                    if (!HasActiveOwnedTransition)
                    {
                        return;
                    }

                    await PublishOperationStateAsync(
                        WorkspaceChangesOperationState.Reviewing,
                        false,
                        "Reviewing changes...").ConfigureAwait(false);
                    await CaptureAndPublishReviewAsync(token, allowBlockedSynchronization: true).ConfigureAwait(false);
                }).ConfigureAwait(false);

            if (!HasActiveOwnedTransition || IsShutdownRequested)
            {
                return;
            }

            await DialogService.ShowAsync(this, request, cancellationToken).ConfigureAwait(false);
            await WaitForActiveOperationAsync().ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            await WaitForActiveOperationAsync().ConfigureAwait(false);
        }
        finally
        {
            await ReleaseUntransferredTransitionAsync().ConfigureAwait(false);
            EndDialogSession();
        }
    }

    /// <summary>Atomically admits one modal session before it begins transition acquisition.</summary>
    /// <param name="request">The exact active dialog request.</param>
    /// <returns><see langword="true"/> when the caller owns the newly admitted session.</returns>
    private bool TryBeginDialogSession(
        WorkspaceChangesDialogRequest request,
        bool requireWorkspace = true)
    {
        lock (LifecycleGate)
        {
            if (IsDialogSessionActive
                || ActiveOperationTask is not null
                || IsShutdownRequested
                || requireWorkspace && WorkspaceCoordinator.CurrentWorkspace is null)
            {
                return false;
            }

            IsDialogSessionActive = true;
            ActiveDialogRequest = request;
            PendingLeaveDisposition = null;
            return true;
        }
    }

    /// <summary>Clears modal-only state after the dialog and any nested operation have drained.</summary>
    private void EndDialogSession()
    {
        lock (LifecycleGate)
        {
            IsDialogSessionActive = false;
            ActiveDialogRequest = null;
            PendingLeaveDisposition = null;
        }

        UiDispatcher.Post(RaiseAllBoundState);
    }

    /// <summary>Acquires and stores one exact transition lease without holding a coordinator borrow.</summary>
    /// <param name="drainMode">Whether an existing editor operation waits naturally or receives cancellation.</param>
    /// <param name="cancellationToken">A token that cancels the unacquired transition waiter.</param>
    /// <returns>A task that completes after transition admission is owned.</returns>
    private async Task AcquireTransitionAsync(
        WorkspaceTransitionDrainMode drainMode,
        CancellationToken cancellationToken)
    {
        var lease = await PresentationArbiter.ReserveWorkspaceTransitionAsync(drainMode, cancellationToken).ConfigureAwait(false);
        lock (LifecycleGate)
        {
            if (IsShutdownRequested || OwnedTransitionLease is not null)
            {
                lease.Dispose();
                return;
            }

            OwnedTransitionLease = lease;
        }

        await UiDispatcher.InvokeAsync(RaiseAllBoundState).ConfigureAwait(false);
    }

    /// <summary>Acquires and stores one leave-only transition request before the captured editor is drained.</summary>
    /// <param name="cancellationToken">A token that cancels pending transition-gate acquisition.</param>
    /// <returns>A task that completes after the request owns transition admission.</returns>
    private async Task AcquireTransitionRequestAsync(CancellationToken cancellationToken)
    {
        var request = await PresentationArbiter.BeginWorkspaceTransitionRequestAsync(cancellationToken).ConfigureAwait(false);
        var accepted = false;
        lock (LifecycleGate)
        {
            if (!IsShutdownRequested && OwnedTransitionRequest is null && OwnedTransitionLease is null)
            {
                OwnedTransitionRequest = request;
                accepted = true;
            }
        }

        if (!accepted)
        {
            await request.DisposeAsync().ConfigureAwait(false);
            return;
        }

        await UiDispatcher.InvokeAsync(RaiseAllBoundState).ConfigureAwait(false);
    }

    /// <summary>Drains the editor captured by the pending request and transfers its continuous admission into a transition lease.</summary>
    /// <param name="drainMode">Whether the captured editor waits naturally or receives a cancellation request before draining.</param>
    /// <returns><see langword="true"/> when the request transferred one active lease to this view model.</returns>
    private async Task<bool> CompleteOwnedTransitionRequestAsync(WorkspaceTransitionDrainMode drainMode)
    {
        WorkspaceTransitionRequest? request;
        lock (LifecycleGate)
        {
            request = OwnedTransitionRequest;
        }

        if (request is null)
        {
            return false;
        }

        WorkspaceTransitionLease? lease = null;
        var accepted = false;
        try
        {
            lease = await request.CompleteAsync(drainMode, CancellationToken.None).ConfigureAwait(false);
            lock (LifecycleGate)
            {
                if (!IsShutdownRequested
                    && ReferenceEquals(OwnedTransitionRequest, request)
                    && OwnedTransitionLease is null)
                {
                    OwnedTransitionRequest = null;
                    OwnedTransitionLease = lease;
                    accepted = true;
                }
            }

            if (!accepted)
            {
                lease.Dispose();
                lease = null;
                throw new InvalidOperationException("The completed workspace transition request could not transfer its admission ownership.");
            }

            await UiDispatcher.InvokeAsync(RaiseAllBoundState).ConfigureAwait(false);
            return true;
        }
        finally
        {
            lock (LifecycleGate)
            {
                if (ReferenceEquals(OwnedTransitionRequest, request))
                {
                    OwnedTransitionRequest = null;
                }
            }

            if (!accepted)
            {
                lease?.Dispose();
            }

            await request.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>Waits for the current Task 21 operation without requesting cancellation.</summary>
    /// <returns>A task that completes after the current operation drains.</returns>
    private async Task WaitForActiveOperationAsync()
    {
        Task? active;
        lock (LifecycleGate)
        {
            active = ActiveOperationTask;
        }

        if (active is not null)
        {
            await active.ConfigureAwait(false);
        }
    }

    /// <summary>Abandons an exact pending request or releases an exact transition lease still owned by this view model.</summary>
    /// <returns>A task that completes after pending request cleanup releases its transition admission.</returns>
    private async Task ReleaseUntransferredTransitionAsync()
    {
        WorkspaceTransitionRequest? request;
        WorkspaceTransitionLease? lease;
        lock (LifecycleGate)
        {
            request = OwnedTransitionRequest;
            OwnedTransitionRequest = null;
            lease = OwnedTransitionLease;
            OwnedTransitionLease = null;
        }

        if (request is not null)
        {
            await request.DisposeAsync().ConfigureAwait(false);
        }

        lease?.Dispose();
        UiDispatcher.Post(RaiseAllBoundState);
    }

    /// <summary>Captures and publishes either a ready preview or the exact blocked synchronization state.</summary>
    /// <param name="cancellationToken">A token that cancels read-only state, preview, or projection work.</param>
    /// <param name="allowBlockedSynchronization">Whether recovery and reopen states should publish without a preview.</param>
    /// <returns><see langword="true"/> when ready state and preview were accepted and projected.</returns>
    private async Task<bool> CaptureAndPublishReviewAsync(
        CancellationToken cancellationToken,
        bool allowBlockedSynchronization)
    {
        var result = await WorkspaceCoordinator.ExecuteAsync(
            async (workspace, token) => await CaptureReviewWithinBorrowAsync(
                workspace,
                allowBlockedSynchronization,
                token).ConfigureAwait(false),
            cancellationToken).ConfigureAwait(false);

        if (!result.Succeeded || result.Value is null)
        {
            await PublishFailureAsync(
                result.Error ?? UnexpectedFailure("The workspace coordinator returned no review or failure reason."),
                "The current workspace changes could not be reviewed.",
                result.Warnings).ConfigureAwait(false);
            return false;
        }

        var capture = result.Value;
        await UiDispatcher.InvokeAsync(() =>
        {
            OutputSynchronizationStatusValue = capture.State.OutputSynchronization.Status;
            WarningsValue = SnapshotWarnings(capture.Warnings);
            ErrorCodeValue = null;
            ErrorMessageValue = null;
            if (capture.State.OutputSynchronization.Status != global::CreationsForge.Core.Engine.Contracts.OutputSynchronizationStatus.Ready)
            {
                IsReviewStaleValue = CurrentReviewValue is not null;
                StatusTextValue = capture.State.OutputSynchronization.Status == global::CreationsForge.Core.Engine.Contracts.OutputSynchronizationStatus.RecoveryRequired
                    ? "Save outcome unknown—recover this save before editing, discarding, or closing."
                    : "Files saved, but the workspace could not reopen them.";
            }

            RaiseAllBoundState();
        }).ConfigureAwait(false);

        if (capture.State.OutputSynchronization.Status != global::CreationsForge.Core.Engine.Contracts.OutputSynchronizationStatus.Ready)
        {
            return false;
        }

        IReadOnlyList<WorkspaceChangeItemViewModel> items;
        try
        {
            items = Array.AsReadOnly(capture.Preview.Comparisons
                .Select(comparison => new WorkspaceChangeItemViewModel(
                    comparison,
                    JsonTreeProjectionService.Project(comparison.Before, cancellationToken),
                    JsonTreeProjectionService.Project(comparison.After, cancellationToken)))
                .Concat(capture.Preview.MajorRecordComparisons.Select(comparison => new WorkspaceChangeItemViewModel(
                    comparison,
                    JsonTreeProjectionService.Project(comparison.Before, cancellationToken),
                    JsonTreeProjectionService.Project(comparison.After, cancellationToken))))
                .ToArray());
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            Logger.Error(exception, "Detached plugin change projection failed for workspace {WorkspaceId}", capture.WorkspaceId);
            await PublishFailureAsync(
                UnexpectedFailure("The detached plugin change review could not be displayed."),
                "The current workspace changes could not be reviewed.",
                capture.Warnings).ConfigureAwait(false);
            return false;
        }

        var warnings = CombineWarnings(
            capture.Warnings,
            capture.Preview.Comparisons.SelectMany(comparison => comparison.Warnings)
                .Concat(capture.Preview.MajorRecordComparisons.SelectMany(comparison => comparison.Warnings)));
        var review = new WorkspaceChangeReview(
            capture.WorkspaceId,
            capture.State.Game,
            capture.State.Release,
            capture.State.Output!,
            capture.State.OutputBaseline!,
            capture.State.Revision,
            capture.Preview.Comparisons,
            items,
            capture.Preview.UnresolvedReferenceCount,
            warnings,
            capture.Preview.MajorRecordComparisons);

        await UiDispatcher.InvokeAsync(() =>
        {
            CurrentReviewValue = review;
            IsReviewStaleValue = false;
            OutputSynchronizationStatusValue = global::CreationsForge.Core.Engine.Contracts.OutputSynchronizationStatus.Ready;
            ErrorCodeValue = null;
            ErrorMessageValue = null;
            WarningsValue = review.Warnings;
            StatusTextValue = review.HasStagedChanges
                ? "Workspace changes staged"
                : review.RequiresOutputCreation
                    ? "New plugin ready to save"
                    : "No unsaved changes";
            RecoveryEnvelopeValue = null;
            PendingRepairEnvelopeValue = null;
            AbandonmentEnvelopeValue = null;
            LastSaveResultValue = null;
            RaiseAllBoundState();
        }).ConfigureAwait(false);
        return true;
    }

    /// <summary>Reads state and, when ready, previews all staged changes inside one coordinator borrow.</summary>
    /// <param name="workspace">The borrowed live workspace.</param>
    /// <param name="allowBlockedSynchronization">Whether to return blocked state without calling preview.</param>
    /// <param name="cancellationToken">A token that cancels read-only work.</param>
    /// <returns>A detached capture or typed identity, state, or preview failure.</returns>
    private static async ValueTask<EngineResult<WorkspaceReviewCapture>> CaptureReviewWithinBorrowAsync(
        IPluginWorkspace workspace,
        bool allowBlockedSynchronization,
        CancellationToken cancellationToken)
    {
        var stateResult = await workspace.ReadStateAsync(cancellationToken).ConfigureAwait(false);
        var stateFailure = ValidateStateResult(workspace, stateResult, requireOutput: true);
        if (stateFailure is not null || stateResult.Value is null)
        {
            return EngineResult<WorkspaceReviewCapture>.Failure(
                stateFailure ?? UnexpectedFailure("The workspace state read returned no state."),
                workspace.WorkspaceId,
                resultRevision: stateResult.ResultRevision,
                warnings: stateResult.Warnings);
        }

        var state = stateResult.Value;
        if (state.OutputSynchronization.Status != global::CreationsForge.Core.Engine.Contracts.OutputSynchronizationStatus.Ready)
        {
            if (!allowBlockedSynchronization)
            {
                return EngineResult<WorkspaceReviewCapture>.Failure(
                    new EngineError(EngineErrorCode.RepairRequired, "The selected output requires recovery or reopen before it can be reviewed."),
                    workspace.WorkspaceId,
                    baseRevision: state.Revision,
                    resultRevision: state.Revision,
                    warnings: stateResult.Warnings);
            }

            return EngineResult<WorkspaceReviewCapture>.Success(
                new WorkspaceReviewCapture(
                    workspace.WorkspaceId,
                    state,
                    new WorkspacePreview(Array.Empty<FormListComparison>(), 0, Array.Empty<EngineWarning>()),
                    stateResult.Warnings),
                workspace.WorkspaceId,
                baseRevision: state.Revision,
                resultRevision: state.Revision,
                warnings: stateResult.Warnings);
        }

        var previewResult = await workspace.PreviewAsync(cancellationToken).ConfigureAwait(false);
        if (!previewResult.Succeeded || previewResult.Value is null)
        {
            return EngineResult<WorkspaceReviewCapture>.Failure(
                previewResult.Error ?? UnexpectedFailure("The workspace preview returned no value or failure reason."),
                workspace.WorkspaceId,
                baseRevision: state.Revision,
                resultRevision: previewResult.ResultRevision,
                warnings: CombineWarnings(stateResult.Warnings, previewResult.Warnings));
        }

        if (previewResult.WorkspaceId != workspace.WorkspaceId
            || previewResult.BaseRevision != state.Revision
            || previewResult.ResultRevision != state.Revision)
        {
            return EngineResult<WorkspaceReviewCapture>.Failure(
                UnexpectedFailure("The workspace preview did not preserve the exact captured workspace revision."),
                workspace.WorkspaceId,
                baseRevision: state.Revision,
                resultRevision: previewResult.ResultRevision,
                warnings: CombineWarnings(stateResult.Warnings, previewResult.Warnings));
        }

        var warnings = CombineWarnings(stateResult.Warnings, previewResult.Warnings);
        return EngineResult<WorkspaceReviewCapture>.Success(
            new WorkspaceReviewCapture(workspace.WorkspaceId, state, previewResult.Value, warnings),
            workspace.WorkspaceId,
            baseRevision: state.Revision,
            resultRevision: state.Revision,
            warnings: warnings);
    }

    /// <summary>Validates an atomic workspace-state result against the borrowed workspace identity and revision.</summary>
    /// <param name="workspace">The borrowed workspace.</param>
    /// <param name="result">The state result to validate.</param>
    /// <param name="requireOutput">Whether a selected output and complete baseline are required.</param>
    /// <returns>A typed validation failure, or <see langword="null"/> when the result is exact.</returns>
    private static EngineError? ValidateStateResult(
        IPluginWorkspace workspace,
        EngineResult<WorkspaceState> result,
        bool requireOutput)
    {
        if (!result.Succeeded)
        {
            return result.Error ?? UnexpectedFailure("The workspace state read returned no failure reason.");
        }

        if (result.Value is null
            || result.WorkspaceId != workspace.WorkspaceId
            || result.BaseRevision != result.Value.Revision
            || result.ResultRevision != result.Value.Revision)
        {
            return UnexpectedFailure("The workspace state result omitted or contradicted its exact identity or revision.");
        }

        if (requireOutput && (result.Value.Output is null || result.Value.OutputBaseline is null))
        {
            return new EngineError(EngineErrorCode.OutputNotSelected, "Select an output before reviewing or persisting workspace changes.");
        }

        return null;
    }

    /// <summary>Combines warning sequences without changing source order.</summary>
    /// <param name="first">The first warning sequence.</param>
    /// <param name="second">The second warning sequence.</param>
    /// <returns>An immutable combined warning sequence.</returns>
    private static IReadOnlyList<EngineWarning> CombineWarnings(
        IEnumerable<EngineWarning> first,
        IEnumerable<EngineWarning> second)
    {
        return Array.AsReadOnly(first.Concat(second).ToArray());
    }

    /// <summary>Creates a stable presentation failure for an inconsistent or absent engine envelope.</summary>
    /// <param name="message">The complete presentation-safe diagnostic.</param>
    /// <returns>A typed unexpected failure.</returns>
    private static EngineError UnexpectedFailure(string message)
    {
        return new EngineError(EngineErrorCode.UnexpectedFailure, message);
    }

    /// <summary>Determines whether two output associations describe the same exact plugin output.</summary>
    /// <param name="left">The first association.</param>
    /// <param name="right">The second association.</param>
    /// <returns><see langword="true"/> when every association field agrees.</returns>
    private static bool OutputAssociationsMatch(OutputAssociation left, OutputAssociation right)
    {
        var pathComparison = OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        return string.Equals(left.PluginPath, right.PluginPath, pathComparison)
            && left.ModKey == right.ModKey
            && left.LocalizedOutputMode == right.LocalizedOutputMode
            && left.MasterStyle == right.MasterStyle;
    }

    /// <summary>Determines whether two complete output baselines agree in identity and every artifact observation.</summary>
    /// <param name="left">The first complete baseline.</param>
    /// <param name="right">The second complete baseline.</param>
    /// <returns><see langword="true"/> when every baseline and artifact field agrees.</returns>
    private static bool OutputBaselinesMatch(OutputArtifactSetBaseline left, OutputArtifactSetBaseline right)
    {
        if (left.BaselineId != right.BaselineId || left.Artifacts.Count != right.Artifacts.Count)
        {
            return false;
        }

        var pathComparison = OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        for (var index = 0; index < left.Artifacts.Count; index++)
        {
            var leftArtifact = left.Artifacts[index];
            var rightArtifact = right.Artifacts[index];
            if (!string.Equals(leftArtifact.Path, rightArtifact.Path, pathComparison)
                || leftArtifact.Role != rightArtifact.Role
                || !string.Equals(leftArtifact.Language, rightArtifact.Language, StringComparison.OrdinalIgnoreCase)
                || !leftArtifact.Fingerprint.Equals(rightArtifact.Fingerprint)
                || !Equals(leftArtifact.FileIdentity, rightArtifact.FileIdentity))
            {
                return false;
            }
        }

        return true;
    }
}
