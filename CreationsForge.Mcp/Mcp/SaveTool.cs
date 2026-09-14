using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace CreationsForge.Mcp;

/// <summary>Runs guarded workspace saves and opportunistically retains complete exact metadata.</summary>
public sealed class SaveTool : McpToolBase
{
    /// <summary>The exact accepted argument names.</summary>
    private static readonly IReadOnlySet<string> AllowedArguments = new HashSet<string>(StringComparer.Ordinal)
    {
        "workspaceId",
        "operationId",
        "expectedRevision",
        "expectedOutputBaselineHandle",
    };

    /// <summary>The closed guarded-save input schema.</summary>
    private static readonly JsonElement InputSchema = ParseSchema(
        "{\"type\":\"object\",\"properties\":{\"workspaceId\":{\"type\":\"string\",\"format\":\"uuid\"},\"operationId\":{\"type\":\"string\",\"format\":\"uuid\"},\"expectedRevision\":" + McpToolSchema.Revision + ",\"expectedOutputBaselineHandle\":{\"type\":\"string\",\"minLength\":1,\"maxLength\":128}},\"required\":[\"workspaceId\",\"operationId\",\"expectedRevision\",\"expectedOutputBaselineHandle\"],\"additionalProperties\":false}");

    /// <summary>The closed bounded save-outcome schema.</summary>
    private static readonly JsonElement OutputSchema = McpToolSchema.Output(
        "{\"type\":\"object\",\"properties\":{\"workspaceId\":{\"type\":\"string\",\"format\":\"uuid\"},\"operationId\":{\"type\":\"string\",\"format\":\"uuid\"},\"baseRevision\":" + McpToolSchema.Revision + ",\"resultRevision\":" + McpToolSchema.Revision + ",\"status\":{\"type\":\"string\",\"enum\":[\"committed\",\"not_committed\",\"commit_outcome_unknown\",\"committed_but_reopen_failed\"]},\"committedBaseline\":" + McpSaveToolSchema.NullableMetadataReference + ",\"output\":" + McpSaveToolSchema.NullableMetadataReference + ",\"resolvedEvidence\":" + McpSaveToolSchema.NullableMetadataReference + ",\"recoveryEvidenceToken\":" + McpToolSchema.NullableString + ",\"errorCode\":" + McpToolSchema.NullableString + ",\"warningCount\":{\"type\":\"integer\",\"minimum\":0},\"details\":" + McpSaveToolSchema.NullableMetadataReference + ",\"detailsUnavailable\":{\"type\":\"boolean\"}},\"required\":[\"workspaceId\",\"operationId\",\"baseRevision\",\"resultRevision\",\"status\",\"committedBaseline\",\"output\",\"resolvedEvidence\",\"recoveryEvidenceToken\",\"errorCode\",\"warningCount\",\"details\",\"detailsUnavailable\"],\"additionalProperties\":false}");

    /// <summary>The host-owned workspace registry.</summary>
    private readonly McpWorkspaceRegistry WorkspaceRegistry;

    /// <summary>The host-owned immutable metadata store.</summary>
    private readonly McpMetadataStore MetadataStore;

    /// <summary>Initializes the guarded-save tool.</summary>
    /// <param name="workspaceRegistry">The registry that borrows active workspaces safely.</param>
    /// <param name="metadataStore">The store that reserves and retains exact save metadata.</param>
    internal SaveTool(McpWorkspaceRegistry workspaceRegistry, McpMetadataStore metadataStore)
    {
        ArgumentNullException.ThrowIfNull(workspaceRegistry);
        ArgumentNullException.ThrowIfNull(metadataStore);
        WorkspaceRegistry = workspaceRegistry;
        MetadataStore = metadataStore;
    }

    /// <summary>Gets the closed idempotent save descriptor.</summary>
    public override Tool ProtocolTool { get; } = new Tool
    {
        Name = "creationsforge_save",
        Title = "Save a CreationsForge workspace",
        Description = "Guardedly saves the selected plugin output against an exact retained baseline and returns commitment knowledge with metadata handles.",
        InputSchema = InputSchema,
        OutputSchema = OutputSchema,
        Annotations = new ToolAnnotations
        {
            ReadOnlyHint = false,
            IdempotentHint = true,
            DestructiveHint = true,
            OpenWorldHint = false,
        },
    };

