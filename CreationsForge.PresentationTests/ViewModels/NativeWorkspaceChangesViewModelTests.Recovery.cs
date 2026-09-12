using CreationsForge.Core.Engine.Contracts;
using CreationsForge.PresentationTests.Support;
using CreationsForge.Services;
using Shouldly;

namespace CreationsForge.PresentationTests.ViewModels;

/// <content>Verifies repair replay, terminal repair mapping, and explicit recovery adoption.</content>
public sealed partial class NativeWorkspaceChangesViewModelTests
{
    /// <summary>Verifies a committed save whose reopen failed retains terminal evidence for explicit reopen.</summary>
    /// <returns>A task that completes after the scenario assertions.</returns>
    [Fact]
    public async Task SaveChangesAsync_CommittedButReopenFailed_OffersResolvedOutputReopen()
    {
        await using var context = NativeWorkspaceChangesTestContext.CreateReady(hasStagedChanges: true);
        var initialState = context.Workspace!.State;
        var operationId = Guid.Empty;
        ResolvedOutputEvidence? evidence = null;
        context.Workspace.OnSaveAsync = (request, _) =>
        {
            operationId = request.OperationId;
            var pending = new PendingSaveIdentity(
                context.Workspace.WorkspaceId,
                request.OperationId,
                request.ExpectedRevision,
                initialState.Game,
                initialState.Release,
                initialState.Output!);
            var committedBaseline = ReplacementBaseline(initialState.Output!);
            evidence = Evidence(
                pending,
                committedBaseline,
                RecoverSaveStatus.Committed,
                new RecoveryEvidenceToken("committed-reopen-token"));
            context.Workspace.State = new WorkspaceState(
                initialState.Game,
                initialState.Release,
                initialState.Output,
                committedBaseline,
                new OutputSynchronizationState(OutputSynchronizationStatus.ReopenRequired, pending),
                initialState.Revision);
            return ValueTask.FromResult(new SaveResult(
                context.Workspace.WorkspaceId,
                request.OperationId,
                request.ExpectedRevision,
                request.ExpectedRevision,
                SaveCommitStatus.CommittedButReopenFailed,
                committedBaseline,
                evidence.EvidenceToken,
                evidence,
                new EngineError(EngineErrorCode.OutputOpenFailed, "The committed output could not reopen."),
                warnings: []));
        };
        context.DialogService.OnShowAsync = async (viewModel, _) =>
        {
            await viewModel.SaveChangesAsync();
            viewModel.OutputSynchronizationStatus.ShouldBe(OutputSynchronizationStatus.ReopenRequired);
            viewModel.CanReopenResolvedOutput.ShouldBeTrue();
            viewModel.CanResumeUnsavedChanges.ShouldBeFalse();
            viewModel.ReopenResolvedOutputLabel.ShouldBe("Reopen Saved Output");
            viewModel.RecoveryEnvelope!.Result.ResolvedEvidence.ShouldBeSameAs(evidence);
            ConfigureSuccessfulAdoption(context, evidence!.ResolvedOutputBaseline);
            await viewModel.ReopenResolvedOutputAsync();
            return NativeWorkspaceChangesDialogResult.KeepEditing;
        };

        await context.ViewModel.ShowSaveChangesDialogAsync();

        operationId.ShouldNotBe(Guid.Empty);
        context.Workspace.ResolveOutputRecoveryRequests.Count.ShouldBe(1);
        context.Workspace.ResolveOutputRecoveryRequests[0].Mode.ShouldBe(OutputRecoveryAdoptionMode.ReopenResolvedOutput);
        context.Workspace.ResolveOutputRecoveryRequests[0].Evidence.ShouldBeSameAs(evidence);
        context.EditParticipant.RefreshRequests.Count.ShouldBe(1);
    }

