using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.PluginInputs;
using CreationsForge.PresentationTests.Support;
using CreationsForge.Services;
using CreationsForge.ViewModels;
using Shouldly;

namespace CreationsForge.PresentationTests.ViewModels;

/// <summary>Verifies first-use review, persistence admission, recovery, and leave behavior.</summary>
public sealed partial class WorkspaceChangesViewModelTests
{
    /// <summary>Verifies a first leave request publishes staged review before presenting user choices.</summary>
    /// <returns>A task that completes after the scenario assertions.</returns>
    [Fact]
    public async Task ReserveLeaveAsync_FirstStagedRequest_PublishesSaveAndDiscardChoices()
    {
        await using var context = WorkspaceChangesTestContext.CreateReady(hasStagedChanges: true);
        context.DialogService.OnShowAsync = (viewModel, request) =>
        {
            request.LeaveReason.ShouldBe(WorkspaceLeaveReason.CloseWorkspace);
            viewModel.CurrentReview.ShouldNotBeNull();
            viewModel.CurrentReview.HasStagedChanges.ShouldBeTrue();
            viewModel.CanSaveAndProceed.ShouldBeTrue();
            viewModel.CanDiscardAndProceed.ShouldBeTrue();
            return Task.FromResult(WorkspaceChangesDialogResult.KeepEditing);
        };

        var reservation = await context.ViewModel.ReserveLeaveAsync(WorkspaceLeaveReason.CloseWorkspace);

        reservation.ShouldBeNull();
        context.Workspace!.PreviewCount.ShouldBeGreaterThanOrEqualTo(1);
        context.DialogService.Requests.Count.ShouldBe(1);
    }

    /// <summary>Verifies a first leave request publishes an empty review so a local draft can be discarded.</summary>
    /// <returns>A task that completes after the scenario assertions.</returns>
    [Fact]
    public async Task ReserveLeaveAsync_FirstLocalDraftRequest_PublishesDiscardChoice()
    {
        await using var context = WorkspaceChangesTestContext.CreateReady(hasDraftChanges: true);
        context.DialogService.OnShowAsync = (viewModel, _) =>
        {
            viewModel.CurrentReview.ShouldNotBeNull();
            viewModel.CurrentReview.HasStagedChanges.ShouldBeFalse();
            viewModel.CanSaveAndProceed.ShouldBeFalse();
            viewModel.CanDiscardAndProceed.ShouldBeTrue();
            viewModel.StatusText.ShouldBe("Form changes not applied");
            return Task.FromResult(WorkspaceChangesDialogResult.KeepEditing);
        };

        var reservation = await context.ViewModel.ReserveLeaveAsync(WorkspaceLeaveReason.OpenWorkspace);

        reservation.ShouldBeNull();
        context.Workspace!.PreviewCount.ShouldBe(1);
    }

    /// <summary>Verifies a first blocked leave request publishes recovery admission without attempting a preview.</summary>
    /// <returns>A task that completes after the scenario assertions.</returns>
    [Fact]
    public async Task ReserveLeaveAsync_FirstRecoveryRequest_PublishesInspectChoice()
    {
        await using var context = WorkspaceChangesTestContext.CreateReady();
        SetRecoveryRequired(context, context.Workspace!.WorkspaceId, out _);
        context.DialogService.OnShowAsync = (viewModel, _) =>
        {
            viewModel.OutputSynchronizationStatus.ShouldBe(OutputSynchronizationStatus.RecoveryRequired);
            viewModel.CanInspectSaveOutcome.ShouldBeTrue();
            return Task.FromResult(WorkspaceChangesDialogResult.KeepEditing);
        };

        var reservation = await context.ViewModel.ReserveLeaveAsync(WorkspaceLeaveReason.ExitApplication);

        reservation.ShouldBeNull();
        context.Workspace.PreviewCount.ShouldBe(0);
    }

    /// <summary>Verifies pending editor replay remains the only first-leave action and skips workspace capture.</summary>
    /// <returns>A task that completes after the scenario assertions.</returns>
    [Fact]
    public async Task ReserveLeaveAsync_FirstPendingEditRequest_RequiresReturnToEditor()
    {
        await using var context = WorkspaceChangesTestContext.CreateReady(hasStagedChanges: true);
        context.EditParticipant.SetState(
            hasDraftChanges: false,
            hasPendingOperation: true,
            operationState: FormListEditorOperationState.PendingOutcome);
        context.DialogService.OnShowAsync = (viewModel, _) =>
        {
            viewModel.ShowReturnToEditor.ShouldBeTrue();
            viewModel.CanReturnToEditor.ShouldBeTrue();
            viewModel.CanSaveAndProceed.ShouldBeFalse();
            viewModel.CanDiscardAndProceed.ShouldBeFalse();
            return Task.FromResult(WorkspaceChangesDialogResult.KeepEditing);
        };

        var reservation = await context.ViewModel.ReserveLeaveAsync(WorkspaceLeaveReason.ShowSettings);

        reservation.ShouldBeNull();
        context.Workspace!.ReadStateCount.ShouldBe(0);
        context.Workspace.PreviewCount.ShouldBe(0);
    }

