using CreationsForge.Engine.Interfaces;
using CreationsForge.Engine.Records;
using CreationsForge.Engine.Workspaces;
using CreationsForge.Mcp.Protocol;
using CreationsForge.Mcp.Sessions;
using ModelContextProtocol.Protocol;
using Mutagen.Bethesda;

namespace CreationsForge.Mcp;

/// <summary>Implements the generic production authoring contract over the UI-neutral engine.</summary>
public sealed partial class McpAuthoringService
{
    private readonly Dictionary<GameRelease, IGameIntegration> _integrations;
    private readonly HashSet<GameRelease> _supported;
    private readonly PluginWorkspaceFactory _factory;
    private readonly McpSessionRegistry _sessions;
    private readonly McpOperationReplay _replay;

    /// <summary>Initializes the authoring service.</summary>
    /// <param name="integrations">The game integrations admitted by the host.</param>
    /// <param name="factory">The workspace factory.</param>
    /// <param name="sessions">The one-session registry.</param>
    /// <param name="replay">The process-lifetime operation replay cache.</param>
    internal McpAuthoringService(
        IReadOnlyList<IGameIntegration> integrations,
        PluginWorkspaceFactory factory,
        McpSessionRegistry sessions,
        McpOperationReplay replay)
    {
        ArgumentNullException.ThrowIfNull(integrations);
        ArgumentNullException.ThrowIfNull(factory);
        ArgumentNullException.ThrowIfNull(sessions);
        ArgumentNullException.ThrowIfNull(replay);
        _integrations = integrations.ToDictionary(integration => integration.Release);
        _supported = _integrations.Keys.ToHashSet();
        _factory = factory;
        _sessions = sessions;
        _replay = replay;
    }

    /// <summary>Returns the production host identity and closed tool names.</summary>
    /// <param name="cancellationToken">The call cancellation token.</param>
    /// <returns>The structured server-info result.</returns>
    public Task<CallToolResult> ServerInfoAsync(CancellationToken cancellationToken)
    {
        return Guard(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            var releases = _supported
                .Select(release => release.ToString())
                .OrderBy(name => name, StringComparer.Ordinal)
                .ToArray();
            return McpToolResults.Success(new McpServerInfoResult(
                "CreationsForge",
                McpHostRunner.GetServerVersion(),
                releases,
                McpAuthoringContract.ToolNames));
        });
    }

    /// <summary>Lists editable families for one supported release.</summary>
    /// <param name="release">The game release name.</param>
    /// <param name="cancellationToken">The call cancellation token.</param>
    /// <returns>The structured record-type result.</returns>
    public Task<CallToolResult> RecordTypesAsync(string? release, CancellationToken cancellationToken)
    {
        return Guard(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            var parsed = McpAuthoringArguments.ParseRelease(release, _supported);
            var families = _integrations[parsed].RecordFamilies
                .Select(family => family.Descriptor.FamilyId)
                .ToArray();
            return McpToolResults.Success(new McpRecordTypesResult(parsed.ToString(), families));
        });
    }

    /// <summary>Returns one page of a family's registered field schema.</summary>
    /// <param name="release">The game release name.</param>
    /// <param name="familyId">The declared family identifier.</param>
    /// <param name="cursor">The schema field cursor, if any.</param>
    /// <param name="cancellationToken">The call cancellation token.</param>
    /// <returns>The structured schema page.</returns>
    public Task<CallToolResult> RecordSchemaAsync(
        string? release,
        string? familyId,
        string? cursor,
        CancellationToken cancellationToken)
    {
        return Guard(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            var parsedRelease = McpAuthoringArguments.ParseRelease(release, _supported);
            var parsedFamily = McpAuthoringArguments.RequireText(familyId, "familyId");
            var descriptor = RequireIntegrationFamily(parsedRelease, parsedFamily);
            var payload = McpAuthoringArguments.DecodeCursor(cursor, McpAuthoringProjection.SchemaCursor);
            var skip = 0;
            if (payload is not null)
            {
                McpAuthoringArguments.Match(payload.Release, parsedRelease.ToString(), "release");
                McpAuthoringArguments.Match(payload.FamilyId, parsedFamily, "familyId");
                skip = payload.Skip;
            }

            var fields = descriptor.Fields.Select(McpAuthoringProjection.SchemaField).ToArray();
            var (page, hasMore) = McpAuthoringProjection.Page(fields, skip);
            string? next = null;
            if (hasMore)
            {
                next = McpCursors.Encode(new McpCursorPayload
                {
                    Kind = McpAuthoringProjection.SchemaCursor,
                    Release = parsedRelease.ToString(),
                    FamilyId = parsedFamily,
                    Skip = skip + page.Length,
                });
            }

            return McpToolResults.Success(new McpRecordSchemaResult(
                parsedRelease.ToString(),
                parsedFamily,
                descriptor.SchemaVersion,
                page,
                next));
        });
    }

    /// <summary>Returns pending or native state for the one session.</summary>
    /// <param name="workspaceId">The session identifier.</param>
    /// <param name="cancellationToken">The call cancellation token.</param>
    /// <returns>The structured workspace state.</returns>
    public Task<CallToolResult> WorkspaceStateAsync(string? workspaceId, CancellationToken cancellationToken)
    {
        return Guard(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            var id = McpAuthoringArguments.RequireText(workspaceId, "workspaceId");
            var failure = _sessions.TryGet(id, out var session);
            if (failure is not null || session is null)
            {
                return failure ?? MissingSession(id);
            }

            return McpToolResults.Success(McpAuthoringProjection.State(session));
        });
    }

    private RecordFamilyDescriptor RequireIntegrationFamily(GameRelease release, string familyId)
    {
        var descriptor = _integrations[release].RecordFamilies
            .Select(family => family.Descriptor)
            .FirstOrDefault(family => string.Equals(family.FamilyId, familyId, StringComparison.Ordinal));
        if (descriptor is null)
        {
            throw new McpContractException(
                "invalid_input",
                $"Family '{familyId}' is not editable for {release}.");
        }

        return descriptor;
    }

    private Task<CallToolResult> Replay(
        string tool,
        string? operationId,
        object arguments,
        Func<CancellationToken, McpInvocation> execute,
        CancellationToken cancellationToken)
    {
        return ReplayAsync(
            tool,
            operationId,
            arguments,
            token => Task.FromResult(execute(token)),
            cancellationToken);
    }

    private async Task<CallToolResult> ReplayAsync(
        string tool,
        string? operationId,
        object arguments,
        Func<CancellationToken, Task<McpInvocation>> execute,
        CancellationToken cancellationToken)
    {
        try
        {
            var id = McpAuthoringArguments.RequireOperationId(operationId);
            var fingerprint = tool + "\n" + McpToolResults.Fingerprint(arguments);
            return await _replay.ExecuteAsync(id, fingerprint, execute, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            return McpErrorMapper.Map(exception).Replay();
        }
    }

    private static Task<CallToolResult> Guard(Func<McpInvocation> execute)
    {
        try
        {
            return Task.FromResult(execute().Replay());
        }
        catch (Exception exception)
        {
            return Task.FromResult(McpErrorMapper.Map(exception).Replay());
        }
    }

    private static McpInvocation MissingSession(string workspaceId)
    {
        return McpToolResults.Failure(
            "workspace_not_found",
            $"Workspace '{workspaceId}' was not found.",
            new { workspaceId });
    }
}
