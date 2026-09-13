using CreationsForge.Core.Engine.Contracts;
using CreationsForge.PresentationTests.Support;
using CreationsForge.Services;
using CreationsForge.ViewModels;
using Shouldly;

namespace CreationsForge.PresentationTests.ViewModels;

/// <content>Verifies save, discard, exact envelope validation, and post-persistence refresh behavior.</content>
public sealed partial class WorkspaceChangesViewModelTests
{
    /// <summary>Verifies mutation methods cannot dispatch Core work without an owned dialog transition.</summary>
    /// <returns>A task that completes after the scenario assertions.</returns>
    [Fact]
    public async Task PersistenceMethods_WithoutOwnedTransition_DoNotDispatch()
    {
        await using var context = WorkspaceChangesTestContext.CreateReady(hasStagedChanges: true);

        await context.ViewModel.SaveChangesAsync();
        await context.ViewModel.DiscardChangesAsync();

        context.Workspace!.SaveRequests.ShouldBeEmpty();
        context.Workspace.DiscardRequests.ShouldBeEmpty();
        context.Coordinator.ExecuteCount.ShouldBe(0);
    }

    /// <summary>Verifies a committed save uses a fresh request and publishes only after exact state and participant refresh.</summary>
    /// <returns>A task that completes after the scenario assertions.</returns>
    [Fact]
    public async Task SaveChangesAsync_Committed_UsesFreshEnvelopeAndRefreshesParticipant()
    {
        await using var context = WorkspaceChangesTestContext.CreateReady(hasStagedChanges: true);
        var initialState = context.Workspace!.State;
        var committedBaseline = ReplacementBaseline(initialState.Output!);
        var committedRevision = initialState.Revision.Next();
        context.Workspace.OnSaveAsync = (request, _) =>
        {
            context.Workspace.State = ReadyState(initialState, committedBaseline, committedRevision);
            return ValueTask.FromResult(new SaveResult(
                context.Workspace.WorkspaceId,
                request.OperationId,
                request.ExpectedRevision,
                committedRevision,
                SaveCommitStatus.Committed,
                committedBaseline,
                recoveryEvidenceToken: null,
                resolvedEvidence: null,
                error: null,
                warnings: []));
        };
        ConfigureSuccessfulParticipantRefresh(context);
        context.DialogService.OnShowAsync = async (viewModel, _) =>
        {
            await viewModel.SaveChangesAsync();
            viewModel.StatusText.ShouldBe("Saved and reopened.");
            viewModel.IsReviewStale.ShouldBeTrue();
            viewModel.CanSaveChanges.ShouldBeFalse();
            return WorkspaceChangesDialogResult.KeepEditing;
        };

        await context.ViewModel.ShowSaveChangesDialogAsync();

        context.Workspace.SaveRequests.Count.ShouldBe(1);
        context.Workspace.SaveRequests[0].ExpectedRevision.ShouldBe(initialState.Revision);
        context.Workspace.SaveRequests[0].ExpectedOutputBaseline.ShouldBeSameAs(initialState.OutputBaseline);
        context.Workspace.SaveRequests[0].OperationId.ShouldNotBe(Guid.Empty);
        context.EditParticipant.RefreshRequests.Count.ShouldBe(1);
        context.EditParticipant.RefreshRequests[0].WorkspaceId.ShouldBe(context.Workspace.WorkspaceId);
        context.EditParticipant.RefreshRequests[0].Revision.ShouldBe(committedRevision);
        context.ViewModel.PendingRefreshEnvelope.ShouldBeNull();
    }

