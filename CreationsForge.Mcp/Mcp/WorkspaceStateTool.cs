using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace CreationsForge.Mcp;

/// <summary>Reads an atomic workspace state snapshot, including while save recovery blocks ordinary operations.</summary>
public sealed class WorkspaceStateTool : McpToolBase
{
    /// <summary>The exact accepted argument names.</summary>
    private static readonly IReadOnlySet<string> AllowedArguments = new HashSet<string>(StringComparer.Ordinal)
    {
        "workspaceId",
    };

    /// <summary>The closed workspace-state input schema.</summary>
    private static readonly JsonElement InputSchema = ParseSchema(
        """{"type":"object","properties":{"workspaceId":{"type":"string","format":"uuid"}},"required":["workspaceId"],"additionalProperties":false}""");

    /// <summary>The closed bounded workspace-state receipt schema.</summary>
    private static readonly JsonElement OutputSchema = McpToolSchema.Output(
        "{\"type\":\"object\",\"properties\":{\"workspaceId\":{\"type\":\"string\",\"format\":\"uuid\"},\"game\":{\"type\":\"string\",\"enum\":[\"starfield\",\"fallout4\",\"skyrim\"]},\"release\":{\"type\":\"string\",\"enum\":[\"starfield\",\"fallout4\",\"skyrim_se\"]},\"revision\":" + McpToolSchema.Revision + ",\"output\":" + McpSaveToolSchema.NullableMetadataReference + ",\"outputBaseline\":" + McpSaveToolSchema.NullableMetadataReference + ",\"outputSynchronization\":{\"type\":\"object\",\"properties\":{\"status\":{\"type\":\"string\",\"enum\":[\"ready\",\"recovery_required\",\"reopen_required\"]},\"pendingSave\":{\"oneOf\":[{\"type\":\"object\",\"properties\":{\"originalWorkspaceId\":{\"type\":\"string\",\"format\":\"uuid\"},\"saveOperationId\":{\"type\":\"string\",\"format\":\"uuid\"},\"saveBaseRevision\":" + McpToolSchema.Revision + ",\"game\":{\"type\":\"string\"},\"release\":{\"type\":\"string\"},\"output\":" + McpSaveToolSchema.MetadataReference + "},\"required\":[\"originalWorkspaceId\",\"saveOperationId\",\"saveBaseRevision\",\"game\",\"release\",\"output\"],\"additionalProperties\":false},{\"type\":\"null\"}]}},\"required\":[\"status\",\"pendingSave\"],\"additionalProperties\":false}},\"required\":[\"workspaceId\",\"game\",\"release\",\"revision\",\"output\",\"outputBaseline\",\"outputSynchronization\"],\"additionalProperties\":false}");

    /// <summary>The host-owned workspace registry.</summary>
    private readonly McpWorkspaceRegistry WorkspaceRegistry;

    /// <summary>The host-owned immutable metadata store.</summary>
    private readonly McpMetadataStore MetadataStore;

    /// <summary>Initializes the atomic workspace-state tool.</summary>
    /// <param name="workspaceRegistry">The registry that owns active workspaces.</param>
    /// <param name="metadataStore">The store retaining exact output metadata references.</param>
    internal WorkspaceStateTool(McpWorkspaceRegistry workspaceRegistry, McpMetadataStore metadataStore)
    {
        ArgumentNullException.ThrowIfNull(workspaceRegistry);
        ArgumentNullException.ThrowIfNull(metadataStore);
        WorkspaceRegistry = workspaceRegistry;
        MetadataStore = metadataStore;
    }

    /// <summary>Gets the closed read-only workspace-state descriptor.</summary>
    public override Tool ProtocolTool { get; } = new Tool
    {
        Name = "creationsforge_workspace_state",
        Title = "Read CreationsForge workspace state",
        Description = "Reads one atomic workspace state and issues exact metadata handles without traversing records.",
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

    /// <summary>Reads and projects one atomic state snapshot, including blocked recovery states.</summary>
    /// <param name="request">The MCP request.</param>
    /// <param name="cancellationToken">The token propagated through the serialized Core read.</param>
    /// <returns>A bounded state receipt with opaque exact metadata references.</returns>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is observed.</exception>
    public override async ValueTask<CallToolResult> InvokeAsync(
        RequestContext<CallToolRequestParams> request,
        CancellationToken cancellationToken = default)
    {
        if (!McpInput.TryGetArguments(request, AllowedArguments, out var arguments, out var error) ||
            !McpInput.TryGetRequiredGuid(arguments, "workspaceId", out var workspaceId, out error))
        {
            return Error("invalid_arguments", error);
        }

        var result = await WorkspaceRegistry.ExecuteAsync(
            workspaceId,
            (workspace, token) => workspace.ReadStateAsync(token),
            cancellationToken).ConfigureAwait(false);
        if (!result.Succeeded)
        {
            return EngineFailure(result);
        }

        var state = result.Value;
        if (state is null || result.ResultRevision != state.Revision)
        {
            return Error("unexpected_failure", "The engine state snapshot is absent or lacks matching contextual revision metadata.");
        }

        var synchronization = state.OutputSynchronization;
        var pending = synchronization.PendingSave;
        if (!MetadataStore.TryPublishWorkspaceStateMetadata(
                workspaceId,
                state.Output,
                state.OutputBaseline,
                pending?.Output,
                out var outputReference,
                out var baselineReference,
                out var pendingOutputReference))
        {
            return Error("metadata_capacity_exceeded", "The host cannot retain new workspace-state metadata; the live workspace was not changed.");
        }

        if ((pending is null) != (pendingOutputReference is null))
        {
            return Error("unexpected_failure", "The pending-save output could not be represented consistently.");
        }

        var projected = McpProjection.Json(new
        {
            workspaceId = workspaceId.ToString("D"),
            game = McpMetadataProjection.Game(state.Game),
            release = McpMetadataProjection.Release(state.Release),
            revision = McpProjection.Revision(state.Revision),
            output = outputReference is null ? null : McpMetadataProjection.Reference(outputReference),
            outputBaseline = baselineReference is null ? null : McpMetadataProjection.Reference(baselineReference),
            outputSynchronization = new
            {
                status = McpMetadataProjection.SynchronizationStatus(synchronization.Status),
                pendingSave = pending is null
                    ? null
                    : new
                    {
                        originalWorkspaceId = pending.OriginalWorkspaceId.ToString("D"),
                        saveOperationId = pending.SaveOperationId.ToString("D"),
                        saveBaseRevision = McpProjection.Revision(pending.SaveBaseRevision),
                        game = McpMetadataProjection.Game(pending.Game),
                        release = McpMetadataProjection.Release(pending.Release),
                        output = McpMetadataProjection.Reference(pendingOutputReference!),
                    },
            },
        });
        return FitsStructuredResultBudget(projected)
            ? Success(projected, $"Read atomic state for workspace {workspaceId:D} at revision {state.Revision}.")
            : Error("result_too_large", "The workspace-state receipt exceeds the 64 KiB structured result budget.");
    }
}
