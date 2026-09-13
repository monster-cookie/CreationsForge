using CreationsForge.Bootstrap.Composition;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.Persistence;
using CreationsForge.Core.Enums;
using Mutagen.Bethesda.Plugins;
using Shouldly;

namespace CreationsForge.UnitTests.Engine.Integration;

/// <content>Verifies already-open owners respect recognized unresolved and physically terminal prior-save journals.</content>
public sealed partial class WorkspaceCrossOwnerConflictTests
{
    /// <summary>Verifies an already-open owner cannot mutate a genuinely mixed prior transaction and both owners retain exact recovery state.</summary>
    /// <returns>A task that completes after evidence-bound repair, live adoption, and fresh B reopen.</returns>
    [Fact]
    public async Task SaveAsync_AlreadyOpenOwnerAgainstMixedPriorTransaction_RequiresRepairWithoutMutation()
    {
        using var fixture = WorkspaceIntegrationFixture.Create(SupportedGame.Skyrim);
        var sourceArtifacts = fixture.SnapshotArtifacts();
        var output = WorkspaceRecoveryFixture.CreateOutputAssociation(
            fixture,
            "MixedCrossOwnerOutput",
            "MixedCrossOwner.esm");
        await using var servicesB = EngineComposition.Create();
        OutputOwnerSession? ownerB = null;
        try
        {
            await using var seed = await WorkspaceRecoveryFixture.CreateAsync(
                fixture,
                output,
                WorkspaceRecoveryPhysicalState.MutationStartedMixed,
                async (setup, _) =>
                {
                    ownerB = await OpenStagedNewOutputOwnerAsync(
                        servicesB,
                        fixture,
                        setup.Output,
                        setup.FormKey,
                        Guid.NewGuid(),
                        "CfMixedOwnerB");
                    ownerB.State.OutputSynchronization.Status.ShouldBe(OutputSynchronizationStatus.Ready);
                    ownerB.State.OutputBaseline.ShouldNotBeNull().BaselineId.ShouldBe(setup.BeforeBaseline.BaselineId);
                    AssertPreviewEditorId(ownerB.Preview, setup.FormKey, "CfMixedOwnerB");
                },
                TestContext.Current.CancellationToken);
            var owner = ownerB.ShouldNotBeNull();
            var originalWorkspace = seed.OriginalWorkspace.ShouldNotBeNull();
            var originalSave = seed.OriginalSaveResult.ShouldNotBeNull();
            originalSave.Status.ShouldBe(SaveCommitStatus.CommitOutcomeUnknown);
            originalSave.WorkspaceId.ShouldBe(seed.OriginalWorkspaceId);
            originalSave.OperationId.ShouldBe(seed.SaveOperationId);
            originalSave.BaseRevision.ShouldBe(seed.SaveBaseRevision);
            seed.OriginalSynchronization.ShouldNotBeNull().Status.ShouldBe(OutputSynchronizationStatus.RecoveryRequired);
            seed.OriginalSynchronization.PendingSave.ShouldNotBeNull().OriginalWorkspaceId.ShouldBe(seed.OriginalWorkspaceId);
            seed.OriginalSynchronization.PendingSave.SaveOperationId.ShouldBe(seed.SaveOperationId);
            originalWorkspace.WorkspaceId.ShouldBe(seed.OriginalWorkspaceId);
            originalWorkspace.OutputSynchronization.Status.ShouldBe(OutputSynchronizationStatus.RecoveryRequired);
            originalWorkspace.OutputSynchronization.PendingSave.ShouldNotBeNull().SaveOperationId.ShouldBe(seed.SaveOperationId);
            AssertPreviewEditorId(seed.OriginalPreview.ShouldNotBeNull(), seed.FormKey, seed.PreparedEditorId.ShouldNotBeNull());

            var recoveryCoordinator = new WorkspaceSaveCoordinator(new OutputDirectoryLeaseProvider());
            var recoveryRequest = new RecoverSaveRequest(
                seed.OriginalWorkspaceId,
                seed.SaveOperationId,
                seed.Output);
            var recoveryBeforeB = await recoveryCoordinator.RecoverAsync(
                recoveryRequest,
                TestContext.Current.CancellationToken);
            AssertMixedRecovery(seed, recoveryBeforeB);
            recoveryBeforeB.EvidenceToken.ShouldBe(originalSave.RecoveryEvidenceToken);
            var physicalBeforeB = seed.SnapshotPhysicalArtifacts();
            var ownerBRevision = owner.Workspace.Revision;
            var ownerBState = await ReadStateAsync(owner.Workspace);
            var ownerBPreview = await ReadPreviewAsync(owner.Workspace);
            var ownerBSaveOperationId = Guid.NewGuid();

            var ownerBSave = await owner.Workspace.SaveAsync(
                new SaveRequest(ownerBSaveOperationId, ownerBRevision, owner.Selection.Baseline),
                TestContext.Current.CancellationToken);

            ownerBSave.Status.ShouldBe(SaveCommitStatus.NotCommitted);
            ownerBSave.WorkspaceId.ShouldBe(owner.WorkspaceId);
            ownerBSave.OperationId.ShouldBe(ownerBSaveOperationId);
            ownerBSave.BaseRevision.ShouldBe(ownerBRevision);
            ownerBSave.ResultRevision.ShouldBe(ownerBRevision);
            ownerBSave.Error.ShouldNotBeNull().Code.ShouldBe(EngineErrorCode.RepairRequired);
            ownerBSave.CommittedBaseline.ShouldBeNull();
            owner.Workspace.Revision.ShouldBe(ownerBRevision);
            owner.Workspace.OutputSynchronization.Status.ShouldBe(OutputSynchronizationStatus.Ready);
            var ownerBAfter = await ReadStateAsync(owner.Workspace);
            ownerBAfter.OutputBaseline.ShouldNotBeNull().BaselineId
                .ShouldBe(ownerBState.OutputBaseline.ShouldNotBeNull().BaselineId);
            AssertPreviewEditorId(ownerBPreview, seed.FormKey, "CfMixedOwnerB");
            AssertPreviewEditorId(await ReadPreviewAsync(owner.Workspace), seed.FormKey, "CfMixedOwnerB");
            AssertPhysicalArtifactsUnchanged(physicalBeforeB, seed.SnapshotPhysicalArtifacts());
            Directory.Exists(new SaveTransactionPaths(
                Path.GetDirectoryName(seed.Output.PluginPath)!,
                owner.WorkspaceId,
                ownerBSaveOperationId).TransactionDirectoryPath).ShouldBeFalse();
            var originalStateAfterB = await ReadStateAsync(originalWorkspace);
            originalStateAfterB.OutputSynchronization.Status.ShouldBe(OutputSynchronizationStatus.RecoveryRequired);
            originalStateAfterB.OutputSynchronization.PendingSave.ShouldNotBeNull().SaveOperationId.ShouldBe(seed.SaveOperationId);
            originalStateAfterB.Revision.ShouldBe(seed.SaveBaseRevision);
            originalStateAfterB.OutputBaseline.ShouldNotBeNull().BaselineId.ShouldBe(seed.BeforeBaseline.BaselineId);
            var blockedOriginalPreview = await originalWorkspace.PreviewAsync(TestContext.Current.CancellationToken);
            blockedOriginalPreview.Succeeded.ShouldBeFalse();
            blockedOriginalPreview.Error.ShouldNotBeNull().Code.ShouldBe(EngineErrorCode.RepairRequired);
            AssertPreviewEditorId(seed.OriginalPreview, seed.FormKey, seed.PreparedEditorId.ShouldNotBeNull());

            var recoveryAfterB = await recoveryCoordinator.RecoverAsync(
                recoveryRequest,
                TestContext.Current.CancellationToken);
            AssertMixedRecovery(seed, recoveryAfterB);
            recoveryAfterB.EvidenceToken.ShouldBe(recoveryBeforeB.EvidenceToken);
            var repairOperationId = Guid.NewGuid();
            var repair = await recoveryCoordinator.RepairAsync(
                new RepairSaveRequest(
                    seed.OriginalWorkspaceId,
                    seed.SaveOperationId,
                    repairOperationId,
                    seed.SaveBaseRevision,
                    seed.Output,
                    recoveryAfterB.EvidenceToken.ShouldNotBeNull(),
                    RepairSaveDirection.CompletePrepared),
                TestContext.Current.CancellationToken);
            repair.Status.ShouldBe(RepairSaveStatus.PreparedSetCompleted, DescribeError(repair.Error));
            repair.RepairOperationId.ShouldBe(repairOperationId);
            repair.ResultingBaseline.ShouldNotBeNull();
            repair.ResolvedEvidence.ShouldNotBeNull();
            var terminal = await recoveryCoordinator.RecoverAsync(
                recoveryRequest,
                TestContext.Current.CancellationToken);
            terminal.Status.ShouldBe(RecoverSaveStatus.Committed);
            terminal.RepairRequired.ShouldBeFalse();
            terminal.ResolvedEvidence.ShouldNotBeNull().EvidenceToken.ShouldBe(repair.ResolvedEvidence.EvidenceToken);

            var adoptionOperationId = Guid.NewGuid();
            var adopted = await originalWorkspace.ResolveOutputRecoveryAsync(
                new ResolveOutputRecoveryRequest(
                    adoptionOperationId,
                    originalWorkspace.Revision,
                    OutputRecoveryAdoptionMode.ReopenResolvedOutput,
                    terminal.ResolvedEvidence),
                TestContext.Current.CancellationToken);
            adopted.Succeeded.ShouldBeTrue(DescribeError(adopted.Error));
            adopted.OperationId.ShouldBe(adoptionOperationId);
            originalWorkspace.OutputSynchronization.Status.ShouldBe(OutputSynchronizationStatus.Ready);
            (await ReadEditorIdAsync(originalWorkspace, seed.Output, seed.FormKey))
                .ShouldBe(seed.PreparedEditorId);

            await owner.DisposeAsync();
            var freshB = await servicesB.WorkspaceFactory.OpenAsync(
                fixture.CreateOpenRequest(Guid.NewGuid()),
                TestContext.Current.CancellationToken);
            freshB.Succeeded.ShouldBeTrue(DescribeError(freshB.Error));
            await using (var freshWorkspace = freshB.Value.ShouldNotBeNull())
            {
                var selected = await SelectOutputAsync(freshWorkspace, seed.Output, OutputSelectionMode.OpenExisting);
                selected.Baseline.BaselineId.ShouldBe(terminal.ResolvedEvidence.ResolvedOutputBaseline.BaselineId);
                (await ReadEditorIdAsync(freshWorkspace, seed.Output, seed.FormKey))
                    .ShouldBe(seed.PreparedEditorId);
            }

            AssertSourceArtifactsUnchanged(sourceArtifacts, fixture.SnapshotArtifacts());
        }
        finally
        {
            if (ownerB is not null)
            {
                await ownerB.DisposeAsync();
            }
        }
    }

