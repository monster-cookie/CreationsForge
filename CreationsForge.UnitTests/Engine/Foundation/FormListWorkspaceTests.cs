using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.RecordInspection;
using CreationsForge.Core.Enums;
using Moq;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Serilog;
using Shouldly;

namespace CreationsForge.UnitTests.Engine.Foundation;

/// <summary>
/// Verifies serialized workspace mutation, replay, cancellation, and disposal behavior.
/// </summary>
public sealed class FormListWorkspaceTests
{
    /// <summary>Verifies repeated disposal releases each owned plugin handle exactly once.</summary>
    [Fact]
    public async Task DisposeAsync_WhenRepeated_DisposesOutputAndSourcesOnce()
    {
        var directory = Directory.CreateTempSubdirectory();
        try
        {
            var source = CreateAsyncDisposableMock<IPluginSourceSet>();
            var output = CreateAsyncDisposableMock<IPluginOutputState>();
            var adapter = CreateAdapter(directory, source.Object);
            var saveCoordinator = new Mock<IWorkspaceSaveCoordinator>();
            var workspace = await OpenWorkspaceAsync(directory, adapter, saveCoordinator.Object);
            await SelectOutputAsync(directory, workspace, adapter, output.Object);

            await workspace.DisposeAsync();
            await workspace.DisposeAsync();

            output.Verify(candidate => candidate.DisposeAsync(), Times.Once);
            source.Verify(candidate => candidate.DisposeAsync(), Times.Once);
        }
        finally
        {
            directory.Delete(true);
        }
    }

    /// <summary>Verifies a rejected candidate is disposed while the published output and revision remain unchanged.</summary>
    [Fact]
    public async Task ApplyFormListEditAsync_WhenAdapterRejectsCandidate_PreservesPublishedState()
    {
        var directory = Directory.CreateTempSubdirectory();
        try
        {
            var source = CreateAsyncDisposableMock<IPluginSourceSet>();
            var initialOutput = CreateAsyncDisposableMock<IPluginOutputState>();
            var editOutput = CreateAsyncDisposableMock<IPluginOutputState>();
            var rejectedCandidate = CreateAsyncDisposableMock<IPluginOutputState>();
            var adapter = CreateAdapter(directory, source.Object);
            var saveCoordinator = new Mock<IWorkspaceSaveCoordinator>();
            var workspace = await OpenWorkspaceAsync(directory, adapter, saveCoordinator.Object);
            await SelectOutputAsync(directory, workspace, adapter, initialOutput.Object);
            var target = CreateFormKey("Output.esp", 0x800);
            var editId = Guid.NewGuid();
            adapter.SetupSequence(candidate => candidate.CloneOutput(
                    It.IsAny<IPluginOutputState>(),
                    It.IsAny<CancellationToken>()))
                .Returns(editOutput.Object)
                .Returns(rejectedCandidate.Object);
            adapter.Setup(candidate => candidate.BeginEdit(
                    It.IsAny<IPluginSourceSet>(),
                    editOutput.Object,
                    It.IsAny<BeginEditRequest>(),
                    It.IsAny<CancellationToken>()))
                .Returns(EngineResult<RecordEditIdentity>.Success(
                    new RecordEditIdentity(editId, target, null, FormListEditRole.New)));
            adapter.Setup(candidate => candidate.PrepareEdit(It.IsAny<FormListEdit>()))
                .Returns(new TestPreparedFormListEdit("set-editor-id:Rejected"));
            adapter.Setup(candidate => candidate.ApplyEdit(
                    It.IsAny<IPluginSourceSet>(),
                    rejectedCandidate.Object,
                    target,
                    It.IsAny<PreparedFormListEdit>()))
                .Returns(EngineResult<RecordEditMutationResult>.Failure(
                    new EngineError(EngineErrorCode.ValidationFailed, "Synthetic plugin validation failed.")));
            var beginResult = await workspace.BeginEditAsync(new BeginEditRequest(
                Guid.NewGuid(),
                workspace.Revision,
                FormListEditRole.New));
            beginResult.Succeeded.ShouldBeTrue();
            var revisionBeforeFailure = workspace.Revision;

            var result = await workspace.ApplyFormListEditAsync(new FormListEditRequest(
                Guid.NewGuid(),
                revisionBeforeFailure,
                editId,
                new SetEditorIdEdit("Rejected")));

            result.Succeeded.ShouldBeFalse();
            result.Error.ShouldNotBeNull();
            result.Error.Code.ShouldBe(EngineErrorCode.ValidationFailed);
            workspace.Revision.ShouldBe(revisionBeforeFailure);
            rejectedCandidate.Verify(candidate => candidate.DisposeAsync(), Times.Once);
            editOutput.Verify(candidate => candidate.DisposeAsync(), Times.Never);

            await workspace.DisposeAsync();
            editOutput.Verify(candidate => candidate.DisposeAsync(), Times.Once);
        }
        finally
        {
            directory.Delete(true);
        }
    }

