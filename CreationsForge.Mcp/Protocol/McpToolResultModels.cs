namespace CreationsForge.Mcp.Protocol;

/// <summary>Returns the production host identity and closed tool names.</summary>
public sealed class McpServerInfoResult
{
    /// <summary>Initializes a server-info result.</summary>
    /// <param name="name">The MCP server name.</param>
    /// <param name="version">The host version advertised during initialization.</param>
    /// <param name="releases">Supported game releases.</param>
    /// <param name="tools">Authoring tool names in registration order.</param>
    public McpServerInfoResult(string name, string version, IReadOnlyList<string> releases, IReadOnlyList<string> tools)
    {
        Name = name;
        Version = version;
        Releases = releases;
        Tools = tools;
    }

    /// <summary>Gets the MCP server name.</summary>
    public string Name { get; }

    /// <summary>Gets the host version.</summary>
    public string Version { get; }

    /// <summary>Gets the supported game releases.</summary>
    public IReadOnlyList<string> Releases { get; }

    /// <summary>Gets the authoring tool names.</summary>
    public IReadOnlyList<string> Tools { get; }
}

/// <summary>Lists the editable families for one release.</summary>
public sealed class McpRecordTypesResult
{
    /// <summary>Initializes a record-type result.</summary>
    /// <param name="release">The game release.</param>
    /// <param name="families">Declared family identifiers.</param>
    public McpRecordTypesResult(string release, IReadOnlyList<string> families)
    {
        Release = release;
        Families = families;
    }

    /// <summary>Gets the game release.</summary>
    public string Release { get; }

    /// <summary>Gets the declared family identifiers.</summary>
    public IReadOnlyList<string> Families { get; }
}

/// <summary>Returns one page of a family's registered field schema.</summary>
public sealed class McpRecordSchemaResult
{
    /// <summary>Initializes a schema page.</summary>
    /// <param name="release">The game release.</param>
    /// <param name="familyId">The declared family identifier.</param>
    /// <param name="schemaVersion">The family schema version.</param>
    /// <param name="fields">The field page.</param>
    /// <param name="nextCursor">The cursor for the next field page, if any.</param>
    public McpRecordSchemaResult(
        string release,
        string familyId,
        int schemaVersion,
        IReadOnlyList<McpSchemaFieldResult> fields,
        string? nextCursor)
    {
        Release = release;
        FamilyId = familyId;
        SchemaVersion = schemaVersion;
        Fields = fields;
        NextCursor = nextCursor;
    }

    /// <summary>Gets the game release.</summary>
    public string Release { get; }

    /// <summary>Gets the declared family identifier.</summary>
    public string FamilyId { get; }

    /// <summary>Gets the family schema version.</summary>
    public int SchemaVersion { get; }

    /// <summary>Gets the field page.</summary>
    public IReadOnlyList<McpSchemaFieldResult> Fields { get; }

    /// <summary>Gets the cursor for the next field page, if any.</summary>
    public string? NextCursor { get; }
}

/// <summary>Describes one registered editable field.</summary>
public sealed class McpSchemaFieldResult
{
    /// <summary>Initializes a schema field.</summary>
    /// <param name="path">The registered field path.</param>
    /// <param name="kind">The closed value kind.</param>
    /// <param name="isNullable">Whether null is legal.</param>
    /// <param name="isFlags">Whether enum choices may be combined.</param>
    /// <param name="choices">Legal enum names.</param>
    /// <param name="operations">Legal operation names.</param>
    /// <param name="alternatives">Legal object alternatives.</param>
    /// <param name="referenceTargets">Legal reference target type names.</param>
    public McpSchemaFieldResult(
        string path,
        string kind,
        bool isNullable,
        bool isFlags,
        IReadOnlyList<string> choices,
        IReadOnlyList<string> operations,
        IReadOnlyList<string> alternatives,
        IReadOnlyList<string> referenceTargets)
    {
        Path = path;
        Kind = kind;
        IsNullable = isNullable;
        IsFlags = isFlags;
        Choices = choices;
        Operations = operations;
        Alternatives = alternatives;
        ReferenceTargets = referenceTargets;
    }

    /// <summary>Gets the registered field path.</summary>
    public string Path { get; }

    /// <summary>Gets the closed value kind.</summary>
    public string Kind { get; }

