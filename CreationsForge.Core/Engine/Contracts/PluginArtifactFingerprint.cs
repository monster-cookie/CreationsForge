namespace CreationsForge.Core.Engine.Contracts;

/// <summary>
/// Captures the existence, length, and content digest of one plugin file baseline.
/// </summary>
public sealed class PluginArtifactFingerprint : IEquatable<PluginArtifactFingerprint>
{
    /// <summary>Initializes an immutable plugin artifact fingerprint.</summary>
    /// <param name="exists">Whether the file existed when the baseline was established.</param>
    /// <param name="length">The file length when present, or zero when absent.</param>
    /// <param name="sha256">The 64-character hexadecimal SHA-256 digest when present, or <see langword="null"/> when absent; lowercase is normalized.</param>
    /// <exception cref="ArgumentException">Thrown when existence, length, and digest disagree.</exception>
    public PluginArtifactFingerprint(bool exists, long length, string? sha256)
    {
        if (!exists && (length != 0 || sha256 is not null))
        {
            throw new ArgumentException("An absent artifact must have zero length and no digest.");
        }

        if (exists && (length < 0
            || string.IsNullOrWhiteSpace(sha256)
            || sha256.Length != 64
            || sha256.Any(character => !Uri.IsHexDigit(character))))
        {
            throw new ArgumentException("A present artifact requires a non-negative length and 64-character SHA-256 digest.");
        }

        Exists = exists;
        Length = length;
        Sha256 = sha256?.ToUpperInvariant();
    }

    /// <summary>Gets whether the file existed when observed.</summary>
    public bool Exists { get; }

    /// <summary>Gets the observed file length, or zero for an absent file.</summary>
    public long Length { get; }

    /// <summary>Gets the uppercase SHA-256 digest, or <see langword="null"/> for an absent file.</summary>
    public string? Sha256 { get; }

    /// <inheritdoc />
    public bool Equals(PluginArtifactFingerprint? other)
    {
        return other is not null
            && Exists == other.Exists
            && Length == other.Length
            && string.Equals(Sha256, other.Sha256, StringComparison.OrdinalIgnoreCase);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return Equals(obj as PluginArtifactFingerprint);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(Exists, Length, Sha256?.ToUpperInvariant());
    }
}
