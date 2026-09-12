using CreationsForge.Bootstrap.Composition;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.Persistence;
using CreationsForge.Core.Enums;
using Shouldly;

namespace CreationsForge.UnitTests.Engine.Integration;

/// <content>Verifies focused revision, physical contention, external-drift, and owner-disposal boundaries.</content>
public sealed partial class NativeWorkspaceCrossOwnerConflictTests
{
    /// <summary>Verifies a cached revision from before a later local Apply cannot authorize a save.</summary>
    /// <returns>A task that completes after the current staged preview and unchanged destination are verified.</returns>
    [Fact]
    public async Task SaveAsync_OldLocalRevision_IsRejectedWithoutLosingLatestPreview()
    {
        using var fixture = NativeWorkspaceIntegrationFixture.Create(SupportedGame.Starfield);
        var sourceArtifacts = fixture.SnapshotArtifacts();
        var output = CreateEmbeddedOutputAssociation(fixture);
        var initial = await CreateExistingOutputAsync(fixture, output, "CfInitialRevision");
        await using var services = NativeEngineComposition.Create();
        await using var owner = await OpenStagedOwnerAsync(
            services,
            fixture,
            output,
            initial.FormKey,
            Guid.NewGuid(),
            "CfOldRevision");
        var oldRevision = owner.Workspace.Revision;
        var oldPreview = await ReadPreviewAsync(owner.Workspace);
        AssertPreviewEditorId(oldPreview, initial.FormKey, "CfOldRevision");
        var destinationBefore = SnapshotOutputArtifacts(initial.Baseline);

        var applied = await owner.Workspace.ApplyFormListEditAsync(
            new FormListEditRequest(
                Guid.NewGuid(),
                owner.Workspace.Revision,
                owner.EditId,
                new SetEditorIdEdit("CfLatestRevision")),
            TestContext.Current.CancellationToken);
        applied.Succeeded.ShouldBeTrue(DescribeError(applied.Error));
        owner.Workspace.Revision.ShouldNotBe(oldRevision);
        var saveOperationId = Guid.NewGuid();
        var save = await owner.Workspace.SaveAsync(
            new SaveRequest(saveOperationId, oldRevision, owner.Selection.Baseline),
            TestContext.Current.CancellationToken);

        save.Status.ShouldBe(SaveCommitStatus.NotCommitted);
        save.OperationId.ShouldBe(saveOperationId);
        save.Error.ShouldNotBeNull().Code.ShouldBe(EngineErrorCode.RevisionConflict);
        save.ResultRevision.ShouldBe(owner.Workspace.Revision);
        save.CommittedBaseline.ShouldBeNull();
        Directory.Exists(new SaveTransactionPaths(
            Path.GetDirectoryName(output.PluginPath)!,
            owner.WorkspaceId,
            saveOperationId).TransactionDirectoryPath).ShouldBeFalse();
        AssertPhysicalArtifactsUnchanged(destinationBefore, SnapshotOutputArtifacts(initial.Baseline));
        AssertPreviewEditorId(await ReadPreviewAsync(owner.Workspace), initial.FormKey, "CfLatestRevision");
        AssertSourceArtifactsUnchanged(sourceArtifacts, fixture.SnapshotArtifacts());
    }

    /// <summary>Verifies simulated external guard-file contention returns a typed busy result and permits a fresh retry after release.</summary>
    /// <returns>A task that completes after the retry commits and native state reopens.</returns>
    [Fact]
    public async Task SaveAsync_ExternalGuardFileContention_PreservesStagedStateAndRetryCommits()
    {
        using var fixture = NativeWorkspaceIntegrationFixture.Create(SupportedGame.Fallout4);
        var sourceArtifacts = fixture.SnapshotArtifacts();
        var output = CreateEmbeddedOutputAssociation(fixture);
        var initial = await CreateExistingOutputAsync(fixture, output, "CfInitialBusy");
        await using var services = NativeEngineComposition.Create();
        await using var owner = await OpenStagedOwnerAsync(
            services,
            fixture,
            output,
            initial.FormKey,
            Guid.NewGuid(),
            "CfBusyPending");
        var revision = owner.Workspace.Revision;
        var destinationBefore = SnapshotOutputArtifacts(initial.Baseline);
        var outputDirectory = Path.GetDirectoryName(output.PluginPath)!;
        var guardPath = Path.Combine(outputDirectory, ".creationsforge-output.lock");
        await using (var simulatedExternalOwner = new FileStream(
            guardPath,
            FileMode.Open,
            FileAccess.ReadWrite,
            FileShare.None,
            bufferSize: 1,
            FileOptions.Asynchronous))
        {
            var save = await owner.Workspace.SaveAsync(
                new SaveRequest(Guid.NewGuid(), revision, owner.Selection.Baseline),
                TestContext.Current.CancellationToken);

            save.Status.ShouldBe(SaveCommitStatus.NotCommitted);
            save.Error.ShouldNotBeNull().Code.ShouldBe(EngineErrorCode.OutputDirectoryBusy);
            save.CommittedBaseline.ShouldBeNull();
            owner.Workspace.Revision.ShouldBe(revision);
            AssertPhysicalArtifactsUnchanged(destinationBefore, SnapshotOutputArtifacts(initial.Baseline));
            AssertPreviewEditorId(await ReadPreviewAsync(owner.Workspace), initial.FormKey, "CfBusyPending");
        }

        var freshState = await ReadStateAsync(owner.Workspace);
        var freshPreview = await ReadPreviewAsync(owner.Workspace);
        freshState.Revision.ShouldBe(revision);
        AssertPreviewEditorId(freshPreview, initial.FormKey, "CfBusyPending");
        var retryOperationId = Guid.NewGuid();
        var retry = await owner.Workspace.SaveAsync(
            new SaveRequest(retryOperationId, freshState.Revision, freshState.OutputBaseline.ShouldNotBeNull()),
            TestContext.Current.CancellationToken);
        retry.Status.ShouldBe(SaveCommitStatus.Committed, DescribeError(retry.Error));
        retry.OperationId.ShouldBe(retryOperationId);
        (await ReadEditorIdAsync(owner.Workspace, output, initial.FormKey)).ShouldBe("CfBusyPending");
        AssertSourceArtifactsUnchanged(sourceArtifacts, fixture.SnapshotArtifacts());
    }