    /// <summary>Verifies a definitive save rejection keeps staged state and reports the typed noncommit cause.</summary>
    /// <returns>A task that completes after the scenario assertions.</returns>
    [Fact]
    public async Task SaveChangesAsync_NotCommitted_PreservesStagedReview()
    {
        await using var context = WorkspaceChangesTestContext.CreateReady(hasStagedChanges: true);
        var state = context.Workspace!.State;
        context.Workspace.OnSaveAsync = (request, _) => ValueTask.FromResult(new SaveResult(
            context.Workspace.WorkspaceId,
            request.OperationId,
            request.ExpectedRevision,
            request.ExpectedRevision,
            SaveCommitStatus.NotCommitted,
            committedBaseline: null,
            recoveryEvidenceToken: null,
            resolvedEvidence: null,
            new EngineError(EngineErrorCode.OutputDirectoryBusy, "The output lease is busy."),
            warnings: []));
        context.DialogService.OnShowAsync = async (viewModel, _) =>
        {
            await viewModel.SaveChangesAsync();
            viewModel.ErrorCode.ShouldBe(EngineErrorCode.OutputDirectoryBusy);
            viewModel.StatusText.ShouldBe("The output folder is in use. No files were changed.");
            viewModel.CurrentReview!.HasStagedChanges.ShouldBeTrue();
            viewModel.CanSaveChanges.ShouldBeTrue();
            return WorkspaceChangesDialogResult.KeepEditing;
        };

        await context.ViewModel.ShowSaveChangesDialogAsync();

        context.Workspace.SaveRequests.Count.ShouldBe(1);
        context.Workspace.State.ShouldBeSameAs(state);
        context.EditParticipant.RefreshRequests.ShouldBeEmpty();
    }

    /// <summary>Verifies a definitive failed save retry receives a fresh operation identifier.</summary>
    /// <returns>A task that completes after the scenario assertions.</returns>
    [Fact]
    public async Task SaveChangesAsync_DefinitiveRetry_UsesNewOperationId()
    {
        await using var context = WorkspaceChangesTestContext.CreateReady(hasStagedChanges: true);
        context.Workspace!.OnSaveAsync = (request, _) => ValueTask.FromResult(new SaveResult(
            context.Workspace.WorkspaceId,
            request.OperationId,
            request.ExpectedRevision,
            request.ExpectedRevision,
            SaveCommitStatus.NotCommitted,
            committedBaseline: null,
            recoveryEvidenceToken: null,
            resolvedEvidence: null,
            new EngineError(EngineErrorCode.ValidationFailed, "The save was rejected."),
            warnings: []));
        context.DialogService.OnShowAsync = async (viewModel, _) =>
        {
            await viewModel.SaveChangesAsync();
            await viewModel.SaveChangesAsync();
            return WorkspaceChangesDialogResult.KeepEditing;
        };

        await context.ViewModel.ShowSaveChangesDialogAsync();

        context.Workspace.SaveRequests.Count.ShouldBe(2);
        context.Workspace.SaveRequests[0].OperationId.ShouldNotBe(context.Workspace.SaveRequests[1].OperationId);
        context.Workspace.SaveRequests.ShouldAllBe(request =>
            request.ExpectedRevision == context.Workspace.State.Revision
            && ReferenceEquals(request.ExpectedOutputBaseline, context.Workspace.State.OutputBaseline));
    }

