using System.Globalization;
using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace CreationsForge.Mcp;

/// <summary>
/// Compares two exact FormList contexts and pages typed views, semantic changes, or warnings independently.
/// </summary>
public sealed class FormListCompareTool : McpToolBase
{
    /// <summary>The exact accepted argument names.</summary>
    private static readonly IReadOnlySet<string> AllowedArguments = new HashSet<string>(StringComparer.Ordinal)
    {
        "workspaceId",
        "before",
        "after",
        "section",
        "expectedRevision",
        "path",
        "maxResults",
        "cursor",
    };

    /// <summary>The closed comparison request schema.</summary>
    private static readonly JsonElement InputSchema = ParseSchema(
        "{\"type\":\"object\",\"properties\":{\"workspaceId\":{\"type\":\"string\",\"format\":\"uuid\"},\"before\":{\"type\":\"object\",\"properties\":{\"formKey\":{\"type\":\"string\",\"minLength\":1,\"maxLength\":1024},\"scope\":{\"type\":\"string\",\"enum\":[\"winning_overrides\",\"all_contexts\",\"source\",\"staged_output\"]},\"containingModKey\":{\"type\":\"string\",\"minLength\":1,\"maxLength\":1024}},\"required\":[\"formKey\",\"scope\"],\"additionalProperties\":false},\"after\":{\"type\":\"object\",\"properties\":{\"formKey\":{\"type\":\"string\",\"minLength\":1,\"maxLength\":1024},\"scope\":{\"type\":\"string\",\"enum\":[\"winning_overrides\",\"all_contexts\",\"source\",\"staged_output\"]},\"containingModKey\":{\"type\":\"string\",\"minLength\":1,\"maxLength\":1024}},\"required\":[\"formKey\",\"scope\"],\"additionalProperties\":false},\"section\":{\"type\":\"string\",\"enum\":[\"changes\",\"before\",\"after\",\"warnings\"]},\"expectedRevision\":" + McpToolSchema.Revision + ",\"path\":{\"type\":\"string\",\"maxLength\":4096},\"maxResults\":{\"type\":\"integer\",\"minimum\":1,\"maximum\":250},\"cursor\":{\"type\":\"string\",\"minLength\":1,\"maxLength\":16384}},\"required\":[\"workspaceId\",\"before\",\"after\",\"section\"],\"additionalProperties\":false}");

    /// <summary>The closed semantic-change page schema.</summary>
    private const string ChangesPageSchema = "{\"type\":\"object\",\"properties\":{\"offset\":{\"type\":\"integer\",\"minimum\":0},\"count\":{\"type\":\"integer\",\"minimum\":0},\"totalCount\":{\"type\":\"integer\",\"minimum\":0},\"changes\":{\"type\":\"array\",\"items\":{\"type\":\"object\",\"properties\":{\"fieldIdentifier\":{\"type\":\"string\",\"minLength\":1},\"kind\":{\"type\":\"string\",\"enum\":[\"value_changed\",\"item_inserted\",\"item_removed\",\"item_changed\",\"writer_normalized\"]},\"beforePosition\":" + McpToolSchema.NullableInteger + ",\"afterPosition\":" + McpToolSchema.NullableInteger + "},\"required\":[\"fieldIdentifier\",\"kind\",\"beforePosition\",\"afterPosition\"],\"additionalProperties\":false}},\"cursor\":" + McpToolSchema.NullableString + "},\"required\":[\"offset\",\"count\",\"totalCount\",\"changes\",\"cursor\"],\"additionalProperties\":false}";

    /// <summary>The closed warning page schema.</summary>
    private const string WarningsPageSchema = "{\"type\":\"object\",\"properties\":{\"offset\":{\"type\":\"integer\",\"minimum\":0},\"count\":{\"type\":\"integer\",\"minimum\":0},\"totalCount\":{\"type\":\"integer\",\"minimum\":0},\"warnings\":{\"type\":\"array\",\"items\":" + McpToolSchema.Warning + "},\"cursor\":" + McpToolSchema.NullableString + "},\"required\":[\"offset\",\"count\",\"totalCount\",\"warnings\",\"cursor\"],\"additionalProperties\":false}";

    /// <summary>The closed comparison output schema.</summary>
    private static readonly JsonElement OutputSchema = McpToolSchema.Output(
        "{\"type\":\"object\",\"properties\":{\"workspaceId\":{\"type\":\"string\",\"format\":\"uuid\"},\"revision\":" + McpToolSchema.Revision + ",\"formKey\":{\"type\":\"string\",\"minLength\":1},\"beforeContext\":" + McpToolSchema.Context + ",\"afterContext\":" + McpToolSchema.Context + ",\"section\":{\"type\":\"string\",\"enum\":[\"changes\",\"before\",\"after\",\"warnings\"]},\"page\":{\"oneOf\":[" + McpToolSchema.JsonPage + "," + ChangesPageSchema + "," + WarningsPageSchema + "]}},\"required\":[\"workspaceId\",\"revision\",\"formKey\",\"beforeContext\",\"afterContext\",\"section\",\"page\"],\"additionalProperties\":false}");

