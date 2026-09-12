using System.Security.Cryptography;
using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeWire;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace CreationsForge.Console.Mcp;

/// <summary>Reads and pages a fresh complete preview of staged FormList changes.</summary>
internal sealed class WorkspacePreviewTool : McpToolBase
{
    /// <summary>The accepted argument names.</summary>
    private static readonly IReadOnlySet<string> AllowedArguments = new HashSet<string>(StringComparer.Ordinal) { "workspaceId", "expectedRevision", "path", "maxResults", "cursor" };

    /// <summary>The closed preview input schema.</summary>
    private static readonly JsonElement InputSchema = ParseSchema("{\"type\":\"object\",\"properties\":{\"workspaceId\":{\"type\":\"string\",\"format\":\"uuid\"},\"expectedRevision\":" + McpToolSchema.Revision + ",\"path\":{\"type\":\"string\",\"maxLength\":4096},\"maxResults\":{\"type\":\"integer\",\"minimum\":1,\"maximum\":250},\"cursor\":{\"type\":\"string\",\"minLength\":1,\"maxLength\":16384}},\"required\":[\"workspaceId\"],\"additionalProperties\":false}");

    /// <summary>The closed preview output schema.</summary>
    private static readonly JsonElement OutputSchema = McpToolSchema.Output("{\"type\":\"object\",\"properties\":{\"workspaceId\":{\"type\":\"string\"},\"revision\":" + McpToolSchema.Revision + ",\"page\":" + McpToolSchema.JsonPage + "},\"required\":[\"workspaceId\",\"revision\",\"page\"],\"additionalProperties\":false}");

    /// <summary>The workspace registry.</summary>
    private readonly McpWorkspaceRegistry Registry;

    /// <summary>Initializes the preview tool.</summary>
    /// <param name="registry">The workspace registry.</param>
    internal WorkspacePreviewTool(McpWorkspaceRegistry registry) => Registry = registry ?? throw new ArgumentNullException(nameof(registry));

    /// <summary>Gets the preview descriptor.</summary>
    public override Tool ProtocolTool { get; } = new()
    {
        Name = "creationsforge_workspace_preview", Title = "Preview staged FormList changes", Description = "Builds a fresh complete native preview and pages its comparisons, warnings, and detached before/after views without writing output files.",
        InputSchema = InputSchema, OutputSchema = OutputSchema,
        Annotations = new ToolAnnotations { ReadOnlyHint = true, IdempotentHint = true, DestructiveHint = false, OpenWorldHint = false },
    };

    /// <summary>Builds a fresh preview before validating revision and cursor bindings.</summary>
    /// <param name="request">The MCP request.</param>
    /// <param name="cancellationToken">The propagated token.</param>
    /// <returns>One bounded page of the fresh preview.</returns>
    public override async ValueTask<CallToolResult> InvokeAsync(RequestContext<CallToolRequestParams> request, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!McpInput.TryGetArguments(request, AllowedArguments, out var arguments, out var error) ||
            !McpInput.TryGetRequiredGuid(arguments, "workspaceId", out var workspaceId, out error) ||
            !McpInput.TryGetOptionalRevision(arguments, out var expectedRevision, out error) ||
            !McpInput.TryGetOptionalString(arguments, "path", McpInput.MaximumJsonPointerLength, string.Empty, out var path, out error) ||
            !McpInput.TryGetMaximumResults(arguments, out var maximumResults, out error) ||
            !McpInput.TryGetOptionalString(arguments, "cursor", McpInput.MaximumCursorLength, null, out var cursor, out error) ||
            (cursor is not null && string.IsNullOrWhiteSpace(cursor)) ||
            !McpJsonPager.TryValidatePath(path!, out error))
        {
            return Error("invalid_arguments", string.IsNullOrEmpty(error) ? "Argument 'cursor' cannot be empty." : error);
        }

        var result = await Registry.ExecuteAsync(workspaceId, (workspace, token) => workspace.PreviewAsync(token), cancellationToken).ConfigureAwait(false);
        if (!result.Succeeded) return EngineFailure(result);
        if (result.Value is null || !McpProjection.TryGetResultRevision(result, out var revision, out error)) return Error("unexpected_failure", result.Value is null ? "The engine reported success without a workspace preview." : error);
        if (expectedRevision.HasValue && expectedRevision.Value != revision) return Error("revision_conflict", "The expected revision does not match the fresh workspace preview revision.");