    /// <summary>Gets whether null is legal.</summary>
    public bool IsNullable { get; }

    /// <summary>Gets whether enum choices may be combined.</summary>
    public bool IsFlags { get; }

    /// <summary>Gets the legal enum names.</summary>
    public IReadOnlyList<string> Choices { get; }

    /// <summary>Gets the legal operation names.</summary>
    public IReadOnlyList<string> Operations { get; }

    /// <summary>Gets the legal object alternatives.</summary>
    public IReadOnlyList<string> Alternatives { get; }

    /// <summary>Gets the legal reference target type names.</summary>
    public IReadOnlyList<string> ReferenceTargets { get; }
}

/// <summary>Returns pending or native workspace state.</summary>
public sealed class McpWorkspaceStateResult
{
    /// <summary>Initializes a workspace state result.</summary>
    /// <param name="workspaceId">The session identifier.</param>
    /// <param name="workspaceOpen">Whether a native workspace is attached.</param>
    /// <param name="release">The selected game release.</param>
    /// <param name="dataDirectory">The selected data directory.</param>
    /// <param name="selectedPlugins">Selected plugin file names.</param>
    /// <param name="outputPath">The output path, when a native workspace is open.</param>
    /// <param name="outputModKey">The output identity, when a native workspace is open.</param>
    /// <param name="masterStyle">The output master style, when a native workspace is open.</param>
    /// <param name="textStorageMode">The output text storage mode, when a native workspace is open.</param>
    /// <param name="isNewOutput">Whether the output was created by this session.</param>
    /// <param name="isDirty">Whether the native workspace has unsaved changes.</param>
    /// <param name="revision">The native workspace revision.</param>
    /// <param name="requiresReopen">Whether the native workspace must be reopened before further edits.</param>
    public McpWorkspaceStateResult(
        string workspaceId,
        bool workspaceOpen,
        string release,
        string dataDirectory,
        IReadOnlyList<string> selectedPlugins,
        string? outputPath,
        string? outputModKey,
        string? masterStyle,
        string? textStorageMode,
        bool isNewOutput,
        bool isDirty,
        ulong revision,
        bool requiresReopen)
    {
        WorkspaceId = workspaceId;
        WorkspaceOpen = workspaceOpen;
        Release = release;
        DataDirectory = dataDirectory;
        SelectedPlugins = selectedPlugins;
        OutputPath = outputPath;
        OutputModKey = outputModKey;
        MasterStyle = masterStyle;
        TextStorageMode = textStorageMode;
        IsNewOutput = isNewOutput;
        IsDirty = isDirty;
        Revision = revision;
        RequiresReopen = requiresReopen;
    }

    /// <summary>Gets the session identifier.</summary>
    public string WorkspaceId { get; }

    /// <summary>Gets whether a native workspace is attached.</summary>
    public bool WorkspaceOpen { get; }

    /// <summary>Gets the selected game release.</summary>
    public string Release { get; }

    /// <summary>Gets the selected data directory.</summary>
    public string DataDirectory { get; }

    /// <summary>Gets the selected plugin file names.</summary>
    public IReadOnlyList<string> SelectedPlugins { get; }

    /// <summary>Gets the output path, when a native workspace is open.</summary>
    public string? OutputPath { get; }

    /// <summary>Gets the output identity, when a native workspace is open.</summary>
    public string? OutputModKey { get; }

    /// <summary>Gets the output master style, when a native workspace is open.</summary>
    public string? MasterStyle { get; }

    /// <summary>Gets the output text storage mode, when a native workspace is open.</summary>
    public string? TextStorageMode { get; }

    /// <summary>Gets whether the output was created by this session.</summary>
    public bool IsNewOutput { get; }

    /// <summary>Gets whether the native workspace has unsaved changes.</summary>
    public bool IsDirty { get; }

    /// <summary>Gets the native workspace revision, or zero when no native workspace is open.</summary>
    public ulong Revision { get; }

    /// <summary>Gets whether the native workspace must be reopened before further edits.</summary>
    public bool RequiresReopen { get; }
}

