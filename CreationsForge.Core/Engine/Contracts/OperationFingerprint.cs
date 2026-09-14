using System.Security.Cryptography;

namespace CreationsForge.Core.Engine.Contracts;

/// <summary>
/// Holds the SHA-256 digest of a complete typed canonical operation payload.
/// </summary>
public sealed class OperationFingerprint : IEquatable<OperationFingerprint>
{
    /// <summary>Stores the required SHA-256 digest length.</summary>
    private const int DigestLength = 32;

    /// <summary>Stores the immutable digest bytes.</summary>
    private readonly byte[] Digest;

    /// <summary>Initializes an immutable operation fingerprint from an existing SHA-256 digest.</summary>
    /// <param name="digest">Exactly 32 SHA-256 digest bytes.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="digest"/> is not exactly 32 bytes.</exception>
    public OperationFingerprint(ReadOnlySpan<byte> digest)
    {
        if (digest.Length != DigestLength)
        {
            throw new ArgumentException("An operation fingerprint must contain exactly 32 SHA-256 bytes.", nameof(digest));
        }

        Digest = digest.ToArray();
    }

    /// <summary>Creates a fingerprint by hashing a complete typed canonical payload.</summary>
    /// <param name="canonicalPayload">The unambiguous, versioned canonical payload bytes.</param>
    /// <returns>The SHA-256 fingerprint of the supplied payload.</returns>
    public static OperationFingerprint Create(ReadOnlySpan<byte> canonicalPayload)
    {
        return new OperationFingerprint(SHA256.HashData(canonicalPayload));
    }

    /// <summary>Copies the digest bytes so callers cannot mutate the stored identity.</summary>
    /// <returns>A new byte array containing the digest.</returns>
    public byte[] ToArray()
    {
        return Digest.ToArray();
    }

    /// <inheritdoc />
    public bool Equals(OperationFingerprint? other)
    {
        return other is not null && CryptographicOperations.FixedTimeEquals(Digest, other.Digest);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return Equals(obj as OperationFingerprint);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return BitConverter.ToInt32(Digest, 0);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return Convert.ToHexString(Digest);
    }
}
