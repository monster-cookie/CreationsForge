using System.ComponentModel;
using CreationsForge.Mcp.Protocol;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace CreationsForge.Mcp.Tools;

/// <summary>Exposes the generic authoring contract as MCP tools.</summary>
public sealed class McpAuthoringTools
{
    private readonly McpAuthoringService _authoring;

    /// <summary>Initializes the tool wrappers.</summary>
    /// <param name="authoring">The authoring service.</param>
    public McpAuthoringTools(McpAuthoringService authoring)
    {
        ArgumentNullException.ThrowIfNull(authoring);
        _authoring = authoring;
    }

    /// <summary>Returns the production host identity and closed tool names.</summary>
    /// <param name="cancellationToken">The call cancellation token.</param>
    /// <returns>The structured server-info result.</returns>
    [McpServerTool(
        Name = McpAuthoringContract.ServerInfoTool,
        ReadOnly = true,
        UseStructuredContent = true,
        OutputSchemaType = typeof(McpServerInfoResult))]
    [Description("Returns the production host identity, supported releases, and closed authoring tool names.")]
    public Task<CallToolResult> ServerInfo(CancellationToken cancellationToken)
    {
        return _authoring.ServerInfoAsync(cancellationToken);
    }

    /// <summary>Lists editable families for one supported release.</summary>
    /// <param name="release">The game release name.</param>
    /// <param name="cancellationToken">The call cancellation token.</param>
    /// <returns>The structured record-type result.</returns>
    [McpServerTool(
        Name = McpAuthoringContract.RecordTypesTool,
        ReadOnly = true,
        UseStructuredContent = true,
        OutputSchemaType = typeof(McpRecordTypesResult))]
    [Description("Lists editable record families for one supported release.")]
    public Task<CallToolResult> RecordTypes(
        [Description("Game release: Starfield, Fallout4, or SkyrimSE.")] string release,
        CancellationToken cancellationToken)
    {
        return _authoring.RecordTypesAsync(release, cancellationToken);
    }

    /// <summary>Returns one page of a family's registered field schema.</summary>
    /// <param name="release">The game release name.</param>
    /// <param name="familyId">The declared family identifier.</param>
    /// <param name="cancellationToken">The call cancellation token.</param>
    /// <param name="cursor">The schema field cursor, if any.</param>
    /// <returns>The structured schema page.</returns>
    [McpServerTool(
        Name = McpAuthoringContract.RecordSchemaTool,
        ReadOnly = true,
        UseStructuredContent = true,
        OutputSchemaType = typeof(McpRecordSchemaResult))]
    [Description("Returns one page of the registered field schema for a family.")]
    public Task<CallToolResult> RecordSchema(
        [Description("Game release: Starfield, Fallout4, or SkyrimSE.")] string release,
        [Description("Declared family identifier, such as Keyword.")] string familyId,
        CancellationToken cancellationToken,
        [Description("Opaque schema cursor from a previous page.")] string? cursor = null)
    {
        return _authoring.RecordSchemaAsync(release, familyId, cursor, cancellationToken);
    }

    /// <summary>Stores pending game and source configuration.</summary>
    /// <param name="operationId">The client operation identifier.</param>
    /// <param name="release">The game release name.</param>
    /// <param name="dataDirectory">The game data directory.</param>
    /// <param name="selectedPlugins">Selected plugin file names.</param>
    /// <param name="cancellationToken">The call cancellation token.</param>
    /// <returns>The pending session state.</returns>
    [McpServerTool(
        Name = McpAuthoringContract.WorkspaceOpenTool,
        UseStructuredContent = true,
        OutputSchemaType = typeof(McpWorkspaceStateResult))]
    [Description("Stores pending game and source configuration. Does not open a native workspace.")]
    public Task<CallToolResult> WorkspaceOpen(
        [Description("Operation identifier, at most 128 characters. Replayed for this process.")] string operationId,
        [Description("Game release: Starfield, Fallout4, or SkyrimSE.")] string release,
        [Description("Game data directory containing the selected plugins and their masters.")] string dataDirectory,
        [Description("Selected plugin file names, not paths, in low-to-high priority order.")] IReadOnlyList<string> selectedPlugins,
        CancellationToken cancellationToken)
    {
        return _authoring.WorkspaceOpenAsync(operationId, release, dataDirectory, selectedPlugins, cancellationToken);
    }

