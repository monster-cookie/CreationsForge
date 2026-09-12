using System.Globalization;
using System.Text.Json;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace CreationsForge.Console.Mcp;

/// <summary>
/// Reads one exact native FormList context and exposes its detached JSON through stateless pointer pages.
/// </summary>
public sealed class FormListInspectTool : McpToolBase
{
    /// <summary>The inspect cursor section identifier.</summary>
    private const string Section = "record";

    /// <summary>The exact accepted argument names.</summary>
    private static readonly IReadOnlySet<string> AllowedArguments = new HashSet<string>(StringComparer.Ordinal)
    {
        "workspaceId",
        "selection",
        "expectedRevision",
        "path",
        "maxResults",
        "cursor",
    };

    /// <summary>The closed contextual read and paging input schema.</summary>
    private static readonly JsonElement InputSchema = ParseSchema(
        "{\"type\":\"object\",\"properties\":{\"workspaceId\":{\"type\":\"string\",\"format\":\"uuid\"},\"selection\":{\"type\":\"object\",\"properties\":{\"formKey\":{\"type\":\"string\",\"minLength\":1,\"maxLength\":1024},\"scope\":{\"type\":\"string\",\"enum\":[\"winning_overrides\",\"all_contexts\",\"source\",\"staged_output\"]},\"containingModKey\":{\"type\":\"string\",\"minLength\":1,\"maxLength\":1024}},\"required\":[\"formKey\",\"scope\"],\"additionalProperties\":false},\"expectedRevision\":" + McpToolSchema.Revision + ",\"path\":{\"type\":\"string\",\"maxLength\":4096},\"maxResults\":{\"type\":\"integer\",\"minimum\":1,\"maximum\":250},\"cursor\":{\"type\":\"string\",\"minLength\":1,\"maxLength\":16384}},\"required\":[\"workspaceId\",\"selection\"],\"additionalProperties\":false}");

    /// <summary>The closed contextual page output schema.</summary>
    private static readonly JsonElement OutputSchema = McpToolSchema.Output(
        "{\"type\":\"object\",\"properties\":{\"workspaceId\":{\"type\":\"string\",\"format\":\"uuid\"},\"revision\":" + McpToolSchema.Revision + ",\"context\":" + McpToolSchema.Context + ",\"page\":" + McpToolSchema.JsonPage + ",\"warnings\":{\"type\":\"array\",\"items\":" + McpToolSchema.Warning + "}},\"required\":[\"workspaceId\",\"revision\",\"context\",\"page\",\"warnings\"],\"additionalProperties\":false}");

    /// <summary>The host-owned workspace registry.</summary>
    private readonly McpWorkspaceRegistry WorkspaceRegistry;

    /// <summary>Initializes the bounded FormList inspection tool.</summary>
    /// <param name="workspaceRegistry">The registry that owns active engine workspaces.</param>
    public FormListInspectTool(McpWorkspaceRegistry workspaceRegistry)
    {
        ArgumentNullException.ThrowIfNull(workspaceRegistry);
        WorkspaceRegistry = workspaceRegistry;
    }

