using System.IO.Pipelines;
using System.Text.Json;
using CreationsForge.Mcp;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.RecordWire;
using CreationsForge.Core.Enums;
using CreationsForge.Starfield.PluginAdapter.Wire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using Moq;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Shouldly;

namespace CreationsForge.UnitTests.McpHost;

/// <summary>Verifies the bounded authoring and schema tools through a real SDK stream boundary.</summary>
public sealed class McpAuthoringToolProtocolTests
{
    /// <summary>Verifies selection publication and that discard and reopen receive the exact retained baseline object.</summary>
    /// <returns>A task that completes after all protocol assertions pass.</returns>
    [Fact]
    public async Task OutputTools_ThroughSdkProtocol_PreserveExactBaselineHandleIdentity()
    {
        var workspaceId = Guid.NewGuid();
        var initialRevision = new WorkspaceRevision(Guid.NewGuid(), 0);
        var selectedRevision = new WorkspaceRevision(initialRevision.BaselineId, 1);
        var resetRevision = new WorkspaceRevision(initialRevision.BaselineId, 2);
        var output = new OutputAssociation(Path.GetFullPath("Output.esm"), ModKey.FromNameAndExtension("Output.esm"), LocalizedOutputMode.Embedded, OutputMasterStyle.Full);
        var baseline = new OutputArtifactSetBaseline(Guid.NewGuid(), [new PluginArtifactAssociation(output.PluginPath, PluginArtifactRole.Plugin, null, new PluginArtifactFingerprint(false, 0, null))]);
        var workspace = CreateWorkspace(workspaceId, initialRevision);
        workspace.Setup(candidate => candidate.SelectOutputAsync(It.IsAny<SelectOutputRequest>(), It.IsAny<CancellationToken>()))
            .Returns(ValueTask.FromResult(EngineResult<OutputSelectionReceipt>.Success(new OutputSelectionReceipt(output, baseline, selectedRevision), workspaceId: workspaceId, resultRevision: selectedRevision)));
        workspace.Setup(candidate => candidate.DiscardChangesAsync(It.IsAny<DiscardChangesRequest>(), It.IsAny<CancellationToken>()))
            .Returns((DiscardChangesRequest request, CancellationToken _) =>
            {
                ReferenceEquals(request.ExpectedBaseline, baseline).ShouldBeTrue();
                return ValueTask.FromResult(EngineResult<OperationReceipt>.Success(new OperationReceipt(request.OperationId, resetRevision), workspaceId: workspaceId, operationId: request.OperationId, baseRevision: request.ExpectedRevision, resultRevision: resetRevision));
            });
        workspace.Setup(candidate => candidate.ReopenOutputAsync(It.IsAny<ReopenOutputRequest>(), It.IsAny<CancellationToken>()))
            .Returns((ReopenOutputRequest request, CancellationToken _) =>
            {
                ReferenceEquals(request.ExpectedBaseline, baseline).ShouldBeTrue();
                return ValueTask.FromResult(EngineResult<OutputSelectionReceipt>.Success(new OutputSelectionReceipt(output, baseline, resetRevision), workspaceId: workspaceId, operationId: request.OperationId, baseRevision: request.ExpectedRevision, resultRevision: resetRevision));
            });

        await using var registry = new McpWorkspaceRegistry();
        await OpenRegistryAsync(registry, workspace.Object, workspaceId);
        using var store = new McpMetadataStore();
        await using var harness = await ProtocolHarness.CreateAsync([new OutputSelectTool(registry, store), new OutputResetTool(registry, store, false), new OutputResetTool(registry, store, true)]);

        var select = GetResult(await harness.Client.CallToolAsync("creationsforge_output_select", new Dictionary<string, object?>
        {
            ["workspaceId"] = workspaceId.ToString("D"), ["operationId"] = Guid.NewGuid().ToString("D"), ["expectedRevision"] = Revision(initialRevision), ["mode"] = "create_new",
            ["output"] = new Dictionary<string, object?> { ["pluginPath"] = output.PluginPath, ["modKey"] = output.ModKey.ToString(), ["localizedOutputMode"] = "embedded", ["masterStyle"] = "full" },
        }));
        var baselineHandle = select.GetProperty("baselineReference").GetProperty("handle").GetString().ShouldNotBeNull();

        foreach (var toolName in new[] { "creationsforge_workspace_discard", "creationsforge_output_reopen" })
        {
            var result = await harness.Client.CallToolAsync(toolName, new Dictionary<string, object?>
            {
                ["workspaceId"] = workspaceId.ToString("D"), ["operationId"] = Guid.NewGuid().ToString("D"), ["expectedRevision"] = Revision(selectedRevision), ["baselineHandle"] = baselineHandle,
            });
            GetResult(result).GetProperty("revision").GetProperty("sequence").GetString().ShouldBe("2");
        }
    }

