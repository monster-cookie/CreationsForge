using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Enums;
using CreationsForge.PresentationTests.Composition;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Shouldly;

namespace CreationsForge.PresentationTests.Support;

/// <summary>Drives one independently owned production MCP workspace through a physical SDK stdio client.</summary>
internal sealed class NativeMcpWorkspaceSession
{
    /// <summary>The initialized SDK client connected to the production child process.</summary>
    private readonly McpClient Client;

    /// <summary>Tracks whether the registry workspace has been explicitly closed.</summary>
    private bool IsClosed;

    /// <summary>Initializes one selected MCP workspace from exact protocol results.</summary>
    /// <param name="client">The initialized physical stdio client.</param>
    /// <param name="workspaceId">The independently generated MCP workspace identity.</param>
    /// <param name="revision">The revision after selecting the existing output.</param>
    /// <param name="selectionOperationId">The output-selection operation identity.</param>
    /// <param name="baselineHandle">The retained exact output baseline handle.</param>
    private NativeMcpWorkspaceSession(
        McpClient client,
        Guid workspaceId,
        WorkspaceRevision revision,
        Guid selectionOperationId,
        string baselineHandle)
    {
        Client = client;
        WorkspaceId = workspaceId;
        Revision = revision;
        SelectionOperationId = selectionOperationId;
        BaselineHandle = baselineHandle;
    }

    /// <summary>Gets the workspace identity owned only by the MCP registry.</summary>
    public Guid WorkspaceId { get; }

    /// <summary>Gets the current MCP-local workspace revision.</summary>
    public WorkspaceRevision Revision { get; private set; }

    /// <summary>Gets the exact operation that selected the existing output.</summary>
    public Guid SelectionOperationId { get; }

    /// <summary>Gets the retained metadata handle for the workspace's current exact output baseline.</summary>
    public string BaselineHandle { get; private set; }

    /// <summary>Opens a fresh MCP workspace and selects the fixture's existing output.</summary>
    /// <param name="client">The physical SDK stdio client.</param>
    /// <param name="fixture">The explicit generated native fixture.</param>
    /// <param name="cancellationToken">The finite case token.</param>
    /// <returns>The selected independently owned MCP workspace.</returns>
    public static async Task<NativeMcpWorkspaceSession> OpenAsync(
        McpClient client,
        NativeDesktopWorkspaceFixture fixture,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(fixture);
        var workspaceId = Guid.NewGuid();
        var request = fixture.CreateSourceRequest();
        var open = GetResult(await client.CallToolAsync(
            "creationsforge_workspace_open",
            new Dictionary<string, object?>
            {
                ["workspaceId"] = workspaceId.ToString("D"),
                ["game"] = GameName(fixture.Game),
                ["release"] = ReleaseName(fixture.Release),
                ["sourcePluginPath"] = request.SourcePluginPath,
                ["loadOrderPluginPaths"] = request.LoadOrderPluginPaths,
                ["dataDirectoryPath"] = request.DataDirectoryPath,
                ["stringDirectoryPaths"] = request.StringDirectoryPaths,
            },
            cancellationToken: cancellationToken));
        open.GetProperty("workspaceId").GetString().ShouldBe(workspaceId.ToString("D"));
        var openRevision = ReadRevision(open.GetProperty("revision"));
        var selectionOperationId = Guid.NewGuid();
        var output = fixture.ExistingOutput;
        var selected = GetResult(await client.CallToolAsync(
            "creationsforge_output_select",
            new Dictionary<string, object?>
            {
                ["workspaceId"] = workspaceId.ToString("D"),
                ["operationId"] = selectionOperationId.ToString("D"),
                ["expectedRevision"] = RevisionArguments(openRevision),
                ["mode"] = "open_existing",
                ["output"] = new Dictionary<string, object?>
                {
                    ["pluginPath"] = output.PluginPath,
                    ["modKey"] = output.ModKey.ToString(),
                    ["localizedOutputMode"] = "embedded",
                    ["masterStyle"] = "full",
                },
            },
            cancellationToken: cancellationToken));
        selected.GetProperty("workspaceId").GetString().ShouldBe(workspaceId.ToString("D"));
        selected.GetProperty("operationId").GetString().ShouldBe(selectionOperationId.ToString("D"));
        selected.GetProperty("detailsUnavailable").GetBoolean().ShouldBeFalse();
        var baselineHandle = selected.GetProperty("baselineReference").GetProperty("handle").GetString().ShouldNotBeNull();
        return new NativeMcpWorkspaceSession(
            client,
            workspaceId,
            ReadRevision(selected.GetProperty("revision")),
            selectionOperationId,
            baselineHandle);
    }