    /// <summary>Verifies physically terminal prior journals auto-finalize before an already-open owner's normal save admission.</summary>
    /// <param name="state">Whether the original or prepared complete output is physically present.</param>
    /// <returns>A task that completes after the normal save result and terminal recovery classification are verified.</returns>
    [Theory]
    [InlineData(WorkspaceRecoveryPhysicalState.PreparingAllBefore)]
    [InlineData(WorkspaceRecoveryPhysicalState.PreparedAllPrepared)]
    public async Task SaveAsync_AlreadyOpenOwnerAgainstTerminalPriorTransaction_AutoFinalizesBeforeAdmission(
        WorkspaceRecoveryPhysicalState state)
    {
        using var fixture = WorkspaceIntegrationFixture.Create(SupportedGame.Starfield);
        var sourceArtifacts = fixture.SnapshotArtifacts();
        var output = WorkspaceRecoveryFixture.CreateEmbeddedOutputAssociation(
            fixture,
            $"TerminalCrossOwner{state}",
            "TerminalCrossOwner.esm");
        await using var servicesB = EngineComposition.Create();
        OutputOwnerSession? ownerB = null;
        try
        {
            await using var seed = await WorkspaceRecoveryFixture.CreateAsync(
                fixture,
                output,
                state,
                async (setup, _) =>
                {
                    ownerB = await OpenStagedOwnerAsync(
                        servicesB,
                        fixture,
                        setup.Output,
                        setup.FormKey,
                        Guid.NewGuid(),
                        $"CfTerminalOwnerB{state}");
                    ownerB.State.OutputSynchronization.Status.ShouldBe(OutputSynchronizationStatus.Ready);
                    ownerB.Selection.Baseline.BaselineId.ShouldBe(setup.BeforeBaseline.BaselineId);
                },
                TestContext.Current.CancellationToken);
            var owner = ownerB.ShouldNotBeNull();
            var retainedRevision = owner.Workspace.Revision;
            var saveOperationId = Guid.NewGuid();

            var save = await owner.Workspace.SaveAsync(
                new SaveRequest(saveOperationId, retainedRevision, owner.Selection.Baseline),
                TestContext.Current.CancellationToken);

            var recovery = await new WorkspaceSaveCoordinator(new OutputDirectoryLeaseProvider()).RecoverAsync(
                new RecoverSaveRequest(seed.OriginalWorkspaceId, seed.SaveOperationId, seed.Output),
                TestContext.Current.CancellationToken);
            recovery.Status.ShouldBe(seed.ExpectedRecoveryStatus);
            recovery.RepairRequired.ShouldBeFalse();
            if (state == WorkspaceRecoveryPhysicalState.PreparingAllBefore)
            {
                save.Status.ShouldBe(SaveCommitStatus.Committed, DescribeError(save.Error));
                save.CommittedBaseline.ShouldNotBeNull();
                owner.Workspace.OutputSynchronization.Status.ShouldBe(OutputSynchronizationStatus.Ready);
                (await ReadEditorIdAsync(owner.Workspace, seed.Output, seed.FormKey))
                    .ShouldBe($"CfTerminalOwnerB{state}");
            }
            else
            {
                save.Status.ShouldBe(SaveCommitStatus.NotCommitted);
                save.Error.ShouldNotBeNull().Code.ShouldBe(EngineErrorCode.ExternalChangeDetected);
                save.CommittedBaseline.ShouldBeNull();
                owner.Workspace.Revision.ShouldBe(retainedRevision);
                owner.Workspace.OutputSynchronization.Status.ShouldBe(OutputSynchronizationStatus.Ready);
                AssertPreviewEditorId(
                    await ReadPreviewAsync(owner.Workspace),
                    seed.FormKey,
                    $"CfTerminalOwnerB{state}");
                await using var verificationServices = EngineComposition.Create();
                var verified = await verificationServices.WorkspaceFactory.OpenAsync(
                    fixture.CreateOpenRequest(Guid.NewGuid()),
                    TestContext.Current.CancellationToken);
                verified.Succeeded.ShouldBeTrue(DescribeError(verified.Error));
                await using var verificationWorkspace = verified.Value.ShouldNotBeNull();
                await SelectOutputAsync(verificationWorkspace, seed.Output, OutputSelectionMode.OpenExisting);
                (await ReadEditorIdAsync(verificationWorkspace, seed.Output, seed.FormKey))
                    .ShouldBe(seed.PreparedEditorId);
            }

            AssertSourceArtifactsUnchanged(sourceArtifacts, fixture.SnapshotArtifacts());
        }
        finally
        {
            if (ownerB is not null)
            {
                await ownerB.DisposeAsync();
            }
        }
    }