    /// <summary>Verifies a successful plugin no-op advances revision, exact replay returns it, and differing reuse is rejected.</summary>
    [Fact]
    public async Task ApplyFormListEditAsync_WhenOperationRepeats_ReplaysOrRejectsByCanonicalPayload()
    {
        var directory = Directory.CreateTempSubdirectory();
        try
        {
            var source = CreateAsyncDisposableMock<IPluginSourceSet>();
            var initialOutput = CreateAsyncDisposableMock<IPluginOutputState>();
            var editOutput = CreateAsyncDisposableMock<IPluginOutputState>();
            var appliedOutput = CreateAsyncDisposableMock<IPluginOutputState>();
            var adapter = CreateAdapter(directory, source.Object);
            var workspace = await OpenWorkspaceAsync(directory, adapter, Mock.Of<IWorkspaceSaveCoordinator>());
            await SelectOutputAsync(directory, workspace, adapter, initialOutput.Object);
            var target = CreateFormKey("Output.esp", 0x801);
            var editId = Guid.NewGuid();
            adapter.SetupSequence(candidate => candidate.CloneOutput(
                    It.IsAny<IPluginOutputState>(),
                    It.IsAny<CancellationToken>()))
                .Returns(editOutput.Object)
                .Returns(appliedOutput.Object);
            adapter.Setup(candidate => candidate.BeginEdit(
                    It.IsAny<IPluginSourceSet>(),
                    editOutput.Object,
                    It.IsAny<BeginEditRequest>(),
                    It.IsAny<CancellationToken>()))
                .Returns(EngineResult<RecordEditIdentity>.Success(
                    new RecordEditIdentity(editId, target, null, FormListEditRole.New)));
            adapter.Setup(candidate => candidate.PrepareEdit(It.IsAny<FormListEdit>()))
                .Returns<FormListEdit>(edit => new TestPreparedFormListEdit(
                    edit is SetEditorIdEdit setEditorId ? $"set:{setEditorId.EditorId}" : edit.CommandName));
            adapter.Setup(candidate => candidate.ApplyEdit(
                    It.IsAny<IPluginSourceSet>(),
                    appliedOutput.Object,
                    target,
                    It.IsAny<PreparedFormListEdit>()))
                .Returns(EngineResult<RecordEditMutationResult>.Success(
                    new RecordEditMutationResult(changed: false)));
            var begin = await workspace.BeginEditAsync(new BeginEditRequest(
                Guid.NewGuid(),
                workspace.Revision,
                FormListEditRole.New));
            begin.Succeeded.ShouldBeTrue();
            var operationId = Guid.NewGuid();
            var request = new FormListEditRequest(
                operationId,
                workspace.Revision,
                editId,
                new SetEditorIdEdit("Stable"));
            var revisionBeforeApply = workspace.Revision;

            var first = await workspace.ApplyFormListEditAsync(request);
            var replay = await workspace.ApplyFormListEditAsync(request);
            var conflict = await workspace.ApplyFormListEditAsync(new FormListEditRequest(
                operationId,
                request.ExpectedRevision,
                editId,
                new SetEditorIdEdit("Different")));

            first.Succeeded.ShouldBeTrue();
            workspace.Revision.ShouldBe(revisionBeforeApply.Next());
            replay.ShouldBeSameAs(first);
            conflict.Succeeded.ShouldBeFalse();
            conflict.Error.ShouldNotBeNull();
            conflict.Error.Code.ShouldBe(EngineErrorCode.OperationIdReuse);
            adapter.Verify(candidate => candidate.ApplyEdit(
                It.IsAny<IPluginSourceSet>(),
                It.IsAny<IPluginOutputState>(),
                It.IsAny<FormKey>(),
                It.IsAny<PreparedFormListEdit>()), Times.Once);

            await workspace.DisposeAsync();
        }
        finally
        {
            directory.Delete(true);
        }
    }

    /// <summary>Verifies malformed UTF-16 is rejected with a replayable complete identity while distinct invalid payloads conflict.</summary>
    [Fact]
    public async Task ApplyFormListEditAsync_WithMalformedUtf16_ReplaysRejectionWithoutIdentityCollapse()
    {
        var directory = Directory.CreateTempSubdirectory();
        try
        {
            var source = CreateAsyncDisposableMock<IPluginSourceSet>();
            var initialOutput = CreateAsyncDisposableMock<IPluginOutputState>();
            var editOutput = CreateAsyncDisposableMock<IPluginOutputState>();
            var adapter = CreateAdapter(directory, source.Object);
            var workspace = await OpenWorkspaceAsync(directory, adapter, Mock.Of<IWorkspaceSaveCoordinator>());
            await SelectOutputAsync(directory, workspace, adapter, initialOutput.Object);
            var target = CreateFormKey("Output.esp", 0x802);
            var editId = Guid.NewGuid();
            adapter.Setup(candidate => candidate.CloneOutput(
                    It.IsAny<IPluginOutputState>(),
                    It.IsAny<CancellationToken>()))
                .Returns(editOutput.Object);
            adapter.Setup(candidate => candidate.BeginEdit(
                    It.IsAny<IPluginSourceSet>(),
                    editOutput.Object,
                    It.IsAny<BeginEditRequest>(),
                    It.IsAny<CancellationToken>()))
                .Returns(EngineResult<RecordEditIdentity>.Success(
                    new RecordEditIdentity(editId, target, null, FormListEditRole.New)));
            adapter.Setup(candidate => candidate.PrepareEdit(It.IsAny<FormListEdit>()))
                .Returns<FormListEdit>(edit => PrepareEditorIdEdit((SetEditorIdEdit)edit));
            var begin = await workspace.BeginEditAsync(new BeginEditRequest(
                Guid.NewGuid(),
                workspace.Revision,
                FormListEditRole.New));
            begin.Succeeded.ShouldBeTrue(begin.Error?.Message);
            var revision = workspace.Revision;
            var operationId = Guid.NewGuid();
            var malformed = string.Concat("A", (char)0xD800, "B");
            var distinctMalformed = string.Concat("A", (char)0xD801, "B");
            var request = new FormListEditRequest(
                operationId,
                revision,
                editId,
                new SetEditorIdEdit(malformed));

            var first = await workspace.ApplyFormListEditAsync(request);
            var replay = await workspace.ApplyFormListEditAsync(request);
            var conflict = await workspace.ApplyFormListEditAsync(new FormListEditRequest(
                operationId,
                revision,
                editId,
                new SetEditorIdEdit(distinctMalformed)));

            first.Error!.Code.ShouldBe(EngineErrorCode.ValidationFailed);
            replay.ShouldBeSameAs(first);
            conflict.Error!.Code.ShouldBe(EngineErrorCode.OperationIdReuse);
            workspace.Revision.ShouldBe(revision);
            adapter.Verify(candidate => candidate.ApplyEdit(
                It.IsAny<IPluginSourceSet>(),
                It.IsAny<IPluginOutputState>(),
                It.IsAny<FormKey>(),
                It.IsAny<PreparedFormListEdit>()), Times.Never);

            await workspace.DisposeAsync();
        }
        finally
        {
            directory.Delete(true);
        }
    }