    /// <summary>Verifies an unknown save latches its original identity and recovery inspection reuses that identity.</summary>
    /// <returns>A task that completes after the scenario assertions.</returns>
    [Fact]
    public async Task SaveChangesAsync_Unknown_RecoversOriginalSaveIdentity()
    {
        await using var context = WorkspaceChangesTestContext.CreateReady(hasStagedChanges: true);
        var initialState = context.Workspace!.State;
        var evidenceToken = new RecoveryEvidenceToken("unknown-save-token");
        context.Workspace.OnSaveAsync = (request, _) =>
        {
            var pending = new PendingSaveIdentity(
                context.Workspace.WorkspaceId,
                request.OperationId,
                request.ExpectedRevision,
                initialState.Game,
                initialState.Release,
                initialState.Output!);
            context.Workspace.State = new WorkspaceState(
                initialState.Game,
                initialState.Release,
                initialState.Output,
                initialState.OutputBaseline,
                new OutputSynchronizationState(OutputSynchronizationStatus.RecoveryRequired, pending),
                initialState.Revision);
            return ValueTask.FromResult(new SaveResult(
                context.Workspace.WorkspaceId,
                request.OperationId,
                request.ExpectedRevision,
                request.ExpectedRevision,
                SaveCommitStatus.CommitOutcomeUnknown,
                committedBaseline: null,
                evidenceToken,
                resolvedEvidence: null,
                new EngineError(EngineErrorCode.CommitOutcomeUnknown, "The commit response is unknown."),
                warnings: []));
        };
        context.SaveCoordinator.OnRecoverAsync = (request, _) => ValueTask.FromResult(new RecoverSaveResult(
            request.WorkspaceId,
            request.SaveOperationId,
            RecoverSaveStatus.StillUnknown,
            initialState.Revision,
            repairRequired: false,
            evidenceToken,
            resolvedEvidence: null,
            new EngineError(EngineErrorCode.CommitOutcomeUnknown, "The save is still unknown.")));
        context.DialogService.OnShowAsync = async (viewModel, _) =>
        {
            await viewModel.SaveChangesAsync();
            viewModel.OutputSynchronizationStatus.ShouldBe(OutputSynchronizationStatus.RecoveryRequired);
            viewModel.CanInspectSaveOutcome.ShouldBeTrue();
            await viewModel.InspectSaveOutcomeAsync();
            return WorkspaceChangesDialogResult.KeepEditing;
        };

        await context.ViewModel.ShowSaveChangesDialogAsync();

        context.Workspace.SaveRequests.Count.ShouldBe(1);
        context.SaveCoordinator.RecoverRequests.Count.ShouldBe(1);
        context.SaveCoordinator.RecoverRequests[0].WorkspaceId.ShouldBe(context.Workspace.WorkspaceId);
        context.SaveCoordinator.RecoverRequests[0].SaveOperationId.ShouldBe(context.Workspace.SaveRequests[0].OperationId);
        context.SaveCoordinator.RecoverRequests[0].Output.ShouldBeSameAs(initialState.Output);
    }

    /// <summary>Verifies a malformed save response is never accepted as a definitive result.</summary>
    /// <returns>A task that completes after the scenario assertions.</returns>
    [Fact]
    public async Task SaveChangesAsync_MismatchedOperationId_RequiresRecovery()
    {
        await using var context = WorkspaceChangesTestContext.CreateReady(hasStagedChanges: true);
        context.Workspace!.OnSaveAsync = (request, _) => ValueTask.FromResult(new SaveResult(
            context.Workspace.WorkspaceId,
            Guid.NewGuid(),
            request.ExpectedRevision,
            request.ExpectedRevision,
            SaveCommitStatus.NotCommitted,
            committedBaseline: null,
            recoveryEvidenceToken: null,
            resolvedEvidence: null,
            new EngineError(EngineErrorCode.ValidationFailed, "Rejected."),
            warnings: []));
        context.DialogService.OnShowAsync = async (viewModel, _) =>
        {
            await viewModel.SaveChangesAsync();
            viewModel.ErrorCode.ShouldBe(EngineErrorCode.UnexpectedFailure);
            viewModel.OutputSynchronizationStatus.ShouldBe(OutputSynchronizationStatus.RecoveryRequired);
            return WorkspaceChangesDialogResult.KeepEditing;
        };

        await context.ViewModel.ShowSaveChangesDialogAsync();

        context.Workspace.SaveRequests.Count.ShouldBe(1);
        context.EditParticipant.RefreshRequests.ShouldBeEmpty();
    }

