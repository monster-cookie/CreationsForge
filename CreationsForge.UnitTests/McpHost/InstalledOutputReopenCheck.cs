using System.Text.Json.Nodes;
using CreationsForge.Mcp.Protocol;

namespace CreationsForge.UnitTests.McpHost;

/// <summary>Reopens the physical installed-Data output in a fresh production host and reports every mismatch.</summary>
/// <remarks>
/// The check returns problems instead of asserting so tests can prove that a missing or incorrect output fails it.
/// Only Keyword records are compared; created records are found through the output's unique EditorID prefix.
/// </remarks>
internal static class InstalledOutputReopenCheck
{
    private const string Family = "Keyword";
    private const int MaximumFieldPages = 50;

    /// <summary>Starts a fresh host, opens the existing output, and compares it with the expected records.</summary>
    /// <param name="output">The disposable installed-Data output.</param>
    /// <param name="textStorageMode">The text storage mode the output must have been saved with.</param>
    /// <param name="expected">Records the output must contain. Created records not listed here are reported as unexpected.</param>
    /// <param name="cancellationToken">Bounds every host call.</param>
    /// <returns>Human-readable problems; empty when the physical output matches.</returns>
    public static async Task<IReadOnlyList<string>> CheckAsync(
        InstalledStarfieldOutput output,
        string textStorageMode,
        IReadOnlyList<ExpectedOutputRecord> expected,
        CancellationToken cancellationToken)
    {
        var problems = new List<string>();
        if (!File.Exists(output.PluginPath))
        {
            problems.Add($"Output plugin '{output.PluginPath}' does not exist.");
        }

        await using var client = await McpStdioClient.StartAsync(cancellationToken);
        var opened = await client.CallAsync(
            McpAuthoringContract.WorkspaceOpenTool,
            McpRequests.WorkspaceOpen("reopen-check-open", "Starfield", output.DataDirectory),
            cancellationToken);
        if (opened.IsError || opened.Structured is null)
        {
            problems.Add($"workspace_open failed: {opened.Describe()}");
            await client.ShutdownAsync(cancellationToken);
            return problems;
        }

        var workspaceId = opened.Structured.RequiredString("workspaceId");
        var attached = await client.CallAsync(
            McpAuthoringContract.OutputOpenTool,
            McpRequests.OutputOpen("reopen-check-output", workspaceId, output.PluginPath, textStorageMode),
            cancellationToken);
        if (attached.IsError || attached.Structured is null)
        {
            problems.Add($"output_open failed: {attached.Describe()}");
            await client.ShutdownAsync(cancellationToken);
            return problems;
        }

        CheckState(attached.Structured, output, textStorageMode, problems);
        await CheckCreatedSetAsync(client, workspaceId, output, expected, problems, cancellationToken);
        foreach (var record in expected)
        {
            await CheckRecordAsync(client, workspaceId, output.PluginName, record, problems, cancellationToken);
        }

        var closed = await client.CallAsync(
            McpAuthoringContract.WorkspaceCloseTool,
            new JsonObject
            {
                ["operationId"] = "reopen-check-close",
                ["workspaceId"] = workspaceId,
                ["discardUnsaved"] = false,
            },
            cancellationToken);
        if (closed.IsError)
        {
            problems.Add($"workspace_close failed: {closed.Describe()}");
        }

        await client.ShutdownAsync(cancellationToken);
        return problems;
    }

    /// <summary>Reads every registered field of one exact Keyword version, following field cursors.</summary>
    /// <param name="client">The host connection.</param>
    /// <param name="workspaceId">The open session.</param>
    /// <param name="formKey">The origin FormKey text.</param>
    /// <param name="containingModKey">The plugin containing the exact version.</param>
    /// <param name="cancellationToken">Bounds every host call.</param>
    /// <returns>The field values keyed by path, or the failed host response when the record cannot be read.</returns>
    public static async Task<(IReadOnlyDictionary<string, JsonNode?>? Fields, string? Error)> ReadFieldsAsync(
        McpStdioClient client,
        string workspaceId,
        string formKey,
        string containingModKey,
        CancellationToken cancellationToken)
    {
        var fields = new Dictionary<string, JsonNode?>(StringComparer.Ordinal);
        string? cursor = null;
        for (var page = 0; page < MaximumFieldPages; page++)
        {
            var arguments = new JsonObject
            {
                ["workspaceId"] = workspaceId,
                ["familyId"] = Family,
                ["formKey"] = formKey,
                ["containingModKey"] = containingModKey,
            };
            if (cursor is not null)
            {
                arguments["cursor"] = cursor;
            }

            var result = await client.CallAsync(McpAuthoringContract.RecordReadTool, arguments, cancellationToken);
            if (result.IsError || result.Structured is null)
            {
                return (null, result.Describe());
            }

            var record = result.Structured.Object("record");
            foreach (var field in record.Array("fields"))
            {
                fields[field.RequiredString("path")] = field.Node["value"]?.DeepClone();
            }

            cursor = record.OptionalString("nextFieldCursor");
            if (string.IsNullOrWhiteSpace(cursor))
            {
                return (fields, null);
            }
        }

        return (null, $"Field paging for '{formKey}' did not finish.");
    }

