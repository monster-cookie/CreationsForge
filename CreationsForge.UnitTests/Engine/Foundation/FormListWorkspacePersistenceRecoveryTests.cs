using System.Security.Cryptography;
using System.Text;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeInputs;
using CreationsForge.Core.Enums;
using Moq;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Serilog;
using Shouldly;

namespace CreationsForge.UnitTests.Engine.Foundation;

/// <summary>Verifies guarded save publication, synchronization latches, and explicit recovery adoption.</summary>
public sealed class FormListWorkspacePersistenceRecoveryTests
{
    /// <summary>Verifies a committed save publishes a fresh native output, clears edit sessions, and replays the final result.</summary>
    [Fact]
    public async Task SaveAsync_WhenCommitted_ReopensFullOutputAndClearsDirtyEditState()
    {
        await using var fixture = await WorkspaceFixture.OpenAsync(selectOutput: true);
        var staged = await fixture.StageEditAsync();
        var stagedOutput = staged.Output;
        var committedOutput = CreateDisposableMock<INativeOutputState>();
        var committedBaseline = fixture.CreateOutputBaseline(Guid.NewGuid(), "committed", 2);
        var saveRequest = new SaveRequest(Guid.NewGuid(), fixture.Workspace.Revision, fixture.SelectedBaseline!);
        fixture.ConfigureCommittedSave(saveRequest, committedBaseline);
        fixture.Adapter.Setup(candidate => candidate.ReopenOutputAsync(
                fixture.Sources.Object,
                fixture.OutputAssociation,
                committedBaseline,
                It.IsAny<CancellationToken>()))
            .Returns(ValueTask.FromResult(EngineResult<NativeOutputOpenResult>.Success(
                new NativeOutputOpenResult(committedOutput.Object, fixture.OutputAssociation, committedBaseline))));

        var result = await fixture.Workspace.SaveAsync(saveRequest);
        var replay = await fixture.Workspace.SaveAsync(saveRequest);
        fixture.Adapter.Setup(candidate => candidate.PrepareEdit(
                It.Is<SetEditorIdEdit>(edit => edit.EditorId == "AfterCommit")))
            .Returns(new TestPreparedFormListEdit("set:AfterCommit"));
        var staleEdit = await fixture.Workspace.ApplyFormListEditAsync(new FormListEditRequest(
            Guid.NewGuid(),
            fixture.Workspace.Revision,
            staged.EditId,
            new SetEditorIdEdit("AfterCommit")));

        result.Status.ShouldBe(SaveCommitStatus.Committed);
        result.Error.ShouldBeNull();
        result.ResultRevision.ShouldBe(fixture.Workspace.Revision);
        result.ResultRevision.Sequence.ShouldBe(saveRequest.ExpectedRevision.Sequence + 1);
        result.CommittedBaseline.ShouldBeSameAs(committedBaseline);
        replay.ShouldBeSameAs(result);
        fixture.Workspace.OutputSynchronization.Status.ShouldBe(OutputSynchronizationStatus.Ready);
        staleEdit.Succeeded.ShouldBeFalse();
        staleEdit.Error!.Code.ShouldBe(EngineErrorCode.EditNotFound);
        stagedOutput.Verify(candidate => candidate.DisposeAsync(), Times.Once);
        committedOutput.Verify(candidate => candidate.DisposeAsync(), Times.Never);
    }

