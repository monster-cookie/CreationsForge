using System.Text.Json;
using ModelContextProtocol.Protocol;

namespace CreationsForge.Mcp.Protocol;

/// <summary>Carries one tool result and whether identical replay may return it.</summary>
internal sealed class McpInvocation
{
    /// <summary>Initializes a tool invocation result.</summary>
    /// <param name="result">The protocol result returned to the client.</param>
    /// <param name="cacheable">Whether the session may replay this result.</param>
    /// <param name="structuredJson">The canonical structured payload stored for replay.</param>
    /// <param name="text">The text content stored for replay.</param>
    /// <param name="isError">Whether the stored result is a domain failure.</param>
    public McpInvocation(CallToolResult result, bool cacheable, string structuredJson, string text, bool isError)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentException.ThrowIfNullOrWhiteSpace(structuredJson);
        ArgumentException.ThrowIfNullOrWhiteSpace(text);
        Result = result;
        Cacheable = cacheable;
        StructuredJson = structuredJson;
        Text = text;
        IsError = isError;
    }

    /// <summary>Gets the protocol result returned to the client.</summary>
    public CallToolResult Result { get; }

    /// <summary>Gets whether the session may replay this result.</summary>
    public bool Cacheable { get; }

    /// <summary>Gets the canonical structured payload stored for replay.</summary>
    public string StructuredJson { get; }

    /// <summary>Gets the text content stored for replay.</summary>
    public string Text { get; }

    /// <summary>Gets whether the stored result is a domain failure.</summary>
    public bool IsError { get; }

    /// <summary>Rebuilds the protocol result from the stored payload.</summary>
    /// <returns>A new result that does not share mutable content with the original call.</returns>
    public CallToolResult Replay()
    {
        return new CallToolResult
        {
            IsError = IsError,
            Content = [new TextContentBlock { Text = Text }],
            StructuredContent = JsonSerializer.Deserialize<JsonElement>(StructuredJson),
        };
    }
}