    /// <summary>Verifies protocol-visible malformed values and direct nested-object parser failures cannot reach codec decode, admission, or Core mutation.</summary>
    /// <returns>A task that completes after the rejection boundaries are verified.</returns>
    [Fact]
    public async Task AuthoringInputs_RejectMalformedDataBeforeMutation()
    {
        var workspaceId = Guid.NewGuid();
        var revision = new WorkspaceRevision(Guid.NewGuid(), 4);
        var workspace = CreateWorkspace(workspaceId, revision);
        var codec = new Mock<IFormListEditWireCodec>();
        codec.SetupGet(candidate => candidate.Game).Returns(SupportedGame.Starfield);
        codec.SetupGet(candidate => candidate.Release).Returns(GameRelease.Starfield);

        await using var registry = new McpWorkspaceRegistry();
        await OpenRegistryAsync(registry, workspace.Object, workspaceId);
        using var store = new McpMetadataStore();
        await using var harness = await ProtocolHarness.CreateAsync([new FormListApplyEditTool(registry, [codec.Object], store), new OutputSelectTool(registry, store), new FormListBeginEditTool(registry, store)]);
        var result = await harness.Client.CallToolAsync("creationsforge_formlist_apply_edit", new Dictionary<string, object?>
        {
            ["workspaceId"] = workspaceId.ToString("D"), ["operationId"] = Guid.NewGuid().ToString("D"), ["expectedRevision"] = Revision(revision), ["editId"] = Guid.NewGuid().ToString("D"),
            ["commandName"] = "form-list.clear-items", ["argumentsJson"] = "{",
        });

        GetErrorCode(result).ShouldBe("invalid_arguments");

        var invalidUnicode = await harness.Client.CallToolAsync("creationsforge_formlist_apply_edit", new Dictionary<string, object?>
        {
            ["workspaceId"] = workspaceId.ToString("D"), ["operationId"] = Guid.NewGuid().ToString("D"), ["expectedRevision"] = Revision(revision), ["editId"] = Guid.NewGuid().ToString("D"),
            ["commandName"] = "form-list.clear-items", ["argumentsJson"] = new string('\uD800', 1),
        });
        GetErrorCode(invalidUnicode).ShouldBe("invalid_arguments");

        foreach (var emptyField in new[] { "workspaceId", "operationId", "editId" })
        {
            var invalidIdentityArguments = new Dictionary<string, object?>
            {
                ["workspaceId"] = workspaceId.ToString("D"), ["operationId"] = Guid.NewGuid().ToString("D"), ["expectedRevision"] = Revision(revision), ["editId"] = Guid.NewGuid().ToString("D"),
                ["commandName"] = "form-list.clear-items", ["argumentsJson"] = "{}",
            };
            invalidIdentityArguments[emptyField] = Guid.Empty.ToString("D");
            GetErrorCode(await harness.Client.CallToolAsync("creationsforge_formlist_apply_edit", invalidIdentityArguments)).ShouldBe("invalid_arguments");
        }

        foreach (var emptyField in new[] { "workspaceId", "operationId" })
        {
            var invalidIdentityArguments = new Dictionary<string, object?>
            {
                ["workspaceId"] = workspaceId.ToString("D"), ["operationId"] = Guid.NewGuid().ToString("D"), ["expectedRevision"] = Revision(revision), ["mode"] = "create_new",
                ["output"] = new Dictionary<string, object?> { ["pluginPath"] = Path.GetFullPath("Output.esm"), ["modKey"] = "Output.esm", ["localizedOutputMode"] = "embedded", ["masterStyle"] = "full" },
            };
            invalidIdentityArguments[emptyField] = Guid.Empty.ToString("D");
            GetErrorCode(await harness.Client.CallToolAsync("creationsforge_output_select", invalidIdentityArguments)).ShouldBe("invalid_arguments");
        }

        using var malformedNameDocument = JsonDocument.Parse("{\"\\uD800\":true}");
        var malformedNameObject = malformedNameDocument.RootElement.Clone();
        await Should.ThrowAsync<JsonException>(async () =>
        {
            await harness.Client.CallToolAsync("creationsforge_formlist_apply_edit", new Dictionary<string, object?>
            {
                ["workspaceId"] = workspaceId.ToString("D"), ["operationId"] = Guid.NewGuid().ToString("D"), ["expectedRevision"] = malformedNameObject, ["editId"] = Guid.NewGuid().ToString("D"), ["commandName"] = "form-list.clear-items", ["argumentsJson"] = "{}",
            });
        });
        McpInput.TryGetOptionalRevision(new Dictionary<string, JsonElement>
        {
            ["expectedRevision"] = malformedNameObject,
        }, out _, out _).ShouldBeFalse();
        McpAuthoringSupport.TryGetOutputAssociation(new Dictionary<string, JsonElement>
        {
            ["output"] = malformedNameObject,
        }, out _, out _).ShouldBeFalse();
        McpInput.TryGetNestedReferenceRequest(new Dictionary<string, JsonElement>
        {
            ["originSelection"] = malformedNameObject,
        }, "originSelection", out _, out _).ShouldBeFalse();

        codec.Verify(candidate => candidate.Decode(It.IsAny<string>(), It.IsAny<JsonElement>(), It.IsAny<RecordWireReadLimits>(), It.IsAny<CancellationToken>()), Times.Never);
        workspace.Verify(candidate => candidate.ReadStateAsync(It.IsAny<CancellationToken>()), Times.Never);
        workspace.Verify(candidate => candidate.ApplyFormListEditAsync(It.IsAny<FormListEditRequest>(), It.IsAny<CancellationToken>()), Times.Never);
        workspace.Verify(candidate => candidate.SelectOutputAsync(It.IsAny<SelectOutputRequest>(), It.IsAny<CancellationToken>()), Times.Never);
        workspace.Verify(candidate => candidate.BeginEditAsync(It.IsAny<BeginEditRequest>(), It.IsAny<CancellationToken>()), Times.Never);
        store.AllocatedSlots.ShouldBe(0);
    }