    /// <summary>Verifies a committed save with a failed post-read retries refresh without replaying persistence.</summary>
    /// <returns>A task that completes after the scenario assertions.</returns>
    [Fact]
    public async Task SaveChangesAsync_CommittedPostReadFailure_RetryDoesNotReplaySave()
    {
        await using var context = WorkspaceChangesTestContext.CreateReady(hasStagedChanges: true);
        var initialState = context.Workspace!.State;
        var committedBaseline = ReplacementBaseline(initialState.Output!);
        var committedRevision = initialState.Revision.Next();
        var failPostRead = true;
        context.Workspace.OnReadStateAsync = _ =>
        {
            if (failPostRead && context.Workspace.ReadStateCount >= 3)
            {
                throw new InvalidOperationException("Injected post-save read failure.");
            }

            return ValueTask.FromResult(StateResult(context.Workspace));
        };
        context.Workspace.OnSaveAsync = (request, _) =>
        {
            context.Workspace.State = ReadyState(initialState, committedBaseline, committedRevision);
            return ValueTask.FromResult(Committed(context.Workspace.WorkspaceId, request, committedRevision, committedBaseline));
        };
        ConfigureSuccessfulParticipantRefresh(context);
        context.DialogService.OnShowAsync = async (viewModel, _) =>
        {
            await viewModel.SaveChangesAsync();
            viewModel.CanRetryCommittedRefresh.ShouldBeTrue();
            context.Workspace.SaveRequests.Count.ShouldBe(1);

            failPostRead = false;
            await viewModel.RetryCommittedRefreshAsync();
            viewModel.CanRetryCommittedRefresh.ShouldBeFalse();
            context.Workspace.SaveRequests.Count.ShouldBe(1);
            return WorkspaceChangesDialogResult.KeepEditing;
        };

        await context.ViewModel.ShowSaveChangesDialogAsync();

        context.EditParticipant.RefreshRequests.Count.ShouldBe(1);
        context.EditParticipant.RefreshRequests[0].Revision.ShouldBe(committedRevision);
    }

    /// <summary>Verifies participant publication failure retries only the participant step after a committed save.</summary>
    /// <returns>A task that completes after the scenario assertions.</returns>
    [Fact]
    public async Task SaveChangesAsync_ParticipantRefreshFailure_RetryDoesNotReplaySaveOrStateRead()
    {
        await using var context = WorkspaceChangesTestContext.CreateReady(hasStagedChanges: true);
        var initialState = context.Workspace!.State;
        var committedBaseline = ReplacementBaseline(initialState.Output!);
        var committedRevision = initialState.Revision.Next();
        context.Workspace.OnSaveAsync = (request, _) =>
        {
            context.Workspace.State = ReadyState(initialState, committedBaseline, committedRevision);
            return ValueTask.FromResult(Committed(context.Workspace.WorkspaceId, request, committedRevision, committedBaseline));
        };
        var refreshAttempt = 0;
        context.EditParticipant.OnRefreshAsync = (workspaceId, revision, _) =>
        {
            refreshAttempt++;
            return Task.FromResult(refreshAttempt == 1
                ? EngineResult<WorkspaceState>.Failure(
                    new EngineError(EngineErrorCode.UnexpectedFailure, "Injected participant failure."),
                    workspaceId,
                    baseRevision: revision,
                    resultRevision: revision)
                : StateResult(context.Workspace));
        };
        context.DialogService.OnShowAsync = async (viewModel, _) =>
        {
            await viewModel.SaveChangesAsync();
            viewModel.PendingRefreshEnvelope!.Step.ShouldBe(WorkspaceRefreshStep.RefreshParticipant);
            var stateReadsBeforeRetry = context.Workspace.ReadStateCount;
            await viewModel.RetryCommittedRefreshAsync();
            context.Workspace.ReadStateCount.ShouldBe(stateReadsBeforeRetry);
            return WorkspaceChangesDialogResult.KeepEditing;
        };

        await context.ViewModel.ShowSaveChangesDialogAsync();

        context.Workspace.SaveRequests.Count.ShouldBe(1);
        context.EditParticipant.RefreshRequests.Count.ShouldBe(2);
        context.ViewModel.PendingRefreshEnvelope.ShouldBeNull();
    }