/// <summary>Reports that a session was closed.</summary>
public sealed class McpWorkspaceCloseResult
{
    /// <summary>Initializes a close result.</summary>
    /// <param name="workspaceId">The closed session identifier.</param>
    /// <param name="closed">Whether the session was removed.</param>
    /// <param name="revision">The revision observed before dispose, or zero when no native workspace was open.</param>
    public McpWorkspaceCloseResult(string workspaceId, bool closed, ulong revision)
    {
        WorkspaceId = workspaceId;
        Closed = closed;
        Revision = revision;
    }

    /// <summary>Gets the closed session identifier.</summary>
    public string WorkspaceId { get; }

    /// <summary>Gets whether the session was removed.</summary>
    public bool Closed { get; }

    /// <summary>Gets the revision observed before dispose.</summary>
    public ulong Revision { get; }
}

/// <summary>Returns one page of winning record identities.</summary>
public sealed class McpRecordSearchResult
{
    /// <summary>Initializes a search page.</summary>
    /// <param name="workspaceId">The session identifier.</param>
    /// <param name="revision">The workspace revision captured with the page.</param>
    /// <param name="familyId">The declared family identifier.</param>
    /// <param name="records">The record page.</param>
    /// <param name="nextCursor">The cursor for the next page, if any.</param>
    public McpRecordSearchResult(
        string workspaceId,
        ulong revision,
        string familyId,
        IReadOnlyList<McpRecordIdentityResult> records,
        string? nextCursor)
    {
        WorkspaceId = workspaceId;
        Revision = revision;
        FamilyId = familyId;
        Records = records;
        NextCursor = nextCursor;
    }

    /// <summary>Gets the session identifier.</summary>
    public string WorkspaceId { get; }

    /// <summary>Gets the workspace revision captured with the page.</summary>
    public ulong Revision { get; }

    /// <summary>Gets the declared family identifier.</summary>
    public string FamilyId { get; }

    /// <summary>Gets the record page.</summary>
    public IReadOnlyList<McpRecordIdentityResult> Records { get; }

    /// <summary>Gets the cursor for the next page, if any.</summary>
    public string? NextCursor { get; }
}

/// <summary>Identifies one winning or exact record version.</summary>
public sealed class McpRecordIdentityResult
{
    /// <summary>Initializes a record identity.</summary>
    /// <param name="formKey">The origin FormKey text.</param>
    /// <param name="containingModKey">The plugin containing this version.</param>
    /// <param name="winningModKey">The plugin that wins for the origin identity.</param>
    /// <param name="editorId">The EditorID, when present.</param>
    public McpRecordIdentityResult(string formKey, string containingModKey, string winningModKey, string? editorId)
    {
        FormKey = formKey;
        ContainingModKey = containingModKey;
        WinningModKey = winningModKey;
        EditorId = editorId;
    }

    /// <summary>Gets the origin FormKey text.</summary>
    public string FormKey { get; }

    /// <summary>Gets the plugin containing this version.</summary>
    public string ContainingModKey { get; }

    /// <summary>Gets the plugin that wins for the origin identity.</summary>
    public string WinningModKey { get; }

    /// <summary>Gets the EditorID, when present.</summary>
    public string? EditorId { get; }
}

/// <summary>Returns one page of registered fields from an exact record version.</summary>
public sealed class McpRecordReadResult
{
    /// <summary>Initializes a record read page.</summary>
    /// <param name="workspaceId">The session identifier.</param>
    /// <param name="revision">The workspace revision captured with the read.</param>
    /// <param name="record">The record identity and field page.</param>
    public McpRecordReadResult(string workspaceId, ulong revision, McpRecordSnapshotResult record)
    {
        WorkspaceId = workspaceId;
        Revision = revision;
        Record = record;
    }

    /// <summary>Gets the session identifier.</summary>
    public string WorkspaceId { get; }

    /// <summary>Gets the workspace revision captured with the read.</summary>
    public ulong Revision { get; }

    /// <summary>Gets the record identity and field page.</summary>
    public McpRecordSnapshotResult Record { get; }
}

/// <summary>Projects one exact record version and a page of its registered fields.</summary>
public sealed class McpRecordSnapshotResult
{
    /// <summary>Initializes a record snapshot page.</summary>
    /// <param name="familyId">The declared family identifier.</param>
    /// <param name="formKey">The origin FormKey text.</param>
    /// <param name="containingModKey">The plugin containing this version.</param>
    /// <param name="fields">The field page.</param>
    /// <param name="nextFieldCursor">The cursor for the next field page, if any.</param>
    public McpRecordSnapshotResult(
        string familyId,
        string formKey,
        string containingModKey,
        IReadOnlyList<McpFieldValueResult> fields,
        string? nextFieldCursor)
    {
        FamilyId = familyId;
        FormKey = formKey;
        ContainingModKey = containingModKey;
        Fields = fields;
        NextFieldCursor = nextFieldCursor;
    }

