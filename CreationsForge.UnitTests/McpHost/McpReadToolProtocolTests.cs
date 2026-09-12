using System.IO.Pipelines;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using CreationsForge.Console.Mcp;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using Moq;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Shouldly;

namespace CreationsForge.UnitTests.McpHost;

/// <summary>
/// Verifies the native MCP read tools through a real SDK client and stream server boundary.
/// </summary>
public sealed class McpReadToolProtocolTests
{
    /// <summary>Verifies that hostile argument names cannot expand structured or text error responses.</summary>
    [Fact]
    public async Task NativeToolErrors_ThroughSdkProtocol_BoundAndSanitizeUntrustedDetails()
    {
        await using var registry = new McpWorkspaceRegistry();
        var factory = new Mock<IFormListWorkspaceFactory>();
        var tools = new McpToolCatalog().CreateTools(registry, "protocol-test", factory.Object);
        await using var harness = await ProtocolHarness.CreateAsync(tools);
        var hostileName = new string('x', 100_000);

        var result = await harness.Client.CallToolAsync(
            "creationsforge_plugins_list",
            new Dictionary<string, object?>
            {
                [hostileName] = true,
            });

        result.IsError.ShouldBe(true);
        result.StructuredContent.ShouldNotBeNull();
        Encoding.UTF8.GetByteCount(result.StructuredContent.Value.GetRawText()).ShouldBeLessThanOrEqualTo(64 * 1024);
        var error = result.StructuredContent.Value.GetProperty("error");
        error.GetProperty("code").GetString().ShouldBe("invalid_arguments");
        error.GetProperty("message").GetString().ShouldBe("The request contains an undeclared argument.");
        var text = result.Content.OfType<TextContentBlock>().ShouldHaveSingleItem().Text;
        text.Length.ShouldBeLessThan(4096);
        text.ShouldNotContain(hostileName);
    }

    /// <summary>Verifies that native domain tools and capability claims require an actual factory.</summary>
    [Fact]
    public async Task ToolCatalog_RegistersNativeToolsOnlyWhenFactoryIsProvided()
    {
        await using var registry = new McpWorkspaceRegistry();
        var factory = new Mock<IFormListWorkspaceFactory>();
        var catalog = new McpToolCatalog();

        var unavailable = catalog.CreateTools(registry, "test");
        var available = catalog.CreateTools(registry, "test", factory.Object);

        unavailable.Select(tool => tool.ProtocolTool.Name).ShouldBe(["creationsforge_server_info"]);
        available.Select(tool => tool.ProtocolTool.Name).ShouldBe([
            "creationsforge_server_info",
            "creationsforge_workspace_open",
            "creationsforge_workspace_close",
            "creationsforge_plugins_list",
            "creationsforge_formlists_list",
            "creationsforge_references_search",
            "creationsforge_formlist_inspect",
            "creationsforge_formlist_compare",
        ]);
        available.ShouldAllBe(tool => tool.ProtocolTool.InputSchema.GetProperty("additionalProperties").GetBoolean() == false);
    }