    /// <summary>Verifies disposal waits for an in-flight plugin read before releasing its source handle.</summary>
    [Fact]
    public async Task DisposeAsync_DuringRead_WaitsForOperationBeforeDisposingSources()
    {
        var directory = Directory.CreateTempSubdirectory();
        try
        {
            var source = CreateAsyncDisposableMock<IPluginSourceSet>();
            var adapter = CreateAdapter(directory, source.Object);
            var started = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            adapter.Setup(candidate => candidate.ListPlugins(
                    It.IsAny<IPluginSourceSet>(),
                    It.IsAny<IPluginOutputState?>(),
                    It.IsAny<CancellationToken>()))
                .Returns(() =>
                {
                    started.SetResult();
                    release.Task.GetAwaiter().GetResult();
                    return EngineResult<IReadOnlyList<PluginSummary>>.Success(
                        Array.AsReadOnly(Array.Empty<PluginSummary>()));
                });
            var workspace = await OpenWorkspaceAsync(directory, adapter, Mock.Of<IWorkspaceSaveCoordinator>());

            var readTask = workspace.ListPluginsAsync().AsTask();
            await started.Task;
            var disposeTask = workspace.DisposeAsync().AsTask();
            await Task.Delay(50);

            disposeTask.IsCompleted.ShouldBeFalse();
            source.Verify(candidate => candidate.DisposeAsync(), Times.Never);

            release.SetResult();
            var readResult = await readTask;
            await disposeTask;

            readResult.Succeeded.ShouldBeTrue();
            source.Verify(candidate => candidate.DisposeAsync(), Times.Once);
        }
        finally
        {
            directory.Delete(true);
        }
    }

    /// <summary>Verifies reads after disposal return a stable typed failure without entering the adapter.</summary>
    [Fact]
    public async Task ListPluginsAsync_AfterDispose_ReturnsWorkspaceDisposed()
    {
        var directory = Directory.CreateTempSubdirectory();
        try
        {
            var source = CreateAsyncDisposableMock<IPluginSourceSet>();
            var adapter = CreateAdapter(directory, source.Object);
            var workspace = await OpenWorkspaceAsync(directory, adapter, Mock.Of<IWorkspaceSaveCoordinator>());
            await workspace.DisposeAsync();

            var result = await workspace.ListPluginsAsync();

            result.Succeeded.ShouldBeFalse();
            result.Error.ShouldNotBeNull();
            result.Error.Code.ShouldBe(EngineErrorCode.WorkspaceDisposed);
            adapter.Verify(candidate => candidate.ListPlugins(
                It.IsAny<IPluginSourceSet>(),
                It.IsAny<IPluginOutputState?>(),
                It.IsAny<CancellationToken>()), Times.Never);
        }
        finally
        {
            directory.Delete(true);
        }
    }

    /// <summary>Verifies reference search receives the exact serialized workspace identity, revision, and cancellable token.</summary>
    [Fact]
    public async Task SearchReferencesAsync_ForwardsCurrentWorkspaceContextAndCancellation()
    {
        var directory = Directory.CreateTempSubdirectory();
        try
        {
            var source = CreateAsyncDisposableMock<IPluginSourceSet>();
            var output = CreateAsyncDisposableMock<IPluginOutputState>();
            var adapter = CreateAdapter(directory, source.Object);
            var workspace = await OpenWorkspaceAsync(directory, adapter, Mock.Of<IWorkspaceSaveCoordinator>());
            await SelectOutputAsync(directory, workspace, adapter, output.Object);
            var request = new ReferenceSearchRequest("target", 10);
            var expectedRevision = workspace.Revision;
            var observedWorkspaceId = Guid.Empty;
            var observedRevision = default(WorkspaceRevision);
            var observedCancellationToken = default(CancellationToken);
            using var cancellationSource = new CancellationTokenSource();
            adapter.Setup(candidate => candidate.SearchReferences(
                    source.Object,
                    output.Object,
                    request,
                    It.IsAny<Guid>(),
                    It.IsAny<WorkspaceRevision>(),
                    It.IsAny<CancellationToken>()))
                .Returns<IPluginSourceSet, IPluginOutputState?, ReferenceSearchRequest, Guid, WorkspaceRevision, CancellationToken>(
                    (_, _, _, workspaceId, revision, cancellationToken) =>
                    {
                        observedWorkspaceId = workspaceId;
                        observedRevision = revision;
                        observedCancellationToken = cancellationToken;
                        cancellationSource.Cancel();
                        cancellationToken.ThrowIfCancellationRequested();
                        return EngineResult<ReferenceSearchPage>.Success(
                            new ReferenceSearchPage(Array.Empty<ReferenceSearchMatch>(), null));
                    });

            await Should.ThrowAsync<OperationCanceledException>(async () =>
                await workspace.SearchReferencesAsync(request, cancellationSource.Token));

            observedWorkspaceId.ShouldBe(workspace.WorkspaceId);
            observedRevision.ShouldBe(expectedRevision);
            observedCancellationToken.ShouldBe(cancellationSource.Token);
            workspace.Revision.ShouldBe(expectedRevision);
            adapter.Verify(candidate => candidate.SearchReferences(
                source.Object,
                output.Object,
                request,
                workspace.WorkspaceId,
                expectedRevision,
                cancellationSource.Token), Times.Once);

            await workspace.DisposeAsync();
        }
        finally
        {
            directory.Delete(true);
        }
    }