    /// <summary>Verifies direct destination corruption after two selections is preserved against both guarded saves.</summary>
    /// <returns>A task that completes after both owners retain their independent staged previews.</returns>
    [Fact]
    public async Task SaveAsync_DirectArtifactDrift_IsRejectedByBothOwnersWithoutOverwrite()
    {
        using var fixture = NativeWorkspaceIntegrationFixture.Create(SupportedGame.Skyrim);
        var sourceArtifacts = fixture.SnapshotArtifacts();
        var output = CreateEmbeddedOutputAssociation(fixture);
        var initial = await CreateExistingOutputAsync(fixture, output, "CfInitialDrift");
        await using var servicesA = NativeEngineComposition.Create();
        await using var servicesB = NativeEngineComposition.Create();
        await using var ownerA = await OpenStagedOwnerAsync(
            servicesA,
            fixture,
            output,
            initial.FormKey,
            Guid.NewGuid(),
            "CfDriftA");
        await using var ownerB = await OpenStagedOwnerAsync(
            servicesB,
            fixture,
            output,
            initial.FormKey,
            Guid.NewGuid(),
            "CfDriftB");
        var externalBytes = new byte[] { 0x43, 0x46, 0x2D, 0x44, 0x52, 0x49, 0x46, 0x54 };
        await File.WriteAllBytesAsync(
            output.PluginPath,
            externalBytes,
            TestContext.Current.CancellationToken);

        var saveA = await ownerA.Workspace.SaveAsync(
            new SaveRequest(ownerA.SaveOperationId, ownerA.Workspace.Revision, ownerA.Selection.Baseline),
            TestContext.Current.CancellationToken);
        saveA.Status.ShouldBe(SaveCommitStatus.NotCommitted);
        saveA.Error.ShouldNotBeNull().Code.ShouldBe(EngineErrorCode.ExternalChangeDetected);
        File.ReadAllBytes(output.PluginPath).ShouldBe(externalBytes);
        var saveB = await ownerB.Workspace.SaveAsync(
            new SaveRequest(ownerB.SaveOperationId, ownerB.Workspace.Revision, ownerB.Selection.Baseline),
            TestContext.Current.CancellationToken);
        saveB.Status.ShouldBe(SaveCommitStatus.NotCommitted);
        saveB.Error.ShouldNotBeNull().Code.ShouldBe(EngineErrorCode.ExternalChangeDetected);
        File.ReadAllBytes(output.PluginPath).ShouldBe(externalBytes);
        ownerA.Workspace.OutputSynchronization.Status.ShouldBe(OutputSynchronizationStatus.Ready);
        ownerB.Workspace.OutputSynchronization.Status.ShouldBe(OutputSynchronizationStatus.Ready);
        AssertPreviewEditorId(await ReadPreviewAsync(ownerA.Workspace), initial.FormKey, "CfDriftA");
        AssertPreviewEditorId(await ReadPreviewAsync(ownerB.Workspace), initial.FormKey, "CfDriftB");
        AssertSourceArtifactsUnchanged(sourceArtifacts, fixture.SnapshotArtifacts());
    }

    /// <summary>Verifies disposing one independent owner leaves another dirty owner's native reads and preview usable.</summary>
    /// <returns>A task that completes after the surviving owner and physical artifacts are inspected.</returns>
    [Fact]
    public async Task DisposeAsync_OneOwnerDoesNotInvalidateIndependentDirtyOwner()
    {
        using var fixture = NativeWorkspaceIntegrationFixture.Create(SupportedGame.Starfield);
        var sourceArtifacts = fixture.SnapshotArtifacts();
        var output = CreateEmbeddedOutputAssociation(fixture);
        var initial = await CreateExistingOutputAsync(fixture, output, "CfInitialDispose");
        var outputBefore = SnapshotOutputArtifacts(initial.Baseline);
        await using var servicesA = NativeEngineComposition.Create();
        await using var servicesB = NativeEngineComposition.Create();
        await using var disposedOwner = await OpenStagedOwnerAsync(
            servicesA,
            fixture,
            output,
            initial.FormKey,
            Guid.NewGuid(),
            "CfDisposedOwner");
        await using var survivor = await OpenStagedOwnerAsync(
            servicesB,
            fixture,
            output,
            initial.FormKey,
            Guid.NewGuid(),
            "CfSurvivingOwner");

        await disposedOwner.DisposeAsync();

        survivor.Workspace.OutputSynchronization.Status.ShouldBe(OutputSynchronizationStatus.Ready);
        (await ReadEditorIdAsync(survivor.Workspace, output, initial.FormKey)).ShouldBe("CfSurvivingOwner");
        AssertPreviewEditorId(await ReadPreviewAsync(survivor.Workspace), initial.FormKey, "CfSurvivingOwner");
        File.Exists(output.PluginPath).ShouldBeTrue();
        AssertPhysicalArtifactsUnchanged(outputBefore, SnapshotOutputArtifacts(initial.Baseline));
        AssertSourceArtifactsUnchanged(sourceArtifacts, fixture.SnapshotArtifacts());
    }
}
