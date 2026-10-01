using CreationsForge.Engine.Records;
using CreationsForge.Engine.Workspaces;
using CreationsForge.Mcp.Sessions;

namespace CreationsForge.Mcp.Protocol;

/// <summary>Projects engine snapshots into closed authoring results and cursor pages.</summary>
internal static class McpAuthoringProjection
{
    /// <summary>Cursor kind for a winning-record page.</summary>
    public const string SearchCursor = "search";

    /// <summary>Cursor kind for a record field page.</summary>
    public const string FieldsCursor = "fields";

    /// <summary>Cursor kind for a schema field page.</summary>
    public const string SchemaCursor = "schema";

    /// <summary>Cursor kind for a comparison page.</summary>
    public const string CompareCursor = "compare";

    /// <summary>Cursor kind for a pending-record page.</summary>
    public const string PreviewCursor = "preview";

    /// <summary>Cursor kind for one pending record's field page.</summary>
    public const string PreviewFieldsCursor = "preview-fields";

    /// <summary>Projects pending or native session state.</summary>
    /// <param name="session">The authoring session.</param>
    /// <returns>The closed state result.</returns>
    public static McpWorkspaceStateResult State(McpAuthoringSession session)
    {
        ArgumentNullException.ThrowIfNull(session);
        var workspace = session.Workspace;
        var state = workspace?.State;
        var open = state is not null;
        return new McpWorkspaceStateResult(
            session.Id,
            open,
            session.Release.ToString(),
            session.DataDirectory,
            session.SelectedPlugins,
            open ? state!.OutputPath : null,
            open ? state!.OutputModKey.ToString() : null,
            open ? state!.MasterStyle.ToString() : null,
            open ? state!.TextStorageMode.ToString() : null,
            open && state!.IsNewOutput,
            open && state!.IsDirty,
            open ? state!.Revision : 0,
            open && state!.RequiresReopen);
    }

    /// <summary>Projects one registered field descriptor.</summary>
    /// <param name="field">The engine descriptor.</param>
    /// <returns>The closed schema field.</returns>
    public static McpSchemaFieldResult SchemaField(RecordFieldDescriptor field)
    {
        ArgumentNullException.ThrowIfNull(field);
        return new McpSchemaFieldResult(
            field.Path,
            field.Kind.ToString(),
            field.IsNullable,
            field.IsFlags,
            field.Choices,
            field.Operations.Select(operation => operation.ToString()).ToArray(),
            field.Alternatives,
            field.ReferenceTargets);
    }

    /// <summary>Projects one winning-record summary.</summary>
    /// <param name="summary">The engine summary.</param>
    /// <returns>The closed identity.</returns>
    public static McpRecordIdentityResult Identity(PluginRecordSummary summary)
    {
        ArgumentNullException.ThrowIfNull(summary);
        return new McpRecordIdentityResult(
            summary.OriginFormKey.ToString(),
            summary.ContainingModKey.ToString(),
            summary.WinningModKey.ToString(),
            summary.EditorId);
    }

    /// <summary>Projects one exact record and a page of its registered fields.</summary>
    /// <param name="snapshot">The engine snapshot.</param>
    /// <param name="orderedFields">Registered fields in descriptor order.</param>
    /// <param name="skip">The number of fields already returned.</param>
    /// <param name="cursorKind">The field-cursor kind to emit.</param>
    /// <param name="workspaceId">The session that owns the snapshot.</param>
    /// <param name="revision">The revision captured with the snapshot.</param>
    /// <returns>The paged snapshot.</returns>
    public static McpRecordSnapshotResult Snapshot(
        RecordSnapshot snapshot,
        IReadOnlyList<McpFieldValueResult> orderedFields,
        int skip,
        string cursorKind,
        string workspaceId,
        ulong revision)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        ArgumentNullException.ThrowIfNull(orderedFields);
        var (page, hasMore) = Page(orderedFields, skip);
        string? next = null;
        if (hasMore)
        {
            next = McpCursors.Encode(new McpCursorPayload
            {
                Kind = cursorKind,
                WorkspaceId = workspaceId,
                Revision = revision,
                FamilyId = snapshot.FamilyId,
                FormKey = snapshot.FormKey.ToString(),
                ContainingModKey = snapshot.ContainingModKey.ToString(),
                Skip = skip + page.Length,
            });
        }

        return new McpRecordSnapshotResult(
            snapshot.FamilyId,
            snapshot.FormKey.ToString(),
            snapshot.ContainingModKey.ToString(),
            page,
            next);
    }

    /// <summary>Returns one fixed-size page.</summary>
    /// <typeparam name="T">The item type.</typeparam>
    /// <param name="items">The complete ordered items.</param>
    /// <param name="skip">The number of items already returned.</param>
    /// <returns>The page and whether another item remains.</returns>
    public static (T[] Page, bool HasMore) Page<T>(IReadOnlyList<T> items, int skip)
    {
        ArgumentNullException.ThrowIfNull(items);
        if (skip >= items.Count)
        {
            return ([], false);
        }

        var count = Math.Min(McpAuthoringContract.PageSize, items.Count - skip);
        var page = new T[count];
        for (var index = 0; index < count; index++)
        {
            page[index] = items[skip + index];
        }

        return (page, skip + count < items.Count);
    }

    /// <summary>Orders registered snapshot fields by their family descriptor.</summary>
    /// <param name="snapshot">The engine snapshot.</param>
    /// <param name="descriptor">The family descriptor.</param>
    /// <returns>The field values in descriptor order.</returns>
    public static McpFieldValueResult[] OrderedFields(RecordSnapshot snapshot, RecordFamilyDescriptor descriptor)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        ArgumentNullException.ThrowIfNull(descriptor);
        return descriptor.Fields
            .Select(field => new McpFieldValueResult(field.Path, McpValueMapper.FromRecordValue(snapshot.Values[field.Path])))
            .ToArray();
    }
}
