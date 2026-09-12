using System.Text.Json;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace CreationsForge.Console.Mcp;

/// <summary>Reads exact retained metadata through bounded immutable JSON Pointer pages.</summary>
public sealed class MetadataReadTool : McpToolBase
{
    /// <summary>The exact accepted argument names.</summary>
    private static readonly IReadOnlySet<string> AllowedArguments = new HashSet<string>(StringComparer.Ordinal)
    {
        "metadataHandle",
        "path",
        "maxResults",
        "cursor",
    };

    /// <summary>The closed bounded metadata-page input schema.</summary>
    private static readonly JsonElement InputSchema = ParseSchema(
        """{"type":"object","properties":{"metadataHandle":{"type":"string","minLength":1,"maxLength":128},"path":{"type":"string","maxLength":4096},"maxResults":{"type":"integer","minimum":1,"maximum":250},"cursor":{"type":"string","minLength":1,"maxLength":16384}},"required":["metadataHandle"],"additionalProperties":false}""");

    /// <summary>The closed metadata-page output schema.</summary>
    private static readonly JsonElement OutputSchema = McpToolSchema.Output(
        "{\"type\":\"object\",\"properties\":{\"metadata\":" + McpSaveToolSchema.MetadataReference + ",\"page\":" + McpToolSchema.JsonPage + "},\"required\":[\"metadata\",\"page\"],\"additionalProperties\":false}");

    /// <summary>The host-owned immutable metadata store.</summary>
    private readonly McpMetadataStore MetadataStore;

    /// <summary>Initializes the bounded metadata reader.</summary>
    /// <param name="metadataStore">The store retaining exact immutable Core objects.</param>
    internal MetadataReadTool(McpMetadataStore metadataStore)
    {
        ArgumentNullException.ThrowIfNull(metadataStore);
        MetadataStore = metadataStore;
    }

    /// <summary>Gets the closed read-only metadata descriptor.</summary>
    public override Tool ProtocolTool { get; } = new Tool
    {
        Name = "creationsforge_metadata_read",
        Title = "Read retained CreationsForge metadata",
        Description = "Reads exact host-retained save, output, baseline, or recovery metadata through bounded JSON Pointer pages.",
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

    /// <summary>Projects the retained object transiently and pages it independently of workspace revision.</summary>
    /// <param name="request">The MCP request.</param>
    /// <param name="cancellationToken">The token checked before metadata projection.</param>
    /// <returns>One bounded metadata page.</returns>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is observed.</exception>
    public override ValueTask<CallToolResult> InvokeAsync(
        RequestContext<CallToolRequestParams> request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!McpInput.TryGetArguments(request, AllowedArguments, out var arguments, out var error) ||
            !McpInput.TryGetRequiredString(arguments, "metadataHandle", McpSaveInput.MaximumMetadataHandleLength, out var handle, out error) ||
            !McpInput.TryGetOptionalString(arguments, "path", McpInput.MaximumJsonPointerLength, string.Empty, out var path, out error) ||
            !McpInput.TryGetMaximumResults(arguments, out var maximumResults, out error) ||
            !McpInput.TryGetOptionalString(arguments, "cursor", McpInput.MaximumCursorLength, null, out var cursor, out error) ||
            !McpJsonPager.TryValidatePath(path!, out error))
        {
            return ValueTask.FromResult(Error("invalid_arguments", error));
        }

        if (!MetadataStore.TryResolve(handle, out var reference, out var value))
        {
            return ValueTask.FromResult(Error("invalid_metadata_handle", "The metadata handle is unknown, expired, or belongs to another host."));
        }

        var position = 0;
        if (cursor is not null && !McpMetadataCursor.TryDecode(cursor, handle, path!, maximumResults, out position))
        {
            return ValueTask.FromResult(Error("invalid_cursor", "The metadata cursor is invalid or does not match the immutable handle, path, and page policy."));
        }

        var projectedValue = McpMetadataProjection.Value(value);
        for (var count = maximumResults; count >= 1; count--)
        {
            if (!McpJsonPager.TryCreatePage(
                    projectedValue,
                    path!,
                    position,
                    count,
                    next => McpMetadataCursor.Encode(handle, path!, maximumResults, next),
                    out var page,
                    out error))
            {
                return ValueTask.FromResult(Error(cursor is null ? "invalid_path" : "invalid_cursor", error));
            }

            var projected = McpProjection.Json(new
            {
                metadata = McpMetadataProjection.Reference(reference),
                page,
            });
            if (FitsStructuredResultBudget(projected))
            {
                return ValueTask.FromResult(Success(projected, $"Read {McpMetadataProjection.Kind(reference.Kind)} metadata at JSON Pointer '{path}'."));
            }
        }

        return ValueTask.FromResult(Error("result_too_large", "A single metadata child or string chunk exceeds the 64 KiB structured result budget; select a narrower path."));
    }
}