    /// <summary>Verifies reference resolution receives the caller's cancellation token through the serialized read path.</summary>
    [Fact]
    public async Task ResolveReferenceAsync_ForwardsCancellationToken()
    {
        var directory = Directory.CreateTempSubdirectory();
        try
        {
            var source = CreateAsyncDisposableMock<IPluginSourceSet>();
            var adapter = CreateAdapter(directory, source.Object);
            var workspace = await OpenWorkspaceAsync(directory, adapter, Mock.Of<IWorkspaceSaveCoordinator>());
            var request = new ReferenceRequest(CreateFormKey("Master.esm", 0x123), RecordScope.Source);
            var resolution = new ReferenceResolution(
                ReferenceResolutionStatus.Unresolved,
                request.FormKey,
                null,
                null);
            using var cancellationSource = new CancellationTokenSource();
            adapter.Setup(candidate => candidate.ResolveReference(
                    source.Object,
                    null,
                    request,
                    cancellationSource.Token))
                .Returns(EngineResult<ReferenceResolution>.Success(resolution));

            var result = await workspace.ResolveReferenceAsync(request, cancellationSource.Token);

            result.Succeeded.ShouldBeTrue(result.Error?.Message);
            result.Value.ShouldBeSameAs(resolution);
            adapter.Verify(candidate => candidate.ResolveReference(
                source.Object,
                null,
                request,
                cancellationSource.Token), Times.Once);

            await workspace.DisposeAsync();
        }
        finally
        {
            directory.Delete(true);
        }
    }

    /// <summary>Verifies cancellation observed after output acquisition disposes the unpublished output.</summary>
    [Fact]
    public async Task SelectOutputAsync_WhenCanceledAfterAcquisition_DisposesUnpublishedOutput()
    {
        var directory = Directory.CreateTempSubdirectory();
        try
        {
            var source = CreateAsyncDisposableMock<IPluginSourceSet>();
            var output = CreateAsyncDisposableMock<IPluginOutputState>();
            var adapter = CreateAdapter(directory, source.Object);
            var workspace = await OpenWorkspaceAsync(directory, adapter, Mock.Of<IWorkspaceSaveCoordinator>());
            using var cancellationSource = new CancellationTokenSource();
            var association = CreateOutputAssociation(directory);
            var baseline = CreateBaseline(association.PluginPath, Guid.NewGuid(), false);
            adapter.Setup(candidate => candidate.OpenOutputAsync(
                    It.IsAny<IPluginSourceSet>(),
                    It.IsAny<SelectOutputRequest>(),
                    cancellationSource.Token))
                .Returns(() =>
                {
                    cancellationSource.Cancel();
                    return ValueTask.FromResult(EngineResult<PluginOutputOpenResult>.Success(
                        new PluginOutputOpenResult(output.Object, association, baseline)));
                });
            var revision = workspace.Revision;

            await Should.ThrowAsync<OperationCanceledException>(async () =>
                await workspace.SelectOutputAsync(
                    new SelectOutputRequest(
                        Guid.NewGuid(),
                        revision,
                        OutputSelectionMode.CreateNew,
                        association),
                    cancellationSource.Token));

            workspace.Revision.ShouldBe(revision);
            output.Verify(candidate => candidate.DisposeAsync(), Times.Once);
            await workspace.DisposeAsync();
        }
        finally
        {
            directory.Delete(true);
        }
    }

    /// <summary>Verifies invalid-path rejection replays exactly and reserves its operation identifier.</summary>
    [Fact]
    public async Task SelectOutputAsync_WhenPathCanonicalizationFails_ReplaysOrRejectsOperationReuse()
    {
        var directory = Directory.CreateTempSubdirectory();
        try
        {
            var source = CreateAsyncDisposableMock<IPluginSourceSet>();
            var adapter = CreateAdapter(directory, source.Object);
            var workspace = await OpenWorkspaceAsync(directory, adapter, Mock.Of<IWorkspaceSaveCoordinator>());
            var operationId = Guid.NewGuid();
            var revision = workspace.Revision;
            var invalidAssociation = new OutputAssociation(
                directory.FullName + '\0' + Path.DirectorySeparatorChar + "Output.esp",
                CreateModKey("Output.esp"),
                LocalizedOutputMode.Embedded,
                OutputMasterStyle.Full);
            var invalidRequest = new SelectOutputRequest(
                operationId,
                revision,
                OutputSelectionMode.CreateNew,
                invalidAssociation);

            var first = await workspace.SelectOutputAsync(invalidRequest);
            var replay = await workspace.SelectOutputAsync(invalidRequest);
            var conflict = await workspace.SelectOutputAsync(new SelectOutputRequest(
                operationId,
                revision,
                OutputSelectionMode.CreateNew,
                CreateOutputAssociation(directory)));

            first.Succeeded.ShouldBeFalse();
            first.Error.ShouldNotBeNull();
            first.Error.Code.ShouldBe(EngineErrorCode.InvalidRequest);
            replay.ShouldBeSameAs(first);
            conflict.Succeeded.ShouldBeFalse();
            conflict.Error.ShouldNotBeNull();
            conflict.Error.Code.ShouldBe(EngineErrorCode.OperationIdReuse);
            adapter.Verify(candidate => candidate.OpenOutputAsync(
                It.IsAny<IPluginSourceSet>(),
                It.IsAny<SelectOutputRequest>(),
                It.IsAny<CancellationToken>()), Times.Never);

            await workspace.DisposeAsync();
        }
        finally
        {
            directory.Delete(true);
        }
    }

