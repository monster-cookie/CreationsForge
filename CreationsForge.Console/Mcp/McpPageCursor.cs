using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;

namespace CreationsForge.Console.Mcp;

/// <summary>
/// Encodes integrity-checked stateless transport paging bound to an exact engine result revision.
/// </summary>
internal static class McpPageCursor
{
    /// <summary>The current cursor contract version.</summary>
    private const int Version = 1;

    /// <summary>The SHA-256 checksum length appended to every payload.</summary>
    private const int ChecksumLength = 32;

    /// <summary>Creates a normalized request fingerprint from length-safe JSON string encoding.</summary>
    /// <param name="parts">The normalized request fields in fixed semantic order.</param>
    /// <returns>A lowercase SHA-256 fingerprint.</returns>
    internal static string CreateFingerprint(params string?[] parts)
    {
        ArgumentNullException.ThrowIfNull(parts);
        var bytes = JsonSerializer.SerializeToUtf8Bytes(parts);
        return Convert.ToHexStringLower(SHA256.HashData(bytes));
    }

    /// <summary>Encodes the next position with all required semantic cursor bindings.</summary>
    /// <param name="workspaceId">The exact workspace identity.</param>
    /// <param name="revision">The exact engine result revision.</param>
    /// <param name="requestFingerprint">The normalized request fingerprint.</param>
    /// <param name="path">The canonical JSON Pointer or empty root path.</param>
    /// <param name="section">The selected result section.</param>
    /// <param name="policy">The paging policy identifier.</param>
    /// <param name="nextPosition">The next zero-based child or string position.</param>
    /// <returns>A bounded canonical Base64Url cursor with a checksum.</returns>
    internal static string Encode(
        Guid workspaceId,
        WorkspaceRevision revision,
        string requestFingerprint,
        string path,
        string section,
        string policy,
        int nextPosition)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(requestFingerprint);
        ArgumentNullException.ThrowIfNull(path);
        ArgumentException.ThrowIfNullOrWhiteSpace(section);
        ArgumentException.ThrowIfNullOrWhiteSpace(policy);
        ArgumentOutOfRangeException.ThrowIfNegative(nextPosition);

        var payload = JsonSerializer.SerializeToUtf8Bytes(new
        {
            version = Version,
            workspaceId = workspaceId.ToString("D"),
            baselineId = revision.BaselineId.ToString("D"),
            sequence = revision.Sequence.ToString(CultureInfo.InvariantCulture),
            requestFingerprint,
            path,
            section,
            policy,
            nextPosition,
        });
        var bytes = new byte[payload.Length + ChecksumLength];
        payload.CopyTo(bytes, 0);
        SHA256.HashData(payload, bytes.AsSpan(payload.Length, ChecksumLength));
        return EncodeBase64Url(bytes);
    }

    /// <summary>Validates checksum, shape, bounds, and every semantic binding before exposing a next position.</summary>
    /// <param name="token">The untrusted caller-supplied cursor.</param>
    /// <param name="workspaceId">The expected workspace.</param>
    /// <param name="revision">The fresh engine result revision.</param>
    /// <param name="requestFingerprint">The expected normalized request fingerprint.</param>
    /// <param name="path">The expected canonical path.</param>
    /// <param name="section">The expected result section.</param>
    /// <param name="policy">The expected paging policy.</param>
    /// <param name="nextPosition">Receives the validated next position.</param>
    /// <returns><see langword="true"/> only when the cursor is canonical and matches the fresh operation.</returns>
    internal static bool TryDecode(
        string token,
        Guid workspaceId,
        WorkspaceRevision revision,
        string requestFingerprint,
        string path,
        string section,
        string policy,
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

        if (!string.Equals(EncodeBase64Url(bytes), token, StringComparison.Ordinal) ||
            bytes.Length <= ChecksumLength)
        {
            return false;
        }

        var payloadLength = bytes.Length - ChecksumLength;
        var payload = bytes.AsSpan(0, payloadLength);
        Span<byte> expectedChecksum = stackalloc byte[ChecksumLength];
        SHA256.HashData(payload, expectedChecksum);
        if (!CryptographicOperations.FixedTimeEquals(
                expectedChecksum,
                bytes.AsSpan(bytes.Length - ChecksumLength)))
        {
            return false;
        }

        try
        {
            using var document = JsonDocument.Parse(bytes.AsMemory(0, payloadLength));
            var root = document.RootElement;
            if (root.ValueKind != JsonValueKind.Object || root.EnumerateObject().Count() != 9 ||
                !root.TryGetProperty("version", out var versionElement) ||
                !versionElement.TryGetInt32(out var cursorVersion) ||
                cursorVersion != Version ||
                !TryGetExactString(root, "workspaceId", workspaceId.ToString("D")) ||
                !TryGetExactString(root, "baselineId", revision.BaselineId.ToString("D")) ||
                !TryGetExactString(root, "sequence", revision.Sequence.ToString(CultureInfo.InvariantCulture)) ||
                !TryGetExactString(root, "requestFingerprint", requestFingerprint) ||
                !TryGetExactString(root, "path", path) ||
                !TryGetExactString(root, "section", section) ||
                !TryGetExactString(root, "policy", policy) ||
                !root.TryGetProperty("nextPosition", out var positionElement) ||
                !positionElement.TryGetInt32(out nextPosition) ||
                nextPosition < 0)
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

    /// <summary>Checks one required string property for exact ordinal equality.</summary>
    /// <param name="root">The decoded cursor object.</param>
    /// <param name="name">The property name.</param>
    /// <param name="expected">The exact expected value.</param>
    /// <returns><see langword="true"/> when the property is an equal string.</returns>
    private static bool TryGetExactString(JsonElement root, string name, string expected)
    {
        return root.TryGetProperty(name, out var element) &&
            element.ValueKind == JsonValueKind.String &&
            string.Equals(element.GetString(), expected, StringComparison.Ordinal);
    }

    /// <summary>Decodes canonical unpadded Base64Url text.</summary>
    /// <param name="token">The token to decode.</param>
    /// <returns>The decoded bytes.</returns>
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
    /// <param name="bytes">The bytes to encode.</param>
    /// <returns>The canonical token.</returns>
    private static string EncodeBase64Url(byte[] bytes)
    {
        return Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }
}
