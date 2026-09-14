using System.Text.Json;
using System.Text.Json.Nodes;
using CreationsForge.Bootstrap.Composition;
using CreationsForge.Mcp;
using CreationsForge.TestSupport;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Enums;
using CreationsForge.UnitTests.Engine.Fallout4;
using CreationsForge.UnitTests.Engine.Skyrim;
using CreationsForge.UnitTests.Engine.Starfield;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Shouldly;

namespace CreationsForge.UnitTests.McpHost;

/// <summary>
/// Verifies that the production stdio route exposes the real three-game plugin read engine.
/// </summary>
public sealed class McpPluginStdioProtocolTests
{
    /// <summary>The exact tools exposed when the production host owns a workspace factory.</summary>
    private static readonly string[] ExpectedToolNames =
    [
        "creationsforge_server_info",
        "creationsforge_workspace_open",
        "creationsforge_workspace_close",
        "creationsforge_plugins_list",
        "creationsforge_formlists_list",
        "creationsforge_references_search",
        "creationsforge_formlist_inspect",
        "creationsforge_formlist_compare",
        "creationsforge_workspace_state",
        "creationsforge_metadata_read",
        "creationsforge_save",
        "creationsforge_save_recover",
        "creationsforge_save_repair",
        "creationsforge_output_recovery_resolve",
        "creationsforge_output_select",
        "creationsforge_formlist_begin_edit",
        "creationsforge_formlist_apply_edit",
        "creationsforge_workspace_preview",
        "creationsforge_workspace_discard",
        "creationsforge_output_reopen",
        "creationsforge_formlist_edit_schemas_list",
        "creationsforge_formlist_edit_schema_read",
    ];

