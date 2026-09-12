using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Enums;
using Moq;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Serilog;
using Shouldly;

namespace CreationsForge.UnitTests.Engine.Foundation;

/// <summary>Verifies output selection and staged FormList allocation identity, replay, and cancellation boundaries.</summary>
public sealed class FormListWorkspaceAllocationTests
{
    /// <summary>Verifies create-new and open-existing requests cannot share an idempotency operation identity.</summary>
    [Fact]
    public async Task SelectOutputAsync_WhenModeChanges_RejectsOperationReuse()
    {
        var directory = Directory.CreateTempSubdirectory();
        try
        {
            var source = CreateAsyncDisposableMock<INativeSourceSet>();
            var output = CreateAsyncDisposableMock<INativeOutputState>();
            var adapter = CreateAdapter(directory, source.Object);
            var workspace = await OpenWorkspaceAsync(directory, adapter);
            var association = CreateOutputAssociation(directory);
            var baseline = CreateBaseline(association.PluginPath);
            adapter.Setup(candidate => candidate.OpenOutputAsync(
                    source.Object,
                    It.IsAny<SelectOutputRequest>(),
                    It.IsAny<CancellationToken>()))
                .Returns(ValueTask.FromResult(EngineResult<NativeOutputOpenResult>.Success(
                    new NativeOutputOpenResult(output.Object, association, baseline))));
            var operationId = Guid.NewGuid();
            var revision = workspace.Revision;
            var createRequest = new SelectOutputRequest(
                operationId,
                revision,
                OutputSelectionMode.CreateNew,
                association);

            var first = await workspace.SelectOutputAsync(createRequest);
            var replay = await workspace.SelectOutputAsync(createRequest);
            var conflict = await workspace.SelectOutputAsync(new SelectOutputRequest(
                operationId,
                revision,
                OutputSelectionMode.OpenExisting,
                association));

            first.Succeeded.ShouldBeTrue(first.Error?.Message);
            replay.ShouldBeSameAs(first);
            conflict.Error!.Code.ShouldBe(EngineErrorCode.OperationIdReuse);
            adapter.Verify(candidate => candidate.OpenOutputAsync(
                source.Object,
                It.Is<SelectOutputRequest>(request => request.Mode == OutputSelectionMode.CreateNew),
                It.IsAny<CancellationToken>()), Times.Once);

            await workspace.DisposeAsync();
        }
        finally
        {
            directory.Delete(true);
        }
    }

