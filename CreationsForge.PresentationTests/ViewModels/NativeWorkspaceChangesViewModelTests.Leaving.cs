using CreationsForge.Core.Engine.Contracts;
using CreationsForge.PresentationTests.Support;
using CreationsForge.Services;
using CreationsForge.ViewModels;
using Shouldly;

namespace CreationsForge.PresentationTests.ViewModels;

/// <content>Verifies final leave proof, lease transfer, and non-fabricable dialog outcomes.</content>
public sealed partial class NativeWorkspaceChangesViewModelTests
{
    /// <summary>Verifies an empty shell transfers a no-workspace permit without borrowing native state.</summary>
    /// <returns>A task that completes after the scenario assertions.</returns>
    [Fact]
    public async Task ReserveLeaveAsync_NoWorkspace_TransfersExactLeaseWithoutBorrow()
    {
        await using var context = NativeWorkspaceChangesTestContext.CreateNoWorkspace();

        using var reservation = await context.ViewModel.ReserveLeaveAsync(NativeWorkspaceLeaveReason.ExitApplication);

        reservation.ShouldNotBeNull();
        reservation.Reason.ShouldBe(NativeWorkspaceLeaveReason.ExitApplication);
        reservation.Disposition.ShouldBe(NativeWorkspaceLeaveDisposition.NoWorkspace);
        reservation.ExpectedWorkspaceId.ShouldBeNull();
        reservation.IsActive.ShouldBeTrue();
        context.Coordinator.ExecuteCount.ShouldBe(0);
        context.DialogService.Requests.ShouldBeEmpty();
        context.Arbiter.IsWorkspaceTransitionPendingOrReserved.ShouldBeTrue();
    }

    /// <summary>Verifies a clean workspace transfers a ready-and-clean permit after a fresh state and preview proof.</summary>
    /// <returns>A task that completes after the scenario assertions.</returns>
    [Fact]
    public async Task ReserveLeaveAsync_ReadyAndClean_TransfersExactLeaseWithoutDialog()
    {
        await using var context = NativeWorkspaceChangesTestContext.CreateReady();

        using var reservation = await context.ViewModel.ReserveLeaveAsync(NativeWorkspaceLeaveReason.ShowSettings);

        reservation.ShouldNotBeNull();
        reservation.Reason.ShouldBe(NativeWorkspaceLeaveReason.ShowSettings);
        reservation.Disposition.ShouldBe(NativeWorkspaceLeaveDisposition.ReadyAndClean);
        reservation.ExpectedWorkspaceId.ShouldBe(context.Workspace!.WorkspaceId);
        reservation.IsActive.ShouldBeTrue();
        context.Workspace.ReadStateCount.ShouldBe(2);
        context.Workspace.PreviewCount.ShouldBe(2);
        context.DialogService.Requests.ShouldBeEmpty();
    }

    /// <summary>Verifies a raw dialog Proceed result cannot fabricate a safe leave disposition or retain admission.</summary>
    /// <returns>A task that completes after the scenario assertions.</returns>
    [Fact]
    public async Task ReserveLeaveAsync_RawDialogProceed_DoesNotAuthorizeLeave()
    {
        await using var context = NativeWorkspaceChangesTestContext.CreateReady(hasStagedChanges: true);
        context.DialogService.Result = NativeWorkspaceChangesDialogResult.Proceed;

        var reservation = await context.ViewModel.ReserveLeaveAsync(NativeWorkspaceLeaveReason.CloseWorkspace);

        reservation.ShouldBeNull();
        context.ViewModel.ErrorCode.ShouldBe(EngineErrorCode.UnexpectedFailure);
        context.Arbiter.IsWorkspaceTransitionPendingOrReserved.ShouldBeFalse();
    }

    /// <summary>Verifies successful staged discard is followed by fresh clean proof before a leave permit transfers.</summary>
    /// <returns>A task that completes after the scenario assertions.</returns>
    [Fact]
    public async Task ReserveLeaveAsync_DiscardAndProceed_ReprovesCleanStateBeforeTransfer()
    {
        await using var context = NativeWorkspaceChangesTestContext.CreateReady(hasStagedChanges: true);
        var initialState = context.Workspace!.State;
        var resultRevision = initialState.Revision.Next();
        context.Workspace.OnDiscardAsync = (request, _) =>
        {
            context.Workspace.State = ReadyState(initialState, initialState.OutputBaseline!, resultRevision);
            context.Workspace.Preview = new WorkspacePreview([], 0, []);
            var receipt = new OperationReceipt(request.OperationId, resultRevision);
            return ValueTask.FromResult(EngineResult<OperationReceipt>.Success(
                receipt,
                context.Workspace.WorkspaceId,
                request.OperationId,
                request.ExpectedRevision,
                resultRevision));
        };
        ConfigureSuccessfulParticipantRefresh(context);
        context.DialogService.OnShowAsync = async (viewModel, _) =>
        {
            var outcome = await viewModel.ApplyLeaveChoiceAsync(NativeWorkspaceChangesDialogChoice.DiscardAndProceed);
            outcome.ShouldBe(NativeWorkspaceLeaveChoiceOutcome.Proceed);
            return NativeWorkspaceChangesDialogResult.Proceed;
        };

        using var reservation = await context.ViewModel.ReserveLeaveAsync(NativeWorkspaceLeaveReason.CloseWorkspace);

        reservation.ShouldNotBeNull();
        reservation.Disposition.ShouldBe(NativeWorkspaceLeaveDisposition.ReadyAndClean);
        reservation.ExpectedWorkspaceId.ShouldBe(context.Workspace.WorkspaceId);
        context.Workspace.DiscardRequests.Count.ShouldBe(1);
        context.Workspace.PreviewCount.ShouldBeGreaterThanOrEqualTo(3);
    }