    /// <summary>Verifies production discovery, plugin reads, recursive paging, errors, closure, and source preservation.</summary>
    /// <param name="game">The generated plugin fixture family exercised through the child process.</param>
    [Theory]
    [InlineData(SupportedGame.Starfield)]
    [InlineData(SupportedGame.Fallout4)]
    [InlineData(SupportedGame.Skyrim)]
    public async Task ProductionStdioHost_ExposesRealPluginReadsForEverySupportedGame(SupportedGame game)
    {
        using var timeoutSource = CancellationTokenSource.CreateLinkedTokenSource(TestContext.Current.CancellationToken);
        timeoutSource.CancelAfter(TimeSpan.FromSeconds(45));
        var cancellationToken = timeoutSource.Token;
        using var fixture = PluginMcpFixture.Create(game);
        var sourceArtifacts = fixture.SnapshotArtifacts();

        await using var directServices = EngineComposition.Create();
        var directOpen = await directServices.WorkspaceFactory.OpenAsync(
            fixture.CreateOpenRequest(Guid.NewGuid()),
            cancellationToken);
        directOpen.Succeeded.ShouldBeTrue(directOpen.Error?.Message);
        await using var directWorkspace = directOpen.Value.ShouldNotBeNull();
        var expectedPlugins = GetValue(await directWorkspace.ListPluginsAsync(cancellationToken));
        var expectedLists = GetValue(await directWorkspace.ListFormListsAsync(RecordScope.Source, cancellationToken));
        var expectedMatches = await ReadDirectReferenceMatchesAsync(directWorkspace, cancellationToken);
        var sourceSelection = new ReferenceRequest(fixture.SourceListFormKey, RecordScope.Source);
        var winningSelection = new ReferenceRequest(fixture.SourceListFormKey, RecordScope.WinningOverrides);
        var deletedSelection = new ReferenceRequest(fixture.DeletedListFormKey, RecordScope.WinningOverrides);
        var expectedSource = GetValue(await directWorkspace.ReadFormListViewAsync(sourceSelection, cancellationToken));
        var expectedWinning = GetValue(await directWorkspace.ReadFormListViewAsync(winningSelection, cancellationToken));
        var expectedDeleted = GetValue(await directWorkspace.ReadFormListViewAsync(deletedSelection, cancellationToken));
        expectedSource.Context.Status.ShouldBe(ReferenceResolutionStatus.Resolved);
        expectedSource.Context.ContainingModKey.ShouldBe(fixture.SourceModKey);
        expectedWinning.Context.Status.ShouldBe(ReferenceResolutionStatus.Resolved);
        expectedWinning.Context.ContainingModKey.ShouldBe(fixture.PatchModKey);
        expectedDeleted.Context.Status.ShouldBe(ReferenceResolutionStatus.Deleted);
        expectedDeleted.Context.ContainingModKey.ShouldBe(fixture.PatchModKey);
        expectedMatches.ShouldNotBeEmpty();
        var expectedComparison = GetValue(await directWorkspace.CompareFormListAsync(
            new CompareFormListRequest(sourceSelection, winningSelection),
            cancellationToken));
        expectedComparison.Changes.ShouldNotBeEmpty();

        await using var processFixture = await McpStdioProcessFixture.StartAsync(
            typeof(McpHostRunner).Assembly.Location,
            cancellationToken);
        var client = processFixture.Client;

        await AssertToolDiscoveryAsync(client, cancellationToken);
        AssertActiveWorkspaceCount(await GetServerInfoAsync(client, cancellationToken), 0);

        var workspaceId = Guid.NewGuid();
        var openRequest = fixture.CreateOpenRequest(workspaceId);
        var openResult = GetResult(await client.CallToolAsync(
            "creationsforge_workspace_open",
            CreateOpenArguments(openRequest),
            cancellationToken: cancellationToken));
        openResult.GetProperty("workspaceId").GetString().ShouldBe(workspaceId.ToString("D"));
        AssertActiveWorkspaceCount(await GetServerInfoAsync(client, cancellationToken), 1);

        var actualPlugins = await ReadArrayPagesAsync(
            client,
            "creationsforge_plugins_list",
            new Dictionary<string, object?>
            {
                ["workspaceId"] = workspaceId.ToString("D"),
                ["maxResults"] = 1,
            },
            "plugins",
            cancellationToken);
        AssertPluginsMatch(expectedPlugins, actualPlugins);

        var actualLists = await ReadArrayPagesAsync(
            client,
            "creationsforge_formlists_list",
            new Dictionary<string, object?>
            {
                ["workspaceId"] = workspaceId.ToString("D"),
                ["scope"] = "source",
                ["maxResults"] = 1,
            },
            "formLists",
            cancellationToken);
        AssertFormListsMatch(expectedLists, actualLists);

        var actualMatches = await ReadArrayPagesAsync(
            client,
            "creationsforge_references_search",
            new Dictionary<string, object?>
            {
                ["workspaceId"] = workspaceId.ToString("D"),
                ["query"] = "SharedList",
                ["scope"] = "all_contexts",
                ["maxResults"] = 1,
            },
            "matches",
            cancellationToken);
        AssertReferenceMatches(expectedMatches, actualMatches);

        await AssertInspectionMatchesAsync(client, workspaceId, sourceSelection, expectedSource, cancellationToken);
        await AssertInspectionMatchesAsync(client, workspaceId, winningSelection, expectedWinning, cancellationToken);
        await AssertInspectionMatchesAsync(client, workspaceId, deletedSelection, expectedDeleted, cancellationToken);
        await AssertComparisonMatchesAsync(client, workspaceId, sourceSelection, winningSelection, expectedComparison, cancellationToken);

        var invalidWorkspace = await client.CallToolAsync(
            "creationsforge_plugins_list",
            new Dictionary<string, object?>
            {
                ["workspaceId"] = "AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA",
            },
            cancellationToken: cancellationToken);
        GetErrorCode(invalidWorkspace).ShouldBe("invalid_arguments");

        var closeResult = GetResult(await client.CallToolAsync(
            "creationsforge_workspace_close",
            new Dictionary<string, object?>
            {
                ["workspaceId"] = workspaceId.ToString("D"),
            },
            cancellationToken: cancellationToken));
        closeResult.GetProperty("closed").GetBoolean().ShouldBeTrue();

        var inactiveWorkspace = await client.CallToolAsync(
            "creationsforge_plugins_list",
            new Dictionary<string, object?>
            {
                ["workspaceId"] = "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
            },
            cancellationToken: cancellationToken);
        GetErrorCode(inactiveWorkspace).ShouldBe("workspace_disposed");

        var missingSourceRequest = fixture.CreateOpenRequest(Guid.NewGuid());
        var missingSourcePath = Path.Combine(missingSourceRequest.DataDirectoryPath, "MissingPluginSource.esm");
        var missingRequest = new WorkspaceOpenRequest(
            missingSourceRequest.WorkspaceId,
            missingSourceRequest.Game,
            missingSourceRequest.Release,
            missingSourcePath,
            [missingSourcePath],
            missingSourceRequest.DataDirectoryPath,
            missingSourceRequest.StringDirectoryPaths);
        var directEngineFailure = await directServices.WorkspaceFactory.OpenAsync(missingRequest, cancellationToken);
        directEngineFailure.Succeeded.ShouldBeFalse();
        directEngineFailure.Error.ShouldNotBeNull().Code.ShouldBe(EngineErrorCode.InvalidRequest);
        var toolFailure = await client.CallToolAsync(
            "creationsforge_workspace_open",
            CreateOpenArguments(missingRequest),
            cancellationToken: cancellationToken);
        GetErrorCode(toolFailure).ShouldBe("invalid_request");
        AssertActiveWorkspaceCount(await GetServerInfoAsync(client, cancellationToken), 0);
        AssertArtifactsEqual(sourceArtifacts, fixture.SnapshotArtifacts());

        var completion = await processFixture.CompleteAsync(cancellationToken);
        completion.ExitCode.ShouldBe(0, completion.StandardError);
    }

    /// <summary>Verifies exact production tool discovery and capability annotations.</summary>
    /// <param name="client">The initialized production stdio client.</param>
    /// <param name="cancellationToken">The bounded test token.</param>
    /// <returns>A task that completes after tool metadata has been checked.</returns>
    private static async Task AssertToolDiscoveryAsync(McpClient client, CancellationToken cancellationToken)
    {
        var tools = await client.ListToolsAsync(cancellationToken: cancellationToken);

        tools.Count.ShouldBe(ExpectedToolNames.Length);
        tools.Select(tool => tool.Name).Distinct(StringComparer.Ordinal).Count().ShouldBe(ExpectedToolNames.Length);
        tools.Select(tool => tool.Name).OrderBy(name => name, StringComparer.Ordinal)
            .ShouldBe(ExpectedToolNames.OrderBy(name => name, StringComparer.Ordinal));
        foreach (var tool in tools)
        {
            tool.JsonSchema.GetProperty("type").GetString().ShouldBe("object");
            tool.JsonSchema.GetProperty("additionalProperties").GetBoolean().ShouldBeFalse();
            tool.ProtocolTool.OutputSchema.ShouldNotBeNull();
            var annotations = tool.ProtocolTool.Annotations.ShouldNotBeNull();
            annotations.DestructiveHint.ShouldBe(tool.Name is "creationsforge_save" or "creationsforge_save_repair" or "creationsforge_workspace_discard" or "creationsforge_output_reopen");
            annotations.OpenWorldHint.ShouldBe(false);
            annotations.ReadOnlyHint.ShouldBe(tool.Name is "creationsforge_server_info" or "creationsforge_plugins_list" or "creationsforge_formlists_list" or "creationsforge_references_search" or "creationsforge_formlist_inspect" or "creationsforge_formlist_compare" or "creationsforge_workspace_state" or "creationsforge_metadata_read" or "creationsforge_save_recover" or "creationsforge_workspace_preview" or "creationsforge_formlist_edit_schemas_list" or "creationsforge_formlist_edit_schema_read");
            annotations.IdempotentHint.ShouldBe(tool.Name != "creationsforge_workspace_open");
        }
    }