    /// <summary>Verifies exact workspace-open forwarding, read-result revision fidelity, native search paging, and idempotent close.</summary>
    [Fact]
    public async Task NativeTools_ThroughSdkProtocol_ForwardOpenListSearchAndClose()
    {
        var workspaceId = Guid.NewGuid();
        var openRevision = new WorkspaceRevision(Guid.NewGuid(), 7);
        var readRevision = new WorkspaceRevision(openRevision.BaselineId, ulong.MaxValue);
        var sampledRevision = new WorkspaceRevision(openRevision.BaselineId, 99);
        var sourcePath = Path.GetFullPath("Source.esm");
        var dataPath = Path.GetFullPath("Data");
        var stringsPath = Path.GetFullPath("Strings");
        var sourceModKey = ModKey.FromNameAndExtension("Source.esm");
        var formKey = new FormKey(sourceModKey, 0x812);
        var workspace = new Mock<IFormListWorkspace>();
        workspace.SetupGet(candidate => candidate.WorkspaceId).Returns(workspaceId);
        workspace.SetupGet(candidate => candidate.Revision).Returns(openRevision);
        workspace.Setup(candidate => candidate.DisposeAsync()).Returns(ValueTask.CompletedTask);
        workspace.Setup(candidate => candidate.ListPluginsAsync(It.IsAny<CancellationToken>()))
            .Returns(ValueTask.FromResult(EngineResult<IReadOnlyList<PluginSummary>>.Success(
                Array.AsReadOnly(new[]
                {
                    new PluginSummary(sourceModKey, sourcePath, 0, PluginRole.Source),
                }),
                workspaceId: workspaceId,
                resultRevision: readRevision)));
        workspace.Setup(candidate => candidate.SearchReferencesAsync(
                It.IsAny<ReferenceSearchRequest>(),
                It.IsAny<CancellationToken>()))
            .Returns((ReferenceSearchRequest searchRequest, CancellationToken _) =>
                ValueTask.FromResult(EngineResult<ReferenceSearchPage>.Success(
                    new ReferenceSearchPage(
                        Array.AsReadOnly(new[]
                        {
                            new ReferenceSearchMatch(
                                formKey,
                                "FLST",
                                "ExampleList",
                                sourceModKey,
                                sourcePath,
                                0,
                                PluginRole.Source),
                        }),
                        searchRequest.ContinuationToken is null ? "native-next" : null),
                    workspaceId: workspaceId,
                    resultRevision: readRevision)));
        WorkspaceOpenRequest? capturedOpenRequest = null;
        var factory = new Mock<IFormListWorkspaceFactory>();
        factory.Setup(candidate => candidate.OpenAsync(
                It.IsAny<WorkspaceOpenRequest>(),
                It.IsAny<CancellationToken>()))
            .Callback<WorkspaceOpenRequest, CancellationToken>((request, _) => capturedOpenRequest = request)
            .Returns(ValueTask.FromResult(EngineResult<IFormListWorkspace>.Success(workspace.Object)));

        await using var registry = new McpWorkspaceRegistry();
        var tools = new McpToolCatalog().CreateTools(registry, "protocol-test", factory.Object);
        await using var harness = await ProtocolHarness.CreateAsync(tools);
        var openResult = await harness.Client.CallToolAsync(
            "creationsforge_workspace_open",
            new Dictionary<string, object?>
            {
                ["workspaceId"] = workspaceId.ToString("D"),
                ["game"] = "starfield",
                ["release"] = "starfield",
                ["sourcePluginPath"] = sourcePath,
                ["loadOrderPluginPaths"] = new[] { sourcePath },
                ["dataDirectoryPath"] = dataPath,
                ["stringDirectoryPaths"] = new[] { stringsPath },
            });

        GetResult(openResult).GetProperty("revision").GetProperty("sequence").GetString().ShouldBe("7");
        capturedOpenRequest.ShouldNotBeNull();
        capturedOpenRequest.WorkspaceId.ShouldBe(workspaceId);
        capturedOpenRequest.Game.ShouldBe(SupportedGame.Starfield);
        capturedOpenRequest.Release.ShouldBe(GameRelease.Starfield);
        capturedOpenRequest.SourcePluginPath.ShouldBe(sourcePath);
        capturedOpenRequest.LoadOrderPluginPaths.ShouldBe([sourcePath]);
        capturedOpenRequest.DataDirectoryPath.ShouldBe(dataPath);
        capturedOpenRequest.StringDirectoryPaths.ShouldBe([stringsPath]);

        workspace.SetupGet(candidate => candidate.Revision).Returns(sampledRevision);
        var pluginsResult = await harness.Client.CallToolAsync(
            "creationsforge_plugins_list",
            new Dictionary<string, object?>
            {
                ["workspaceId"] = workspaceId.ToString("D"),
            });
        var plugins = GetResult(pluginsResult);
        plugins.GetProperty("revision").GetProperty("sequence").GetString().ShouldBe(ulong.MaxValue.ToString());
        plugins.GetProperty("plugins")[0].GetProperty("modKey").GetString().ShouldBe(sourceModKey.ToString());

        var searchResult = await harness.Client.CallToolAsync(
            "creationsforge_references_search",
            new Dictionary<string, object?>
            {
                ["workspaceId"] = workspaceId.ToString("D"),
                ["query"] = " ExampleList ",
                ["scope"] = "source",
                ["containingModKey"] = sourceModKey.ToString(),
                ["maxResults"] = 17,
            });
        var search = GetResult(searchResult);
        search.GetProperty("matches")[0].GetProperty("formKey").GetString().ShouldBe(formKey.ToString());
        search.GetProperty("cursor").GetString().ShouldBe("native-next");
        workspace.Verify(candidate => candidate.SearchReferencesAsync(
            It.Is<ReferenceSearchRequest>(request =>
                request.Query == "ExampleList" &&
                request.MaximumResults == 17 &&
                request.ContinuationToken == null &&
                request.Scope == RecordScope.Source &&
                request.ContainingModKey == sourceModKey),
            It.IsAny<CancellationToken>()), Times.Once);

        var firstClose = await harness.Client.CallToolAsync(
            "creationsforge_workspace_close",
            new Dictionary<string, object?> { ["workspaceId"] = workspaceId.ToString("D") });
        var secondClose = await harness.Client.CallToolAsync(
            "creationsforge_workspace_close",
            new Dictionary<string, object?> { ["workspaceId"] = workspaceId.ToString("D") });

        GetResult(firstClose).GetProperty("closed").GetBoolean().ShouldBeTrue();
        GetResult(secondClose).GetProperty("closed").GetBoolean().ShouldBeFalse();
        workspace.Verify(candidate => candidate.DisposeAsync(), Times.Once);
    }