    /// <summary>Returns pending or native workspace state.</summary>
    /// <param name="workspaceId">The session identifier.</param>
    /// <param name="cancellationToken">The call cancellation token.</param>
    /// <returns>The structured workspace state.</returns>
    [McpServerTool(
        Name = McpAuthoringContract.WorkspaceStateTool,
        ReadOnly = true,
        UseStructuredContent = true,
        OutputSchemaType = typeof(McpWorkspaceStateResult))]
    [Description("Returns the pending or native workspace state.")]
    public Task<CallToolResult> WorkspaceState(
        [Description("Session identifier returned by workspace_open.")] string workspaceId,
        CancellationToken cancellationToken)
    {
        return _authoring.WorkspaceStateAsync(workspaceId, cancellationToken);
    }

    /// <summary>Closes the session.</summary>
    /// <param name="operationId">The client operation identifier.</param>
    /// <param name="workspaceId">The session identifier.</param>
    /// <param name="discardUnsaved">Whether unsaved changes may be discarded.</param>
    /// <param name="cancellationToken">The call cancellation token.</param>
    /// <returns>The close result.</returns>
    [McpServerTool(
        Name = McpAuthoringContract.WorkspaceCloseTool,
        UseStructuredContent = true,
        OutputSchemaType = typeof(McpWorkspaceCloseResult))]
    [Description("Closes the session. Unsaved changes are discarded only when discardUnsaved is true.")]
    public Task<CallToolResult> WorkspaceClose(
        [Description("Operation identifier, at most 128 characters. Replayed for this process.")] string operationId,
        [Description("Session identifier returned by workspace_open.")] string workspaceId,
        [Description("Set true to discard unsaved changes. Otherwise a dirty workspace is rejected.")] bool discardUnsaved,
        CancellationToken cancellationToken)
    {
        return _authoring.WorkspaceCloseAsync(operationId, workspaceId, discardUnsaved, cancellationToken);
    }

    /// <summary>Returns one page of winning records.</summary>
    /// <param name="workspaceId">The session identifier.</param>
    /// <param name="familyId">The declared family identifier.</param>
    /// <param name="cancellationToken">The call cancellation token.</param>
    /// <param name="editorId">The optional EditorID filter.</param>
    /// <param name="cursor">The search cursor, if any.</param>
    /// <returns>The structured search page.</returns>
    [McpServerTool(
        Name = McpAuthoringContract.RecordsSearchTool,
        ReadOnly = true,
        UseStructuredContent = true,
        OutputSchemaType = typeof(McpRecordSearchResult))]
    [Description("Returns one page of winning records for a declared family.")]
    public Task<CallToolResult> RecordsSearch(
        [Description("Session identifier returned by workspace_open.")] string workspaceId,
        [Description("Declared family identifier, such as Keyword.")] string familyId,
        CancellationToken cancellationToken,
        [Description("Optional EditorID filter. Blank means no filter.")] string? editorId = null,
        [Description("Opaque search cursor from a previous page.")] string? cursor = null)
    {
        return _authoring.RecordsSearchAsync(workspaceId, familyId, editorId, cursor, cancellationToken);
    }

    /// <summary>Reads one page of registered fields.</summary>
    /// <param name="workspaceId">The session identifier.</param>
    /// <param name="familyId">The declared family identifier.</param>
    /// <param name="formKey">The origin FormKey text.</param>
    /// <param name="containingModKey">The plugin containing the exact version.</param>
    /// <param name="cancellationToken">The call cancellation token.</param>
    /// <param name="cursor">The field cursor, if any.</param>
    /// <returns>The structured record page.</returns>
    [McpServerTool(
        Name = McpAuthoringContract.RecordReadTool,
        ReadOnly = true,
        UseStructuredContent = true,
        OutputSchemaType = typeof(McpRecordReadResult))]
    [Description("Reads one page of registered fields from an exact record version.")]
    public Task<CallToolResult> RecordRead(
        [Description("Session identifier returned by workspace_open.")] string workspaceId,
        [Description("Declared family identifier, such as Keyword.")] string familyId,
        [Description("Origin FormKey text.")] string formKey,
        [Description("Plugin file name containing the exact version.")] string containingModKey,
        CancellationToken cancellationToken,
        [Description("Opaque field cursor from a previous page.")] string? cursor = null)
    {
        return _authoring.RecordReadAsync(workspaceId, familyId, formKey, containingModKey, cursor, cancellationToken);
    }