    /// <summary>Verifies rejected output selections release provisional metadata capacity before a later valid selection.</summary>
    /// <returns>A task that completes after invalid and valid protocol requests are observed.</returns>
    [Fact]
    public async Task OutputSelect_UnknownWorkspaces_DoNotConsumeLaterPublicationCapacity()
    {
        var workspaceId = Guid.NewGuid();
        var revision = new WorkspaceRevision(Guid.NewGuid(), 0);
        var selectedRevision = revision.Next();
        var output = new OutputAssociation(Path.GetFullPath("CapacityOutput.esm"), ModKey.FromNameAndExtension("CapacityOutput.esm"), LocalizedOutputMode.Embedded, OutputMasterStyle.Full);
        var baseline = new OutputArtifactSetBaseline(Guid.NewGuid(), [new PluginArtifactAssociation(output.PluginPath, PluginArtifactRole.Plugin, null, new PluginArtifactFingerprint(false, 0, null))]);
        var workspace = CreateWorkspace(workspaceId, revision);
        workspace.Setup(candidate => candidate.SelectOutputAsync(It.IsAny<SelectOutputRequest>(), It.IsAny<CancellationToken>()))
            .Returns(ValueTask.FromResult(EngineResult<OutputSelectionReceipt>.Success(
                new OutputSelectionReceipt(output, baseline, selectedRevision),
                workspaceId: workspaceId,
                resultRevision: selectedRevision)));
        await using var registry = new McpWorkspaceRegistry();
        using var store = new McpMetadataStore(maximumHandles: 2, operationReservationSize: 2);
        await using var harness = await ProtocolHarness.CreateAsync([new OutputSelectTool(registry, store)]);

        for (var index = 0; index < 5; index++)
        {
            var rejected = await harness.Client.CallToolAsync("creationsforge_output_select", new Dictionary<string, object?>
            {
                ["workspaceId"] = Guid.NewGuid().ToString("D"), ["operationId"] = Guid.NewGuid().ToString("D"), ["expectedRevision"] = Revision(revision), ["mode"] = "create_new",
                ["output"] = new Dictionary<string, object?> { ["pluginPath"] = output.PluginPath, ["modKey"] = output.ModKey.ToString(), ["localizedOutputMode"] = "embedded", ["masterStyle"] = "full" },
            });
            GetErrorCode(rejected).ShouldBe("workspace_disposed");
        }

        store.AllocatedSlots.ShouldBe(0);
        await OpenRegistryAsync(registry, workspace.Object, workspaceId);
        var selected = GetResult(await harness.Client.CallToolAsync("creationsforge_output_select", new Dictionary<string, object?>
        {
            ["workspaceId"] = workspaceId.ToString("D"), ["operationId"] = Guid.NewGuid().ToString("D"), ["expectedRevision"] = Revision(revision), ["mode"] = "create_new",
            ["output"] = new Dictionary<string, object?> { ["pluginPath"] = output.PluginPath, ["modKey"] = output.ModKey.ToString(), ["localizedOutputMode"] = "embedded", ["masterStyle"] = "full" },
        }));

        selected.GetProperty("baselineReference").GetProperty("kind").GetString().ShouldBe("output_baseline");
        store.AllocatedSlots.ShouldBe(2);
    }

    /// <summary>Verifies escaped invalid UTF-16 inside command JSON becomes a typed input failure before Core mutation.</summary>
    /// <returns>A task that completes after the plugin codec rejection is verified.</returns>
    [Fact]
    public async Task ApplyEdit_ThroughSdkProtocol_RejectsInnerInvalidUnicodeBeforeMutation()
    {
        var workspaceId = Guid.NewGuid();
        var revision = new WorkspaceRevision(Guid.NewGuid(), 5);
        var workspace = CreateWorkspace(workspaceId, revision);
        workspace.Setup(candidate => candidate.ReadStateAsync(It.IsAny<CancellationToken>()))
            .Returns(ValueTask.FromResult(EngineResult<WorkspaceState>.Success(
                new WorkspaceState(SupportedGame.Starfield, GameRelease.Starfield, null, null, new OutputSynchronizationState(OutputSynchronizationStatus.Ready, null), revision),
                workspaceId: workspaceId,
                resultRevision: revision)));

        await using var registry = new McpWorkspaceRegistry();
        await OpenRegistryAsync(registry, workspace.Object, workspaceId);
        using var store = new McpMetadataStore();
        await using var harness = await ProtocolHarness.CreateAsync([new FormListApplyEditTool(registry, [new StarfieldFormListEditWireCodec()], store)]);
        var result = await harness.Client.CallToolAsync("creationsforge_formlist_apply_edit", new Dictionary<string, object?>
        {
            ["workspaceId"] = workspaceId.ToString("D"), ["operationId"] = Guid.NewGuid().ToString("D"), ["expectedRevision"] = Revision(revision), ["editId"] = Guid.NewGuid().ToString("D"),
            ["commandName"] = "form-list.set-editor-id", ["argumentsJson"] = "{\"editorId\":\"\\uD800\"}",
        });

        GetErrorCode(result).ShouldBe("invalid_request");
        workspace.Verify(candidate => candidate.ApplyFormListEditAsync(It.IsAny<FormListEditRequest>(), It.IsAny<CancellationToken>()), Times.Never);
        store.AllocatedSlots.ShouldBe(0);
    }