    /// <summary>Gets the declared family identifier.</summary>
    public string FamilyId { get; }

    /// <summary>Gets the origin FormKey text.</summary>
    public string FormKey { get; }

    /// <summary>Gets the plugin containing this version.</summary>
    public string ContainingModKey { get; }

    /// <summary>Gets the field page.</summary>
    public IReadOnlyList<McpFieldValueResult> Fields { get; }

    /// <summary>Gets the cursor for the next field page, if any.</summary>
    public string? NextFieldCursor { get; }
}

/// <summary>Transports one registered field value.</summary>
public sealed class McpFieldValueResult
{
    /// <summary>Initializes a field value.</summary>
    /// <param name="path">The registered field path.</param>
    /// <param name="value">The closed field value.</param>
    public McpFieldValueResult(string path, McpRecordValueDto value)
    {
        Path = path;
        Value = value;
    }

    /// <summary>Gets the registered field path.</summary>
    public string Path { get; }

    /// <summary>Gets the closed field value.</summary>
    public McpRecordValueDto Value { get; }
}

/// <summary>Returns one page of differences between two exact record versions.</summary>
public sealed class McpRecordCompareResult
{
    /// <summary>Initializes a comparison page.</summary>
    /// <param name="workspaceId">The session identifier.</param>
    /// <param name="revision">The workspace revision captured with the comparison.</param>
    /// <param name="familyId">The declared family identifier.</param>
    /// <param name="differences">The difference page.</param>
    /// <param name="nextCursor">The cursor for the next difference page, if any.</param>
    public McpRecordCompareResult(
        string workspaceId,
        ulong revision,
        string familyId,
        IReadOnlyList<McpRecordDifferenceResult> differences,
        string? nextCursor)
    {
        WorkspaceId = workspaceId;
        Revision = revision;
        FamilyId = familyId;
        Differences = differences;
        NextCursor = nextCursor;
    }

    /// <summary>Gets the session identifier.</summary>
    public string WorkspaceId { get; }

    /// <summary>Gets the workspace revision captured with the comparison.</summary>
    public ulong Revision { get; }

    /// <summary>Gets the declared family identifier.</summary>
    public string FamilyId { get; }

    /// <summary>Gets the difference page.</summary>
    public IReadOnlyList<McpRecordDifferenceResult> Differences { get; }

    /// <summary>Gets the cursor for the next difference page, if any.</summary>
    public string? NextCursor { get; }
}

/// <summary>Projects one registered field difference.</summary>
public sealed class McpRecordDifferenceResult
{
    /// <summary>Initializes a field difference.</summary>
    /// <param name="path">The registered field path.</param>
    /// <param name="left">The left value.</param>
    /// <param name="right">The right value.</param>
    public McpRecordDifferenceResult(string path, McpRecordValueDto left, McpRecordValueDto right)
    {
        Path = path;
        Left = left;
        Right = right;
    }

    /// <summary>Gets the registered field path.</summary>
    public string Path { get; }

    /// <summary>Gets the left value.</summary>
    public McpRecordValueDto Left { get; }

    /// <summary>Gets the right value.</summary>
    public McpRecordValueDto Right { get; }
}

/// <summary>Returns the revision and published snapshots from one atomic apply.</summary>
public sealed class McpRecordApplyResult
{
    /// <summary>Initializes an apply result.</summary>
    /// <param name="workspaceId">The session identifier.</param>
    /// <param name="revision">The revision after publication.</param>
    /// <param name="records">The published output snapshots.</param>
    public McpRecordApplyResult(string workspaceId, ulong revision, IReadOnlyList<McpRecordSnapshotResult> records)
    {
        WorkspaceId = workspaceId;
        Revision = revision;
        Records = records;
    }

    /// <summary>Gets the session identifier.</summary>
    public string WorkspaceId { get; }

    /// <summary>Gets the revision after publication.</summary>
    public ulong Revision { get; }