    /// <summary>Reads the number and identities of workspaces visible only to the MCP registry.</summary>
    /// <param name="client">The physical SDK stdio client.</param>
    /// <param name="cancellationToken">The finite case token.</param>
    /// <returns>The detached active workspace identities.</returns>
    public static async Task<IReadOnlyList<Guid>> ReadActiveWorkspaceIdsAsync(
        McpClient client,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(client);
        var result = GetResult(await client.CallToolAsync(
            "creationsforge_server_info",
            new Dictionary<string, object?>(),
            cancellationToken: cancellationToken));
        return result.GetProperty("lifecycle").GetProperty("activeWorkspaceIds")
            .EnumerateArray()
            .Select(value => Guid.Parse(value.GetString().ShouldNotBeNull()))
            .ToArray();
    }

    /// <summary>Begins and applies one exact existing-output EditorID change through protocol tools.</summary>
    /// <param name="formKey">The output FormList to stage.</param>
    /// <param name="editorId">The distinct valid EditorID.</param>
    /// <param name="cancellationToken">The finite case token.</param>
    /// <returns>The exact begin and apply identities and resulting revision.</returns>
    public async Task<NativeMcpStagedEdit> StageEditorIdAsync(
        FormKey formKey,
        string editorId,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(editorId);
        ThrowIfClosed();
        var beginOperationId = Guid.NewGuid();
        var begin = GetResult(await Client.CallToolAsync(
            "creationsforge_formlist_begin_edit",
            new Dictionary<string, object?>
            {
                ["workspaceId"] = WorkspaceId.ToString("D"),
                ["operationId"] = beginOperationId.ToString("D"),
                ["expectedRevision"] = RevisionArguments(Revision),
                ["role"] = "existing_output",
                ["targetFormKey"] = formKey.ToString(),
            },
            cancellationToken: cancellationToken));
        begin.GetProperty("operationId").GetString().ShouldBe(beginOperationId.ToString("D"));
        var editId = Guid.Parse(begin.GetProperty("editId").GetString().ShouldNotBeNull());
        var beginRevision = ReadRevision(begin.GetProperty("revision"));
        var applyOperationId = Guid.NewGuid();
        var apply = GetResult(await Client.CallToolAsync(
            "creationsforge_formlist_apply_edit",
            new Dictionary<string, object?>
            {
                ["workspaceId"] = WorkspaceId.ToString("D"),
                ["operationId"] = applyOperationId.ToString("D"),
                ["expectedRevision"] = RevisionArguments(beginRevision),
                ["editId"] = editId.ToString("D"),
                ["commandName"] = "form-list.set-editor-id",
                ["argumentsJson"] = JsonSerializer.Serialize(new { editorId }),
            },
            cancellationToken: cancellationToken));
        apply.GetProperty("operationId").GetString().ShouldBe(applyOperationId.ToString("D"));
        apply.GetProperty("editId").GetString().ShouldBe(editId.ToString("D"));
        Revision = ReadRevision(apply.GetProperty("revision"));
        return new NativeMcpStagedEdit(
            editId,
            beginOperationId,
            applyOperationId,
            editorId,
            Revision);
    }

    /// <summary>Reads and reconstructs the complete detached staged preview at the current revision.</summary>
    /// <param name="cancellationToken">The finite case token.</param>
    /// <returns>The complete preview JSON value.</returns>
    public async Task<JsonElement> ReadPreviewAsync(CancellationToken cancellationToken)
    {
        ThrowIfClosed();
        var root = await ReconstructJsonAsync(string.Empty, cancellationToken);
        return JsonSerializer.SerializeToElement(root);
    }

    /// <summary>Creates an immutable save request from the current local revision and exact current baseline.</summary>
    /// <returns>A new logical request with a fresh operation identity.</returns>
    public NativeMcpSaveRequest CreateSaveRequest()
    {
        ThrowIfClosed();
        return new NativeMcpSaveRequest(WorkspaceId, Guid.NewGuid(), Revision, BaselineHandle);
    }