    /// <summary>Verifies override selectors ignore native filename casing while retaining every semantic identity field.</summary>
    [Fact]
    public async Task BeginEditAsync_WhenOriginSelectorChanges_DistinguishesSemanticChangesFromCasing()
    {
        var directory = Directory.CreateTempSubdirectory();
        try
        {
            var source = CreateAsyncDisposableMock<INativeSourceSet>();
            var initialOutput = CreateAsyncDisposableMock<INativeOutputState>();
            var candidate = CreateAsyncDisposableMock<INativeOutputState>();
            var adapter = CreateAdapter(directory, source.Object);
            var workspace = await OpenWorkspaceAsync(directory, adapter);
            await SelectOutputAsync(directory, workspace, adapter, initialOutput.Object);
            var origin = CreateFormKey("Source.esp", 0x800);
            var editId = Guid.NewGuid();
            adapter.Setup(candidateAdapter => candidateAdapter.CloneOutput(
                    It.IsAny<INativeOutputState>(),
                    It.IsAny<CancellationToken>()))
                .Returns(candidate.Object);
            adapter.Setup(candidateAdapter => candidateAdapter.BeginEdit(
                    source.Object,
                    candidate.Object,
                    It.IsAny<BeginEditRequest>(),
                    It.IsAny<CancellationToken>()))
                .Returns(EngineResult<NativeEditIdentity>.Success(
                    new NativeEditIdentity(editId, origin, origin, FormListEditRole.Override)));
            var operationId = Guid.NewGuid();
            var revision = workspace.Revision;
            var sourceSelection = new ReferenceRequest(origin, RecordScope.Source, origin.ModKey);
            var sourceRequest = new BeginEditRequest(
                operationId,
                revision,
                FormListEditRole.Override,
                origin,
                sourceSelection);
            var caseVariantOrigin = CreateFormKey("source.esp", 0x800);
            var caseVariantSelection = new ReferenceRequest(
                caseVariantOrigin,
                RecordScope.Source,
                ModKey.FromNameAndExtension("SOURCE.esp"));

            var first = await workspace.BeginEditAsync(sourceRequest);
            var replay = await workspace.BeginEditAsync(sourceRequest);
            var caseVariantReplay = await workspace.BeginEditAsync(new BeginEditRequest(
                operationId,
                revision,
                FormListEditRole.Override,
                caseVariantOrigin,
                caseVariantSelection));
            var scopeConflict = await workspace.BeginEditAsync(new BeginEditRequest(
                operationId,
                revision,
                FormListEditRole.Override,
                origin,
                new ReferenceRequest(origin, RecordScope.AllContexts, origin.ModKey)));
            var differentOrigin = CreateFormKey("Source.esp", 0x801);
            var identityConflict = await workspace.BeginEditAsync(new BeginEditRequest(
                operationId,
                revision,
                FormListEditRole.Override,
                differentOrigin,
                new ReferenceRequest(differentOrigin, RecordScope.Source, differentOrigin.ModKey)));

            first.Succeeded.ShouldBeTrue(first.Error?.Message);
            replay.ShouldBeSameAs(first);
            caseVariantOrigin.ModKey.Name.ShouldNotBe(origin.ModKey.Name);
            caseVariantSelection.ContainingModKey!.Value.Name.ShouldNotBe(sourceSelection.ContainingModKey!.Value.Name);
            caseVariantReplay.ShouldBeSameAs(first);
            scopeConflict.Error!.Code.ShouldBe(EngineErrorCode.OperationIdReuse);
            identityConflict.Error!.Code.ShouldBe(EngineErrorCode.OperationIdReuse);
            adapter.Verify(candidateAdapter => candidateAdapter.BeginEdit(
                source.Object,
                candidate.Object,
                sourceRequest,
                It.IsAny<CancellationToken>()), Times.Once);

            await workspace.DisposeAsync();
        }
        finally
        {
            directory.Delete(true);
        }
    }

