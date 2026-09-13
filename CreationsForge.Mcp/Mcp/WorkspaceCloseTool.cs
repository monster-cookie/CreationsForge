using System.Text.Json;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace CreationsForge.Mcp;

/// <summary>
/// Closes one registry-owned workspace and awaits its engine disposal.
/// </summary>
public sealed class WorkspaceCloseTool : McpToolBase
{
    /// <summary>The exact accepted argument names.</summary>
    private static readonly IReadOnlySet<string> AllowedArguments = new HashSet<string>(StringComparer.Ordinal)
    {
        "workspaceId",
    };

    /// <summary>The closed workspace identity input schema.</summary>
    private static readonly JsonElement InputSchema = ParseSchema(
        """{"type":"object","properties":{"workspaceId":{"type":"string","format":"uuid"}},"required":["workspaceId"],"additionalProperties":false}""");

    /// <summary>The closed disposal result schema.</summary>
    private static readonly JsonElement OutputSchema = McpToolSchema.Output(
        "{\"type\":\"object\",\"properties\":{\"workspaceId\":{\"type\":\"string\",\"format\":\"uuid\"},\"closed\":{\"type\":\"boolean\"}},\"required\":[\"workspaceId\",\"closed\"],\"additionalProperties\":false}");

    /// <summary>The host-owned workspace registry.</summary>
    private readonly McpWorkspaceRegistry WorkspaceRegistry;

    /// <summary>Initializes the workspace-close tool.</summary>
    /// <param name="workspaceRegistry">The registry that owns active workspaces.</param>
    public WorkspaceCloseTool(McpWorkspaceRegistry workspaceRegistry)
    {
        ArgumentNullException.ThrowIfNull(workspaceRegistry);
        WorkspaceRegistry = workspaceRegistry;
    }

    /// <summary>Gets the closed workspace-close protocol descriptor.</summary>
    public override Tool ProtocolTool { get; } = new Tool
    {
        Name = "creationsforge_workspace_close",
        Title = "Close a CreationsForge workspace",
        Description = "Stops new operations for one workspace, waits for active work, and releases its engine resources.",
        InputSchema = InputSchema,
        OutputSchema = OutputSchema,
        Annotations = new ToolAnnotations
        {
            ReadOnlyHint = false,
            IdempotentHint = true,
            DestructiveHint = false,
            OpenWorldHint = false,
        },
    };

    /// <summary>Checks cancellation before starting non-cancellable registry-owned disposal.</summary>
    /// <param name="request">The MCP request.</param>
    /// <param name="cancellationToken">The token observed before disposal ownership transfers.</param>
    /// <returns>A successful close receipt or a stable inactive-workspace error.</returns>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is already requested.</exception>
    public override async ValueTask<CallToolResult> InvokeAsync(
        RequestContext<CallToolRequestParams> request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!McpInput.TryGetArguments(request, AllowedArguments, out var arguments, out var error) ||
            !McpInput.TryGetRequiredGuid(arguments, "workspaceId", out var workspaceId, out error))
        {
            return Error("invalid_arguments", error);
        }

        var closed = await WorkspaceRegistry.CloseAsync(workspaceId).ConfigureAwait(false);
        return Success(
            McpProjection.Json(new
            {
                workspaceId = workspaceId.ToString("D"),
                closed,
            }),
            closed
                ? $"Closed CreationsForge workspace {workspaceId:D}."
                : $"Workspace {workspaceId:D} was already inactive.");
    }
}
