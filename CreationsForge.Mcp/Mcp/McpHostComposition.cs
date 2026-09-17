using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.RecordWire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace CreationsForge.Mcp;

/// <summary>
/// Creates the isolated generic-host composition used by the stdio MCP route.
/// </summary>
public static class McpHostComposition
{
    /// <summary>Creates the complete production MCP builder with authoring, save, recovery, metadata, and schema tools.</summary>
    /// <param name="workspaceRegistry">The host-owned workspace registry.</param>
    /// <param name="serverVersion">The informational server version.</param>
    /// <param name="workspaceFactory">The complete workspace factory.</param>
    /// <param name="saveCoordinator">The guarded save and recovery coordinator.</param>
    /// <param name="codecs">The exact per-game typed edit codecs.</param>
    /// <param name="schemaCatalogs">The immutable per-game schema catalogs.</param>
    /// <param name="metadataStore">The host-scoped metadata store and operation ledger.</param>
    /// <returns>A configured production host builder.</returns>
    internal static HostApplicationBuilder CreateProductionBuilder(
        McpWorkspaceRegistry workspaceRegistry,
        string serverVersion,
        IPluginWorkspaceFactory workspaceFactory,
        IWorkspaceSaveCoordinator saveCoordinator,
        IReadOnlyList<IFormListEditWireCodec> codecs,
        IReadOnlyList<IFormListEditWireSchemaCatalog> schemaCatalogs,
        McpMetadataStore metadataStore)
    {
        ArgumentNullException.ThrowIfNull(workspaceRegistry);
        ArgumentException.ThrowIfNullOrWhiteSpace(serverVersion);
        ArgumentNullException.ThrowIfNull(workspaceFactory);
        ArgumentNullException.ThrowIfNull(saveCoordinator);
        ArgumentNullException.ThrowIfNull(codecs);
        ArgumentNullException.ThrowIfNull(schemaCatalogs);
        ArgumentNullException.ThrowIfNull(metadataStore);
        var tools = new McpToolCatalog().CreateTools(
            workspaceRegistry,
            serverVersion,
            workspaceFactory,
            saveCoordinator,
            codecs,
            schemaCatalogs,
            metadataStore);
        return CreateBuilder(workspaceRegistry, tools, serverVersion);
    }

    /// <summary>Creates an MCP host builder whose domain tools are enabled only by an actual workspace factory.</summary>
    /// <param name="workspaceRegistry">The registry shared by workspace tools.</param>
    /// <param name="serverVersion">The informational version advertised during initialization.</param>
    /// <param name="workspaceFactory">The real workspace factory, or <see langword="null"/> while game adapters are unavailable.</param>
    /// <param name="configureServices">An optional composition hook for other engine infrastructure.</param>
    /// <returns>A configured builder with an honest tool catalog for its supplied capabilities.</returns>
    public static HostApplicationBuilder CreateBuilder(
        McpWorkspaceRegistry workspaceRegistry,
        string serverVersion,
        IPluginWorkspaceFactory? workspaceFactory,
        Action<IServiceCollection>? configureServices = null)
    {
        ArgumentNullException.ThrowIfNull(workspaceRegistry);
        ArgumentException.ThrowIfNullOrWhiteSpace(serverVersion);
        var tools = new McpToolCatalog().CreateTools(workspaceRegistry, serverVersion, workspaceFactory);
        return CreateBuilder(
            workspaceRegistry,
            tools,
            serverVersion,
            services =>
            {
                if (workspaceFactory is not null)
                {
                    services.AddSingleton(workspaceFactory);
                }

                configureServices?.Invoke(services);
            });
    }

    /// <summary>Creates an MCP host builder without invoking legacy CreationsForge bootstrap services.</summary>
    /// <param name="workspaceRegistry">The registry shared by current and future workspace tools.</param>
    /// <param name="tools">The explicit tool catalog for this host.</param>
    /// <param name="serverVersion">The informational version advertised during initialization.</param>
    /// <param name="configureServices">An optional composition hook for engine adapters and engine infrastructure.</param>
    /// <returns>A configured builder whose protocol output is reserved for stdio transport.</returns>
    /// <exception cref="ArgumentNullException">Thrown when a required argument is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="serverVersion"/> is empty.</exception>
    public static HostApplicationBuilder CreateBuilder(
        McpWorkspaceRegistry workspaceRegistry,
        IReadOnlyList<McpServerTool> tools,
        string serverVersion,
        Action<IServiceCollection>? configureServices = null)
    {
        ArgumentNullException.ThrowIfNull(workspaceRegistry);
        ArgumentNullException.ThrowIfNull(tools);
        ArgumentException.ThrowIfNullOrWhiteSpace(serverVersion);

        var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
        {
            Args = [],
            ApplicationName = typeof(McpHostComposition).Assembly.GetName().Name,
        });
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole(options => options.LogToStandardErrorThreshold = LogLevel.Trace);
        builder.Services.AddSingleton(workspaceRegistry);
        configureServices?.Invoke(builder.Services);
        builder.Services
            .AddMcpServer(options =>
            {
                options.ServerInfo = new Implementation
                {
                    Name = "CreationsForge",
                    Version = serverVersion,
                };
                options.ServerInstructions = "Use explicit CreationsForge domain tools. Every tool uses a closed input schema and returns a structured ok/result or ok/error envelope.";
            })
            .WithStdioServerTransport()
            .WithTools(tools);
        return builder;
    }
}