    /// <summary>Verifies that an SDK protocol cancellation notification reaches an active native read.</summary>
    /// <returns>A task that completes after the server observes cancellation and the client request waiter stops.</returns>
    [Fact(Timeout = 30_000)]
    public async Task NativeRead_ThroughSdkProtocol_PropagatesCancellation()
    {
        using var deadlineSource = CancellationTokenSource.CreateLinkedTokenSource(
            TestContext.Current.CancellationToken);
        deadlineSource.CancelAfter(TimeSpan.FromSeconds(15));
        var workspaceId = Guid.NewGuid();
        var revision = new WorkspaceRevision(Guid.NewGuid(), 4);
        var readStarted = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var cancellationObserved = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var pendingRead = new TaskCompletionSource<EngineResult<IReadOnlyList<PluginSummary>>>(
            TaskCreationOptions.RunContinuationsAsynchronously);
        var workspace = CreateWorkspace(workspaceId, revision);
        workspace.Setup(candidate => candidate.ListPluginsAsync(It.IsAny<CancellationToken>()))
            .Returns((CancellationToken token) =>
            {
                readStarted.TrySetResult();
                token.Register(() => cancellationObserved.TrySetResult());
                return new ValueTask<EngineResult<IReadOnlyList<PluginSummary>>>(pendingRead.Task.WaitAsync(token));
            });
        var factory = CreateFactory(workspace.Object);

        await using var registry = new McpWorkspaceRegistry();
        var tools = new McpToolCatalog().CreateTools(registry, "protocol-test", factory.Object);
        await using var harness = await ProtocolHarness.CreateAsync(tools, deadlineSource.Token);
        await OpenWorkspaceAsync(harness.Client, workspaceId, deadlineSource.Token);
        var requestId = new RequestId(Guid.NewGuid().ToString("D"));
        var requestParameters = new CallToolRequestParams
        {
            Name = "creationsforge_plugins_list",
            Arguments = new Dictionary<string, JsonElement>
            {
                ["workspaceId"] = JsonSerializer.SerializeToElement(workspaceId.ToString("D")),
            },
        };
        var request = new JsonRpcRequest
        {
            Id = requestId,
            Method = RequestMethods.ToolsCall,
            Params = JsonSerializer.SerializeToNode(requestParameters, ModelContextProtocol.McpJsonUtilities.DefaultOptions),
        };
        using var clientWaitSource = CancellationTokenSource.CreateLinkedTokenSource(deadlineSource.Token);
        var callTask = harness.Client.SendRequestAsync(request, clientWaitSource.Token);
        try
        {
            await readStarted.Task.WaitAsync(deadlineSource.Token);

            await harness.Client.SendNotificationAsync(
                NotificationMethods.CancelledNotification,
                new CancelledNotificationParams
                {
                    RequestId = requestId,
                    Reason = "Protocol cancellation test.",
                },
                ModelContextProtocol.McpJsonUtilities.DefaultOptions,
                deadlineSource.Token);

            await cancellationObserved.Task.WaitAsync(deadlineSource.Token);
            await Should.ThrowAsync<OperationCanceledException>(async () =>
                await callTask.WaitAsync(deadlineSource.Token));
            deadlineSource.IsCancellationRequested.ShouldBeFalse();
            callTask.IsCanceled.ShouldBeTrue();
        }
        finally
        {
            clientWaitSource.Cancel();
            pendingRead.TrySetResult(EngineResult<IReadOnlyList<PluginSummary>>.Success(
                Array.Empty<PluginSummary>(),
                workspaceId: workspaceId,
                resultRevision: revision));
            using var drainSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            await ((Task)callTask).WaitAsync(drainSource.Token)
                .ConfigureAwait(
                    ConfigureAwaitOptions.ContinueOnCapturedContext |
                    ConfigureAwaitOptions.SuppressThrowing);
        }
    }