    /// <summary>Reads current server lifecycle metadata through the production protocol.</summary>
    /// <param name="client">The initialized production stdio client.</param>
    /// <param name="cancellationToken">The bounded test token.</param>
    /// <returns>The successful server-information payload.</returns>
    private static async Task<JsonElement> GetServerInfoAsync(McpClient client, CancellationToken cancellationToken)
    {
        return GetResult(await client.CallToolAsync(
            "creationsforge_server_info",
            new Dictionary<string, object?>(),
            cancellationToken: cancellationToken));
    }

    /// <summary>Checks the registry count and exact listed identity cardinality.</summary>
    /// <param name="serverInfo">The successful server-information payload.</param>
    /// <param name="expectedCount">The expected active registry count.</param>
    private static void AssertActiveWorkspaceCount(JsonElement serverInfo, int expectedCount)
    {
        var lifecycle = serverInfo.GetProperty("lifecycle");
        lifecycle.GetProperty("activeWorkspaceCount").GetInt32().ShouldBe(expectedCount);
        lifecycle.GetProperty("activeWorkspaceIds").GetArrayLength().ShouldBe(expectedCount);
    }

    /// <summary>Creates exact MCP workspace-open arguments from an engine request.</summary>
    /// <param name="request">The explicit plugin request.</param>
    /// <returns>A closed protocol argument dictionary.</returns>
    private static Dictionary<string, object?> CreateOpenArguments(WorkspaceOpenRequest request)
    {
        return new Dictionary<string, object?>
        {
            ["workspaceId"] = request.WorkspaceId.ToString("D"),
            ["game"] = GameName(request.Game),
            ["release"] = ReleaseName(request.Release),
            ["sourcePluginPath"] = request.SourcePluginPath,
            ["loadOrderPluginPaths"] = request.LoadOrderPluginPaths,
            ["dataDirectoryPath"] = request.DataDirectoryPath,
            ["stringDirectoryPaths"] = request.StringDirectoryPaths,
            ["recordTextLanguage"] = request.RecordTextLanguage.ToString().ToLowerInvariant(),
        };
    }

    /// <summary>Reads every cursor page from a tool result array.</summary>
    /// <param name="client">The initialized production stdio client.</param>
    /// <param name="toolName">The exact tool name.</param>
    /// <param name="baseArguments">The arguments shared by every page.</param>
    /// <param name="arrayProperty">The result-array property to concatenate.</param>
    /// <param name="cancellationToken">The bounded test token.</param>
    /// <returns>Detached array entries in protocol order.</returns>
    private static async Task<IReadOnlyList<JsonElement>> ReadArrayPagesAsync(
        McpClient client,
        string toolName,
        IReadOnlyDictionary<string, object?> baseArguments,
        string arrayProperty,
        CancellationToken cancellationToken)
    {
        var values = new List<JsonElement>();
        string? cursor = null;
        do
        {
            var arguments = baseArguments.ToDictionary(
                pair => pair.Key,
                pair => pair.Value,
                StringComparer.Ordinal);
            if (cursor is not null)
            {
                arguments["cursor"] = cursor;
            }

            var result = GetResult(await client.CallToolAsync(toolName, arguments, cancellationToken: cancellationToken));
            values.AddRange(result.GetProperty(arrayProperty).EnumerateArray().Select(item => item.Clone()));
            cursor = ReadNullableString(result.GetProperty("cursor"));
        }
        while (cursor is not null);

        return values;
    }

    /// <summary>Reads direct reference pages using the same query and page size as the protocol request.</summary>
    /// <param name="workspace">The independently opened real workspace.</param>
    /// <param name="cancellationToken">The bounded test token.</param>
    /// <returns>All direct plugin matches in deterministic page order.</returns>
    private static async Task<IReadOnlyList<ReferenceSearchMatch>> ReadDirectReferenceMatchesAsync(
        IFormListWorkspace workspace,
        CancellationToken cancellationToken)
    {
        var matches = new List<ReferenceSearchMatch>();
        string? cursor = null;
        do
        {
            var page = GetValue(await workspace.SearchReferencesAsync(
                new ReferenceSearchRequest("SharedList", 1, cursor, RecordScope.AllContexts),
                cancellationToken));
            matches.AddRange(page.Matches);
            cursor = page.ContinuationToken;
        }
        while (cursor is not null);

        return matches;
    }

