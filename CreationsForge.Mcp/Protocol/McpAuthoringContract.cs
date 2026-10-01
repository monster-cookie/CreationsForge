using CreationsForge.Engine.Workspaces;

namespace CreationsForge.Mcp.Protocol;

/// <summary>Names the production authoring tools and the closed page size shared by every cursor.</summary>
public static class McpAuthoringContract
{
    /// <summary>Returns host identity and the closed tool names.</summary>
    public const string ServerInfoTool = "server_info";

    /// <summary>Lists editable record families for one supported release.</summary>
    public const string RecordTypesTool = "record_types";

    /// <summary>Returns one page of the registered field schema for a family.</summary>
    public const string RecordSchemaTool = "record_schema";

    /// <summary>Stores pending game and source configuration without opening a native workspace.</summary>
    public const string WorkspaceOpenTool = "workspace_open";

    /// <summary>Returns the pending or native workspace state.</summary>
    public const string WorkspaceStateTool = "workspace_state";

    /// <summary>Closes the session, discarding unsaved changes only when explicitly requested.</summary>
    public const string WorkspaceCloseTool = "workspace_close";

    /// <summary>Returns one page of winning records for a declared family.</summary>
    public const string RecordsSearchTool = "records_search";

    /// <summary>Reads one page of registered fields from an exact record version.</summary>
    public const string RecordReadTool = "record_read";

    /// <summary>Compares registered fields from two exact versions of one family.</summary>
    public const string RecordCompareTool = "record_compare";

    /// <summary>Creates a new native output and attaches it to the pending session.</summary>
    public const string OutputCreateTool = "output_create";

    /// <summary>Opens an existing native output and attaches it to the pending session.</summary>
    public const string OutputOpenTool = "output_open";

    /// <summary>Creates one record and publishes it with the current workspace revision.</summary>
    public const string RecordCreateTool = "record_create";

    /// <summary>Overrides one exact record context and publishes it with the current workspace revision.</summary>
    public const string RecordOverrideTool = "record_override";

    /// <summary>Publishes an ordered batch of create and override mutations as one revision.</summary>
    public const string RecordApplyTool = "record_apply";

    /// <summary>Returns one page of unsaved registered changes.</summary>
    public const string WorkspacePreviewTool = "workspace_preview";

    /// <summary>Saves and publishes the native output.</summary>
    public const string WorkspaceSaveTool = "workspace_save";

    /// <summary>Restores the last native saved baseline without writing destination files.</summary>
    public const string WorkspaceDiscardTool = "workspace_discard";

    /// <summary>The maximum number of records or fields returned by one cursor page.</summary>
    public const int PageSize = PluginRecordSearchPage.MaximumTake;

    /// <summary>Gets the tool names in stable registration order.</summary>
    public static IReadOnlyList<string> ToolNames { get; } =
    [
        ServerInfoTool,
        RecordTypesTool,
        RecordSchemaTool,
        WorkspaceOpenTool,
        WorkspaceStateTool,
        WorkspaceCloseTool,
        RecordsSearchTool,
        RecordReadTool,
        RecordCompareTool,
        OutputCreateTool,
        OutputOpenTool,
        RecordCreateTool,
        RecordOverrideTool,
        RecordApplyTool,
        WorkspacePreviewTool,
        WorkspaceSaveTool,
        WorkspaceDiscardTool,
    ];
}
