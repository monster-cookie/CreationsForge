using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Services;

namespace CreationsForge.ViewModels;

/// <content>Owns the shared dirty-workspace leave state machine and exact transition-lease transfer.</content>
public sealed partial class WorkspaceChangesViewModel
{
    /// <summary>Attempts to prove and reserve one exact workspace-leave action while retaining presentation admission.</summary>
    /// <param name="reason">The exact Open, Close, Settings, or Exit action seeking a permit.</param>
    /// <param name="cancellationToken">A token that cancels the attempt before a reservation is transferred.</param>
    /// <returns>The exact transferred leave reservation, or <see langword="null"/> when the user stays.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="reason"/> is undefined.</exception>
    public async ValueTask<WorkspaceLeaveReservation?> ReserveLeaveAsync(
        WorkspaceLeaveReason reason,
        CancellationToken cancellationToken = default)
    {
        if (!Enum.IsDefined(reason))
        {
            throw new ArgumentOutOfRangeException(nameof(reason));
        }

        var request = WorkspaceChangesDialogRequest.ForLeave(reason);
        if (!TryBeginDialogSession(request, requireWorkspace: false))
        {
            return null;
        }

        WorkspaceLeaveReservation? transferredReservation = null;
        try
        {
            await RunOperationAsync(
                WorkspaceChangesOperationState.ReservingTransition,
                cancellationToken,
                AcquireTransitionRequestAsync).ConfigureAwait(false);
            if (!HasActiveOwnedTransitionRequest || IsShutdownRequested)
            {
                return null;
            }

            if (IsCapturedEditorOperationActive)
            {
                await UiDispatcher.InvokeAsync(() =>
                {
                    StatusTextValue = "An editor operation is still running. Wait for it, request cancellation and wait for a safe result, or keep editing.";
                    RaiseAllBoundState();
                }).ConfigureAwait(false);

                if (!IsCapturedEditorOperationActive)
                {
                    var completedBeforeDialogOutcome = await CompleteEditorTransitionAndClassifyAsync(
                        WorkspaceTransitionDrainMode.WaitForCurrentOperation,
                        WorkspaceChangesOperationState.WaitingForEditorOperation).ConfigureAwait(false);
                    if (completedBeforeDialogOutcome == WorkspaceLeaveChoiceOutcome.Proceed)
                    {
                        transferredReservation = await FinalizeProvedLeaveAsync(reason, cancellationToken).ConfigureAwait(false);
                        return transferredReservation;
                    }

                    if (!HasActiveOwnedTransition || IsShutdownRequested)
                    {
                        return null;
                    }
                }
            }
            else
            {
                var initialOutcome = await CompleteEditorTransitionAndClassifyAsync(
                    WorkspaceTransitionDrainMode.WaitForCurrentOperation,
                    WorkspaceChangesOperationState.WaitingForEditorOperation).ConfigureAwait(false);
                if (initialOutcome == WorkspaceLeaveChoiceOutcome.Proceed)
                {
                    transferredReservation = await FinalizeProvedLeaveAsync(reason, cancellationToken).ConfigureAwait(false);
                    return transferredReservation;
                }

                if (!HasActiveOwnedTransition || IsShutdownRequested)
                {
                    return null;
                }
            }

            var dialogResult = await DialogService.ShowAsync(this, request, cancellationToken).ConfigureAwait(false);
            await WaitForActiveOperationAsync().ConfigureAwait(false);
            if (dialogResult != WorkspaceChangesDialogResult.Proceed)
            {
                return null;
            }

            transferredReservation = await FinalizeProvedLeaveAsync(reason, cancellationToken).ConfigureAwait(false);
            return transferredReservation;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            await WaitForActiveOperationAsync().ConfigureAwait(false);
            return null;
        }
        finally
        {
            if (transferredReservation is null)
            {
                await ReleaseUntransferredTransitionAsync().ConfigureAwait(false);
            }

            EndDialogSession();
        }
    }

