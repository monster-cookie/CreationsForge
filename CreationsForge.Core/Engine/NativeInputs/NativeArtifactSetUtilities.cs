using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Text;
using CreationsForge.Core.Engine.Contracts;

namespace CreationsForge.Core.Engine.NativeInputs;

/// <summary>Provides deterministic comparison and identity operations for complete native artifact sets.</summary>
internal static class NativeArtifactSetUtilities
{
    /// <summary>Compares complete ordered artifact associations including existence, content, and platform identity.</summary>
    /// <param name="expected">The established artifact set.</param>
    /// <param name="actual">The newly observed artifact set.</param>
    /// <returns><see langword="true"/> when every association matches exactly.</returns>
    internal static bool Match(
        IReadOnlyList<NativeArtifactAssociation> expected,
        IReadOnlyList<NativeArtifactAssociation> actual)
    {
        if (expected.Count != actual.Count)
        {
            return false;
        }

        var pathComparison = OperatingSystem.IsWindows()
            ? StringComparison.OrdinalIgnoreCase
            : StringComparison.Ordinal;
        for (var index = 0; index < expected.Count; index++)
        {
            var left = expected[index];
            var right = actual[index];
            if (!string.Equals(left.Path, right.Path, pathComparison)
                || left.Role != right.Role
                || !string.Equals(left.Language, right.Language, StringComparison.Ordinal)
                || !left.Fingerprint.Equals(right.Fingerprint)
                || !Equals(left.FileIdentity, right.FileIdentity))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>Creates a deterministic non-empty GUID projection of a complete ordered artifact set.</summary>
    /// <param name="format">The versioned artifact-set format discriminator.</param>
    /// <param name="artifacts">The complete ordered native artifacts.</param>
    /// <returns>A stable non-empty 128-bit projection of a SHA-256 canonical digest.</returns>
    internal static Guid CreateBaselineId(
        string format,
        IReadOnlyList<NativeArtifactAssociation> artifacts)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(format);
        using var hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
        AppendToken(hash, format);
        foreach (var artifact in artifacts)
        {
            AppendToken(hash, artifact.Path);
            AppendToken(hash, ((int)artifact.Role).ToString(System.Globalization.CultureInfo.InvariantCulture));
            AppendToken(hash, artifact.Language ?? string.Empty);
            AppendToken(hash, artifact.Fingerprint.Exists ? "1" : "0");
            AppendToken(hash, artifact.Fingerprint.Length.ToString(System.Globalization.CultureInfo.InvariantCulture));
            AppendToken(hash, artifact.Fingerprint.Sha256 ?? string.Empty);
            AppendToken(hash, artifact.FileIdentity?.Provider ?? string.Empty);
            AppendToken(hash, artifact.FileIdentity?.VolumeId ?? string.Empty);
            AppendToken(hash, artifact.FileIdentity?.FileId ?? string.Empty);
            AppendToken(hash, artifact.FileIdentity?.LinkCount?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty);
        }

        var digest = hash.GetHashAndReset();
        var bytes = digest.AsSpan(0, 16).ToArray();
        if (bytes.All(value => value == 0))
        {
            bytes[15] = 1;
        }

        return new Guid(bytes);
    }

    /// <summary>Appends one length-delimited UTF-8 token to a canonical digest.</summary>
    /// <param name="hash">The digest receiving the token.</param>
    /// <param name="value">The canonical token value.</param>
    private static void AppendToken(IncrementalHash hash, string value)
    {
        var bytes = Encoding.UTF8.GetBytes(value);
        Span<byte> lengthBytes = stackalloc byte[sizeof(int)];
        BinaryPrimitives.WriteInt32LittleEndian(lengthBytes, bytes.Length);
        hash.AppendData(lengthBytes);
        hash.AppendData(bytes);
    }
}
