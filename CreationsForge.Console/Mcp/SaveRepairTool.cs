using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace CreationsForge.Console.Mcp;

/// <summary>Runs an explicit evidence-guarded repair with result capacity reserved before destination mutation.</summary>
public sealed class SaveRepairTool : McpToolBase
{
    /// <summary>The maximum distinct metadata references emitted by one fresh repair result.</summary>
    private const int RequiredPublicationSlots = 4;

    /// <summary>The exact accepted argument names.</summary>
    private static readonly IReadOnlySet<string> AllowedArguments = new HashSet<string>(StringComparer.Ordinal)
    {
        "originalWorkspaceId",
        "saveOperationId",
        "repairOperationId",
        "expectedSaveRevision",
        "output",
        "evidenceToken",
        "direction",
    };

    /// <summary>The closed explicit repair input schema.</summary>
    private static readonly JsonElement InputSchema = ParseSchema(
        "{\"type\":\"object\",\"properties\":{\"originalWorkspaceId\":{\"type\":\"string\",\"format\":\"uuid\"},\"saveOperationId\":{\"type\":\"string\",\"format\":\"uuid\"},\"repairOperationId\":{\"type\":\"string\",\"format\":\"uuid\"},\"expectedSaveRevision\":" + McpToolSchema.Revision + ",\"output\":" + McpSaveToolSchema.OutputAssociationInput + ",\"evidenceToken\":{\"type\":\"string\",\"minLength\":1,\"maxLength\":4096},\"direction\":{\"type\":\"string\",\"enum\":[\"complete_prepared\",\"restore_baseline\"]}},\"required\":[\"originalWorkspaceId\",\"saveOperationId\",\"repairOperationId\",\"expectedSaveRevision\",\"output\",\"evidenceToken\",\"direction\"],\"additionalProperties\":false}");

    /// <summary>The closed bounded repair-outcome schema.</summary>
    private static readonly JsonElement OutputSchema = McpToolSchema.Output(
        "{\"type\":\"object\",\"properties\":{\"originalWorkspaceId\":{\"type\":\"string\",\"format\":\"uuid\"},\"saveOperationId\":{\"type\":\"string\",\"format\":\"uuid\"},\"repairOperationId\":{\"type\":\"string\",\"format\":\"uuid\"},\"status\":{\"type\":\"string\",\"enum\":[\"prepared_set_completed\",\"baseline_restored\",\"blocked_by_external_change\",\"not_started\",\"still_unknown\"]},\"resultingBaseline\":" + McpSaveToolSchema.NullableMetadataReference + ",\"output\":" + McpSaveToolSchema.NullableMetadataReference + ",\"resolvedEvidence\":" + McpSaveToolSchema.NullableMetadataReference + ",\"evidenceToken\":" + McpToolSchema.NullableString + ",\"errorCode\":" + McpToolSchema.NullableString + ",\"details\":" + McpSaveToolSchema.NullableMetadataReference + ",\"detailsUnavailable\":{\"type\":\"boolean\"}},\"required\":[\"originalWorkspaceId\",\"saveOperationId\",\"repairOperationId\",\"status\",\"resultingBaseline\",\"output\",\"resolvedEvidence\",\"evidenceToken\",\"errorCode\",\"details\",\"detailsUnavailable\"],\"additionalProperties\":false}");

    /// <summary>The Core coordinator that owns repair journals and mutation.</summary>
    private readonly IWorkspaceSaveCoordinator SaveCoordinator;

    /// <summary>The host-owned immutable metadata store.</summary>
    private readonly McpMetadataStore MetadataStore;

    /// <summary>Initializes the explicit save-repair tool.</summary>
    /// <param name="saveCoordinator">The Core coordinator that validates and applies repairs.</param>
    /// <param name="metadataStore">The store that pre-reserves and retains exact repair results.</param>
    internal SaveRepairTool(IWorkspaceSaveCoordinator saveCoordinator, McpMetadataStore metadataStore)
    {
        ArgumentNullException.ThrowIfNull(saveCoordinator);
        ArgumentNullException.ThrowIfNull(metadataStore);
        SaveCoordinator = saveCoordinator;
        MetadataStore = metadataStore;
    }