    /// <summary>Verifies exact nested reconstruction, escaped property paths, Unicode-safe chunks, byte-budget shrinkage, and revision-bound cursors.</summary>
    [Fact]
    public async Task FormListInspect_ThroughSdkProtocol_PagesCompleteDetachedValuesSafely()
    {
        var workspaceId = Guid.NewGuid();
        var revision = new WorkspaceRevision(Guid.NewGuid(), 3);
        var sourceModKey = ModKey.FromNameAndExtension("Source.esm");
        var formKey = new FormKey(sourceModKey, 0x901);
        var selection = new ReferenceRequest(formKey, RecordScope.Source, sourceModKey);
        var context = new FormListContext(
            selection,
            ReferenceResolutionStatus.Resolved,
            sourceModKey,
            Path.GetFullPath("Source.esm"),
            0,
            PluginRole.Source);
        var propertyValues = Enumerable.Range(0, 250)
            .ToDictionary(
                index => $"large/{index:D3}~{new string('x', 300)}",
                index => (object?)index,
                StringComparer.Ordinal);
        propertyValues["nested"] = new Dictionary<string, object?>
        {
            ["empty"] = Array.Empty<object>(),
            ["null"] = null,
        };
        propertyValues["unicode"] = "A😀B😀C";
        var record = JsonSerializer.SerializeToElement(propertyValues);
        var workspace = CreateWorkspace(workspaceId, revision);
        workspace.Setup(candidate => candidate.ReadFormListViewAsync(
                It.IsAny<ReferenceRequest>(),
                It.IsAny<CancellationToken>()))
            .Returns(() => ValueTask.FromResult(EngineResult<FormListReadView>.Success(
                new FormListReadView(context, record),
                workspaceId: workspaceId,
                resultRevision: revision)));
        var otherWorkspaceId = Guid.NewGuid();
        var otherWorkspace = CreateWorkspace(otherWorkspaceId, revision);
        otherWorkspace.Setup(candidate => candidate.ReadFormListViewAsync(
                It.IsAny<ReferenceRequest>(),
                It.IsAny<CancellationToken>()))
            .Returns(() => ValueTask.FromResult(EngineResult<FormListReadView>.Success(
                new FormListReadView(context, record),
                workspaceId: otherWorkspaceId,
                resultRevision: revision)));
        var factory = CreateFactory(workspace.Object);
        factory.Setup(candidate => candidate.OpenAsync(
                It.Is<WorkspaceOpenRequest>(request => request.WorkspaceId == otherWorkspaceId),
                It.IsAny<CancellationToken>()))
            .Returns(ValueTask.FromResult(EngineResult<IFormListWorkspace>.Success(otherWorkspace.Object)));

        await using var registry = new McpWorkspaceRegistry();
        var tools = new McpToolCatalog().CreateTools(registry, "protocol-test", factory.Object);
        await using var harness = await ProtocolHarness.CreateAsync(tools);
        await OpenWorkspaceAsync(harness.Client, workspaceId);

        var firstResult = await harness.Client.CallToolAsync(
            "creationsforge_formlist_inspect",
            CreateInspectArguments(workspaceId, formKey, sourceModKey, string.Empty, 250));
        var firstStructured = firstResult.StructuredContent.ShouldNotBeNull();
        Encoding.UTF8.GetByteCount(firstStructured.GetRawText()).ShouldBeLessThanOrEqualTo(64 * 1024);
        var firstPage = firstStructured.GetProperty("result").GetProperty("page");
        firstPage.GetProperty("kind").GetString().ShouldBe("object");
        firstPage.GetProperty("count").GetInt32().ShouldBeLessThan(firstPage.GetProperty("totalCount").GetInt32());
        var firstChild = firstPage.GetProperty("children")[0];
        var firstChildPath = firstChild.GetProperty("path").GetString().ShouldNotBeNull();
        firstChildPath.ShouldContain("~1");
        firstChildPath.ShouldContain("~0");

        var unicodeChunks = new StringBuilder();
        string? cursor = null;
        do
        {
            var arguments = CreateInspectArguments(workspaceId, formKey, sourceModKey, "/unicode", 2);
            if (cursor is not null)
            {
                arguments["cursor"] = cursor;
            }

            var result = await harness.Client.CallToolAsync("creationsforge_formlist_inspect", arguments);
            var page = GetResult(result).GetProperty("page");
            page.GetProperty("kind").GetString().ShouldBe("string");
            var chunk = page.GetProperty("value").GetString();
            chunk.ShouldNotBeNull();
            char.IsHighSurrogate(chunk[^1]).ShouldBeFalse();
            unicodeChunks.Append(chunk);
            cursor = page.GetProperty("cursor").ValueKind == JsonValueKind.Null
                ? null
                : page.GetProperty("cursor").GetString();
        }
        while (cursor is not null);

        unicodeChunks.ToString().ShouldBe("A😀B😀C");

        var staleCursor = firstPage.GetProperty("cursor").GetString();
        var invalidVersionArguments = CreateInspectArguments(workspaceId, formKey, sourceModKey, string.Empty, 250);
        invalidVersionArguments["cursor"] = RewriteCursorVersion(staleCursor!, "1.5");
        var invalidVersionResult = await harness.Client.CallToolAsync("creationsforge_formlist_inspect", invalidVersionArguments);
        invalidVersionResult.IsError.ShouldBe(true);
        invalidVersionResult.StructuredContent.ShouldNotBeNull()
            .GetProperty("error").GetProperty("code").GetString().ShouldBe("invalid_cursor");

        await OpenWorkspaceAsync(harness.Client, otherWorkspaceId);
        var otherWorkspacePage = GetResult(await harness.Client.CallToolAsync(
            "creationsforge_formlist_inspect",
            CreateInspectArguments(otherWorkspaceId, formKey, sourceModKey, string.Empty, 250))).GetProperty("page");
        var crossWorkspaceArguments = CreateInspectArguments(workspaceId, formKey, sourceModKey, string.Empty, 250);
        crossWorkspaceArguments["cursor"] = otherWorkspacePage.GetProperty("cursor").GetString();
        var crossWorkspaceResult = await harness.Client.CallToolAsync(
            "creationsforge_formlist_inspect",
            crossWorkspaceArguments);
        crossWorkspaceResult.IsError.ShouldBe(true);
        crossWorkspaceResult.StructuredContent.ShouldNotBeNull()
            .GetProperty("error").GetProperty("code").GetString().ShouldBe("invalid_cursor");

        using var duplicateSelectionDocument = JsonDocument.Parse(
            $"{{\"formKey\":\"{formKey}\",\"formKey\":\"{formKey}\",\"scope\":\"source\",\"containingModKey\":\"{sourceModKey}\"}}");
        var duplicateArguments = CreateInspectArguments(workspaceId, formKey, sourceModKey, string.Empty, 50);
        duplicateArguments["selection"] = duplicateSelectionDocument.RootElement.Clone();
        var duplicateResult = await harness.Client.CallToolAsync("creationsforge_formlist_inspect", duplicateArguments);
        duplicateResult.IsError.ShouldBe(true);
        duplicateResult.StructuredContent.ShouldNotBeNull()
            .GetProperty("error").GetProperty("code").GetString().ShouldBe("invalid_arguments");

        revision = revision.Next();
        var staleArguments = CreateInspectArguments(workspaceId, formKey, sourceModKey, string.Empty, 250);
        staleArguments["cursor"] = staleCursor;
        var staleResult = await harness.Client.CallToolAsync("creationsforge_formlist_inspect", staleArguments);

        staleResult.IsError.ShouldBe(true);
        staleResult.StructuredContent.ShouldNotBeNull()
            .GetProperty("error").GetProperty("code").GetString().ShouldBe("invalid_cursor");
        workspace.Verify(candidate => candidate.ReadFormListViewAsync(
            It.Is<ReferenceRequest>(request =>
                request.FormKey == formKey &&
                request.Scope == RecordScope.Source &&
                request.ContainingModKey == sourceModKey),
            It.IsAny<CancellationToken>()), Times.AtLeast(4));
    }

