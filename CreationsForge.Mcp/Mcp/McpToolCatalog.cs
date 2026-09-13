using ModelContextProtocol.Server;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.RecordWire;

namespace CreationsForge.Mcp;

/// <summary>
/// Constructs the explicit bounded MCP tool set for one host process.
/// </summary>
public sealed class McpToolCatalog
{
    /// <summary>Creates tools bound to the supplied host-owned state.</summary>
    /// <param name="workspaceRegistry">The registry shared by lifecycle-aware tools.</param>
    /// <param name="serverVersion">The server informational version reported by discovery tools.</param>
    /// <param name="workspaceFactory">The actual engine factory that enables engine domain tools, or <see langword="null"/> when production adapters are not composed.</param>
    /// <returns>An immutable tool list suitable for <c>WithTools</c> registration.</returns>
    public IReadOnlyList<McpServerTool> CreateTools(
        McpWorkspaceRegistry workspaceRegistry,
        string serverVersion,
        IFormListWorkspaceFactory? workspaceFactory = null)
    {
        ArgumentNullException.ThrowIfNull(workspaceRegistry);
        ArgumentException.ThrowIfNullOrWhiteSpace(serverVersion);

        var tools = new List<McpServerTool>
        {
            new ServerInfoTool(workspaceRegistry, serverVersion, workspaceFactory is not null),
        };
        if (workspaceFactory is not null)
        {
            tools.Add(new WorkspaceOpenTool(workspaceFactory, workspaceRegistry));
            tools.Add(new WorkspaceCloseTool(workspaceRegistry));
            tools.Add(new PluginsListTool(workspaceRegistry));
            tools.Add(new FormListsListTool(workspaceRegistry));
            tools.Add(new ReferencesSearchTool(workspaceRegistry));
            tools.Add(new FormListInspectTool(workspaceRegistry));
            tools.Add(new FormListCompareTool(workspaceRegistry));
        }

        return Array.AsReadOnly(tools.ToArray());
    }

    /// <summary>Creates the complete production tool catalog when every authoring dependency is composed.</summary>
    /// <param name="workspaceRegistry">The registry shared by lifecycle-aware tools.</param>
    /// <param name="serverVersion">The server informational version.</param>
    /// <param name="workspaceFactory">The complete workspace factory.</param>
    /// <param name="saveCoordinator">The guarded save and recovery coordinator.</param>
    /// <param name="codecs">The exact per-game typed edit codecs.</param>
    /// <param name="schemaCatalogs">The exact immutable per-game schema catalogs.</param>
    /// <param name="metadataStore">The host-scoped opaque metadata store and operation ledger.</param>
    /// <returns>The complete immutable production tool list.</returns>
    internal IReadOnlyList<McpServerTool> CreateTools(
        McpWorkspaceRegistry workspaceRegistry,
        string serverVersion,
        IFormListWorkspaceFactory workspaceFactory,
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

        return Array.AsReadOnly<McpServerTool>(
        [
            new ServerInfoTool(workspaceRegistry, serverVersion, true, true),
            new WorkspaceOpenTool(workspaceFactory, workspaceRegistry),
            new WorkspaceCloseTool(workspaceRegistry),
            new PluginsListTool(workspaceRegistry),
            new FormListsListTool(workspaceRegistry),
            new ReferencesSearchTool(workspaceRegistry),
            new FormListInspectTool(workspaceRegistry),
            new FormListCompareTool(workspaceRegistry),
            new WorkspaceStateTool(workspaceRegistry, metadataStore),
            new MetadataReadTool(metadataStore),
            new SaveTool(workspaceRegistry, metadataStore),
            new SaveRecoverTool(saveCoordinator, metadataStore),
            new SaveRepairTool(saveCoordinator, metadataStore),
            new OutputRecoveryResolveTool(workspaceRegistry, metadataStore),
            new OutputSelectTool(workspaceRegistry, metadataStore),
            new FormListBeginEditTool(workspaceRegistry, metadataStore),
            new FormListApplyEditTool(workspaceRegistry, codecs, metadataStore),
            new WorkspacePreviewTool(workspaceRegistry),
            new OutputResetTool(workspaceRegistry, metadataStore, false),
            new OutputResetTool(workspaceRegistry, metadataStore, true),
            new FormListEditSchemasListTool(schemaCatalogs),
            new FormListEditSchemaReadTool(schemaCatalogs),
        ]);
    }
}