    /// <summary>Opens and stages one independently owned new output while its shared destination remains absent.</summary>
    /// <param name="services">The independently owned production service lifetime.</param>
    /// <param name="fixture">The generated plugin source fixture.</param>
    /// <param name="output">The absent shared output association.</param>
    /// <param name="formKey">The deterministic FormList identity allocated by owner A.</param>
    /// <param name="workspaceId">The caller-selected owner B identity.</param>
    /// <param name="editorId">The owner-specific staged EditorID.</param>
    /// <returns>The live independently disposable owner session with an absent selected baseline.</returns>
    private static async Task<OutputOwnerSession> OpenStagedNewOutputOwnerAsync(
        EngineServices services,
        WorkspaceIntegrationFixture fixture,
        OutputAssociation output,
        FormKey formKey,
        Guid workspaceId,
        string editorId)
    {
        var opened = await services.WorkspaceFactory.OpenAsync(
            fixture.CreateOpenRequest(workspaceId),
            TestContext.Current.CancellationToken);
        opened.Succeeded.ShouldBeTrue(DescribeError(opened.Error));
        var workspace = opened.Value.ShouldNotBeNull();
        try
        {
            workspace.WorkspaceId.ShouldBe(workspaceId);
            var selectOperationId = Guid.NewGuid();
            var selected = await workspace.SelectOutputAsync(
                new SelectOutputRequest(
                    selectOperationId,
                    workspace.Revision,
                    OutputSelectionMode.CreateNew,
                    output),
                TestContext.Current.CancellationToken);
            selected.Succeeded.ShouldBeTrue(DescribeError(selected.Error));
            selected.OperationId.ShouldBe(selectOperationId);
            var selection = selected.Value.ShouldNotBeNull();
            selection.Baseline.Artifacts.All(artifact => !artifact.Fingerprint.Exists).ShouldBeTrue();
            var beginOperationId = Guid.NewGuid();
            var begun = await workspace.BeginEditAsync(
                new BeginEditRequest(
                    beginOperationId,
                    workspace.Revision,
                    FormListEditRole.New),
                TestContext.Current.CancellationToken);
            begun.Succeeded.ShouldBeTrue(DescribeError(begun.Error));
            begun.OperationId.ShouldBe(beginOperationId);
            var edit = begun.Value.ShouldNotBeNull();
            edit.FormKey.ShouldBe(formKey);
            var applyOperationId = Guid.NewGuid();
            var applied = await workspace.ApplyFormListEditAsync(
                new FormListEditRequest(
                    applyOperationId,
                    workspace.Revision,
                    edit.EditId,
                    new SetEditorIdEdit(editorId)),
                TestContext.Current.CancellationToken);
            applied.Succeeded.ShouldBeTrue(DescribeError(applied.Error));
            applied.OperationId.ShouldBe(applyOperationId);
            var state = await ReadStateAsync(workspace);
            var preview = await ReadPreviewAsync(workspace);
            AssertPreviewEditorId(preview, formKey, editorId);
            return new OutputOwnerSession(
                workspace,
                selection,
                edit.EditId,
                editorId,
                selectOperationId,
                beginOperationId,
                applyOperationId,
                Guid.NewGuid(),
                state,
                preview);
        }
        catch
        {
            await workspace.DisposeAsync();
            throw;
        }
    }

    /// <summary>Asserts one mixed recovery result retains exact prior-save identity, revision, and repair requirement.</summary>
    /// <param name="seed">The expected mixed recovery seed.</param>
    /// <param name="recovery">The read-only recovery result.</param>
    private static void AssertMixedRecovery(
        WorkspaceRecoverySeed seed,
        RecoverSaveResult recovery)
    {
        recovery.WorkspaceId.ShouldBe(seed.OriginalWorkspaceId);
        recovery.SaveOperationId.ShouldBe(seed.SaveOperationId);
        recovery.SaveBaseRevision.ShouldBe(seed.SaveBaseRevision);
        recovery.Status.ShouldBe(seed.ExpectedRecoveryStatus);
        recovery.RepairRequired.ShouldBeTrue();
        recovery.Error.ShouldNotBeNull().Code.ShouldBe(seed.ExpectedRecoveryErrorCode.ShouldNotBeNull());
        recovery.EvidenceToken.ShouldNotBeNull();
        recovery.ResolvedEvidence.ShouldBeNull();
    }
}