    /// <summary>Verifies an incomplete partial-repair result invalidates reviewed evidence until a fresh inspection.</summary>
    /// <returns>A task that completes after the scenario assertions.</returns>
    [Fact]
    public async Task CompletePreparedSaveAsync_StillUnknown_RequiresFreshInspectionBeforeNewRepair()
    {
        await using var context = NativeWorkspaceChangesTestContext.CreateReady(hasStagedChanges: true);
        SetRecoveryRequired(context, context.Workspace!.WorkspaceId, out var pending);
        var currentToken = new RecoveryEvidenceToken("partial-before-token");
        context.SaveCoordinator.OnRecoverAsync = (_, _) => ValueTask.FromResult(RepairRequiredRecovery(pending, currentToken));
        context.SaveCoordinator.OnRepairAsync = (request, _) => ValueTask.FromResult(new RepairSaveResult(
            request.WorkspaceId,
            request.SaveOperationId,
            request.RepairOperationId,
            RepairSaveStatus.StillUnknown,
            resultingBaseline: null,
            new RecoveryEvidenceToken("partial-result-token"),
            resolvedEvidence: null,
            new EngineError(EngineErrorCode.CommitOutcomeUnknown, "The partial repair outcome remains unknown.")));
        context.DialogService.OnShowAsync = async (viewModel, _) =>
        {
            await viewModel.InspectSaveOutcomeAsync();
            await viewModel.CompletePreparedSaveAsync();
            viewModel.CanCompletePreparedSave.ShouldBeFalse();
            viewModel.CanRestorePreviousOutput.ShouldBeFalse();

            await viewModel.CompletePreparedSaveAsync();
            context.SaveCoordinator.RepairRequests.Count.ShouldBe(1);

            currentToken = new RecoveryEvidenceToken("partial-after-inspection-token");
            await viewModel.InspectSaveOutcomeAsync();
            viewModel.CanCompletePreparedSave.ShouldBeTrue();
            await viewModel.CompletePreparedSaveAsync();
            return NativeWorkspaceChangesDialogResult.KeepEditing;
        };

        await context.ViewModel.ReserveLeaveAsync(NativeWorkspaceLeaveReason.CloseWorkspace);

        context.SaveCoordinator.RepairRequests.Count.ShouldBe(2);
        context.SaveCoordinator.RepairRequests[0].SaveOperationId.ShouldBe(pending.SaveOperationId);
        context.SaveCoordinator.RepairRequests[1].SaveOperationId.ShouldBe(pending.SaveOperationId);
        context.SaveCoordinator.RepairRequests[0].EvidenceToken.Value.ShouldBe("partial-before-token");
        context.SaveCoordinator.RepairRequests[1].EvidenceToken.Value.ShouldBe("partial-after-inspection-token");
        context.SaveCoordinator.RepairRequests[0].RepairOperationId
            .ShouldNotBe(context.SaveCoordinator.RepairRequests[1].RepairOperationId);
    }

    /// <summary>Verifies response loss retains and exactly replays the original repair request.</summary>
    /// <returns>A task that completes after the scenario assertions.</returns>
    [Fact]
    public async Task CompletePreparedSaveAsync_ResponseLoss_ReplaysExactRequest()
    {
        await using var context = NativeWorkspaceChangesTestContext.CreateReady(hasStagedChanges: true);
        SetRecoveryRequired(context, context.Workspace!.WorkspaceId, out var pending);
        var token = new RecoveryEvidenceToken("repair-response-loss-token");
        context.SaveCoordinator.OnRecoverAsync = (_, _) => ValueTask.FromResult(RepairRequiredRecovery(pending, token));
        var attempt = 0;
        context.SaveCoordinator.OnRepairAsync = (request, _) =>
        {
            attempt++;
            if (attempt == 1)
            {
                throw new InvalidOperationException("Injected response loss.");
            }

            return ValueTask.FromResult(new RepairSaveResult(
                request.WorkspaceId,
                request.SaveOperationId,
                request.RepairOperationId,
                RepairSaveStatus.NotStarted,
                resultingBaseline: null,
                evidenceToken: null,
                resolvedEvidence: null,
                new EngineError(EngineErrorCode.RepairRequired, "The replayed repair did not start.")));
        };
        context.DialogService.OnShowAsync = async (viewModel, _) =>
        {
            await viewModel.InspectSaveOutcomeAsync();
            await viewModel.CompletePreparedSaveAsync();
            viewModel.CanCompletePreparedSave.ShouldBeTrue();
            viewModel.CanRestorePreviousOutput.ShouldBeFalse();
            await viewModel.CompletePreparedSaveAsync();
            return NativeWorkspaceChangesDialogResult.KeepEditing;
        };

        await context.ViewModel.ReserveLeaveAsync(NativeWorkspaceLeaveReason.CloseWorkspace);

        context.SaveCoordinator.RepairRequests.Count.ShouldBe(2);
        context.SaveCoordinator.RepairRequests[1].ShouldBeSameAs(context.SaveCoordinator.RepairRequests[0]);
        context.SaveCoordinator.RepairRequests[0].RepairOperationId.ShouldNotBe(Guid.Empty);
        context.SaveCoordinator.RepairRequests[0].RepairOperationId.ShouldNotBe(pending.SaveOperationId);
    }