    /// <summary>Compares two exact record versions.</summary>
    /// <param name="workspaceId">The session identifier.</param>
    /// <param name="familyId">The declared family identifier.</param>
    /// <param name="leftFormKey">The left origin FormKey text.</param>
    /// <param name="leftContainingModKey">The left containing plugin.</param>
    /// <param name="rightFormKey">The right origin FormKey text.</param>
    /// <param name="rightContainingModKey">The right containing plugin.</param>
    /// <param name="cancellationToken">The call cancellation token.</param>
    /// <param name="cursor">The comparison cursor, if any.</param>
    /// <returns>The structured comparison page.</returns>
    [McpServerTool(
        Name = McpAuthoringContract.RecordCompareTool,
        ReadOnly = true,
        UseStructuredContent = true,
        OutputSchemaType = typeof(McpRecordCompareResult))]
    [Description("Compares registered fields from two exact versions of one family.")]
    public Task<CallToolResult> RecordCompare(
        [Description("Session identifier returned by workspace_open.")] string workspaceId,
        [Description("Declared family identifier, such as Keyword.")] string familyId,
        [Description("Left origin FormKey text.")] string leftFormKey,
        [Description("Left plugin file name.")] string leftContainingModKey,
        [Description("Right origin FormKey text.")] string rightFormKey,
        [Description("Right plugin file name.")] string rightContainingModKey,
        CancellationToken cancellationToken,
        [Description("Opaque comparison cursor from a previous page.")] string? cursor = null)
    {
        return _authoring.RecordCompareAsync(
            workspaceId,
            familyId,
            leftFormKey,
            leftContainingModKey,
            rightFormKey,
            rightContainingModKey,
            cursor,
            cancellationToken);
    }

    /// <summary>Creates a new native output.</summary>
    /// <param name="operationId">The client operation identifier.</param>
    /// <param name="workspaceId">The session identifier.</param>
    /// <param name="outputPath">The output plugin path.</param>
    /// <param name="masterStyle">The requested master style.</param>
    /// <param name="textStorageMode">The requested text storage mode.</param>
    /// <param name="cancellationToken">The call cancellation token.</param>
    /// <param name="language">The active translated-string language.</param>
    /// <returns>The opened session state.</returns>
    [McpServerTool(
        Name = McpAuthoringContract.OutputCreateTool,
        UseStructuredContent = true,
        OutputSchemaType = typeof(McpWorkspaceStateResult))]
    [Description("Creates a new native output and attaches it to the pending session.")]
    public Task<CallToolResult> OutputCreate(
        [Description("Operation identifier, at most 128 characters. Replayed for this process.")] string operationId,
        [Description("Session identifier returned by workspace_open.")] string workspaceId,
        [Description("Output plugin path. The file name must be a plugin name such as Output.esp.")] string outputPath,
        [Description("Master style: Full, Medium, or Small.")] string masterStyle,
        [Description("Text storage: Embedded or Localized.")] string textStorageMode,
        CancellationToken cancellationToken,
        [Description("Optional translated-string language. Defaults to English.")] string? language = null)
    {
        return _authoring.OutputCreateAsync(
            operationId,
            workspaceId,
            outputPath,
            masterStyle,
            textStorageMode,
            language,
            cancellationToken);
    }