    /// <summary>Checks complete MCP plugin projections against public direct engine results.</summary>
    /// <param name="expected">The direct engine plugin list.</param>
    /// <param name="actual">The recursively paged MCP plugin list.</param>
    private static void AssertPluginsMatch(IReadOnlyList<PluginSummary> expected, IReadOnlyList<JsonElement> actual)
    {
        actual.Count.ShouldBe(expected.Count);
        for (var index = 0; index < expected.Count; index++)
        {
            actual[index].GetProperty("modKey").GetString().ShouldBe(expected[index].ModKey.ToString());
            actual[index].GetProperty("path").GetString().ShouldBe(expected[index].Path);
            actual[index].GetProperty("loadOrderIndex").GetInt32().ShouldBe(expected[index].LoadOrderIndex);
            actual[index].GetProperty("role").GetString().ShouldBe(RoleName(expected[index].Role));
            ReadNullableInt64(actual[index].GetProperty("recordCount")).ShouldBe(expected[index].RecordCount);
            ReadNullableInt64(actual[index].GetProperty("uniqueRecordContributionCount"))
                .ShouldBe(expected[index].UniqueRecordContributionCount);
        }
    }

    /// <summary>Reads a nullable 64-bit integer from a closed MCP projection.</summary>
    /// <param name="element">The JSON number or null value.</param>
    /// <returns>The projected integer, or <see langword="null"/>.</returns>
    private static long? ReadNullableInt64(JsonElement element)
    {
        return element.ValueKind == JsonValueKind.Null ? null : element.GetInt64();
    }

    /// <summary>Checks complete MCP FormList summary projections against public direct engine results.</summary>
    /// <param name="expected">The direct engine FormList summaries.</param>
    /// <param name="actual">The recursively paged MCP FormList summaries.</param>
    private static void AssertFormListsMatch(IReadOnlyList<FormListSummary> expected, IReadOnlyList<JsonElement> actual)
    {
        actual.Count.ShouldBe(expected.Count);
        for (var index = 0; index < expected.Count; index++)
        {
            var item = actual[index];
            item.GetProperty("formKey").GetString().ShouldBe(expected[index].FormKey.ToString());
            ReadNullableString(item.GetProperty("editorId")).ShouldBe(expected[index].EditorId);
            item.GetProperty("overrideCount").GetInt32().ShouldBe(expected[index].OverrideCount);
            item.GetProperty("scope").GetString().ShouldBe(ScopeName(expected[index].Scope));
            ReadNullableString(item.GetProperty("containingModKey")).ShouldBe(expected[index].ContainingModKey?.ToString());
            ReadNullableString(item.GetProperty("sourcePath")).ShouldBe(expected[index].SourcePath);
            ReadNullableInt32(item.GetProperty("loadOrderIndex")).ShouldBe(expected[index].LoadOrderIndex);
            ReadNullableString(item.GetProperty("role")).ShouldBe(expected[index].Role is { } role ? RoleName(role) : null);
        }
    }

    /// <summary>Checks complete MCP reference projections against public direct engine search results.</summary>
    /// <param name="expected">The direct plugin search matches.</param>
    /// <param name="actual">The recursively paged MCP matches.</param>
    private static void AssertReferenceMatches(IReadOnlyList<ReferenceSearchMatch> expected, IReadOnlyList<JsonElement> actual)
    {
        actual.Count.ShouldBe(expected.Count);
        for (var index = 0; index < expected.Count; index++)
        {
            var item = actual[index];
            item.GetProperty("formKey").GetString().ShouldBe(expected[index].FormKey.ToString());
            item.GetProperty("recordType").GetString().ShouldBe(expected[index].RecordType);
            ReadNullableString(item.GetProperty("editorId")).ShouldBe(expected[index].EditorId);
            ReadNullableString(item.GetProperty("containingModKey")).ShouldBe(expected[index].ContainingModKey?.ToString());
            ReadNullableString(item.GetProperty("sourcePath")).ShouldBe(expected[index].SourcePath);
            ReadNullableInt32(item.GetProperty("loadOrderIndex")).ShouldBe(expected[index].LoadOrderIndex);
            ReadNullableString(item.GetProperty("role")).ShouldBe(expected[index].Role is { } role ? RoleName(role) : null);
            item.GetProperty("isDeleted").GetBoolean().ShouldBe(expected[index].IsDeleted);
        }
    }

    /// <summary>Reconstructs one complete inspect view and compares its context and JSON to a direct engine read.</summary>
    /// <param name="client">The initialized production stdio client.</param>
    /// <param name="workspaceId">The MCP workspace identity.</param>
    /// <param name="selection">The exact contextual selection.</param>
    /// <param name="expected">The corresponding direct real-engine view.</param>
    /// <param name="cancellationToken">The bounded test token.</param>
    /// <returns>A task that completes after recursive paging and comparison.</returns>
    private static async Task AssertInspectionMatchesAsync(
        McpClient client,
        Guid workspaceId,
        ReferenceRequest selection,
        FormListReadView expected,
        CancellationToken cancellationToken)
    {
        var root = GetResult(await client.CallToolAsync(
            "creationsforge_formlist_inspect",
            CreateInspectArguments(workspaceId, selection, string.Empty),
            cancellationToken: cancellationToken));
        AssertContextMatches(expected.Context, root.GetProperty("context"));

        var actual = await ReconstructJsonAsync(
            path => ReadInspectPagesAsync(client, workspaceId, selection, path, cancellationToken),
            string.Empty);
        var expectedNode = JsonNode.Parse(expected.Record.ShouldNotBeNull().GetRawText());
        JsonNode.DeepEquals(expectedNode, actual).ShouldBeTrue();
    }