    /// <summary>Verifies completing the prepared set publishes committed evidence for explicit reopen.</summary>
    /// <returns>A task that completes after the scenario assertions.</returns>
    [Fact]
    public async Task CompletePreparedSaveAsync_PreparedSetCompleted_OffersReopenOnly()
    {
        await using var context = NativeWorkspaceChangesTestContext.CreateReady(hasStagedChanges: true);
        SetRecoveryRequired(context, context.Workspace!.WorkspaceId, out var pending);
        var token = new RecoveryEvidenceToken("complete-prepared-token");
        var baseline = ReplacementBaseline(context.Workspace.State.Output!);
        var evidence = Evidence(pending, baseline, RecoverSaveStatus.Committed, token);
        context.SaveCoordinator.OnRecoverAsync = (_, _) => ValueTask.FromResult(RepairRequiredRecovery(pending, token));
        context.SaveCoordinator.OnRepairAsync = (request, _) => ValueTask.FromResult(new RepairSaveResult(
            request.WorkspaceId,
            request.SaveOperationId,
            request.RepairOperationId,
            RepairSaveStatus.PreparedSetCompleted,
            baseline,
            evidence.EvidenceToken,
            evidence,
            error: null));
        context.DialogService.OnShowAsync = async (viewModel, _) =>
        {
            await viewModel.InspectSaveOutcomeAsync();
            await viewModel.CompletePreparedSaveAsync();
            viewModel.CanReopenResolvedOutput.ShouldBeTrue();
            viewModel.CanResumeUnsavedChanges.ShouldBeFalse();
            viewModel.StatusText.ShouldBe("The prepared complete output set was committed. Reopen the saved output.");
            return NativeWorkspaceChangesDialogResult.KeepEditing;
        };

        await context.ViewModel.ReserveLeaveAsync(NativeWorkspaceLeaveReason.CloseWorkspace);

        context.SaveCoordinator.RepairRequests.Count.ShouldBe(1);
        context.SaveCoordinator.RepairRequests[0].Direction.ShouldBe(RepairSaveDirection.CompletePrepared);
    }

    /// <summary>Verifies reopening after baseline restore explicitly labels and discards the original retained candidate.</summary>
    /// <returns>A task that completes after the scenario assertions.</returns>
    [Fact]
    public async Task ReopenResolvedOutputAsync_BaselineRestored_LabelsDiscardAndUsesReopenMode()
    {
        await using var context = NativeWorkspaceChangesTestContext.CreateReady(hasStagedChanges: true);
        SetRecoveryRequired(context, context.Workspace!.WorkspaceId, out var pending);
        var token = new RecoveryEvidenceToken("restore-baseline-token");
        var baseline = ReplacementBaseline(context.Workspace.State.Output!);
        var evidence = Evidence(pending, baseline, RecoverSaveStatus.NotCommitted, token);
        context.SaveCoordinator.OnRecoverAsync = (_, _) => ValueTask.FromResult(RepairRequiredRecovery(pending, token));
        context.SaveCoordinator.OnRepairAsync = (request, _) => ValueTask.FromResult(new RepairSaveResult(
            request.WorkspaceId,
            request.SaveOperationId,
            request.RepairOperationId,
            RepairSaveStatus.BaselineRestored,
            baseline,
            evidence.EvidenceToken,
            evidence,
            error: null));
        context.DialogService.OnShowAsync = async (viewModel, _) =>
        {
            await viewModel.InspectSaveOutcomeAsync();
            await viewModel.RestorePreviousOutputAsync();
            viewModel.CanResumeUnsavedChanges.ShouldBeTrue();
            viewModel.CanReopenResolvedOutput.ShouldBeTrue();
            viewModel.ReopenResolvedOutputLabel.ShouldBe("Discard Staged Changes and Reopen Output");
            ConfigureSuccessfulAdoption(context, baseline, "Discard Staged Changes and Reopen Output");
            await viewModel.ReopenResolvedOutputAsync();
            return NativeWorkspaceChangesDialogResult.KeepEditing;
        };

        await context.ViewModel.ReserveLeaveAsync(NativeWorkspaceLeaveReason.CloseWorkspace);

        context.SaveCoordinator.RepairRequests.Count.ShouldBe(1);
        context.SaveCoordinator.RepairRequests[0].Direction.ShouldBe(RepairSaveDirection.RestoreBaseline);
        context.Workspace.ResolveOutputRecoveryRequests.Count.ShouldBe(1);
        context.Workspace.ResolveOutputRecoveryRequests[0].Mode.ShouldBe(OutputRecoveryAdoptionMode.ReopenResolvedOutput);
        context.Workspace.ResolveOutputRecoveryRequests[0].Evidence.ShouldBeSameAs(evidence);
    }