        var root = Project(result.Value);
        var fingerprint = McpPageCursor.CreateFingerprint(workspaceId.ToString("D"), path, maximumResults.ToString(System.Globalization.CultureInfo.InvariantCulture), expectedRevision?.ToString());
        var position = 0;
        if (cursor is not null && !McpPageCursor.TryDecode(cursor, workspaceId, revision, fingerprint, path!, "preview", McpJsonPager.Policy, out position)) return Error("invalid_cursor", "The preview cursor is invalid or does not match the fresh workspace revision and normalized request.");
        for (var count = maximumResults; count >= 1; count--)
        {
            if (!McpJsonPager.TryCreatePage(root, path!, position, count, next => McpPageCursor.Encode(workspaceId, revision, fingerprint, path!, "preview", McpJsonPager.Policy, next), out var page, out error)) return Error(cursor is null ? "invalid_arguments" : "invalid_cursor", error);
            var projected = McpProjection.Json(new { workspaceId = workspaceId.ToString("D"), revision = McpProjection.Revision(revision), page });
            if (FitsStructuredResultBudget(projected)) return Success(projected, $"Previewed staged changes at revision {revision} from offset {position}.");
        }

        return Error("result_too_large", "A single preview descriptor or scalar chunk exceeds the 64 KiB structured result budget.");
    }

    /// <summary>Projects the complete detached preview tree in stable engine order.</summary>
    /// <param name="preview">The fresh complete Core preview.</param>
    /// <returns>A detached JSON tree suitable for stateless paging.</returns>
    private static JsonElement Project(WorkspacePreview preview)
    {
        return McpProjection.Json(new
        {
            unresolvedReferenceCount = preview.UnresolvedReferenceCount,
            warnings = preview.Warnings.Select(McpProjection.Warning).ToArray(),
            comparisons = preview.Comparisons.Select(comparison => new
            {
                formKey = comparison.FormKey.ToString(), beforeContext = McpProjection.Context(comparison.BeforeContext), afterContext = McpProjection.Context(comparison.AfterContext),
                before = comparison.Before, after = comparison.After, changes = comparison.Changes.Select(McpProjection.Change).ToArray(), warnings = comparison.Warnings.Select(McpProjection.Warning).ToArray(),
            }).ToArray(),
        });
    }
}

/// <summary>Lists bounded immutable native edit schema keys for one exact game and release.</summary>
internal sealed class FormListEditSchemasListTool : McpToolBase
{
    /// <summary>The accepted argument names.</summary>
    private static readonly IReadOnlySet<string> AllowedArguments = new HashSet<string>(StringComparer.Ordinal) { "game", "release", "maxResults", "cursor" };

    /// <summary>The closed list schema.</summary>
    private static readonly JsonElement InputSchema = ParseSchema("{\"type\":\"object\",\"properties\":{\"game\":{\"type\":\"string\",\"enum\":[\"starfield\",\"fallout4\",\"skyrim\"]},\"release\":{\"type\":\"string\",\"enum\":[\"starfield\",\"fallout4\",\"skyrim_se\"]},\"maxResults\":{\"type\":\"integer\",\"minimum\":1,\"maximum\":250},\"cursor\":{\"type\":\"string\",\"minLength\":1,\"maxLength\":16384}},\"required\":[\"game\",\"release\"],\"additionalProperties\":false}");

    /// <summary>The closed list output schema.</summary>
    private static readonly JsonElement OutputSchema = McpToolSchema.Output("{\"type\":\"object\",\"properties\":{\"game\":{\"type\":\"string\"},\"release\":{\"type\":\"string\"},\"schemaVersion\":{\"type\":\"string\"},\"catalogId\":{\"type\":\"string\"},\"offset\":{\"type\":\"integer\"},\"count\":{\"type\":\"integer\"},\"totalCount\":{\"type\":\"integer\"},\"nodes\":{\"type\":\"array\",\"items\":{\"type\":\"object\",\"properties\":{\"kind\":{\"type\":\"string\",\"enum\":[\"command\",\"type\"]},\"name\":{\"type\":\"string\",\"minLength\":1}},\"required\":[\"kind\",\"name\"],\"additionalProperties\":false}},\"cursor\":" + McpToolSchema.NullableString + "},\"required\":[\"game\",\"release\",\"schemaVersion\",\"catalogId\",\"offset\",\"count\",\"totalCount\",\"nodes\",\"cursor\"],\"additionalProperties\":false}");