    /// <summary>Checks source-to-winning comparison contexts and semantic changes against a direct engine comparison.</summary>
    /// <param name="client">The initialized production stdio client.</param>
    /// <param name="workspaceId">The MCP workspace identity.</param>
    /// <param name="before">The source-context selection.</param>
    /// <param name="after">The winning-context selection.</param>
    /// <param name="expected">The corresponding public direct engine comparison.</param>
    /// <param name="cancellationToken">The bounded test token.</param>
    /// <returns>A task that completes after the comparison result has been checked.</returns>
    private static async Task AssertComparisonMatchesAsync(
        McpClient client,
        Guid workspaceId,
        ReferenceRequest before,
        ReferenceRequest after,
        FormListComparison expected,
        CancellationToken cancellationToken)
    {
        var changes = new List<JsonElement>();
        string? cursor = null;
        JsonElement firstResult = default;
        do
        {
            var arguments = CreateCompareArguments(workspaceId, before, after, "changes", string.Empty);
            if (cursor is not null)
            {
                arguments["cursor"] = cursor;
            }

            var result = GetResult(await client.CallToolAsync(
                "creationsforge_formlist_compare",
                arguments,
                cancellationToken: cancellationToken));
            if (firstResult.ValueKind == JsonValueKind.Undefined)
            {
                firstResult = result.Clone();
            }

            var page = result.GetProperty("page");
            changes.AddRange(page.GetProperty("changes").EnumerateArray().Select(item => item.Clone()));
            cursor = ReadNullableString(page.GetProperty("cursor"));
        }
        while (cursor is not null);

        AssertContextMatches(expected.BeforeContext, firstResult.GetProperty("beforeContext"));
        AssertContextMatches(expected.AfterContext, firstResult.GetProperty("afterContext"));
        changes.Count.ShouldBe(expected.Changes.Count);
        for (var index = 0; index < expected.Changes.Count; index++)
        {
            changes[index].GetProperty("fieldIdentifier").GetString().ShouldBe(expected.Changes[index].FieldIdentifier);
            changes[index].GetProperty("kind").GetString().ShouldBe(ChangeKindName(expected.Changes[index].Kind));
            ReadNullableInt32(changes[index].GetProperty("beforePosition")).ShouldBe(expected.Changes[index].BeforePosition);
            ReadNullableInt32(changes[index].GetProperty("afterPosition")).ShouldBe(expected.Changes[index].AfterPosition);
        }

    }

    /// <summary>Reads every MCP inspection page for one JSON Pointer.</summary>
    /// <param name="client">The initialized production stdio client.</param>
    /// <param name="workspaceId">The MCP workspace identity.</param>
    /// <param name="selection">The exact contextual selection.</param>
    /// <param name="path">The canonical JSON Pointer.</param>
    /// <param name="cancellationToken">The bounded test token.</param>
    /// <returns>All detached pages for the selected immediate children or scalar.</returns>
    private static Task<IReadOnlyList<JsonElement>> ReadInspectPagesAsync(
        McpClient client,
        Guid workspaceId,
        ReferenceRequest selection,
        string path,
        CancellationToken cancellationToken)
    {
        return ReadJsonPagesAsync(
            client,
            "creationsforge_formlist_inspect",
            CreateInspectArguments(workspaceId, selection, path),
            cancellationToken);
    }

    /// <summary>Reads every cursor-bound JSON page for an inspect or compare request.</summary>
    /// <param name="client">The initialized production stdio client.</param>
    /// <param name="toolName">The inspect or compare tool name.</param>
    /// <param name="baseArguments">The pointer request shared by each page.</param>
    /// <param name="cancellationToken">The bounded test token.</param>
    /// <returns>All page objects in offset order.</returns>
    private static async Task<IReadOnlyList<JsonElement>> ReadJsonPagesAsync(
        McpClient client,
        string toolName,
        IReadOnlyDictionary<string, object?> baseArguments,
        CancellationToken cancellationToken)
    {
        var pages = new List<JsonElement>();
        string? cursor = null;
        do
        {
            var arguments = baseArguments.ToDictionary(
                pair => pair.Key,
                pair => pair.Value,
                StringComparer.Ordinal);
            if (cursor is not null)
            {
                arguments["cursor"] = cursor;
            }

            var result = GetResult(await client.CallToolAsync(toolName, arguments, cancellationToken: cancellationToken));
            var page = result.GetProperty("page").Clone();
            pages.Add(page);
            cursor = page.TryGetProperty("cursor", out var cursorElement)
                ? ReadNullableString(cursorElement)
                : null;
        }
        while (cursor is not null);

        return pages;
    }

