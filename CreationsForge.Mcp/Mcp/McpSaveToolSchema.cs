namespace CreationsForge.Mcp;

/// <summary>Provides closed JSON Schema fragments shared by save and metadata tools.</summary>
internal static class McpSaveToolSchema
{
    /// <summary>An opaque typed metadata descriptor whose baseline identity is informative only.</summary>
    internal const string MetadataReference = "{\"type\":\"object\",\"properties\":{\"handle\":{\"type\":\"string\",\"minLength\":1,\"maxLength\":128},\"kind\":{\"type\":\"string\",\"enum\":[\"output_association\",\"output_baseline\",\"resolved_output_evidence\",\"save_result\",\"recover_save_result\",\"repair_save_result\"]},\"baselineId\":" + McpToolSchema.NullableString + "},\"required\":[\"handle\",\"kind\",\"baselineId\"],\"additionalProperties\":false}";

    /// <summary>A nullable opaque typed metadata descriptor.</summary>
    internal const string NullableMetadataReference = "{\"oneOf\":[" + MetadataReference + ",{\"type\":\"null\"}]}";

    /// <summary>The exact inline output-association input needed for restart-safe recovery.</summary>
    internal const string OutputAssociationInput = "{\"type\":\"object\",\"properties\":{\"pluginPath\":{\"type\":\"string\",\"minLength\":1,\"maxLength\":32767},\"modKey\":{\"type\":\"string\",\"minLength\":1,\"maxLength\":1024},\"localizedOutputMode\":{\"type\":\"string\",\"enum\":[\"embedded\",\"separate_string_files\"]},\"masterStyle\":{\"type\":\"string\",\"enum\":[\"full\",\"small\",\"medium\"]}},\"required\":[\"pluginPath\",\"modKey\",\"localizedOutputMode\",\"masterStyle\"],\"additionalProperties\":false}";

    /// <summary>A nullable exact workspace revision.</summary>
    internal const string NullableRevision = "{\"oneOf\":[" + McpToolSchema.Revision + ",{\"type\":\"null\"}]}";
}
