using System.Text.Json;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace CreationsForge.Console.Mcp;

/// <summary>
/// Reports the running MCP server identity and its current host-owned workspace lifecycle state.
/// </summary>
public sealed class ServerInfoTool : McpToolBase
{
    /// <summary>The closed no-argument input schema shared by every invocation.</summary>
    private static readonly JsonElement InputSchema = ParseSchema(
        """{"type":"object","properties":{},"additionalProperties":false}""");

    /// <summary>The closed success and error structured-output alternatives advertised to MCP clients.</summary>
    private static readonly JsonElement OutputSchema = ParseSchema(
        """{"oneOf":[{"type":"object","properties":{"ok":{"const":true},"result":{"type":"object","properties":{"serverName":{"const":"CreationsForge"},"serverVersion":{"type":"string","minLength":1},"transport":{"const":"stdio"},"lifecycle":{"type":"object","properties":{"state":{"const":"running"},"activeWorkspaceCount":{"type":"integer","minimum":0},"activeWorkspaceIds":{"type":"array","items":{"type":"string","format":"uuid"},"uniqueItems":true}},"required":["state","activeWorkspaceCount","activeWorkspaceIds"],"additionalProperties":false},"capabilities":{"type":"array","items":{"type":"string","enum":["server_info","workspace_registry","workspace_open","workspace_close","plugins_list","formlists_list","references_search","formlist_inspect","formlist_compare","workspace_state","metadata_read","save","save_recover","save_repair","output_recovery_resolve","output_select","formlist_begin_edit","formlist_apply_edit","workspace_preview","workspace_discard","output_reopen","formlist_edit_schemas_list","formlist_edit_schema_read"]},"uniqueItems":true}},"required":["serverName","serverVersion","transport","lifecycle","capabilities"],"additionalProperties":false}},"required":["ok","result"],"additionalProperties":false},{"type":"object","properties":{"ok":{"const":false},"error":{"type":"object","properties":{"code":{"type":"string","minLength":1},"message":{"type":"string","minLength":1}},"required":["code","message"],"additionalProperties":false}},"required":["ok","error"],"additionalProperties":false}]}""");

    /// <summary>The host-owned registry whose current lifecycle state is projected by this tool.</summary>
    private readonly McpWorkspaceRegistry WorkspaceRegistry;

    /// <summary>The informational version reported during protocol discovery and tool invocation.</summary>
    private readonly string ServerVersion;

    /// <summary>Indicates whether a real native workspace factory enabled the domain tool surface.</summary>
    private readonly bool DomainToolsAvailable;

    /// <summary>Indicates whether the complete authoring, save, recovery, and schema service set is available.</summary>
    private readonly bool AuthoringToolsAvailable;

    /// <summary>Initializes the server information tool.</summary>
    /// <param name="workspaceRegistry">The registry that owns active engine workspaces for this host.</param>
    /// <param name="serverVersion">The non-empty server informational version.</param>
    /// <param name="domainToolsAvailable">Whether real native workspace composition enabled domain capabilities.</param>
    /// <param name="authoringToolsAvailable">Whether the complete authoring service set enabled all production capabilities.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="workspaceRegistry"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="serverVersion"/> is empty.</exception>
    public ServerInfoTool(
        McpWorkspaceRegistry workspaceRegistry,
        string serverVersion,
        bool domainToolsAvailable = false,
        bool authoringToolsAvailable = false)
    {
        ArgumentNullException.ThrowIfNull(workspaceRegistry);
        ArgumentException.ThrowIfNullOrWhiteSpace(serverVersion);
        WorkspaceRegistry = workspaceRegistry;
        ServerVersion = serverVersion;
        DomainToolsAvailable = domainToolsAvailable;
        AuthoringToolsAvailable = authoringToolsAvailable;
    }

    /// <summary>Gets the explicit closed schemas and read-only lifecycle annotations for this tool.</summary>
    public override Tool ProtocolTool { get; } = new Tool
    {
        Name = "creationsforge_server_info",
        Title = "CreationsForge server information",
        Description = "Reports the stdio server identity and active host-owned workspace lifecycle state.",
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

    /// <summary>Validates the empty request and returns the current server and workspace lifecycle state.</summary>
    /// <param name="request">The protocol request, which must not contain arguments.</param>
    /// <param name="cancellationToken">The request cancellation token checked before reading registry state.</param>
    /// <returns>The current structured server information or a structured invalid-arguments error.</returns>
    /// <exception cref="OperationCanceledException">Thrown when the request is cancelled.</exception>
    public override ValueTask<CallToolResult> InvokeAsync(
        RequestContext<CallToolRequestParams> request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (request.Params?.Arguments is { Count: > 0 })
        {
            return ValueTask.FromResult(Error(
                "invalid_arguments",
                "creationsforge_server_info does not accept arguments."));
        }

        var workspaceIds = WorkspaceRegistry.GetWorkspaceIds();
        var result = JsonSerializer.SerializeToElement(new
        {
            serverName = "CreationsForge",
            serverVersion = ServerVersion,
            transport = "stdio",
            lifecycle = new
            {
                state = "running",
                activeWorkspaceCount = workspaceIds.Count,
                activeWorkspaceIds = workspaceIds.Select(id => id.ToString("D")).ToArray(),
            },
            capabilities = GetCapabilities(),
        });

        return ValueTask.FromResult(Success(
            result,
            $"CreationsForge MCP is running with {workspaceIds.Count} active workspace(s)."));
    }

    /// <summary>Returns only capabilities whose production dependencies were actually composed.</summary>
    /// <returns>The stable capability names in discovery order.</returns>
    private IReadOnlyList<string> GetCapabilities()
    {
        if (!DomainToolsAvailable)
        {
            return Array.AsReadOnly(new[]
            {
                "server_info",
                "workspace_registry",
            });
        }

        if (!AuthoringToolsAvailable)
        {
            return Array.AsReadOnly(new[]
            {
                "server_info",
                "workspace_registry",
                "workspace_open",
                "workspace_close",
                "plugins_list",
                "formlists_list",
                "references_search",
                "formlist_inspect",
                "formlist_compare",
            });
        }

        return Array.AsReadOnly(new[]
        {
            "server_info",
            "workspace_registry",
            "workspace_open",
            "workspace_close",
            "plugins_list",
            "formlists_list",
            "references_search",
            "formlist_inspect",
            "formlist_compare",
            "workspace_state",
            "metadata_read",
            "save",
            "save_recover",
            "save_repair",
            "output_recovery_resolve",
            "output_select",
            "formlist_begin_edit",
            "formlist_apply_edit",
            "workspace_preview",
            "workspace_discard",
            "output_reopen",
            "formlist_edit_schemas_list",
            "formlist_edit_schema_read",
        });
    }
}