    /// <summary>Verifies an authentic repair conflict disables stale repair evidence and exposes Open-only abandonment.</summary>
    /// <returns>A task that completes after the scenario assertions.</returns>
    [Fact]
    public async Task CompletePreparedSaveAsync_ExternalConflict_OffersOpenAbandonmentWithoutRepairRetry()
    {
        await using var context = NativeWorkspaceChangesTestContext.CreateReady(hasStagedChanges: true);
        SetRecoveryRequired(context, context.Workspace!.WorkspaceId, out var pending);
        var token = new RecoveryEvidenceToken("repair-conflict-token");
        context.SaveCoordinator.OnRecoverAsync = (_, _) => ValueTask.FromResult(RepairRequiredRecovery(pending, token));
        context.SaveCoordinator.OnRepairAsync = (request, _) => ValueTask.FromResult(new RepairSaveResult(
            request.WorkspaceId,
            request.SaveOperationId,
            request.RepairOperationId,
            RepairSaveStatus.BlockedByExternalChange,
            resultingBaseline: null,
            evidenceToken: null,
            resolvedEvidence: null,
            new EngineError(EngineErrorCode.ExternalChangeDetected, "The output changed externally.")));
        context.DialogService.OnShowAsync = async (viewModel, _) =>
        {
            await viewModel.InspectSaveOutcomeAsync();
            await viewModel.CompletePreparedSaveAsync();
            viewModel.CanCompletePreparedSave.ShouldBeFalse();
            viewModel.CanRestorePreviousOutput.ShouldBeFalse();
            viewModel.ShowConfirmedAbandonmentForOpen.ShouldBeTrue();
            viewModel.CanConfirmAbandonmentForOpen.ShouldBeTrue();
            return NativeWorkspaceChangesDialogResult.KeepEditing;
        };

        await context.ViewModel.ReserveLeaveAsync(NativeWorkspaceLeaveReason.OpenWorkspace);

        context.SaveCoordinator.RepairRequests.Count.ShouldBe(1);
        context.ViewModel.ErrorCode.ShouldBe(EngineErrorCode.ExternalChangeDetected);
    }

    /// <summary>Verifies original-workspace resume adopts exact evidence and refreshes without reopening persistence.</summary>
    /// <returns>A task that completes after the scenario assertions.</returns>
    [Fact]
    public async Task ResumeUnsavedChangesAsync_OriginalWorkspace_AdoptsAndRefreshes()
    {
        await using var context = NativeWorkspaceChangesTestContext.CreateReady(hasStagedChanges: true);
        SetRecoveryRequired(context, context.Workspace!.WorkspaceId, out var pending);
        var baseline = ReplacementBaseline(context.Workspace.State.Output!);
        var evidence = Evidence(pending, baseline);
        context.SaveCoordinator.OnRecoverAsync = (_, _) => ValueTask.FromResult(TerminalRecovery(pending, evidence));
        ConfigureSuccessfulAdoption(context, baseline);
        context.DialogService.OnShowAsync = async (viewModel, _) =>
        {
            await viewModel.InspectSaveOutcomeAsync();
            await viewModel.ResumeUnsavedChangesAsync();
            viewModel.StatusText.ShouldBe("Unsaved changes resumed.");
            return NativeWorkspaceChangesDialogResult.KeepEditing;
        };

        await context.ViewModel.ReserveLeaveAsync(NativeWorkspaceLeaveReason.CloseWorkspace);

        context.Workspace.ResolveOutputRecoveryRequests.Count.ShouldBe(1);
        var request = context.Workspace.ResolveOutputRecoveryRequests[0];
        request.Mode.ShouldBe(OutputRecoveryAdoptionMode.ResumeStagedAfterNotCommitted);
        request.Evidence.ShouldBeSameAs(evidence);
        context.EditParticipant.RefreshRequests.Count.ShouldBe(1);
    }