    /// <summary>Verifies semantic descriptor order and independent before/after/warning section navigation.</summary>
    [Fact]
    public async Task FormListCompare_ThroughSdkProtocol_PreservesEngineSemanticsAndSectionPaging()
    {
        var workspaceId = Guid.NewGuid();
        var revision = new WorkspaceRevision(Guid.NewGuid(), 12);
        var sourceModKey = ModKey.FromNameAndExtension("Source.esm");
        var outputModKey = ModKey.FromNameAndExtension("Output.esp");
        var formKey = new FormKey(sourceModKey, 0xA11);
        var beforeSelection = new ReferenceRequest(formKey, RecordScope.Source, sourceModKey);
        var afterSelection = new ReferenceRequest(formKey, RecordScope.StagedOutput, outputModKey);
        var beforeContext = new FormListContext(beforeSelection, ReferenceResolutionStatus.Resolved, sourceModKey, Path.GetFullPath("Source.esm"), 0, PluginRole.Source);
        var afterContext = new FormListContext(afterSelection, ReferenceResolutionStatus.Resolved, outputModKey, Path.GetFullPath("Output.esp"), 1, PluginRole.Output);
        var before = JsonSerializer.SerializeToElement(new { EditorId = "Before", Items = new[] { "A", "B" } });
        var after = JsonSerializer.SerializeToElement(new { EditorId = "After", Items = new[] { "A", "C" } });
        var changes = Array.AsReadOnly(new[]
        {
            new SemanticChangeDescriptor("EditorId", SemanticChangeKind.ValueChanged),
            new SemanticChangeDescriptor("Items", SemanticChangeKind.ItemChanged, 1, 1),
        });
        var warnings = Array.AsReadOnly(new[]
        {
            new EngineWarning("writer_normalized", "The writer normalized one native value."),
        });
        var comparison = new FormListComparison(beforeContext, afterContext, before, after, changes, warnings);
        var workspace = CreateWorkspace(workspaceId, revision);
        workspace.Setup(candidate => candidate.CompareFormListAsync(
                It.IsAny<CompareFormListRequest>(),
                It.IsAny<CancellationToken>()))
            .Returns(ValueTask.FromResult(EngineResult<FormListComparison>.Success(
                comparison,
                workspaceId: workspaceId,
                resultRevision: revision)));
        var factory = CreateFactory(workspace.Object);

        await using var registry = new McpWorkspaceRegistry();
        var tools = new McpToolCatalog().CreateTools(registry, "protocol-test", factory.Object);
        await using var harness = await ProtocolHarness.CreateAsync(tools);
        await OpenWorkspaceAsync(harness.Client, workspaceId);
        var arguments = CreateCompareArguments(workspaceId, formKey, sourceModKey, outputModKey, "changes", 1);
        var first = GetResult(await harness.Client.CallToolAsync("creationsforge_formlist_compare", arguments));
        var firstPage = first.GetProperty("page");
        firstPage.GetProperty("changes")[0].GetProperty("fieldIdentifier").GetString().ShouldBe("EditorId");
        var cursor = firstPage.GetProperty("cursor").GetString();

        arguments["cursor"] = cursor;
        var second = GetResult(await harness.Client.CallToolAsync("creationsforge_formlist_compare", arguments));
        var secondChange = second.GetProperty("page").GetProperty("changes")[0];
        secondChange.GetProperty("fieldIdentifier").GetString().ShouldBe("Items");
        secondChange.GetProperty("beforePosition").GetInt32().ShouldBe(1);
        secondChange.GetProperty("afterPosition").GetInt32().ShouldBe(1);

        var beforeArguments = CreateCompareArguments(workspaceId, formKey, sourceModKey, outputModKey, "before", 50);
        beforeArguments["path"] = "/Items/1";
        var beforeValue = GetResult(await harness.Client.CallToolAsync("creationsforge_formlist_compare", beforeArguments))
            .GetProperty("page");
        beforeValue.GetProperty("kind").GetString().ShouldBe("string");
        beforeValue.GetProperty("value").GetString().ShouldBe("B");

        var warningArguments = CreateCompareArguments(workspaceId, formKey, sourceModKey, outputModKey, "warnings", 50);
        var warningPage = GetResult(await harness.Client.CallToolAsync("creationsforge_formlist_compare", warningArguments))
            .GetProperty("page");
        warningPage.GetProperty("warnings")[0].GetProperty("code").GetString().ShouldBe("writer_normalized");
    }