    /// <summary>The immutable catalogs.</summary>
    private readonly IReadOnlyList<IFormListEditWireSchemaCatalog> Catalogs;

    /// <summary>Initializes the schema-list tool.</summary>
    /// <param name="catalogs">The exact per-game catalogs.</param>
    internal FormListEditSchemasListTool(IReadOnlyList<IFormListEditWireSchemaCatalog> catalogs) => Catalogs = catalogs ?? throw new ArgumentNullException(nameof(catalogs));

    /// <summary>Gets the schema-list descriptor.</summary>
    public override Tool ProtocolTool { get; } = new()
    {
        Name = "creationsforge_formlist_edit_schemas_list", Title = "List FormList edit schemas", Description = "Lists command and native-type schema keys from one immutable game catalog. Resolve referenced type URIs lazily with the schema-read tool under the same catalog identity.",
        InputSchema = InputSchema, OutputSchema = OutputSchema,
        Annotations = new ToolAnnotations { ReadOnlyHint = true, IdempotentHint = true, DestructiveHint = false, OpenWorldHint = false },
    };

    /// <summary>Returns one bounded deterministic key page.</summary>
    /// <param name="request">The MCP request.</param>
    /// <param name="cancellationToken">The token checked before projection.</param>
    /// <returns>The catalog identity and key page.</returns>
    public override ValueTask<CallToolResult> InvokeAsync(RequestContext<CallToolRequestParams> request, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!McpInput.TryGetArguments(request, AllowedArguments, out var arguments, out var error) || !McpInput.TryGetGameRelease(arguments, out var game, out var release, out error) || !McpInput.TryGetMaximumResults(arguments, out var maximumResults, out error) || !McpInput.TryGetOptionalString(arguments, "cursor", McpInput.MaximumCursorLength, null, out var cursor, out error)) return ValueTask.FromResult(Error("invalid_arguments", error));
        var catalog = Catalogs.SingleOrDefault(candidate => candidate.Identity.Game == game && candidate.Identity.Release == release);
        if (catalog is null) return ValueTask.FromResult(Error("unsupported_game_release", "No native edit schema catalog is available for the requested game and release."));
        var (syntheticId, revision) = CursorIdentity(catalog.Identity.CatalogId);
        var fingerprint = McpPageCursor.CreateFingerprint(catalog.Identity.CatalogId, maximumResults.ToString(System.Globalization.CultureInfo.InvariantCulture));
        var position = 0;
        if (cursor is not null && !McpPageCursor.TryDecode(cursor, syntheticId, revision, fingerprint, string.Empty, "schema_list", McpJsonPager.Policy, out position)) return ValueTask.FromResult(Error("invalid_cursor", "The schema-list cursor is invalid or belongs to another catalog request."));
        if (position > catalog.Nodes.Count) return ValueTask.FromResult(Error("invalid_cursor", "The schema-list cursor position is outside the immutable catalog."));
        for (var count = maximumResults; count >= 1; count--)
        {
            var nodes = catalog.Nodes.Skip(position).Take(count).Select(node => new { kind = node.Kind == NativeWireSchemaNodeKind.Command ? "command" : "type", name = node.Name }).ToArray();
            var next = position + nodes.Length;
            var projected = McpProjection.Json(new
            {
                game = McpAuthoringSupport.Game(game), release = McpAuthoringSupport.Release(release), schemaVersion = catalog.Identity.SchemaVersion, catalogId = catalog.Identity.CatalogId,
                offset = position, count = nodes.Length, totalCount = catalog.Nodes.Count, nodes,
                cursor = next < catalog.Nodes.Count ? McpPageCursor.Encode(syntheticId, revision, fingerprint, string.Empty, "schema_list", McpJsonPager.Policy, next) : null,
            });
            if (FitsStructuredResultBudget(projected)) return ValueTask.FromResult(Success(projected, $"Listed {nodes.Length} of {catalog.Nodes.Count} schema nodes from offset {position}."));
        }