    /// <summary>Verifies an output-owned target ignores native filename casing and reuses its existing staged edit session without another mutation.</summary>
    [Fact]
    public async Task BeginEditAsync_WhenExistingOutputTargetAlreadyStaged_ReusesSession()
    {
        var directory = Directory.CreateTempSubdirectory();
        try
        {
            var source = CreateAsyncDisposableMock<INativeSourceSet>();
            var initialOutput = CreateAsyncDisposableMock<INativeOutputState>();
            var candidate = CreateAsyncDisposableMock<INativeOutputState>();
            var adapter = CreateAdapter(directory, source.Object);
            var workspace = await OpenWorkspaceAsync(directory, adapter);
            await SelectOutputAsync(directory, workspace, adapter, initialOutput.Object);
            var target = CreateFormKey("Output.esp", 0x800);
            var editId = Guid.NewGuid();
            adapter.Setup(candidateAdapter => candidateAdapter.CloneOutput(
                    It.IsAny<INativeOutputState>(),
                    It.IsAny<CancellationToken>()))
                .Returns(candidate.Object);
            adapter.Setup(candidateAdapter => candidateAdapter.BeginEdit(
                    source.Object,
                    candidate.Object,
                    It.IsAny<BeginEditRequest>(),
                    It.IsAny<CancellationToken>()))
                .Returns(EngineResult<NativeEditIdentity>.Success(
                    new NativeEditIdentity(editId, target, null, FormListEditRole.ExistingOutput)));
            var operationId = Guid.NewGuid();
            var revision = workspace.Revision;
            var firstRequest = new BeginEditRequest(
                operationId,
                revision,
                FormListEditRole.ExistingOutput,
                targetFormKey: target);

            var first = await workspace.BeginEditAsync(firstRequest);
            var caseVariantTarget = CreateFormKey("output.esp", 0x800);
            var caseVariantReplay = await workspace.BeginEditAsync(new BeginEditRequest(
                operationId,
                revision,
                FormListEditRole.ExistingOutput,
                targetFormKey: caseVariantTarget));
            var identityConflict = await workspace.BeginEditAsync(new BeginEditRequest(
                operationId,
                revision,
                FormListEditRole.ExistingOutput,
                targetFormKey: CreateFormKey("Output.esp", 0x801)));
            var revisionAfterFirst = workspace.Revision;
            var reuse = await workspace.BeginEditAsync(new BeginEditRequest(
                Guid.NewGuid(),
                revisionAfterFirst,
                FormListEditRole.ExistingOutput,
                targetFormKey: target));

            first.Succeeded.ShouldBeTrue(first.Error?.Message);
            caseVariantTarget.ModKey.Name.ShouldNotBe(target.ModKey.Name);
            caseVariantReplay.ShouldBeSameAs(first);
            identityConflict.Error!.Code.ShouldBe(EngineErrorCode.OperationIdReuse);
            reuse.Succeeded.ShouldBeTrue(reuse.Error?.Message);
            reuse.Value!.EditId.ShouldBe(editId);
            reuse.Value.FormKey.ShouldBe(target);
            reuse.Value.Role.ShouldBe(FormListEditRole.ExistingOutput);
            reuse.Value.Revision.ShouldBe(revisionAfterFirst);
            workspace.Revision.ShouldBe(revisionAfterFirst);
            adapter.Verify(candidateAdapter => candidateAdapter.CloneOutput(
                It.IsAny<INativeOutputState>(),
                It.IsAny<CancellationToken>()), Times.Once);
            adapter.Verify(candidateAdapter => candidateAdapter.BeginEdit(
                It.IsAny<INativeSourceSet>(),
                It.IsAny<INativeOutputState>(),
                It.IsAny<BeginEditRequest>(),
                It.IsAny<CancellationToken>()), Times.Once);

            await workspace.DisposeAsync();
        }
        finally
        {
            directory.Delete(true);
        }
    }

    /// <summary>Verifies cancellation observed after native candidate copying disposes the unpublished copy before allocation.</summary>
    [Fact]
    public async Task BeginEditAsync_WhenCanceledAfterClone_DisposesCandidateWithoutAllocation()
    {
        var directory = Directory.CreateTempSubdirectory();
        try
        {
            var source = CreateAsyncDisposableMock<INativeSourceSet>();
            var initialOutput = CreateAsyncDisposableMock<INativeOutputState>();
            var candidate = CreateAsyncDisposableMock<INativeOutputState>();
            var adapter = CreateAdapter(directory, source.Object);
            var workspace = await OpenWorkspaceAsync(directory, adapter);
            await SelectOutputAsync(directory, workspace, adapter, initialOutput.Object);
            using var cancellationSource = new CancellationTokenSource();
            adapter.Setup(candidateAdapter => candidateAdapter.CloneOutput(
                    It.IsAny<INativeOutputState>(),
                    cancellationSource.Token))
                .Returns(() =>
                {
                    cancellationSource.Cancel();
                    return candidate.Object;
                });
            var revision = workspace.Revision;

            await Should.ThrowAsync<OperationCanceledException>(async () =>
                await workspace.BeginEditAsync(
                    new BeginEditRequest(Guid.NewGuid(), revision, FormListEditRole.New),
                    cancellationSource.Token));

            workspace.Revision.ShouldBe(revision);
            candidate.Verify(value => value.DisposeAsync(), Times.Once);
            adapter.Verify(candidateAdapter => candidateAdapter.BeginEdit(
                It.IsAny<INativeSourceSet>(),
                It.IsAny<INativeOutputState>(),
                It.IsAny<BeginEditRequest>(),
                It.IsAny<CancellationToken>()), Times.Never);

            await workspace.DisposeAsync();
        }
        finally
        {
            directory.Delete(true);
        }
    }