    /// <summary>Verifies revision baseline composition is stable for the same source and output and distinct for another output.</summary>
    [Fact]
    public async Task SelectOutputAsync_ComposesDeterministicSourceAndOutputRevisionBaseline()
    {
        var directory = Directory.CreateTempSubdirectory();
        try
        {
            var firstSource = CreateAsyncDisposableMock<IPluginSourceSet>();
            var secondSource = CreateAsyncDisposableMock<IPluginSourceSet>();
            var differentSourceHandle = CreateAsyncDisposableMock<IPluginSourceSet>();
            var firstOutput = CreateAsyncDisposableMock<IPluginOutputState>();
            var secondOutput = CreateAsyncDisposableMock<IPluginOutputState>();
            var differentOutput = CreateAsyncDisposableMock<IPluginOutputState>();
            var firstAdapter = CreateAdapter(directory, firstSource.Object);
            var secondAdapter = CreateAdapter(directory, secondSource.Object);
            var differentAdapter = CreateAdapter(directory, differentSourceHandle.Object);
            var firstWorkspace = await OpenWorkspaceAsync(directory, firstAdapter, Mock.Of<IWorkspaceSaveCoordinator>());
            var secondWorkspace = await OpenWorkspaceAsync(directory, secondAdapter, Mock.Of<IWorkspaceSaveCoordinator>());
            var differentWorkspace = await OpenWorkspaceAsync(directory, differentAdapter, Mock.Of<IWorkspaceSaveCoordinator>());
            var sharedAssociation = CreateOutputAssociation(directory);
            var sharedBaselineId = new Guid("f958781b-5cf7-4511-b8a9-af7886d53209");
            var sharedBaseline = CreateBaseline(sharedAssociation.PluginPath, sharedBaselineId, false);
            var differentAssociation = CreateOutputAssociation(directory, "Different.esp");
            var differentBaseline = CreateBaseline(differentAssociation.PluginPath, sharedBaselineId, false);

            var first = await SelectOutputAsync(
                firstWorkspace,
                firstAdapter,
                firstOutput.Object,
                sharedAssociation,
                sharedBaseline);
            var second = await SelectOutputAsync(
                secondWorkspace,
                secondAdapter,
                secondOutput.Object,
                sharedAssociation,
                sharedBaseline);
            var different = await SelectOutputAsync(
                differentWorkspace,
                differentAdapter,
                differentOutput.Object,
                differentAssociation,
                differentBaseline);

            first.Value!.Revision.Sequence.ShouldBe(1UL);
            second.Value!.Revision.BaselineId.ShouldBe(first.Value.Revision.BaselineId);
            different.Value!.Revision.BaselineId.ShouldNotBe(first.Value.Revision.BaselineId);

            await firstWorkspace.DisposeAsync();
            await secondWorkspace.DisposeAsync();
            await differentWorkspace.DisposeAsync();
        }
        finally
        {
            directory.Delete(true);
        }
    }

    /// <summary>Verifies a committed save advances once, adopts its complete baseline, and replays exactly.</summary>
    [Fact]
    public async Task SaveAsync_WhenCommitted_AdvancesRevisionAndReplaysResult()
    {
        var directory = Directory.CreateTempSubdirectory();
        try
        {
            var source = CreateAsyncDisposableMock<IPluginSourceSet>();
            var output = CreateAsyncDisposableMock<IPluginOutputState>();
            var reopenedOutput = CreateAsyncDisposableMock<IPluginOutputState>();
            var adapter = CreateAdapter(directory, source.Object);
            var saveCoordinator = new Mock<IWorkspaceSaveCoordinator>();
            var workspace = await OpenWorkspaceAsync(directory, adapter, saveCoordinator.Object);
            var selection = await SelectOutputAsync(directory, workspace, adapter, output.Object);
            var savedBaseline = CreateBaseline(selection.Value!.Output.PluginPath, Guid.NewGuid(), true);
            var operationId = Guid.NewGuid();
            var saveRequest = new SaveRequest(operationId, workspace.Revision, selection.Value.Baseline);
            adapter.Setup(candidate => candidate.ReopenOutputAsync(
                    source.Object,
                    selection.Value.Output,
                    savedBaseline,
                    It.IsAny<CancellationToken>()))
                .Returns(ValueTask.FromResult(EngineResult<PluginOutputOpenResult>.Success(
                    new PluginOutputOpenResult(reopenedOutput.Object, selection.Value.Output, savedBaseline))));
            saveCoordinator.Setup(candidate => candidate.SaveAsync(
                    It.IsAny<WorkspaceSaveContext>(),
                    saveRequest,
                    It.IsAny<CancellationToken>()))
                .Returns<WorkspaceSaveContext, SaveRequest, CancellationToken>((context, _, _) =>
                {
                    var evidenceToken = new RecoveryEvidenceToken("committed-save");
                    var evidence = new ResolvedOutputEvidence(
                        evidenceToken,
                        context.Game,
                        context.Release,
                        context.WorkspaceId,
                        operationId,
                        context.Revision,
                        context.Sources.Baseline,
                        context.OutputAssociation,
                        savedBaseline,
                        RecoverSaveStatus.Committed);
                    return ValueTask.FromResult(new SaveResult(
                        context.WorkspaceId,
                        operationId,
                        context.Revision,
                        context.Revision,
                        SaveCommitStatus.Committed,
                        savedBaseline,
                        evidenceToken,
                        evidence,
                        null,
                        Array.Empty<EngineWarning>()));
                });

            var first = await workspace.SaveAsync(saveRequest);
            var replay = await workspace.SaveAsync(saveRequest);

            first.Status.ShouldBe(SaveCommitStatus.Committed);
            first.ResultRevision.Sequence.ShouldBe(saveRequest.ExpectedRevision.Sequence + 1);
            first.ResultRevision.BaselineId.ShouldNotBe(saveRequest.ExpectedRevision.BaselineId);
            workspace.Revision.ShouldBe(first.ResultRevision);
            replay.ShouldBeSameAs(first);
            saveCoordinator.Verify(candidate => candidate.SaveAsync(
                It.IsAny<WorkspaceSaveContext>(),
                saveRequest,
                It.IsAny<CancellationToken>()), Times.Once);

            await workspace.DisposeAsync();
        }
        finally
        {
            directory.Delete(true);
        }
    }