    /// <summary>Reconstructs a complete JSON value by recursively following immediate-child pointer pages.</summary>
    /// <param name="pageReader">Reads every page for one canonical JSON Pointer.</param>
    /// <param name="path">The current canonical JSON Pointer.</param>
    /// <returns>The complete JSON value at the selected pointer.</returns>
    private static async Task<JsonNode?> ReconstructJsonAsync(
        Func<string, Task<IReadOnlyList<JsonElement>>> pageReader,
        string path)
    {
        var pages = await pageReader(path);
        pages.ShouldNotBeEmpty();
        var kind = pages[0].GetProperty("kind").GetString();
        if (kind == "object")
        {
            var result = new JsonObject();
            foreach (var child in pages.SelectMany(page => page.GetProperty("children").EnumerateArray()).ToArray())
            {
                result[child.GetProperty("property").GetString().ShouldNotBeNull()] =
                    await ReconstructJsonAsync(pageReader, child.GetProperty("path").GetString().ShouldNotBeNull());
            }

            return result;
        }

        if (kind == "array")
        {
            var result = new JsonArray();
            foreach (var child in pages.SelectMany(page => page.GetProperty("children").EnumerateArray()).ToArray())
            {
                child.GetProperty("index").GetInt32().ShouldBe(result.Count);
                result.Add(await ReconstructJsonAsync(pageReader, child.GetProperty("path").GetString().ShouldNotBeNull()));
            }

            return result;
        }

        if (kind == "string")
        {
            return JsonValue.Create(string.Concat(pages.Select(page => page.GetProperty("value").GetString())));
        }

        if (kind == "number")
        {
            return JsonNode.Parse(pages.ShouldHaveSingleItem().GetProperty("rawValue").GetString().ShouldNotBeNull());
        }

        if (kind == "boolean")
        {
            return JsonValue.Create(pages.ShouldHaveSingleItem().GetProperty("value").GetBoolean());
        }

        if (kind == "null")
        {
            pages.ShouldHaveSingleItem();
            return null;
        }

        throw new InvalidOperationException($"Unexpected paged JSON kind '{kind}'.");
    }

    /// <summary>Creates a closed inspect request for one JSON Pointer with single-entry paging.</summary>
    /// <param name="workspaceId">The MCP workspace identity.</param>
    /// <param name="selection">The exact plugin selection.</param>
    /// <param name="path">The canonical JSON Pointer.</param>
    /// <returns>The protocol argument dictionary.</returns>
    private static Dictionary<string, object?> CreateInspectArguments(
        Guid workspaceId,
        ReferenceRequest selection,
        string path)
    {
        return new Dictionary<string, object?>
        {
            ["workspaceId"] = workspaceId.ToString("D"),
            ["selection"] = CreateSelectionArguments(selection),
            ["path"] = path,
            ["maxResults"] = 8,
        };
    }

    /// <summary>Creates a closed compare request for one section and JSON Pointer with single-entry paging.</summary>
    /// <param name="workspaceId">The MCP workspace identity.</param>
    /// <param name="before">The prior plugin selection.</param>
    /// <param name="after">The resulting plugin selection.</param>
    /// <param name="section">The comparison section.</param>
    /// <param name="path">The canonical JSON Pointer, ignored for non-view sections.</param>
    /// <returns>The protocol argument dictionary.</returns>
    private static Dictionary<string, object?> CreateCompareArguments(
        Guid workspaceId,
        ReferenceRequest before,
        ReferenceRequest after,
        string section,
        string path)
    {
        var arguments = new Dictionary<string, object?>
        {
            ["workspaceId"] = workspaceId.ToString("D"),
            ["before"] = CreateSelectionArguments(before),
            ["after"] = CreateSelectionArguments(after),
            ["section"] = section,
            ["maxResults"] = 1,
        };
        if (section is "before" or "after")
        {
            arguments["path"] = path;
        }

        return arguments;
    }

    /// <summary>Creates one closed contextual-selection object.</summary>
    /// <param name="selection">The exact engine selection.</param>
    /// <returns>The protocol selection dictionary.</returns>
    private static Dictionary<string, object?> CreateSelectionArguments(ReferenceRequest selection)
    {
        var arguments = new Dictionary<string, object?>
        {
            ["formKey"] = selection.FormKey.ToString(),
            ["scope"] = ScopeName(selection.Scope),
        };
        if (selection.ContainingModKey is { } containingModKey)
        {
            arguments["containingModKey"] = containingModKey.ToString();
        }

        return arguments;
    }

    /// <summary>Checks one projected context against its exact public engine contract.</summary>
    /// <param name="expected">The direct engine context.</param>
    /// <param name="actual">The MCP context projection.</param>
    private static void AssertContextMatches(FormListContext expected, JsonElement actual)
    {
        actual.GetProperty("formKey").GetString().ShouldBe(expected.Selection.FormKey.ToString());
        actual.GetProperty("scope").GetString().ShouldBe(ScopeName(expected.Selection.Scope));
        ReadNullableString(actual.GetProperty("requestedContainingModKey")).ShouldBe(expected.Selection.ContainingModKey?.ToString());
        actual.GetProperty("status").GetString().ShouldBe(StatusName(expected.Status));
        ReadNullableString(actual.GetProperty("containingModKey")).ShouldBe(expected.ContainingModKey?.ToString());
        ReadNullableString(actual.GetProperty("sourcePath")).ShouldBe(expected.Path);
        ReadNullableInt32(actual.GetProperty("loadOrderIndex")).ShouldBe(expected.LoadOrderIndex);
        ReadNullableString(actual.GetProperty("role")).ShouldBe(expected.Role is { } role ? RoleName(role) : null);
    }

    /// <summary>Returns a successful structured tool result.</summary>
    /// <param name="result">The SDK tool response.</param>
    /// <returns>The nested successful result value.</returns>
    private static JsonElement GetResult(CallToolResult result)
    {
        result.IsError.ShouldNotBe(true);
        result.StructuredContent.ShouldNotBeNull();
        var content = result.StructuredContent.Value;
        content.GetProperty("ok").GetBoolean().ShouldBeTrue();
        return content.GetProperty("result");
    }