    /// <summary>Verifies cancellation observed after native allocation disposes the unpublished candidate without publishing identity or revision.</summary>
    [Fact]
    public async Task BeginEditAsync_WhenCanceledAfterAllocation_DisposesCandidateWithoutPublication()
    {
        var directory = Directory.CreateTempSubdirectory();
        try
        {
            var source = CreateAsyncDisposableMock<INativeSourceSet>();
            var initialOutput = CreateAsyncDisposableMock<INativeOutputState>();
            var candidate = CreateAsyncDisposableMock<INativeOutputState>();
            var adapter = CreateAdapter(directory, source.Object);
            var workspace = await OpenWorkspaceAsync(directory, adapter);
            await SelectOutputAsync(directory, workspace, adapter, initialOutput.Object);
            using var cancellationSource = new CancellationTokenSource();
            var target = CreateFormKey("Output.esp", 0x800);
            adapter.Setup(candidateAdapter => candidateAdapter.CloneOutput(
                    It.IsAny<INativeOutputState>(),
                    cancellationSource.Token))
                .Returns(candidate.Object);
            adapter.Setup(candidateAdapter => candidateAdapter.BeginEdit(
                    source.Object,
                    candidate.Object,
                    It.IsAny<BeginEditRequest>(),
                    cancellationSource.Token))
                .Returns(() =>
                {
                    cancellationSource.Cancel();
                    return EngineResult<NativeEditIdentity>.Success(
                        new NativeEditIdentity(Guid.NewGuid(), target, null, FormListEditRole.New));
                });
            var revision = workspace.Revision;

            await Should.ThrowAsync<OperationCanceledException>(async () =>
                await workspace.BeginEditAsync(
                    new BeginEditRequest(Guid.NewGuid(), revision, FormListEditRole.New),
                    cancellationSource.Token));

            workspace.Revision.ShouldBe(revision);
            candidate.Verify(value => value.DisposeAsync(), Times.Once);

            await workspace.DisposeAsync();
        }
        finally
        {
            directory.Delete(true);
        }
    }

    /// <summary>Creates and opens a workspace through the production factory with synthetic native state.</summary>
    /// <param name="directory">The temporary workspace root.</param>
    /// <param name="adapter">The synthetic game adapter.</param>
    /// <returns>The successfully opened workspace.</returns>
    private static async Task<IFormListWorkspace> OpenWorkspaceAsync(
        DirectoryInfo directory,
        Mock<IFormListGameAdapter> adapter)
    {
        var saveCoordinator = Mock.Of<IWorkspaceSaveCoordinator>();
        TestWorkspaceInfrastructure.ConfigureReadyAdmission(saveCoordinator);
        var factory = new Core.Engine.FormListWorkspaceFactory(
            new[] { adapter.Object },
            saveCoordinator,
            TestWorkspaceInfrastructure.CreateLeaseProvider(),
            Mock.Of<ILogger>());
        var result = await factory.OpenAsync(CreateOpenRequest(directory));
        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        return result.Value!;
    }