    /// <summary>Verifies reopening the same complete output observation preserves its composed baseline, advances once, and replays exactly.</summary>
    [Fact]
    public async Task ReopenOutputAsync_WhenOutputObservationIsUnchanged_PreservesBaselineAndReplaysResult()
    {
        var directory = Directory.CreateTempSubdirectory();
        try
        {
            var source = CreateAsyncDisposableMock<IPluginSourceSet>();
            var output = CreateAsyncDisposableMock<IPluginOutputState>();
            var reopenedOutput = CreateAsyncDisposableMock<IPluginOutputState>();
            var adapter = CreateAdapter(directory, source.Object);
            var workspace = await OpenWorkspaceAsync(directory, adapter, Mock.Of<IWorkspaceSaveCoordinator>());
            var selection = await SelectOutputAsync(directory, workspace, adapter, output.Object);
            var selectedOutput = selection.Value!;
            var revisionBeforeReopen = workspace.Revision;
            adapter.Setup(candidate => candidate.ReopenOutputAsync(
                    It.IsAny<IPluginSourceSet>(),
                    selectedOutput.Output,
                    selectedOutput.Baseline,
                    It.IsAny<CancellationToken>()))
                .Returns(ValueTask.FromResult(EngineResult<PluginOutputOpenResult>.Success(
                    new PluginOutputOpenResult(reopenedOutput.Object, selectedOutput.Output, selectedOutput.Baseline))));
            var request = new ReopenOutputRequest(
                Guid.NewGuid(),
                revisionBeforeReopen,
                selectedOutput.Baseline);

            var first = await workspace.ReopenOutputAsync(request);
            var replay = await workspace.ReopenOutputAsync(request);

            first.Succeeded.ShouldBeTrue(first.Error?.Message);
            first.Value!.Revision.BaselineId.ShouldBe(revisionBeforeReopen.BaselineId);
            first.Value.Revision.Sequence.ShouldBe(revisionBeforeReopen.Sequence + 1);
            workspace.Revision.ShouldBe(first.Value.Revision);
            replay.ShouldBeSameAs(first);
            adapter.Verify(candidate => candidate.ReopenOutputAsync(
                It.IsAny<IPluginSourceSet>(),
                selectedOutput.Output,
                selectedOutput.Baseline,
                It.IsAny<CancellationToken>()), Times.Once);

            await workspace.DisposeAsync();
        }
        finally
        {
            directory.Delete(true);
        }
    }

    /// <summary>Verifies discard republishes the complete output observation with its composed baseline and exact replay receipt.</summary>
    [Fact]
    public async Task DiscardChangesAsync_WhenOutputObservationIsUnchanged_PreservesBaselineAndReplaysResult()
    {
        var directory = Directory.CreateTempSubdirectory();
        try
        {
            var source = CreateAsyncDisposableMock<IPluginSourceSet>();
            var output = CreateAsyncDisposableMock<IPluginOutputState>();
            var reopenedOutput = CreateAsyncDisposableMock<IPluginOutputState>();
            var adapter = CreateAdapter(directory, source.Object);
            var workspace = await OpenWorkspaceAsync(directory, adapter, Mock.Of<IWorkspaceSaveCoordinator>());
            var selection = await SelectOutputAsync(directory, workspace, adapter, output.Object);
            var selectedOutput = selection.Value!;
            var revisionBeforeDiscard = workspace.Revision;
            adapter.Setup(candidate => candidate.ReopenOutputAsync(
                    It.IsAny<IPluginSourceSet>(),
                    selectedOutput.Output,
                    selectedOutput.Baseline,
                    It.IsAny<CancellationToken>()))
                .Returns(ValueTask.FromResult(EngineResult<PluginOutputOpenResult>.Success(
                    new PluginOutputOpenResult(reopenedOutput.Object, selectedOutput.Output, selectedOutput.Baseline))));
            var request = new DiscardChangesRequest(
                Guid.NewGuid(),
                revisionBeforeDiscard,
                selectedOutput.Baseline);

            var first = await workspace.DiscardChangesAsync(request);
            var replay = await workspace.DiscardChangesAsync(request);

            first.Succeeded.ShouldBeTrue(first.Error?.Message);
            first.Value!.Revision.BaselineId.ShouldBe(revisionBeforeDiscard.BaselineId);
            first.Value.Revision.Sequence.ShouldBe(revisionBeforeDiscard.Sequence + 1);
            workspace.Revision.ShouldBe(first.Value.Revision);
            replay.ShouldBeSameAs(first);
            adapter.Verify(candidate => candidate.ReopenOutputAsync(
                It.IsAny<IPluginSourceSet>(),
                selectedOutput.Output,
                selectedOutput.Baseline,
                It.IsAny<CancellationToken>()), Times.Once);

            await workspace.DisposeAsync();
        }
        finally
        {
            directory.Delete(true);
        }
    }