    /// <summary>Applies one closed dialog choice through the view model's authoritative state machine.</summary>
    /// <param name="choice">The exact user choice forwarded by the dialog adapter.</param>
    /// <param name="cancellationToken">A token that cancels cancellable work before a terminal choice is proved.</param>
    /// <returns>Whether the dialog remains open, closes to keep editing, or closes so final leave proof can run.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="choice"/> is undefined.</exception>
    internal async Task<WorkspaceLeaveChoiceOutcome> ApplyLeaveChoiceAsync(
        WorkspaceChangesDialogChoice choice,
        CancellationToken cancellationToken = default)
    {
        if (!Enum.IsDefined(choice))
        {
            throw new ArgumentOutOfRangeException(nameof(choice));
        }

        var request = ActiveDialogRequest;
        if (!IsDialogSessionActive || request is null)
        {
            return WorkspaceLeaveChoiceOutcome.ContinueDialog;
        }

        switch (choice)
        {
            case WorkspaceChangesDialogChoice.WaitForEditorOperation:
                if (!CanWaitForEditorOperation)
                {
                    return WorkspaceLeaveChoiceOutcome.ContinueDialog;
                }

                cancellationToken.ThrowIfCancellationRequested();
                return await CompleteEditorTransitionAndClassifyAsync(
                    WorkspaceTransitionDrainMode.WaitForCurrentOperation,
                    WorkspaceChangesOperationState.WaitingForEditorOperation).ConfigureAwait(false);
            case WorkspaceChangesDialogChoice.CancelEditorOperationAndWait:
                if (!CanCancelEditorOperationAndWait)
                {
                    return WorkspaceLeaveChoiceOutcome.ContinueDialog;
                }

                cancellationToken.ThrowIfCancellationRequested();
                return await CompleteEditorTransitionAndClassifyAsync(
                    WorkspaceTransitionDrainMode.CancelAndWaitForCurrentOperation,
                    WorkspaceChangesOperationState.CancelRequestedWaitingForResult).ConfigureAwait(false);
            case WorkspaceChangesDialogChoice.KeepEditing:
                return CanDismissDialog
                    ? WorkspaceLeaveChoiceOutcome.KeepEditing
                    : WorkspaceLeaveChoiceOutcome.ContinueDialog;
            case WorkspaceChangesDialogChoice.ReturnToEditor:
                return CanReturnToEditor
                    ? WorkspaceLeaveChoiceOutcome.KeepEditing
                    : WorkspaceLeaveChoiceOutcome.ContinueDialog;
            case WorkspaceChangesDialogChoice.SaveAndProceed:
                if (request.Purpose is not WorkspaceChangesDialogPurpose.Review
                    and not WorkspaceChangesDialogPurpose.Save
                    and not WorkspaceChangesDialogPurpose.Leave)
                {
                    return WorkspaceLeaveChoiceOutcome.ContinueDialog;
                }

                return await ApplyPersistenceDialogChoiceAsync(
                    save: true,
                    request,
                    cancellationToken).ConfigureAwait(false);
            case WorkspaceChangesDialogChoice.DiscardAndProceed:
                if (request.Purpose is not WorkspaceChangesDialogPurpose.Review
                    and not WorkspaceChangesDialogPurpose.Discard
                    and not WorkspaceChangesDialogPurpose.Leave)
                {
                    return WorkspaceLeaveChoiceOutcome.ContinueDialog;
                }

                return await ApplyPersistenceDialogChoiceAsync(
                    save: false,
                    request,
                    cancellationToken).ConfigureAwait(false);
            case WorkspaceChangesDialogChoice.ConfirmAbandonmentForOpen:
                if (!CanConfirmAbandonmentForOpen
                    || request.LeaveReason != WorkspaceLeaveReason.OpenWorkspace
                    || !TryValidateConfirmedAbandonment(request.LeaveReason.Value, out _))
                {
                    return WorkspaceLeaveChoiceOutcome.ContinueDialog;
                }

                lock (LifecycleGate)
                {
                    PendingLeaveDisposition = WorkspaceLeaveDisposition.ConfirmedAbandonmentForOpen;
                }

                return WorkspaceLeaveChoiceOutcome.Proceed;
            default:
                return WorkspaceLeaveChoiceOutcome.ContinueDialog;
        }
    }