        return ValueTask.FromResult(Error("result_too_large", "A single schema key exceeds the 64 KiB structured result budget."));
    }

    /// <summary>Derives cursor-only identity from an immutable catalog digest.</summary>
    /// <param name="catalogId">The canonical SHA-256 catalog identity.</param>
    /// <returns>A deterministic cursor workspace and revision identity.</returns>
    internal static (Guid Id, WorkspaceRevision Revision) CursorIdentity(string catalogId)
    {
        var bytes = SHA256.HashData(System.Text.Encoding.UTF8.GetBytes($"creationsforge-schema-cursor:{catalogId}"));
        var id = new Guid(bytes.AsSpan(0, 16));
        var baseline = new Guid(bytes.AsSpan(16, 16));
        if (id == Guid.Empty) id = new Guid("cf000001-0000-0000-0000-000000000001");
        if (baseline == Guid.Empty) baseline = new Guid("cf000002-0000-0000-0000-000000000002");
        return (id, new WorkspaceRevision(baseline, 0));
    }
}

/// <summary>Reads and pages one immutable command schema, type schema, or default template.</summary>
internal sealed class FormListEditSchemaReadTool : McpToolBase
{
    /// <summary>The accepted argument names.</summary>
    private static readonly IReadOnlySet<string> AllowedArguments = new HashSet<string>(StringComparer.Ordinal) { "game", "release", "catalogId", "kind", "name", "section", "path", "maxResults", "cursor" };

    /// <summary>The closed schema-read input.</summary>
    private static readonly JsonElement InputSchema = ParseSchema("{\"type\":\"object\",\"properties\":{\"game\":{\"type\":\"string\"},\"release\":{\"type\":\"string\"},\"catalogId\":{\"type\":\"string\",\"minLength\":64,\"maxLength\":64},\"kind\":{\"type\":\"string\",\"enum\":[\"command\",\"type\"]},\"name\":{\"type\":\"string\",\"minLength\":1,\"maxLength\":4096},\"section\":{\"type\":\"string\",\"enum\":[\"schema\",\"default\"]},\"path\":{\"type\":\"string\",\"maxLength\":4096},\"maxResults\":{\"type\":\"integer\",\"minimum\":1,\"maximum\":250},\"cursor\":{\"type\":\"string\",\"minLength\":1,\"maxLength\":16384}},\"required\":[\"game\",\"release\",\"catalogId\",\"kind\",\"name\",\"section\"],\"additionalProperties\":false}");

    /// <summary>The closed schema-read output.</summary>
    private static readonly JsonElement OutputSchema = McpToolSchema.Output("{\"type\":\"object\",\"properties\":{\"game\":{\"type\":\"string\"},\"release\":{\"type\":\"string\"},\"catalogId\":{\"type\":\"string\"},\"kind\":{\"type\":\"string\"},\"name\":{\"type\":\"string\"},\"section\":{\"type\":\"string\"},\"page\":" + McpToolSchema.JsonPage + "},\"required\":[\"game\",\"release\",\"catalogId\",\"kind\",\"name\",\"section\",\"page\"],\"additionalProperties\":false}");

    /// <summary>The immutable catalogs.</summary>
    private readonly IReadOnlyList<IFormListEditWireSchemaCatalog> Catalogs;

    /// <summary>Initializes the schema-read tool.</summary>
    /// <param name="catalogs">The exact catalogs.</param>
    internal FormListEditSchemaReadTool(IReadOnlyList<IFormListEditWireSchemaCatalog> catalogs) => Catalogs = catalogs ?? throw new ArgumentNullException(nameof(catalogs));

    /// <summary>Gets the schema-read descriptor.</summary>
    public override Tool ProtocolTool { get; } = new()
    {
        Name = "creationsforge_formlist_edit_schema_read", Title = "Read a FormList edit schema", Description = "Reads one immutable command or native-type schema section. Resolve each schema $ref lazily as a type node in the same catalog.",
        InputSchema = InputSchema, OutputSchema = OutputSchema,
        Annotations = new ToolAnnotations { ReadOnlyHint = true, IdempotentHint = true, DestructiveHint = false, OpenWorldHint = false },
    };