    /// <summary>Verifies matching baseline identity cannot conceal different artifact content.</summary>
    [Fact]
    public async Task ReopenOutputAsync_WhenArtifactContentDiffersUnderSameBaselineId_ReturnsExternalChange()
    {
        var directory = Directory.CreateTempSubdirectory();
        try
        {
            var source = CreateAsyncDisposableMock<IPluginSourceSet>();
            var output = CreateAsyncDisposableMock<IPluginOutputState>();
            var adapter = CreateAdapter(directory, source.Object);
            var workspace = await OpenWorkspaceAsync(directory, adapter, Mock.Of<IWorkspaceSaveCoordinator>());
            var selection = await SelectOutputAsync(directory, workspace, adapter, output.Object);
            var conflictingBaseline = CreateBaseline(
                selection.Value!.Output.PluginPath,
                selection.Value.Baseline.BaselineId,
                true);

            var result = await workspace.ReopenOutputAsync(new ReopenOutputRequest(
                Guid.NewGuid(),
                workspace.Revision,
                conflictingBaseline));

            result.Succeeded.ShouldBeFalse();
            result.Error.ShouldNotBeNull();
            result.Error.Code.ShouldBe(EngineErrorCode.ExternalChangeDetected);
            adapter.Verify(candidate => candidate.ReopenOutputAsync(
                It.IsAny<IPluginSourceSet>(),
                It.IsAny<OutputAssociation>(),
                It.IsAny<OutputArtifactSetBaseline>(),
                It.IsAny<CancellationToken>()), Times.Never);

            await workspace.DisposeAsync();
        }
        finally
        {
            directory.Delete(true);
        }
    }

    /// <summary>Defensively fingerprints one synthetic EditorID edit through the production canonical writer.</summary>
    /// <param name="edit">The caller-owned typed edit to snapshot.</param>
    /// <returns>A prepared edit containing the complete fingerprint and deterministic validation result.</returns>
    private static PreparedFormListEdit PrepareEditorIdEdit(SetEditorIdEdit edit)
    {
        var result = RecordEditFingerprintFactory.Create(
            edit.CommandName,
            (writer, context) =>
            {
                writer.WriteStartObject();
                writer.WritePropertyName("editorId");
                RecordJsonLeafWriter.WriteString(writer, edit.EditorId, context);
                writer.WriteEndObject();
            });
        return new TestPreparedFormListEdit(result.Fingerprint, result.ValidationError);
    }

    /// <summary>Creates and opens a workspace through the production factory with synthetic plugin state.</summary>
    private static async Task<IPluginWorkspace> OpenWorkspaceAsync(
        DirectoryInfo directory,
        Mock<IFormListGameAdapter> adapter,
        IWorkspaceSaveCoordinator saveCoordinator)
    {
        var request = CreateOpenRequest(directory);
        TestWorkspaceInfrastructure.ConfigureReadyAdmission(saveCoordinator);
        var factory = new Core.Engine.PluginWorkspaceFactory(
            [adapter.Object],
            saveCoordinator,
            TestWorkspaceInfrastructure.CreateLeaseProvider(),
            Mock.Of<ILogger>());
        var result = await factory.OpenAsync(request);
        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        result.Value.ShouldNotBeNull();
        return result.Value;
    }

    /// <summary>Creates a valid explicit workspace-open request and disposable synthetic plugin files.</summary>
    private static WorkspaceOpenRequest CreateOpenRequest(DirectoryInfo directory)
    {
        var dataDirectory = directory.CreateSubdirectory("Data");
        var stringsDirectory = directory.CreateSubdirectory("Strings");
        var masterPath = Path.Combine(dataDirectory.FullName, "Master.esm");
        var sourcePath = Path.Combine(dataDirectory.FullName, "Source.esp");
        File.WriteAllBytes(masterPath, Array.Empty<byte>());
        File.WriteAllBytes(sourcePath, Array.Empty<byte>());
        return new WorkspaceOpenRequest(
            Guid.NewGuid(),
            SupportedGame.Starfield,
            GameRelease.Starfield,
            sourcePath,
            [masterPath, sourcePath],
            dataDirectory.FullName,
            [stringsDirectory.FullName]);
    }

