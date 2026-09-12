using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Text;
using CreationsForge.Core.Engine.Contracts;

namespace CreationsForge.Core.Engine.NativeReading;

/// <summary>
/// Encodes and validates bounded native-search continuation state without retaining server-side result state.
/// </summary>
internal static class NativeReferenceCursor
{
    /// <summary>The binary continuation contract version.</summary>
    private const byte Version = 1;

    /// <summary>The SHA-256 integrity suffix length.</summary>
    private const int DigestLength = 32;

    /// <summary>The largest decoded containing-plugin identity accepted from an untrusted token.</summary>
    private const int MaximumContainingModKeyLength = 512;

    /// <summary>The payload bytes occupied by fixed-size fields and both string-length prefixes.</summary>
    private const int FixedPayloadLength = 1 + 16 + 16 + 8 + 4 + 4 + 4 + 4 + 4 + 4;

    /// <summary>The largest canonical Base64Url token emitted for all accepted UTF-8 field lengths.</summary>
    private static readonly int MaximumTokenLength = CalculateMaximumTokenLength();

    /// <summary>Encodes the next native scan location and all semantic cursor bindings.</summary>
    /// <param name="request">The normalized search request.</param>
    /// <param name="workspaceId">The workspace that owns the cursor.</param>
    /// <param name="revision">The exact source/output and staged-mutation revision.</param>
    /// <param name="sourceIndex">The next source index to scan.</param>
    /// <param name="recordIndex">The next native record ordinal within that source.</param>
    /// <returns>An integrity-protected Base64Url token.</returns>
    internal static string Encode(
        ReferenceSearchRequest request,
        Guid workspaceId,
        WorkspaceRevision revision,
        int sourceIndex,
        int recordIndex)
    {
        var queryBytes = Encoding.UTF8.GetBytes(request.Query);
        var containingModKey = request.ContainingModKey?.ToString() ?? string.Empty;
        var modKeyBytes = Encoding.UTF8.GetBytes(containingModKey);
        var payloadLength = 1 + 16 + 16 + 8 + 4 + 4 + 4 + 4 + 4 + queryBytes.Length + 4 + modKeyBytes.Length;
        var payload = new byte[payloadLength];
        var offset = 0;

        payload[offset++] = Version;
        workspaceId.TryWriteBytes(payload.AsSpan(offset, 16));
        offset += 16;
        revision.BaselineId.TryWriteBytes(payload.AsSpan(offset, 16));
        offset += 16;
        BinaryPrimitives.WriteUInt64LittleEndian(payload.AsSpan(offset, 8), revision.Sequence);
        offset += 8;
        BinaryPrimitives.WriteInt32LittleEndian(payload.AsSpan(offset, 4), (int)request.Scope);
        offset += 4;
        BinaryPrimitives.WriteInt32LittleEndian(payload.AsSpan(offset, 4), request.MaximumResults);
        offset += 4;
        BinaryPrimitives.WriteInt32LittleEndian(payload.AsSpan(offset, 4), sourceIndex);
        offset += 4;
        BinaryPrimitives.WriteInt32LittleEndian(payload.AsSpan(offset, 4), recordIndex);
        offset += 4;
        WriteBytes(payload, ref offset, queryBytes);
        WriteBytes(payload, ref offset, modKeyBytes);

        var tokenBytes = new byte[payload.Length + DigestLength];
        payload.CopyTo(tokenBytes, 0);
        SHA256.HashData(payload, tokenBytes.AsSpan(payload.Length, DigestLength));
        return EncodeBase64Url(tokenBytes);
    }

    /// <summary>Validates a continuation token and extracts its next native scan position.</summary>
    /// <param name="token">The untrusted caller-provided token.</param>
    /// <param name="request">The current normalized search request.</param>
    /// <param name="workspaceId">The current owning workspace.</param>
    /// <param name="revision">The current workspace revision.</param>
    /// <param name="sourceIndex">Receives the next source index on success.</param>
    /// <param name="recordIndex">Receives the next record ordinal on success.</param>
    /// <returns><see langword="true"/> only when integrity and every semantic binding match.</returns>
    internal static bool TryDecode(
        string token,
        ReferenceSearchRequest request,
        Guid workspaceId,
        WorkspaceRevision revision,
        out int sourceIndex,
        out int recordIndex)
    {
        sourceIndex = 0;
        recordIndex = 0;
        if (string.IsNullOrWhiteSpace(token) || token.Length > MaximumTokenLength)
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

        if (!string.Equals(EncodeBase64Url(bytes), token, StringComparison.Ordinal))
        {
            return false;
        }

        if (bytes.Length <= DigestLength)
        {
            return false;
        }

        var payload = bytes.AsSpan(0, bytes.Length - DigestLength);
        Span<byte> expectedDigest = stackalloc byte[DigestLength];
        SHA256.HashData(payload, expectedDigest);
        if (!CryptographicOperations.FixedTimeEquals(expectedDigest, bytes.AsSpan(bytes.Length - DigestLength)))
        {
            return false;
        }

        return TryReadPayload(payload, request, workspaceId, revision, out sourceIndex, out recordIndex);
    }

    /// <summary>Decodes Base64Url text while rejecting impossible padding lengths.</summary>
    /// <param name="token">The encoded token.</param>
    /// <returns>The decoded binary token.</returns>
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

