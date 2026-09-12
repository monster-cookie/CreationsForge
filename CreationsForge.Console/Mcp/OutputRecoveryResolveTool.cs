using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace CreationsForge.Console.Mcp;

/// <summary>Adopts exact terminal save evidence into a live workspace after Core revalidation.</summary>
public sealed class OutputRecoveryResolveTool : McpToolBase
{
    /// <summary>The two exact output references returned by successful adoption.</summary>
    private const int RequiredPublicationSlots = 2;

    /// <summary>The exact accepted argument names.</summary>
    private static readonly IReadOnlySet<string> AllowedArguments = new HashSet<string>(StringComparer.Ordinal)
    {
        "workspaceId",
        "operationId",
        "expectedRevision",
        "mode",
        "resolvedEvidenceHandle",
    };

    /// <summary>The closed recovery-adoption input schema.</summary>
    private static readonly JsonElement InputSchema = ParseSchema(
        "{\"type\":\"object\",\"properties\":{\"workspaceId\":{\"type\":\"string\",\"format\":\"uuid\"},\"operationId\":{\"type\":\"string\",\"format\":\"uuid\"},\"expectedRevision\":" + McpToolSchema.Revision + ",\"mode\":{\"type\":\"string\",\"enum\":[\"resume_staged_after_not_committed\",\"reopen_resolved_output\"]},\"resolvedEvidenceHandle\":{\"type\":\"string\",\"minLength\":1,\"maxLength\":128}},\"required\":[\"workspaceId\",\"operationId\",\"expectedRevision\",\"mode\",\"resolvedEvidenceHandle\"],\"additionalProperties\":false}");

    /// <summary>The closed bounded recovery-adoption receipt schema.</summary>
    private static readonly JsonElement OutputSchema = McpToolSchema.Output(
        "{\"type\":\"object\",\"properties\":{\"workspaceId\":{\"type\":\"string\",\"format\":\"uuid\"},\"operationId\":{\"type\":\"string\",\"format\":\"uuid\"},\"mode\":{\"type\":\"string\"},\"revision\":" + McpToolSchema.Revision + ",\"output\":" + McpSaveToolSchema.NullableMetadataReference + ",\"outputBaseline\":" + McpSaveToolSchema.NullableMetadataReference + ",\"warningCount\":{\"type\":\"integer\",\"minimum\":0},\"detailsUnavailable\":{\"type\":\"boolean\"}},\"required\":[\"workspaceId\",\"operationId\",\"mode\",\"revision\",\"output\",\"outputBaseline\",\"warningCount\",\"detailsUnavailable\"],\"additionalProperties\":false}");

    /// <summary>The host-owned workspace registry.</summary>
    private readonly McpWorkspaceRegistry WorkspaceRegistry;

    /// <summary>The host-owned immutable metadata store.</summary>
    private readonly McpMetadataStore MetadataStore;

    /// <summary>Initializes the recovery-adoption tool.</summary>
    /// <param name="workspaceRegistry">The registry that borrows the current live workspace.</param>
    /// <param name="metadataStore">The store that resolves exact evidence and retains adopted output metadata.</param>
    internal OutputRecoveryResolveTool(McpWorkspaceRegistry workspaceRegistry, McpMetadataStore metadataStore)
    {
        ArgumentNullException.ThrowIfNull(workspaceRegistry);
        ArgumentNullException.ThrowIfNull(metadataStore);
        WorkspaceRegistry = workspaceRegistry;
        MetadataStore = metadataStore;
    }

    /// <summary>Gets the closed idempotent recovery-adoption descriptor.</summary>
    public override Tool ProtocolTool { get; } = new Tool
    {
        Name = "creationsforge_output_recovery_resolve",
        Title = "Resolve CreationsForge output recovery",
        Description = "Revalidates exact terminal evidence and explicitly resumes staged state or reopens the resolved native output.",
        InputSchema = InputSchema,
        OutputSchema = OutputSchema,
        Annotations = new ToolAnnotations
        {
            ReadOnlyHint = false,
            IdempotentHint = true,
            DestructiveHint = false,
            OpenWorldHint = false,
        },
    };

    /// <summary>Resolves exact evidence before Core invocation and publishes the adopted output pair from reserved capacity.</summary>
    /// <param name="request">The MCP request.</param>
    /// <param name="cancellationToken">The token propagated through evidence revalidation and state publication.</param>
    /// <returns>A bounded adopted-output receipt.</returns>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is observed.</exception>
    public override async ValueTask<CallToolResult> InvokeAsync(
        RequestContext<CallToolRequestParams> request,
        CancellationToken cancellationToken = default)
    {
        if (!McpInput.TryGetArguments(request, AllowedArguments, out var arguments, out var error) ||
            !McpInput.TryGetRequiredGuid(arguments, "workspaceId", out var workspaceId, out error) ||
            !McpInput.TryGetRequiredGuid(arguments, "operationId", out var operationId, out error) ||
            !McpSaveInput.TryGetRequiredRevision(arguments, "expectedRevision", out var expectedRevision, out error) ||
            !McpSaveInput.TryGetAdoptionMode(arguments, out var mode, out error) ||
            !McpInput.TryGetRequiredString(arguments, "resolvedEvidenceHandle", McpSaveInput.MaximumMetadataHandleLength, out var evidenceHandle, out error))
        {
            return Error("invalid_arguments", error);
        }

        if (!MetadataStore.TryResolveResolvedEvidence(evidenceHandle, out var evidence))
        {
            return Error("invalid_metadata_handle", "The resolved evidence handle is unknown, expired, or has the wrong metadata kind.");
        }

        var engineRequest = new ResolveOutputRecoveryRequest(operationId, expectedRevision, mode, evidence);
        await using var lease = await MetadataStore.AcquireOperationAsync(
            workspaceId,
            operationId,
            McpMetadataOperationKind.WorkspaceMutation,
            RequiredPublicationSlots,
            cancellationToken).ConfigureAwait(false);
        if (lease is null)
        {
            return Error("metadata_capacity_exceeded", "The host cannot reserve adopted output handles before workspace recovery; the live workspace was not changed.");
        }

        var result = await WorkspaceRegistry.ExecuteAsync(
            workspaceId,
            (workspace, token) => workspace.ResolveOutputRecoveryAsync(engineRequest, token),
            cancellationToken).ConfigureAwait(false);
        if (!result.Succeeded)
        {
            return EngineFailure(result);
        }

        var receipt = result.Value;
        if (receipt is null || result.ResultRevision != receipt.Revision)
        {
            return Error("unexpected_failure", "The engine recovery-adoption receipt is absent or lacks matching contextual revision metadata.");
        }

        var outputReference = lease.Publish(receipt.Output);
        var baselineReference = lease.Publish(receipt.Baseline);
        var publicationFailed = outputReference is null || baselineReference is null;

        var projected = McpProjection.Json(new
        {
            workspaceId = workspaceId.ToString("D"),
            operationId = operationId.ToString("D"),
            mode = McpMetadataProjection.AdoptionMode(mode),
            revision = McpProjection.Revision(receipt.Revision),
            output = outputReference is null ? null : McpMetadataProjection.Reference(outputReference),
            outputBaseline = baselineReference is null ? null : McpMetadataProjection.Reference(baselineReference),
            warningCount = result.Warnings.Count,
            detailsUnavailable = publicationFailed,
        });
        return FitsStructuredResultBudget(projected)
            ? Success(projected, $"Adopted recovered output for workspace {workspaceId:D} at revision {receipt.Revision}.")
            : Error("result_too_large", "The bounded recovery-adoption receipt exceeds the 64 KiB structured result budget.");
    }
}
