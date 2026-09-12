using System.Text.Json;

namespace CreationsForge.Console.Mcp;

/// <summary>
/// Composes reusable closed JSON Schema fragments for the explicit MCP tool catalog.
/// </summary>
internal static class McpToolSchema
{
    /// <summary>A nullable string schema used for optional native identifiers and paths.</summary>
    internal const string NullableString = "{\"oneOf\":[{\"type\":\"string\"},{\"type\":\"null\"}]}";

    /// <summary>A nullable integer schema used for optional native positions.</summary>
    internal const string NullableInteger = "{\"oneOf\":[{\"type\":\"integer\",\"minimum\":0},{\"type\":\"null\"}]}";

    /// <summary>The closed exact-revision schema with an unsigned decimal string sequence.</summary>
    internal const string Revision = "{\"type\":\"object\",\"properties\":{\"baselineId\":{\"type\":\"string\",\"format\":\"uuid\"},\"sequence\":{\"type\":\"string\",\"pattern\":\"^(0|[1-9][0-9]*)$\"}},\"required\":[\"baselineId\",\"sequence\"],\"additionalProperties\":false}";

    /// <summary>The closed engine-warning schema.</summary>
    internal const string Warning = "{\"type\":\"object\",\"properties\":{\"code\":{\"type\":\"string\",\"minLength\":1},\"message\":{\"type\":\"string\",\"minLength\":1}},\"required\":[\"code\",\"message\"],\"additionalProperties\":false}";

    /// <summary>The closed contextual native selection schema.</summary>
    internal const string Context = "{\"type\":\"object\",\"properties\":{\"formKey\":{\"type\":\"string\",\"minLength\":1},\"scope\":{\"type\":\"string\",\"enum\":[\"winning_overrides\",\"all_contexts\",\"source\",\"staged_output\"]},\"requestedContainingModKey\":" + NullableString + ",\"status\":{\"type\":\"string\",\"enum\":[\"resolved\",\"unresolved\",\"unsupported\",\"deleted\",\"ambiguous\",\"unknown_family\"]},\"containingModKey\":" + NullableString + ",\"sourcePath\":" + NullableString + ",\"loadOrderIndex\":" + NullableInteger + ",\"role\":" + NullableString + "},\"required\":[\"formKey\",\"scope\",\"requestedContainingModKey\",\"status\",\"containingModKey\",\"sourcePath\",\"loadOrderIndex\",\"role\"],\"additionalProperties\":false}";

    /// <summary>The closed opaque metadata-reference schema shared by output mutation receipts.</summary>
    internal const string MetadataReference = "{\"type\":\"object\",\"properties\":{\"handle\":{\"type\":\"string\",\"minLength\":1},\"kind\":{\"type\":\"string\",\"enum\":[\"output_association\",\"output_baseline\"]},\"baselineId\":" + NullableString + "},\"required\":[\"handle\",\"kind\",\"baselineId\"],\"additionalProperties\":false}";

