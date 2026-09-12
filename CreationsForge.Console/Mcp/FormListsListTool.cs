using System.Globalization;
using System.Text.Json;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace CreationsForge.Console.Mcp;

/// <summary>
/// Lists native FormLists in bounded stateless summary pages.
/// </summary>
public sealed class FormListsListTool : McpToolBase
{
    /// <summary>The summary cursor section identifier.</summary>
    private const string Section = "formlists";

    /// <summary>The summary cursor policy identifier.</summary>
    private const string Policy = "summary_v1";

    /// <summary>The exact accepted argument names.</summary>
    private static readonly IReadOnlySet<string> AllowedArguments = new HashSet<string>(StringComparer.Ordinal)
    {
        "workspaceId",
        "scope",
        "maxResults",
        "cursor",
    };

    /// <summary>The closed bounded FormList-summary input schema.</summary>
    private static readonly JsonElement InputSchema = ParseSchema(
        """{"type":"object","properties":{"workspaceId":{"type":"string","format":"uuid"},"scope":{"type":"string","enum":["winning_overrides","all_contexts","source","staged_output"]},"maxResults":{"type":"integer","minimum":1,"maximum":250},"cursor":{"type":"string","minLength":1,"maxLength":16384}},"required":["workspaceId","scope"],"additionalProperties":false}""");

    /// <summary>The closed FormList summary output schema.</summary>
    private static readonly JsonElement OutputSchema = McpToolSchema.Output(
        "{\"type\":\"object\",\"properties\":{\"workspaceId\":{\"type\":\"string\",\"format\":\"uuid\"},\"revision\":" + McpToolSchema.Revision + ",\"scope\":{\"type\":\"string\",\"enum\":[\"winning_overrides\",\"all_contexts\",\"source\",\"staged_output\"]},\"offset\":{\"type\":\"integer\",\"minimum\":0},\"count\":{\"type\":\"integer\",\"minimum\":0},\"totalCount\":{\"type\":\"integer\",\"minimum\":0},\"formLists\":{\"type\":\"array\",\"items\":{\"type\":\"object\",\"properties\":{\"formKey\":{\"type\":\"string\",\"minLength\":1},\"editorId\":" + McpToolSchema.NullableString + ",\"overrideCount\":{\"type\":\"integer\",\"minimum\":0},\"scope\":{\"type\":\"string\",\"enum\":[\"winning_overrides\",\"all_contexts\",\"source\",\"staged_output\"]},\"containingModKey\":" + McpToolSchema.NullableString + ",\"sourcePath\":" + McpToolSchema.NullableString + ",\"loadOrderIndex\":" + McpToolSchema.NullableInteger + ",\"role\":" + McpToolSchema.NullableString + "},\"required\":[\"formKey\",\"editorId\",\"overrideCount\",\"scope\",\"containingModKey\",\"sourcePath\",\"loadOrderIndex\",\"role\"],\"additionalProperties\":false}},\"cursor\":" + McpToolSchema.NullableString + ",\"warnings\":{\"type\":\"array\",\"items\":" + McpToolSchema.Warning + "}},\"required\":[\"workspaceId\",\"revision\",\"scope\",\"offset\",\"count\",\"totalCount\",\"formLists\",\"cursor\",\"warnings\"],\"additionalProperties\":false}");

    /// <summary>The host-owned workspace registry.</summary>
    private readonly McpWorkspaceRegistry WorkspaceRegistry;

    /// <summary>Initializes the bounded FormList-list tool.</summary>
    /// <param name="workspaceRegistry">The registry that owns active engine workspaces.</param>
    public FormListsListTool(McpWorkspaceRegistry workspaceRegistry)
    {
        ArgumentNullException.ThrowIfNull(workspaceRegistry);
        WorkspaceRegistry = workspaceRegistry;
    }

    /// <summary>Gets the closed read-only FormList-list descriptor.</summary>
    public override Tool ProtocolTool { get; } = new Tool
    {
        Name = "creationsforge_formlists_list",
        Title = "List workspace FormLists",
        Description = "Lists native FormList summaries for one explicit record scope in bounded deterministic transport pages.",
        InputSchema = InputSchema,
        OutputSchema = OutputSchema,
        Annotations = new ToolAnnotations
        {
            ReadOnlyHint = true,
            IdempotentHint = true,
            DestructiveHint = false,
            OpenWorldHint = false,
        },
    };