    /// <summary>Verifies begin, typed decode/apply, and fresh preview forwarding with exact identities and revisions.</summary>
    /// <returns>A task that completes after the authoring sequence is observed through the SDK protocol.</returns>
    [Fact]
    public async Task AuthoringTools_ThroughSdkProtocol_ForwardTypedMutationAndFreshPreview()
    {
        var workspaceId = Guid.NewGuid();
        var operationId = Guid.NewGuid();
        var applyOperationId = Guid.NewGuid();
        var editId = Guid.NewGuid();
        var revision0 = new WorkspaceRevision(Guid.NewGuid(), 0);
        var revision1 = new WorkspaceRevision(revision0.BaselineId, 1);
        var revision2 = new WorkspaceRevision(revision0.BaselineId, 2);
        var formKey = new FormKey(ModKey.FromNameAndExtension("Output.esm"), 0x800);
        var edit = new ClearItemsEdit();
        var output = new OutputAssociation(Path.GetFullPath("Output.esm"), ModKey.FromNameAndExtension("Output.esm"), LocalizedOutputMode.Embedded, OutputMasterStyle.Full);
        var baseline = new OutputArtifactSetBaseline(Guid.NewGuid(), [new PluginArtifactAssociation(output.PluginPath, PluginArtifactRole.Plugin, null, new PluginArtifactFingerprint(false, 0, null))]);
        var workspace = CreateWorkspace(workspaceId, revision0);
        workspace.Setup(candidate => candidate.BeginEditAsync(It.IsAny<BeginEditRequest>(), It.IsAny<CancellationToken>()))
            .Returns((BeginEditRequest request, CancellationToken _) =>
            {
                request.OperationId.ShouldBe(operationId);
                request.Role.ShouldBe(FormListEditRole.New);
                return ValueTask.FromResult(EngineResult<EditReceipt>.Success(new EditReceipt(editId, formKey, null, FormListEditRole.New, revision1), workspaceId: workspaceId, operationId: operationId, baseRevision: revision0, resultRevision: revision1));
            });
        workspace.Setup(candidate => candidate.ReadStateAsync(It.IsAny<CancellationToken>()))
            .Returns(ValueTask.FromResult(EngineResult<WorkspaceState>.Success(new WorkspaceState(SupportedGame.Starfield, GameRelease.Starfield, output, baseline, new OutputSynchronizationState(OutputSynchronizationStatus.Ready, null), revision1), workspaceId: workspaceId, resultRevision: revision1)));
        workspace.Setup(candidate => candidate.ApplyFormListEditAsync(It.IsAny<FormListEditRequest>(), It.IsAny<CancellationToken>()))
            .Returns((FormListEditRequest request, CancellationToken _) =>
            {
                request.OperationId.ShouldBe(applyOperationId);
                request.EditId.ShouldBe(editId);
                ReferenceEquals(request.Edit, edit).ShouldBeTrue();
                return ValueTask.FromResult(EngineResult<OperationReceipt>.Success(new OperationReceipt(applyOperationId, revision2), workspaceId: workspaceId, operationId: applyOperationId, baseRevision: revision1, resultRevision: revision2));
            });
        workspace.Setup(candidate => candidate.PreviewAsync(It.IsAny<CancellationToken>()))
            .Returns(ValueTask.FromResult(EngineResult<WorkspacePreview>.Success(new WorkspacePreview([], 0, []), workspaceId: workspaceId, resultRevision: revision2)));
        var codec = new Mock<IFormListEditWireCodec>();
        codec.SetupGet(candidate => candidate.Game).Returns(SupportedGame.Starfield);
        codec.SetupGet(candidate => candidate.Release).Returns(GameRelease.Starfield);
        codec.Setup(candidate => candidate.Decode("form-list.clear-items", It.Is<JsonElement>(value => value.ValueKind == JsonValueKind.Object && !value.EnumerateObject().Any()), It.IsAny<RecordWireReadLimits>(), It.IsAny<CancellationToken>()))
            .Returns(RecordWireDecodeResult<FormListEdit>.Success(edit));

        await using var registry = new McpWorkspaceRegistry();
        await OpenRegistryAsync(registry, workspace.Object, workspaceId);
        using var store = new McpMetadataStore();
        await using var harness = await ProtocolHarness.CreateAsync([new FormListBeginEditTool(registry, store), new FormListApplyEditTool(registry, [codec.Object], store), new WorkspacePreviewTool(registry)]);
        var begin = GetResult(await harness.Client.CallToolAsync("creationsforge_formlist_begin_edit", new Dictionary<string, object?>
        {
            ["workspaceId"] = workspaceId.ToString("D"), ["operationId"] = operationId.ToString("D"), ["expectedRevision"] = Revision(revision0), ["role"] = "new",
        }));
        begin.GetProperty("editId").GetString().ShouldBe(editId.ToString("D"));

        var applied = GetResult(await harness.Client.CallToolAsync("creationsforge_formlist_apply_edit", new Dictionary<string, object?>
        {
            ["workspaceId"] = workspaceId.ToString("D"), ["operationId"] = applyOperationId.ToString("D"), ["expectedRevision"] = Revision(revision1), ["editId"] = editId.ToString("D"), ["commandName"] = "form-list.clear-items", ["argumentsJson"] = "{}",
        }));
        applied.GetProperty("revision").GetProperty("sequence").GetString().ShouldBe("2");

        var preview = GetResult(await harness.Client.CallToolAsync("creationsforge_workspace_preview", new Dictionary<string, object?>
        {
            ["workspaceId"] = workspaceId.ToString("D"), ["expectedRevision"] = Revision(revision2), ["maxResults"] = 1,
        }));
        preview.GetProperty("page").GetProperty("kind").GetString().ShouldBe("object");
        workspace.Verify(candidate => candidate.PreviewAsync(It.IsAny<CancellationToken>()), Times.Once);
        store.HandleCount.ShouldBe(1);
        store.AllocatedSlots.ShouldBe(1);
    }