    /// <summary>Reads the exact catalog node before applying bounded stateless paging.</summary>
    /// <param name="request">The MCP request.</param>
    /// <param name="cancellationToken">The propagated token.</param>
    /// <returns>One page of schema or default JSON.</returns>
    public override ValueTask<CallToolResult> InvokeAsync(RequestContext<CallToolRequestParams> request, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!McpInput.TryGetArguments(request, AllowedArguments, out var arguments, out var error) || !McpInput.TryGetGameRelease(arguments, out var game, out var release, out error) || !McpInput.TryGetRequiredString(arguments, "catalogId", 64, out var catalogId, out error) || !McpInput.TryGetRequiredString(arguments, "kind", 16, out var kindText, out error) || !McpInput.TryGetRequiredString(arguments, "name", 4096, out var name, out error) || !McpInput.TryGetRequiredString(arguments, "section", 16, out var section, out error) || !McpInput.TryGetOptionalString(arguments, "path", McpInput.MaximumJsonPointerLength, string.Empty, out var path, out error) || !McpInput.TryGetMaximumResults(arguments, out var maximumResults, out error) || !McpInput.TryGetOptionalString(arguments, "cursor", McpInput.MaximumCursorLength, null, out var cursor, out error) || !McpJsonPager.TryValidatePath(path!, out error)) return ValueTask.FromResult(Error("invalid_arguments", error));
        var catalog = Catalogs.SingleOrDefault(candidate => candidate.Identity.Game == game && candidate.Identity.Release == release && string.Equals(candidate.Identity.CatalogId, catalogId, StringComparison.Ordinal));
        if (catalog is null) return ValueTask.FromResult(Error("invalid_schema_key", "The catalog identity is stale, foreign, or unavailable for the requested game and release."));
        var kind = kindText switch { "command" => NativeWireSchemaNodeKind.Command, "type" => NativeWireSchemaNodeKind.Type, _ => (NativeWireSchemaNodeKind)(-1) };
        if (!Enum.IsDefined(kind) || section is not "schema" and not "default") return ValueTask.FromResult(Error("invalid_arguments", "Arguments 'kind' and 'section' must identify command or type and schema or default."));
        NativeWireSchemaNodeKey key;
        try { key = new NativeWireSchemaNodeKey(catalogId, kind, name); } catch (ArgumentException exception) { return ValueTask.FromResult(Error("invalid_schema_key", exception.Message)); }
        var nodeResult = catalog.ReadNode(key, cancellationToken);
        if (!nodeResult.Succeeded) return ValueTask.FromResult(EngineFailure(nodeResult));
        if (nodeResult.Value is null) return ValueTask.FromResult(Error("unexpected_failure", "The schema catalog reported success without a node."));
        JsonElement? root = section == "schema" ? nodeResult.Value.Schema : nodeResult.Value.DefaultTemplate;
        var (syntheticId, revision) = FormListEditSchemasListTool.CursorIdentity(catalogId);
        var fingerprint = McpPageCursor.CreateFingerprint(catalogId, kindText, name, section, path, maximumResults.ToString(System.Globalization.CultureInfo.InvariantCulture));
        var position = 0;
        if (cursor is not null && !McpPageCursor.TryDecode(cursor, syntheticId, revision, fingerprint, path!, section, McpJsonPager.Policy, out position)) return ValueTask.FromResult(Error("invalid_cursor", "The schema cursor is invalid or belongs to another immutable node request."));
        for (var count = maximumResults; count >= 1; count--)
        {
            if (!McpJsonPager.TryCreatePage(root, path!, position, count, next => McpPageCursor.Encode(syntheticId, revision, fingerprint, path!, section, McpJsonPager.Policy, next), out var page, out error)) return ValueTask.FromResult(Error(cursor is null ? "invalid_arguments" : "invalid_cursor", error));
            var projected = McpProjection.Json(new { game = McpAuthoringSupport.Game(game), release = McpAuthoringSupport.Release(release), catalogId, kind = kindText, name, section, page });
            if (FitsStructuredResultBudget(projected)) return ValueTask.FromResult(Success(projected, $"Read {section} for {kindText} schema '{name}' from offset {position}."));
        }

        return ValueTask.FromResult(Error("result_too_large", "A single schema descriptor or scalar chunk exceeds the 64 KiB structured result budget."));
    }
}
