using CreationsForge.Core.Engine;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.Internal;
using CreationsForge.Core.Enums;
using Moq;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Serilog;
using Shouldly;

namespace CreationsForge.UnitTests.Engine.Foundation;

/// <summary>Verifies serialized atomic workspace metadata snapshots independently of native record traversal.</summary>
public sealed class FormListWorkspaceStateTests
{
    /// <summary>Verifies output association and baseline cannot be observed as a torn pair.</summary>
    [Fact]
    public void WorkspaceState_WithTornOutputMetadata_RejectsSnapshot()
    {
        var outputPath = Path.GetFullPath(Path.Combine(Path.GetTempPath(), "WorkspaceStateOutput.esp"));
        var output = new OutputAssociation(
            outputPath,
            ModKey.FromNameAndExtension("WorkspaceStateOutput.esp"),
            LocalizedOutputMode.Embedded,
            OutputMasterStyle.Full);

        Should.Throw<ArgumentException>(() => new WorkspaceState(
            SupportedGame.Starfield,
            GameRelease.Starfield,
            output,
            null,
            new OutputSynchronizationState(OutputSynchronizationStatus.Ready, null),
            new WorkspaceRevision(Guid.NewGuid(), 0)));
    }

    /// <summary>Verifies the initial snapshot carries the exact game, release, synchronization, and revision without output metadata.</summary>
    [Fact]
    public async Task ReadStateAsync_BeforeOutputSelection_ReturnsAtomicOpenState()
    {
        await using var fixture = CreateFixture();

        var result = await fixture.Workspace.ReadStateAsync();

        result.Succeeded.ShouldBeTrue();
        result.WorkspaceId.ShouldBe(fixture.Workspace.WorkspaceId);
        result.BaseRevision.ShouldBe(fixture.Workspace.Revision);
        result.ResultRevision.ShouldBe(fixture.Workspace.Revision);
        result.Value!.Game.ShouldBe(SupportedGame.Starfield);
        result.Value.Release.ShouldBe(GameRelease.Starfield);
        result.Value.Output.ShouldBeNull();
        result.Value.OutputBaseline.ShouldBeNull();
        result.Value.OutputSynchronization.Status.ShouldBe(OutputSynchronizationStatus.Ready);
        result.Value.Revision.ShouldBe(fixture.Workspace.Revision);
    }

    /// <summary>Verifies selected output association, complete baseline, and resulting revision are observed together.</summary>
    [Fact]
    public async Task ReadStateAsync_AfterOutputSelection_ReturnsMatchingOutputAndRevision()
    {
        await using var fixture = CreateFixture();
        var outputPath = Path.GetFullPath(Path.Combine(Path.GetTempPath(), "WorkspaceStateOutput.esp"));
        var output = new OutputAssociation(
            outputPath,
            ModKey.FromNameAndExtension("WorkspaceStateOutput.esp"),
            LocalizedOutputMode.Embedded,
            OutputMasterStyle.Full);
        var baseline = new OutputArtifactSetBaseline(
            Guid.NewGuid(),
            new[]
            {
                new NativeArtifactAssociation(
                    outputPath,
                    NativeArtifactRole.Plugin,
                    null,
                    new NativeArtifactFingerprint(false, 0, null))
            });
        var nativeOutput = new Mock<INativeOutputState>();
        nativeOutput.Setup(candidate => candidate.DisposeAsync()).Returns(ValueTask.CompletedTask);
        fixture.Adapter.Setup(candidate => candidate.OpenOutputAsync(
                fixture.Sources.Object,
                It.IsAny<SelectOutputRequest>(),
                It.IsAny<CancellationToken>()))
            .Returns(ValueTask.FromResult(EngineResult<NativeOutputOpenResult>.Success(
                new NativeOutputOpenResult(nativeOutput.Object, output, baseline))));
        var selection = await fixture.Workspace.SelectOutputAsync(new SelectOutputRequest(
            Guid.NewGuid(),
            fixture.Workspace.Revision,
            OutputSelectionMode.CreateNew,
            output));

        var result = await fixture.Workspace.ReadStateAsync();

        selection.Succeeded.ShouldBeTrue();
        result.Succeeded.ShouldBeTrue();
        result.Value!.Output.ShouldBeSameAs(output);
        result.Value.OutputBaseline.ShouldBeSameAs(baseline);
        result.Value.Revision.ShouldBe(selection.Value!.Revision);
        result.Value.Revision.ShouldBe(fixture.Workspace.Revision);
    }