    /// <summary>Gets the closed idempotent repair descriptor.</summary>
    public override Tool ProtocolTool { get; } = new Tool
    {
        Name = "creationsforge_save_repair",
        Title = "Repair a CreationsForge save",
        Description = "Explicitly completes or restores a recognized incomplete save after rechecking the caller-reviewed evidence token.",
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

    /// <summary>Reserves four fresh result slots before invoking the coordinator repair, including replay observations.</summary>
    /// <param name="request">The MCP request.</param>
    /// <param name="cancellationToken">The token honored before and through the repair mutation boundary.</param>
    /// <returns>A bounded repair outcome with complete details retained behind handles.</returns>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is observed.</exception>
    public override async ValueTask<CallToolResult> InvokeAsync(
        RequestContext<CallToolRequestParams> request,
        CancellationToken cancellationToken = default)
    {
        if (!McpInput.TryGetArguments(request, AllowedArguments, out var arguments, out var error) ||
            !McpInput.TryGetRequiredGuid(arguments, "originalWorkspaceId", out var workspaceId, out error) ||
            !McpInput.TryGetRequiredGuid(arguments, "saveOperationId", out var saveOperationId, out error) ||
            !McpInput.TryGetRequiredGuid(arguments, "repairOperationId", out var repairOperationId, out error) ||
            !McpSaveInput.TryGetRequiredRevision(arguments, "expectedSaveRevision", out var expectedSaveRevision, out error) ||
            !McpSaveInput.TryGetOutputAssociation(arguments, "output", out var output, out error) ||
            !McpInput.TryGetRequiredString(arguments, "evidenceToken", McpSaveInput.MaximumEvidenceTokenLength, out var evidenceToken, out error) ||
            !McpSaveInput.TryGetRepairDirection(arguments, out var direction, out error))
        {
            return Error("invalid_arguments", error);
        }

        RepairSaveRequest engineRequest;
        try
        {
            engineRequest = new RepairSaveRequest(
                workspaceId,
                saveOperationId,
                repairOperationId,
                expectedSaveRevision,
                output,
                new RecoveryEvidenceToken(evidenceToken),
                direction);
        }
        catch (ArgumentException exception)
        {
            return Error("invalid_arguments", exception.Message);
        }

        await using var lease = await MetadataStore.AcquireOperationAsync(
            workspaceId,
            repairOperationId,
            McpMetadataOperationKind.Repair,
            RequiredPublicationSlots,
            cancellationToken).ConfigureAwait(false);
        if (lease is null)
        {
            return Error("metadata_capacity_exceeded", "The host cannot reserve exact repair-result handles before destination mutation; restart the MCP host and recover again before retrying the persisted repair request.");
        }

        var result = await SaveCoordinator.RepairAsync(engineRequest, cancellationToken).ConfigureAwait(false);
        var baselineReference = result.ResultingBaseline is null ? null : lease.Publish(result.ResultingBaseline);
        var evidenceReference = result.ResolvedEvidence is null ? null : lease.Publish(result.ResolvedEvidence);
        var outputReference = result.ResolvedEvidence is null ? null : lease.Publish(result.ResolvedEvidence.Output);
        var detailsReference = lease.Publish(result);
        var publicationFailed = detailsReference is null ||
            (result.ResultingBaseline is not null && baselineReference is null) ||
            (result.ResolvedEvidence is not null && (evidenceReference is null || outputReference is null));

        var projected = McpProjection.Json(new
        {
            originalWorkspaceId = result.WorkspaceId.ToString("D"),
            saveOperationId = result.SaveOperationId.ToString("D"),
            repairOperationId = result.RepairOperationId.ToString("D"),
            status = McpMetadataProjection.RepairStatus(result.Status),
            resultingBaseline = baselineReference is null ? null : McpMetadataProjection.Reference(baselineReference),
            output = outputReference is null ? null : McpMetadataProjection.Reference(outputReference),
            resolvedEvidence = evidenceReference is null ? null : McpMetadataProjection.Reference(evidenceReference),
            evidenceToken = result.EvidenceToken?.Value,
            errorCode = result.Error is null ? null : McpProjection.ErrorCode(result.Error.Code),
            details = detailsReference is null ? null : McpMetadataProjection.Reference(detailsReference),
            detailsUnavailable = publicationFailed,
        });
        return FitsStructuredResultBudget(projected)
            ? Success(projected, $"Repair {result.RepairOperationId:D} returned {McpMetadataProjection.RepairStatus(result.Status)}.")
            : Error("result_too_large", "The bounded repair outcome exceeds the 64 KiB structured result budget.");
    }
}