    /// <summary>Verifies an authentic external discard conflict can authorize abandonment for Open only.</summary>
    /// <returns>A task that completes after the scenario assertions.</returns>
    [Fact]
    public async Task ReserveLeaveAsync_ExternalDiscardConflict_ConfirmsAbandonmentForOpenOnly()
    {
        await using var context = NativeWorkspaceChangesTestContext.CreateReady(hasStagedChanges: true);
        context.Workspace!.OnDiscardAsync = (request, _) => ValueTask.FromResult(EngineResult<OperationReceipt>.Failure(
            new EngineError(EngineErrorCode.ExternalChangeDetected, "The output changed externally."),
            context.Workspace.WorkspaceId,
            request.OperationId,
            request.ExpectedRevision,
            request.ExpectedRevision));
        context.DialogService.OnShowAsync = async (viewModel, request) =>
        {
            request.LeaveReason.ShouldBe(NativeWorkspaceLeaveReason.OpenWorkspace);
            var discardOutcome = await viewModel.ApplyLeaveChoiceAsync(NativeWorkspaceChangesDialogChoice.DiscardAndProceed);
            discardOutcome.ShouldBe(NativeWorkspaceLeaveChoiceOutcome.ContinueDialog);
            viewModel.ShowConfirmedAbandonmentForOpen.ShouldBeTrue();
            viewModel.CanConfirmAbandonmentForOpen.ShouldBeTrue();
            var confirmOutcome = await viewModel.ApplyLeaveChoiceAsync(NativeWorkspaceChangesDialogChoice.ConfirmAbandonmentForOpen);
            confirmOutcome.ShouldBe(NativeWorkspaceLeaveChoiceOutcome.Proceed);
            return NativeWorkspaceChangesDialogResult.Proceed;
        };

        using var reservation = await context.ViewModel.ReserveLeaveAsync(NativeWorkspaceLeaveReason.OpenWorkspace);

        reservation.ShouldNotBeNull();
        reservation.Disposition.ShouldBe(NativeWorkspaceLeaveDisposition.ConfirmedAbandonmentForOpen);
        reservation.ExpectedWorkspaceId.ShouldBe(context.Workspace.WorkspaceId);
    }

    /// <summary>Verifies the same authentic external conflict cannot expose abandonment for Close.</summary>
    /// <returns>A task that completes after the scenario assertions.</returns>
    [Fact]
    public async Task ReserveLeaveAsync_ExternalDiscardConflict_DoesNotAuthorizeCloseAbandonment()
    {
        await using var context = NativeWorkspaceChangesTestContext.CreateReady(hasStagedChanges: true);
        context.Workspace!.OnDiscardAsync = (request, _) => ValueTask.FromResult(EngineResult<OperationReceipt>.Failure(
            new EngineError(EngineErrorCode.ExternalChangeDetected, "The output changed externally."),
            context.Workspace.WorkspaceId,
            request.OperationId,
            request.ExpectedRevision,
            request.ExpectedRevision));
        context.DialogService.OnShowAsync = async (viewModel, _) =>
        {
            await viewModel.ApplyLeaveChoiceAsync(NativeWorkspaceChangesDialogChoice.DiscardAndProceed);
            viewModel.ShowConfirmedAbandonmentForOpen.ShouldBeFalse();
            var outcome = await viewModel.ApplyLeaveChoiceAsync(NativeWorkspaceChangesDialogChoice.ConfirmAbandonmentForOpen);
            outcome.ShouldBe(NativeWorkspaceLeaveChoiceOutcome.ContinueDialog);
            return NativeWorkspaceChangesDialogResult.KeepEditing;
        };

        var reservation = await context.ViewModel.ReserveLeaveAsync(NativeWorkspaceLeaveReason.CloseWorkspace);

        reservation.ShouldBeNull();
        context.Arbiter.IsWorkspaceTransitionPendingOrReserved.ShouldBeFalse();
    }
}
