using CreationsForge.Core.Engine.Contracts;
using CreationsForge.PresentationTests.Support;
using CreationsForge.Services;
using CreationsForge.ViewModels;
using Shouldly;

namespace CreationsForge.PresentationTests.ViewModels;

/// <content>Verifies active-editor leave choices retain exact admission and classify only terminal state.</content>
public sealed partial class NativeWorkspaceChangesViewModelTests
{
    /// <summary>Verifies every leave reason presents the active-editor decision before any drain or workspace borrow.</summary>
    /// <param name="reason">The exact leave reason under test.</param>
    /// <returns>A task that completes after the scenario assertions.</returns>
    [Theory]
    [InlineData(NativeWorkspaceLeaveReason.OpenWorkspace)]
    [InlineData(NativeWorkspaceLeaveReason.CloseWorkspace)]
    [InlineData(NativeWorkspaceLeaveReason.ShowSettings)]
    [InlineData(NativeWorkspaceLeaveReason.ExitApplication)]
    public async Task ReserveLeaveAsync_ActiveEditor_ShowsDecisionBeforeDrain(NativeWorkspaceLeaveReason reason)
    {
        await using var context = NativeWorkspaceChangesTestContext.CreateReady(hasStagedChanges: true);
        using var editorLease = context.Arbiter.TryBeginEditorOperation().ShouldNotBeNull();
        context.EditParticipant.SetState(
            hasDraftChanges: false,
            isEditorBusy: true,
            operationState: NativeFormListEditorOperationState.Applying);
        context.DialogService.OnShowAsync = async (viewModel, request) =>
        {
            request.LeaveReason.ShouldBe(reason);
            viewModel.ShowActiveEditorOperationDecision.ShouldBeTrue();
            viewModel.CanWaitForEditorOperation.ShouldBeTrue();
            viewModel.CanCancelEditorOperationAndWait.ShouldBeTrue();
            viewModel.ShowSaveAndProceed.ShouldBeFalse();
            viewModel.ShowDiscardAndProceed.ShouldBeFalse();
            viewModel.ShowReturnToEditor.ShouldBeFalse();
            viewModel.StatusText.ShouldBe("An editor operation is still running. Wait for it, request cancellation and wait for a safe result, or keep editing.");
            context.Arbiter.IsWorkspaceTransitionPendingOrReserved.ShouldBeTrue();
            context.Arbiter.TryBeginEditorOperation().ShouldBeNull();
            context.Coordinator.ExecuteCount.ShouldBe(0);
            context.Workspace!.ReadStateCount.ShouldBe(0);
            context.Workspace.PreviewCount.ShouldBe(0);
            editorLease.CancellationToken.IsCancellationRequested.ShouldBeFalse();

            var outcome = await viewModel.ApplyLeaveChoiceAsync(NativeWorkspaceChangesDialogChoice.KeepEditing);
            outcome.ShouldBe(NativeWorkspaceLeaveChoiceOutcome.KeepEditing);
            return NativeWorkspaceChangesDialogResult.KeepEditing;
        };

        var reservation = await context.ViewModel.ReserveLeaveAsync(reason);

        reservation.ShouldBeNull();
        editorLease.IsActive.ShouldBeTrue();
        editorLease.CancellationToken.IsCancellationRequested.ShouldBeFalse();
        context.DialogService.Requests.Count.ShouldBe(1);
    }