    /// <summary>Gets the closed read-only FormList inspection descriptor.</summary>
    public override Tool ProtocolTool { get; } = new Tool
    {
        Name = "creationsforge_formlist_inspect",
        Title = "Inspect a native FormList",
        Description = "Re-reads one exact native FormList context and pages its detached typed JSON by immediate JSON Pointer children or Unicode-safe scalar chunks.",
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

    /// <summary>Validates the selection, performs a fresh engine read, and only then validates paging bindings.</summary>
    /// <param name="request">The MCP request.</param>
    /// <param name="cancellationToken">The token propagated through native read and typed inspection.</param>
    /// <returns>One bounded contextual JSON page.</returns>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is observed.</exception>
    public override async ValueTask<CallToolResult> InvokeAsync(
        RequestContext<CallToolRequestParams> request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!McpInput.TryGetArguments(request, AllowedArguments, out var arguments, out var error) ||
            !McpInput.TryGetRequiredGuid(arguments, "workspaceId", out var workspaceId, out error) ||
            !McpInput.TryGetNestedReferenceRequest(arguments, "selection", out var selection, out error) ||
            !McpInput.TryGetOptionalRevision(arguments, out var expectedRevision, out error) ||
            !McpInput.TryGetOptionalString(arguments, "path", McpInput.MaximumJsonPointerLength, string.Empty, out var path, out error) ||
            !McpInput.TryGetMaximumResults(arguments, out var maximumResults, out error) ||
            !McpInput.TryGetOptionalString(arguments, "cursor", McpInput.MaximumCursorLength, null, out var cursor, out error) ||
            (cursor is not null && string.IsNullOrWhiteSpace(cursor)) ||
            !McpJsonPager.TryValidatePath(path!, out error))
        {
            return Error("invalid_arguments", string.IsNullOrEmpty(error) ? "Argument 'cursor' cannot be empty." : error);
        }

        var engineResult = await WorkspaceRegistry.ExecuteAsync(
            workspaceId,
            (workspace, token) => workspace.ReadFormListViewAsync(selection, token),
            cancellationToken).ConfigureAwait(false);
        if (!engineResult.Succeeded)
        {
            return EngineFailure(engineResult);
        }

        if (engineResult.Value is null ||
            !McpProjection.TryGetResultRevision(engineResult, out var revision, out error))
        {
            return Error("unexpected_failure", engineResult.Value is null
                ? "The engine reported success without a contextual FormList view."
                : error);
        }

        if (expectedRevision.HasValue && expectedRevision.Value != revision)
        {
            return Error("revision_conflict", "The expected revision does not match the fresh FormList read result revision.");
        }

        var expectedBaseline = expectedRevision?.BaselineId.ToString("D") ?? string.Empty;
        var expectedSequence = expectedRevision?.Sequence.ToString(CultureInfo.InvariantCulture) ?? string.Empty;
        var fingerprint = McpPageCursor.CreateFingerprint(
            workspaceId.ToString("D"),
            selection.FormKey.ToString(),
            McpProjection.Scope(selection.Scope),
            selection.ContainingModKey?.ToString(),
            path,
            maximumResults.ToString(CultureInfo.InvariantCulture),
            expectedBaseline,
            expectedSequence);
        var position = 0;
        if (cursor is not null && !McpPageCursor.TryDecode(
                cursor,
                workspaceId,
                revision,
                fingerprint,
                path!,
                Section,
                McpJsonPager.Policy,
                out position))
        {
            return Error("invalid_cursor", "The inspection cursor is invalid or does not match the fresh workspace revision and normalized request.");
        }

        for (var count = maximumResults; count >= 1; count--)
        {
            if (!McpJsonPager.TryCreatePage(
                    engineResult.Value.Record,
                    path!,
                    position,
                    count,
                    nextPosition => McpPageCursor.Encode(
                        workspaceId,
                        revision,
                        fingerprint,
                        path!,
                        Section,
                        McpJsonPager.Policy,
                        nextPosition),
                    out var page,
                    out error))
            {
                return Error(cursor is null ? "invalid_arguments" : "invalid_cursor", error);
            }

            var projected = McpProjection.Json(new
            {
                workspaceId = workspaceId.ToString("D"),
                revision = McpProjection.Revision(revision),
                context = McpProjection.Context(engineResult.Value.Context),
                page,
                warnings = engineResult.Warnings.Select(McpProjection.Warning).ToArray(),
            });
            if (FitsStructuredResultBudget(projected))
            {
                return Success(
                    projected,
                    $"Returned one bounded FormList inspection page with status {McpProjection.ResolutionStatus(engineResult.Value.Context.Status)}.");
            }
        }

        return Error("result_too_large", "The selected scalar, one child descriptor, or operation metadata exceeds the 64 KiB structured result budget.");
    }
}
