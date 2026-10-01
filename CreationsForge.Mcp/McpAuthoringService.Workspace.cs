using CreationsForge.Engine.Workspaces;
using CreationsForge.Mcp.Protocol;
using CreationsForge.Mcp.Sessions;
using ModelContextProtocol.Protocol;
using Mutagen.Bethesda.Plugins;

namespace CreationsForge.Mcp;

public sealed partial class McpAuthoringService
{
    /// <summary>Stores pending game and source configuration without opening a native workspace.</summary>
    /// <param name="operationId">The client operation identifier.</param>
    /// <param name="release">The game release name.</param>
    /// <param name="dataDirectory">The game data directory.</param>
    /// <param name="selectedPlugins">Selected plugin file names in low-to-high priority order.</param>
    /// <param name="cancellationToken">The call cancellation token.</param>
    /// <returns>The pending session state, or the replayed result.</returns>
    public Task<CallToolResult> WorkspaceOpenAsync(
        string? operationId,
        string? release,
        string? dataDirectory,
        IReadOnlyList<string>? selectedPlugins,
        CancellationToken cancellationToken)
    {
        return Replay(
            McpAuthoringContract.WorkspaceOpenTool,
            operationId,
            new { release, dataDirectory, selectedPlugins },
            token => OpenWorkspace(release, dataDirectory, selectedPlugins, token),
            cancellationToken);
    }

    /// <summary>Closes the session, discarding unsaved changes only when explicitly requested.</summary>
    /// <param name="operationId">The client operation identifier.</param>
    /// <param name="workspaceId">The session identifier.</param>
    /// <param name="discardUnsaved">Whether unsaved changes may be discarded.</param>
    /// <param name="cancellationToken">The call cancellation token.</param>
    /// <returns>The close result, or the replayed result.</returns>
    public Task<CallToolResult> WorkspaceCloseAsync(
        string? operationId,
        string? workspaceId,
        bool discardUnsaved,
        CancellationToken cancellationToken)
    {
        return Replay(
            McpAuthoringContract.WorkspaceCloseTool,
            operationId,
            new { workspaceId, discardUnsaved },
            token => CloseWorkspace(workspaceId, discardUnsaved, token),
            cancellationToken);
    }

    /// <summary>Creates a new native output and attaches it to the pending session.</summary>
    /// <param name="operationId">The client operation identifier.</param>
    /// <param name="workspaceId">The session identifier.</param>
    /// <param name="outputPath">The output plugin path.</param>
    /// <param name="masterStyle">The requested master style.</param>
    /// <param name="textStorageMode">The requested text storage mode.</param>
    /// <param name="language">The active translated-string language, or English when omitted.</param>
    /// <param name="cancellationToken">The call cancellation token.</param>
    /// <returns>The opened session state, or the replayed result.</returns>
    public Task<CallToolResult> OutputCreateAsync(
        string? operationId,
        string? workspaceId,
        string? outputPath,
        string? masterStyle,
        string? textStorageMode,
        string? language,
        CancellationToken cancellationToken)
    {
        return OpenOutputTool(
            McpAuthoringContract.OutputCreateTool,
            operationId,
            workspaceId,
            outputPath,
            masterStyle,
            textStorageMode,
            language,
            createNew: true,
            cancellationToken);
    }

    /// <summary>Opens an existing native output and attaches it to the pending session.</summary>
    /// <param name="operationId">The client operation identifier.</param>
    /// <param name="workspaceId">The session identifier.</param>
    /// <param name="outputPath">The existing output plugin path.</param>
    /// <param name="masterStyle">The requested master style.</param>
    /// <param name="textStorageMode">The requested text storage mode.</param>
    /// <param name="language">The active translated-string language, or English when omitted.</param>
    /// <param name="cancellationToken">The call cancellation token.</param>
    /// <returns>The opened session state, or the replayed result.</returns>
    public Task<CallToolResult> OutputOpenAsync(
        string? operationId,
        string? workspaceId,
        string? outputPath,
        string? masterStyle,
        string? textStorageMode,
        string? language,
        CancellationToken cancellationToken)
    {
        return OpenOutputTool(
            McpAuthoringContract.OutputOpenTool,
            operationId,
            workspaceId,
            outputPath,
            masterStyle,
            textStorageMode,
            language,
            createNew: false,
            cancellationToken);
    }