    /// <summary>Verifies more than 256 valid edit operations reuse one retained selected baseline instead of reserving operation slots.</summary>
    /// <returns>A task that completes after every protocol mutation succeeds.</returns>
    [Fact]
    public async Task BeginEdit_ThroughSdkProtocol_DoesNotExhaustMetadataCapacityWithOperationReservations()
    {
        const int operationCount = 257;
        var workspaceId = Guid.NewGuid();
        var revision = new WorkspaceRevision(Guid.NewGuid(), 1);
        var output = new OutputAssociation(Path.GetFullPath("Output.esm"), ModKey.FromNameAndExtension("Output.esm"), LocalizedOutputMode.Embedded, OutputMasterStyle.Full);
        var baseline = new OutputArtifactSetBaseline(Guid.NewGuid(), [new PluginArtifactAssociation(output.PluginPath, PluginArtifactRole.Plugin, null, new PluginArtifactFingerprint(false, 0, null))]);
        var workspace = CreateWorkspace(workspaceId, revision);
        workspace.Setup(candidate => candidate.ReadStateAsync(It.IsAny<CancellationToken>()))
            .Returns(ValueTask.FromResult(EngineResult<WorkspaceState>.Success(new WorkspaceState(SupportedGame.Starfield, GameRelease.Starfield, output, baseline, new OutputSynchronizationState(OutputSynchronizationStatus.Ready, null), revision), workspaceId: workspaceId, resultRevision: revision)));
        workspace.Setup(candidate => candidate.BeginEditAsync(It.IsAny<BeginEditRequest>(), It.IsAny<CancellationToken>()))
            .Returns((BeginEditRequest request, CancellationToken _) => ValueTask.FromResult(EngineResult<EditReceipt>.Success(new EditReceipt(Guid.NewGuid(), new FormKey(output.ModKey, 0x800), null, FormListEditRole.New, revision), workspaceId: workspaceId, operationId: request.OperationId, baseRevision: revision, resultRevision: revision)));

        await using var registry = new McpWorkspaceRegistry();
        await OpenRegistryAsync(registry, workspace.Object, workspaceId);
        using var store = new McpMetadataStore();
        await using var harness = await ProtocolHarness.CreateAsync([new FormListBeginEditTool(registry, store)]);
        for (var index = 0; index < operationCount; index++)
        {
            GetResult(await harness.Client.CallToolAsync("creationsforge_formlist_begin_edit", new Dictionary<string, object?>
            {
                ["workspaceId"] = workspaceId.ToString("D"), ["operationId"] = Guid.NewGuid().ToString("D"), ["expectedRevision"] = Revision(revision), ["role"] = "new",
            })).GetProperty("role").GetString().ShouldBe("new");
        }

        workspace.Verify(candidate => candidate.BeginEditAsync(It.IsAny<BeginEditRequest>(), It.IsAny<CancellationToken>()), Times.Exactly(operationCount));
        store.HandleCount.ShouldBe(1);
        store.AllocatedSlots.ShouldBe(1);
    }