    /// <summary>Completes the pending editor transition without caller cancellation and classifies fresh leave state.</summary>
    /// <param name="drainMode">Whether the captured editor waits naturally or receives cancellation.</param>
    /// <param name="operationState">The exact user-visible drain state.</param>
    /// <returns>The dialog outcome derived from post-drain editor and workspace state.</returns>
    private async Task<WorkspaceLeaveChoiceOutcome> CompleteEditorTransitionAndClassifyAsync(
        WorkspaceTransitionDrainMode drainMode,
        WorkspaceChangesOperationState operationState)
    {
        var outcome = WorkspaceLeaveChoiceOutcome.ContinueDialog;
        await RunOperationAsync(
            operationState,
            CancellationToken.None,
            async _ =>
            {
                if (drainMode == WorkspaceTransitionDrainMode.CancelAndWaitForCurrentOperation)
                {
                    await PublishOperationStateAsync(
                        WorkspaceChangesOperationState.CancelRequestedWaitingForResult,
                        isCancelRequested: true,
                        "Cancel requested; waiting for a safe result.").ConfigureAwait(false);
                }

                if (!await CompleteOwnedTransitionRequestAsync(drainMode).ConfigureAwait(false))
                {
                    return;
                }

                outcome = await ClassifyPostDrainLeaveStateAsync(CancellationToken.None).ConfigureAwait(false);
            }).ConfigureAwait(false);
        return outcome;
    }

    /// <summary>Classifies the exact editor and workspace state after the captured operation has drained.</summary>
    /// <param name="cancellationToken">A token used for fresh read-only workspace review.</param>
    /// <returns>The dialog outcome derived from the newly published state.</returns>
    private async Task<WorkspaceLeaveChoiceOutcome> ClassifyPostDrainLeaveStateAsync(CancellationToken cancellationToken)
    {
        if (!HasActiveOwnedTransition)
        {
            return WorkspaceLeaveChoiceOutcome.ContinueDialog;
        }

        if (HasPendingEditorOperation)
        {
            await PublishPostDrainStatusAsync("Return to editor and resolve the pending form operation.").ConfigureAwait(false);
            return WorkspaceLeaveChoiceOutcome.ContinueDialog;
        }

        if (WorkspaceCoordinator.CurrentWorkspace is null)
        {
            if (!HasDraftChanges && TryRecordNoWorkspaceDisposition())
            {
                return WorkspaceLeaveChoiceOutcome.Proceed;
            }

            if (HasDraftChanges)
            {
                await PublishPostDrainStatusAsync("Form changes not applied").ConfigureAwait(false);
            }

            return WorkspaceLeaveChoiceOutcome.ContinueDialog;
        }

        if (WorkspaceCoordinator.CurrentWorkspace.Output is null)
        {
            lock (LifecycleGate)
            {
                PendingLeaveDisposition = WorkspaceLeaveDisposition.ReadyAndClean;
            }

            return WorkspaceLeaveChoiceOutcome.Proceed;
        }

        var acceptedReview = await CaptureAndPublishReviewAsync(
            cancellationToken,
            allowBlockedSynchronization: true).ConfigureAwait(false);
        if (HasPendingEditorOperation)
        {
            await PublishPostDrainStatusAsync("Return to editor and resolve the pending form operation.").ConfigureAwait(false);
            return WorkspaceLeaveChoiceOutcome.ContinueDialog;
        }

        if (HasDraftChanges)
        {
            await PublishPostDrainStatusAsync("Form changes not applied").ConfigureAwait(false);
            return WorkspaceLeaveChoiceOutcome.ContinueDialog;
        }

        if (!acceptedReview || CurrentReview?.HasStagedChanges != false)
        {
            return WorkspaceLeaveChoiceOutcome.ContinueDialog;
        }

        lock (LifecycleGate)
        {
            PendingLeaveDisposition = WorkspaceLeaveDisposition.ReadyAndClean;
        }

        return WorkspaceLeaveChoiceOutcome.Proceed;
    }