    /// <summary>Gets the published output snapshots.</summary>
    public IReadOnlyList<McpRecordSnapshotResult> Records { get; }
}

/// <summary>Returns one page of unsaved registered changes.</summary>
public sealed class McpWorkspacePreviewResult
{
    /// <summary>Initializes a preview page.</summary>
    /// <param name="workspaceId">The session identifier.</param>
    /// <param name="revision">The workspace revision captured with the preview.</param>
    /// <param name="isDirty">Whether the workspace has unsaved changes.</param>
    /// <param name="records">The pending-record page.</param>
    /// <param name="nextCursor">The cursor for the next pending-record page, if any.</param>
    public McpWorkspacePreviewResult(
        string workspaceId,
        ulong revision,
        bool isDirty,
        IReadOnlyList<McpRecordSnapshotResult> records,
        string? nextCursor)
    {
        WorkspaceId = workspaceId;
        Revision = revision;
        IsDirty = isDirty;
        Records = records;
        NextCursor = nextCursor;
    }

    /// <summary>Gets the session identifier.</summary>
    public string WorkspaceId { get; }

    /// <summary>Gets the workspace revision captured with the preview.</summary>
    public ulong Revision { get; }

    /// <summary>Gets whether the workspace has unsaved changes.</summary>
    public bool IsDirty { get; }

    /// <summary>Gets the pending-record page.</summary>
    public IReadOnlyList<McpRecordSnapshotResult> Records { get; }

    /// <summary>Gets the cursor for the next pending-record page, if any.</summary>
    public string? NextCursor { get; }
}

/// <summary>Returns a save that published and adopted its output.</summary>
public sealed class McpWorkspaceSaveResult
{
    /// <summary>Initializes a successful save result.</summary>
    /// <param name="workspaceId">The session identifier.</param>
    /// <param name="status">The terminal save status.</param>
    /// <param name="revision">The revision after save.</param>
    /// <param name="destinationPath">The published output path.</param>
    /// <param name="publicationState">The observed publication state.</param>
    /// <param name="requiresReopen">Whether the workspace must be reopened.</param>
    /// <param name="publishedPaths">Paths published by the save.</param>
    public McpWorkspaceSaveResult(
        string workspaceId,
        string status,
        ulong revision,
        string destinationPath,
        string publicationState,
        bool requiresReopen,
        IReadOnlyList<string> publishedPaths)
    {
        WorkspaceId = workspaceId;
        Status = status;
        Revision = revision;
        DestinationPath = destinationPath;
        PublicationState = publicationState;
        RequiresReopen = requiresReopen;
        PublishedPaths = publishedPaths;
    }

    /// <summary>Gets the session identifier.</summary>
    public string WorkspaceId { get; }

    /// <summary>Gets the terminal save status.</summary>
    public string Status { get; }

    /// <summary>Gets the revision after save.</summary>
    public ulong Revision { get; }

    /// <summary>Gets the published output path.</summary>
    public string DestinationPath { get; }

    /// <summary>Gets the observed publication state.</summary>
    public string PublicationState { get; }

    /// <summary>Gets whether the workspace must be reopened.</summary>
    public bool RequiresReopen { get; }

    /// <summary>Gets the paths published by the save.</summary>
    public IReadOnlyList<string> PublishedPaths { get; }
}

/// <summary>Returns the workspace state after a discard.</summary>
public sealed class McpWorkspaceDiscardResult
{
    /// <summary>Initializes a discard result.</summary>
    /// <param name="workspaceId">The session identifier.</param>
    /// <param name="revision">The revision after discard.</param>
    /// <param name="changed">Whether discard changed the in-memory output.</param>
    /// <param name="isDirty">Whether the workspace remains dirty.</param>
    public McpWorkspaceDiscardResult(string workspaceId, ulong revision, bool changed, bool isDirty)
    {
        WorkspaceId = workspaceId;
        Revision = revision;
        Changed = changed;
        IsDirty = isDirty;
    }

    /// <summary>Gets the session identifier.</summary>
    public string WorkspaceId { get; }

    /// <summary>Gets the revision after discard.</summary>
    public ulong Revision { get; }

    /// <summary>Gets whether discard changed the in-memory output.</summary>
    public bool Changed { get; }

    /// <summary>Gets whether the workspace remains dirty.</summary>
    public bool IsDirty { get; }
}