    /// <summary>Verifies an empty fresh preview discards only request-local input and never calls Core discard.</summary>
    /// <returns>A task that completes after the scenario assertions.</returns>
    [Fact]
    public async Task DiscardChangesAsync_LocalDraftOnly_DoesNotCallCoreDiscard()
    {
        await using var context = WorkspaceChangesTestContext.CreateReady(hasDraftChanges: true);
        context.DialogService.OnShowAsync = async (viewModel, _) =>
        {
            await viewModel.DiscardChangesAsync();
            viewModel.HasDraftChanges.ShouldBeFalse();
            viewModel.CurrentReview!.HasStagedChanges.ShouldBeFalse();
            return WorkspaceChangesDialogResult.KeepEditing;
        };

        await context.ViewModel.ShowDiscardChangesDialogAsync();

        context.EditParticipant.DiscardRequestLocalCount.ShouldBe(1);
        context.Workspace!.DiscardRequests.ShouldBeEmpty();
    }

    /// <summary>Verifies staged discard uses an exact request and refreshes the resulting revision.</summary>
    /// <returns>A task that completes after the scenario assertions.</returns>
    [Fact]
    public async Task DiscardChangesAsync_StagedChanges_UsesExactEnvelopeAndRefreshes()
    {
        await using var context = WorkspaceChangesTestContext.CreateReady(hasStagedChanges: true);
        var initialState = context.Workspace!.State;
        var discardedRevision = initialState.Revision.Next();
        context.Workspace.OnDiscardAsync = (request, _) =>
        {
            context.Workspace.State = ReadyState(initialState, initialState.OutputBaseline!, discardedRevision);
            var receipt = new OperationReceipt(request.OperationId, discardedRevision);
            return ValueTask.FromResult(EngineResult<OperationReceipt>.Success(
                receipt,
                context.Workspace.WorkspaceId,
                request.OperationId,
                request.ExpectedRevision,
                discardedRevision));
        };
        ConfigureSuccessfulParticipantRefresh(context);
        context.DialogService.OnShowAsync = async (viewModel, _) =>
        {
            await viewModel.DiscardChangesAsync();
            viewModel.StatusText.ShouldBe("Discarded staged changes and reopened the selected output.");
            return WorkspaceChangesDialogResult.KeepEditing;
        };

        await context.ViewModel.ShowDiscardChangesDialogAsync();

        context.Workspace.DiscardRequests.Count.ShouldBe(1);
        context.Workspace.DiscardRequests[0].ExpectedRevision.ShouldBe(initialState.Revision);
        context.Workspace.DiscardRequests[0].ExpectedBaseline.ShouldBeSameAs(initialState.OutputBaseline);
        context.EditParticipant.RefreshRequests.Count.ShouldBe(1);
        context.EditParticipant.RefreshRequests[0].WorkspaceId.ShouldBe(context.Workspace.WorkspaceId);
        context.EditParticipant.RefreshRequests[0].Revision.ShouldBe(discardedRevision);
    }

    /// <summary>Verifies cancellation during the fresh pre-save preview drains without dispatching persistence.</summary>
    /// <returns>A task that completes after the scenario assertions.</returns>
    [Fact]
    public async Task SaveChangesAsync_CanceledDuringFreshPreview_DoesNotDispatchSave()
    {
        await using var context = WorkspaceChangesTestContext.CreateReady(hasStagedChanges: true);
        var freshPreviewEntered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        context.Workspace!.OnPreviewAsync = async token =>
        {
            if (context.Workspace.PreviewCount == 1)
            {
                return EngineResult<WorkspacePreview>.Success(
                    context.Workspace.Preview,
                    context.Workspace.WorkspaceId,
                    baseRevision: context.Workspace.Revision,
                    resultRevision: context.Workspace.Revision);
            }

            freshPreviewEntered.TrySetResult();
            await Task.Delay(Timeout.InfiniteTimeSpan, token);
            throw new InvalidOperationException("The cancellable preview delay unexpectedly completed.");
        };
        context.DialogService.OnShowAsync = async (viewModel, _) =>
        {
            var saveTask = viewModel.SaveChangesAsync();
            await freshPreviewEntered.Task;
            await viewModel.CancelActiveOperationAndDrainAsync();
            await saveTask;
            return WorkspaceChangesDialogResult.KeepEditing;
        };

        await context.ViewModel.ShowSaveChangesDialogAsync();

        context.Workspace.SaveRequests.ShouldBeEmpty();
        context.ViewModel.IsBusy.ShouldBeFalse();
        context.Arbiter.IsWorkspaceTransitionPendingOrReserved.ShouldBeFalse();
    }