    /// <summary>Invokes one exact save request and projects its independent structured commitment outcome.</summary>
    /// <param name="request">The immutable request, reusable only to verify idempotent replay.</param>
    /// <param name="cancellationToken">The finite case token.</param>
    /// <returns>The complete bounded save result fields needed by cross-surface assertions.</returns>
    public async Task<NativeMcpSaveOutcome> SaveAsync(
        NativeMcpSaveRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ThrowIfClosed();
        request.WorkspaceId.ShouldBe(WorkspaceId);
        var result = GetResult(await Client.CallToolAsync(
            "creationsforge_save",
            request.CreateArguments(),
            cancellationToken: cancellationToken));
        var outcome = NativeMcpSaveOutcome.FromJson(result);
        outcome.WorkspaceId.ShouldBe(WorkspaceId);
        outcome.OperationId.ShouldBe(request.OperationId);
        Revision = outcome.ResultRevision;
        if (outcome.Status == "committed")
        {
            BaselineHandle = outcome.CommittedBaselineHandle.ShouldNotBeNull();
        }

        return outcome;
    }

    /// <summary>Closes this exact MCP registry workspace and waits for native resource disposal.</summary>
    /// <param name="cancellationToken">The finite case token.</param>
    /// <returns>A task that completes after the close receipt is verified.</returns>
    public async Task CloseAsync(CancellationToken cancellationToken)
    {
        if (IsClosed)
        {
            return;
        }

        var result = GetResult(await Client.CallToolAsync(
            "creationsforge_workspace_close",
            new Dictionary<string, object?>
            {
                ["workspaceId"] = WorkspaceId.ToString("D"),
            },
            cancellationToken: cancellationToken));
        result.GetProperty("workspaceId").GetString().ShouldBe(WorkspaceId.ToString("D"));
        result.GetProperty("closed").GetBoolean().ShouldBeTrue();
        IsClosed = true;
    }

    /// <summary>Recursively reconstructs one complete JSON value from bounded immediate-child pages.</summary>
    /// <param name="path">The canonical JSON Pointer to reconstruct.</param>
    /// <param name="cancellationToken">The finite case token.</param>
    /// <returns>The complete detached JSON node, including <see langword="null"/> values.</returns>
    private async Task<JsonNode?> ReconstructJsonAsync(string path, CancellationToken cancellationToken)
    {
        var pages = await ReadPagesAsync(path, cancellationToken);
        pages.ShouldNotBeEmpty();
        var kind = pages[0].GetProperty("kind").GetString();
        if (kind == "object")
        {
            var value = new JsonObject();
            foreach (var child in pages.SelectMany(page => page.GetProperty("children").EnumerateArray()).ToArray())
            {
                value[child.GetProperty("property").GetString().ShouldNotBeNull()] = await ReconstructJsonAsync(
                    child.GetProperty("path").GetString().ShouldNotBeNull(),
                    cancellationToken);
            }

            return value;
        }

        if (kind == "array")
        {
            var value = new JsonArray();
            foreach (var child in pages.SelectMany(page => page.GetProperty("children").EnumerateArray()).ToArray())
            {
                child.GetProperty("index").GetInt32().ShouldBe(value.Count);
                value.Add(await ReconstructJsonAsync(
                    child.GetProperty("path").GetString().ShouldNotBeNull(),
                    cancellationToken));
            }

            return value;
        }

        return kind switch
        {
            "string" => JsonValue.Create(string.Concat(pages.Select(page => page.GetProperty("value").GetString()))),
            "number" => JsonNode.Parse(pages.ShouldHaveSingleItem().GetProperty("rawValue").GetString().ShouldNotBeNull()),
            "boolean" => JsonValue.Create(pages.ShouldHaveSingleItem().GetProperty("value").GetBoolean()),
            "null" => null,
            _ => throw new InvalidOperationException($"Unsupported MCP preview page kind '{kind}'."),
        };
    }

