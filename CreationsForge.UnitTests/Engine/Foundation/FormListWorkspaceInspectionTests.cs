using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Enums;
using Moq;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Serilog;
using Shouldly;

namespace CreationsForge.UnitTests.Engine.Foundation;

/// <summary>Verifies contextual FormList views and comparisons remain detached, serialized, and cancellable.</summary>
public sealed class FormListWorkspaceInspectionTests
{
    /// <summary>Verifies a resolved read captures detached typed JSON and exact containing-plugin provenance.</summary>
    [Fact]
    public async Task ReadFormListViewAsync_WhenResolved_CapturesDetachedViewAndProvenance()
    {
        var directory = Directory.CreateTempSubdirectory();
        try
        {
            var source = CreateAsyncDisposableMock<INativeSourceSet>();
            var inspector = new Mock<IFormListNativeInspector>();
            var adapter = CreateAdapter(directory, source.Object, inspector.Object);
            var workspace = await OpenWorkspaceAsync(directory, adapter);
            var formKey = CreateFormKey("Source.esp", 0x800);
            var record = CreateRecord(formKey, "DetachedView");
            var request = new ReferenceRequest(formKey, RecordScope.Source, formKey.ModKey);
            var context = CreateContext(directory, request, ReferenceResolutionStatus.Resolved, PluginRole.Source, 1);
            var warning = new EngineWarning("missing-native-reference", "Synthetic missing native reference.");
            adapter.Setup(candidate => candidate.ReadFormListContext(
                    source.Object,
                    null,
                    request,
                    It.IsAny<CancellationToken>()))
                .Returns(EngineResult<NativeRecordRead>.Success(
                    new NativeRecordRead(context, "FormList", record.Object),
                    warnings: new[] { warning }));
            inspector.Setup(candidate => candidate.WriteReadView(
                    record.Object,
                    It.IsAny<Utf8JsonWriter>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IMajorRecordGetter, Utf8JsonWriter, CancellationToken>((nativeRecord, writer, cancellationToken) =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    writer.WriteStartObject();
                    writer.WriteString("formKey", nativeRecord.FormKey.ToString());
                    writer.WriteString("editorId", nativeRecord.EditorID);
                    writer.WriteEndObject();
                });

            var result = await workspace.ReadFormListViewAsync(request, TestContext.Current.CancellationToken);

            result.Succeeded.ShouldBeTrue(result.Error?.Message);
            result.Value!.Context.ShouldBeSameAs(context);
            result.Value.Context.ContainingModKey.ShouldBe(formKey.ModKey);
            result.Value.Context.Path.ShouldBe(Path.Combine(directory.FullName, "Data", "Source.esp"));
            result.Value.Record!.Value.GetProperty("editorId").GetString().ShouldBe("DetachedView");
            result.Warnings.Single().ShouldBeSameAs(warning);
            result.WorkspaceId.ShouldBe(workspace.WorkspaceId);
            result.ResultRevision.ShouldBe(workspace.Revision);

            await workspace.DisposeAsync();
        }
        finally
        {
            directory.Delete(true);
        }
    }

    /// <summary>Verifies a deleted context retains its status while exposing detached typed fields beyond workspace disposal.</summary>
    [Fact]
    public async Task ReadFormListViewAsync_WhenDeleted_CapturesTypedViewProvenanceAndIndependentLifetime()
    {
        var directory = Directory.CreateTempSubdirectory();
        try
        {
            var source = CreateAsyncDisposableMock<INativeSourceSet>();
            var inspector = new Mock<IFormListNativeInspector>();
            var adapter = CreateAdapter(directory, source.Object, inspector.Object);
            var workspace = await OpenWorkspaceAsync(directory, adapter);
            var formKey = CreateFormKey("Source.esp", 0x800);
            var request = new ReferenceRequest(formKey, RecordScope.Source, formKey.ModKey);
            var context = CreateContext(directory, request, ReferenceResolutionStatus.Deleted, PluginRole.Source, 1);
            var record = CreateRecord(formKey, "DeletedView", true);
            adapter.Setup(candidate => candidate.ReadFormListContext(
                    source.Object,
                    null,
                    request,
                    It.IsAny<CancellationToken>()))
                .Returns(EngineResult<NativeRecordRead>.Success(
                    new NativeRecordRead(context, "FormList", record.Object)));
            inspector.Setup(candidate => candidate.WriteReadView(
                    record.Object,
                    It.IsAny<Utf8JsonWriter>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IMajorRecordGetter, Utf8JsonWriter, CancellationToken>((nativeRecord, writer, cancellationToken) =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    writer.WriteStartObject();
                    writer.WriteString("editorId", nativeRecord.EditorID);
                    writer.WriteBoolean("isDeleted", nativeRecord.IsDeleted);
                    writer.WriteEndObject();
                });

            var result = await workspace.ReadFormListViewAsync(request, TestContext.Current.CancellationToken);
            await workspace.DisposeAsync();

            result.Succeeded.ShouldBeTrue(result.Error?.Message);
            result.Value!.Context.Status.ShouldBe(ReferenceResolutionStatus.Deleted);
            result.Value.Context.ContainingModKey.ShouldBe(formKey.ModKey);
            result.Value.Context.Path.ShouldBe(Path.Combine(directory.FullName, "Data", "Source.esp"));
            result.Value.Context.LoadOrderIndex.ShouldBe(1);
            result.Value.Context.Role.ShouldBe(PluginRole.Source);
            result.Value.Record!.Value.GetProperty("editorId").GetString().ShouldBe("DeletedView");
            result.Value.Record.Value.GetProperty("isDeleted").GetBoolean().ShouldBeTrue();
        }
        finally
        {
            directory.Delete(true);
        }
    }

    /// <summary>Verifies an ambiguous contextual read returns provenance status without inventing a JSON record.</summary>
    [Fact]
    public async Task ReadFormListViewAsync_WhenAmbiguous_ReturnsStatusWithoutRecordView()
    {
        var directory = Directory.CreateTempSubdirectory();
        try
        {
            var source = CreateAsyncDisposableMock<INativeSourceSet>();
            var inspector = new Mock<IFormListNativeInspector>();
            var adapter = CreateAdapter(directory, source.Object, inspector.Object);
            var workspace = await OpenWorkspaceAsync(directory, adapter);
            var request = new ReferenceRequest(CreateFormKey("Source.esp", 0x800), RecordScope.AllContexts);
            var context = new FormListContext(
                request,
                ReferenceResolutionStatus.Ambiguous,
                null,
                null,
                null,
                null);
            adapter.Setup(candidate => candidate.ReadFormListContext(
                    source.Object,
                    null,
                    request,
                    It.IsAny<CancellationToken>()))
                .Returns(EngineResult<NativeRecordRead>.Success(new NativeRecordRead(context, "FormList", null)));

            var result = await workspace.ReadFormListViewAsync(request, TestContext.Current.CancellationToken);

            result.Succeeded.ShouldBeTrue(result.Error?.Message);
            result.Value!.Context.Status.ShouldBe(ReferenceResolutionStatus.Ambiguous);
            result.Value.Record.ShouldBeNull();
            inspector.Verify(candidate => candidate.WriteReadView(
                It.IsAny<IMajorRecordGetter>(),
                It.IsAny<Utf8JsonWriter>(),
                It.IsAny<CancellationToken>()), Times.Never);

            await workspace.DisposeAsync();
        }
        finally
        {
            directory.Delete(true);
        }
    }

    /// <summary>Verifies comparison reads two explicit contexts and retains the deleted side's exact detached view.</summary>
    [Fact]
    public async Task CompareFormListAsync_WithDeletedAfterContext_CapturesBothViewsAndWarnings()
    {
        var directory = Directory.CreateTempSubdirectory();
        try
        {
            var source = CreateAsyncDisposableMock<INativeSourceSet>();
            var inspector = new Mock<IFormListNativeInspector>();
            var adapter = CreateAdapter(directory, source.Object, inspector.Object);
            var workspace = await OpenWorkspaceAsync(directory, adapter);
            var formKey = CreateFormKey("Source.esp", 0x800);
            var beforeRequest = new ReferenceRequest(formKey, RecordScope.Source, formKey.ModKey);
            var afterModKey = CreateModKey("Patch.esp");
            var afterRequest = new ReferenceRequest(formKey, RecordScope.AllContexts, afterModKey);
            var beforeRecord = CreateRecord(formKey, "Before");
            var afterRecord = CreateRecord(formKey, "DeletedAfter", true);
            var beforeContext = CreateContext(directory, beforeRequest, ReferenceResolutionStatus.Resolved, PluginRole.Source, 1);
            var afterContext = new FormListContext(
                afterRequest,
                ReferenceResolutionStatus.Deleted,
                afterModKey,
                Path.Combine(directory.FullName, "Data", "Patch.esp"),
                2,
                PluginRole.LoadOrder);
            var beforeWarning = new EngineWarning("missing-native-reference", "Synthetic prior missing reference.");
            var afterWarning = new EngineWarning("missing-native-reference", "Synthetic resulting missing reference.");
            adapter.SetupSequence(candidate => candidate.ReadFormListContext(
                    source.Object,
                    null,
                    It.IsAny<ReferenceRequest>(),
                    It.IsAny<CancellationToken>()))
                .Returns(EngineResult<NativeRecordRead>.Success(
                    new NativeRecordRead(beforeContext, "FormList", beforeRecord.Object),
                    warnings: new[] { beforeWarning }))
                .Returns(EngineResult<NativeRecordRead>.Success(
                    new NativeRecordRead(afterContext, "FormList", afterRecord.Object),
                    warnings: new[] { afterWarning }));
            inspector.Setup(candidate => candidate.Compare(
                    beforeRecord.Object,
                    afterRecord.Object,
                    It.IsAny<CancellationToken>()))
                .Returns(new[]
                {
                    new SemanticChangeDescriptor("EditorID", SemanticChangeKind.ValueChanged),
                });
            inspector.Setup(candidate => candidate.WriteReadView(
                    It.IsAny<IMajorRecordGetter>(),
                    It.IsAny<Utf8JsonWriter>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IMajorRecordGetter, Utf8JsonWriter, CancellationToken>((nativeRecord, writer, cancellationToken) =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    writer.WriteStartObject();
                    writer.WriteString("editorId", nativeRecord.EditorID);
                    writer.WriteBoolean("isDeleted", nativeRecord.IsDeleted);
                    writer.WriteEndObject();
                });
            var request = new CompareFormListRequest(beforeRequest, afterRequest);

            var result = await workspace.CompareFormListAsync(request, TestContext.Current.CancellationToken);

            result.Succeeded.ShouldBeTrue(result.Error?.Message);
            result.Value!.BeforeContext.ShouldBeSameAs(beforeContext);
            result.Value.AfterContext.ShouldBeSameAs(afterContext);
            result.Value.Before!.Value.GetProperty("editorId").GetString().ShouldBe("Before");
            result.Value.After!.Value.GetProperty("isDeleted").GetBoolean().ShouldBeTrue();
            result.Value.Changes.Single().FieldIdentifier.ShouldBe("EditorID");
            result.Value.Warnings.ShouldBe(new[] { beforeWarning, afterWarning });
            result.Warnings.ShouldBe(new[] { beforeWarning, afterWarning });
            adapter.Verify(candidate => candidate.ReadFormListContext(
                source.Object,
                null,
                It.IsAny<ReferenceRequest>(),
                It.IsAny<CancellationToken>()), Times.Exactly(2));

            await workspace.DisposeAsync();
        }
        finally
        {
            directory.Delete(true);
        }
    }

    /// <summary>Verifies uncertain and uninspectable contexts fail before they can be represented as record insertion or removal.</summary>
    /// <param name="beforeStatus">The synthetic prior context status.</param>
    /// <param name="afterStatus">The synthetic resulting context status.</param>
    /// <param name="expectedCode">The stable comparison failure category.</param>
    /// <param name="expectedSide">The first invalid comparison side expected in the diagnostic.</param>
    [Theory]
    [InlineData(ReferenceResolutionStatus.Resolved, ReferenceResolutionStatus.Ambiguous, EngineErrorCode.InvalidRequest, "resulting")]
    [InlineData(ReferenceResolutionStatus.Ambiguous, ReferenceResolutionStatus.Resolved, EngineErrorCode.InvalidRequest, "prior")]
    [InlineData(ReferenceResolutionStatus.Ambiguous, ReferenceResolutionStatus.Ambiguous, EngineErrorCode.InvalidRequest, "prior")]
    [InlineData(ReferenceResolutionStatus.Unsupported, ReferenceResolutionStatus.UnknownFamily, EngineErrorCode.UnsupportedOperation, "prior")]
    [InlineData(ReferenceResolutionStatus.Unsupported, ReferenceResolutionStatus.Resolved, EngineErrorCode.UnsupportedOperation, "prior")]
    [InlineData(ReferenceResolutionStatus.Resolved, ReferenceResolutionStatus.UnknownFamily, EngineErrorCode.UnsupportedOperation, "resulting")]
    public async Task CompareFormListAsync_WhenContextCannotEstablishPresence_ReturnsTypedFailureBeforeInspection(
        ReferenceResolutionStatus beforeStatus,
        ReferenceResolutionStatus afterStatus,
        EngineErrorCode expectedCode,
        string expectedSide)
    {
        var directory = Directory.CreateTempSubdirectory();
        try
        {
            var source = CreateAsyncDisposableMock<INativeSourceSet>();
            var inspector = new Mock<IFormListNativeInspector>();
            var adapter = CreateAdapter(directory, source.Object, inspector.Object);
            var workspace = await OpenWorkspaceAsync(directory, adapter);
            var formKey = CreateFormKey("Source.esp", 0x800);
            var before = CreateReadForStatus(directory, formKey, beforeStatus, "Before");
            var after = CreateReadForStatus(directory, formKey, afterStatus, "After");
            adapter.SetupSequence(candidate => candidate.ReadFormListContext(
                    source.Object,
                    null,
                    It.IsAny<ReferenceRequest>(),
                    It.IsAny<CancellationToken>()))
                .Returns(EngineResult<NativeRecordRead>.Success(before))
                .Returns(EngineResult<NativeRecordRead>.Success(after));

            var result = await workspace.CompareFormListAsync(
                new CompareFormListRequest(before.Context.Selection, after.Context.Selection),
                TestContext.Current.CancellationToken);

            result.Succeeded.ShouldBeFalse();
            result.Value.ShouldBeNull();
            result.Error!.Code.ShouldBe(expectedCode);
            result.Error.Message.ShouldContain(expectedSide);
            result.Error.Message.ShouldContain(formKey.ToString());
            result.Error.Message.ShouldContain(
                expectedSide == "prior"
                    ? before.Context.Status.ToString()
                    : after.Context.Status.ToString(),
                Case.Insensitive);
            result.WorkspaceId.ShouldBe(workspace.WorkspaceId);
            result.BaseRevision.ShouldBe(workspace.Revision);
            result.ResultRevision.ShouldBe(workspace.Revision);
            inspector.Verify(candidate => candidate.Compare(
                It.IsAny<IMajorRecordGetter?>(),
                It.IsAny<IMajorRecordGetter?>(),
                It.IsAny<CancellationToken>()), Times.Never);
            inspector.Verify(candidate => candidate.WriteReadView(
                It.IsAny<IMajorRecordGetter>(),
                It.IsAny<Utf8JsonWriter>(),
                It.IsAny<CancellationToken>()), Times.Never);

            await workspace.DisposeAsync();
        }
        finally
        {
            directory.Delete(true);
        }
    }

    /// <summary>Verifies cancellation during typed comparison propagates before any result is published.</summary>
    [Fact]
    public async Task CompareFormListAsync_WhenInspectorCancels_PropagatesCancellation()
    {
        var directory = Directory.CreateTempSubdirectory();
        try
        {
            var source = CreateAsyncDisposableMock<INativeSourceSet>();
            var inspector = new Mock<IFormListNativeInspector>();
            var adapter = CreateAdapter(directory, source.Object, inspector.Object);
            var workspace = await OpenWorkspaceAsync(directory, adapter);
            var formKey = CreateFormKey("Source.esp", 0x800);
            var selection = new ReferenceRequest(formKey, RecordScope.Source, formKey.ModKey);
            var context = CreateContext(directory, selection, ReferenceResolutionStatus.Resolved, PluginRole.Source, 1);
            var beforeRecord = CreateRecord(formKey, "Before");
            var afterRecord = CreateRecord(formKey, "After");
            adapter.SetupSequence(candidate => candidate.ReadFormListContext(
                    source.Object,
                    null,
                    selection,
                    It.IsAny<CancellationToken>()))
                .Returns(EngineResult<NativeRecordRead>.Success(new NativeRecordRead(context, "FormList", beforeRecord.Object)))
                .Returns(EngineResult<NativeRecordRead>.Success(new NativeRecordRead(context, "FormList", afterRecord.Object)));
            using var cancellationSource = new CancellationTokenSource();
            inspector.Setup(candidate => candidate.Compare(
                    beforeRecord.Object,
                    afterRecord.Object,
                    cancellationSource.Token))
                .Returns(() =>
                {
                    cancellationSource.Cancel();
                    cancellationSource.Token.ThrowIfCancellationRequested();
                    return Array.Empty<SemanticChangeDescriptor>();
                });
            var revision = workspace.Revision;

            await Should.ThrowAsync<OperationCanceledException>(async () =>
                await workspace.CompareFormListAsync(
                    new CompareFormListRequest(selection, selection),
                    cancellationSource.Token));

            workspace.Revision.ShouldBe(revision);
            inspector.Verify(candidate => candidate.WriteReadView(
                It.IsAny<IMajorRecordGetter>(),
                It.IsAny<Utf8JsonWriter>(),
                It.IsAny<CancellationToken>()), Times.Never);

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
    /// <returns>The successfully opened native workspace.</returns>
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

    /// <summary>Creates a valid explicit workspace-open request and synthetic plugin files.</summary>
    /// <param name="directory">The temporary workspace root.</param>
    /// <returns>The canonical synthetic open request.</returns>
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

    /// <summary>Creates a loose adapter with successful explicit source opening and the supplied inspector.</summary>
    /// <param name="directory">The temporary workspace root.</param>
    /// <param name="sources">The synthetic native source handle.</param>
    /// <param name="inspector">The stateless typed record inspector.</param>
    /// <returns>The configured adapter mock.</returns>
    private static Mock<IFormListGameAdapter> CreateAdapter(
        DirectoryInfo directory,
        INativeSourceSet sources,
        IFormListNativeInspector inspector)
    {
        var baselineId = new Guid("6195dcd8-7cab-49ba-b25a-56af1a86a4e7");
        TestWorkspaceInfrastructure.ConfigureSourceBaseline(
            sources,
            baselineId,
            Path.Combine(directory.FullName, "Data", "Source.esp"));
        var adapter = new Mock<IFormListGameAdapter>();
        adapter.SetupGet(candidate => candidate.Game).Returns(SupportedGame.Starfield);
        adapter.SetupGet(candidate => candidate.Inspector).Returns(inspector);
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

    /// <summary>Creates one complete singular FormList context under the supplied native selection.</summary>
    /// <param name="directory">The temporary workspace root.</param>
    /// <param name="request">The exact native context selection.</param>
    /// <param name="status">The selected context status.</param>
    /// <param name="role">The selected plugin's workspace role.</param>
    /// <param name="loadOrderIndex">The selected plugin's load-order position.</param>
    /// <returns>The complete contextual provenance.</returns>
    private static FormListContext CreateContext(
        DirectoryInfo directory,
        ReferenceRequest request,
        ReferenceResolutionStatus status,
        PluginRole role,
        int loadOrderIndex)
    {
        return new FormListContext(
            request,
            status,
            request.ContainingModKey,
            Path.Combine(directory.FullName, "Data", request.ContainingModKey!.Value.FileName),
            loadOrderIndex,
            role);
    }

    /// <summary>Creates a valid detached contextual read for a requested synthetic resolution status.</summary>
    /// <param name="directory">The temporary workspace root.</param>
    /// <param name="formKey">The native FormList identity.</param>
    /// <param name="status">The synthetic context-selection outcome.</param>
    /// <param name="editorId">The EditorID used when the status requires a detached record.</param>
    /// <returns>A contextual read whose record presence and provenance match the supplied status.</returns>
    private static NativeRecordRead CreateReadForStatus(
        DirectoryInfo directory,
        FormKey formKey,
        ReferenceResolutionStatus status,
        string editorId)
    {
        var hasSingularContext = status is ReferenceResolutionStatus.Resolved
            or ReferenceResolutionStatus.Deleted
            or ReferenceResolutionStatus.Unsupported
            or ReferenceResolutionStatus.UnknownFamily;
        var selection = hasSingularContext
            ? new ReferenceRequest(formKey, RecordScope.Source, formKey.ModKey)
            : new ReferenceRequest(formKey, RecordScope.AllContexts);
        var context = hasSingularContext
            ? CreateContext(directory, selection, status, PluginRole.Source, 1)
            : new FormListContext(selection, status, null, null, null, null);
        var record = status is ReferenceResolutionStatus.Resolved or ReferenceResolutionStatus.Deleted
            ? CreateRecord(formKey, editorId, status == ReferenceResolutionStatus.Deleted).Object
            : null;
        var recordType = status == ReferenceResolutionStatus.UnknownFamily ? null : "FormList";
        return new NativeRecordRead(context, recordType, record);
    }

    /// <summary>Creates a detached native record mock with stable identity and status.</summary>
    /// <param name="formKey">The native record identity.</param>
    /// <param name="editorId">The native EditorID.</param>
    /// <param name="isDeleted">Whether this exact context carries the deletion flag.</param>
    /// <returns>The configured detached getter mock.</returns>
    private static Mock<IMajorRecordGetter> CreateRecord(FormKey formKey, string editorId, bool isDeleted = false)
    {
        var record = new Mock<IMajorRecordGetter>();
        record.SetupGet(candidate => candidate.FormKey).Returns(formKey);
        record.SetupGet(candidate => candidate.EditorID).Returns(editorId);
        record.SetupGet(candidate => candidate.IsDeleted).Returns(isDeleted);
        return record;
    }

    /// <summary>Parses a native ModKey for synthetic tests.</summary>
    /// <param name="pluginFileName">The plugin file name.</param>
    /// <returns>The parsed native ModKey.</returns>
    private static ModKey CreateModKey(string pluginFileName)
    {
        ModKey.TryFromNameAndExtension(pluginFileName, out var modKey, out var error).ShouldBeTrue(error);
        return modKey;
    }

    /// <summary>Creates a native FormKey for synthetic tests.</summary>
    /// <param name="pluginFileName">The origin plugin file name.</param>
    /// <param name="id">The numeric native identity.</param>
    /// <returns>The native FormKey.</returns>
    private static FormKey CreateFormKey(string pluginFileName, uint id)
    {
        return new FormKey(CreateModKey(pluginFileName), id);
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