    /// <summary>Publishes one exact post-drain leave status while preserving the accepted review and evidence.</summary>
    /// <param name="statusText">The complete user-visible status.</param>
    /// <returns>A task that completes after bound state is published.</returns>
    private Task PublishPostDrainStatusAsync(string statusText)
    {
        return UiDispatcher.InvokeAsync(() =>
        {
            StatusTextValue = statusText;
            RaiseAllBoundState();
        });
    }

    /// <summary>Repeats the final proof for the dialog's preliminary disposition and transfers the exact admission lease.</summary>
    /// <param name="reason">The exact requested leave action.</param>
    /// <param name="cancellationToken">The original leave token checked before and after final proof.</param>
    /// <returns>The transferred leave reservation, or <see langword="null"/> when final proof fails.</returns>
    private async Task<WorkspaceLeaveReservation?> FinalizeProvedLeaveAsync(
        WorkspaceLeaveReason reason,
        CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return null;
        }

        WorkspaceLeaveDisposition? preliminaryDisposition;
        lock (LifecycleGate)
        {
            preliminaryDisposition = PendingLeaveDisposition;
        }

        switch (preliminaryDisposition)
        {
            case WorkspaceLeaveDisposition.ReadyAndClean:
                var cleanWorkspaceId = await ProveReadyAndCleanAsync(CancellationToken.None).ConfigureAwait(false);
                return cleanWorkspaceId is null || cancellationToken.IsCancellationRequested
                    ? null
                    : TransferOwnedTransition(
                        reason,
                        WorkspaceLeaveDisposition.ReadyAndClean,
                        cleanWorkspaceId,
                        cancellationToken);
            case WorkspaceLeaveDisposition.NoWorkspace:
                return TryTransferNoWorkspace(reason, cancellationToken, out var noWorkspaceReservation)
                    ? noWorkspaceReservation
                    : null;
            case WorkspaceLeaveDisposition.ConfirmedAbandonmentForOpen:
                return !cancellationToken.IsCancellationRequested
                    && TryValidateConfirmedAbandonment(reason, out var abandonedWorkspaceId)
                    ? TransferOwnedTransition(
                        reason,
                        WorkspaceLeaveDisposition.ConfirmedAbandonmentForOpen,
                        abandonedWorkspaceId,
                        cancellationToken)
                    : null;
            default:
                await PublishFailureAsync(
                    UnexpectedFailure("The dialog requested leave without a disposition proved by the change lifecycle."),
                    "Keep editing until the workspace reaches a safe leave disposition.",
                    WarningsValue).ConfigureAwait(false);
                return null;
        }
    }

    /// <summary>Runs the selected save or discard and records only a newly proved completed disposition.</summary>
    /// <param name="save">Whether to save; <see langword="false"/> selects explicit discard.</param>
    /// <param name="request">The active immutable dialog request.</param>
    /// <param name="cancellationToken">A token passed to cancellable persistence work.</param>
    /// <returns>The closed dialog outcome after result mapping and any required proof.</returns>
    private async Task<WorkspaceLeaveChoiceOutcome> ApplyPersistenceDialogChoiceAsync(
        bool save,
        WorkspaceChangesDialogRequest request,
        CancellationToken cancellationToken)
    {
        var startingVersion = Interlocked.Read(ref CompletedPersistenceVersion);
        if (!save
            && WorkspaceCoordinator.CurrentWorkspace is null
            && HasDraftChanges
            && !HasPendingEditorOperation
            && HasActiveOwnedTransition)
        {
            EditParticipant.DiscardRequestLocalFormChanges();
            Interlocked.Increment(ref CompletedPersistenceVersion);
            await PublishSuccessAsync("No unsaved changes", WarningsValue).ConfigureAwait(false);
        }
        else if (save)
        {
            await SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
        else
        {
            await DiscardChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        if (Interlocked.Read(ref CompletedPersistenceVersion) == startingVersion
            || PendingRefreshEnvelopeValue is not null)
        {
            return WorkspaceLeaveChoiceOutcome.ContinueDialog;
        }

        if (request.Purpose != WorkspaceChangesDialogPurpose.Leave)
        {
            return WorkspaceLeaveChoiceOutcome.Proceed;
        }

        if (TryRecordNoWorkspaceDisposition())
        {
            return WorkspaceLeaveChoiceOutcome.Proceed;
        }

        var cleanWorkspaceId = await ProveReadyAndCleanAsync(cancellationToken).ConfigureAwait(false);
        if (cleanWorkspaceId is null)
        {
            return WorkspaceLeaveChoiceOutcome.ContinueDialog;
        }

        lock (LifecycleGate)
        {
            PendingLeaveDisposition = WorkspaceLeaveDisposition.ReadyAndClean;
        }

        return WorkspaceLeaveChoiceOutcome.Proceed;
    }

    /// <summary>Repeats the complete live ready-and-clean proof without retaining a coordinator borrow.</summary>
    /// <param name="cancellationToken">A token that cancels the read-only proof.</param>
    /// <returns>The exact clean workspace identity, or <see langword="null"/> when proof fails.</returns>
    private async Task<Guid?> ProveReadyAndCleanAsync(CancellationToken cancellationToken)
    {
        if (!HasActiveOwnedTransition
            || HasDraftChanges
            || HasPendingEditorOperation
            || EditParticipant.IsEditorBusy
            || WorkspaceCoordinator.CurrentWorkspace is null)
        {
            return null;
        }

        var currentDescriptor = WorkspaceCoordinator.CurrentWorkspace;
        if (currentDescriptor.Output is null)
        {
            var stateResult = await WorkspaceCoordinator.ExecuteAsync(
                (workspace, token) => workspace.ReadStateAsync(token),
                cancellationToken).ConfigureAwait(false);
            if (!stateResult.Succeeded
                || stateResult.Value is null
                || stateResult.WorkspaceId != currentDescriptor.WorkspaceId
                || stateResult.Value.Game != currentDescriptor.Game
                || stateResult.Value.Release != currentDescriptor.Release
                || stateResult.Value.Output is not null
                || stateResult.Value.Revision != currentDescriptor.Revision
                || WorkspaceCoordinator.CurrentWorkspace?.WorkspaceId != currentDescriptor.WorkspaceId)
            {
                await PublishFailureAsync(
                    stateResult.Error ?? new EngineError(EngineErrorCode.RevisionConflict, "The read-only workspace changed before the final leave proof completed."),
                    "Keep the workspace open until its read-only state can be verified.",
                    stateResult.Warnings).ConfigureAwait(false);
                return null;
            }

            return currentDescriptor.WorkspaceId;
        }

        Guid? provedWorkspaceId = null;
        await RunOperationAsync(
            WorkspaceChangesOperationState.Reviewing,
            cancellationToken,
            async token =>
            {
                var result = await WorkspaceCoordinator.ExecuteAsync(
                    async (workspace, borrowToken) => await CaptureReviewWithinBorrowAsync(
                        workspace,
                        allowBlockedSynchronization: false,
                        borrowToken).ConfigureAwait(false),
                    token).ConfigureAwait(false);
                if (!result.Succeeded || result.Value is null)
                {
                    await PublishFailureAsync(
                        result.Error ?? UnexpectedFailure("The final clean proof returned no review or failure reason."),
                        "Keep editing until the workspace reaches a safe leave disposition.",
                        result.Warnings).ConfigureAwait(false);
                    return;
                }

                var capture = result.Value;
                var descriptor = WorkspaceCoordinator.CurrentWorkspace;
                if (descriptor is null
                    || descriptor.WorkspaceId != capture.WorkspaceId
                    || descriptor.Game != capture.State.Game
                    || descriptor.Release != capture.State.Release
                    || descriptor.Output is null
                    || capture.State.Output is null
                    || !OutputAssociationsMatch(descriptor.Output, capture.State.Output)
                    || capture.Preview.Comparisons.Count != 0
                    || HasDraftChanges
                    || HasPendingEditorOperation)
                {
                    await PublishFailureAsync(
                        new EngineError(EngineErrorCode.RevisionConflict, "The workspace changed before the final clean leave proof completed."),
                        "Keep editing until the workspace reaches a safe leave disposition.",
                        capture.Warnings).ConfigureAwait(false);
                    return;
                }

                await PublishAcceptedReviewCaptureAsync(capture, "No unsaved changes").ConfigureAwait(false);
                provedWorkspaceId = capture.WorkspaceId;
            }).ConfigureAwait(false);
        return provedWorkspaceId;
    }

    /// <summary>Transfers a no-workspace reservation without borrowing or reading a workspace.</summary>
    /// <param name="reason">The exact requested leave action.</param>
    /// <param name="cancellationToken">The original leave token checked at the transfer linearization point.</param>
    /// <param name="reservation">Receives the transferred reservation when proof succeeds.</param>
    /// <returns><see langword="true"/> when null coordinator identity and local-state checks were proved under the lease.</returns>
    private bool TryTransferNoWorkspace(
        WorkspaceLeaveReason reason,
        CancellationToken cancellationToken,
        out WorkspaceLeaveReservation? reservation)
    {
        reservation = null;
        if (!TryRecordNoWorkspaceDisposition())
        {
            return false;
        }

        reservation = TransferOwnedTransition(
            reason,
            WorkspaceLeaveDisposition.NoWorkspace,
            expectedWorkspaceId: null,
            cancellationToken);
        return true;
    }

    /// <summary>Records a no-workspace disposition only after local editor state is drained and admission remains held.</summary>
    /// <returns><see langword="true"/> when the exact no-workspace disposition was proved.</returns>
    private bool TryRecordNoWorkspaceDisposition()
    {
        if (!HasActiveOwnedTransition
            || WorkspaceCoordinator.CurrentWorkspace is not null
            || HasDraftChanges
            || HasPendingEditorOperation
            || EditParticipant.IsEditorBusy)
        {
            return false;
        }

        lock (LifecycleGate)
        {
            PendingLeaveDisposition = WorkspaceLeaveDisposition.NoWorkspace;
        }

        return true;
    }

    /// <summary>Revalidates the exact Open-only external-conflict confirmation while the same transition remains owned.</summary>
    /// <param name="reason">The requested final leave action.</param>
    /// <param name="workspaceId">Receives the exact still-live conflicted workspace identity.</param>
    /// <returns><see langword="true"/> when reason, workspace, and output still match the confirmed conflict.</returns>
    private bool TryValidateConfirmedAbandonment(
        WorkspaceLeaveReason reason,
        out Guid workspaceId)
    {
        workspaceId = Guid.Empty;
        var envelope = AbandonmentEnvelopeValue;
        var descriptor = WorkspaceCoordinator.CurrentWorkspace;
        if (!HasActiveOwnedTransition
            || reason != WorkspaceLeaveReason.OpenWorkspace
            || envelope is null
            || descriptor is null
            || descriptor.WorkspaceId != envelope.WorkspaceId
            || descriptor.Output is null
            || !OutputAssociationsMatch(descriptor.Output, envelope.Output))
        {
            return false;
        }

        workspaceId = descriptor.WorkspaceId;
        return true;
    }

    /// <summary>Moves the exact active transition lease into a non-fabricable caller-owned leave reservation.</summary>
    /// <param name="reason">The exact final action.</param>
    /// <param name="disposition">The exact proved disposition.</param>
    /// <param name="expectedWorkspaceId">The exact live workspace identity, or <see langword="null"/> for no workspace.</param>
    /// <param name="cancellationToken">The original leave token checked while transition ownership is locked.</param>
    /// <returns>The caller-owned reservation, or <see langword="null"/> if transition ownership was lost.</returns>
    private WorkspaceLeaveReservation? TransferOwnedTransition(
        WorkspaceLeaveReason reason,
        WorkspaceLeaveDisposition disposition,
        Guid? expectedWorkspaceId,
        CancellationToken cancellationToken)
    {
        lock (LifecycleGate)
        {
            var lease = OwnedTransitionLease;
            if (cancellationToken.IsCancellationRequested || lease?.IsActive != true)
            {
                return null;
            }

            var reservation = new WorkspaceLeaveReservation(lease, reason, disposition, expectedWorkspaceId);
            OwnedTransitionLease = null;
            PendingLeaveDisposition = null;
            return reservation;
        }
    }
}