    /// <summary>Verifies state reads preserve the standard typed disposed-workspace failure.</summary>
    [Fact]
    public async Task ReadStateAsync_AfterDisposal_ReturnsWorkspaceDisposed()
    {
        var fixture = CreateFixture();
        await fixture.Workspace.DisposeAsync();

        var result = await fixture.Workspace.ReadStateAsync();

        result.Succeeded.ShouldBeFalse();
        result.Error!.Code.ShouldBe(EngineErrorCode.WorkspaceDisposed);
        result.ResultRevision.ShouldBe(fixture.Workspace.Revision);
    }

    /// <summary>Verifies exact replays remain available while fresh mutations are rejected before native work at capacity.</summary>
    [Fact]
    public async Task BeginEditAsync_AtReplayCapacity_ReplaysExistingAndRejectsFreshOperation()
    {
        await using var fixture = CreateFixture(operationReplayCapacity: 3);
        var operationId = Guid.NewGuid();
        var request = new BeginEditRequest(operationId, fixture.Workspace.Revision, FormListEditRole.New);

        var first = await fixture.Workspace.BeginEditAsync(request);
        var replay = await fixture.Workspace.BeginEditAsync(request);
        var rejected = await fixture.Workspace.BeginEditAsync(new BeginEditRequest(
            Guid.NewGuid(),
            fixture.Workspace.Revision,
            FormListEditRole.New));

        first.Succeeded.ShouldBeFalse();
        first.Error!.Code.ShouldBe(EngineErrorCode.OutputNotSelected);
        replay.ShouldBeSameAs(first);
        rejected.Succeeded.ShouldBeFalse();
        rejected.Error!.Code.ShouldBe(EngineErrorCode.OperationCapacityExceeded);
        fixture.Adapter.Verify(candidate => candidate.BeginEdit(
            It.IsAny<INativeSourceSet>(),
            It.IsAny<INativeOutputState>(),
            It.IsAny<BeginEditRequest>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    /// <summary>Verifies ordinary capacity exhaustion preserves the reserved path needed to save an admitted edit.</summary>
    [Fact]
    public async Task SaveAsync_AfterOrdinaryReplayCapacityIsExhausted_CommitsAdmittedEdit()
    {
        await using var fixture = CreateFixture(operationReplayCapacity: 5);
        var outputPath = Path.GetFullPath(Path.Combine(Path.GetTempPath(), "WorkspaceCapacityOutput.esp"));
        var outputAssociation = new OutputAssociation(
            outputPath,
            ModKey.FromNameAndExtension("WorkspaceCapacityOutput.esp"),
            LocalizedOutputMode.Embedded,
            OutputMasterStyle.Full);
        var initialBaseline = new OutputArtifactSetBaseline(
            Guid.NewGuid(),
            [new NativeArtifactAssociation(outputPath, NativeArtifactRole.Plugin, null, new NativeArtifactFingerprint(false, 0, null))]);
        var savedBaseline = new OutputArtifactSetBaseline(
            Guid.NewGuid(),
            [new NativeArtifactAssociation(outputPath, NativeArtifactRole.Plugin, null, new NativeArtifactFingerprint(true, 1, new string('A', 64)), new NativeFileIdentity("test", "volume", "saved", 1))]);
        var initialOutput = new Mock<INativeOutputState>();
        var editOutput = new Mock<INativeOutputState>();
        var appliedOutput = new Mock<INativeOutputState>();
        var reopenedOutput = new Mock<INativeOutputState>();
        foreach (var nativeOutput in new[] { initialOutput, editOutput, appliedOutput, reopenedOutput })
        {
            nativeOutput.Setup(candidate => candidate.DisposeAsync()).Returns(ValueTask.CompletedTask);
        }

        fixture.Adapter.Setup(candidate => candidate.OpenOutputAsync(
                fixture.Sources.Object,
                It.IsAny<SelectOutputRequest>(),
                It.IsAny<CancellationToken>()))
            .Returns(ValueTask.FromResult(EngineResult<NativeOutputOpenResult>.Success(
                new NativeOutputOpenResult(initialOutput.Object, outputAssociation, initialBaseline))));
        fixture.Adapter.SetupSequence(candidate => candidate.CloneOutput(
                It.IsAny<INativeOutputState>(),
                It.IsAny<CancellationToken>()))
            .Returns(editOutput.Object)
            .Returns(appliedOutput.Object);
        var editId = Guid.NewGuid();
        var formKey = new FormKey(outputAssociation.ModKey, 0x800);
        fixture.Adapter.Setup(candidate => candidate.BeginEdit(
                fixture.Sources.Object,
                editOutput.Object,
                It.IsAny<BeginEditRequest>(),
                It.IsAny<CancellationToken>()))
            .Returns(EngineResult<NativeEditIdentity>.Success(
                new NativeEditIdentity(editId, formKey, null, FormListEditRole.New)));
        fixture.Adapter.Setup(candidate => candidate.PrepareEdit(It.IsAny<FormListEdit>()))
            .Returns(new TestPreparedFormListEdit("capacity-edit"));
        fixture.Adapter.Setup(candidate => candidate.ApplyEdit(
                fixture.Sources.Object,
                appliedOutput.Object,
                formKey,
                It.IsAny<PreparedFormListEdit>()))
            .Returns(EngineResult<NativeEditMutationResult>.Success(new NativeEditMutationResult(changed: true)));
        fixture.Adapter.Setup(candidate => candidate.ReopenOutputAsync(
                fixture.Sources.Object,
                outputAssociation,
                savedBaseline,
                It.IsAny<CancellationToken>()))
            .Returns(ValueTask.FromResult(EngineResult<NativeOutputOpenResult>.Success(
                new NativeOutputOpenResult(reopenedOutput.Object, outputAssociation, savedBaseline))));

        var selection = await fixture.Workspace.SelectOutputAsync(new SelectOutputRequest(
            Guid.NewGuid(), fixture.Workspace.Revision, OutputSelectionMode.CreateNew, outputAssociation));
        var begin = await fixture.Workspace.BeginEditAsync(new BeginEditRequest(
            Guid.NewGuid(), fixture.Workspace.Revision, FormListEditRole.New));
        var apply = await fixture.Workspace.ApplyFormListEditAsync(new FormListEditRequest(
            Guid.NewGuid(), fixture.Workspace.Revision, editId, new SetEditorIdEdit("CapacityEdit")));
        var blocked = await fixture.Workspace.ApplyFormListEditAsync(new FormListEditRequest(
            Guid.NewGuid(), fixture.Workspace.Revision, editId, new SetEditorIdEdit("Blocked")));
        var saveRevision = fixture.Workspace.Revision;
        var saveAttemptCount = 0;
        fixture.SaveCoordinator.Setup(candidate => candidate.SaveAsync(
                It.IsAny<WorkspaceSaveContext>(),
                It.IsAny<SaveRequest>(),
                It.IsAny<CancellationToken>()))
            .Returns<WorkspaceSaveContext, SaveRequest, CancellationToken>((context, attemptedRequest, _) =>
            {
                saveAttemptCount++;
                if (saveAttemptCount <= 2)
                {
                    return ValueTask.FromResult(new SaveResult(
                        context.WorkspaceId,
                        attemptedRequest.OperationId,
                        context.Revision,
                        context.Revision,
                        SaveCommitStatus.NotCommitted,
                        null,
                        null,
                        null,
                        new EngineError(EngineErrorCode.OutputDirectoryBusy, "Synthetic transient contention."),
                        Array.Empty<EngineWarning>()));
                }

                var evidenceToken = new RecoveryEvidenceToken("capacity-save");
                var evidence = new ResolvedOutputEvidence(
                    evidenceToken,
                    context.Game,
                    context.Release,
                    context.WorkspaceId,
                    attemptedRequest.OperationId,
                    context.Revision,
                    context.Sources.Baseline,
                    context.OutputAssociation,
                    savedBaseline,
                    RecoverSaveStatus.Committed);
                return ValueTask.FromResult(new SaveResult(
                    context.WorkspaceId,
                    attemptedRequest.OperationId,
                    context.Revision,
                    context.Revision,
                    SaveCommitStatus.Committed,
                    savedBaseline,
                    evidenceToken,
                    evidence,
                    null,
                    Array.Empty<EngineWarning>()));
            });

        var firstSaveRequest = new SaveRequest(Guid.NewGuid(), saveRevision, initialBaseline);
        var firstSave = await fixture.Workspace.SaveAsync(firstSaveRequest);
        var secondSave = await fixture.Workspace.SaveAsync(new SaveRequest(Guid.NewGuid(), saveRevision, initialBaseline));
        var saved = await fixture.Workspace.SaveAsync(new SaveRequest(Guid.NewGuid(), saveRevision, initialBaseline));
        var expiredReplay = await fixture.Workspace.SaveAsync(firstSaveRequest);
        var expiredReuse = await fixture.Workspace.SaveAsync(new SaveRequest(
            firstSaveRequest.OperationId,
            fixture.Workspace.Revision,
            saved.CommittedBaseline!));

        selection.Succeeded.ShouldBeTrue(selection.Error?.Message);
        begin.Succeeded.ShouldBeTrue(begin.Error?.Message);
        apply.Succeeded.ShouldBeTrue(apply.Error?.Message);
        blocked.Error!.Code.ShouldBe(EngineErrorCode.OperationCapacityExceeded);
        firstSave.Status.ShouldBe(SaveCommitStatus.NotCommitted);
        secondSave.Status.ShouldBe(SaveCommitStatus.NotCommitted);
        saved.Status.ShouldBe(SaveCommitStatus.Committed);
        saved.ResultRevision.Sequence.ShouldBe(saveRevision.Sequence + 1);
        expiredReplay.Error!.Code.ShouldBe(EngineErrorCode.OperationReplayExpired);
        expiredReuse.Error!.Code.ShouldBe(EngineErrorCode.OperationIdReuse);
        fixture.SaveCoordinator.Verify(candidate => candidate.SaveAsync(
            It.IsAny<WorkspaceSaveContext>(), It.IsAny<SaveRequest>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
    }

    /// <summary>Creates one directly constructed workspace with deterministic source, lease, and admission infrastructure.</summary>
    /// <param name="operationReplayCapacity">The positive replay capacity used by the directly constructed workspace.</param>
    /// <returns>The disposable workspace fixture.</returns>
    private static WorkspaceFixture CreateFixture(
        int operationReplayCapacity = OperationReplayStore.DefaultMaximumEntryCount)
    {
        var workspaceId = Guid.NewGuid();
        var baselineId = Guid.NewGuid();
        var sourcePath = Path.GetFullPath(Path.Combine(Path.GetTempPath(), "WorkspaceStateSource.esm"));
        var sources = new Mock<INativeSourceSet>();
        var sourceBaseline = TestWorkspaceInfrastructure.ConfigureSourceBaseline(
            sources.Object,
            baselineId,
            sourcePath);
        sources.Setup(candidate => candidate.DisposeAsync()).Returns(ValueTask.CompletedTask);
        var adapter = new Mock<IFormListGameAdapter>();
        adapter.SetupGet(candidate => candidate.Game).Returns(SupportedGame.Starfield);
        adapter.Setup(candidate => candidate.SupportsRelease(GameRelease.Starfield)).Returns(true);
        var saveCoordinator = new Mock<IWorkspaceSaveCoordinator>();
        TestWorkspaceInfrastructure.ConfigureReadyAdmission(saveCoordinator.Object);
        var request = new WorkspaceOpenRequest(
            workspaceId,
            SupportedGame.Starfield,
            GameRelease.Starfield,
            sourcePath,
            new[] { sourcePath },
            Path.GetDirectoryName(sourcePath)!,
            Array.Empty<string>());
        var sourceOpenResult = new NativeSourceOpenResult(
            sources.Object,
            baselineId,
            sourceBaseline.Artifacts);
        var workspace = new FormListWorkspace(
            request,
            adapter.Object,
            sourceOpenResult,
            saveCoordinator.Object,
            TestWorkspaceInfrastructure.CreateLeaseProvider(),
            new Mock<ILogger>().Object,
            operationReplayCapacity);
        return new WorkspaceFixture(workspace, adapter, sources, saveCoordinator);
    }

    /// <summary>Owns one directly constructed workspace and the mocks needed by state tests.</summary>
    private sealed class WorkspaceFixture : IAsyncDisposable
    {
        /// <summary>Initializes the disposable workspace fixture.</summary>
        /// <param name="workspace">The directly constructed workspace.</param>
        /// <param name="adapter">The game adapter mock.</param>
        /// <param name="sources">The source lifetime mock.</param>
        /// <param name="saveCoordinator">The save coordinator mock.</param>
        internal WorkspaceFixture(
            FormListWorkspace workspace,
            Mock<IFormListGameAdapter> adapter,
            Mock<INativeSourceSet> sources,
            Mock<IWorkspaceSaveCoordinator> saveCoordinator)
        {
            Workspace = workspace;
            Adapter = adapter;
            Sources = sources;
            SaveCoordinator = saveCoordinator;
        }

        /// <summary>Gets the directly constructed workspace.</summary>
        internal FormListWorkspace Workspace { get; }

        /// <summary>Gets the game adapter mock.</summary>
        internal Mock<IFormListGameAdapter> Adapter { get; }

        /// <summary>Gets the source lifetime mock.</summary>
        internal Mock<INativeSourceSet> Sources { get; }

        /// <summary>Gets the save coordinator mock.</summary>
        internal Mock<IWorkspaceSaveCoordinator> SaveCoordinator { get; }

        /// <summary>Disposes the owned workspace.</summary>
        /// <returns>A task representing asynchronous disposal.</returns>
        public ValueTask DisposeAsync()
        {
            return Workspace.DisposeAsync();
        }
    }
}