    /// <summary>Reads every cursor page for one exact preview JSON Pointer.</summary>
    /// <param name="path">The canonical JSON Pointer.</param>
    /// <param name="cancellationToken">The finite case token.</param>
    /// <returns>Detached pages in protocol order.</returns>
    private async Task<IReadOnlyList<JsonElement>> ReadPagesAsync(string path, CancellationToken cancellationToken)
    {
        var pages = new List<JsonElement>();
        string? cursor = null;
        do
        {
            var arguments = new Dictionary<string, object?>
            {
                ["workspaceId"] = WorkspaceId.ToString("D"),
                ["expectedRevision"] = RevisionArguments(Revision),
                ["path"] = path,
                ["maxResults"] = 250,
            };
            if (cursor is not null)
            {
                arguments["cursor"] = cursor;
            }

            var result = GetResult(await Client.CallToolAsync(
                "creationsforge_workspace_preview",
                arguments,
                cancellationToken: cancellationToken));
            ReadRevision(result.GetProperty("revision")).ShouldBe(Revision);
            var page = result.GetProperty("page").Clone();
            pages.Add(page);
            cursor = page.TryGetProperty("cursor", out var cursorElement) && cursorElement.ValueKind != JsonValueKind.Null
                ? cursorElement.GetString()
                : null;
        }
        while (cursor is not null);

        return pages;
    }

    /// <summary>Throws when a caller attempts to reuse a closed registry workspace.</summary>
    /// <exception cref="InvalidOperationException">Thrown after the workspace close receipt was accepted.</exception>
    private void ThrowIfClosed()
    {
        if (IsClosed)
        {
            throw new InvalidOperationException("The MCP workspace session has already been closed.");
        }
    }

    /// <summary>Reads a canonical protocol revision.</summary>
    /// <param name="value">The structured revision object.</param>
    /// <returns>The exact engine revision.</returns>
    private static WorkspaceRevision ReadRevision(JsonElement value)
    {
        return new WorkspaceRevision(
            Guid.Parse(value.GetProperty("baselineId").GetString().ShouldNotBeNull()),
            ulong.Parse(value.GetProperty("sequence").GetString().ShouldNotBeNull(), CultureInfo.InvariantCulture));
    }

    /// <summary>Projects an engine revision into canonical protocol arguments.</summary>
    /// <param name="revision">The exact local revision.</param>
    /// <returns>The closed protocol revision object.</returns>
    private static Dictionary<string, object?> RevisionArguments(WorkspaceRevision revision)
    {
        return new Dictionary<string, object?>
        {
            ["baselineId"] = revision.BaselineId.ToString("D"),
            ["sequence"] = revision.Sequence.ToString(CultureInfo.InvariantCulture),
        };
    }

    /// <summary>Returns a successful tool-specific structured result.</summary>
    /// <param name="result">The SDK tool response.</param>
    /// <returns>The nested result object.</returns>
    private static JsonElement GetResult(CallToolResult result)
    {
        result.IsError.ShouldNotBe(true);
        result.StructuredContent.ShouldNotBeNull();
        var content = result.StructuredContent.Value;
        content.GetProperty("ok").GetBoolean().ShouldBeTrue();
        return content.GetProperty("result");
    }

    /// <summary>Maps a supported game to its stable MCP value.</summary>
    /// <param name="game">The engine game.</param>
    /// <returns>The protocol game name.</returns>
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

    /// <summary>Maps a native release to its stable MCP value.</summary>
    /// <param name="release">The exact native release.</param>
    /// <returns>The protocol release name.</returns>
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
}

/// <summary>Retains the exact identities and revision from one MCP begin-and-apply sequence.</summary>
internal sealed class NativeMcpStagedEdit
{
    /// <summary>Initializes one exact staged edit receipt.</summary>
    /// <param name="editId">The staged edit identity.</param>
    /// <param name="beginOperationId">The begin operation identity.</param>
    /// <param name="applyOperationId">The apply operation identity.</param>
    /// <param name="editorId">The staged EditorID.</param>
    /// <param name="revision">The revision after Apply.</param>
    public NativeMcpStagedEdit(
        Guid editId,
        Guid beginOperationId,
        Guid applyOperationId,
        string editorId,
        WorkspaceRevision revision)
    {
        EditId = editId;
        BeginOperationId = beginOperationId;
        ApplyOperationId = applyOperationId;
        EditorId = editorId;
        Revision = revision;
    }

    /// <summary>Gets the MCP edit-session identity.</summary>
    public Guid EditId { get; }

    /// <summary>Gets the exact begin operation identity.</summary>
    public Guid BeginOperationId { get; }

    /// <summary>Gets the exact apply operation identity.</summary>
    public Guid ApplyOperationId { get; }

    /// <summary>Gets the staged EditorID.</summary>
    public string EditorId { get; }

    /// <summary>Gets the revision produced by Apply.</summary>
    public WorkspaceRevision Revision { get; }
}