    /// <summary>Verifies an unknown save retains the candidate and blocks ordinary operations while exact command replay remains available.</summary>
    [Fact]
    public async Task SaveAsync_WhenOutcomeUnknown_LatchesAndAllowsOnlyExactReplay()
    {
        await using var fixture = await WorkspaceFixture.OpenAsync(selectOutput: true);
        var staged = await fixture.StageEditAsync();
        var saveRequest = new SaveRequest(Guid.NewGuid(), fixture.Workspace.Revision, fixture.SelectedBaseline!);
        fixture.SaveCoordinator.Setup(candidate => candidate.SaveAsync(
                It.IsAny<WorkspaceSaveContext>(),
                saveRequest,
                It.IsAny<CancellationToken>()))
            .Returns<WorkspaceSaveContext, SaveRequest, CancellationToken>((context, _, _) =>
                ValueTask.FromResult(new SaveResult(
                    context.WorkspaceId,
                    saveRequest.OperationId,
                    context.Revision,
                    context.Revision,
                    SaveCommitStatus.CommitOutcomeUnknown,
                    null,
                    new RecoveryEvidenceToken("unknown-save"),
                    null,
                    new EngineError(EngineErrorCode.CommitOutcomeUnknown, "Synthetic unknown outcome."),
                    Array.Empty<EngineWarning>())));

        var result = await fixture.Workspace.SaveAsync(saveRequest);
        var saveReplay = await fixture.Workspace.SaveAsync(saveRequest);
        var beginReplay = await fixture.Workspace.BeginEditAsync(staged.Request);
        var read = await fixture.Workspace.ListPluginsAsync();
        var newEdit = await fixture.Workspace.BeginEditAsync(new BeginEditRequest(
            Guid.NewGuid(),
            fixture.Workspace.Revision,
            FormListEditRole.New));
        var newSave = await fixture.Workspace.SaveAsync(new SaveRequest(
            Guid.NewGuid(),
            fixture.Workspace.Revision,
            fixture.SelectedBaseline!));

        result.Status.ShouldBe(SaveCommitStatus.CommitOutcomeUnknown);
        saveReplay.ShouldBeSameAs(result);
        beginReplay.ShouldBeSameAs(staged.Result);
        fixture.Workspace.OutputSynchronization.Status.ShouldBe(OutputSynchronizationStatus.RecoveryRequired);
        fixture.Workspace.OutputSynchronization.PendingSave!.OriginalWorkspaceId.ShouldBe(fixture.Workspace.WorkspaceId);
        fixture.Workspace.OutputSynchronization.PendingSave.SaveOperationId.ShouldBe(saveRequest.OperationId);
        read.Error!.Code.ShouldBe(EngineErrorCode.RepairRequired);
        newEdit.Error!.Code.ShouldBe(EngineErrorCode.RepairRequired);
        newSave.Error!.Code.ShouldBe(EngineErrorCode.RepairRequired);
        staged.Output.Verify(candidate => candidate.DisposeAsync(), Times.Never);
        fixture.Adapter.Verify(candidate => candidate.ListPlugins(
            It.IsAny<INativeSourceSet>(),
            It.IsAny<INativeOutputState?>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    /// <summary>Verifies a known committed destination whose workspace reopen fails preserves the candidate and requires explicit reopen adoption.</summary>
    [Fact]
    public async Task SaveAsync_WhenCommittedReopenFails_PreservesCandidateAndLatchesReopen()
    {
        await using var fixture = await WorkspaceFixture.OpenAsync(selectOutput: true);
        var staged = await fixture.StageEditAsync();
        var committedBaseline = fixture.CreateOutputBaseline(Guid.NewGuid(), "committed", 2);
        var saveRequest = new SaveRequest(Guid.NewGuid(), fixture.Workspace.Revision, fixture.SelectedBaseline!);
        fixture.ConfigureCommittedSave(saveRequest, committedBaseline);
        fixture.Adapter.Setup(candidate => candidate.ReopenOutputAsync(
                fixture.Sources.Object,
                fixture.OutputAssociation,
                committedBaseline,
                It.IsAny<CancellationToken>()))
            .Returns(ValueTask.FromResult(EngineResult<NativeOutputOpenResult>.Failure(
                new EngineError(EngineErrorCode.OutputOpenFailed, "Synthetic reopen failure."))));

        var result = await fixture.Workspace.SaveAsync(saveRequest);
        var replay = await fixture.Workspace.SaveAsync(saveRequest);
        var preview = await fixture.Workspace.PreviewAsync();

        result.Status.ShouldBe(SaveCommitStatus.CommittedButReopenFailed);
        result.CommittedBaseline.ShouldBeSameAs(committedBaseline);
        result.ResolvedEvidence.ShouldNotBeNull();
        result.ResultRevision.ShouldBe(saveRequest.ExpectedRevision);
        replay.ShouldBeSameAs(result);
        fixture.Workspace.OutputSynchronization.Status.ShouldBe(OutputSynchronizationStatus.ReopenRequired);
        preview.Error!.Code.ShouldBe(EngineErrorCode.RepairRequired);
        staged.Output.Verify(candidate => candidate.DisposeAsync(), Times.Never);
    }

    /// <summary>Verifies not-committed evidence resumes the same candidate, adopts the current guard baseline, and permits another edit and save.</summary>
    [Fact]
    public async Task ResolveOutputRecoveryAsync_WhenNotCommitted_ResumesCandidateAndUsesResolvedGuardBaseline()
    {
        await using var fixture = await WorkspaceFixture.OpenAsync(selectOutput: true);
        var staged = await fixture.StageEditAsync();
        var unknownRequest = new SaveRequest(Guid.NewGuid(), fixture.Workspace.Revision, fixture.SelectedBaseline!);
        fixture.ConfigureUnknownSave(unknownRequest);
        await fixture.Workspace.SaveAsync(unknownRequest);
        var restoredBaseline = fixture.CreateOutputBaseline(Guid.NewGuid(), "original", 9);
        var evidence = fixture.CreateEvidence(
            unknownRequest.OperationId,
            unknownRequest.ExpectedRevision,
            restoredBaseline,
            RecoverSaveStatus.NotCommitted,
            "restored-not-committed");
        fixture.ConfigureValidatedEvidence(evidence);
        var proofOutput = CreateDisposableMock<INativeOutputState>();
        fixture.Adapter.Setup(candidate => candidate.ReopenOutputAsync(
                fixture.Sources.Object,
                fixture.OutputAssociation,
                restoredBaseline,
                It.IsAny<CancellationToken>()))
            .Returns(ValueTask.FromResult(EngineResult<NativeOutputOpenResult>.Success(
                new NativeOutputOpenResult(proofOutput.Object, fixture.OutputAssociation, restoredBaseline))));
        var resolveRequest = new ResolveOutputRecoveryRequest(
            Guid.NewGuid(),
            fixture.Workspace.Revision,
            OutputRecoveryAdoptionMode.ResumeStagedAfterNotCommitted,
            evidence);

        var resolved = await fixture.Workspace.ResolveOutputRecoveryAsync(resolveRequest);
        var appliedOutput = CreateDisposableMock<INativeOutputState>();
        fixture.Adapter.Setup(candidate => candidate.CloneOutput(staged.Output.Object, It.IsAny<CancellationToken>()))
            .Returns(appliedOutput.Object);
        fixture.Adapter.Setup(candidate => candidate.PrepareEdit(It.IsAny<FormListEdit>()))
            .Returns(new TestPreparedFormListEdit("set:resumed"));
        fixture.Adapter.Setup(candidate => candidate.ApplyEdit(
                fixture.Sources.Object,
                appliedOutput.Object,
                staged.Result.Value!.FormKey,
                It.IsAny<PreparedFormListEdit>()))
            .Returns(EngineResult<NativeEditMutationResult>.Success(new NativeEditMutationResult(true)));
        var applied = await fixture.Workspace.ApplyFormListEditAsync(new FormListEditRequest(
            Guid.NewGuid(),
            fixture.Workspace.Revision,
            staged.EditId,
            new SetEditorIdEdit("Resumed")));
        WorkspaceSaveContext? nextContext = null;
        var nextSaveRequest = new SaveRequest(Guid.NewGuid(), fixture.Workspace.Revision, restoredBaseline);
        fixture.SaveCoordinator.Setup(candidate => candidate.SaveAsync(
                It.IsAny<WorkspaceSaveContext>(),
                nextSaveRequest,
                It.IsAny<CancellationToken>()))
            .Returns<WorkspaceSaveContext, SaveRequest, CancellationToken>((context, _, _) =>
            {
                nextContext = context;
                return ValueTask.FromResult(new SaveResult(
                    context.WorkspaceId,
                    nextSaveRequest.OperationId,
                    context.Revision,
                    context.Revision,
                    SaveCommitStatus.NotCommitted,
                    null,
                    null,
                    null,
                    new EngineError(EngineErrorCode.ValidationFailed, "Synthetic precommit rejection."),
                    Array.Empty<EngineWarning>()));
            });
        var nextSave = await fixture.Workspace.SaveAsync(nextSaveRequest);

        resolved.Succeeded.ShouldBeTrue(resolved.Error?.Message);
        resolved.Value!.Baseline.ShouldBeSameAs(restoredBaseline);
        fixture.Workspace.OutputSynchronization.Status.ShouldBe(OutputSynchronizationStatus.Ready);
        proofOutput.Verify(candidate => candidate.DisposeAsync(), Times.Once);
        staged.Output.Verify(candidate => candidate.DisposeAsync(), Times.Once);
        applied.Succeeded.ShouldBeTrue(applied.Error?.Message);
        nextSave.Status.ShouldBe(SaveCommitStatus.NotCommitted);
        nextContext.ShouldNotBeNull();
        nextContext.OutputBaseline.ShouldBeSameAs(restoredBaseline);
    }

    /// <summary>Verifies validated terminal evidence can reopen a matching selected or unselected output.</summary>
    /// <param name="selectOutput">Whether the workspace starts with selected native state.</param>
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ResolveOutputRecoveryAsync_WhenEvidenceIsTerminal_ReopensSelectedOrUnselectedOutput(bool selectOutput)
    {
        await using var fixture = await WorkspaceFixture.OpenAsync(selectOutput);
        var originalWorkspaceId = selectOutput ? fixture.Workspace.WorkspaceId : Guid.NewGuid();
        var saveOperationId = Guid.NewGuid();
        var resolvedBaseline = fixture.CreateOutputBaseline(Guid.NewGuid(), "resolved", 3);
        var evidence = fixture.CreateEvidence(
            saveOperationId,
            new WorkspaceRevision(Guid.NewGuid(), 4),
            resolvedBaseline,
            RecoverSaveStatus.Committed,
            "external-committed",
            originalWorkspaceId);
        fixture.ConfigureValidatedEvidence(evidence);
        var reopenedOutput = CreateDisposableMock<INativeOutputState>();
        fixture.Adapter.Setup(candidate => candidate.ReopenOutputAsync(
                fixture.Sources.Object,
                fixture.OutputAssociation,
                resolvedBaseline,
                It.IsAny<CancellationToken>()))
            .Returns(ValueTask.FromResult(EngineResult<NativeOutputOpenResult>.Success(
                new NativeOutputOpenResult(reopenedOutput.Object, fixture.OutputAssociation, resolvedBaseline))));

        var result = await fixture.Workspace.ResolveOutputRecoveryAsync(new ResolveOutputRecoveryRequest(
            Guid.NewGuid(),
            fixture.Workspace.Revision,
            OutputRecoveryAdoptionMode.ReopenResolvedOutput,
            evidence));

        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        result.Value!.Output.ShouldBeSameAs(fixture.OutputAssociation);
        result.Value.Baseline.ShouldBeSameAs(resolvedBaseline);
        fixture.Workspace.OutputSynchronization.Status.ShouldBe(OutputSynchronizationStatus.Ready);
        fixture.Adapter.Verify(candidate => candidate.ReopenOutputAsync(
            fixture.Sources.Object,
            fixture.OutputAssociation,
            resolvedBaseline,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>Verifies cross-game, cross-release, stale-source, wrong-output, and invalid-token evidence cannot be adopted.</summary>
    [Fact]
    public async Task ResolveOutputRecoveryAsync_WhenEvidenceIdentityIsWrong_RejectsBeforeNativePublication()
    {
        await using var fixture = await WorkspaceFixture.OpenAsync(selectOutput: true);
        var baseline = fixture.CreateOutputBaseline(Guid.NewGuid(), "resolved", 3);
        var saveOperationId = Guid.NewGuid();
        var saveRevision = new WorkspaceRevision(Guid.NewGuid(), 2);
        var valid = fixture.CreateEvidence(saveOperationId, saveRevision, baseline, RecoverSaveStatus.Committed, "valid");
        var wrongGame = new ResolvedOutputEvidence(
            valid.EvidenceToken,
            SupportedGame.Fallout4,
            GameRelease.Fallout4,
            valid.OriginalWorkspaceId,
            valid.SaveOperationId,
            valid.SaveBaseRevision,
            valid.SourceBaseline,
            valid.Output,
            valid.ResolvedOutputBaseline,
            valid.Status);
        var wrongRelease = new ResolvedOutputEvidence(
            valid.EvidenceToken,
            SupportedGame.Starfield,
            GameRelease.SkyrimSE,
            valid.OriginalWorkspaceId,
            valid.SaveOperationId,
            valid.SaveBaseRevision,
            valid.SourceBaseline,
            valid.Output,
            valid.ResolvedOutputBaseline,
            valid.Status);
        var staleSources = new ResolvedOutputEvidence(
            valid.EvidenceToken,
            valid.Game,
            valid.Release,
            valid.OriginalWorkspaceId,
            valid.SaveOperationId,
            valid.SaveBaseRevision,
            new NativeSourceInputBaseline(Guid.NewGuid(), valid.SourceBaseline.Artifacts),
            valid.Output,
            valid.ResolvedOutputBaseline,
            valid.Status);
        var wrongOutput = new ResolvedOutputEvidence(
            valid.EvidenceToken,
            valid.Game,
            valid.Release,
            valid.OriginalWorkspaceId,
            valid.SaveOperationId,
            valid.SaveBaseRevision,
            valid.SourceBaseline,
            fixture.CreateOutputAssociation("Other.esp"),
            valid.ResolvedOutputBaseline,
            valid.Status);
        var wrongToken = new ResolvedOutputEvidence(
            new RecoveryEvidenceToken("wrong-token"),
            valid.Game,
            valid.Release,
            valid.OriginalWorkspaceId,
            valid.SaveOperationId,
            valid.SaveBaseRevision,
            valid.SourceBaseline,
            valid.Output,
            valid.ResolvedOutputBaseline,
            valid.Status);
        fixture.SaveCoordinator.Setup(candidate => candidate.ValidateResolvedEvidenceAsync(
                It.IsAny<IOutputDirectoryLease>(),
                wrongToken,
                It.IsAny<CancellationToken>()))
            .Returns(ValueTask.FromResult(EngineResult<ResolvedOutputEvidence>.Failure(
                new EngineError(EngineErrorCode.NoRecoveryEvidence, "Synthetic token rejection."))));

        foreach (var evidence in new[] { wrongGame, wrongRelease, staleSources, wrongOutput, wrongToken })
        {
            var result = await fixture.Workspace.ResolveOutputRecoveryAsync(new ResolveOutputRecoveryRequest(
                Guid.NewGuid(),
                fixture.Workspace.Revision,
                OutputRecoveryAdoptionMode.ReopenResolvedOutput,
                evidence));
            result.Succeeded.ShouldBeFalse();
        }

        fixture.Adapter.Verify(candidate => candidate.ReopenOutputAsync(
            It.IsAny<INativeSourceSet>(),
            It.IsAny<OutputAssociation>(),
            It.IsAny<OutputArtifactSetBaseline>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    /// <summary>Verifies cancellation after native recovery acquisition disposes both the unpublished output and the held lease.</summary>
    [Fact]
    public async Task ResolveOutputRecoveryAsync_WhenCanceledAfterNativeOpen_CleansUpAndRetainsState()
    {
        await using var fixture = await WorkspaceFixture.OpenAsync(selectOutput: false);
        var baseline = fixture.CreateOutputBaseline(Guid.NewGuid(), "resolved", 3);
        var evidence = fixture.CreateEvidence(
            Guid.NewGuid(),
            new WorkspaceRevision(Guid.NewGuid(), 1),
            baseline,
            RecoverSaveStatus.Committed,
            "cancel-recovery",
            Guid.NewGuid());
        fixture.ConfigureValidatedEvidence(evidence);
        var reopenedOutput = CreateDisposableMock<INativeOutputState>();
        using var cancellationSource = new CancellationTokenSource();
        fixture.Adapter.Setup(candidate => candidate.ReopenOutputAsync(
                fixture.Sources.Object,
                fixture.OutputAssociation,
                baseline,
                cancellationSource.Token))
            .Returns(() =>
            {
                cancellationSource.Cancel();
                return ValueTask.FromResult(EngineResult<NativeOutputOpenResult>.Success(
                    new NativeOutputOpenResult(reopenedOutput.Object, fixture.OutputAssociation, baseline)));
            });

        await Should.ThrowAsync<OperationCanceledException>(async () =>
            await fixture.Workspace.ResolveOutputRecoveryAsync(new ResolveOutputRecoveryRequest(
                Guid.NewGuid(),
                fixture.Workspace.Revision,
                OutputRecoveryAdoptionMode.ReopenResolvedOutput,
                evidence), cancellationSource.Token));

        reopenedOutput.Verify(candidate => candidate.DisposeAsync(), Times.Once);
        fixture.AcquiredLeases.Last().Verify(candidate => candidate.DisposeAsync(), Times.Once);
        fixture.Workspace.Revision.Sequence.ShouldBe(0UL);
    }

    /// <summary>Verifies lease contention rejects output selection before admission or native opening.</summary>
    [Fact]
    public async Task SelectOutputAsync_WhenLeaseIsBusy_ReturnsTypedFailureBeforeNativeOpen()
    {
        await using var fixture = await WorkspaceFixture.OpenAsync(selectOutput: false);
        fixture.LeaseProvider.Reset();
        fixture.LeaseProvider.Setup(candidate => candidate.AcquireAsync(
                It.IsAny<string>(),
                OutputDirectoryLeaseMode.CreateOrOpen,
                It.IsAny<CancellationToken>()))
            .Returns(ValueTask.FromResult(EngineResult<OutputDirectoryLeaseAcquisition>.Failure(
                new EngineError(EngineErrorCode.OutputDirectoryBusy, "Synthetic lease contention."))));

        var result = await fixture.Workspace.SelectOutputAsync(new SelectOutputRequest(
            Guid.NewGuid(),
            fixture.Workspace.Revision,
            OutputSelectionMode.CreateNew,
            fixture.OutputAssociation));

        result.Succeeded.ShouldBeFalse();
        result.Error!.Code.ShouldBe(EngineErrorCode.OutputDirectoryBusy);
        fixture.SaveCoordinator.Verify(candidate => candidate.InspectOutputAdmissionAsync(
            It.IsAny<IOutputDirectoryLease>(),
            It.IsAny<OutputAdmissionRequest>(),
            It.IsAny<CancellationToken>()), Times.Never);
        fixture.Adapter.Verify(candidate => candidate.OpenOutputAsync(
            It.IsAny<INativeSourceSet>(),
            It.IsAny<SelectOutputRequest>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    /// <summary>Creates an independently verifiable asynchronous native lifetime mock.</summary>
    /// <typeparam name="T">The native lifetime contract.</typeparam>
    /// <returns>The configured disposable mock.</returns>
    private static Mock<T> CreateDisposableMock<T>()
        where T : class, IAsyncDisposable
    {
        var value = new Mock<T>();
        value.Setup(candidate => candidate.DisposeAsync()).Returns(ValueTask.CompletedTask);
        return value;
    }

    /// <summary>Owns one deterministic workspace test graph and its disposable temporary files.</summary>
    private sealed class WorkspaceFixture : IAsyncDisposable
    {
        /// <summary>Initializes the already-open fixture.</summary>
        private WorkspaceFixture(
            DirectoryInfo directory,
            Mock<INativeSourceSet> sources,
            Mock<IFormListGameAdapter> adapter,
            Mock<IWorkspaceSaveCoordinator> saveCoordinator,
            Mock<IOutputDirectoryLeaseProvider> leaseProvider,
            List<Mock<IOutputDirectoryLease>> acquiredLeases,
            IFormListWorkspace workspace,
            OutputAssociation outputAssociation,
            OutputArtifactSetBaseline? selectedBaseline)
        {
            Directory = directory;
            Sources = sources;
            Adapter = adapter;
            SaveCoordinator = saveCoordinator;
            LeaseProvider = leaseProvider;
            AcquiredLeases = acquiredLeases;
            Workspace = workspace;
            OutputAssociation = outputAssociation;
            SelectedBaseline = selectedBaseline;
        }

        /// <summary>Gets the disposable temporary directory.</summary>
        internal DirectoryInfo Directory { get; }

        /// <summary>Gets the mocked source lifetime.</summary>
        internal Mock<INativeSourceSet> Sources { get; }

        /// <summary>Gets the mocked game adapter.</summary>
        internal Mock<IFormListGameAdapter> Adapter { get; }

        /// <summary>Gets the mocked save coordinator.</summary>
        internal Mock<IWorkspaceSaveCoordinator> SaveCoordinator { get; }

        /// <summary>Gets the mocked output-directory lease provider.</summary>
        internal Mock<IOutputDirectoryLeaseProvider> LeaseProvider { get; }

        /// <summary>Gets every lease issued to the workspace in acquisition order.</summary>
        internal List<Mock<IOutputDirectoryLease>> AcquiredLeases { get; }

        /// <summary>Gets the live workspace under test.</summary>
        internal IFormListWorkspace Workspace { get; }

        /// <summary>Gets the canonical test output association.</summary>
        internal OutputAssociation OutputAssociation { get; }

        /// <summary>Gets the baseline selected while opening the fixture, when requested.</summary>
        internal OutputArtifactSetBaseline? SelectedBaseline { get; }

        /// <summary>Creates a complete workspace fixture and optionally selects its output.</summary>
        /// <param name="selectOutput">Whether to publish initial output state.</param>
        /// <returns>The independently disposable opened fixture.</returns>
        internal static async Task<WorkspaceFixture> OpenAsync(bool selectOutput)
        {
            var directory = System.IO.Directory.CreateTempSubdirectory();
            var dataDirectory = directory.CreateSubdirectory("Data");
            var stringsDirectory = directory.CreateSubdirectory("Strings");
            var masterPath = Path.Combine(dataDirectory.FullName, "Master.esm");
            var sourcePath = Path.Combine(dataDirectory.FullName, "Source.esp");
            File.WriteAllBytes(masterPath, Array.Empty<byte>());
            File.WriteAllBytes(sourcePath, Array.Empty<byte>());
            var outputAssociation = new OutputAssociation(
                Path.Combine(directory.FullName, "Output.esp"),
                CreateModKey("Output.esp"),
                LocalizedOutputMode.Embedded,
                OutputMasterStyle.Full);
            var sourceBaselineId = Guid.NewGuid();
            var sources = CreateDisposableMock<INativeSourceSet>();
            var sourceBaseline = TestWorkspaceInfrastructure.ConfigureSourceBaseline(
                sources.Object,
                sourceBaselineId,
                sourcePath);
            var adapter = new Mock<IFormListGameAdapter>();
            adapter.SetupGet(candidate => candidate.Game).Returns(SupportedGame.Starfield);
            adapter.Setup(candidate => candidate.SupportsRelease(GameRelease.Starfield)).Returns(true);
            adapter.Setup(candidate => candidate.OpenSourcesAsync(
                    It.IsAny<WorkspaceOpenRequest>(),
                    It.IsAny<CancellationToken>()))
                .Returns(ValueTask.FromResult(EngineResult<NativeSourceOpenResult>.Success(
                    new NativeSourceOpenResult(sources.Object, sourceBaseline.BaselineId, sourceBaseline.Artifacts))));
            adapter.Setup(candidate => candidate.ListPlugins(
                    It.IsAny<INativeSourceSet>(),
                    It.IsAny<INativeOutputState?>(),
                    It.IsAny<CancellationToken>()))
                .Returns(EngineResult<IReadOnlyList<PluginSummary>>.Success(Array.Empty<PluginSummary>()));
            var saveCoordinator = new Mock<IWorkspaceSaveCoordinator>();
            saveCoordinator.Setup(candidate => candidate.InspectOutputAdmissionAsync(
                    It.IsAny<IOutputDirectoryLease>(),
                    It.IsAny<OutputAdmissionRequest>(),
                    It.IsAny<CancellationToken>()))
                .Returns(ValueTask.FromResult(EngineResult<OutputAdmissionResult>.Success(
                    new OutputAdmissionResult(OutputSynchronizationStatus.Ready, null))));
            var leases = new List<Mock<IOutputDirectoryLease>>();
            var leaseProvider = new Mock<IOutputDirectoryLeaseProvider>();
            leaseProvider.Setup(candidate => candidate.AcquireAsync(
                    It.IsAny<string>(),
                    OutputDirectoryLeaseMode.CreateOrOpen,
                    It.IsAny<CancellationToken>()))
                .Returns<string, OutputDirectoryLeaseMode, CancellationToken>((path, _, _) =>
                {
                    var lease = CreateDisposableMock<IOutputDirectoryLease>();
                    lease.SetupGet(candidate => candidate.OutputDirectoryPath).Returns(path);
                    leases.Add(lease);
                    return ValueTask.FromResult(EngineResult<OutputDirectoryLeaseAcquisition>.Success(
                        new OutputDirectoryLeaseAcquisition(OutputDirectoryLeaseAcquisitionStatus.Acquired, lease.Object)));
                });
            var request = new WorkspaceOpenRequest(
                Guid.NewGuid(),
                SupportedGame.Starfield,
                GameRelease.Starfield,
                sourcePath,
                new[] { masterPath, sourcePath },
                dataDirectory.FullName,
                new[] { stringsDirectory.FullName });
            var factory = new Core.Engine.FormListWorkspaceFactory(
                new[] { adapter.Object },
                saveCoordinator.Object,
                leaseProvider.Object,
                Mock.Of<ILogger>());
            var openResult = await factory.OpenAsync(request);
            openResult.Succeeded.ShouldBeTrue(openResult.Error?.Message);
            var workspace = openResult.Value!;
            OutputArtifactSetBaseline? selectedBaseline = null;
            if (selectOutput)
            {
                var initialOutput = CreateDisposableMock<INativeOutputState>();
                selectedBaseline = CreateOutputBaseline(outputAssociation, Guid.NewGuid(), "original", 1);
                adapter.Setup(candidate => candidate.OpenOutputAsync(
                        sources.Object,
                        It.IsAny<SelectOutputRequest>(),
                        It.IsAny<CancellationToken>()))
                    .Returns(ValueTask.FromResult(EngineResult<NativeOutputOpenResult>.Success(
                        new NativeOutputOpenResult(initialOutput.Object, outputAssociation, selectedBaseline))));
                var selection = await workspace.SelectOutputAsync(new SelectOutputRequest(
                    Guid.NewGuid(),
                    workspace.Revision,
                    OutputSelectionMode.CreateNew,
                    outputAssociation));
                selection.Succeeded.ShouldBeTrue(selection.Error?.Message);
            }

            return new WorkspaceFixture(
                directory,
                sources,
                adapter,
                saveCoordinator,
                leaseProvider,
                leases,
                workspace,
                outputAssociation,
                selectedBaseline);
        }

        /// <summary>Stages one new FormList edit and returns its request, receipt, and published candidate.</summary>
        /// <returns>The staged edit fixture.</returns>
        internal async Task<StagedEditFixture> StageEditAsync()
        {
            var candidate = CreateDisposableMock<INativeOutputState>();
            var editId = Guid.NewGuid();
            var target = new FormKey(OutputAssociation.ModKey, 0x800);
            Adapter.Setup(candidateAdapter => candidateAdapter.CloneOutput(
                    It.IsAny<INativeOutputState>(),
                    It.IsAny<CancellationToken>()))
                .Returns(candidate.Object);
            Adapter.Setup(candidateAdapter => candidateAdapter.BeginEdit(
                    Sources.Object,
                    candidate.Object,
                    It.IsAny<BeginEditRequest>(),
                    It.IsAny<CancellationToken>()))
                .Returns(EngineResult<NativeEditIdentity>.Success(
                    new NativeEditIdentity(editId, target, null, FormListEditRole.New)));
            var request = new BeginEditRequest(Guid.NewGuid(), Workspace.Revision, FormListEditRole.New);
            var result = await Workspace.BeginEditAsync(request);
            result.Succeeded.ShouldBeTrue(result.Error?.Message);
            return new StagedEditFixture(request, result, candidate);
        }

        /// <summary>Configures a complete coordinator committed result carrying terminal evidence.</summary>
        /// <param name="request">The exact save request.</param>
        /// <param name="committedBaseline">The committed destination baseline.</param>
        internal void ConfigureCommittedSave(SaveRequest request, OutputArtifactSetBaseline committedBaseline)
        {
            SaveCoordinator.Setup(candidate => candidate.SaveAsync(
                    It.IsAny<WorkspaceSaveContext>(),
                    request,
                    It.IsAny<CancellationToken>()))
                .Returns<WorkspaceSaveContext, SaveRequest, CancellationToken>((context, _, _) =>
                {
                    var evidence = CreateEvidence(
                        request.OperationId,
                        context.Revision,
                        committedBaseline,
                        RecoverSaveStatus.Committed,
                        "committed-save");
                    return ValueTask.FromResult(new SaveResult(
                        context.WorkspaceId,
                        request.OperationId,
                        context.Revision,
                        context.Revision,
                        SaveCommitStatus.Committed,
                        committedBaseline,
                        evidence.EvidenceToken,
                        evidence,
                        null,
                        Array.Empty<EngineWarning>()));
                });
        }

        /// <summary>Configures an unknown coordinator save result for the supplied request.</summary>
        /// <param name="request">The exact save request.</param>
        internal void ConfigureUnknownSave(SaveRequest request)
        {
            SaveCoordinator.Setup(candidate => candidate.SaveAsync(
                    It.IsAny<WorkspaceSaveContext>(),
                    request,
                    It.IsAny<CancellationToken>()))
                .Returns<WorkspaceSaveContext, SaveRequest, CancellationToken>((context, _, _) =>
                    ValueTask.FromResult(new SaveResult(
                        context.WorkspaceId,
                        request.OperationId,
                        context.Revision,
                        context.Revision,
                        SaveCommitStatus.CommitOutcomeUnknown,
                        null,
                        new RecoveryEvidenceToken("unknown-save"),
                        null,
                        new EngineError(EngineErrorCode.CommitOutcomeUnknown, "Synthetic unknown outcome."),
                        Array.Empty<EngineWarning>())));
        }

        /// <summary>Creates terminal evidence matching this fixture's exact game, source baseline, and output.</summary>
        /// <param name="saveOperationId">The original save identifier.</param>
        /// <param name="saveBaseRevision">The original save base revision.</param>
        /// <param name="resolvedBaseline">The terminal destination baseline.</param>
        /// <param name="status">The terminal commitment status.</param>
        /// <param name="token">The opaque evidence token.</param>
        /// <param name="originalWorkspaceId">An optional external original workspace identifier.</param>
        /// <returns>The immutable terminal evidence claims.</returns>
        internal ResolvedOutputEvidence CreateEvidence(
            Guid saveOperationId,
            WorkspaceRevision saveBaseRevision,
            OutputArtifactSetBaseline resolvedBaseline,
            RecoverSaveStatus status,
            string token,
            Guid? originalWorkspaceId = null)
        {
            return new ResolvedOutputEvidence(
                new RecoveryEvidenceToken(token),
                SupportedGame.Starfield,
                GameRelease.Starfield,
                originalWorkspaceId ?? Workspace.WorkspaceId,
                saveOperationId,
                saveBaseRevision,
                Sources.Object.Baseline,
                OutputAssociation,
                resolvedBaseline,
                status);
        }

        /// <summary>Configures coordinator validation to return the exact supplied terminal evidence.</summary>
        /// <param name="evidence">The evidence to validate.</param>
        internal void ConfigureValidatedEvidence(ResolvedOutputEvidence evidence)
        {
            SaveCoordinator.Setup(candidate => candidate.ValidateResolvedEvidenceAsync(
                    It.IsAny<IOutputDirectoryLease>(),
                    evidence,
                    It.IsAny<CancellationToken>()))
                .Returns(ValueTask.FromResult(EngineResult<ResolvedOutputEvidence>.Success(evidence)));
        }

        /// <summary>Creates a complete output baseline under this fixture's canonical output.</summary>
        /// <param name="baselineId">The output baseline identifier.</param>
        /// <param name="contentIdentity">The synthetic content hash seed.</param>
        /// <param name="fileIdentity">The synthetic physical file identity suffix.</param>
        /// <returns>The complete output baseline.</returns>
        internal OutputArtifactSetBaseline CreateOutputBaseline(
            Guid baselineId,
            string contentIdentity,
            int fileIdentity)
        {
            return CreateOutputBaseline(OutputAssociation, baselineId, contentIdentity, fileIdentity);
        }

        /// <summary>Creates another canonical output association under the fixture directory.</summary>
        /// <param name="pluginFileName">The requested native plugin file name.</param>
        /// <returns>The canonical output association.</returns>
        internal OutputAssociation CreateOutputAssociation(string pluginFileName)
        {
            return new OutputAssociation(
                Path.Combine(Directory.FullName, pluginFileName),
                CreateModKey(pluginFileName),
                LocalizedOutputMode.Embedded,
                OutputMasterStyle.Full);
        }

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            await Workspace.DisposeAsync();
            Directory.Delete(true);
        }

        /// <summary>Creates one complete deterministic output artifact baseline.</summary>
        /// <param name="output">The canonical output association.</param>
        /// <param name="baselineId">The complete observation identifier.</param>
        /// <param name="contentIdentity">The synthetic content identity seed.</param>
        /// <param name="fileIdentity">The synthetic physical file identity suffix.</param>
        /// <returns>The complete output baseline.</returns>
        private static OutputArtifactSetBaseline CreateOutputBaseline(
            OutputAssociation output,
            Guid baselineId,
            string contentIdentity,
            int fileIdentity)
        {
            var content = Encoding.UTF8.GetBytes(contentIdentity);
            return new OutputArtifactSetBaseline(
                baselineId,
                Array.AsReadOnly(new[]
                {
                    new NativeArtifactAssociation(
                        output.PluginPath,
                        NativeArtifactRole.Plugin,
                        null,
                        new NativeArtifactFingerprint(true, content.LongLength, Convert.ToHexString(SHA256.HashData(content))),
                        new NativeFileIdentity("test", "volume", $"output-{fileIdentity}", 1)),
                }));
        }

        /// <summary>Parses a native ModKey for a synthetic plugin file name.</summary>
        /// <param name="pluginFileName">The native plugin file name.</param>
        /// <returns>The parsed native ModKey.</returns>
        private static ModKey CreateModKey(string pluginFileName)
        {
            ModKey.TryFromNameAndExtension(pluginFileName, out var modKey, out var error).ShouldBeTrue(error);
            return modKey;
        }
    }

    /// <summary>Retains one staged edit's replay request, receipt, identity, and published candidate.</summary>
    private sealed class StagedEditFixture
    {
        /// <summary>Initializes a staged edit fixture.</summary>
        /// <param name="request">The replayable begin-edit request.</param>
        /// <param name="result">The successful begin-edit result.</param>
        /// <param name="output">The published staged output candidate.</param>
        internal StagedEditFixture(
            BeginEditRequest request,
            EngineResult<EditReceipt> result,
            Mock<INativeOutputState> output)
        {
            Request = request;
            Result = result;
            Output = output;
        }

        /// <summary>Gets the replayable begin-edit request.</summary>
        internal BeginEditRequest Request { get; }

        /// <summary>Gets the successful begin-edit result.</summary>
        internal EngineResult<EditReceipt> Result { get; }

        /// <summary>Gets the stable edit identifier.</summary>
        internal Guid EditId => Result.Value!.EditId;

        /// <summary>Gets the published staged output candidate.</summary>
        internal Mock<INativeOutputState> Output { get; }
    }
}