    /// <summary>Verifies an original live candidate can resume after restore publishes a new physical baseline identity.</summary>
    /// <returns>A task that completes after the scenario assertions.</returns>
    [Fact]
    public async Task InspectSaveOutcomeAsync_RestoredBaselineWithNewIdentity_AllowsOriginalWorkspaceResume()
    {
        await using var context = WorkspaceChangesTestContext.CreateReady(hasStagedChanges: true);
        SetRecoveryRequired(context, context.Workspace!.WorkspaceId, out var pending);
        var resolvedBaseline = ReplacementBaseline(context.Workspace.State.Output!);
        var evidence = Evidence(pending, resolvedBaseline);
        context.SaveCoordinator.OnRecoverAsync = (_, _) => ValueTask.FromResult(new RecoverSaveResult(
            pending.OriginalWorkspaceId,
            pending.SaveOperationId,
            RecoverSaveStatus.NotCommitted,
            pending.SaveBaseRevision,
            repairRequired: false,
            evidence.EvidenceToken,
            evidence,
            error: null));
        context.DialogService.OnShowAsync = async (viewModel, _) =>
        {
            await viewModel.InspectSaveOutcomeAsync();
            viewModel.CanResumeUnsavedChanges.ShouldBeTrue();
            viewModel.CanReopenResolvedOutput.ShouldBeTrue();
            return WorkspaceChangesDialogResult.KeepEditing;
        };

        var reservation = await context.ViewModel.ReserveLeaveAsync(WorkspaceLeaveReason.CloseWorkspace);

        reservation.ShouldBeNull();
        resolvedBaseline.BaselineId.ShouldNotBe(context.Workspace.State.OutputBaseline!.BaselineId);
        resolvedBaseline.Artifacts[0].Fingerprint.ShouldBe(context.Workspace.State.OutputBaseline.Artifacts[0].Fingerprint);
        resolvedBaseline.Artifacts[0].FileIdentity.ShouldNotBe(context.Workspace.State.OutputBaseline.Artifacts[0].FileIdentity);
    }

    /// <summary>Verifies a fresh workspace cannot resume another workspace's retained staged candidate.</summary>
    /// <returns>A task that completes after the scenario assertions.</returns>
    [Fact]
    public async Task InspectSaveOutcomeAsync_FreshWorkspace_DoesNotAllowResume()
    {
        await using var context = WorkspaceChangesTestContext.CreateReady(hasStagedChanges: true);
        SetRecoveryRequired(context, Guid.NewGuid(), out var pending);
        var evidence = Evidence(pending, ReplacementBaseline(context.Workspace!.State.Output!));
        context.SaveCoordinator.OnRecoverAsync = (_, _) => ValueTask.FromResult(new RecoverSaveResult(
            pending.OriginalWorkspaceId,
            pending.SaveOperationId,
            RecoverSaveStatus.NotCommitted,
            pending.SaveBaseRevision,
            repairRequired: false,
            evidence.EvidenceToken,
            evidence,
            error: null));
        context.DialogService.OnShowAsync = async (viewModel, _) =>
        {
            await viewModel.InspectSaveOutcomeAsync();
            viewModel.CanResumeUnsavedChanges.ShouldBeFalse();
            viewModel.CanReopenResolvedOutput.ShouldBeTrue();
            return WorkspaceChangesDialogResult.KeepEditing;
        };

        var reservation = await context.ViewModel.ReserveLeaveAsync(WorkspaceLeaveReason.CloseWorkspace);

        reservation.ShouldBeNull();
    }