    /// <summary>The closed immediate-child and scalar JSON page alternatives.</summary>
    internal const string JsonPage = "{\"oneOf\":[" +
        "{\"type\":\"object\",\"properties\":{\"path\":{\"type\":\"string\"},\"kind\":{\"const\":\"object\"},\"offset\":{\"type\":\"integer\",\"minimum\":0},\"count\":{\"type\":\"integer\",\"minimum\":0},\"totalCount\":{\"type\":\"integer\",\"minimum\":0},\"children\":{\"type\":\"array\",\"items\":{\"type\":\"object\",\"properties\":{\"property\":{\"type\":\"string\"},\"path\":{\"type\":\"string\"},\"kind\":{\"type\":\"string\",\"enum\":[\"object\",\"array\",\"string\",\"number\",\"boolean\",\"null\"]},\"count\":{\"type\":\"integer\",\"minimum\":0}},\"required\":[\"property\",\"path\",\"kind\",\"count\"],\"additionalProperties\":false}},\"cursor\":" + NullableString + "},\"required\":[\"path\",\"kind\",\"offset\",\"count\",\"totalCount\",\"children\",\"cursor\"],\"additionalProperties\":false}," +
        "{\"type\":\"object\",\"properties\":{\"path\":{\"type\":\"string\"},\"kind\":{\"const\":\"array\"},\"offset\":{\"type\":\"integer\",\"minimum\":0},\"count\":{\"type\":\"integer\",\"minimum\":0},\"totalCount\":{\"type\":\"integer\",\"minimum\":0},\"children\":{\"type\":\"array\",\"items\":{\"type\":\"object\",\"properties\":{\"index\":{\"type\":\"integer\",\"minimum\":0},\"path\":{\"type\":\"string\"},\"kind\":{\"type\":\"string\",\"enum\":[\"object\",\"array\",\"string\",\"number\",\"boolean\",\"null\"]},\"count\":{\"type\":\"integer\",\"minimum\":0}},\"required\":[\"index\",\"path\",\"kind\",\"count\"],\"additionalProperties\":false}},\"cursor\":" + NullableString + "},\"required\":[\"path\",\"kind\",\"offset\",\"count\",\"totalCount\",\"children\",\"cursor\"],\"additionalProperties\":false}," +
        "{\"type\":\"object\",\"properties\":{\"path\":{\"type\":\"string\"},\"kind\":{\"const\":\"string\"},\"offset\":{\"type\":\"integer\",\"minimum\":0},\"length\":{\"type\":\"integer\",\"minimum\":0},\"totalLength\":{\"type\":\"integer\",\"minimum\":0},\"value\":{\"type\":\"string\"},\"cursor\":" + NullableString + "},\"required\":[\"path\",\"kind\",\"offset\",\"length\",\"totalLength\",\"value\",\"cursor\"],\"additionalProperties\":false}," +
        "{\"type\":\"object\",\"properties\":{\"path\":{\"type\":\"string\"},\"kind\":{\"const\":\"number\"},\"rawValue\":{\"type\":\"string\",\"minLength\":1},\"count\":{\"const\":1}},\"required\":[\"path\",\"kind\",\"rawValue\",\"count\"],\"additionalProperties\":false}," +
        "{\"type\":\"object\",\"properties\":{\"path\":{\"type\":\"string\"},\"kind\":{\"const\":\"boolean\"},\"value\":{\"type\":\"boolean\"},\"count\":{\"const\":1}},\"required\":[\"path\",\"kind\",\"value\",\"count\"],\"additionalProperties\":false}," +
        "{\"type\":\"object\",\"properties\":{\"path\":{\"type\":\"string\"},\"kind\":{\"type\":\"string\",\"enum\":[\"null\",\"absent\"]},\"count\":{\"const\":0}},\"required\":[\"path\",\"kind\",\"count\"],\"additionalProperties\":false}" +
        "]}";

    /// <summary>Creates one complete success-or-error output schema from a closed result schema.</summary>
    /// <param name="resultSchema">The closed result schema JSON.</param>
    /// <returns>A detached output schema element.</returns>
    internal static JsonElement Output(string resultSchema)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(resultSchema);
        var json = "{\"oneOf\":[" +
            "{\"type\":\"object\",\"properties\":{\"ok\":{\"const\":true},\"result\":" + resultSchema + "},\"required\":[\"ok\",\"result\"],\"additionalProperties\":false}," +
            "{\"type\":\"object\",\"properties\":{\"ok\":{\"const\":false},\"error\":{\"type\":\"object\",\"properties\":{\"code\":{\"type\":\"string\",\"minLength\":1},\"message\":{\"type\":\"string\",\"minLength\":1}},\"required\":[\"code\",\"message\"],\"additionalProperties\":false}},\"required\":[\"ok\",\"error\"],\"additionalProperties\":false}" +
            "]}";
        using var document = JsonDocument.Parse(json);
        return document.RootElement.Clone();
    }
}
