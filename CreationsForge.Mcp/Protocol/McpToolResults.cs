using System.Text.Json;
using System.Text.Json.Serialization;
using ModelContextProtocol.Protocol;

namespace CreationsForge.Mcp.Protocol;

/// <summary>Builds structured MCP tool results without writing protocol frames to standard error.</summary>
internal static class McpToolResults
{
    private static readonly JsonSerializerOptions SerializerOptions = CreateSerializerOptions();

    /// <summary>Gets the serializer used for structured tool payloads and replay fingerprints.</summary>
    public static JsonSerializerOptions JsonOptions => SerializerOptions;

    /// <summary>Creates a successful structured result.</summary>
    /// <param name="result">The closed result payload. Its runtime type is serialized so record properties are retained.</param>
    /// <returns>A cacheable success result whose structured content is the result object itself.</returns>
    public static McpInvocation Success(object result)
    {
        ArgumentNullException.ThrowIfNull(result);
        var structured = JsonSerializer.SerializeToElement(result, result.GetType(), SerializerOptions);
        return Create("The operation completed.", structured, isError: false, cacheable: true);
    }

    /// <summary>Creates a structured domain failure.</summary>
    /// <param name="code">The stable domain error code.</param>
    /// <param name="message">The actionable failure message.</param>
    /// <param name="details">Optional closed details such as a path, revision, or publication state.</param>
    /// <param name="cacheable">Whether an identical operation should return this failure instead of running again.</param>
    /// <returns>The structured failure <c>{ code, message, details }</c>.</returns>
    public static McpInvocation Failure(string code, string message, object? details = null, bool cacheable = true)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(message);
        var body = new McpErrorBody(code, message, details);
        var structured = JsonSerializer.SerializeToElement(body, SerializerOptions);
        return Create(message, structured, isError: true, cacheable);
    }

    /// <summary>Serializes a payload with the tool serializer.</summary>
    /// <param name="value">The value to serialize.</param>
    /// <returns>Canonical JSON for fingerprints and cursors.</returns>
    public static string Serialize(object value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return JsonSerializer.Serialize(value, value.GetType(), SerializerOptions);
    }

    private static McpInvocation Create(string text, JsonElement structured, bool isError, bool cacheable)
    {
        var result = new CallToolResult
        {
            IsError = isError,
            Content = [new TextContentBlock { Text = text }],
            StructuredContent = structured,
        };
        return new McpInvocation(result, cacheable, structured.GetRawText(), text, isError);
    }

    private static JsonSerializerOptions CreateSerializerOptions()
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };
        options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        return options;
    }

    private sealed class McpErrorBody
    {
        public McpErrorBody(string code, string message, object? details)
        {
            Code = code;
            Message = message;
            Details = details;
        }

        public string Code { get; }

        public string Message { get; }

        public object? Details { get; }
    }
}