    /// <summary>Opens an existing native output.</summary>
    /// <param name="operationId">The client operation identifier.</param>
    /// <param name="workspaceId">The session identifier.</param>
    /// <param name="outputPath">The existing output plugin path.</param>
    /// <param name="masterStyle">The requested master style.</param>
    /// <param name="textStorageMode">The requested text storage mode.</param>
    /// <param name="cancellationToken">The call cancellation token.</param>
    /// <param name="language">The active translated-string language.</param>
    /// <returns>The opened session state.</returns>
    [McpServerTool(
        Name = McpAuthoringContract.OutputOpenTool,
        UseStructuredContent = true,
        OutputSchemaType = typeof(McpWorkspaceStateResult))]
    [Description("Opens an existing native output and attaches it to the pending session.")]
    public Task<CallToolResult> OutputOpen(
        [Description("Operation identifier, at most 128 characters. Replayed for this process.")] string operationId,
        [Description("Session identifier returned by workspace_open.")] string workspaceId,
        [Description("Existing output plugin path. The file name must match the plugin identity.")] string outputPath,
        [Description("Master style: Full, Medium, or Small.")] string masterStyle,
        [Description("Text storage: Embedded or Localized.")] string textStorageMode,
        CancellationToken cancellationToken,
        [Description("Optional translated-string language. Defaults to English.")] string? language = null)
    {
        return _authoring.OutputOpenAsync(
            operationId,
            workspaceId,
            outputPath,
            masterStyle,
            textStorageMode,
            language,
            cancellationToken);
    }

    /// <summary>Creates one record.</summary>
    /// <param name="operationId">The client operation identifier.</param>
    /// <param name="workspaceId">The session identifier.</param>
    /// <param name="familyId">The declared family identifier.</param>
    /// <param name="expectedRevision">The revision the change is based on.</param>
    /// <param name="changes">The initial field changes.</param>
    /// <param name="cancellationToken">The call cancellation token.</param>
    /// <returns>The published snapshot.</returns>
    [McpServerTool(
        Name = McpAuthoringContract.RecordCreateTool,
        UseStructuredContent = true,
        OutputSchemaType = typeof(McpRecordApplyResult))]
    [Description("Creates one record and publishes it with the expected workspace revision.")]
    public Task<CallToolResult> RecordCreate(
        [Description("Operation identifier, at most 128 characters. Replayed for this process.")] string operationId,
        [Description("Session identifier returned by workspace_open.")] string workspaceId,
        [Description("Declared family identifier, such as Keyword.")] string familyId,
        [Description("Exact workspace revision this create is based on.")] ulong? expectedRevision,
        [Description("Ordered field changes. Use Set for scalar fields.")] IReadOnlyList<McpFieldChangeDto> changes,
        CancellationToken cancellationToken)
    {
        return _authoring.RecordCreateAsync(
            operationId,
            workspaceId,
            familyId,
            expectedRevision,
            changes,
            cancellationToken);
    }

    /// <summary>Overrides one exact record context.</summary>
    /// <param name="operationId">The client operation identifier.</param>
    /// <param name="workspaceId">The session identifier.</param>
    /// <param name="familyId">The declared family identifier.</param>
    /// <param name="formKey">The origin FormKey text.</param>
    /// <param name="containingModKey">The exact containing plugin.</param>
    /// <param name="expectedRevision">The revision the change is based on.</param>
    /// <param name="changes">The field changes.</param>
    /// <param name="cancellationToken">The call cancellation token.</param>
    /// <returns>The published snapshot.</returns>
    [McpServerTool(
        Name = McpAuthoringContract.RecordOverrideTool,
        UseStructuredContent = true,
        OutputSchemaType = typeof(McpRecordApplyResult))]
    [Description("Overrides one exact record context and publishes it with the expected workspace revision.")]
    public Task<CallToolResult> RecordOverride(
        [Description("Operation identifier, at most 128 characters. Replayed for this process.")] string operationId,
        [Description("Session identifier returned by workspace_open.")] string workspaceId,
        [Description("Declared family identifier, such as Keyword.")] string familyId,
        [Description("Origin FormKey text.")] string formKey,
        [Description("Plugin file name containing the exact version to override.")] string containingModKey,
        [Description("Exact workspace revision this override is based on.")] ulong? expectedRevision,
        [Description("Ordered field changes.")] IReadOnlyList<McpFieldChangeDto> changes,
        CancellationToken cancellationToken)
    {
        return _authoring.RecordOverrideAsync(
            operationId,
            workspaceId,
            familyId,
            formKey,
            containingModKey,
            expectedRevision,
            changes,
            cancellationToken);
    }