    /// <summary>Creates a loose game adapter with successful explicit source opening.</summary>
    private static Mock<IFormListGameAdapter> CreateAdapter(DirectoryInfo directory, IPluginSourceSet sources)
    {
        var baselineId = new Guid("6195dcd8-7cab-49ba-b25a-56af1a86a4e7");
        TestWorkspaceInfrastructure.ConfigureSourceBaseline(
            sources,
            baselineId,
            Path.Combine(directory.FullName, "Data", "Source.esp"));
        var adapter = new Mock<IFormListGameAdapter>();
        adapter.SetupGet(candidate => candidate.Game).Returns(SupportedGame.Starfield);
        adapter.Setup(candidate => candidate.SupportsRelease(GameRelease.Starfield)).Returns(true);
        adapter.Setup(candidate => candidate.OpenSourcesAsync(
                It.IsAny<WorkspaceOpenRequest>(),
                It.IsAny<CancellationToken>()))
            .Returns(ValueTask.FromResult(EngineResult<PluginSourceOpenResult>.Success(
                new PluginSourceOpenResult(
                    sources,
                    baselineId,
                    Array.AsReadOnly(new[]
                    {
                        new PluginArtifactAssociation(
                            Path.Combine(directory.FullName, "Data", "Source.esp"),
                            PluginArtifactRole.Plugin,
                            null,
                            new PluginArtifactFingerprint(true, 0, new string('A', 64)),
                            new ArtifactFileIdentity("test", "volume", "source", 1))
                    })))));
        return adapter;
    }

    /// <summary>Selects a generated synthetic output observation and returns its receipt.</summary>
    /// <param name="directory">The temporary root containing the synthetic output.</param>
    /// <param name="workspace">The open workspace that receives the output.</param>
    /// <param name="adapter">The synthetic adapter used to open the output.</param>
    /// <param name="output">The disposable plugin output handle to publish.</param>
    /// <returns>The successful output-selection result.</returns>
    private static async Task<EngineResult<OutputSelectionReceipt>> SelectOutputAsync(
        DirectoryInfo directory,
        IPluginWorkspace workspace,
        Mock<IFormListGameAdapter> adapter,
        IPluginOutputState output)
    {
        var association = CreateOutputAssociation(directory);
        var baseline = CreateBaseline(association.PluginPath, Guid.NewGuid(), false);
        return await SelectOutputAsync(workspace, adapter, output, association, baseline);
    }

    /// <summary>Selects a supplied synthetic output observation and returns its receipt.</summary>
    /// <param name="workspace">The open workspace that receives the output.</param>
    /// <param name="adapter">The synthetic adapter used to open the output.</param>
    /// <param name="output">The disposable plugin output handle to publish.</param>
    /// <param name="association">The canonical output association to publish.</param>
    /// <param name="baseline">The complete output artifact-set observation to publish.</param>
    /// <returns>The successful output-selection result.</returns>
    private static async Task<EngineResult<OutputSelectionReceipt>> SelectOutputAsync(
        IPluginWorkspace workspace,
        Mock<IFormListGameAdapter> adapter,
        IPluginOutputState output,
        OutputAssociation association,
        OutputArtifactSetBaseline baseline)
    {
        adapter.Setup(candidate => candidate.OpenOutputAsync(
                It.IsAny<IPluginSourceSet>(),
                It.IsAny<SelectOutputRequest>(),
                It.IsAny<CancellationToken>()))
            .Returns(ValueTask.FromResult(EngineResult<PluginOutputOpenResult>.Success(
                new PluginOutputOpenResult(output, association, baseline))));
        var result = await workspace.SelectOutputAsync(new SelectOutputRequest(
            Guid.NewGuid(),
            workspace.Revision,
            OutputSelectionMode.CreateNew,
            association));
        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        return result;
    }

    /// <summary>Creates a canonical synthetic output association.</summary>
    /// <param name="directory">The temporary root containing the synthetic output.</param>
    /// <param name="pluginFileName">The output plugin file name.</param>
    /// <returns>The canonical synthetic output association.</returns>
    private static OutputAssociation CreateOutputAssociation(
        DirectoryInfo directory,
        string pluginFileName = "Output.esp")
    {
        return new OutputAssociation(
            Path.Combine(directory.FullName, pluginFileName),
            CreateModKey(pluginFileName),
            LocalizedOutputMode.Embedded,
            OutputMasterStyle.Full);
    }

    /// <summary>Creates a complete synthetic output baseline.</summary>
    private static OutputArtifactSetBaseline CreateBaseline(string path, Guid baselineId, bool exists)
    {
        return new OutputArtifactSetBaseline(
            baselineId,
            Array.AsReadOnly(new[]
            {
                new PluginArtifactAssociation(
                    path,
                    PluginArtifactRole.Plugin,
                    null,
                    exists
                        ? new PluginArtifactFingerprint(true, 1, new string('B', 64))
                        : new PluginArtifactFingerprint(false, 0, null),
                    exists ? new ArtifactFileIdentity("test", "volume", "output", 1) : null)
            }));
    }

    /// <summary>Creates a plugin FormKey for synthetic tests.</summary>
    private static FormKey CreateFormKey(string pluginFileName, uint id)
    {
        return new FormKey(CreateModKey(pluginFileName), id);
    }

    /// <summary>Parses a plugin ModKey for synthetic tests.</summary>
    private static ModKey CreateModKey(string pluginFileName)
    {
        ModKey.TryFromNameAndExtension(pluginFileName, out var modKey, out var error).ShouldBeTrue(error);
        return modKey;
    }

    /// <summary>Creates an independently verifiable asynchronous plugin lifetime mock.</summary>
    private static Mock<T> CreateAsyncDisposableMock<T>()
        where T : class, IAsyncDisposable
    {
        var value = new Mock<T>();
        value.Setup(candidate => candidate.DisposeAsync()).Returns(ValueTask.CompletedTask);
        return value;
    }
}