/// <summary>Stores one immutable MCP save request for exact replay assertions.</summary>
internal sealed class NativeMcpSaveRequest
{
    /// <summary>Initializes an exact save request.</summary>
    /// <param name="workspaceId">The owning MCP workspace.</param>
    /// <param name="operationId">The idempotent save identity.</param>
    /// <param name="expectedRevision">The current local revision.</param>
    /// <param name="expectedOutputBaselineHandle">The exact retained output baseline handle.</param>
    public NativeMcpSaveRequest(
        Guid workspaceId,
        Guid operationId,
        WorkspaceRevision expectedRevision,
        string expectedOutputBaselineHandle)
    {
        WorkspaceId = workspaceId;
        OperationId = operationId;
        ExpectedRevision = expectedRevision;
        ExpectedOutputBaselineHandle = expectedOutputBaselineHandle;
    }

    /// <summary>Gets the owning MCP workspace identity.</summary>
    public Guid WorkspaceId { get; }

    /// <summary>Gets the idempotent save operation identity.</summary>
    public Guid OperationId { get; }

    /// <summary>Gets the exact local revision captured for this request.</summary>
    public WorkspaceRevision ExpectedRevision { get; }

    /// <summary>Gets the exact retained output baseline handle.</summary>
    public string ExpectedOutputBaselineHandle { get; }

    /// <summary>Creates the closed SDK argument dictionary without changing retained identity.</summary>
    /// <returns>The exact protocol arguments.</returns>
    public IReadOnlyDictionary<string, object?> CreateArguments()
    {
        return new Dictionary<string, object?>
        {
            ["workspaceId"] = WorkspaceId.ToString("D"),
            ["operationId"] = OperationId.ToString("D"),
            ["expectedRevision"] = new Dictionary<string, object?>
            {
                ["baselineId"] = ExpectedRevision.BaselineId.ToString("D"),
                ["sequence"] = ExpectedRevision.Sequence.ToString(CultureInfo.InvariantCulture),
            },
            ["expectedOutputBaselineHandle"] = ExpectedOutputBaselineHandle,
        };
    }
}

/// <summary>Projects the complete bounded MCP save outcome used by cross-surface assertions.</summary>
internal sealed class NativeMcpSaveOutcome
{
    /// <summary>Initializes one projected structured save outcome.</summary>
    /// <param name="workspaceId">The owning workspace.</param>
    /// <param name="operationId">The save operation identity.</param>
    /// <param name="baseRevision">The exact request revision.</param>
    /// <param name="resultRevision">The exact result revision.</param>
    /// <param name="status">The independent commitment status.</param>
    /// <param name="errorCode">The stable typed failure code, when present.</param>
    /// <param name="committedBaselineHandle">The committed baseline handle, when committed.</param>
    /// <param name="outputHandle">The resolved output-association handle, when recovery evidence supplies one.</param>
    /// <param name="resolvedEvidenceHandle">The resolved-evidence handle, when present.</param>
    /// <param name="recoveryEvidenceToken">The exact recovery token, when the save outcome is uncertain.</param>
    /// <param name="warningCount">The reported warning count.</param>
    /// <param name="detailsUnavailable">Whether complete result metadata could not be retained.</param>
    /// <param name="detailsHandle">The complete save-result metadata handle, when retained.</param>
    /// <param name="detailsKind">The retained details metadata kind, when present.</param>
    private NativeMcpSaveOutcome(
        Guid workspaceId,
        Guid operationId,
        WorkspaceRevision baseRevision,
        WorkspaceRevision resultRevision,
        string status,
        string? errorCode,
        string? committedBaselineHandle,
        string? outputHandle,
        string? resolvedEvidenceHandle,
        string? recoveryEvidenceToken,
        int warningCount,
        bool detailsUnavailable,
        string? detailsHandle,
        string? detailsKind)
    {
        WorkspaceId = workspaceId;
        OperationId = operationId;
        BaseRevision = baseRevision;
        ResultRevision = resultRevision;
        Status = status;
        ErrorCode = errorCode;
        CommittedBaselineHandle = committedBaselineHandle;
        OutputHandle = outputHandle;
        ResolvedEvidenceHandle = resolvedEvidenceHandle;
        RecoveryEvidenceToken = recoveryEvidenceToken;
        WarningCount = warningCount;
        DetailsUnavailable = detailsUnavailable;
        DetailsHandle = detailsHandle;
        DetailsKind = detailsKind;
    }