    /// <summary>Verifies cancellation after save dispatch waits for the definitive result and never truncates refresh mapping.</summary>
    /// <returns>A task that completes after the scenario assertions.</returns>
    [Fact]
    public async Task SaveChangesAsync_CanceledAfterDispatch_DrainsDefinitiveResultAndRefresh()
    {
        await using var context = WorkspaceChangesTestContext.CreateReady(hasStagedChanges: true);
        var initialState = context.Workspace!.State;
        var committedBaseline = ReplacementBaseline(initialState.Output!);
        var committedRevision = initialState.Revision.Next();
        var saveEntered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var releaseSave = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        context.Workspace.OnSaveAsync = async (request, _) =>
        {
            saveEntered.TrySetResult();
            await releaseSave.Task;
            context.Workspace.State = ReadyState(initialState, committedBaseline, committedRevision);
            return Committed(context.Workspace.WorkspaceId, request, committedRevision, committedBaseline);
        };
        ConfigureSuccessfulParticipantRefresh(context);
        context.DialogService.OnShowAsync = async (viewModel, _) =>
        {
            var saveTask = viewModel.SaveChangesAsync();
            await saveEntered.Task;
            var drainTask = viewModel.CancelActiveOperationAndDrainAsync();
            await Task.Yield();
            drainTask.IsCompleted.ShouldBeFalse();
            viewModel.IsCancelRequested.ShouldBeTrue();

            releaseSave.TrySetResult();
            await Task.WhenAll(saveTask, drainTask);
            viewModel.StatusText.ShouldBe("Saved and reopened.");
            return WorkspaceChangesDialogResult.KeepEditing;
        };

        await context.ViewModel.ShowSaveChangesDialogAsync();

        context.Workspace.SaveRequests.Count.ShouldBe(1);
        context.EditParticipant.RefreshRequests.Count.ShouldBe(1);
        context.ViewModel.LastSaveResult!.Status.ShouldBe(SaveCommitStatus.Committed);
    }

    /// <summary>Creates an exact committed save result.</summary>
    private static SaveResult Committed(
        Guid workspaceId,
        SaveRequest request,
        WorkspaceRevision resultRevision,
        OutputArtifactSetBaseline baseline)
        => new(
            workspaceId,
            request.OperationId,
            request.ExpectedRevision,
            resultRevision,
            SaveCommitStatus.Committed,
            baseline,
            recoveryEvidenceToken: null,
            resolvedEvidence: null,
            error: null,
            warnings: []);

    /// <summary>Creates a ready state derived from an earlier state.</summary>
    private static WorkspaceState ReadyState(
        WorkspaceState prior,
        OutputArtifactSetBaseline baseline,
        WorkspaceRevision revision)
        => new(
            prior.Game,
            prior.Release,
            prior.Output,
            baseline,
            new OutputSynchronizationState(OutputSynchronizationStatus.Ready, null),
            revision);

    /// <summary>Creates a successful exact state result from the recording workspace's current state.</summary>
    private static EngineResult<WorkspaceState> StateResult(RecordingWorkspaceChangesWorkspace workspace)
        => EngineResult<WorkspaceState>.Success(
            workspace.State,
            workspace.WorkspaceId,
            baseRevision: workspace.State.Revision,
            resultRevision: workspace.State.Revision);

    /// <summary>Configures the participant to accept the recording workspace's current exact state.</summary>
    private static void ConfigureSuccessfulParticipantRefresh(WorkspaceChangesTestContext context)
    {
        context.EditParticipant.OnRefreshAsync = (_, _, _) => Task.FromResult(StateResult(context.Workspace!));
    }
}