    /// <summary>Returns one page of unsaved registered changes.</summary>
    /// <param name="workspaceId">The session identifier.</param>
    /// <param name="cursor">The preview or preview-field cursor, if any.</param>
    /// <param name="cancellationToken">The call cancellation token.</param>
    /// <returns>The structured preview page.</returns>
    public Task<CallToolResult> WorkspacePreviewAsync(
        string? workspaceId,
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

            return Preview(workspace!, id, cursor);
        });
    }

    /// <summary>Saves and publishes the native output.</summary>
    /// <param name="operationId">The client operation identifier.</param>
    /// <param name="workspaceId">The session identifier.</param>
    /// <param name="cancellationToken">The call cancellation token.</param>
    /// <returns>The structured save outcome, or the replayed result.</returns>
    public Task<CallToolResult> WorkspaceSaveAsync(
        string? operationId,
        string? workspaceId,
        CancellationToken cancellationToken)
    {
        return ReplayAsync(
            McpAuthoringContract.WorkspaceSaveTool,
            operationId,
            new { workspaceId },
            token => SaveAsync(workspaceId, token),
            cancellationToken);
    }

    /// <summary>Restores the last native saved baseline without writing destination files.</summary>
    /// <param name="operationId">The client operation identifier.</param>
    /// <param name="workspaceId">The session identifier.</param>
    /// <param name="cancellationToken">The call cancellation token.</param>
    /// <returns>The discard result, or the replayed result.</returns>
    public Task<CallToolResult> WorkspaceDiscardAsync(
        string? operationId,
        string? workspaceId,
        CancellationToken cancellationToken)
    {
        return Replay(
            McpAuthoringContract.WorkspaceDiscardTool,
            operationId,
            new { workspaceId },
            token => Discard(workspaceId, token),
            cancellationToken);
    }

    private Task<CallToolResult> OpenOutputTool(
        string tool,
        string? operationId,
        string? workspaceId,
        string? outputPath,
        string? masterStyle,
        string? textStorageMode,
        string? language,
        bool createNew,
        CancellationToken cancellationToken)
    {
        return Replay(
            tool,
            operationId,
            new { workspaceId, outputPath, masterStyle, textStorageMode, language, createNew },
            token => OpenOutput(workspaceId, outputPath, masterStyle, textStorageMode, language, createNew, token),
            cancellationToken);
    }

    private McpInvocation OpenWorkspace(
        string? release,
        string? dataDirectory,
        IReadOnlyList<string>? selectedPlugins,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var parsedRelease = McpAuthoringArguments.ParseRelease(release, _supported);
        var directory = McpAuthoringArguments.RequireText(dataDirectory, "dataDirectory");
        var plugins = McpAuthoringArguments.ParsePlugins(selectedPlugins);
        var session = new McpAuthoringSession(Guid.NewGuid().ToString("N"), parsedRelease, directory, plugins);
        var failure = _sessions.TryCreate(session);
        return failure ?? McpToolResults.Success(McpAuthoringProjection.State(session));
    }

    private McpInvocation CloseWorkspace(string? workspaceId, bool discardUnsaved, CancellationToken cancellationToken)
    {
        var id = McpAuthoringArguments.RequireText(workspaceId, "workspaceId");
        var failure = _sessions.TryBeginClose(id, out var session, out var workspace);
        if (failure is not null || session is null)
        {
            return failure ?? MissingSession(id);
        }

        if (workspace is null)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                _sessions.CompleteClose(session);
                return McpToolResults.Success(new McpWorkspaceCloseResult(id, closed: true, revision: 0));
            }
            catch (Exception exception)
            {
                _sessions.AbortClose(session);
                return McpErrorMapper.Map(exception);
            }
        }

        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            var state = workspace.State;
            if (!discardUnsaved && (state.IsDirty || state.RequiresReopen))
            {
                _sessions.AbortClose(session);
                return McpToolResults.Failure(
                    "unsaved_changes",
                    "The workspace has unsaved changes. Close again with discardUnsaved set to true to discard them.",
                    new { revision = state.Revision, requiresReopen = state.RequiresReopen });
            }

            if (discardUnsaved && state.IsDirty && !state.RequiresReopen)
            {
                workspace.Discard();
            }
        }
        catch (Exception exception)
        {
            _sessions.AbortClose(session);
            return McpErrorMapper.Map(exception);
        }

        var revision = workspace.State.Revision;
        try
        {
            workspace.Dispose();
        }
        catch (Exception exception)
        {
            _sessions.CompleteClose(session);
            return McpErrorMapper.HostFailure(exception);
        }

        _sessions.CompleteClose(session);
        return McpToolResults.Success(new McpWorkspaceCloseResult(id, closed: true, revision));
    }

    private McpInvocation OpenOutput(
        string? workspaceId,
        string? outputPath,
        string? masterStyle,
        string? textStorageMode,
        string? language,
        bool createNew,
        CancellationToken cancellationToken)
    {
        var id = McpAuthoringArguments.RequireText(workspaceId, "workspaceId");
        var failure = _sessions.TryBeginOutput(id, out var session);
        if (failure is not null || session is null)
        {
            return failure ?? MissingSession(id);
        }

        PluginWorkspace? opened = null;
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            var request = new PluginWorkspaceOpenRequest(
                session.Release,
                session.DataDirectory,
                ToModKeys(session.SelectedPlugins),
                CreateOutput(outputPath, masterStyle, textStorageMode, language, createNew));
            opened = _factory.Open(request);
            _sessions.CompleteOutput(session, opened);
            opened = null;
            return McpToolResults.Success(McpAuthoringProjection.State(session));
        }
        catch (Exception exception)
        {
            if (opened is not null)
            {
                try
                {
                    opened.Dispose();
                }
                catch (Exception disposeException)
                {
                    _sessions.FailOutput(session);
                    return McpErrorMapper.HostFailure(disposeException);
                }
            }

            _sessions.FailOutput(session);
            return McpErrorMapper.Map(exception);
        }
    }

    private McpInvocation Preview(PluginWorkspace workspace, string workspaceId, string? cursor)
    {
        var payload = McpAuthoringArguments.DecodeCursor(
            cursor,
            McpAuthoringProjection.PreviewCursor,
            McpAuthoringProjection.PreviewFieldsCursor);
        var preview = workspace.Preview();
        if (payload is not null)
        {
            McpAuthoringArguments.BindWorkspace(payload, workspaceId, preview.State.Revision);
        }

        if (payload?.Kind == McpAuthoringProjection.PreviewFieldsCursor)
        {
            return PreviewFields(workspace, workspaceId, preview, payload);
        }

        var skip = payload?.Skip ?? 0;
        var (records, hasMore) = McpAuthoringProjection.Page(preview.PendingRecords, skip);
        var page = records
            .Select(record => ProjectSnapshot(
                workspace,
                record,
                workspaceId,
                preview.State.Revision,
                McpAuthoringProjection.PreviewFieldsCursor,
                0))
            .ToArray();
        string? next = null;
        if (hasMore)
        {
            next = McpCursors.Encode(new McpCursorPayload
            {
                Kind = McpAuthoringProjection.PreviewCursor,
                WorkspaceId = workspaceId,
                Revision = preview.State.Revision,
                Skip = skip + page.Length,
            });
        }

        return McpToolResults.Success(new McpWorkspacePreviewResult(
            workspaceId,
            preview.State.Revision,
            preview.State.IsDirty,
            page,
            next));
    }

    private static McpInvocation PreviewFields(
        PluginWorkspace workspace,
        string workspaceId,
        PluginWorkspacePreview preview,
        McpCursorPayload payload)
    {
        var match = preview.PendingRecords.FirstOrDefault(record =>
            string.Equals(record.FamilyId, payload.FamilyId, StringComparison.Ordinal)
            && string.Equals(record.FormKey.ToString(), payload.FormKey, StringComparison.Ordinal)
            && string.Equals(record.ContainingModKey.ToString(), payload.ContainingModKey, StringComparison.Ordinal));
        if (match is null)
        {
            throw new McpContractException("invalid_input", "The preview cursor does not match a pending record.");
        }

        var snapshot = ProjectSnapshot(
            workspace,
            match,
            workspaceId,
            preview.State.Revision,
            McpAuthoringProjection.PreviewFieldsCursor,
            payload.Skip);
        return McpToolResults.Success(new McpWorkspacePreviewResult(
            workspaceId,
            preview.State.Revision,
            preview.State.IsDirty,
            [snapshot],
            null));
    }

    private async Task<McpInvocation> SaveAsync(string? workspaceId, CancellationToken cancellationToken)
    {
        if (!TryOpen(workspaceId, out var workspace, out var id, out var failure))
        {
            return failure!;
        }

        var result = await workspace!.SaveAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        var success = new McpWorkspaceSaveResult(
            id,
            result.Status.ToString(),
            result.Revision,
            result.DestinationPath,
            result.PublicationState.ToString(),
            result.RequiresWorkspaceReopen,
            result.PublishedPaths);
        return McpErrorMapper.MapSave(result, success);
    }

    private McpInvocation Discard(string? workspaceId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!TryOpen(workspaceId, out var workspace, out var id, out var failure))
        {
            return failure!;
        }

        var result = workspace!.Discard();
        return McpToolResults.Success(new McpWorkspaceDiscardResult(id, result.Revision, result.Changed, result.IsDirty));
    }

    private bool TryOpen(
        string? workspaceId,
        out PluginWorkspace? workspace,
        out string id,
        out McpInvocation? failure)
    {
        id = McpAuthoringArguments.RequireText(workspaceId, "workspaceId");
        failure = _sessions.TryGetOpen(id, out workspace);
        if (failure is not null || workspace is null)
        {
            workspace = null;
            failure ??= MissingSession(id);
            return false;
        }

        return true;
    }

    private static PluginOutputDefinition CreateOutput(
        string? outputPath,
        string? masterStyle,
        string? textStorageMode,
        string? language,
        bool createNew)
    {
        var path = McpAuthoringArguments.RequireText(outputPath, "outputPath");
        return new PluginOutputDefinition(
            path,
            McpAuthoringArguments.ParseModKey(Path.GetFileName(path)),
            McpAuthoringArguments.ParseMasterStyle(masterStyle),
            McpAuthoringArguments.ParseTextStorage(textStorageMode),
            createNew,
            McpAuthoringArguments.ParseLanguage(language));
    }

    private static ModKey[] ToModKeys(IReadOnlyList<string> plugins)
    {
        var keys = new ModKey[plugins.Count];
        for (var index = 0; index < plugins.Count; index++)
        {
            keys[index] = McpAuthoringArguments.ParseModKey(plugins[index]);
        }

        return keys;
    }
}
