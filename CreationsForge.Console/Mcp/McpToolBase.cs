using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace CreationsForge.Console.Mcp;

/// <summary>
/// Provides the shared metadata and structured result conventions for CreationsForge MCP tools.
/// </summary>
public abstract class McpToolBase : McpServerTool
{
    /// <summary>The maximum UTF-8 byte count of one complete successful structured result envelope.</summary>
    protected const int MaximumStructuredResultBytes = 64 * 1024;

    /// <summary>The maximum human-readable error length before a visible truncation suffix.</summary>
    private const int MaximumErrorMessageCharacters = 2048;

    /// <summary>The maximum machine-readable error-code length.</summary>
    private const int MaximumErrorCodeCharacters = 64;

    /// <summary>Gets an empty metadata collection because each tool supplies an explicit protocol descriptor.</summary>
    public override IReadOnlyList<object> Metadata { get; } = Array.Empty<object>();

    /// <summary>Parses and detaches an immutable JSON schema from its source document.</summary>
    /// <param name="json">The complete JSON schema text.</param>
    /// <returns>A detached schema element that remains valid for the process lifetime.</returns>
    /// <exception cref="JsonException">Thrown when <paramref name="json"/> is not valid JSON.</exception>
    protected static JsonElement ParseSchema(string json)
    {
        using var document = JsonDocument.Parse(json);
        return document.RootElement.Clone();
    }

    /// <summary>Creates a successful tool result with a stable <c>ok</c> and <c>result</c> envelope.</summary>
    /// <param name="result">The structured tool-specific result.</param>
    /// <param name="summary">A concise plain-text projection for clients that do not consume structured content.</param>
    /// <returns>A successful protocol result containing both structured and textual representations.</returns>
    protected static CallToolResult Success(JsonElement result, string summary)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(summary);

        return new CallToolResult
        {
            StructuredContent = JsonSerializer.SerializeToElement(new
            {
                ok = true,
                result,
            }),
            Content =
            [
                new TextContentBlock
                {
                    Text = summary,
                },
            ],
        };
    }

    /// <summary>Checks whether a tool result fits the complete successful structured-content byte budget.</summary>
    /// <param name="result">The tool-specific result whose envelope will be measured.</param>
    /// <returns><see langword="true"/> when the escaped UTF-8 success envelope does not exceed 64 KiB.</returns>
    protected static bool FitsStructuredResultBudget(JsonElement result)
    {
        var envelope = JsonSerializer.SerializeToUtf8Bytes(new
        {
            ok = true,
            result,
        });
        return envelope.Length <= MaximumStructuredResultBytes;
    }

    /// <summary>Projects one typed engine failure without changing its stable category or diagnostic message.</summary>
    /// <typeparam name="T">The engine result value type.</typeparam>
    /// <param name="result">The failed engine result.</param>
    /// <returns>A failed protocol result.</returns>
    protected static CallToolResult EngineFailure<T>(EngineResult<T> result)
    {
        ArgumentNullException.ThrowIfNull(result);
        var error = result.Error ?? new EngineError(
            EngineErrorCode.UnexpectedFailure,
            "The engine returned a failure without an error.");
        return Error(McpProjection.ErrorCode(error.Code), error.Message);
    }

    /// <summary>Creates a failed tool result with a stable machine-readable error code and message.</summary>
    /// <param name="code">The stable lower-snake-case error category.</param>
    /// <param name="message">The non-empty human-readable failure description.</param>
    /// <returns>A failed protocol result containing both structured and textual representations.</returns>
    protected static CallToolResult Error(string code, string message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(message);
        var boundedCode = BoundText(code, MaximumErrorCodeCharacters);
        var boundedMessage = BoundText(message, MaximumErrorMessageCharacters);

        return new CallToolResult
        {
            IsError = true,
            StructuredContent = JsonSerializer.SerializeToElement(new
            {
                ok = false,
                error = new
                {
                    code = boundedCode,
                    message = boundedMessage,
                },
            }),
            Content =
            [
                new TextContentBlock
                {
                    Text = $"{boundedCode}: {boundedMessage}",
                },
            ],
        };
    }

    /// <summary>Removes control characters and bounds text without splitting a UTF-16 surrogate pair.</summary>
    /// <param name="value">The non-empty text to sanitize.</param>
    /// <param name="maximumCharacters">The maximum UTF-16 character count before the suffix.</param>
    /// <returns>Sanitized bounded text.</returns>
    private static string BoundText(string value, int maximumCharacters)
    {
        var characters = value.Select(character => char.IsControl(character) ? ' ' : character).ToArray();
        var sanitized = new string(characters);
        if (sanitized.Length <= maximumCharacters)
        {
            return sanitized;
        }

        var length = maximumCharacters;
        if (length > 0 && char.IsHighSurrogate(sanitized[length - 1]))
        {
            length--;
        }

        return $"{sanitized[..length]}…";
    }
}