    /// <summary>Creates a deterministic workspace mock with registry-compatible identity and disposal behavior.</summary>
    /// <param name="workspaceId">The workspace identity.</param>
    /// <param name="revision">The open revision.</param>
    /// <returns>The configured workspace mock.</returns>
    private static Mock<IFormListWorkspace> CreateWorkspace(Guid workspaceId, WorkspaceRevision revision)
    {
        var workspace = new Mock<IFormListWorkspace>();
        workspace.SetupGet(candidate => candidate.WorkspaceId).Returns(workspaceId);
        workspace.SetupGet(candidate => candidate.Revision).Returns(() => revision);
        workspace.Setup(candidate => candidate.DisposeAsync()).Returns(ValueTask.CompletedTask);
        return workspace;
    }

    /// <summary>Creates a deterministic native factory for one workspace.</summary>
    /// <param name="workspace">The workspace returned after acquisition.</param>
    /// <returns>The configured factory mock.</returns>
    private static Mock<IFormListWorkspaceFactory> CreateFactory(IFormListWorkspace workspace)
    {
        var factory = new Mock<IFormListWorkspaceFactory>();
        factory.Setup(candidate => candidate.OpenAsync(
                It.IsAny<WorkspaceOpenRequest>(),
                It.IsAny<CancellationToken>()))
            .Returns(ValueTask.FromResult(EngineResult<IFormListWorkspace>.Success(workspace)));
        return factory;
    }