    /// <summary>Selects a synthetic newly created output for allocation tests.</summary>
    /// <param name="directory">The temporary workspace root.</param>
    /// <param name="workspace">The workspace receiving output state.</param>
    /// <param name="adapter">The synthetic game adapter.</param>
    /// <param name="output">The native output state to publish.</param>
    /// <returns>The successful selection result.</returns>
    private static async Task<EngineResult<OutputSelectionReceipt>> SelectOutputAsync(
        DirectoryInfo directory,
        IFormListWorkspace workspace,
        Mock<IFormListGameAdapter> adapter,
        INativeOutputState output)
    {
        var association = CreateOutputAssociation(directory);
        var baseline = CreateBaseline(association.PluginPath);
        adapter.Setup(candidate => candidate.OpenOutputAsync(
                It.IsAny<INativeSourceSet>(),
                It.IsAny<SelectOutputRequest>(),
                It.IsAny<CancellationToken>()))
            .Returns(ValueTask.FromResult(EngineResult<NativeOutputOpenResult>.Success(
                new NativeOutputOpenResult(output, association, baseline))));
        var result = await workspace.SelectOutputAsync(new SelectOutputRequest(
            Guid.NewGuid(),
            workspace.Revision,
            OutputSelectionMode.CreateNew,
            association));
        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        return result;
    }

    /// <summary>Creates a loose game adapter with successful explicit source opening.</summary>
    /// <param name="directory">The temporary workspace root.</param>
    /// <param name="sources">The synthetic native source handle.</param>
    /// <returns>The configured adapter mock.</returns>
    private static Mock<IFormListGameAdapter> CreateAdapter(DirectoryInfo directory, INativeSourceSet sources)
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
            .Returns(ValueTask.FromResult(EngineResult<NativeSourceOpenResult>.Success(
                new NativeSourceOpenResult(
                    sources,
                    baselineId,
                    new[]
                    {
                        new NativeArtifactAssociation(
                            Path.Combine(directory.FullName, "Data", "Source.esp"),
                            NativeArtifactRole.Plugin,
                            null,
                            new NativeArtifactFingerprint(true, 0, new string('A', 64)),
                            new NativeFileIdentity("test", "volume", "source", 1)),
                    }))));
        return adapter;
    }

    /// <summary>Creates a valid explicit workspace request and its synthetic plugin inputs.</summary>
    /// <param name="directory">The temporary workspace root.</param>
    /// <returns>The canonical workspace-open request.</returns>
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
            new[] { masterPath, sourcePath },
            dataDirectory.FullName,
            new[] { stringsDirectory.FullName });
    }

    /// <summary>Creates a canonical synthetic output association.</summary>
    /// <param name="directory">The temporary workspace root.</param>
    /// <returns>The output association.</returns>
    private static OutputAssociation CreateOutputAssociation(DirectoryInfo directory)
    {
        return new OutputAssociation(
            Path.Combine(directory.FullName, "Output.esp"),
            ModKey.FromNameAndExtension("Output.esp"),
            LocalizedOutputMode.Embedded,
            OutputMasterStyle.Full);
    }

    /// <summary>Creates an absent output baseline for a create-new selection.</summary>
    /// <param name="path">The canonical output plugin path.</param>
    /// <returns>The complete synthetic output baseline.</returns>
    private static OutputArtifactSetBaseline CreateBaseline(string path)
    {
        return new OutputArtifactSetBaseline(
            Guid.NewGuid(),
            new[]
            {
                new NativeArtifactAssociation(
                    path,
                    NativeArtifactRole.Plugin,
                    null,
                    new NativeArtifactFingerprint(false, 0, null)),
            });
    }

    /// <summary>Creates a native FormKey for a synthetic plugin.</summary>
    /// <param name="pluginFileName">The native plugin name.</param>
    /// <param name="id">The native local record identifier.</param>
    /// <returns>The native FormKey.</returns>
    private static FormKey CreateFormKey(string pluginFileName, uint id)
    {
        return new FormKey(ModKey.FromNameAndExtension(pluginFileName), id);
    }

    /// <summary>Creates an independently verifiable asynchronous native lifetime mock.</summary>
    /// <typeparam name="T">The native handle contract.</typeparam>
    /// <returns>The configured disposable handle mock.</returns>
    private static Mock<T> CreateAsyncDisposableMock<T>()
        where T : class, IAsyncDisposable
    {
        var value = new Mock<T>();
        value.Setup(candidate => candidate.DisposeAsync()).Returns(ValueTask.CompletedTask);
        return value;
    }
}