    /// <summary>Publishes a batch of mutations.</summary>
    /// <param name="operationId">The client operation identifier.</param>
    /// <param name="workspaceId">The session identifier.</param>
    /// <param name="expectedRevision">The revision the batch is based on.</param>
    /// <param name="mutations">The ordered mutations.</param>
    /// <param name="cancellationToken">The call cancellation token.</param>
    /// <returns>The published snapshots.</returns>
    [McpServerTool(
        Name = McpAuthoringContract.RecordApplyTool,
        UseStructuredContent = true,
        OutputSchemaType = typeof(McpRecordApplyResult))]
    [Description("Publishes an ordered batch of create and override mutations as one revision.")]
    public Task<CallToolResult> RecordApply(
        [Description("Operation identifier, at most 128 characters. Replayed for this process.")] string operationId,
        [Description("Session identifier returned by workspace_open.")] string workspaceId,
        [Description("Exact workspace revision this batch is based on.")] ulong? expectedRevision,
        [Description("Ordered create and override mutations.")] IReadOnlyList<McpMutationDto> mutations,
        CancellationToken cancellationToken)
    {
        return _authoring.RecordApplyAsync(operationId, workspaceId, expectedRevision, mutations, cancellationToken);
    }

    /// <summary>Returns one page of unsaved registered changes.</summary>
    /// <param name="workspaceId">The session identifier.</param>
    /// <param name="cancellationToken">The call cancellation token.</param>
    /// <param name="cursor">The preview cursor, if any.</param>
    /// <returns>The structured preview page.</returns>
    [McpServerTool(
        Name = McpAuthoringContract.WorkspacePreviewTool,
        ReadOnly = true,
        UseStructuredContent = true,
        OutputSchemaType = typeof(McpWorkspacePreviewResult))]
    [Description("Returns one page of unsaved registered changes.")]
    public Task<CallToolResult> WorkspacePreview(
        [Description("Session identifier returned by workspace_open.")] string workspaceId,
        CancellationToken cancellationToken,
        [Description("Opaque preview or preview-fields cursor from a previous page.")] string? cursor = null)
    {
        return _authoring.WorkspacePreviewAsync(workspaceId, cursor, cancellationToken);
    }

    /// <summary>Saves and publishes the native output.</summary>
    /// <param name="operationId">The client operation identifier.</param>
    /// <param name="workspaceId">The session identifier.</param>
    /// <param name="cancellationToken">The call cancellation token.</param>
    /// <returns>The structured save outcome.</returns>
    [McpServerTool(
        Name = McpAuthoringContract.WorkspaceSaveTool,
        UseStructuredContent = true,
        OutputSchemaType = typeof(McpWorkspaceSaveResult))]
    [Description("Saves and publishes the native output.")]
    public Task<CallToolResult> WorkspaceSave(
        [Description("Operation identifier, at most 128 characters. Replayed for this process.")] string operationId,
        [Description("Session identifier returned by workspace_open.")] string workspaceId,
        CancellationToken cancellationToken)
    {
        return _authoring.WorkspaceSaveAsync(operationId, workspaceId, cancellationToken);
    }

    /// <summary>Restores the last native saved baseline.</summary>
    /// <param name="operationId">The client operation identifier.</param>
    /// <param name="workspaceId">The session identifier.</param>
    /// <param name="cancellationToken">The call cancellation token.</param>
    /// <returns>The discard result.</returns>
    [McpServerTool(
        Name = McpAuthoringContract.WorkspaceDiscardTool,
        UseStructuredContent = true,
        OutputSchemaType = typeof(McpWorkspaceDiscardResult))]
    [Description("Restores the last native saved baseline without writing destination files.")]
    public Task<CallToolResult> WorkspaceDiscard(
        [Description("Operation identifier, at most 128 characters. Replayed for this process.")] string operationId,
        [Description("Session identifier returned by workspace_open.")] string workspaceId,
        CancellationToken cancellationToken)
    {
        return _authoring.WorkspaceDiscardAsync(operationId, workspaceId, cancellationToken);
    }
}
