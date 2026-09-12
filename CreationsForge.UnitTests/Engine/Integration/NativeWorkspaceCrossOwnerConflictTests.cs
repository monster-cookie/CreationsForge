using CreationsForge.Bootstrap.Composition;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Enums;
using Shouldly;

namespace CreationsForge.UnitTests.Engine.Integration;

/// <summary>Verifies independent native workspace owners cannot overwrite shared output through stale local authority.</summary>
public sealed partial class NativeWorkspaceCrossOwnerConflictTests
{
    /// <summary>Verifies the first of two independent owners commits and the stale owner must explicitly reopen before saving.</summary>
    /// <param name="game">The native game exercised through separate production service graphs.</param>
    /// <param name="ownerAFirst">Whether owner A or owner B saves first.</param>
    /// <returns>A task that completes after both owner orders and a third fresh disk read are verified.</returns>
    [Theory]
    [InlineData(SupportedGame.Starfield, true)]
    [InlineData(SupportedGame.Starfield, false)]
    [InlineData(SupportedGame.Fallout4, true)]
    [InlineData(SupportedGame.Fallout4, false)]
    [InlineData(SupportedGame.Skyrim, true)]
    [InlineData(SupportedGame.Skyrim, false)]
    public async Task SaveAsync_TwoIndependentOwners_FirstWriterWinsAndStaleOwnerMustReopen(
        SupportedGame game,
        bool ownerAFirst)
    {
        using var fixture = NativeWorkspaceIntegrationFixture.Create(game);
        var sourceArtifacts = fixture.SnapshotArtifacts();
        var output = CreateEmbeddedOutputAssociation(fixture);
        var initial = await CreateExistingOutputAsync(fixture, output, $"CfInitial{game}");
        var initialOutput = SnapshotOutputArtifacts(initial.Baseline);
        await using var servicesA = NativeEngineComposition.Create();
        await using var servicesB = NativeEngineComposition.Create();
        var workspaceAId = Guid.NewGuid();
        var workspaceBId = Guid.NewGuid();
        await using var ownerA = await OpenStagedOwnerAsync(
            servicesA,
            fixture,
            output,
            initial.FormKey,
            workspaceAId,
            $"CfUiFirst{game}");
        await using var ownerB = await OpenStagedOwnerAsync(
            servicesB,
            fixture,
            output,
            initial.FormKey,
            workspaceBId,
            $"CfMcpFirst{game}");

        ReferenceEquals(ownerA.Workspace, ownerB.Workspace).ShouldBeFalse();
        ownerA.Workspace.WorkspaceId.ShouldNotBe(ownerB.Workspace.WorkspaceId);
        ownerA.EditId.ShouldNotBe(ownerB.EditId);
        ReferenceEquals(ownerA.Preview, ownerB.Preview).ShouldBeFalse();
        ReferenceEquals(ownerA.Selection.Baseline, ownerB.Selection.Baseline).ShouldBeFalse();
        new[]
        {
            ownerA.SelectOperationId,
            ownerA.BeginOperationId,
            ownerA.ApplyOperationId,
            ownerA.SaveOperationId,
            ownerB.SelectOperationId,
            ownerB.BeginOperationId,
            ownerB.ApplyOperationId,
            ownerB.SaveOperationId,
        }.Distinct().Count().ShouldBe(8);
        ownerA.State.OutputSynchronization.Status.ShouldBe(OutputSynchronizationStatus.Ready);
        ownerB.State.OutputSynchronization.Status.ShouldBe(OutputSynchronizationStatus.Ready);
        ownerA.State.Revision.ShouldBe(ownerB.State.Revision);
        ownerA.State.Revision.BaselineId.ShouldBe(ownerB.State.Revision.BaselineId);
        ownerA.State.Revision.Sequence.ShouldBe(ownerB.State.Revision.Sequence);
        ownerA.State.OutputBaseline.ShouldNotBeNull().BaselineId.ShouldBe(ownerA.Selection.Baseline.BaselineId);
        ownerB.State.OutputBaseline.ShouldNotBeNull().BaselineId.ShouldBe(ownerB.Selection.Baseline.BaselineId);
        AssertBaselineArtifactsEqual(ownerA.Selection.Baseline, ownerB.Selection.Baseline);
        AssertPhysicalArtifactsUnchanged(initialOutput, SnapshotOutputArtifacts(initial.Baseline));

        var winner = ownerAFirst ? ownerA : ownerB;
        var loser = ownerAFirst ? ownerB : ownerA;
        var winnerRequest = new SaveRequest(
            winner.SaveOperationId,
            winner.State.Revision,
            winner.Selection.Baseline);
        var winnerSave = await winner.Workspace.SaveAsync(
            winnerRequest,
            TestContext.Current.CancellationToken);
        winnerSave.Status.ShouldBe(SaveCommitStatus.Committed, DescribeError(winnerSave.Error));
        winnerSave.WorkspaceId.ShouldBe(winner.Workspace.WorkspaceId);
        winnerSave.OperationId.ShouldBe(winner.SaveOperationId);
        winnerSave.BaseRevision.ShouldBe(winner.State.Revision);
        winnerSave.CommittedBaseline.ShouldNotBeNull();
        winner.Workspace.OutputSynchronization.Status.ShouldBe(OutputSynchronizationStatus.Ready);
        (await ReadEditorIdAsync(winner.Workspace, output, initial.FormKey)).ShouldBe(winner.EditorId);
        var winnerOutput = SnapshotOutputArtifacts(winnerSave.CommittedBaseline);
        AssertSourceArtifactsUnchanged(sourceArtifacts, fixture.SnapshotArtifacts());

        var lateWinnerEditorId = $"CfWinnerPending{game}";
        await StageExistingEditAsync(winner.Workspace, initial.FormKey, lateWinnerEditorId);
        var lateWinnerState = await ReadStateAsync(winner.Workspace);
        var lateWinnerPreview = await ReadPreviewAsync(winner.Workspace);
        AssertPreviewEditorId(lateWinnerPreview, initial.FormKey, lateWinnerEditorId);

        var loserRevision = loser.Workspace.Revision;
        var loserRequest = new SaveRequest(
            loser.SaveOperationId,
            loserRevision,
            loser.Selection.Baseline);
        var loserSave = await loser.Workspace.SaveAsync(
            loserRequest,
            TestContext.Current.CancellationToken);
        AssertDefinitiveSaveFailure(loserSave, loser, EngineErrorCode.ExternalChangeDetected, loserRevision);
        loser.Workspace.OutputSynchronization.Status.ShouldBe(OutputSynchronizationStatus.Ready);
        AssertPhysicalArtifactsUnchanged(winnerOutput, SnapshotOutputArtifacts(winnerSave.CommittedBaseline));
        AssertPreviewEditorId(await ReadPreviewAsync(loser.Workspace), initial.FormKey, loser.EditorId);

        var exactReplay = await loser.Workspace.SaveAsync(
            loserRequest,
            TestContext.Current.CancellationToken);
        AssertDefinitiveSaveFailure(exactReplay, loser, EngineErrorCode.ExternalChangeDetected, loserRevision);
        exactReplay.ResultRevision.ShouldBe(loserSave.ResultRevision);
        AssertPhysicalArtifactsUnchanged(winnerOutput, SnapshotOutputArtifacts(winnerSave.CommittedBaseline));
        var newStaleOperationId = Guid.NewGuid();
        var newStaleAttempt = await loser.Workspace.SaveAsync(
            new SaveRequest(newStaleOperationId, loserRevision, loser.Selection.Baseline),
            TestContext.Current.CancellationToken);
        newStaleAttempt.Status.ShouldBe(SaveCommitStatus.NotCommitted);
        newStaleAttempt.OperationId.ShouldBe(newStaleOperationId);
        newStaleAttempt.Error.ShouldNotBeNull().Code.ShouldBe(EngineErrorCode.ExternalChangeDetected);
        newStaleAttempt.CommittedBaseline.ShouldBeNull();
        AssertPhysicalArtifactsUnchanged(winnerOutput, SnapshotOutputArtifacts(winnerSave.CommittedBaseline));

        var loserIntendedEditorId = loser.EditorId;
        await loser.DisposeAsync();
        await using var reopenedLoser = await OpenStagedOwnerAsync(
            ownerAFirst ? servicesB : servicesA,
            fixture,
            output,
            initial.FormKey,
            Guid.NewGuid(),
            loserIntendedEditorId);
        reopenedLoser.Workspace.WorkspaceId.ShouldNotBe(loser.WorkspaceId);
        reopenedLoser.Selection.Baseline.BaselineId.ShouldBe(winnerSave.CommittedBaseline.BaselineId);
        var reopenedLoserSave = await reopenedLoser.Workspace.SaveAsync(
            new SaveRequest(
                reopenedLoser.SaveOperationId,
                reopenedLoser.Workspace.Revision,
                reopenedLoser.Selection.Baseline),
            TestContext.Current.CancellationToken);
        reopenedLoserSave.Status.ShouldBe(SaveCommitStatus.Committed, DescribeError(reopenedLoserSave.Error));
        var finalBaseline = reopenedLoserSave.CommittedBaseline.ShouldNotBeNull();
        var finalOutput = SnapshotOutputArtifacts(finalBaseline);
        (await ReadEditorIdAsync(reopenedLoser.Workspace, output, initial.FormKey)).ShouldBe(loserIntendedEditorId);

        var staleWinnerOperationId = Guid.NewGuid();
        var staleWinnerSave = await winner.Workspace.SaveAsync(
            new SaveRequest(
                staleWinnerOperationId,
                lateWinnerState.Revision,
                lateWinnerState.OutputBaseline.ShouldNotBeNull()),
            TestContext.Current.CancellationToken);
        staleWinnerSave.Status.ShouldBe(SaveCommitStatus.NotCommitted);
        staleWinnerSave.OperationId.ShouldBe(staleWinnerOperationId);
        staleWinnerSave.Error.ShouldNotBeNull().Code.ShouldBe(EngineErrorCode.ExternalChangeDetected);
        winner.Workspace.Revision.ShouldBe(lateWinnerState.Revision);
        winner.Workspace.OutputSynchronization.Status.ShouldBe(OutputSynchronizationStatus.Ready);
        AssertPreviewEditorId(await ReadPreviewAsync(winner.Workspace), initial.FormKey, lateWinnerEditorId);
        AssertPhysicalArtifactsUnchanged(
            finalOutput,
            SnapshotOutputArtifacts(finalBaseline));

        await using var verificationServices = NativeEngineComposition.Create();
        var verifiedOpen = await verificationServices.WorkspaceFactory.OpenAsync(
            fixture.CreateOpenRequest(Guid.NewGuid()),
            TestContext.Current.CancellationToken);
        verifiedOpen.Succeeded.ShouldBeTrue(DescribeError(verifiedOpen.Error));
        await using (var verificationWorkspace = verifiedOpen.Value.ShouldNotBeNull())
        {
            await SelectOutputAsync(verificationWorkspace, output, OutputSelectionMode.OpenExisting);
            (await ReadEditorIdAsync(verificationWorkspace, output, initial.FormKey))
                .ShouldBe(loserIntendedEditorId);
        }

        AssertSourceArtifactsUnchanged(sourceArtifacts, fixture.SnapshotArtifacts());
    }

    /// <summary>Asserts a definitive stale-owner save failure retains local state and exact identities.</summary>
    /// <param name="save">The guarded save result.</param>
    /// <param name="owner">The staged owner whose request was rejected.</param>
    /// <param name="errorCode">The expected typed rejection.</param>
    /// <param name="expectedRevision">The unchanged local revision.</param>
    private static void AssertDefinitiveSaveFailure(
        SaveResult save,
        OutputOwnerSession owner,
        EngineErrorCode errorCode,
        WorkspaceRevision expectedRevision)
    {
        save.Status.ShouldBe(SaveCommitStatus.NotCommitted);
        save.WorkspaceId.ShouldBe(owner.WorkspaceId);
        save.OperationId.ShouldBe(owner.SaveOperationId);
        save.BaseRevision.ShouldBe(expectedRevision);
        save.ResultRevision.ShouldBe(expectedRevision);
        save.Error.ShouldNotBeNull().Code.ShouldBe(errorCode);
        save.CommittedBaseline.ShouldBeNull();
        owner.Workspace.Revision.ShouldBe(expectedRevision);
    }
}
