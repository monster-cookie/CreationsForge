using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace CreationsForge.Console.Mcp;

/// <summary>
/// Lists participating native plugins in bounded stateless summary pages.
/// </summary>
public sealed class PluginsListTool : McpToolBase
{
    /// <summary>The summary cursor section identifier.</summary>
    private const string Section = "plugins";

    /// <summary>The summary cursor policy identifier.</summary>
    private const string Policy = "summary_v1";

    /// <summary>The exact accepted argument names.</summary>
    private static readonly IReadOnlySet<string> AllowedArguments = new HashSet<string>(StringComparer.Ordinal)
    {
        "workspaceId",
        "maxResults",
        "cursor",
    };

    /// <summary>The closed bounded-page input schema.</summary>
    private static readonly JsonElement InputSchema = ParseSchema(
        """{"type":"object","properties":{"workspaceId":{"type":"string","format":"uuid"},"maxResults":{"type":"integer","minimum":1,"maximum":250},"cursor":{"type":"string","minLength":1,"maxLength":16384}},"required":["workspaceId"],"additionalProperties":false}""");

    /// <summary>The closed plugin summary output schema.</summary>
    private static readonly JsonElement OutputSchema = McpToolSchema.Output(
        "{\"type\":\"object\",\"properties\":{\"workspaceId\":{\"type\":\"string\",\"format\":\"uuid\"},\"revision\":" + McpToolSchema.Revision + ",\"offset\":{\"type\":\"integer\",\"minimum\":0},\"count\":{\"type\":\"integer\",\"minimum\":0},\"totalCount\":{\"type\":\"integer\",\"minimum\":0},\"plugins\":{\"type\":\"array\",\"items\":{\"type\":\"object\",\"properties\":{\"modKey\":{\"type\":\"string\",\"minLength\":1},\"path\":{\"type\":\"string\",\"minLength\":1},\"loadOrderIndex\":{\"type\":\"integer\",\"minimum\":0},\"role\":{\"type\":\"string\",\"enum\":[\"source\",\"load_order\",\"output\"]}},\"required\":[\"modKey\",\"path\",\"loadOrderIndex\",\"role\"],\"additionalProperties\":false}},\"cursor\":" + McpToolSchema.NullableString + ",\"warnings\":{\"type\":\"array\",\"items\":" + McpToolSchema.Warning + "}},\"required\":[\"workspaceId\",\"revision\",\"offset\",\"count\",\"totalCount\",\"plugins\",\"cursor\",\"warnings\"],\"additionalProperties\":false}");

    /// <summary>The host-owned workspace registry.</summary>
    private readonly McpWorkspaceRegistry WorkspaceRegistry;

    /// <summary>Initializes the bounded plugin-list tool.</summary>
    /// <param name="workspaceRegistry">The registry that owns active engine workspaces.</param>
    public PluginsListTool(McpWorkspaceRegistry workspaceRegistry)
    {
        ArgumentNullException.ThrowIfNull(workspaceRegistry);
        WorkspaceRegistry = workspaceRegistry;
    }

    /// <summary>Gets the closed read-only plugin-list descriptor.</summary>
    public override Tool ProtocolTool { get; } = new Tool
    {
        Name = "creationsforge_plugins_list",
        Title = "List workspace plugins",
        Description = "Lists source, load-order, and selected-output plugins in bounded deterministic transport pages.",
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

    /// <summary>Re-reads the native plugin summary and applies stateless transport paging.</summary>
    /// <param name="request">The MCP request.</param>
    /// <param name="cancellationToken">The token propagated through the engine read.</param>
    /// <returns>One bounded ordered plugin page.</returns>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is observed.</exception>
    public override async ValueTask<CallToolResult> InvokeAsync(
        RequestContext<CallToolRequestParams> request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!McpInput.TryGetArguments(request, AllowedArguments, out var arguments, out var error) ||
            !McpInput.TryGetRequiredGuid(arguments, "workspaceId", out var workspaceId, out error) ||
            !McpInput.TryGetMaximumResults(arguments, out var maximumResults, out error) ||
            !McpInput.TryGetOptionalString(arguments, "cursor", McpInput.MaximumCursorLength, null, out var cursor, out error) ||
            (cursor is not null && string.IsNullOrWhiteSpace(cursor)))
        {
            return Error("invalid_arguments", string.IsNullOrEmpty(error) ? "Argument 'cursor' cannot be empty." : error);
        }

        var engineResult = await WorkspaceRegistry.ExecuteAsync(
            workspaceId,
            (workspace, token) => workspace.ListPluginsAsync(token),
            cancellationToken).ConfigureAwait(false);
        if (!engineResult.Succeeded)
        {
            return EngineFailure(engineResult);
        }

        if (engineResult.Value is null ||
            !McpProjection.TryGetResultRevision(engineResult, out var revision, out error))
        {
            return Error("unexpected_failure", engineResult.Value is null
                ? "The engine reported success without plugin summaries."
                : error);
        }

        var fingerprint = McpPageCursor.CreateFingerprint(workspaceId.ToString("D"), maximumResults.ToString());
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
            return Error("invalid_cursor", "The plugin-list cursor is invalid or does not match the fresh workspace revision and request.");
        }

        var plugins = engineResult.Value;
        if (position > plugins.Count)
        {
            return Error("invalid_cursor", "The plugin-list cursor position is outside the fresh result.");
        }

        var requestedCount = Math.Min(maximumResults, Math.Max(plugins.Count - position, 1));
        for (var count = requestedCount; count >= 1; count--)
        {
            var page = plugins.Skip(position).Take(count).ToArray();
            var nextPosition = position + page.Length;
            var projected = McpProjection.Json(new
            {
                workspaceId = workspaceId.ToString("D"),
                revision = McpProjection.Revision(revision),
                offset = position,
                count = page.Length,
                totalCount = plugins.Count,
                plugins = page.Select(plugin => new
                {
                    modKey = plugin.ModKey.ToString(),
                    path = plugin.Path,
                    loadOrderIndex = plugin.LoadOrderIndex,
                    role = McpProjection.PluginRole(plugin.Role),
                }).ToArray(),
                cursor = nextPosition < plugins.Count
                    ? McpPageCursor.Encode(workspaceId, revision, fingerprint, string.Empty, Section, Policy, nextPosition)
                    : null,
                warnings = engineResult.Warnings.Select(McpProjection.Warning).ToArray(),
            });
            if (FitsStructuredResultBudget(projected))
            {
                return Success(projected, $"Returned {page.Length} of {plugins.Count} workspace plugin(s) from offset {position}.");
            }
        }

        return Error("result_too_large", "A single plugin summary or its operation metadata exceeds the 64 KiB structured result budget.");
    }
}