    /// <summary>Verifies a full metadata store rejects a prospective edit before Core when its selected baseline was not retained.</summary>
    /// <returns>A task that completes after the fallback-save guard is verified.</returns>
    [Fact]
    public async Task BeginEdit_ThroughSdkProtocol_WhenBaselineCannotBeRetained_RejectsBeforeCoreMutation()
    {
        var workspaceId = Guid.NewGuid();
        var revision = new WorkspaceRevision(Guid.NewGuid(), 1);
        var output = new OutputAssociation(Path.GetFullPath("Output.esm"), ModKey.FromNameAndExtension("Output.esm"), LocalizedOutputMode.Embedded, OutputMasterStyle.Full);
        var baseline = new OutputArtifactSetBaseline(Guid.NewGuid(), [new PluginArtifactAssociation(output.PluginPath, PluginArtifactRole.Plugin, null, new PluginArtifactFingerprint(false, 0, null))]);
        var workspace = CreateWorkspace(workspaceId, revision);
        workspace.Setup(candidate => candidate.ReadStateAsync(It.IsAny<CancellationToken>()))
            .Returns(ValueTask.FromResult(EngineResult<WorkspaceState>.Success(new WorkspaceState(SupportedGame.Starfield, GameRelease.Starfield, output, baseline, new OutputSynchronizationState(OutputSynchronizationStatus.Ready, null), revision), workspaceId: workspaceId, resultRevision: revision)));

        await using var registry = new McpWorkspaceRegistry();
        await OpenRegistryAsync(registry, workspace.Object, workspaceId);
        using var store = new McpMetadataStore(maximumHandles: 1, operationReservationSize: 1);
        store.TryPublishWorkspaceMetadata(Guid.NewGuid(), new OutputAssociation(Path.GetFullPath("Other.esm"), ModKey.FromNameAndExtension("Other.esm"), LocalizedOutputMode.Embedded, OutputMasterStyle.Full), out _).ShouldBeTrue();
        await using var harness = await ProtocolHarness.CreateAsync([new FormListBeginEditTool(registry, store)]);
        var result = await harness.Client.CallToolAsync("creationsforge_formlist_begin_edit", new Dictionary<string, object?>
        {
            ["workspaceId"] = workspaceId.ToString("D"), ["operationId"] = Guid.NewGuid().ToString("D"), ["expectedRevision"] = Revision(revision), ["role"] = "new",
        });

        GetErrorCode(result).ShouldBe("metadata_capacity_exceeded");
        workspace.Verify(candidate => candidate.ReadStateAsync(It.IsAny<CancellationToken>()), Times.Once);
        workspace.Verify(candidate => candidate.BeginEditAsync(It.IsAny<BeginEditRequest>(), It.IsAny<CancellationToken>()), Times.Never);
        store.AllocatedSlots.ShouldBe(1);
    }

