using System.Globalization;
using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace CreationsForge.Mcp;

/// <summary>
/// Reads one exact major-record context and pages its detached fields or warnings independently.
/// </summary>
public sealed class MajorRecordInspectTool : McpToolBase
{
    /// <summary>The exact accepted argument names.</summary>
    private static readonly IReadOnlySet<string> AllowedArguments = new HashSet<string>(StringComparer.Ordinal)
    {
        "workspaceId",
        "selection",
        "section",
        "expectedRevision",
        "path",
        "maxResults",
        "cursor",
    };

    /// <summary>The closed contextual read and paging input schema.</summary>
    private static readonly JsonElement InputSchema = ParseSchema(
        "{\"type\":\"object\",\"properties\":{\"workspaceId\":{\"type\":\"string\",\"format\":\"uuid\"},\"selection\":{\"type\":\"object\",\"properties\":{\"formKey\":{\"type\":\"string\",\"minLength\":1,\"maxLength\":1024},\"scope\":{\"type\":\"string\",\"enum\":[\"winning_overrides\",\"all_contexts\",\"source\",\"staged_output\"]},\"containingModKey\":{\"type\":\"string\",\"minLength\":1,\"maxLength\":1024}},\"required\":[\"formKey\",\"scope\"],\"additionalProperties\":false},\"section\":{\"type\":\"string\",\"enum\":[\"record\",\"warnings\"]},\"expectedRevision\":" + McpToolSchema.Revision + ",\"path\":{\"type\":\"string\",\"maxLength\":4096},\"maxResults\":{\"type\":\"integer\",\"minimum\":1,\"maximum\":250},\"cursor\":{\"type\":\"string\",\"minLength\":1,\"maxLength\":16384}},\"required\":[\"workspaceId\",\"selection\"],\"additionalProperties\":false}");

    /// <summary>The closed warning page schema.</summary>
    private const string WarningsPageSchema = "{\"type\":\"object\",\"properties\":{\"offset\":{\"type\":\"integer\",\"minimum\":0},\"count\":{\"type\":\"integer\",\"minimum\":0},\"totalCount\":{\"type\":\"integer\",\"minimum\":0},\"warnings\":{\"type\":\"array\",\"items\":" + McpToolSchema.Warning + "},\"cursor\":" + McpToolSchema.NullableString + "},\"required\":[\"offset\",\"count\",\"totalCount\",\"warnings\",\"cursor\"],\"additionalProperties\":false}";

    /// <summary>The closed contextual page output schema.</summary>
    private static readonly JsonElement OutputSchema = McpToolSchema.Output(
        "{\"type\":\"object\",\"properties\":{\"workspaceId\":{\"type\":\"string\",\"format\":\"uuid\"},\"revision\":" + McpToolSchema.Revision + ",\"context\":" + McpToolSchema.Context + ",\"recordType\":" + McpToolSchema.NullableString + ",\"section\":{\"type\":\"string\",\"enum\":[\"record\",\"warnings\"]},\"page\":{\"oneOf\":[" + McpToolSchema.JsonPage + "," + WarningsPageSchema + "]}},\"required\":[\"workspaceId\",\"revision\",\"context\",\"recordType\",\"section\",\"page\"],\"additionalProperties\":false}");

    /// <summary>The host-owned workspace registry.</summary>
    private readonly McpWorkspaceRegistry WorkspaceRegistry;

    /// <summary>Initializes the bounded major-record inspection tool.</summary>
    /// <param name="workspaceRegistry">The registry that owns active engine workspaces.</param>
    public MajorRecordInspectTool(McpWorkspaceRegistry workspaceRegistry)
    {
        ArgumentNullException.ThrowIfNull(workspaceRegistry);
        WorkspaceRegistry = workspaceRegistry;
    }