    /// <summary>Encodes one canonical unpadded Base64Url representation.</summary>
    /// <param name="bytes">The binary value to encode.</param>
    /// <returns>The canonical unpadded Base64Url text.</returns>
    private static string EncodeBase64Url(byte[] bytes)
    {
        return Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    /// <summary>Reads and validates the complete binary cursor payload.</summary>
    /// <param name="payload">The integrity-verified cursor payload.</param>
    /// <param name="request">The current search request.</param>
    /// <param name="workspaceId">The current workspace identity.</param>
    /// <param name="revision">The current workspace revision.</param>
    /// <param name="sourceIndex">Receives the next source index.</param>
    /// <param name="recordIndex">Receives the next record ordinal.</param>
    /// <returns><see langword="true"/> only when the payload is well formed and bound to the current operation.</returns>
    private static bool TryReadPayload(
        ReadOnlySpan<byte> payload,
        ReferenceSearchRequest request,
        Guid workspaceId,
        WorkspaceRevision revision,
        out int sourceIndex,
        out int recordIndex)
    {
        sourceIndex = 0;
        recordIndex = 0;
        if (payload.Length < FixedPayloadLength)
        {
            return false;
        }

        var offset = 0;
        if (payload[offset++] != Version)
        {
            return false;
        }

        var tokenWorkspaceId = new Guid(payload.Slice(offset, 16));
        offset += 16;
        var tokenBaselineId = new Guid(payload.Slice(offset, 16));
        offset += 16;
        var tokenSequence = BinaryPrimitives.ReadUInt64LittleEndian(payload.Slice(offset, 8));
        offset += 8;
        var tokenScope = BinaryPrimitives.ReadInt32LittleEndian(payload.Slice(offset, 4));
        offset += 4;
        var tokenMaximumResults = BinaryPrimitives.ReadInt32LittleEndian(payload.Slice(offset, 4));
        offset += 4;
        sourceIndex = BinaryPrimitives.ReadInt32LittleEndian(payload.Slice(offset, 4));
        offset += 4;
        recordIndex = BinaryPrimitives.ReadInt32LittleEndian(payload.Slice(offset, 4));
        offset += 4;

        if (!TryReadString(payload, ref offset, ReferenceSearchRequest.MaximumQueryLength, out var query) ||
            !TryReadString(payload, ref offset, MaximumContainingModKeyLength, out var containingModKey) ||
            offset != payload.Length)
        {
            return false;
        }

        return tokenWorkspaceId == workspaceId &&
            tokenBaselineId == revision.BaselineId &&
            tokenSequence == revision.Sequence &&
            tokenScope == (int)request.Scope &&
            tokenMaximumResults == request.MaximumResults &&
            sourceIndex >= 0 &&
            recordIndex >= 0 &&
            string.Equals(query, request.Query, StringComparison.Ordinal) &&
            string.Equals(containingModKey, request.ContainingModKey?.ToString() ?? string.Empty, StringComparison.Ordinal);
    }

    /// <summary>Calculates the encoded safety bound from every accepted decoded UTF-8 field.</summary>
    /// <returns>The padded Base64 length, which also bounds the canonical unpadded representation.</returns>
    private static int CalculateMaximumTokenLength()
    {
        var maximumTokenBytes = FixedPayloadLength +
            Encoding.UTF8.GetMaxByteCount(ReferenceSearchRequest.MaximumQueryLength) +
            Encoding.UTF8.GetMaxByteCount(MaximumContainingModKeyLength) +
            DigestLength;
        return checked(((maximumTokenBytes + 2) / 3) * 4);
    }

    /// <summary>Reads one length-prefixed UTF-8 string within a fixed safety bound.</summary>
    /// <param name="payload">The complete cursor payload.</param>
    /// <param name="offset">The current payload offset, advanced on success.</param>
    /// <param name="maximumCharacterCount">The maximum decoded character count.</param>
    /// <param name="value">Receives the decoded string.</param>
    /// <returns><see langword="true"/> when the field is valid and entirely present.</returns>
    private static bool TryReadString(
        ReadOnlySpan<byte> payload,
        ref int offset,
        int maximumCharacterCount,
        out string value)
    {
        value = string.Empty;
        if (payload.Length - offset < 4)
        {
            return false;
        }

        var length = BinaryPrimitives.ReadInt32LittleEndian(payload.Slice(offset, 4));
        offset += 4;
        if (length < 0 || length > payload.Length - offset)
        {
            return false;
        }

        value = Encoding.UTF8.GetString(payload.Slice(offset, length));
        offset += length;
        return value.Length <= maximumCharacterCount;
    }

    /// <summary>Writes one length-prefixed byte field.</summary>
    /// <param name="destination">The cursor payload buffer.</param>
    /// <param name="offset">The current buffer offset, advanced after writing.</param>
    /// <param name="value">The bytes to write.</param>
    private static void WriteBytes(byte[] destination, ref int offset, byte[] value)
    {
        BinaryPrimitives.WriteInt32LittleEndian(destination.AsSpan(offset, 4), value.Length);
        offset += 4;
        value.CopyTo(destination, offset);
        offset += value.Length;
    }
}