    /// <summary>Verifies a fresh workspace may reopen terminal evidence but never resume another retained candidate.</summary>
    /// <returns>A task that completes after the scenario assertions.</returns>
    [Fact]
    public async Task ReopenResolvedOutputAsync_FreshWorkspace_AdoptsReopenOnly()
    {
        await using var context = NativeWorkspaceChangesTestContext.CreateReady(hasStagedChanges: true);
        SetRecoveryRequired(context, Guid.NewGuid(), out var pending);
        var baseline = ReplacementBaseline(context.Workspace!.State.Output!);
        var evidence = Evidence(pending, baseline);
        context.SaveCoordinator.OnRecoverAsync = (_, _) => ValueTask.FromResult(TerminalRecovery(pending, evidence));
        ConfigureSuccessfulAdoption(context, baseline);
        context.DialogService.OnShowAsync = async (viewModel, _) =>
        {
            await viewModel.InspectSaveOutcomeAsync();
            viewModel.CanResumeUnsavedChanges.ShouldBeFalse();
            viewModel.ReopenResolvedOutputLabel.ShouldBe("Reopen Resolved Output");
            await viewModel.ReopenResolvedOutputAsync();
            viewModel.StatusText.ShouldBe("Recovered output reopened.");
            return NativeWorkspaceChangesDialogResult.KeepEditing;
        };

        await context.ViewModel.ReserveLeaveAsync(NativeWorkspaceLeaveReason.CloseWorkspace);

        context.Workspace.ResolveOutputRecoveryRequests.Count.ShouldBe(1);
        context.Workspace.ResolveOutputRecoveryRequests[0].Mode.ShouldBe(OutputRecoveryAdoptionMode.ReopenResolvedOutput);
        context.Workspace.ResolveOutputRecoveryRequests[0].Evidence.ShouldBeSameAs(evidence);
    }

    /// <summary>Creates a repair-required unknown recovery result with exact original identity.</summary>
    internal static RecoverSaveResult RepairRequiredRecovery(
        PendingSaveIdentity pending,
        RecoveryEvidenceToken token)
        => new(
            pending.OriginalWorkspaceId,
            pending.SaveOperationId,
            RecoverSaveStatus.StillUnknown,
            pending.SaveBaseRevision,
            repairRequired: true,
            token,
            resolvedEvidence: null,
            new EngineError(EngineErrorCode.RepairRequired, "Explicit repair is required."));

    /// <summary>Creates an adoptable not-committed recovery result.</summary>
    internal static RecoverSaveResult TerminalRecovery(
        PendingSaveIdentity pending,
        ResolvedOutputEvidence evidence)
        => new(
            pending.OriginalWorkspaceId,
            pending.SaveOperationId,
            RecoverSaveStatus.NotCommitted,
            pending.SaveBaseRevision,
            repairRequired: false,
            evidence.EvidenceToken,
            evidence,
            error: null);

    /// <summary>Creates terminal evidence with an explicit status and reviewed token.</summary>
    internal static ResolvedOutputEvidence Evidence(
        PendingSaveIdentity pending,
        OutputArtifactSetBaseline resolvedBaseline,
        RecoverSaveStatus status,
        RecoveryEvidenceToken token)
        => new(
            token,
            pending.Game,
            pending.Release,
            pending.OriginalWorkspaceId,
            pending.SaveOperationId,
            pending.SaveBaseRevision,
            new CreationsForge.Core.Engine.NativeInputs.NativeSourceInputBaseline(Guid.NewGuid(), []),
            pending.Output,
            resolvedBaseline,
            status);

    /// <summary>Configures successful exact output-recovery adoption and participant refresh.</summary>
    /// <param name="context">The active changes test context.</param>
    /// <param name="baseline">The terminal baseline adopted by the workspace.</param>
    /// <param name="expectedBusyLabel">The optional action label that must remain stable while adoption is active.</param>
    private static void ConfigureSuccessfulAdoption(
        NativeWorkspaceChangesTestContext context,
        OutputArtifactSetBaseline baseline,
        string? expectedBusyLabel = null)
    {
        context.Workspace!.OnResolveOutputRecoveryAsync = (request, _) =>
        {
            if (expectedBusyLabel is not null)
            {
                context.ViewModel.IsBusy.ShouldBeTrue();
                context.ViewModel.ReopenResolvedOutputLabel.ShouldBe(expectedBusyLabel);
            }

            var prior = context.Workspace.State;
            var revision = prior.Revision.Next();
            context.Workspace.State = ReadyState(prior, baseline, revision);
            var receipt = new OutputSelectionReceipt(prior.Output!, baseline, revision);
            return ValueTask.FromResult(EngineResult<OutputSelectionReceipt>.Success(
                receipt,
                context.Workspace.WorkspaceId,
                request.OperationId,
                request.ExpectedRevision,
                revision));
        };
        ConfigureSuccessfulParticipantRefresh(context);
    }
}