    /// <summary>Opens one test workspace through the SDK protocol using explicit native inputs.</summary>
    /// <param name="client">The connected SDK client.</param>
    /// <param name="workspaceId">The workspace identity.</param>
    /// <param name="cancellationToken">The token that bounds the protocol request.</param>
    /// <returns>A task that completes after successful protocol publication.</returns>
    /// <exception cref="OperationCanceledException">Thrown when the protocol request is cancelled.</exception>
    private static async Task OpenWorkspaceAsync(
        McpClient client,
        Guid workspaceId,
        CancellationToken cancellationToken = default)
    {
        var result = await client.CallToolAsync(
            "creationsforge_workspace_open",
            new Dictionary<string, object?>
            {
                ["workspaceId"] = workspaceId.ToString("D"),
                ["game"] = "starfield",
                ["release"] = "starfield",
                ["sourcePluginPath"] = Path.GetFullPath("Source.esm"),
                ["loadOrderPluginPaths"] = new[] { Path.GetFullPath("Source.esm") },
                ["dataDirectoryPath"] = Path.GetFullPath("Data"),
                ["stringDirectoryPaths"] = Array.Empty<string>(),
            },
            cancellationToken: cancellationToken);
        result.IsError.ShouldNotBe(true);
    }

    /// <summary>Creates a complete inspection argument object for one source context.</summary>
    /// <param name="workspaceId">The workspace identity.</param>
    /// <param name="formKey">The exact FormList identity.</param>
    /// <param name="sourceModKey">The exact containing source plugin.</param>
    /// <param name="path">The selected JSON Pointer.</param>
    /// <param name="maximumResults">The requested transport page size.</param>
    /// <returns>A mutable SDK argument dictionary.</returns>
    private static Dictionary<string, object?> CreateInspectArguments(
        Guid workspaceId,
        FormKey formKey,
        ModKey sourceModKey,
        string path,
        int maximumResults)
    {
        return new Dictionary<string, object?>
        {
            ["workspaceId"] = workspaceId.ToString("D"),
            ["selection"] = new Dictionary<string, object?>
            {
                ["formKey"] = formKey.ToString(),
                ["scope"] = "source",
                ["containingModKey"] = sourceModKey.ToString(),
            },
            ["path"] = path,
            ["maxResults"] = maximumResults,
        };
    }

    /// <summary>Creates a complete comparison argument object for two exact native contexts.</summary>
    /// <param name="workspaceId">The workspace identity.</param>
    /// <param name="formKey">The shared FormList identity.</param>
    /// <param name="sourceModKey">The prior containing source plugin.</param>
    /// <param name="outputModKey">The resulting containing output plugin.</param>
    /// <param name="section">The comparison section.</param>
    /// <param name="maximumResults">The requested page size.</param>
    /// <returns>A mutable SDK argument dictionary.</returns>
    private static Dictionary<string, object?> CreateCompareArguments(
        Guid workspaceId,
        FormKey formKey,
        ModKey sourceModKey,
        ModKey outputModKey,
        string section,
        int maximumResults)
    {
        return new Dictionary<string, object?>
        {
            ["workspaceId"] = workspaceId.ToString("D"),
            ["before"] = new Dictionary<string, object?>
            {
                ["formKey"] = formKey.ToString(),
                ["scope"] = "source",
                ["containingModKey"] = sourceModKey.ToString(),
            },
            ["after"] = new Dictionary<string, object?>
            {
                ["formKey"] = formKey.ToString(),
                ["scope"] = "staged_output",
                ["containingModKey"] = outputModKey.ToString(),
            },
            ["section"] = section,
            ["maxResults"] = maximumResults,
        };
    }