    /// <summary>Invokes the workspace save and preserves its independent status even when metadata capacity is full or the result carries an error.</summary>
    /// <param name="request">The MCP request.</param>
    /// <param name="cancellationToken">The token honored through the Core save boundary.</param>
    /// <returns>A small save outcome with exact retained metadata handles.</returns>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is observed.</exception>
    public override async ValueTask<CallToolResult> InvokeAsync(
        RequestContext<CallToolRequestParams> request,
        CancellationToken cancellationToken = default)
    {
        if (!McpInput.TryGetArguments(request, AllowedArguments, out var arguments, out var error) ||
            !McpInput.TryGetRequiredGuid(arguments, "workspaceId", out var workspaceId, out error) ||
            !McpInput.TryGetRequiredGuid(arguments, "operationId", out var operationId, out error) ||
            !McpSaveInput.TryGetRequiredRevision(arguments, "expectedRevision", out var expectedRevision, out error) ||
            !McpInput.TryGetRequiredString(arguments, "expectedOutputBaselineHandle", McpSaveInput.MaximumMetadataHandleLength, out var baselineHandle, out error))
        {
            return Error("invalid_arguments", error);
        }

        if (!MetadataStore.TryResolveOutputBaseline(baselineHandle, workspaceId, out var expectedBaseline))
        {
            return Error("invalid_metadata_handle", "The expected output baseline handle is unknown, has the wrong kind, or was not issued for this workspace.");
        }

        var engineRequest = new SaveRequest(operationId, expectedRevision, expectedBaseline);
        var carrier = await WorkspaceRegistry.ExecuteAsync(
            workspaceId,
            async (workspace, token) =>
            {
                var save = await workspace.SaveAsync(engineRequest, token).ConfigureAwait(false);
                var metadataAvailable = MetadataStore.TryPublishSaveMetadata(
                    workspaceId,
                    save,
                    out var references);
                return EngineResult<SaveInvocationOutcome>.Success(
                    new SaveInvocationOutcome(save, references, metadataAvailable),
                    workspaceId: save.WorkspaceId,
                    operationId: save.OperationId,
                    baseRevision: save.BaseRevision,
                    resultRevision: save.ResultRevision,
                    warnings: save.Warnings);
            },
            cancellationToken).ConfigureAwait(false);
        if (!carrier.Succeeded)
        {
            return EngineFailure(carrier);
        }

        var invocation = carrier.Value;
        if (invocation is null)
        {
            return Error("unexpected_failure", "The workspace save completed without a save outcome.");
        }

        var saveResult = invocation.Result;
        var references = invocation.References;
        var projected = McpProjection.Json(new
        {
            workspaceId = saveResult.WorkspaceId.ToString("D"),
            operationId = saveResult.OperationId.ToString("D"),
            baseRevision = McpProjection.Revision(saveResult.BaseRevision),
            resultRevision = McpProjection.Revision(saveResult.ResultRevision),
            status = McpMetadataProjection.SaveStatus(saveResult.Status),
            committedBaseline = references.CommittedBaseline is null ? null : McpMetadataProjection.Reference(references.CommittedBaseline),
            output = references.Output is null ? null : McpMetadataProjection.Reference(references.Output),
            resolvedEvidence = references.ResolvedEvidence is null ? null : McpMetadataProjection.Reference(references.ResolvedEvidence),
            recoveryEvidenceToken = saveResult.RecoveryEvidenceToken?.Value,
            errorCode = saveResult.Error is null ? null : McpProjection.ErrorCode(saveResult.Error.Code),
            warningCount = saveResult.Warnings.Count,
            details = references.Details is null ? null : McpMetadataProjection.Reference(references.Details),
            detailsUnavailable = !invocation.MetadataAvailable,
        });
        return FitsStructuredResultBudget(projected)
            ? Success(projected, $"Save {saveResult.OperationId:D} returned {McpMetadataProjection.SaveStatus(saveResult.Status)}.")
            : Error("result_too_large", "The bounded save outcome exceeds the 64 KiB structured result budget.");
    }

    /// <summary>Carries a completed save outcome and its atomic opportunistic metadata publication through the registry borrow.</summary>
    private sealed class SaveInvocationOutcome
    {
        /// <summary>Initializes one completed save invocation.</summary>
        /// <param name="result">The complete independent Core save outcome.</param>
        /// <param name="references">The atomically published metadata reference group.</param>
        /// <param name="metadataAvailable">Whether the complete group was retained.</param>
        /// <exception cref="ArgumentNullException">Thrown when a required result or reference group is <see langword="null"/>.</exception>
        internal SaveInvocationOutcome(
            SaveResult result,
            McpSaveMetadataReferences references,
            bool metadataAvailable)
        {
            ArgumentNullException.ThrowIfNull(result);
            ArgumentNullException.ThrowIfNull(references);
            Result = result;
            References = references;
            MetadataAvailable = metadataAvailable;
        }

        /// <summary>Gets the complete independent Core save outcome.</summary>
        internal SaveResult Result { get; }

        /// <summary>Gets the complete or deliberately unavailable metadata reference group.</summary>
        internal McpSaveMetadataReferences References { get; }

        /// <summary>Gets whether every exact result reference was retained.</summary>
        internal bool MetadataAvailable { get; }
    }
}
