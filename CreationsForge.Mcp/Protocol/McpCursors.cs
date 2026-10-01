using System.Text;
using System.Text.Json;

namespace CreationsForge.Mcp.Protocol;

/// <summary>Encodes revision- or release-bound page cursors as opaque Base64Url JSON.</summary>
internal static class McpCursors
{
    /// <summary>Encodes a cursor payload.</summary>
    /// <param name="payload">The closed cursor state.</param>
    /// <returns>An opaque cursor that does not expose engine objects.</returns>
    public static string Encode(McpCursorPayload payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        var json = McpToolResults.Serialize(payload);
        return Base64UrlEncode(Encoding.UTF8.GetBytes(json));
    }

    /// <summary>Decodes a cursor produced by <see cref="Encode"/>.</summary>
    /// <param name="cursor">The client-supplied cursor.</param>
    /// <param name="payload">The decoded payload when decoding succeeds.</param>
    /// <returns><see langword="true"/> when the cursor is well-formed.</returns>
    public static bool TryDecode(string cursor, out McpCursorPayload? payload)
    {
        payload = null;
        if (string.IsNullOrWhiteSpace(cursor) || cursor.Length > 8192)
        {
            return false;
        }

        try
        {
            var json = Encoding.UTF8.GetString(Base64UrlDecode(cursor));
            payload = JsonSerializer.Deserialize<McpCursorPayload>(json, McpToolResults.JsonOptions);
            return payload is not null && !string.IsNullOrWhiteSpace(payload.Kind);
        }
        catch (FormatException)
        {
            return false;
        }
        catch (JsonException)
        {
            return false;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }

    private static string Base64UrlEncode(byte[] bytes)
    {
        return Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }

    private static byte[] Base64UrlDecode(string text)
    {
        var padded = text.Replace('-', '+').Replace('_', '/');
        var remainder = padded.Length % 4;
        if (remainder != 0)
        {
            padded = padded.PadRight(padded.Length + (4 - remainder), '=');
        }

        return Convert.FromBase64String(padded);
    }
}

/// <summary>Carries the query identity and skip position for one page cursor.</summary>
internal sealed class McpCursorPayload
{
    /// <summary>Gets or sets the cursor kind: search, fields, schema, compare, preview, or preview-fields.</summary>
    public string Kind { get; set; } = string.Empty;

    /// <summary>Gets or sets the workspace session that issued a workspace-bound cursor.</summary>
    public string? WorkspaceId { get; set; }

    /// <summary>Gets or sets the workspace revision for workspace-bound cursors.</summary>
    public ulong? Revision { get; set; }

    /// <summary>Gets or sets the game release for schema cursors.</summary>
    public string? Release { get; set; }

    /// <summary>Gets or sets the declared family identifier.</summary>
    public string? FamilyId { get; set; }

    /// <summary>Gets or sets the EditorID filter captured by a search cursor.</summary>
    public string? EditorId { get; set; }

    /// <summary>Gets or sets the origin FormKey text.</summary>
    public string? FormKey { get; set; }

    /// <summary>Gets or sets the exact containing plugin.</summary>
    public string? ContainingModKey { get; set; }

    /// <summary>Gets or sets the right-side origin FormKey for a comparison cursor.</summary>
    public string? RightFormKey { get; set; }

    /// <summary>Gets or sets the right-side containing plugin for a comparison cursor.</summary>
    public string? RightContainingModKey { get; set; }

    /// <summary>Gets or sets the number of items already returned.</summary>
    public int Skip { get; set; }
}