    /// <summary>Extracts a successful tool-specific structured result.</summary>
    /// <param name="result">The SDK call result.</param>
    /// <returns>The nested result object.</returns>
    private static JsonElement GetResult(CallToolResult result)
    {
        result.IsError.ShouldNotBe(true);
        result.StructuredContent.ShouldNotBeNull();
        var structured = result.StructuredContent.Value;
        structured.GetProperty("ok").GetBoolean().ShouldBeTrue();
        return structured.GetProperty("result");
    }

    /// <summary>Rewrites a cursor version and recomputes its checksum to exercise typed cursor decoding.</summary>
    /// <param name="cursor">The valid cursor to rewrite.</param>
    /// <param name="replacementVersion">The raw JSON number used as the replacement version.</param>
    /// <returns>A checksum-valid cursor with an invalid typed version.</returns>
    private static string RewriteCursorVersion(string cursor, string replacementVersion)
    {
        var base64 = cursor.Replace('-', '+').Replace('_', '/');
        base64 = base64.PadRight(base64.Length + ((4 - (base64.Length % 4)) % 4), '=');
        var decoded = Convert.FromBase64String(base64);
        const int checksumLength = 32;
        var payload = Encoding.UTF8.GetString(decoded, 0, decoded.Length - checksumLength);
        payload = payload.Replace("\"version\":1", $"\"version\":{replacementVersion}", StringComparison.Ordinal);
        var payloadBytes = Encoding.UTF8.GetBytes(payload);
        var rewritten = new byte[payloadBytes.Length + checksumLength];
        payloadBytes.CopyTo(rewritten, 0);
        SHA256.HashData(payloadBytes, rewritten.AsSpan(payloadBytes.Length, checksumLength));
        return Convert.ToBase64String(rewritten)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    /// <summary>Owns a real in-process SDK stream server and connected SDK client.</summary>
    private sealed class ProtocolHarness : IAsyncDisposable
    {
        /// <summary>The running generic host.</summary>
        private readonly IHost Host;

        /// <summary>Gets the connected SDK client.</summary>
        internal McpClient Client { get; }

        /// <summary>Initializes ownership of the connected client and server host.</summary>
        /// <param name="host">The running server host.</param>
        /// <param name="client">The connected SDK client.</param>
        private ProtocolHarness(IHost host, McpClient client)
        {
            Host = host;
            Client = client;
        }

        /// <summary>Starts a real SDK server over cross-connected in-memory streams.</summary>
        /// <param name="tools">The exact tool instances registered on the server.</param>
        /// <param name="cancellationToken">The token that bounds host startup and client initialization.</param>
        /// <returns>A connected client/server harness.</returns>
        /// <exception cref="OperationCanceledException">Thrown when startup or client initialization is cancelled.</exception>
        internal static async Task<ProtocolHarness> CreateAsync(
            IReadOnlyList<ModelContextProtocol.Server.McpServerTool> tools,
            CancellationToken cancellationToken = default)
        {
            var clientToServer = new Pipe();
            var serverToClient = new Pipe();
            var serverInput = clientToServer.Reader.AsStream();
            var serverOutput = serverToClient.Writer.AsStream();
            var clientInput = serverToClient.Reader.AsStream();
            var clientOutput = clientToServer.Writer.AsStream();
            var builder = Microsoft.Extensions.Hosting.Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
            {
                Args = [],
                ApplicationName = typeof(McpReadToolProtocolTests).Assembly.GetName().Name,
            });
            builder.Logging.ClearProviders();
            builder.Services
                .AddMcpServer(options =>
                {
                    options.ServerInfo = new Implementation
                    {
                        Name = "CreationsForge test",
                        Version = "1.0.0-test",
                    };
                })
                .WithStreamServerTransport(serverInput, serverOutput)
                .WithTools(tools);
            var host = builder.Build();
            try
            {
                await host.StartAsync(cancellationToken);
                var client = await McpClient.CreateAsync(
                    new StreamClientTransport(clientOutput, clientInput),
                    cancellationToken: cancellationToken);
                return new ProtocolHarness(host, client);
            }
            catch
            {
                using var cleanupSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                await host.StopAsync(cleanupSource.Token)
                    .ConfigureAwait(
                        ConfigureAwaitOptions.ContinueOnCapturedContext |
                        ConfigureAwaitOptions.SuppressThrowing);
                host.Dispose();
                throw;
            }
        }

        /// <summary>Disposes the client transport before stopping and disposing the server host.</summary>
        /// <returns>A task that completes after both protocol endpoints stop.</returns>
        public async ValueTask DisposeAsync()
        {
            await Client.DisposeAsync();
            await Host.StopAsync();
            Host.Dispose();
        }
    }
}