    /// <summary>Re-reads the selected native scope and applies stateless transport paging.</summary>
    /// <param name="request">The MCP request.</param>
    /// <param name="cancellationToken">The token propagated through the engine read.</param>
    /// <returns>One bounded ordered FormList summary page.</returns>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is observed.</exception>
    public override async ValueTask<CallToolResult> InvokeAsync(
        RequestContext<CallToolRequestParams> request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!McpInput.TryGetArguments(request, AllowedArguments, out var arguments, out var error) ||
            !McpInput.TryGetRequiredGuid(arguments, "workspaceId", out var workspaceId, out error) ||
            !McpInput.TryGetScope(arguments, "scope", out var scope, out error) ||
            !McpInput.TryGetMaximumResults(arguments, out var maximumResults, out error) ||
            !McpInput.TryGetOptionalString(arguments, "cursor", McpInput.MaximumCursorLength, null, out var cursor, out error) ||
            (cursor is not null && string.IsNullOrWhiteSpace(cursor)))
        {
            return Error("invalid_arguments", string.IsNullOrEmpty(error) ? "Argument 'cursor' cannot be empty." : error);
        }

        var engineResult = await WorkspaceRegistry.ExecuteAsync(
            workspaceId,
            (workspace, token) => workspace.ListFormListsAsync(scope, token),
            cancellationToken).ConfigureAwait(false);
        if (!engineResult.Succeeded)
        {
            return EngineFailure(engineResult);
        }

        if (engineResult.Value is null ||
            !McpProjection.TryGetResultRevision(engineResult, out var revision, out error))
        {
            return Error("unexpected_failure", engineResult.Value is null
                ? "The engine reported success without FormList summaries."
                : error);
        }

        var scopeName = McpProjection.Scope(scope);
        var fingerprint = McpPageCursor.CreateFingerprint(
            workspaceId.ToString("D"),
            scopeName,
            maximumResults.ToString(CultureInfo.InvariantCulture));
        var position = 0;
        if (cursor is not null && !McpPageCursor.TryDecode(
                cursor,
                workspaceId,
                revision,
                fingerprint,
                string.Empty,
                Section,
                Policy,
                out position))
        {
            return Error("invalid_cursor", "The FormList-list cursor is invalid or does not match the fresh workspace revision and request.");
        }

        var formLists = engineResult.Value;
        if (position > formLists.Count)
        {
            return Error("invalid_cursor", "The FormList-list cursor position is outside the fresh result.");
        }

        var requestedCount = Math.Min(maximumResults, Math.Max(formLists.Count - position, 1));
        for (var count = requestedCount; count >= 1; count--)
        {
            var page = formLists.Skip(position).Take(count).ToArray();
            var nextPosition = position + page.Length;
            var projected = McpProjection.Json(new
            {
                workspaceId = workspaceId.ToString("D"),
                revision = McpProjection.Revision(revision),
                scope = scopeName,
                offset = position,
                count = page.Length,
                totalCount = formLists.Count,
                formLists = page.Select(formList => new
                {
                    formKey = formList.FormKey.ToString(),
                    editorId = formList.EditorId,
                    overrideCount = formList.OverrideCount,
                    scope = McpProjection.Scope(formList.Scope),
                    containingModKey = formList.ContainingModKey?.ToString(),
                    sourcePath = formList.SourcePath,
                    loadOrderIndex = formList.LoadOrderIndex,
                    role = formList.Role.HasValue ? McpProjection.PluginRole(formList.Role.Value) : null,
                }).ToArray(),
                cursor = nextPosition < formLists.Count
                    ? McpPageCursor.Encode(workspaceId, revision, fingerprint, string.Empty, Section, Policy, nextPosition)
                    : null,
                warnings = engineResult.Warnings.Select(McpProjection.Warning).ToArray(),
            });
            if (FitsStructuredResultBudget(projected))
            {
                return Success(projected, $"Returned {page.Length} of {formLists.Count} FormList summary item(s) from offset {position}.");
            }
        }

        return Error("result_too_large", "A single FormList summary or its operation metadata exceeds the 64 KiB structured result budget.");
    }
}