    /// <summary>Verifies stale begin and apply requests reach Core replay and guard handling without requiring new metadata capacity.</summary>
    /// <returns>A task that completes after both stale requests are forwarded through the locked workspace entry.</returns>
    [Fact]
    public async Task MutationTools_ThroughSdkProtocol_WithStaleRevision_ForwardWhenMetadataStoreIsFull()
    {
        var workspaceId = Guid.NewGuid();
        var staleRevision = new WorkspaceRevision(Guid.NewGuid(), 0);
        var currentRevision = new WorkspaceRevision(staleRevision.BaselineId, 1);
        var editId = Guid.NewGuid();
        var output = new OutputAssociation(Path.GetFullPath("Output.esm"), ModKey.FromNameAndExtension("Output.esm"), LocalizedOutputMode.Embedded, OutputMasterStyle.Full);
        var baseline = new OutputArtifactSetBaseline(Guid.NewGuid(), [new PluginArtifactAssociation(output.PluginPath, PluginArtifactRole.Plugin, null, new PluginArtifactFingerprint(false, 0, null))]);
        var workspace = CreateWorkspace(workspaceId, currentRevision);
        workspace.Setup(candidate => candidate.ReadStateAsync(It.IsAny<CancellationToken>()))
            .Returns(ValueTask.FromResult(EngineResult<WorkspaceState>.Success(new WorkspaceState(SupportedGame.Starfield, GameRelease.Starfield, output, baseline, new OutputSynchronizationState(OutputSynchronizationStatus.Ready, null), currentRevision), workspaceId: workspaceId, resultRevision: currentRevision)));
        workspace.Setup(candidate => candidate.BeginEditAsync(It.IsAny<BeginEditRequest>(), It.IsAny<CancellationToken>()))
            .Returns((BeginEditRequest request, CancellationToken _) => ValueTask.FromResult(EngineResult<EditReceipt>.Failure(new EngineError(EngineErrorCode.RevisionConflict, "The workspace revision changed."), workspaceId, request.OperationId, request.ExpectedRevision, currentRevision)));
        workspace.Setup(candidate => candidate.ApplyFormListEditAsync(It.IsAny<FormListEditRequest>(), It.IsAny<CancellationToken>()))
            .Returns((FormListEditRequest request, CancellationToken _) => ValueTask.FromResult(EngineResult<OperationReceipt>.Failure(new EngineError(EngineErrorCode.RevisionConflict, "The workspace revision changed."), workspaceId, request.OperationId, request.ExpectedRevision, currentRevision)));
        var codec = new Mock<IFormListEditWireCodec>();
        codec.SetupGet(candidate => candidate.Game).Returns(SupportedGame.Starfield);
        codec.SetupGet(candidate => candidate.Release).Returns(GameRelease.Starfield);
        codec.Setup(candidate => candidate.Decode("form-list.clear-items", It.IsAny<JsonElement>(), It.IsAny<RecordWireReadLimits>(), It.IsAny<CancellationToken>()))
            .Returns(RecordWireDecodeResult<FormListEdit>.Success(new ClearItemsEdit()));

        await using var registry = new McpWorkspaceRegistry();
        await OpenRegistryAsync(registry, workspace.Object, workspaceId);
        using var store = new McpMetadataStore(maximumHandles: 1, operationReservationSize: 1);
        store.TryPublishWorkspaceMetadata(Guid.NewGuid(), new OutputAssociation(Path.GetFullPath("Other.esm"), ModKey.FromNameAndExtension("Other.esm"), LocalizedOutputMode.Embedded, OutputMasterStyle.Full), out _).ShouldBeTrue();
        await using var harness = await ProtocolHarness.CreateAsync([new FormListBeginEditTool(registry, store), new FormListApplyEditTool(registry, [codec.Object], store)]);

        GetErrorCode(await harness.Client.CallToolAsync("creationsforge_formlist_begin_edit", new Dictionary<string, object?>
        {
            ["workspaceId"] = workspaceId.ToString("D"), ["operationId"] = Guid.NewGuid().ToString("D"), ["expectedRevision"] = Revision(staleRevision), ["role"] = "new",
        })).ShouldBe("revision_conflict");
        GetErrorCode(await harness.Client.CallToolAsync("creationsforge_formlist_apply_edit", new Dictionary<string, object?>
        {
            ["workspaceId"] = workspaceId.ToString("D"), ["operationId"] = Guid.NewGuid().ToString("D"), ["expectedRevision"] = Revision(staleRevision), ["editId"] = editId.ToString("D"), ["commandName"] = "form-list.clear-items", ["argumentsJson"] = "{}",
        })).ShouldBe("revision_conflict");

        workspace.Verify(candidate => candidate.BeginEditAsync(It.IsAny<BeginEditRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        workspace.Verify(candidate => candidate.ApplyFormListEditAsync(It.IsAny<FormListEditRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        store.AllocatedSlots.ShouldBe(1);
    }

    /// <summary>Verifies schema discovery is deterministic and reads schema and default sections without embedding them in list results.</summary>
    /// <returns>A task that completes after list and read paging are verified.</returns>
    [Fact]
    public async Task SchemaTools_ThroughSdkProtocol_ListKeysAndPageExactNodeSections()
    {
        var catalogId = new string('A', 64);
        var key = new RecordWireSchemaNodeKey(catalogId, RecordWireSchemaNodeKind.Command, "form-list.clear-items");
        var schema = JsonSerializer.SerializeToElement(new { type = "object", additionalProperties = false, properties = new { value = new { type = "string" } } });
        var template = JsonSerializer.SerializeToElement(new { value = "example" });
        var node = new RecordWireSchemaNode(key, schema, template);
        var catalog = new Mock<IFormListEditWireSchemaCatalog>();
        catalog.SetupGet(candidate => candidate.Identity).Returns(new RecordWireSchemaCatalogIdentity(SupportedGame.Starfield, GameRelease.Starfield, "v1", catalogId));
        catalog.SetupGet(candidate => candidate.Nodes).Returns([key]);
        catalog.Setup(candidate => candidate.ReadNode(It.Is<RecordWireSchemaNodeKey>(value => value.CatalogId == catalogId && value.Kind == key.Kind && value.Name == key.Name), It.IsAny<CancellationToken>()))
            .Returns(EngineResult<RecordWireSchemaNode>.Success(node));

        await using var harness = await ProtocolHarness.CreateAsync([new FormListEditSchemasListTool([catalog.Object]), new FormListEditSchemaReadTool([catalog.Object])]);
        var listed = GetResult(await harness.Client.CallToolAsync("creationsforge_formlist_edit_schemas_list", new Dictionary<string, object?> { ["game"] = "starfield", ["release"] = "starfield", ["maxResults"] = 1 }));
        listed.GetProperty("catalogId").GetString().ShouldBe(catalogId);
        listed.GetProperty("nodes")[0].GetProperty("name").GetString().ShouldBe(key.Name);
        listed.TryGetProperty("schema", out _).ShouldBeFalse();

        foreach (var section in new[] { "schema", "default" })
        {
            var path = section == "schema" ? "/properties/value" : "/value";
            var read = GetResult(await harness.Client.CallToolAsync("creationsforge_formlist_edit_schema_read", new Dictionary<string, object?>
            {
                ["game"] = "starfield", ["release"] = "starfield", ["catalogId"] = catalogId, ["kind"] = "command", ["name"] = key.Name, ["section"] = section, ["path"] = path,
            }));
            read.GetProperty("section").GetString().ShouldBe(section);
            read.GetProperty("page").GetProperty("kind").GetString().ShouldBe(section == "schema" ? "object" : "string");
        }
    }

    /// <summary>Creates a registry-compatible workspace mock.</summary>
    /// <param name="workspaceId">The mock workspace identity.</param>
    /// <param name="revision">The mock opening revision.</param>
    /// <returns>The configured workspace mock.</returns>
    private static Mock<IPluginWorkspace> CreateWorkspace(Guid workspaceId, WorkspaceRevision revision)
    {
        var workspace = new Mock<IPluginWorkspace>();
        workspace.SetupGet(candidate => candidate.WorkspaceId).Returns(workspaceId);
        workspace.SetupGet(candidate => candidate.Revision).Returns(revision);
        workspace.Setup(candidate => candidate.DisposeAsync()).Returns(ValueTask.CompletedTask);
        return workspace;
    }

    /// <summary>Publishes one mock workspace into the real lifetime registry.</summary>
    /// <param name="registry">The registry that receives ownership.</param>
    /// <param name="workspace">The mock workspace.</param>
    /// <param name="workspaceId">The exact workspace identity.</param>
    /// <returns>A task that completes after registry publication.</returns>
    private static async Task OpenRegistryAsync(McpWorkspaceRegistry registry, IPluginWorkspace workspace, Guid workspaceId)
    {
        var factory = new Mock<IPluginWorkspaceFactory>();
        factory.Setup(candidate => candidate.OpenAsync(It.IsAny<WorkspaceOpenRequest>(), It.IsAny<CancellationToken>())).Returns(ValueTask.FromResult(EngineResult<IPluginWorkspace>.Success(workspace)));
        var request = new WorkspaceOpenRequest(workspaceId, SupportedGame.Starfield, GameRelease.Starfield, Path.GetFullPath("Source.esm"), [], Path.GetFullPath("Data"), []);
        (await registry.OpenAsync(factory.Object, request)).Succeeded.ShouldBeTrue();
    }

    /// <summary>Projects an exact revision into SDK request values.</summary>
    /// <param name="revision">The exact Core revision.</param>
    /// <returns>A closed canonical request object.</returns>
    private static Dictionary<string, object?> Revision(WorkspaceRevision revision) => new() { ["baselineId"] = revision.BaselineId.ToString("D"), ["sequence"] = revision.Sequence.ToString(System.Globalization.CultureInfo.InvariantCulture) };

    /// <summary>Gets a successful tool-specific result.</summary>
    /// <param name="result">The SDK tool result.</param>
    /// <returns>The nested successful result object.</returns>
    private static JsonElement GetResult(CallToolResult result)
    {
        result.IsError.ShouldNotBe(true);
        return result.StructuredContent.ShouldNotBeNull().GetProperty("result");
    }

    /// <summary>Gets the stable code from a failed tool result.</summary>
    /// <param name="result">The SDK tool result.</param>
    /// <returns>The stable error code.</returns>
    private static string GetErrorCode(CallToolResult result)
    {
        result.IsError.ShouldBe(true);
        return result.StructuredContent.ShouldNotBeNull().GetProperty("error").GetProperty("code").GetString().ShouldNotBeNull();
    }

    /// <summary>Owns a real in-process MCP SDK stream server and client.</summary>
    private sealed class ProtocolHarness : IAsyncDisposable
    {
        /// <summary>The running host.</summary>
        private readonly IHost Host;

        /// <summary>Initializes the protocol harness.</summary>
        /// <param name="host">The running server host.</param>
        /// <param name="client">The connected SDK client.</param>
        private ProtocolHarness(IHost host, McpClient client)
        {
            Host = host;
            Client = client;
        }

        /// <summary>Gets the connected client.</summary>
        internal McpClient Client { get; }

        /// <summary>Starts the in-memory SDK server and client.</summary>
        /// <param name="tools">The exact tools to register.</param>
        /// <returns>The connected owned harness.</returns>
        internal static async Task<ProtocolHarness> CreateAsync(IReadOnlyList<McpServerTool> tools)
        {
            var clientToServer = new Pipe();
            var serverToClient = new Pipe();
            var builder = Microsoft.Extensions.Hosting.Host.CreateApplicationBuilder();
            builder.Logging.ClearProviders();
            builder.Services.AddMcpServer().WithStreamServerTransport(clientToServer.Reader.AsStream(), serverToClient.Writer.AsStream()).WithTools(tools);
            var host = builder.Build();
            try
            {
                await host.StartAsync(TestContext.Current.CancellationToken);
                var client = await McpClient.CreateAsync(new StreamClientTransport(clientToServer.Writer.AsStream(), serverToClient.Reader.AsStream()), cancellationToken: TestContext.Current.CancellationToken);
                return new ProtocolHarness(host, client);
            }
            catch
            {
                host.Dispose();
                throw;
            }
        }

        /// <summary>Disposes the client before stopping the host.</summary>
        /// <returns>A task that completes after both endpoints stop.</returns>
        public async ValueTask DisposeAsync()
        {
            await Client.DisposeAsync();
            await Host.StopAsync(TestContext.Current.CancellationToken);
            Host.Dispose();
        }
    }
}