    /// <summary>Requires the reopened session to report the existing, clean output in the expected Data directory and mode.</summary>
    /// <param name="state">The <c>output_open</c> state result.</param>
    /// <param name="output">The disposable installed-Data output.</param>
    /// <param name="textStorageMode">The expected text storage mode.</param>
    /// <param name="problems">Receives one entry per mismatch.</param>
    private static void CheckState(
        McpJson state,
        InstalledStarfieldOutput output,
        string textStorageMode,
        List<string> problems)
    {
        if (state.RequiredBool("isNewOutput"))
        {
            problems.Add("The reopened output was reported as new.");
        }

        if (state.RequiredBool("isDirty"))
        {
            problems.Add("The reopened output was dirty before any edit.");
        }

        if (!string.Equals(state.OptionalString("outputModKey"), output.PluginName, StringComparison.OrdinalIgnoreCase))
        {
            problems.Add($"The reopened output identity was '{state.OptionalString("outputModKey")}'.");
        }

        if (!string.Equals(state.OptionalString("textStorageMode"), textStorageMode, StringComparison.Ordinal))
        {
            problems.Add($"The reopened output text storage was '{state.OptionalString("textStorageMode")}'.");
        }

        if (!string.Equals(
                Path.GetFullPath(state.OptionalString("dataDirectory") ?? string.Empty),
                output.DataDirectory,
                StringComparison.OrdinalIgnoreCase))
        {
            problems.Add($"The reopen used Data directory '{state.OptionalString("dataDirectory")}'.");
        }
    }

    /// <summary>Requires the output's prefixed created Keywords to be exactly the expected created records.</summary>
    /// <param name="client">The host connection.</param>
    /// <param name="workspaceId">The open session.</param>
    /// <param name="output">The disposable installed-Data output.</param>
    /// <param name="expected">The expected records; only those originating in the output are compared here.</param>
    /// <param name="problems">Receives one entry per missing or unexpected record.</param>
    /// <param name="cancellationToken">Bounds every host call.</param>
    /// <returns>A task that completes when the search is compared.</returns>
    private static async Task CheckCreatedSetAsync(
        McpStdioClient client,
        string workspaceId,
        InstalledStarfieldOutput output,
        IReadOnlyList<ExpectedOutputRecord> expected,
        List<string> problems,
        CancellationToken cancellationToken)
    {
        var found = (await McpRequests.SearchAllAsync(client, workspaceId, output.EditorIdPrefix, cancellationToken))
            .Where(record => string.Equals(
                record.RequiredString("containingModKey"),
                output.PluginName,
                StringComparison.OrdinalIgnoreCase))
            .Select(record => record.RequiredString("formKey"))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var wanted = expected
            .Where(record => record.OriginatesIn(output.PluginName))
            .Select(record => record.FormKey)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        foreach (var missing in wanted.Except(found, StringComparer.OrdinalIgnoreCase))
        {
            problems.Add($"Created record '{missing}' is missing from the reopened output.");
        }

        foreach (var unexpected in found.Except(wanted, StringComparer.OrdinalIgnoreCase))
        {
            problems.Add($"Unexpected created record '{unexpected}' is in the reopened output.");
        }
    }

    /// <summary>Reads one record's output version and compares every expected field exactly.</summary>
    /// <param name="client">The host connection.</param>
    /// <param name="workspaceId">The open session.</param>
    /// <param name="outputPlugin">The output plugin that must contain the version.</param>
    /// <param name="record">The expected record.</param>
    /// <param name="problems">Receives one entry per unreadable record or mismatched field.</param>
    /// <param name="cancellationToken">Bounds every host call.</param>
    /// <returns>A task that completes when the record is compared.</returns>
    private static async Task CheckRecordAsync(
        McpStdioClient client,
        string workspaceId,
        string outputPlugin,
        ExpectedOutputRecord record,
        List<string> problems,
        CancellationToken cancellationToken)
    {
        var (fields, error) = await ReadFieldsAsync(client, workspaceId, record.FormKey, outputPlugin, cancellationToken);
        if (fields is null)
        {
            problems.Add($"{record.Label} record '{record.FormKey}' could not be read from the output: {error}");
            return;
        }

        foreach (var pair in record.Fields)
        {
            if (!fields.TryGetValue(pair.Key, out var actual))
            {
                problems.Add($"{record.Label} record '{record.FormKey}' has no field '{pair.Key}'.");
                continue;
            }

            if (!JsonNode.DeepEquals(pair.Value, actual))
            {
                problems.Add(
                    $"{record.Label} record '{record.FormKey}' field '{pair.Key}' was {actual?.ToJsonString() ?? "null"}, expected {pair.Value?.ToJsonString() ?? "null"}.");
            }
        }
    }
}
