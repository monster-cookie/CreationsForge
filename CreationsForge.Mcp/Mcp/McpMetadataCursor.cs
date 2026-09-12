using System.Security.Cryptography;
using System.Text.Json;

namespace CreationsForge.Mcp;

/// <summary>Encodes integrity-checked metadata continuations independent of mutable workspace revision.</summary>
internal static class McpMetadataCursor
{
    /// <summary>The current metadata cursor contract version.</summary>
    private const int Version = 1;

    /// <summary>The checksum length appended to the canonical JSON payload.</summary>
    private const int ChecksumLength = 32;

    /// <summary>Encodes the next metadata page position with its immutable request bindings.</summary>
    /// <param name="handle">The exact host-local metadata handle.</param>
    /// <param name="path">The canonical JSON Pointer.</param>
    /// <param name="maximumResults">The requested page count bound.</param>
    /// <param name="nextPosition">The next immediate-child or UTF-16 position.</param>
    /// <returns>A canonical checksum-protected Base64Url cursor.</returns>
    internal static string Encode(string handle, string path, int maximumResults, int nextPosition)
    {
        var payload = JsonSerializer.SerializeToUtf8Bytes(new
        {
            version = Version,
            handle,
            path,
            maximumResults,
            policy = McpJsonPager.Policy,
            nextPosition,
        });
        var bytes = new byte[payload.Length + ChecksumLength];
        payload.CopyTo(bytes, 0);
        SHA256.HashData(payload, bytes.AsSpan(payload.Length, ChecksumLength));
        return EncodeBase64Url(bytes);
    }

    /// <summary>Validates a metadata cursor and every immutable request binding.</summary>
    /// <param name="token">The untrusted cursor.</param>
    /// <param name="handle">The expected host-local metadata handle.</param>
    /// <param name="path">The expected canonical JSON Pointer.</param>
    /// <param name="maximumResults">The expected page count bound.</param>
    /// <param name="nextPosition">Receives the validated next position.</param>
    /// <returns><see langword="true"/> only for a canonical matching cursor.</returns>
    internal static bool TryDecode(
        string token,
        string handle,
        string path,
        int maximumResults,
        out int nextPosition)
    {
        nextPosition = 0;
        if (string.IsNullOrWhiteSpace(token) || token.Length > McpInput.MaximumCursorLength)
        {
            return false;
        }

        byte[] bytes;
        try
        {
            bytes = DecodeBase64Url(token);
        }
        catch (FormatException)
        {
            return false;
        }

        if (!string.Equals(EncodeBase64Url(bytes), token, StringComparison.Ordinal) || bytes.Length <= ChecksumLength)
        {
            return false;
        }

        var payloadLength = bytes.Length - ChecksumLength;
        Span<byte> checksum = stackalloc byte[ChecksumLength];
        SHA256.HashData(bytes.AsSpan(0, payloadLength), checksum);
        if (!CryptographicOperations.FixedTimeEquals(checksum, bytes.AsSpan(payloadLength, ChecksumLength)))
        {
            return false;
        }

        try
        {
            using var document = JsonDocument.Parse(bytes.AsMemory(0, payloadLength));
            var root = document.RootElement;
            if (root.ValueKind != JsonValueKind.Object || root.EnumerateObject().Count() != 6 ||
                !root.TryGetProperty("version", out var version) ||
                !version.TryGetInt32(out var parsedVersion) || parsedVersion != Version ||
                !TryGetExactString(root, "handle", handle) ||
                !TryGetExactString(root, "path", path) ||
                !root.TryGetProperty("maximumResults", out var count) ||
                !count.TryGetInt32(out var parsedCount) || parsedCount != maximumResults ||
                !TryGetExactString(root, "policy", McpJsonPager.Policy) ||
                !root.TryGetProperty("nextPosition", out var position) ||
                !position.TryGetInt32(out nextPosition) || nextPosition < 0)
            {
                nextPosition = 0;
                return false;
            }

            return true;
        }
        catch (JsonException)
        {
            nextPosition = 0;
            return false;
        }
        catch (InvalidOperationException)
        {
            nextPosition = 0;
            return false;
        }
    }

    /// <summary>Checks one cursor string property using ordinal equality.</summary>
    private static bool TryGetExactString(JsonElement root, string name, string expected)
    {
        return root.TryGetProperty(name, out var element) &&
            element.ValueKind == JsonValueKind.String &&
            string.Equals(element.GetString(), expected, StringComparison.Ordinal);
    }

    /// <summary>Decodes canonical unpadded Base64Url text.</summary>
    private static byte[] DecodeBase64Url(string token)
    {
        var base64 = token.Replace('-', '+').Replace('_', '/');
        var remainder = base64.Length % 4;
        if (remainder == 1)
        {
            throw new FormatException("Invalid Base64Url length.");
        }

        if (remainder != 0)
        {
            base64 = base64.PadRight(base64.Length + (4 - remainder), '=');
        }

        return Convert.FromBase64String(base64);
    }

    /// <summary>Encodes bytes as canonical unpadded Base64Url text.</summary>
    private static string EncodeBase64Url(byte[] bytes)
    {
        return Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }
}