    /// <summary>Verifies natural wait leaves cancellation untouched and awaits terminal publication before a fresh dirty review.</summary>
    /// <returns>A task that completes after the scenario assertions.</returns>
    [Fact]
    public async Task ApplyLeaveChoiceAsync_WaitForEditorOperation_DrainsThenPublishesFreshDirtyReview()
    {
        await using var context = NativeWorkspaceChangesTestContext.CreateReady(hasStagedChanges: true);
        using var editorLease = context.Arbiter.TryBeginEditorOperation().ShouldNotBeNull();
        var editorAdmissionReleased = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var allowDrainPublication = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        context.Arbiter.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(context.Arbiter.IsEditorOperationActive)
                && !context.Arbiter.IsEditorOperationActive)
            {
                editorAdmissionReleased.TrySetResult();
                allowDrainPublication.Task.GetAwaiter().GetResult();
            }
        };
        context.EditParticipant.SetState(
            hasDraftChanges: false,
            isEditorBusy: true,
            operationState: NativeFormListEditorOperationState.Applying);
        context.DialogService.OnShowAsync = async (viewModel, _) =>
        {
            var choiceTask = viewModel.ApplyLeaveChoiceAsync(NativeWorkspaceChangesDialogChoice.WaitForEditorOperation);

            choiceTask.IsCompleted.ShouldBeFalse();
            viewModel.OperationState.ShouldBe(NativeWorkspaceChangesOperationState.WaitingForEditorOperation);
            viewModel.StatusText.ShouldBe("Waiting for the current editor operation to finish.");
            editorLease.CancellationToken.IsCancellationRequested.ShouldBeFalse();
            context.Coordinator.ExecuteCount.ShouldBe(0);

            context.EditParticipant.SetState(hasDraftChanges: false);
            var disposeTask = Task.Run(editorLease.Dispose);
            await editorAdmissionReleased.Task;
            choiceTask.IsCompleted.ShouldBeFalse();
            context.Coordinator.ExecuteCount.ShouldBe(0);
            allowDrainPublication.TrySetResult();
            await disposeTask;
            var outcome = await choiceTask;

            outcome.ShouldBe(NativeWorkspaceLeaveChoiceOutcome.ContinueDialog);
            viewModel.ShowActiveEditorOperationDecision.ShouldBeFalse();
            viewModel.CurrentReview.ShouldNotBeNull();
            viewModel.CurrentReview.HasStagedChanges.ShouldBeTrue();
            viewModel.CanSaveAndProceed.ShouldBeTrue();
            viewModel.CanDiscardAndProceed.ShouldBeTrue();
            return NativeWorkspaceChangesDialogResult.KeepEditing;
        };

        var reservation = await context.ViewModel.ReserveLeaveAsync(NativeWorkspaceLeaveReason.CloseWorkspace);

        reservation.ShouldBeNull();
        editorLease.CancellationToken.IsCancellationRequested.ShouldBeFalse();
        context.Workspace!.PreviewCount.ShouldBe(1);
        context.Arbiter.IsWorkspaceTransitionPendingOrReserved.ShouldBeFalse();
    }

    /// <summary>Verifies cancel-and-wait signals only the captured editor and blocks until its pending result is published.</summary>
    /// <returns>A task that completes after the scenario assertions.</returns>
    [Fact]
    public async Task ApplyLeaveChoiceAsync_CancelEditorOperationAndWait_PendingResultBlocksWorkspaceReview()
    {
        await using var context = NativeWorkspaceChangesTestContext.CreateReady(hasStagedChanges: true);
        using var editorLease = context.Arbiter.TryBeginEditorOperation().ShouldNotBeNull();
        context.EditParticipant.SetState(
            hasDraftChanges: false,
            isEditorBusy: true,
            operationState: NativeFormListEditorOperationState.Applying);
        context.DialogService.OnShowAsync = async (viewModel, _) =>
        {
            var choiceTask = viewModel.ApplyLeaveChoiceAsync(NativeWorkspaceChangesDialogChoice.CancelEditorOperationAndWait);

            editorLease.CancellationToken.IsCancellationRequested.ShouldBeTrue();
            choiceTask.IsCompleted.ShouldBeFalse();
            viewModel.OperationState.ShouldBe(NativeWorkspaceChangesOperationState.CancelRequestedWaitingForResult);
            viewModel.IsCancelRequested.ShouldBeTrue();
            viewModel.StatusText.ShouldBe("Cancel requested; waiting for a safe result.");

            context.EditParticipant.SetState(
                hasDraftChanges: false,
                hasPendingOperation: true,
                operationState: NativeFormListEditorOperationState.PendingOutcome);
            editorLease.Dispose();
            var outcome = await choiceTask;

            outcome.ShouldBe(NativeWorkspaceLeaveChoiceOutcome.ContinueDialog);
            viewModel.ShowActiveEditorOperationDecision.ShouldBeFalse();
            viewModel.ShowReturnToEditor.ShouldBeTrue();
            viewModel.CanReturnToEditor.ShouldBeTrue();
            viewModel.ShowSaveAndProceed.ShouldBeFalse();
            viewModel.ShowDiscardAndProceed.ShouldBeFalse();
            viewModel.CanInspectSaveOutcome.ShouldBeFalse();
            viewModel.CanCompletePreparedSave.ShouldBeFalse();
            viewModel.CanRestorePreviousOutput.ShouldBeFalse();
            viewModel.CanResumeUnsavedChanges.ShouldBeFalse();
            viewModel.CanReopenResolvedOutput.ShouldBeFalse();
            viewModel.CanRetryCommittedRefresh.ShouldBeFalse();
            viewModel.StatusText.ShouldBe("Return to editor and resolve the pending form operation.");
            context.Coordinator.ExecuteCount.ShouldBe(0);
            return NativeWorkspaceChangesDialogResult.KeepEditing;
        };

        var reservation = await context.ViewModel.ReserveLeaveAsync(NativeWorkspaceLeaveReason.ShowSettings);

        reservation.ShouldBeNull();
        context.Workspace!.ReadStateCount.ShouldBe(0);
        context.Workspace.PreviewCount.ShouldBe(0);
    }

    /// <summary>Verifies Keep Editing abandons the pending request without canceling or waiting for the active editor.</summary>
    /// <returns>A task that completes after the scenario assertions.</returns>
    [Fact]
    public async Task ReserveLeaveAsync_ActiveEditorKeepEditing_ReturnsBeforeEditorDrain()
    {
        await using var context = NativeWorkspaceChangesTestContext.CreateReady(hasStagedChanges: true);
        using var editorLease = context.Arbiter.TryBeginEditorOperation().ShouldNotBeNull();
        context.EditParticipant.SetState(
            hasDraftChanges: false,
            isEditorBusy: true,
            operationState: NativeFormListEditorOperationState.Applying);
        context.DialogService.OnShowAsync = async (viewModel, _) =>
        {
            var outcome = await viewModel.ApplyLeaveChoiceAsync(NativeWorkspaceChangesDialogChoice.KeepEditing);
            outcome.ShouldBe(NativeWorkspaceLeaveChoiceOutcome.KeepEditing);
            return NativeWorkspaceChangesDialogResult.KeepEditing;
        };

        var reservation = await context.ViewModel.ReserveLeaveAsync(NativeWorkspaceLeaveReason.OpenWorkspace);

        reservation.ShouldBeNull();
        editorLease.IsActive.ShouldBeTrue();
        editorLease.CancellationToken.IsCancellationRequested.ShouldBeFalse();
        context.Arbiter.IsWorkspaceTransitionPendingOrReserved.ShouldBeFalse();
        context.Arbiter.TryBeginEditorOperation().ShouldBeNull();
        context.Coordinator.ExecuteCount.ShouldBe(0);
    }

    /// <summary>Verifies editor completion during request acquisition skips a stale prompt without reopening admission.</summary>
    /// <returns>A task that completes after the scenario assertions.</returns>
    [Fact]
    public async Task ReserveLeaveAsync_EditorCompletesBeforePrompt_SkipsDecisionAndTransfersContinuousLease()
    {
        await using var context = NativeWorkspaceChangesTestContext.CreateReady();
        using var editorLease = context.Arbiter.TryBeginEditorOperation().ShouldNotBeNull();
        context.EditParticipant.SetState(
            hasDraftChanges: false,
            isEditorBusy: true,
            operationState: NativeFormListEditorOperationState.Applying);
        NativeWorkspaceEditorOperationLease? replacementLease = null;
        context.Arbiter.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(context.Arbiter.IsWorkspaceTransitionPendingOrReserved)
                && context.Arbiter.IsWorkspaceTransitionPendingOrReserved
                && editorLease.IsActive)
            {
                context.EditParticipant.SetState(hasDraftChanges: false);
                editorLease.Dispose();
                replacementLease = context.Arbiter.TryBeginEditorOperation();
            }
        };

        using var reservation = await context.ViewModel.ReserveLeaveAsync(NativeWorkspaceLeaveReason.ExitApplication);

        reservation.ShouldNotBeNull();
        reservation.Disposition.ShouldBe(NativeWorkspaceLeaveDisposition.ReadyAndClean);
        context.DialogService.Requests.ShouldBeEmpty();
        replacementLease.ShouldBeNull();
        context.Arbiter.IsWorkspaceTransitionPendingOrReserved.ShouldBeTrue();
    }

    /// <summary>Verifies caller cancellation before a wait choice abandons admission without signaling the editor.</summary>
    /// <returns>A task that completes after the scenario assertions.</returns>
    [Fact]
    public async Task ReserveLeaveAsync_CallerCanceledBeforeWaitChoice_AbandonsWithoutEditorCancellation()
    {
        await using var context = NativeWorkspaceChangesTestContext.CreateReady(hasStagedChanges: true);
        using var editorLease = context.Arbiter.TryBeginEditorOperation().ShouldNotBeNull();
        context.EditParticipant.SetState(
            hasDraftChanges: false,
            isEditorBusy: true,
            operationState: NativeFormListEditorOperationState.Applying);
        using var cancellation = new CancellationTokenSource();
        context.DialogService.OnShowAsync = async (viewModel, _) =>
        {
            cancellation.Cancel();
            await viewModel.ApplyLeaveChoiceAsync(
                NativeWorkspaceChangesDialogChoice.WaitForEditorOperation,
                cancellation.Token);
            throw new InvalidOperationException("The canceled wait choice unexpectedly completed.");
        };

        var reservation = await context.ViewModel.ReserveLeaveAsync(
            NativeWorkspaceLeaveReason.CloseWorkspace,
            cancellation.Token);

        reservation.ShouldBeNull();
        editorLease.IsActive.ShouldBeTrue();
        editorLease.CancellationToken.IsCancellationRequested.ShouldBeFalse();
        context.Arbiter.IsWorkspaceTransitionPendingOrReserved.ShouldBeFalse();
    }

    /// <summary>Verifies cancellation after a wait choice starts cannot truncate the accepted editor drain.</summary>
    /// <returns>A task that completes after the scenario assertions.</returns>
    [Fact]
    public async Task ReserveLeaveAsync_CallerCanceledAfterWaitChoice_DrainStillReachesTerminalClassification()
    {
        await using var context = NativeWorkspaceChangesTestContext.CreateReady(hasStagedChanges: true);
        using var editorLease = context.Arbiter.TryBeginEditorOperation().ShouldNotBeNull();
        context.EditParticipant.SetState(
            hasDraftChanges: false,
            isEditorBusy: true,
            operationState: NativeFormListEditorOperationState.Applying);
        using var cancellation = new CancellationTokenSource();
        context.DialogService.OnShowAsync = async (viewModel, _) =>
        {
            var choiceTask = viewModel.ApplyLeaveChoiceAsync(NativeWorkspaceChangesDialogChoice.WaitForEditorOperation);
            cancellation.Cancel();

            choiceTask.IsCompleted.ShouldBeFalse();
            context.EditParticipant.SetState(hasDraftChanges: false);
            editorLease.Dispose();
            var outcome = await choiceTask;

            outcome.ShouldBe(NativeWorkspaceLeaveChoiceOutcome.ContinueDialog);
            viewModel.CurrentReview.ShouldNotBeNull();
            viewModel.CurrentReview.HasStagedChanges.ShouldBeTrue();
            return NativeWorkspaceChangesDialogResult.KeepEditing;
        };

        var reservation = await context.ViewModel.ReserveLeaveAsync(
            NativeWorkspaceLeaveReason.CloseWorkspace,
            cancellation.Token);

        reservation.ShouldBeNull();
        context.Workspace!.PreviewCount.ShouldBe(1);
        context.Arbiter.IsWorkspaceTransitionPendingOrReserved.ShouldBeFalse();
    }

    /// <summary>Verifies caller cancellation during an automatic final clean proof cannot transfer a leave reservation.</summary>
    /// <returns>A task that completes after the scenario assertions.</returns>
    [Fact]
    public async Task ReserveLeaveAsync_CallerCanceledDuringAutomaticFinalProof_DoesNotTransferReservation()
    {
        await using var context = NativeWorkspaceChangesTestContext.CreateReady();
        using var cancellation = new CancellationTokenSource();
        var finalProofEntered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var releaseFinalProof = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        context.Workspace!.OnReadStateAsync = async _ =>
        {
            if (context.Workspace.ReadStateCount == 2)
            {
                finalProofEntered.TrySetResult();
                await releaseFinalProof.Task;
            }

            return StateResult(context.Workspace);
        };

        var leaveTask = context.ViewModel.ReserveLeaveAsync(
            NativeWorkspaceLeaveReason.ShowSettings,
            cancellation.Token).AsTask();
        await finalProofEntered.Task.WaitAsync(TimeSpan.FromSeconds(5));
        cancellation.Cancel();
        releaseFinalProof.TrySetResult();

        var reservation = await leaveTask;

        reservation.ShouldBeNull();
        context.DialogService.Requests.ShouldBeEmpty();
        context.Workspace.ReadStateCount.ShouldBe(2);
        context.Workspace.PreviewCount.ShouldBe(2);
        context.Arbiter.IsWorkspaceTransitionPendingOrReserved.ShouldBeFalse();
    }

    /// <summary>Verifies cancellation during final proof after accepted wait cannot revoke drain but still blocks transfer.</summary>
    /// <returns>A task that completes after the scenario assertions.</returns>
    [Fact]
    public async Task ReserveLeaveAsync_CallerCanceledDuringPostWaitFinalProof_DrainsWithoutTransfer()
    {
        await using var context = NativeWorkspaceChangesTestContext.CreateReady();
        using var editorLease = context.Arbiter.TryBeginEditorOperation().ShouldNotBeNull();
        context.EditParticipant.SetState(
            hasDraftChanges: false,
            isEditorBusy: true,
            operationState: NativeFormListEditorOperationState.Applying);
        using var cancellation = new CancellationTokenSource();
        var finalProofEntered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var releaseFinalProof = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        context.Workspace!.OnReadStateAsync = async _ =>
        {
            if (context.Workspace.ReadStateCount == 2)
            {
                finalProofEntered.TrySetResult();
                await releaseFinalProof.Task;
            }

            return StateResult(context.Workspace);
        };
        context.DialogService.OnShowAsync = async (viewModel, _) =>
        {
            var waitTask = viewModel.ApplyLeaveChoiceAsync(NativeWorkspaceChangesDialogChoice.WaitForEditorOperation);
            context.EditParticipant.SetState(hasDraftChanges: false);
            editorLease.Dispose();
            var outcome = await waitTask;

            outcome.ShouldBe(NativeWorkspaceLeaveChoiceOutcome.Proceed);
            editorLease.CancellationToken.IsCancellationRequested.ShouldBeFalse();
            return NativeWorkspaceChangesDialogResult.Proceed;
        };

        var leaveTask = context.ViewModel.ReserveLeaveAsync(
            NativeWorkspaceLeaveReason.CloseWorkspace,
            cancellation.Token).AsTask();
        await finalProofEntered.Task.WaitAsync(TimeSpan.FromSeconds(5));
        cancellation.Cancel();
        releaseFinalProof.TrySetResult();

        var reservation = await leaveTask;

        reservation.ShouldBeNull();
        editorLease.IsActive.ShouldBeFalse();
        context.Workspace.ReadStateCount.ShouldBe(2);
        context.Workspace.PreviewCount.ShouldBe(2);
        context.Arbiter.IsWorkspaceTransitionPendingOrReserved.ShouldBeFalse();
    }

    /// <summary>Verifies a concurrent second leave is rejected without creating another prompt or request.</summary>
    /// <returns>A task that completes after the scenario assertions.</returns>
    [Fact]
    public async Task ReserveLeaveAsync_ActiveDialog_RejectsRepeatedLeaveRequest()
    {
        await using var context = NativeWorkspaceChangesTestContext.CreateReady(hasStagedChanges: true);
        using var editorLease = context.Arbiter.TryBeginEditorOperation().ShouldNotBeNull();
        context.EditParticipant.SetState(
            hasDraftChanges: false,
            isEditorBusy: true,
            operationState: NativeFormListEditorOperationState.Applying);
        context.DialogService.OnShowAsync = async (viewModel, _) =>
        {
            var repeated = await viewModel.ReserveLeaveAsync(NativeWorkspaceLeaveReason.ExitApplication);
            repeated.ShouldBeNull();
            context.DialogService.Requests.Count.ShouldBe(1);
            context.Arbiter.IsWorkspaceTransitionPendingOrReserved.ShouldBeTrue();
            return NativeWorkspaceChangesDialogResult.KeepEditing;
        };

        var reservation = await context.ViewModel.ReserveLeaveAsync(NativeWorkspaceLeaveReason.ShowSettings);

        reservation.ShouldBeNull();
        context.DialogService.Requests.Count.ShouldBe(1);
        editorLease.CancellationToken.IsCancellationRequested.ShouldBeFalse();
    }

    /// <summary>Verifies a successful Apply that beats cancellation is reviewed and saved from its fresh revision.</summary>
    /// <returns>A task that completes after the scenario assertions.</returns>
    [Fact]
    public async Task ApplyLeaveChoiceAsync_CancelLosesToSuccessfulApply_UsesFreshRevisionForSave()
    {
        await using var context = NativeWorkspaceChangesTestContext.CreateReady(hasStagedChanges: true);
        var stagedPreview = context.Workspace!.Preview;
        context.Workspace.Preview = new WorkspacePreview([], 0, []);
        var initialState = context.Workspace.State;
        var appliedRevision = initialState.Revision.Next();
        var savedRevision = appliedRevision.Next();
        var committedBaseline = ReplacementBaseline(initialState.Output!);
        using var editorLease = context.Arbiter.TryBeginEditorOperation().ShouldNotBeNull();
        context.EditParticipant.SetState(
            hasDraftChanges: false,
            isEditorBusy: true,
            operationState: NativeFormListEditorOperationState.Applying);
        context.Workspace.OnSaveAsync = (request, _) =>
        {
            request.ExpectedRevision.ShouldBe(appliedRevision);
            context.Workspace.State = ReadyState(context.Workspace.State, committedBaseline, savedRevision);
            context.Workspace.Preview = new WorkspacePreview([], 0, []);
            return ValueTask.FromResult(Committed(
                context.Workspace.WorkspaceId,
                request,
                savedRevision,
                committedBaseline));
        };
        ConfigureSuccessfulParticipantRefresh(context);
        context.DialogService.OnShowAsync = async (viewModel, _) =>
        {
            var cancelTask = viewModel.ApplyLeaveChoiceAsync(NativeWorkspaceChangesDialogChoice.CancelEditorOperationAndWait);
            editorLease.CancellationToken.IsCancellationRequested.ShouldBeTrue();

            context.Workspace.State = ReadyState(initialState, initialState.OutputBaseline!, appliedRevision);
            context.Workspace.Preview = stagedPreview;
            context.EditParticipant.SetState(hasDraftChanges: false);
            editorLease.Dispose();
            var cancelOutcome = await cancelTask;

            cancelOutcome.ShouldBe(NativeWorkspaceLeaveChoiceOutcome.ContinueDialog);
            viewModel.CurrentReview.ShouldNotBeNull();
            viewModel.CurrentReview.Revision.ShouldBe(appliedRevision);
            viewModel.CanSaveAndProceed.ShouldBeTrue();

            var saveOutcome = await viewModel.ApplyLeaveChoiceAsync(NativeWorkspaceChangesDialogChoice.SaveAndProceed);
            saveOutcome.ShouldBe(NativeWorkspaceLeaveChoiceOutcome.Proceed);
            return NativeWorkspaceChangesDialogResult.Proceed;
        };

        using var reservation = await context.ViewModel.ReserveLeaveAsync(NativeWorkspaceLeaveReason.CloseWorkspace);

        reservation.ShouldNotBeNull();
        reservation.Disposition.ShouldBe(NativeWorkspaceLeaveDisposition.ReadyAndClean);
        context.Workspace.SaveRequests.Count.ShouldBe(1);
        context.Workspace.SaveRequests[0].ExpectedRevision.ShouldBe(appliedRevision);
    }

    /// <summary>Verifies a clean no-workspace result repeats final proof and keeps admission through the caller-owned reservation.</summary>
    /// <returns>A task that completes after the scenario assertions.</returns>
    [Fact]
    public async Task ApplyLeaveChoiceAsync_WaitProducesNoWorkspace_TransfersFinalLeaseContinuously()
    {
        await using var context = NativeWorkspaceChangesTestContext.CreateNoWorkspace();
        using var editorLease = context.Arbiter.TryBeginEditorOperation().ShouldNotBeNull();
        context.EditParticipant.SetState(
            hasDraftChanges: false,
            isEditorBusy: true,
            operationState: NativeFormListEditorOperationState.Applying);
        context.DialogService.OnShowAsync = async (viewModel, _) =>
        {
            var choiceTask = viewModel.ApplyLeaveChoiceAsync(NativeWorkspaceChangesDialogChoice.WaitForEditorOperation);
            context.EditParticipant.SetState(hasDraftChanges: false);
            editorLease.Dispose();
            var outcome = await choiceTask;

            outcome.ShouldBe(NativeWorkspaceLeaveChoiceOutcome.Proceed);
            context.Arbiter.IsWorkspaceTransitionPendingOrReserved.ShouldBeTrue();
            context.Arbiter.TryBeginEditorOperation().ShouldBeNull();
            return NativeWorkspaceChangesDialogResult.Proceed;
        };

        using var reservation = await context.ViewModel.ReserveLeaveAsync(NativeWorkspaceLeaveReason.ExitApplication);

        reservation.ShouldNotBeNull();
        reservation.Disposition.ShouldBe(NativeWorkspaceLeaveDisposition.NoWorkspace);
        reservation.IsActive.ShouldBeTrue();
        context.Coordinator.ExecuteCount.ShouldBe(0);
        context.Arbiter.IsWorkspaceTransitionPendingOrReserved.ShouldBeTrue();
        context.Arbiter.TryBeginEditorOperation().ShouldBeNull();
    }
}