    /// <summary>The host-owned workspace registry.</summary>
    private readonly McpWorkspaceRegistry WorkspaceRegistry;

    /// <summary>Initializes the bounded FormList comparison tool.</summary>
    /// <param name="workspaceRegistry">The registry that owns active engine workspaces.</param>
    public FormListCompareTool(McpWorkspaceRegistry workspaceRegistry)
    {
        ArgumentNullException.ThrowIfNull(workspaceRegistry);
        WorkspaceRegistry = workspaceRegistry;
    }

    /// <summary>Gets the closed read-only FormList comparison descriptor.</summary>
    public override Tool ProtocolTool { get; } = new Tool
    {
        Name = "creationsforge_formlist_compare",
        Title = "Compare FormList contexts",
        Description = "Re-reads two exact contexts and independently pages semantic changes, prior fields, resulting fields, or warnings.",
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

    /// <summary>Performs a fresh engine comparison before validating revision and cursor bindings.</summary>
    /// <param name="request">The MCP request.</param>
    /// <param name="cancellationToken">The token propagated through both engine reads and typed comparison.</param>
    /// <returns>One independently navigable comparison section page.</returns>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is observed.</exception>
    public override async ValueTask<CallToolResult> InvokeAsync(
        RequestContext<CallToolRequestParams> request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!McpInput.TryGetArguments(request, AllowedArguments, out var arguments, out var error) ||
            !McpInput.TryGetRequiredGuid(arguments, "workspaceId", out var workspaceId, out error))
        {
            return Error("invalid_arguments", error);
        }

        if (!McpInput.TryGetNestedReferenceRequest(arguments, "before", out var before, out error) ||
            !McpInput.TryGetNestedReferenceRequest(arguments, "after", out var after, out error))
        {
            return Error("invalid_arguments", error);
        }

        if (before.FormKey != after.FormKey)
        {
            return Error("invalid_arguments", "Arguments 'before' and 'after' must select the same canonical FormKey.");
        }

        if (!McpInput.TryGetRequiredString(arguments, "section", 16, out var section, out error) ||
            section is not "changes" and not "before" and not "after" and not "warnings")
        {
            return Error("invalid_arguments", string.IsNullOrEmpty(error)
                ? "Argument 'section' must be one of changes, before, after, or warnings."
                : error);
        }

        if ((section is "changes" or "warnings") && arguments.ContainsKey("path"))
        {
            return Error("invalid_arguments", "Argument 'path' is accepted only for the before and after view sections.");
        }

        if (!McpInput.TryGetOptionalRevision(arguments, out var expectedRevision, out error) ||
            !McpInput.TryGetOptionalString(arguments, "path", McpInput.MaximumJsonPointerLength, string.Empty, out var path, out error) ||
            !McpInput.TryGetMaximumResults(arguments, out var maximumResults, out error) ||
            !McpInput.TryGetOptionalString(arguments, "cursor", McpInput.MaximumCursorLength, null, out var cursor, out error) ||
            (cursor is not null && string.IsNullOrWhiteSpace(cursor)) ||
            !McpJsonPager.TryValidatePath(path!, out error))
        {
            return Error("invalid_arguments", string.IsNullOrEmpty(error) ? "Argument 'cursor' cannot be empty." : error);
        }

        var comparisonRequest = new CompareFormListRequest(before, after);
        var engineResult = await WorkspaceRegistry.ExecuteAsync(
            workspaceId,
            (workspace, token) => workspace.CompareFormListAsync(comparisonRequest, token),
            cancellationToken).ConfigureAwait(false);
        if (!engineResult.Succeeded)
        {
            return EngineFailure(engineResult);
        }

        if (engineResult.Value is null ||
            !McpProjection.TryGetResultRevision(engineResult, out var revision, out error))
        {
            return Error("unexpected_failure", engineResult.Value is null
                ? "The engine reported success without a FormList comparison."
                : error);
        }

        if (expectedRevision.HasValue && expectedRevision.Value != revision)
        {
            return Error("revision_conflict", "The expected revision does not match the fresh FormList comparison result revision.");
        }

        var expectedBaseline = expectedRevision?.BaselineId.ToString("D") ?? string.Empty;
        var expectedSequence = expectedRevision?.Sequence.ToString(CultureInfo.InvariantCulture) ?? string.Empty;
        var fingerprint = McpPageCursor.CreateFingerprint(
            workspaceId.ToString("D"),
            before.FormKey.ToString(),
            McpProjection.Scope(before.Scope),
            before.ContainingModKey?.ToString(),
            McpProjection.Scope(after.Scope),
            after.ContainingModKey?.ToString(),
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
                section,
                McpJsonPager.Policy,
                out position))
        {
            return Error("invalid_cursor", "The comparison cursor is invalid or does not match the fresh workspace revision and normalized request.");
        }