    /// <summary>Returns the stable error code from a failed structured tool result.</summary>
    /// <param name="result">The SDK tool response.</param>
    /// <returns>The protocol error code.</returns>
    private static string GetErrorCode(CallToolResult result)
    {
        result.IsError.ShouldBe(true);
        result.StructuredContent.ShouldNotBeNull();
        var content = result.StructuredContent.Value;
        content.GetProperty("ok").GetBoolean().ShouldBeFalse();
        return content.GetProperty("error").GetProperty("code").GetString().ShouldNotBeNull();
    }

    /// <summary>Returns a non-null value from a successful engine result.</summary>
    /// <typeparam name="T">The engine result value type.</typeparam>
    /// <param name="result">The engine operation result.</param>
    /// <returns>The successful non-null value.</returns>
    private static T GetValue<T>(EngineResult<T> result)
        where T : class
    {
        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        return result.Value.ShouldNotBeNull();
    }

    /// <summary>Reads a protocol string while preserving JSON null.</summary>
    /// <param name="element">The string-or-null element.</param>
    /// <returns>The string value, or <see langword="null"/>.</returns>
    private static string? ReadNullableString(JsonElement element)
    {
        return element.ValueKind == JsonValueKind.Null ? null : element.GetString();
    }

    /// <summary>Reads a protocol integer while preserving JSON null.</summary>
    /// <param name="element">The integer-or-null element.</param>
    /// <returns>The integer value, or <see langword="null"/>.</returns>
    private static int? ReadNullableInt32(JsonElement element)
    {
        return element.ValueKind == JsonValueKind.Null ? null : element.GetInt32();
    }

    /// <summary>Checks exact source artifact paths and bytes.</summary>
    /// <param name="expected">The snapshot captured before MCP access.</param>
    /// <param name="actual">The snapshot captured after MCP closure.</param>
    private static void AssertArtifactsEqual(
        IReadOnlyDictionary<string, byte[]> expected,
        IReadOnlyDictionary<string, byte[]> actual)
    {
        actual.Keys.ShouldBe(expected.Keys, ignoreOrder: false);
        foreach (var path in expected.Keys)
        {
            actual[path].ShouldBe(expected[path]);
        }
    }

    /// <summary>Maps a supported game to its protocol name.</summary>
    /// <param name="game">The engine game.</param>
    /// <returns>The stable protocol name.</returns>
    private static string GameName(SupportedGame game)
    {
        return game switch
        {
            SupportedGame.Starfield => "starfield",
            SupportedGame.Fallout4 => "fallout4",
            SupportedGame.Skyrim => "skyrim",
            _ => throw new ArgumentOutOfRangeException(nameof(game)),
        };
    }

    /// <summary>Maps a plugin release to its protocol name.</summary>
    /// <param name="release">The plugin release.</param>
    /// <returns>The stable protocol name.</returns>
    private static string ReleaseName(GameRelease release)
    {
        return release switch
        {
            GameRelease.Starfield => "starfield",
            GameRelease.Fallout4 => "fallout4",
            GameRelease.SkyrimSE => "skyrim_se",
            _ => throw new ArgumentOutOfRangeException(nameof(release)),
        };
    }

    /// <summary>Maps an engine scope to its protocol name.</summary>
    /// <param name="scope">The engine scope.</param>
    /// <returns>The stable protocol name.</returns>
    private static string ScopeName(RecordScope scope)
    {
        return scope switch
        {
            RecordScope.WinningOverrides => "winning_overrides",
            RecordScope.AllContexts => "all_contexts",
            RecordScope.Source => "source",
            RecordScope.StagedOutput => "staged_output",
            _ => throw new ArgumentOutOfRangeException(nameof(scope)),
        };
    }

    /// <summary>Maps an engine plugin role to its protocol name.</summary>
    /// <param name="role">The engine plugin role.</param>
    /// <returns>The stable protocol name.</returns>
    private static string RoleName(PluginRole role)
    {
        return role switch
        {
            PluginRole.Source => "source",
            PluginRole.LoadOrder => "load_order",
            PluginRole.Output => "output",
            _ => throw new ArgumentOutOfRangeException(nameof(role)),
        };
    }

    /// <summary>Maps an engine resolution status to its protocol name.</summary>
    /// <param name="status">The engine status.</param>
    /// <returns>The stable protocol name.</returns>
    private static string StatusName(ReferenceResolutionStatus status)
    {
        return status switch
        {
            ReferenceResolutionStatus.Resolved => "resolved",
            ReferenceResolutionStatus.Unresolved => "unresolved",
            ReferenceResolutionStatus.Unsupported => "unsupported",
            ReferenceResolutionStatus.Deleted => "deleted",
            ReferenceResolutionStatus.Ambiguous => "ambiguous",
            ReferenceResolutionStatus.UnknownFamily => "unknown_family",
            _ => throw new ArgumentOutOfRangeException(nameof(status)),
        };
    }

    /// <summary>Maps an engine semantic change kind to its protocol name.</summary>
    /// <param name="kind">The engine change kind.</param>
    /// <returns>The stable protocol name.</returns>
    private static string ChangeKindName(SemanticChangeKind kind)
    {
        return kind switch
        {
            SemanticChangeKind.ValueChanged => "value_changed",
            SemanticChangeKind.ItemInserted => "item_inserted",
            SemanticChangeKind.ItemRemoved => "item_removed",
            SemanticChangeKind.ItemChanged => "item_changed",
            SemanticChangeKind.WriterNormalized => "writer_normalized",
            _ => throw new ArgumentOutOfRangeException(nameof(kind)),
        };
    }