    /// <summary>Gets the closed read-only major-record inspection descriptor.</summary>
    public override Tool ProtocolTool { get; } = new Tool
    {
        Name = "creationsforge_record_inspect",
        Title = "Inspect a major record",
        Description = "Re-reads one exact major-record context and pages typed fields or warnings independently; the record section defaults when section is omitted.",
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
    /// <param name="cancellationToken">The token propagated through engine read and typed inspection.</param>
    /// <returns>One bounded contextual JSON page.</returns>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is observed.</exception>
    public override async ValueTask<CallToolResult> InvokeAsync(
        RequestContext<CallToolRequestParams> request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string? section = "record";
        if (!McpInput.TryGetArguments(request, AllowedArguments, out var arguments, out var error) ||
            !McpInput.TryGetRequiredGuid(arguments, "workspaceId", out var workspaceId, out error) ||
            !McpInput.TryGetNestedReferenceRequest(arguments, "selection", out var selection, out error) ||
            !McpInput.TryGetOptionalString(arguments, "section", 16, "record", out section, out error) ||
            section is not "record" and not "warnings" ||
            !McpInput.TryGetOptionalRevision(arguments, out var expectedRevision, out error) ||
            !McpInput.TryGetOptionalString(arguments, "path", McpInput.MaximumJsonPointerLength, string.Empty, out var path, out error) ||
            !McpInput.TryGetMaximumResults(arguments, out var maximumResults, out error) ||
            !McpInput.TryGetOptionalString(arguments, "cursor", McpInput.MaximumCursorLength, null, out var cursor, out error) ||
            (cursor is not null && string.IsNullOrWhiteSpace(cursor)) ||
            !McpJsonPager.TryValidatePath(path!, out error) ||
            (section == "warnings" && arguments.ContainsKey("path")))
        {
            return Error("invalid_arguments", string.IsNullOrEmpty(error)
                ? section == "warnings" && arguments.ContainsKey("path")
                    ? "Argument 'path' is accepted only for the record section."
                    : "Argument 'section' or 'cursor' is invalid."
                : error);
        }

        var engineResult = await WorkspaceRegistry.ExecuteAsync(
            workspaceId,
            (workspace, token) => workspace.ReadMajorRecordViewAsync(selection, token),
            cancellationToken).ConfigureAwait(false);
        if (!engineResult.Succeeded)
        {
            return EngineFailure(engineResult);
        }

        if (engineResult.Value is null ||
            !McpProjection.TryGetResultRevision(engineResult, out var revision, out error))
        {
            return Error("unexpected_failure", engineResult.Value is null
                ? "The engine reported success without a contextual major-record view."
                : error);
        }

        if (expectedRevision.HasValue && expectedRevision.Value != revision)
        {
            return Error("revision_conflict", "The expected revision does not match the fresh major-record read result revision.");
        }

        var expectedBaseline = expectedRevision?.BaselineId.ToString("D") ?? string.Empty;
        var expectedSequence = expectedRevision?.Sequence.ToString(CultureInfo.InvariantCulture) ?? string.Empty;
        var fingerprint = McpPageCursor.CreateFingerprint(
            workspaceId.ToString("D"),
            selection.FormKey.ToString(),
            McpProjection.Scope(selection.Scope),
            selection.ContainingModKey?.ToString(),
            section,
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
                section!,
                McpJsonPager.Policy,
                out position))
        {
            return Error("invalid_cursor", "The inspection cursor is invalid or does not match the fresh workspace revision and normalized request.");
        }

        for (var count = maximumResults; count >= 1; count--)
        {
            if (!TryCreateSectionPage(
                    engineResult.Value,
                    engineResult.Warnings,
                    workspaceId,
                    revision,
                    fingerprint,
                    section!,
                    path!,
                    position,
                    count,
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
                recordType = engineResult.Value.RecordType,
                section,
                page,
            });
            if (FitsStructuredResultBudget(projected))
            {
                return Success(
                    projected,
                    $"Returned one bounded major-record {section} page with status {McpProjection.ResolutionStatus(engineResult.Value.Context.Status)}.");
            }
        }

        return Error("result_too_large", "The selected scalar, one child descriptor, one warning, or context metadata exceeds the 64 KiB structured result budget.");
    }

    /// <summary>Creates one independently paged field-tree or warning section.</summary>
    /// <param name="view">The fresh detached major-record read view.</param>
    /// <param name="warnings">The fresh ordered engine warnings.</param>
    /// <param name="workspaceId">The owning workspace.</param>
    /// <param name="revision">The exact fresh result revision.</param>
    /// <param name="fingerprint">The normalized request fingerprint.</param>
    /// <param name="section">The requested record or warnings section.</param>
    /// <param name="path">The record pointer or empty warning path.</param>
    /// <param name="position">The next page offset.</param>
    /// <param name="maximumResults">The candidate page size.</param>
    /// <param name="page">Receives the section page.</param>
    /// <param name="error">Receives a paging error.</param>
    /// <returns><see langword="true"/> when the page position is valid.</returns>
    private static bool TryCreateSectionPage(
        MajorRecordReadView view,
        IReadOnlyList<EngineWarning> warnings,
        Guid workspaceId,
        WorkspaceRevision revision,
        string fingerprint,
        string section,
        string path,
        int position,
        int maximumResults,
        out JsonElement page,
        out string error)
    {
        if (section == "record")
        {
            return McpJsonPager.TryCreatePage(
                view.Record,
                path,
                position,
                maximumResults,
                nextPosition => McpPageCursor.Encode(
                    workspaceId, revision, fingerprint, path, section, McpJsonPager.Policy, nextPosition),
                out page,
                out error);
        }

        if (position > warnings.Count)
        {
            page = default;
            error = "The inspection cursor position is outside the fresh warning array.";
            return false;
        }

        var items = warnings.Skip(position).Take(maximumResults).Select(McpProjection.Warning).ToArray();
        var nextPosition = position + items.Length;
        page = McpProjection.Json(new
        {
            offset = position,
            count = items.Length,
            totalCount = warnings.Count,
            warnings = items,
            cursor = nextPosition < warnings.Count
                ? McpPageCursor.Encode(workspaceId, revision, fingerprint, string.Empty, section, McpJsonPager.Policy, nextPosition)
                : null,
        });
        error = string.Empty;
        return true;
    }
}