    /// <summary>Verifies a trustworthy nonterminal repair result requires fresh recovery evidence before another repair.</summary>
    /// <returns>A task that completes after the scenario assertions.</returns>
    [Fact]
    public async Task CompletePreparedSaveAsync_NotStarted_RequiresFreshInspectionBeforeNewRepair()
    {
        await using var context = WorkspaceChangesTestContext.CreateReady(hasStagedChanges: true);
        SetRecoveryRequired(context, context.Workspace!.WorkspaceId, out var pending);
        var currentToken = new RecoveryEvidenceToken("first-reviewed-token");
        context.SaveCoordinator.OnRecoverAsync = (_, _) => ValueTask.FromResult(new RecoverSaveResult(
            pending.OriginalWorkspaceId,
            pending.SaveOperationId,
            RecoverSaveStatus.StillUnknown,
            pending.SaveBaseRevision,
            repairRequired: true,
            currentToken,
            resolvedEvidence: null,
            new EngineError(EngineErrorCode.RepairRequired, "Explicit repair is required.")));
        context.SaveCoordinator.OnRepairAsync = (request, _) => ValueTask.FromResult(new RepairSaveResult(
            request.WorkspaceId,
            request.SaveOperationId,
            request.RepairOperationId,
            RepairSaveStatus.NotStarted,
            resultingBaseline: null,
            evidenceToken: null,
            resolvedEvidence: null,
            new EngineError(EngineErrorCode.RepairRequired, "The repair did not start.")));
        context.DialogService.OnShowAsync = async (viewModel, _) =>
        {
            await viewModel.InspectSaveOutcomeAsync();
            viewModel.CanCompletePreparedSave.ShouldBeTrue();
            await viewModel.CompletePreparedSaveAsync();
            viewModel.CanCompletePreparedSave.ShouldBeFalse();

            await viewModel.CompletePreparedSaveAsync();
            context.SaveCoordinator.RepairRequests.Count.ShouldBe(1);

            currentToken = new RecoveryEvidenceToken("second-reviewed-token");
            await viewModel.InspectSaveOutcomeAsync();
            viewModel.CanCompletePreparedSave.ShouldBeTrue();
            await viewModel.CompletePreparedSaveAsync();
            return WorkspaceChangesDialogResult.KeepEditing;
        };

        var reservation = await context.ViewModel.ReserveLeaveAsync(WorkspaceLeaveReason.CloseWorkspace);

        reservation.ShouldBeNull();
        context.SaveCoordinator.RepairRequests.Count.ShouldBe(2);
        context.SaveCoordinator.RepairRequests.ShouldAllBe(request =>
            request.WorkspaceId == pending.OriginalWorkspaceId
            && request.SaveOperationId == pending.SaveOperationId);
        context.SaveCoordinator.RepairRequests[0].RepairOperationId
            .ShouldNotBe(context.SaveCoordinator.RepairRequests[1].RepairOperationId);
        context.SaveCoordinator.RepairRequests[0].EvidenceToken.Value.ShouldBe("first-reviewed-token");
        context.SaveCoordinator.RepairRequests[1].EvidenceToken.Value.ShouldBe("second-reviewed-token");
    }

    /// <summary>Sets a recovery-required state with an exact original pending save identity.</summary>
    internal static void SetRecoveryRequired(
        WorkspaceChangesTestContext context,
        Guid originalWorkspaceId,
        out PendingSaveIdentity pending)
    {
        var workspace = context.Workspace!;
        var state = workspace.State;
        pending = new PendingSaveIdentity(
            originalWorkspaceId,
            Guid.NewGuid(),
            state.Revision,
            state.Game,
            state.Release,
            state.Output!);
        workspace.State = new WorkspaceState(
            state.Game,
            state.Release,
            state.Output,
            CurrentPhysicalBaseline(state.Output!),
            new OutputSynchronizationState(OutputSynchronizationStatus.RecoveryRequired, pending),
            state.Revision);
    }

    /// <summary>Creates a present current baseline with stable logical bytes and its original physical file identity.</summary>
    private static OutputArtifactSetBaseline CurrentPhysicalBaseline(OutputAssociation output)
        => PhysicalBaseline(output, "before-repair");

    /// <summary>Creates a replacement baseline with the same logical bytes and a new physical file identity.</summary>
    internal static OutputArtifactSetBaseline ReplacementBaseline(OutputAssociation output)
        => PhysicalBaseline(output, "after-repair");

    /// <summary>Creates one complete output baseline for a named physical publication.</summary>
    private static OutputArtifactSetBaseline PhysicalBaseline(OutputAssociation output, string fileId)
        => new(
            Guid.NewGuid(),
            [new PluginArtifactAssociation(
                output.PluginPath,
                PluginArtifactRole.Plugin,
                language: null,
                new PluginArtifactFingerprint(true, 42, new string('A', 64)),
                new ArtifactFileIdentity("test", "volume", fileId, 1))]);

    /// <summary>Creates exact terminal not-committed evidence for the supplied original save.</summary>
    internal static ResolvedOutputEvidence Evidence(
        PendingSaveIdentity pending,
        OutputArtifactSetBaseline resolvedBaseline)
        => new(
            new RecoveryEvidenceToken(Guid.NewGuid().ToString("N")),
            pending.Game,
            pending.Release,
            pending.OriginalWorkspaceId,
            pending.SaveOperationId,
            pending.SaveBaseRevision,
            new PluginSourceInputBaseline(Guid.NewGuid(), []),
            pending.Output,
            resolvedBaseline,
            RecoverSaveStatus.NotCommitted);
}