        for (var count = maximumResults; count >= 1; count--)
        {
            if (!TryCreateSectionPage(
                    engineResult.Value,
                    workspaceId,
                    revision,
                    fingerprint,
                    section,
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
                formKey = engineResult.Value.FormKey.ToString(),
                beforeContext = McpProjection.Context(engineResult.Value.BeforeContext),
                afterContext = McpProjection.Context(engineResult.Value.AfterContext),
                section,
                page,
            });
            if (FitsStructuredResultBudget(projected))
            {
                return Success(projected, $"Compared {engineResult.Value.FormKey}; returned the {section} section from offset {position}.");
            }
        }

        return Error("result_too_large", "A single comparison descriptor, scalar chunk, or context metadata exceeds the 64 KiB structured result budget.");
    }

    /// <summary>Creates one independently paged comparison section.</summary>
    /// <param name="comparison">The complete fresh detached comparison.</param>
    /// <param name="workspaceId">The owning workspace.</param>
    /// <param name="revision">The exact fresh result revision.</param>
    /// <param name="fingerprint">The normalized request fingerprint.</param>
    /// <param name="section">The selected section.</param>
    /// <param name="path">The view pointer or empty path.</param>
    /// <param name="position">The next page position.</param>
    /// <param name="maximumResults">The candidate page size.</param>
    /// <param name="page">Receives the section page.</param>
    /// <param name="error">Receives a paging error.</param>
    /// <returns><see langword="true"/> when the page position is valid.</returns>
    private static bool TryCreateSectionPage(
        FormListComparison comparison,
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
        if (section == "before" || section == "after")
        {
            var view = section == "before" ? comparison.Before : comparison.After;
            return McpJsonPager.TryCreatePage(
                view,
                path,
                position,
                maximumResults,
                nextPosition => McpPageCursor.Encode(
                    workspaceId,
                    revision,
                    fingerprint,
                    path,
                    section,
                    McpJsonPager.Policy,
                    nextPosition),
                out page,
                out error);
        }

        if (section == "changes")
        {
            return TryCreateChangesPage(
                comparison.Changes,
                workspaceId,
                revision,
                fingerprint,
                position,
                maximumResults,
                out page,
                out error);
        }

        return TryCreateWarningsPage(
            comparison.Warnings,
            workspaceId,
            revision,
            fingerprint,
            position,
            maximumResults,
            out page,
            out error);
    }

    /// <summary>Pages semantic descriptors in their unchanged engine array order.</summary>
    /// <param name="changes">The ordered semantic descriptors.</param>
    /// <param name="workspaceId">The owning workspace.</param>
    /// <param name="revision">The exact fresh result revision.</param>
    /// <param name="fingerprint">The normalized request fingerprint.</param>
    /// <param name="position">The next descriptor offset.</param>
    /// <param name="maximumResults">The candidate page size.</param>
    /// <param name="page">Receives the descriptor page.</param>
    /// <param name="error">Receives a position error.</param>
    /// <returns><see langword="true"/> when the position is valid.</returns>
    private static bool TryCreateChangesPage(
        IReadOnlyList<SemanticChangeDescriptor> changes,
        Guid workspaceId,
        WorkspaceRevision revision,
        string fingerprint,
        int position,
        int maximumResults,
        out JsonElement page,
        out string error)
    {
        if (position > changes.Count)
        {
            page = default;
            error = "The comparison cursor position is outside the fresh semantic change array.";
            return false;
        }

        var items = changes.Skip(position).Take(maximumResults).Select(McpProjection.Change).ToArray();
        var nextPosition = position + items.Length;
        page = McpProjection.Json(new
        {
            offset = position,
            count = items.Length,
            totalCount = changes.Count,
            changes = items,
            cursor = nextPosition < changes.Count
                ? McpPageCursor.Encode(workspaceId, revision, fingerprint, string.Empty, "changes", McpJsonPager.Policy, nextPosition)
                : null,
        });
        error = string.Empty;
        return true;
    }

    /// <summary>Pages warnings without truncating their stable codes or messages.</summary>
    /// <param name="warnings">The ordered engine warnings.</param>
    /// <param name="workspaceId">The owning workspace.</param>
    /// <param name="revision">The exact fresh result revision.</param>
    /// <param name="fingerprint">The normalized request fingerprint.</param>
    /// <param name="position">The next warning offset.</param>
    /// <param name="maximumResults">The candidate page size.</param>
    /// <param name="page">Receives the warning page.</param>
    /// <param name="error">Receives a position error.</param>
    /// <returns><see langword="true"/> when the position is valid.</returns>
    private static bool TryCreateWarningsPage(
        IReadOnlyList<EngineWarning> warnings,
        Guid workspaceId,
        WorkspaceRevision revision,
        string fingerprint,
        int position,
        int maximumResults,
        out JsonElement page,
        out string error)
    {
        if (position > warnings.Count)
        {
            page = default;
            error = "The comparison cursor position is outside the fresh warning array.";
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
                ? McpPageCursor.Encode(workspaceId, revision, fingerprint, string.Empty, "warnings", McpJsonPager.Policy, nextPosition)
                : null,
        });
        error = string.Empty;
        return true;
    }
}
