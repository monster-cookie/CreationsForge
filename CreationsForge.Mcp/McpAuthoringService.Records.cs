using CreationsForge.Engine.Records;
using CreationsForge.Engine.Workspaces;
using CreationsForge.Mcp.Protocol;
using ModelContextProtocol.Protocol;

namespace CreationsForge.Mcp;

public sealed partial class McpAuthoringService
{
    /// <summary>Returns one page of winning records for a declared family.</summary>
    /// <param name="workspaceId">The session identifier.</param>
    /// <param name="familyId">The declared family identifier.</param>
    /// <param name="editorId">The optional EditorID filter. Blank means no filter.</param>
    /// <param name="cursor">The search cursor, if any.</param>
    /// <param name="cancellationToken">The call cancellation token.</param>
    /// <returns>The structured search page.</returns>
    public Task<CallToolResult> RecordsSearchAsync(
        string? workspaceId,
        string? familyId,
        string? editorId,
        string? cursor,
        CancellationToken cancellationToken)
    {
        var normalizedEditorId = NormalizeEditorId(editorId);
        return Guard(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!TryOpen(workspaceId, out var workspace, out var id, out var failure))
            {
                return failure!;
            }

            return Search(workspace!, id, familyId, normalizedEditorId, cursor);
        });
    }

    /// <summary>Reads one page of registered fields from an exact record version.</summary>
    /// <param name="workspaceId">The session identifier.</param>
    /// <param name="familyId">The declared family identifier.</param>
    /// <param name="formKey">The origin FormKey text.</param>
    /// <param name="containingModKey">The plugin containing the exact version.</param>
    /// <param name="cursor">The field cursor, if any.</param>
    /// <param name="cancellationToken">The call cancellation token.</param>
    /// <returns>The structured record page.</returns>
    public Task<CallToolResult> RecordReadAsync(
        string? workspaceId,
        string? familyId,
        string? formKey,
        string? containingModKey,
        string? cursor,
        CancellationToken cancellationToken)
    {
        return Guard(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!TryOpen(workspaceId, out var workspace, out var id, out var failure))
            {
                return failure!;
            }

            return Read(workspace!, id, familyId, formKey, containingModKey, cursor);
        });
    }

    /// <summary>Compares registered fields from two exact versions of one family.</summary>
    /// <param name="workspaceId">The session identifier.</param>
    /// <param name="familyId">The declared family identifier.</param>
    /// <param name="leftFormKey">The left origin FormKey text.</param>
    /// <param name="leftContainingModKey">The left containing plugin.</param>
    /// <param name="rightFormKey">The right origin FormKey text.</param>
    /// <param name="rightContainingModKey">The right containing plugin.</param>
    /// <param name="cursor">The comparison cursor, if any.</param>
    /// <param name="cancellationToken">The call cancellation token.</param>
    /// <returns>The structured comparison page.</returns>
    public Task<CallToolResult> RecordCompareAsync(
        string? workspaceId,
        string? familyId,
        string? leftFormKey,
        string? leftContainingModKey,
        string? rightFormKey,
        string? rightContainingModKey,
        string? cursor,
        CancellationToken cancellationToken)
    {
        return Guard(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!TryOpen(workspaceId, out var workspace, out var id, out var failure))
            {
                return failure!;
            }

            return Compare(
                workspace!,
                id,
                familyId,
                leftFormKey,
                leftContainingModKey,
                rightFormKey,
                rightContainingModKey,
                cursor);
        });
    }

    /// <summary>Creates one record and publishes it with the expected workspace revision.</summary>
    /// <param name="operationId">The client operation identifier.</param>
    /// <param name="workspaceId">The session identifier.</param>
    /// <param name="familyId">The declared family identifier.</param>
    /// <param name="expectedRevision">The revision the change is based on.</param>
    /// <param name="changes">The initial field changes.</param>
    /// <param name="cancellationToken">The call cancellation token.</param>
    /// <returns>The published snapshot, or the replayed result.</returns>
    public Task<CallToolResult> RecordCreateAsync(
        string? operationId,
        string? workspaceId,
        string? familyId,
        ulong? expectedRevision,
        IReadOnlyList<McpFieldChangeDto>? changes,
        CancellationToken cancellationToken)
    {
        return Replay(
            McpAuthoringContract.RecordCreateTool,
            operationId,
            new { workspaceId, familyId, expectedRevision, changes },
            token => ApplyOne(
                workspaceId,
                expectedRevision,
                CreateMutation(nameof(RecordMutationKind.Create), familyId, null, null, changes),
                token),
            cancellationToken);
    }

    /// <summary>Overrides one exact record context and publishes it with the expected workspace revision.</summary>
    /// <param name="operationId">The client operation identifier.</param>
    /// <param name="workspaceId">The session identifier.</param>
    /// <param name="familyId">The declared family identifier.</param>
    /// <param name="formKey">The origin FormKey text.</param>
    /// <param name="containingModKey">The exact containing plugin.</param>
    /// <param name="expectedRevision">The revision the change is based on.</param>
    /// <param name="changes">The field changes.</param>
    /// <param name="cancellationToken">The call cancellation token.</param>
    /// <returns>The published snapshot, or the replayed result.</returns>
    public Task<CallToolResult> RecordOverrideAsync(
        string? operationId,
        string? workspaceId,
        string? familyId,
        string? formKey,
        string? containingModKey,
        ulong? expectedRevision,
        IReadOnlyList<McpFieldChangeDto>? changes,
        CancellationToken cancellationToken)
    {
        return Replay(
            McpAuthoringContract.RecordOverrideTool,
            operationId,
            new { workspaceId, familyId, formKey, containingModKey, expectedRevision, changes },
            token => ApplyOne(
                workspaceId,
                expectedRevision,
                CreateMutation(nameof(RecordMutationKind.Override), familyId, formKey, containingModKey, changes),
                token),
            cancellationToken);
    }

    /// <summary>Publishes an ordered batch of create and override mutations as one revision.</summary>
    /// <param name="operationId">The client operation identifier.</param>
    /// <param name="workspaceId">The session identifier.</param>
    /// <param name="expectedRevision">The revision the batch is based on.</param>
    /// <param name="mutations">The ordered mutations.</param>
    /// <param name="cancellationToken">The call cancellation token.</param>
    /// <returns>The published snapshots, or the replayed result.</returns>
    public Task<CallToolResult> RecordApplyAsync(
        string? operationId,
        string? workspaceId,
        ulong? expectedRevision,
        IReadOnlyList<McpMutationDto>? mutations,
        CancellationToken cancellationToken)
    {
        return Replay(
            McpAuthoringContract.RecordApplyTool,
            operationId,
            new { workspaceId, expectedRevision, mutations },
            token => Apply(workspaceId, expectedRevision, mutations, token),
            cancellationToken);
    }

    private McpInvocation Search(
        PluginWorkspace workspace,
        string workspaceId,
        string? familyId,
        string? editorId,
        string? cursor)
    {
        var parsedFamily = McpAuthoringArguments.RequireText(familyId, "familyId");
        var payload = McpAuthoringArguments.DecodeCursor(cursor, McpAuthoringProjection.SearchCursor);
        var skip = 0;
        if (payload is not null)
        {
            McpAuthoringArguments.BindWorkspace(payload, workspaceId, workspace.State.Revision);
            McpAuthoringArguments.Match(payload.FamilyId, parsedFamily, "familyId");
            McpAuthoringArguments.Match(payload.EditorId, editorId, "editorId");
            skip = payload.Skip;
        }

        var page = workspace.SearchWinningRecords(parsedFamily, editorId, skip, McpAuthoringContract.PageSize);
        string? next = null;
        if (page.HasMore)
        {
            next = McpCursors.Encode(new McpCursorPayload
            {
                Kind = McpAuthoringProjection.SearchCursor,
                WorkspaceId = workspaceId,
                Revision = page.Revision,
                FamilyId = parsedFamily,
                EditorId = editorId,
                Skip = skip + page.Records.Count,
            });
        }

        return McpToolResults.Success(new McpRecordSearchResult(
            workspaceId,
            page.Revision,
            parsedFamily,
            page.Records.Select(McpAuthoringProjection.Identity).ToArray(),
            next));
    }

    private McpInvocation Read(
        PluginWorkspace workspace,
        string workspaceId,
        string? familyId,
        string? formKey,
        string? containingModKey,
        string? cursor)
    {
        var locator = ParseLocator(familyId, formKey, containingModKey);
        var payload = McpAuthoringArguments.DecodeCursor(cursor, McpAuthoringProjection.FieldsCursor);
        var skip = 0;
        if (payload is not null)
        {
            McpAuthoringArguments.BindWorkspace(payload, workspaceId, workspace.State.Revision);
            McpAuthoringArguments.Match(payload.FamilyId, locator.FamilyId, "familyId");
            McpAuthoringArguments.Match(payload.FormKey, locator.FormKey.ToString(), "formKey");
            McpAuthoringArguments.Match(payload.ContainingModKey, locator.ContainingModKey.ToString(), "containingModKey");
            skip = payload.Skip;
        }

        var snapshot = workspace.Records.Read(locator);
        var revision = workspace.State.Revision;
        return McpToolResults.Success(new McpRecordReadResult(
            workspaceId,
            revision,
            ProjectSnapshot(workspace, snapshot, workspaceId, revision, McpAuthoringProjection.FieldsCursor, skip)));
    }

    private McpInvocation Compare(
        PluginWorkspace workspace,
        string workspaceId,
        string? familyId,
        string? leftFormKey,
        string? leftContainingModKey,
        string? rightFormKey,
        string? rightContainingModKey,
        string? cursor)
    {
        var left = ParseLocator(familyId, leftFormKey, leftContainingModKey);
        var right = ParseLocator(familyId, rightFormKey, rightContainingModKey);
        var payload = McpAuthoringArguments.DecodeCursor(cursor, McpAuthoringProjection.CompareCursor);
        var skip = 0;
        if (payload is not null)
        {
            McpAuthoringArguments.BindWorkspace(payload, workspaceId, workspace.State.Revision);
            McpAuthoringArguments.Match(payload.FamilyId, left.FamilyId, "familyId");
            McpAuthoringArguments.Match(payload.FormKey, left.FormKey.ToString(), "formKey");
            McpAuthoringArguments.Match(payload.ContainingModKey, left.ContainingModKey.ToString(), "containingModKey");
            McpAuthoringArguments.Match(payload.RightFormKey, right.FormKey.ToString(), "rightFormKey");
            McpAuthoringArguments.Match(
                payload.RightContainingModKey,
                right.ContainingModKey.ToString(),
                "rightContainingModKey");
            skip = payload.Skip;
        }

        var comparison = workspace.Records.Compare(left, right);
        var revision = workspace.State.Revision;
        var differences = comparison.Differences
            .Select(difference => new McpRecordDifferenceResult(
                difference.Path,
                McpValueMapper.FromRecordValue(difference.Left),
                McpValueMapper.FromRecordValue(difference.Right)))
            .ToArray();
        var (page, hasMore) = McpAuthoringProjection.Page(differences, skip);
        string? next = null;
        if (hasMore)
        {
            next = McpCursors.Encode(new McpCursorPayload
            {
                Kind = McpAuthoringProjection.CompareCursor,
                WorkspaceId = workspaceId,
                Revision = revision,
                FamilyId = left.FamilyId,
                FormKey = left.FormKey.ToString(),
                ContainingModKey = left.ContainingModKey.ToString(),
                RightFormKey = right.FormKey.ToString(),
                RightContainingModKey = right.ContainingModKey.ToString(),
                Skip = skip + page.Length,
            });
        }

        return McpToolResults.Success(new McpRecordCompareResult(workspaceId, revision, left.FamilyId, page, next));
    }

    private McpInvocation ApplyOne(
        string? workspaceId,
        ulong? expectedRevision,
        McpMutationDto mutation,
        CancellationToken cancellationToken)
    {
        return Apply(workspaceId, expectedRevision, [mutation], cancellationToken);
    }

    private McpInvocation Apply(
        string? workspaceId,
        ulong? expectedRevision,
        IReadOnlyList<McpMutationDto>? mutations,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!TryOpen(workspaceId, out var workspace, out var id, out var failure))
        {
            return failure!;
        }

        if (mutations is null)
        {
            throw new McpContractException("invalid_input", "mutations are required.");
        }

        var parsed = mutations.Select(McpAuthoringArguments.ParseMutation).ToArray();
        var applied = workspace!.Records.Apply(
            new RecordChangeSet(McpAuthoringArguments.RequireRevision(expectedRevision), parsed));
        var records = new McpRecordSnapshotResult[applied.Records.Count];
        for (var index = 0; index < applied.Records.Count; index++)
        {
            records[index] = ProjectSnapshot(
                workspace,
                applied.Records[index],
                id,
                applied.Revision,
                McpAuthoringProjection.FieldsCursor,
                0);
        }

        return McpToolResults.Success(new McpRecordApplyResult(id, applied.Revision, records));
    }

    private static McpRecordSnapshotResult ProjectSnapshot(
        PluginWorkspace workspace,
        RecordSnapshot snapshot,
        string workspaceId,
        ulong revision,
        string cursorKind,
        int skip)
    {
        var descriptor = workspace.Records.Families.FirstOrDefault(family =>
            string.Equals(family.FamilyId, snapshot.FamilyId, StringComparison.Ordinal));
        if (descriptor is null)
        {
            throw new McpContractException(
                "invalid_input",
                $"Family '{snapshot.FamilyId}' is not editable in this workspace.");
        }

        return McpAuthoringProjection.Snapshot(
            snapshot,
            McpAuthoringProjection.OrderedFields(snapshot, descriptor),
            skip,
            cursorKind,
            workspaceId,
            revision);
    }

    private static McpMutationDto CreateMutation(
        string kind,
        string? familyId,
        string? formKey,
        string? containingModKey,
        IReadOnlyList<McpFieldChangeDto>? changes)
    {
        if (changes is null)
        {
            throw new McpContractException("invalid_input", "changes are required.");
        }

        return new McpMutationDto
        {
            Kind = kind,
            FamilyId = familyId ?? string.Empty,
            FormKey = formKey,
            ContainingModKey = containingModKey,
            Changes = changes.ToList(),
        };
    }

    private static RecordLocator ParseLocator(string? familyId, string? formKey, string? containingModKey)
    {
        return new RecordLocator(
            McpAuthoringArguments.RequireText(familyId, "familyId"),
            McpAuthoringArguments.ParseFormKey(formKey),
            McpAuthoringArguments.ParseModKey(containingModKey));
    }

    private static string? NormalizeEditorId(string? editorId)
    {
        return string.IsNullOrWhiteSpace(editorId) ? null : editorId;
    }
}