    /// <summary>Adapts each existing generated plugin fixture to one common protocol-test shape.</summary>
    private sealed class PluginMcpFixture : IDisposable
    {
        /// <summary>The concrete generated fixture owner.</summary>
        private readonly IDisposable Owner;

        /// <summary>Creates exact open requests for this fixture.</summary>
        private readonly Func<Guid, WorkspaceOpenRequest> OpenRequestFactory;

        /// <summary>Captures every task-owned plugin artifact.</summary>
        private readonly Func<IReadOnlyDictionary<string, byte[]>> ArtifactSnapshotFactory;

        /// <summary>Initializes a common view over one concrete generated fixture.</summary>
        /// <param name="owner">The concrete fixture owner.</param>
        /// <param name="openRequestFactory">The exact request factory.</param>
        /// <param name="artifactSnapshotFactory">The complete artifact snapshot function.</param>
        /// <param name="sourceModKey">The source plugin identity.</param>
        /// <param name="patchModKey">The winning patch identity.</param>
        /// <param name="sourceListFormKey">The overridden source FormList identity.</param>
        /// <param name="deletedListFormKey">The deleted-winning FormList identity.</param>
        private PluginMcpFixture(
            IDisposable owner,
            Func<Guid, WorkspaceOpenRequest> openRequestFactory,
            Func<IReadOnlyDictionary<string, byte[]>> artifactSnapshotFactory,
            ModKey sourceModKey,
            ModKey patchModKey,
            FormKey sourceListFormKey,
            FormKey deletedListFormKey)
        {
            Owner = owner;
            OpenRequestFactory = openRequestFactory;
            ArtifactSnapshotFactory = artifactSnapshotFactory;
            SourceModKey = sourceModKey;
            PatchModKey = patchModKey;
            SourceListFormKey = sourceListFormKey;
            DeletedListFormKey = deletedListFormKey;
        }

        /// <summary>Gets the source plugin identity.</summary>
        public ModKey SourceModKey { get; }

        /// <summary>Gets the winning patch identity.</summary>
        public ModKey PatchModKey { get; }

        /// <summary>Gets the overridden source FormList identity.</summary>
        public FormKey SourceListFormKey { get; }

        /// <summary>Gets the deleted-winning FormList identity.</summary>
        public FormKey DeletedListFormKey { get; }

        /// <summary>Creates the fixture adapter for one supported game.</summary>
        /// <param name="game">The generated fixture family.</param>
        /// <returns>An owned common fixture adapter.</returns>
        public static PluginMcpFixture Create(SupportedGame game)
        {
            return game switch
            {
                SupportedGame.Starfield => CreateStarfield(),
                SupportedGame.Fallout4 => CreateFallout4(),
                SupportedGame.Skyrim => CreateSkyrim(),
                _ => throw new ArgumentOutOfRangeException(nameof(game)),
            };
        }

        /// <summary>Creates an exact plugin open request with the supplied workspace identity.</summary>
        /// <param name="workspaceId">The non-empty workspace identity.</param>
        /// <returns>The explicit generated-fixture request.</returns>
        public WorkspaceOpenRequest CreateOpenRequest(Guid workspaceId)
        {
            return OpenRequestFactory(workspaceId);
        }

        /// <summary>Captures exact bytes for every generated source artifact.</summary>
        /// <returns>The path-keyed artifact snapshot.</returns>
        public IReadOnlyDictionary<string, byte[]> SnapshotArtifacts()
        {
            return ArtifactSnapshotFactory();
        }

        /// <summary>Disposes the concrete fixture and deletes its generated artifacts.</summary>
        public void Dispose()
        {
            Owner.Dispose();
        }

        /// <summary>Creates a common adapter over the existing Starfield fixture.</summary>
        /// <returns>The owned fixture adapter.</returns>
        private static PluginMcpFixture CreateStarfield()
        {
            var fixture = StarfieldPluginTestFixture.Create();
            return new PluginMcpFixture(
                fixture,
                workspaceId => fixture.CreateOpenRequest(workspaceId),
                fixture.SnapshotArtifacts,
                fixture.SourceModKey,
                fixture.PatchModKey,
                fixture.SourceListFormKey,
                fixture.DeletedListFormKey);
        }

        /// <summary>Creates a common adapter over the existing Fallout 4 fixture.</summary>
        /// <returns>The owned fixture adapter.</returns>
        private static PluginMcpFixture CreateFallout4()
        {
            var fixture = Fallout4PluginTestFixture.Create();
            return new PluginMcpFixture(
                fixture,
                workspaceId => fixture.CreateOpenRequest(workspaceId),
                fixture.SnapshotArtifacts,
                fixture.SourceModKey,
                fixture.PatchModKey,
                fixture.SourceListFormKey,
                fixture.DeletedListFormKey);
        }

        /// <summary>Creates a common adapter over the existing Skyrim fixture.</summary>
        /// <returns>The owned fixture adapter.</returns>
        private static PluginMcpFixture CreateSkyrim()
        {
            var fixture = SkyrimPluginTestFixture.Create();
            return new PluginMcpFixture(
                fixture,
                workspaceId => fixture.CreateOpenRequest(workspaceId),
                fixture.SnapshotArtifacts,
                fixture.SourceModKey,
                fixture.PatchModKey,
                fixture.SourceListFormKey,
                fixture.DeletedListFormKey);
        }
    }
}