    /// <summary>Gets the exact workspace identity returned by Save.</summary>
    public Guid WorkspaceId { get; }

    /// <summary>Gets the exact save operation identity.</summary>
    public Guid OperationId { get; }

    /// <summary>Gets the request revision echoed by the result.</summary>
    public WorkspaceRevision BaseRevision { get; }

    /// <summary>Gets the local revision after the save attempt.</summary>
    public WorkspaceRevision ResultRevision { get; }

    /// <summary>Gets the commitment status independently from transport success.</summary>
    public string Status { get; }

    /// <summary>Gets the stable failure code, when present.</summary>
    public string? ErrorCode { get; }

    /// <summary>Gets the committed baseline metadata handle, when the result committed.</summary>
    public string? CommittedBaselineHandle { get; }

    /// <summary>Gets the output-association metadata handle supplied with resolved evidence, when present.</summary>
    public string? OutputHandle { get; }

    /// <summary>Gets the resolved-output-evidence metadata handle, when present.</summary>
    public string? ResolvedEvidenceHandle { get; }

    /// <summary>Gets the exact recovery evidence token, when the save outcome is uncertain.</summary>
    public string? RecoveryEvidenceToken { get; }

    /// <summary>Gets the number of warnings retained by the bounded result.</summary>
    public int WarningCount { get; }

    /// <summary>Gets whether the result could not retain its complete metadata group.</summary>
    public bool DetailsUnavailable { get; }

    /// <summary>Gets the complete save-result metadata handle, when retained.</summary>
    public string? DetailsHandle { get; }

    /// <summary>Gets the retained result metadata kind, when available.</summary>
    public string? DetailsKind { get; }

    /// <summary>Projects one successful structured response into typed test state.</summary>
    /// <param name="value">The nested tool-specific result.</param>
    /// <returns>The complete bounded save outcome.</returns>
    public static NativeMcpSaveOutcome FromJson(JsonElement value)
    {
        var baseline = value.GetProperty("committedBaseline");
        var output = value.GetProperty("output");
        var resolvedEvidence = value.GetProperty("resolvedEvidence");
        var details = value.GetProperty("details");
        return new NativeMcpSaveOutcome(
            Guid.Parse(value.GetProperty("workspaceId").GetString().ShouldNotBeNull()),
            Guid.Parse(value.GetProperty("operationId").GetString().ShouldNotBeNull()),
            ReadRevision(value.GetProperty("baseRevision")),
            ReadRevision(value.GetProperty("resultRevision")),
            value.GetProperty("status").GetString().ShouldNotBeNull(),
            ReadNullableString(value.GetProperty("errorCode")),
            ReadNullableHandle(baseline),
            ReadNullableHandle(output),
            ReadNullableHandle(resolvedEvidence),
            ReadNullableString(value.GetProperty("recoveryEvidenceToken")),
            value.GetProperty("warningCount").GetInt32(),
            value.GetProperty("detailsUnavailable").GetBoolean(),
            ReadNullableHandle(details),
            details.ValueKind == JsonValueKind.Null ? null : details.GetProperty("kind").GetString());
    }

    /// <summary>Reads one canonical protocol revision.</summary>
    /// <param name="value">The revision object.</param>
    /// <returns>The exact engine revision.</returns>
    private static WorkspaceRevision ReadRevision(JsonElement value)
    {
        return new WorkspaceRevision(
            Guid.Parse(value.GetProperty("baselineId").GetString().ShouldNotBeNull()),
            ulong.Parse(value.GetProperty("sequence").GetString().ShouldNotBeNull(), CultureInfo.InvariantCulture));
    }

    /// <summary>Reads a nullable JSON string without conflating JSON null with an empty string.</summary>
    /// <param name="value">The JSON property value.</param>
    /// <returns>The string or <see langword="null"/>.</returns>
    private static string? ReadNullableString(JsonElement value)
    {
        return value.ValueKind == JsonValueKind.Null ? null : value.GetString();
    }

    /// <summary>Reads the opaque handle from a nullable metadata-reference object.</summary>
    /// <param name="value">The nullable metadata-reference property.</param>
    /// <returns>The non-empty opaque handle, or <see langword="null"/>.</returns>
    private static string? ReadNullableHandle(JsonElement value)
    {
        return value.ValueKind == JsonValueKind.Null
            ? null
            : value.GetProperty("handle").GetString().ShouldNotBeNull();
    }
}
