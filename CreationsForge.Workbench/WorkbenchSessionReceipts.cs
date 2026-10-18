using System.Text.Json.Nodes;

namespace CreationsForge.Workbench;

/// <summary>Copies session and revision identifiers between tool results and later tool arguments.</summary>
internal static class WorkbenchSessionReceipts
{
    /// <summary>Fills <c>workspaceId</c> and <c>expectedRevision</c> when the schema requires them and the call left them out.</summary>
    /// <param name="arguments">The arguments that will be sent. This method writes only into this object.</param>
    /// <param name="inputSchema">The discovered input schema, or <see langword="null"/> when the tool was not listed.</param>
    /// <param name="workspaceId">The session identifier from the latest successful tool result.</param>
    /// <param name="revision">The revision from the latest successful tool result. Zero is a real revision.</param>
    public static void Apply(JsonObject arguments, JsonObject? inputSchema, string? workspaceId, ulong? revision)
    {
        ArgumentNullException.ThrowIfNull(arguments);
        if (inputSchema?["properties"] is not JsonObject properties)
        {
            return;
        }

        if (!string.IsNullOrWhiteSpace(workspaceId)
            && properties.ContainsKey("workspaceId")
            && !HasText(arguments, "workspaceId"))
        {
            arguments["workspaceId"] = workspaceId;
        }

        if (revision is not null
            && properties.ContainsKey("expectedRevision")
            && !HasNumber(arguments, "expectedRevision"))
        {
            arguments["expectedRevision"] = JsonValue.Create(revision.Value);
        }
    }

    /// <summary>Stores identifiers from a successful tool result, and clears them after <c>workspace_close</c>.</summary>
    /// <param name="tool">The tool that produced <paramref name="result"/>.</param>
    /// <param name="result">The tool result object, including <c>structuredContent</c> when the host sent it.</param>
    /// <param name="workspaceId">The stored session identifier. Cleared when <paramref name="tool"/> is <c>workspace_close</c>.</param>
    /// <param name="revision">The stored revision. Cleared when <paramref name="tool"/> is <c>workspace_close</c>.</param>
    public static void Capture(string tool, JsonObject result, ref string? workspaceId, ref ulong? revision)
    {
        ArgumentNullException.ThrowIfNull(result);
        if (string.Equals(tool, "workspace_close", StringComparison.Ordinal))
        {
            workspaceId = null;
            revision = null;
            return;
        }

        if (result["structuredContent"] is not JsonObject content)
        {
            return;
        }

        var id = ReadString(content["workspaceId"]);
        if (!string.IsNullOrWhiteSpace(id))
        {
            workspaceId = id;
        }

        if (TryReadUInt64(content["revision"], out var observed))
        {
            revision = observed;
        }
    }

    /// <summary>Reads a non-negative integer stored as a JSON number.</summary>
    /// <param name="node">The JSON value to read.</param>
    /// <param name="value">The integer when this method returns <see langword="true"/>.</param>
    /// <returns><see langword="true"/> when <paramref name="node"/> is a non-negative integer that fits in <see cref="ulong"/>.</returns>
    public static bool TryReadUInt64(JsonNode? node, out ulong value)
    {
        value = 0;
        if (node is not JsonValue json)
        {
            return false;
        }

        if (json.TryGetValue<ulong>(out value))
        {
            return true;
        }

        if (json.TryGetValue<long>(out var signed) && signed >= 0)
        {
            value = (ulong)signed;
            return true;
        }

        if (json.TryGetValue<int>(out var integer) && integer >= 0)
        {
            value = (ulong)integer;
            return true;
        }

        return false;
    }

    private static bool HasText(JsonObject arguments, string name)
    {
        return arguments[name] is JsonValue value
            && value.TryGetValue<string>(out var text)
            && !string.IsNullOrWhiteSpace(text);
    }

    private static bool HasNumber(JsonObject arguments, string name)
    {
        return arguments[name] is JsonValue value && value.GetValueKind() == System.Text.Json.JsonValueKind.Number;
    }

    private static string? ReadString(JsonNode? node)
    {
        return node is JsonValue value && value.TryGetValue<string>(out var text) ? text : null;
    }
}
