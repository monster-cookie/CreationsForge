using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace CreationsForge.Mcp;

/// <summary>
/// Forwards bounded reference searches without retaining an MCP-side index or cursor.
/// </summary>
public sealed class ReferencesSearchTool : McpToolBase
{
    /// <summary>The exact accepted argument names.</summary>
    private static readonly IReadOnlySet<string> AllowedArguments = new HashSet<string>(StringComparer.Ordinal)
    {
        "workspaceId",
        "query",
        "scope",
        "containingModKey",
        "maxResults",
        "cursor",
    };

    /// <summary>The closed engine-search input schema with engine-owned bounds.</summary>
    private static readonly JsonElement InputSchema = ParseSchema(
        """{"type":"object","properties":{"workspaceId":{"type":"string","format":"uuid"},"query":{"type":"string","minLength":1,"maxLength":256},"scope":{"type":"string","enum":["winning_overrides","all_contexts","source","staged_output"]},"containingModKey":{"type":"string","minLength":1,"maxLength":1024},"maxResults":{"type":"integer","minimum":1,"maximum":250},"cursor":{"type":"string","minLength":1,"maxLength":16384}},"required":["workspaceId","query","scope"],"additionalProperties":false}""");

    /// <summary>The closed engine-search output schema.</summary>
    private static readonly JsonElement OutputSchema = McpToolSchema.Output(
        "{\"type\":\"object\",\"properties\":{\"workspaceId\":{\"type\":\"string\",\"format\":\"uuid\"},\"revision\":" + McpToolSchema.Revision + ",\"matches\":{\"type\":\"array\",\"items\":{\"type\":\"object\",\"properties\":{\"formKey\":{\"type\":\"string\",\"minLength\":1},\"recordType\":{\"type\":\"string\",\"minLength\":1},\"editorId\":" + McpToolSchema.NullableString + ",\"containingModKey\":" + McpToolSchema.NullableString + ",\"sourcePath\":" + McpToolSchema.NullableString + ",\"loadOrderIndex\":" + McpToolSchema.NullableInteger + ",\"role\":" + McpToolSchema.NullableString + ",\"isDeleted\":{\"type\":\"boolean\"}},\"required\":[\"formKey\",\"recordType\",\"editorId\",\"containingModKey\",\"sourcePath\",\"loadOrderIndex\",\"role\",\"isDeleted\"],\"additionalProperties\":false}},\"cursor\":" + McpToolSchema.NullableString + ",\"warnings\":{\"type\":\"array\",\"items\":" + McpToolSchema.Warning + "}},\"required\":[\"workspaceId\",\"revision\",\"matches\",\"cursor\",\"warnings\"],\"additionalProperties\":false}");

    /// <summary>The host-owned workspace registry.</summary>
    private readonly McpWorkspaceRegistry WorkspaceRegistry;

    /// <summary>Initializes the reference-search tool.</summary>
    /// <param name="workspaceRegistry">The registry that owns active engine workspaces.</param>
    public ReferencesSearchTool(McpWorkspaceRegistry workspaceRegistry)
    {
        ArgumentNullException.ThrowIfNull(workspaceRegistry);
        WorkspaceRegistry = workspaceRegistry;
    }

    /// <summary>Gets the closed read-only reference-search descriptor.</summary>
    public override Tool ProtocolTool { get; } = new Tool
    {
        Name = "creationsforge_references_search",
        Title = "Search references",
        Description = "Searches record identities and EditorIDs through the engine's bounded deterministic pager.",
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

    /// <summary>Passes the normalized bounded request and engine-issued cursor unchanged to the workspace.</summary>
    /// <param name="request">The MCP request.</param>
    /// <param name="cancellationToken">The token propagated through engine traversal.</param>
    /// <returns>One exact engine search page.</returns>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is observed.</exception>
    public override async ValueTask<CallToolResult> InvokeAsync(
        RequestContext<CallToolRequestParams> request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!McpInput.TryGetArguments(request, AllowedArguments, out var arguments, out var error) ||
            !McpInput.TryGetRequiredGuid(arguments, "workspaceId", out var workspaceId, out error) ||
            !McpInput.TryGetRequiredString(arguments, "query", ReferenceSearchRequest.MaximumQueryLength, out var query, out error) ||
            !McpInput.TryGetScope(arguments, "scope", out var scope, out error) ||
            !McpInput.TryGetOptionalModKey(arguments, "containingModKey", out var containingModKey, out error) ||
            !McpInput.TryGetMaximumResults(arguments, out var maximumResults, out error) ||
            !McpInput.TryGetOptionalString(arguments, "cursor", McpInput.MaximumCursorLength, null, out var cursor, out error) ||
            (cursor is not null && string.IsNullOrWhiteSpace(cursor)))
        {
            return Error("invalid_arguments", string.IsNullOrEmpty(error) ? "Argument 'cursor' cannot be empty." : error);
        }

        ReferenceSearchRequest engineRequest;
        try
        {
            engineRequest = new ReferenceSearchRequest(
                query,
                maximumResults,
                cursor,
                scope,
                containingModKey);
        }
        catch (ArgumentException exception)
        {
            return Error("invalid_arguments", exception.Message);
        }

        var engineResult = await WorkspaceRegistry.ExecuteAsync(
            workspaceId,
            (workspace, token) => workspace.SearchReferencesAsync(engineRequest, token),
            cancellationToken).ConfigureAwait(false);
        if (!engineResult.Succeeded)
        {
            return EngineFailure(engineResult);
        }

        if (engineResult.Value is null ||
            !McpProjection.TryGetResultRevision(engineResult, out var revision, out error))
        {
            return Error("unexpected_failure", engineResult.Value is null
                ? "The engine reported success without a reference-search page."
                : error);
        }

        var page = engineResult.Value;
        var projected = McpProjection.Json(new
        {
            workspaceId = workspaceId.ToString("D"),
            revision = McpProjection.Revision(revision),
            matches = page.Matches.Select(match => new
            {
                formKey = match.FormKey.ToString(),
                recordType = match.RecordType,
                editorId = match.EditorId,
                containingModKey = match.ContainingModKey?.ToString(),
                sourcePath = match.SourcePath,
                loadOrderIndex = match.LoadOrderIndex,
                role = match.Role.HasValue ? McpProjection.PluginRole(match.Role.Value) : null,
                isDeleted = match.IsDeleted,
            }).ToArray(),
            cursor = page.ContinuationToken,
            warnings = engineResult.Warnings.Select(McpProjection.Warning).ToArray(),
        });
        if (!FitsStructuredResultBudget(projected))
        {
            return Error("result_too_large", "The exact engine search page exceeds the 64 KiB structured result budget; retry with a smaller maxResults value.");
        }

        return Success(projected, $"Returned {page.Matches.Count} reference match(es).");
    }
}
