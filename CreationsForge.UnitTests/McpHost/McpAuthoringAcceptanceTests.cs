using System.Text.Json.Nodes;
using CreationsForge.Engine.Interfaces;
using CreationsForge.Engine.Workspaces;
using CreationsForge.Fallout4;
using CreationsForge.Mcp;
using CreationsForge.Mcp.Protocol;
using CreationsForge.Mcp.Sessions;
using CreationsForge.Skyrim;
using CreationsForge.Starfield;

namespace CreationsForge.UnitTests.McpHost;

/// <summary>Exercises the production authoring contract across stdio and the service boundary.</summary>
public sealed class McpAuthoringAcceptanceTests
{
    /// <summary>Lists the closed tool surface, saves an empty Starfield output, and shuts the host down on stdin close.</summary>
    [Fact]
    public async Task ProductionHostListsContractAndSavesEmptyStarfieldOutput()
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(60));
        using var directory = new TestDirectory();
        McpPluginFixtures.WriteStarfieldMaster(directory.DirectoryPath);
        await using var client = await McpStdioClient.StartAsync(timeout.Token);

        Assert.Equal("2025-06-18", client.ProtocolVersion);
        Assert.Equal("CreationsForge", client.ServerName);
        Assert.False(string.IsNullOrWhiteSpace(client.ServerVersion));

        var listed = await ListToolsAsync(client, timeout.Token);
        Assert.Equal(
            McpAuthoringContract.ToolNames.OrderBy(name => name, StringComparer.Ordinal).ToArray(),
            listed.OrderBy(name => name, StringComparer.Ordinal).ToArray());

        var info = (await client.CallAsync(McpAuthoringContract.ServerInfoTool, new JsonObject(), timeout.Token))
            .RequireSuccess();
        Assert.Equal("CreationsForge", info.RequiredString("name"));
        Assert.Equal(client.ServerVersion, info.RequiredString("version"));
        Assert.Equal(["Fallout4", "SkyrimSE", "Starfield"], info.Strings("releases"));
        Assert.Equal(McpAuthoringContract.ToolNames, info.Strings("tools"));

        var types = (await client.CallAsync(
            McpAuthoringContract.RecordTypesTool,
            new JsonObject { ["release"] = "Starfield" },
            timeout.Token)).RequireSuccess();
        Assert.Contains("Keyword", types.Strings("families"));

        var schemaPaths = await SchemaPathsAsync(client, timeout.Token);
        Assert.Contains(schemaPaths, field => field.Path == "EditorID" && field.Kind == "String");

        var opened = (await client.CallAsync(
            McpAuthoringContract.WorkspaceOpenTool,
            McpRequests.WorkspaceOpen("open-empty", "Starfield", directory.DirectoryPath),
            timeout.Token)).RequireSuccess();
        var workspaceId = opened.RequiredString("workspaceId");
        Assert.False(opened.RequiredBool("workspaceOpen"));
        Assert.Equal(0ul, opened.RequiredUInt64("revision"));

        var replayed = (await client.CallAsync(
            McpAuthoringContract.WorkspaceOpenTool,
            McpRequests.WorkspaceOpen("open-empty", "Starfield", directory.DirectoryPath),
            timeout.Token)).RequireSuccess();
        Assert.Equal(workspaceId, replayed.RequiredString("workspaceId"));

        var conflict = await client.CallAsync(
            McpAuthoringContract.WorkspaceOpenTool,
            McpRequests.WorkspaceOpen("open-empty", "Starfield", directory.DirectoryPath + "-other"),
            timeout.Token);
        Assert.Equal("replay_conflict", conflict.ErrorCode());

        var alreadyOpen = await client.CallAsync(
            McpAuthoringContract.WorkspaceOpenTool,
            McpRequests.WorkspaceOpen("open-empty-again", "Starfield", directory.DirectoryPath),
            timeout.Token);
        Assert.Equal("workspace_already_open", alreadyOpen.ErrorCode());

        var created = (await client.CallAsync(
            McpAuthoringContract.OutputCreateTool,
            McpRequests.OutputCreate(
                "create-empty",
                workspaceId,
                Path.Combine(directory.DirectoryPath, McpPluginFixtures.OutputPlugin)),
            timeout.Token)).RequireSuccess();
        Assert.True(created.RequiredBool("workspaceOpen"));
        Assert.True(created.RequiredBool("isDirty"));
        Assert.True(created.RequiredBool("isNewOutput"));
        Assert.Equal(0ul, created.RequiredUInt64("revision"));
        Assert.Equal(McpPluginFixtures.OutputPlugin, created.RequiredString("outputModKey"));

        var discarded = (await client.CallAsync(
            McpAuthoringContract.WorkspaceDiscardTool,
            new JsonObject
            {
                ["operationId"] = "discard-empty",
                ["workspaceId"] = workspaceId,
            },
            timeout.Token)).RequireSuccess();
        Assert.False(discarded.RequiredBool("changed"));
        Assert.True(discarded.RequiredBool("isDirty"));

        var saved = (await client.CallAsync(
            McpAuthoringContract.WorkspaceSaveTool,
            new JsonObject
            {
                ["operationId"] = "save-empty",
                ["workspaceId"] = workspaceId,
            },
            timeout.Token)).RequireSuccess();
        Assert.Equal("Succeeded", saved.RequiredString("status"));
        Assert.Equal(0ul, saved.RequiredUInt64("revision"));
        Assert.False(saved.RequiredBool("requiresReopen"));
        Assert.Contains(McpPluginFixtures.OutputPlugin, saved.RequiredString("destinationPath"), StringComparison.OrdinalIgnoreCase);

        var state = (await client.CallAsync(
            McpAuthoringContract.WorkspaceStateTool,
            new JsonObject { ["workspaceId"] = workspaceId },
            timeout.Token)).RequireSuccess();
        Assert.False(state.RequiredBool("isDirty"));

        var closed = (await client.CallAsync(
            McpAuthoringContract.WorkspaceCloseTool,
            new JsonObject
            {
                ["operationId"] = "close-empty",
                ["workspaceId"] = workspaceId,
                ["discardUnsaved"] = false,
            },
            timeout.Token)).RequireSuccess();
        Assert.True(closed.RequiredBool("closed"));
        await client.ShutdownAsync(timeout.Token);
    }

    /// <summary>Creates, overrides, replays, saves, and reopens a Starfield keyword through the production host.</summary>
    [Fact]
    public async Task ProductionHostRoundTripsKeywordCreateOverrideAndReopen()
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(90));
        using var directory = new TestDirectory();
        McpPluginFixtures.WriteSourceKeyword(directory.DirectoryPath);
        await using var client = await McpStdioClient.StartAsync(timeout.Token);
        var outputPath = Path.Combine(directory.DirectoryPath, McpPluginFixtures.OutputPlugin);
        var workspaceId = await OpenAndCreateAsync(client, directory.DirectoryPath, outputPath, "keyword", timeout.Token);

        var source = Assert.Single(
            await McpRequests.SearchAllAsync(client, workspaceId, McpPluginFixtures.SourceEditorId, timeout.Token),
            record => record.RequiredString("containingModKey") == McpPluginFixtures.SourcePlugin);
        var sourceFormKey = source.RequiredString("formKey");
        Assert.Equal(McpPluginFixtures.SourcePlugin, source.RequiredString("winningModKey"));

        var createArguments = McpRequests.RecordCreate("create-keyword", workspaceId, 0, "CreatedKeyword");
        var created = (await client.CallAsync(McpAuthoringContract.RecordCreateTool, createArguments, timeout.Token))
            .RequireSuccess();
        Assert.Equal(1ul, created.RequiredUInt64("revision"));
        var createdRecord = Assert.Single(created.Array("records"));
        var createdFormKey = createdRecord.RequiredString("formKey");
        Assert.Equal(McpPluginFixtures.OutputPlugin, createdRecord.RequiredString("containingModKey"));

        var replayed = (await client.CallAsync(McpAuthoringContract.RecordCreateTool, createArguments, timeout.Token))
            .RequireSuccess();
        Assert.Equal(createdFormKey, Assert.Single(replayed.Array("records")).RequiredString("formKey"));
        Assert.Equal(1ul, replayed.RequiredUInt64("revision"));
        Assert.Single(await McpRequests.SearchAllAsync(client, workspaceId, "CreatedKeyword", timeout.Token));

        var conflictArguments = McpRequests.RecordCreate("create-keyword", workspaceId, 0, "DifferentKeyword");
        var conflict = await client.CallAsync(McpAuthoringContract.RecordCreateTool, conflictArguments, timeout.Token);
        Assert.Equal("replay_conflict", conflict.ErrorCode());

        var overridden = (await client.CallAsync(
            McpAuthoringContract.RecordOverrideTool,
            McpRequests.RecordOverride(
                "override-source",
                workspaceId,
                sourceFormKey,
                McpPluginFixtures.SourcePlugin,
                1,
                "SourceOverride"),
            timeout.Token)).RequireSuccess();
        Assert.Equal(2ul, overridden.RequiredUInt64("revision"));

        var applied = (await client.CallAsync(
            McpAuthoringContract.RecordApplyTool,
            McpRequests.RecordApply(
                "apply-edit",
                workspaceId,
                2,
                createdFormKey,
                McpPluginFixtures.OutputPlugin,
                "CreatedKeywordEdited"),
            timeout.Token)).RequireSuccess();
        Assert.Equal(3ul, applied.RequiredUInt64("revision"));
        Assert.Equal(
            "CreatedKeywordEdited",
            await McpRequests.ReadEditorIdAsync(
                client,
                workspaceId,
                createdFormKey,
                McpPluginFixtures.OutputPlugin,
                timeout.Token));

        var preview = (await client.CallAsync(
            McpAuthoringContract.WorkspacePreviewTool,
            new JsonObject { ["workspaceId"] = workspaceId },
            timeout.Token)).RequireSuccess();
        Assert.True(preview.RequiredBool("isDirty"));
        Assert.NotEmpty(preview.Array("records"));

        await AssertEditorIdDifferenceAsync(client, workspaceId, sourceFormKey, timeout.Token);

        var stale = await client.CallAsync(
            McpAuthoringContract.RecordCreateTool,
            McpRequests.RecordCreate("stale-create", workspaceId, 99, "StaleKeyword"),
            timeout.Token);
        Assert.Equal("stale_revision", stale.ErrorCode());

        var rejectedClose = await client.CallAsync(
            McpAuthoringContract.WorkspaceCloseTool,
            new JsonObject
            {
                ["operationId"] = "close-dirty",
                ["workspaceId"] = workspaceId,
                ["discardUnsaved"] = false,
            },
            timeout.Token);
        Assert.Equal("unsaved_changes", rejectedClose.ErrorCode());

        var saved = (await client.CallAsync(
            McpAuthoringContract.WorkspaceSaveTool,
            new JsonObject
            {
                ["operationId"] = "save-keywords",
                ["workspaceId"] = workspaceId,
            },
            timeout.Token)).RequireSuccess();
        Assert.Equal("Succeeded", saved.RequiredString("status"));
        Assert.Equal(3ul, saved.RequiredUInt64("revision"));

        var extra = (await client.CallAsync(
            McpAuthoringContract.RecordCreateTool,
            McpRequests.RecordCreate("create-discarded", workspaceId, 3, "DiscardedKeyword"),
            timeout.Token)).RequireSuccess();
        var discarded = (await client.CallAsync(
            McpAuthoringContract.WorkspaceDiscardTool,
            new JsonObject
            {
                ["operationId"] = "discard-extra",
                ["workspaceId"] = workspaceId,
            },
            timeout.Token)).RequireSuccess();
        Assert.True(discarded.RequiredBool("changed"));
        Assert.False(discarded.RequiredBool("isDirty"));
        Assert.True(discarded.RequiredUInt64("revision") > extra.RequiredUInt64("revision"));
        Assert.Empty(await McpRequests.SearchAllAsync(client, workspaceId, "DiscardedKeyword", timeout.Token));
        Assert.Single(await McpRequests.SearchAllAsync(client, workspaceId, "CreatedKeywordEdited", timeout.Token));

        var closed = (await client.CallAsync(
            McpAuthoringContract.WorkspaceCloseTool,
            new JsonObject
            {
                ["operationId"] = "close-keywords",
                ["workspaceId"] = workspaceId,
                ["discardUnsaved"] = false,
            },
            timeout.Token)).RequireSuccess();
        Assert.True(closed.RequiredBool("closed"));
        await client.ShutdownAsync(timeout.Token);

        var editorIds = McpPluginFixtures.ReadKeywordEditorIds(directory.DirectoryPath);
        Assert.Contains("CreatedKeywordEdited", editorIds);
        Assert.Contains("SourceOverride", editorIds);
        Assert.DoesNotContain("DiscardedKeyword", editorIds);

        await using var reopened = await McpStdioClient.StartAsync(timeout.Token);
        var reopenedId = await OpenExistingAsync(reopened, directory.DirectoryPath, outputPath, timeout.Token);
        var reread = await McpRequests.ReadEditorIdAsync(
            reopened,
            reopenedId,
            createdFormKey,
            McpPluginFixtures.OutputPlugin,
            timeout.Token);
        Assert.Equal("CreatedKeywordEdited", reread);
        var closedAgain = (await reopened.CallAsync(
            McpAuthoringContract.WorkspaceCloseTool,
            new JsonObject
            {
                ["operationId"] = "close-reopened",
                ["workspaceId"] = reopenedId,
                ["discardUnsaved"] = false,
            },
            timeout.Token)).RequireSuccess();
        Assert.True(closedAgain.RequiredBool("closed"));
        await reopened.ShutdownAsync(timeout.Token);
    }

    /// <summary>Returns an uncached cancellation for a token that is already canceled, then accepts the same operation.</summary>
    [Fact]
    public async Task CanceledWorkspaceOpenIsNotCached()
    {
        var service = CreateService();
        using var canceled = new CancellationTokenSource();
        await canceled.CancelAsync();

        var first = await service.WorkspaceOpenAsync("open-canceled", "Starfield", "unused", [], canceled.Token);
        Assert.True(first.IsError);
        Assert.Equal("canceled", first.StructuredContent!.Value.GetProperty("code").GetString());

        var second = await service.WorkspaceOpenAsync("open-canceled", "Starfield", "unused", [], CancellationToken.None);
        Assert.False(second.IsError);
        Assert.False(string.IsNullOrWhiteSpace(second.StructuredContent!.Value.GetProperty("workspaceId").GetString()));
    }

    /// <summary>Accepts a cancellation notification, a protocol cancellation, or a successful open that won the race.</summary>
    [Fact]
    public async Task CancellationNotificationDoesNotFailOpenWithAnotherError()
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(45));
        using var directory = new TestDirectory();
        await using var client = await McpStdioClient.StartAsync(timeout.Token);
        var result = await client.CallAfterCancellationAsync(
            McpAuthoringContract.WorkspaceOpenTool,
            McpRequests.WorkspaceOpen("open-cancel", "Starfield", directory.DirectoryPath),
            timeout.Token);

        var accepted = result is null
            || !result.IsError
            || result.ErrorCode() == "canceled"
            || result.ProtocolErrorCode == -32800
            || (result.ProtocolErrorMessage?.Contains("cancel", StringComparison.OrdinalIgnoreCase) ?? false);
        Assert.True(accepted, result?.Describe() ?? "The canceled call returned no response.");

        var followUp = await client.CallAsync(
            McpAuthoringContract.WorkspaceOpenTool,
            McpRequests.WorkspaceOpen("open-after-cancel", "Starfield", directory.DirectoryPath),
            timeout.Token);
        var hostAlive = !followUp.IsError || followUp.ErrorCode() == "workspace_already_open";
        Assert.True(hostAlive, followUp.Describe());
        await client.ShutdownAsync(timeout.Token);
    }

    /// <summary>Reports a missing selected-plugin master when the native workspace is created.</summary>
    [Fact]
    public async Task MissingMasterFailsWhenOutputIsCreated()
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(45));
        using var directory = new TestDirectory();
        McpPluginFixtures.WriteMissingMasterSelection(directory.DirectoryPath);
        await using var client = await McpStdioClient.StartAsync(timeout.Token);
        var opened = (await client.CallAsync(
            McpAuthoringContract.WorkspaceOpenTool,
            McpRequests.WorkspaceOpen("open-missing", "Starfield", directory.DirectoryPath, McpPluginFixtures.SelectedPlugin),
            timeout.Token)).RequireSuccess();

        var failed = await client.CallAsync(
            McpAuthoringContract.OutputCreateTool,
            McpRequests.OutputCreate(
                "create-missing",
                opened.RequiredString("workspaceId"),
                Path.Combine(directory.DirectoryPath, McpPluginFixtures.OutputPlugin)),
            timeout.Token);
        Assert.Equal("missing_master", failed.ErrorCode());
        Assert.Contains("Missing.esm", failed.Describe(), StringComparison.OrdinalIgnoreCase);
        await client.ShutdownAsync(timeout.Token);
    }

    /// <summary>Reports a host failure when the selected plugin cannot be read, without leaking a stack trace.</summary>
    [Fact]
    public async Task UnreadablePluginFailsWhenOutputIsCreated()
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(45));
        using var directory = new TestDirectory();
        McpPluginFixtures.WriteUnreadableSelection(directory.DirectoryPath);
        await using var client = await McpStdioClient.StartAsync(timeout.Token);
        var opened = (await client.CallAsync(
            McpAuthoringContract.WorkspaceOpenTool,
            McpRequests.WorkspaceOpen("open-garbage", "Starfield", directory.DirectoryPath, McpPluginFixtures.SelectedPlugin),
            timeout.Token)).RequireSuccess();

        var failed = await client.CallAsync(
            McpAuthoringContract.OutputCreateTool,
            McpRequests.OutputCreate(
                "create-garbage",
                opened.RequiredString("workspaceId"),
                Path.Combine(directory.DirectoryPath, McpPluginFixtures.OutputPlugin)),
            timeout.Token);
        Assert.Equal("host_failure", failed.ErrorCode());
        Assert.DoesNotContain("   at ", failed.Describe(), StringComparison.Ordinal);
        await client.ShutdownAsync(timeout.Token);
    }

    /// <summary>Retries a busy create with the same operation identifier after the other process releases the output.</summary>
    [Fact]
    public async Task FileBusyCreateIsNotCachedAndSucceedsAfterRelease()
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(60));
        using var directory = new TestDirectory();
        await using var holder = await CrossProcessLock.AcquireAsync(directory.DirectoryPath, timeout.Token);
        await using var client = await McpStdioClient.StartAsync(timeout.Token);
        var opened = (await client.CallAsync(
            McpAuthoringContract.WorkspaceOpenTool,
            McpRequests.WorkspaceOpen("open-busy", "Fallout4", directory.DirectoryPath),
            timeout.Token)).RequireSuccess();
        var arguments = McpRequests.OutputCreate(
            "create-busy",
            opened.RequiredString("workspaceId"),
            Path.Combine(directory.DirectoryPath, McpPluginFixtures.BusyOutput));

        var busy = await client.CallAsync(McpAuthoringContract.OutputCreateTool, arguments, timeout.Token);
        Assert.Equal("file_busy", busy.ErrorCode());
        await holder.ReleaseAsync(timeout.Token);

        var created = (await client.CallAsync(McpAuthoringContract.OutputCreateTool, arguments, timeout.Token))
            .RequireSuccess();
        Assert.True(created.RequiredBool("workspaceOpen"));
        var closed = (await client.CallAsync(
            McpAuthoringContract.WorkspaceCloseTool,
            new JsonObject
            {
                ["operationId"] = "close-busy",
                ["workspaceId"] = opened.RequiredString("workspaceId"),
                ["discardUnsaved"] = true,
            },
            timeout.Token)).RequireSuccess();
        Assert.True(closed.RequiredBool("closed"));
        await client.ShutdownAsync(timeout.Token);
    }

    /// <summary>Rejects command-line arguments before the host starts.</summary>
    [Fact]
    public async Task HostRejectsCommandLineArguments()
    {
        var exitCode = await McpHostRunner.RunAsync(["--help"], TestContext.Current.CancellationToken);
        Assert.Equal(2, exitCode);
    }

    private static async Task<string> OpenAndCreateAsync(
        McpStdioClient client,
        string directory,
        string outputPath,
        string operationPrefix,
        CancellationToken cancellationToken)
    {
        var opened = (await client.CallAsync(
            McpAuthoringContract.WorkspaceOpenTool,
            McpRequests.WorkspaceOpen(operationPrefix + "-open", "Starfield", directory, McpPluginFixtures.SourcePlugin),
            cancellationToken)).RequireSuccess();
        var workspaceId = opened.RequiredString("workspaceId");
        var created = (await client.CallAsync(
            McpAuthoringContract.OutputCreateTool,
            McpRequests.OutputCreate(operationPrefix + "-output", workspaceId, outputPath),
            cancellationToken)).RequireSuccess();
        Assert.Equal(workspaceId, created.RequiredString("workspaceId"));
        return workspaceId;
    }

    private static async Task<string> OpenExistingAsync(
        McpStdioClient client,
        string directory,
        string outputPath,
        CancellationToken cancellationToken)
    {
        var opened = (await client.CallAsync(
            McpAuthoringContract.WorkspaceOpenTool,
            McpRequests.WorkspaceOpen("reopen-session", "Starfield", directory, McpPluginFixtures.SourcePlugin),
            cancellationToken)).RequireSuccess();
        var workspaceId = opened.RequiredString("workspaceId");
        var existing = (await client.CallAsync(
            McpAuthoringContract.OutputOpenTool,
            McpRequests.OutputOpen("reopen-output", workspaceId, outputPath),
            cancellationToken)).RequireSuccess();
        Assert.False(existing.RequiredBool("isNewOutput"));
        Assert.False(existing.RequiredBool("isDirty"));
        return workspaceId;
    }

    private static async Task AssertEditorIdDifferenceAsync(
        McpStdioClient client,
        string workspaceId,
        string formKey,
        CancellationToken cancellationToken)
    {
        string? cursor = null;
        for (var page = 0; page < 20; page++)
        {
            var arguments = new JsonObject
            {
                ["workspaceId"] = workspaceId,
                ["familyId"] = "Keyword",
                ["leftFormKey"] = formKey,
                ["leftContainingModKey"] = McpPluginFixtures.SourcePlugin,
                ["rightFormKey"] = formKey,
                ["rightContainingModKey"] = McpPluginFixtures.OutputPlugin,
            };
            if (cursor is not null)
            {
                arguments["cursor"] = cursor;
            }

            var compared = (await client.CallAsync(McpAuthoringContract.RecordCompareTool, arguments, cancellationToken))
                .RequireSuccess();
            var difference = compared.Array("differences")
                .FirstOrDefault(item => item.RequiredString("path") == "EditorID");
            if (difference is not null)
            {
                Assert.Equal(McpPluginFixtures.SourceEditorId, difference.Object("left").OptionalString("string"));
                Assert.Equal("SourceOverride", difference.Object("right").OptionalString("string"));
                return;
            }

            cursor = compared.OptionalString("nextCursor");
            if (string.IsNullOrWhiteSpace(cursor))
            {
                break;
            }
        }

        Assert.Fail("The override did not report an EditorID difference.");
    }

    private static async Task<IReadOnlyList<string>> ListToolsAsync(
        McpStdioClient client,
        CancellationToken cancellationToken)
    {
        var names = new List<string>();
        string? cursor = null;
        for (var page = 0; page < 5; page++)
        {
            var parameters = new JsonObject();
            if (cursor is not null)
            {
                parameters["cursor"] = cursor;
            }

            var result = await client.RequestAsync("tools/list", parameters, cancellationToken);
            foreach (var tool in result["tools"] as JsonArray ?? [])
            {
                var item = tool as JsonObject ?? throw new InvalidOperationException("A listed tool was not an object.");
                var schema = item["inputSchema"] as JsonObject;
                Assert.NotNull(schema);
                names.Add(item["name"]?.GetValue<string>() ?? string.Empty);
            }

            cursor = result["nextCursor"]?.GetValue<string>();
            if (string.IsNullOrWhiteSpace(cursor))
            {
                return names;
            }
        }

        throw new InvalidOperationException("Tool listing did not finish.");
    }

    private static async Task<IReadOnlyList<(string Path, string Kind)>> SchemaPathsAsync(
        McpStdioClient client,
        CancellationToken cancellationToken)
    {
        var fields = new List<(string Path, string Kind)>();
        string? cursor = null;
        for (var page = 0; page < 10; page++)
        {
            var arguments = new JsonObject
            {
                ["release"] = "Starfield",
                ["familyId"] = "Keyword",
            };
            if (cursor is not null)
            {
                arguments["cursor"] = cursor;
            }

            var result = (await client.CallAsync(McpAuthoringContract.RecordSchemaTool, arguments, cancellationToken))
                .RequireSuccess();
            fields.AddRange(result.Array("fields").Select(field =>
                (field.RequiredString("path"), field.RequiredString("kind"))));
            cursor = result.OptionalString("nextCursor");
            if (string.IsNullOrWhiteSpace(cursor))
            {
                return fields;
            }
        }

        throw new InvalidOperationException("Schema paging did not finish.");
    }

    private static McpAuthoringService CreateService()
    {
        IGameIntegration[] integrations =
        [
            new StarfieldGameIntegration(),
            new Fallout4GameIntegration(),
            new SkyrimGameIntegration(),
        ];
        return new McpAuthoringService(
            integrations,
            new PluginWorkspaceFactory(integrations),
            new McpSessionRegistry(),
            new McpOperationReplay());
    }
}
