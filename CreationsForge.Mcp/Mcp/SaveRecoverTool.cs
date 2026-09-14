using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace CreationsForge.Mcp;

/// <summary>Inspects durable save recovery without requiring the original workspace to remain open.</summary>
public sealed class SaveRecoverTool : McpToolBase
{
    /// <summary>The maximum distinct metadata references emitted by one fresh recovery observation.</summary>
    private const int RequiredPublicationSlots = 4;

    /// <summary>The exact accepted argument names.</summary>
    private static readonly IReadOnlySet<string> AllowedArguments = new HashSet<string>(StringComparer.Ordinal)
    {
        "originalWorkspaceId",
        "saveOperationId",
        "output",
    };

    /// <summary>The closed restart-safe recovery input schema.</summary>
    private static readonly JsonElement InputSchema = ParseSchema(
        "{\"type\":\"object\",\"properties\":{\"originalWorkspaceId\":{\"type\":\"string\",\"format\":\"uuid\"},\"saveOperationId\":{\"type\":\"string\",\"format\":\"uuid\"},\"output\":" + McpSaveToolSchema.OutputAssociationInput + "},\"required\":[\"originalWorkspaceId\",\"saveOperationId\",\"output\"],\"additionalProperties\":false}");

    /// <summary>The closed bounded recovery-outcome schema.</summary>
    private static readonly JsonElement OutputSchema = McpToolSchema.Output(
        "{\"type\":\"object\",\"properties\":{\"originalWorkspaceId\":{\"type\":\"string\",\"format\":\"uuid\"},\"saveOperationId\":{\"type\":\"string\",\"format\":\"uuid\"},\"status\":{\"type\":\"string\",\"enum\":[\"committed\",\"not_committed\",\"still_unknown\"]},\"saveBaseRevision\":" + McpSaveToolSchema.NullableRevision + ",\"repairRequired\":{\"type\":\"boolean\"},\"evidenceToken\":" + McpToolSchema.NullableString + ",\"output\":" + McpSaveToolSchema.NullableMetadataReference + ",\"resolvedBaseline\":" + McpSaveToolSchema.NullableMetadataReference + ",\"resolvedEvidence\":" + McpSaveToolSchema.NullableMetadataReference + ",\"errorCode\":" + McpToolSchema.NullableString + ",\"details\":" + McpSaveToolSchema.NullableMetadataReference + ",\"detailsUnavailable\":{\"type\":\"boolean\"}},\"required\":[\"originalWorkspaceId\",\"saveOperationId\",\"status\",\"saveBaseRevision\",\"repairRequired\",\"evidenceToken\",\"output\",\"resolvedBaseline\",\"resolvedEvidence\",\"errorCode\",\"details\",\"detailsUnavailable\"],\"additionalProperties\":false}");

    /// <summary>The Core save coordinator used independently of live registry entries.</summary>
    private readonly IWorkspaceSaveCoordinator SaveCoordinator;

    /// <summary>The host-owned immutable metadata store.</summary>
    private readonly McpMetadataStore MetadataStore;

    /// <summary>Initializes the restart-safe save-recovery tool.</summary>
    /// <param name="saveCoordinator">The Core coordinator that owns durable save journals.</param>
    /// <param name="metadataStore">The store retaining exact observed recovery metadata.</param>
    internal SaveRecoverTool(IWorkspaceSaveCoordinator saveCoordinator, McpMetadataStore metadataStore)
    {
        ArgumentNullException.ThrowIfNull(saveCoordinator);
        ArgumentNullException.ThrowIfNull(metadataStore);
        SaveCoordinator = saveCoordinator;
        MetadataStore = metadataStore;
    }

    /// <summary>Gets the closed read-only durable-recovery descriptor.</summary>
    public override Tool ProtocolTool { get; } = new Tool
    {
        Name = "creationsforge_save_recover",
        Title = "Recover a CreationsForge save outcome",
        Description = "Inspects durable save evidence using the original identities and exact inline output association, including after host restart.",
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

    /// <summary>Reserves room for a fresh observation before asking the coordinator to inspect durable state.</summary>
    /// <param name="request">The MCP request.</param>
    /// <param name="cancellationToken">The token propagated through recovery inspection.</param>
    /// <returns>A bounded outcome retaining complete details and terminal evidence.</returns>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is observed.</exception>
    public override async ValueTask<CallToolResult> InvokeAsync(
        RequestContext<CallToolRequestParams> request,
        CancellationToken cancellationToken = default)
    {
        if (!McpInput.TryGetArguments(request, AllowedArguments, out var arguments, out var error) ||
            !McpInput.TryGetRequiredGuid(arguments, "originalWorkspaceId", out var workspaceId, out error) ||
            !McpInput.TryGetRequiredGuid(arguments, "saveOperationId", out var saveOperationId, out error) ||
            !McpSaveInput.TryGetOutputAssociation(arguments, "output", out var output, out error))
        {
            return Error("invalid_arguments", error);
        }

        await using var lease = await MetadataStore.AcquireOperationAsync(
            workspaceId,
            saveOperationId,
            McpMetadataOperationKind.Recover,
            RequiredPublicationSlots,
            cancellationToken).ConfigureAwait(false);
        if (lease is null)
        {
            return Error("metadata_capacity_exceeded", "The host cannot reserve exact recovery-observation handles before coordinator inspection; restart the MCP host and retry with the same original identities and output.");
        }

        RecoverSaveResult result;
        try
        {
            result = await SaveCoordinator.RecoverAsync(
                new RecoverSaveRequest(workspaceId, saveOperationId, output),
                cancellationToken).ConfigureAwait(false);
        }
        catch (ArgumentException exception)
        {
            return Error("invalid_arguments", exception.Message);
        }

        var evidenceReference = result.ResolvedEvidence is null ? null : lease.Publish(result.ResolvedEvidence);
        var baselineReference = result.ResolvedEvidence is null ? null : lease.Publish(result.ResolvedEvidence.ResolvedOutputBaseline);
        var outputReference = result.ResolvedEvidence is null ? null : lease.Publish(result.ResolvedEvidence.Output);
        var detailsReference = lease.Publish(result);
        var publicationFailed = detailsReference is null ||
            (result.ResolvedEvidence is not null &&
                (evidenceReference is null || baselineReference is null || outputReference is null));

        var projected = McpProjection.Json(new
        {
            originalWorkspaceId = result.WorkspaceId.ToString("D"),
            saveOperationId = result.SaveOperationId.ToString("D"),
            status = McpMetadataProjection.RecoverStatus(result.Status),
            saveBaseRevision = result.SaveBaseRevision.HasValue
                ? McpProjection.Revision(result.SaveBaseRevision.Value)
                : null,
            repairRequired = result.RepairRequired,
            evidenceToken = result.EvidenceToken?.Value,
            output = outputReference is null ? null : McpMetadataProjection.Reference(outputReference),
            resolvedBaseline = baselineReference is null ? null : McpMetadataProjection.Reference(baselineReference),
            resolvedEvidence = evidenceReference is null ? null : McpMetadataProjection.Reference(evidenceReference),
            errorCode = result.Error is null ? null : McpProjection.ErrorCode(result.Error.Code),
            details = detailsReference is null ? null : McpMetadataProjection.Reference(detailsReference),
            detailsUnavailable = publicationFailed,
        });
        return FitsStructuredResultBudget(projected)
            ? Success(projected, $"Recovery inspection for save {result.SaveOperationId:D} returned {McpMetadataProjection.RecoverStatus(result.Status)}.")
            : Error("result_too_large", "The bounded recovery outcome exceeds the 64 KiB structured result budget.");
    }
}
