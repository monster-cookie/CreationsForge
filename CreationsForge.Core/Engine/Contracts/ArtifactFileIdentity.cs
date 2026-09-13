namespace CreationsForge.Core.Engine.Contracts;

/// <summary>
/// Captures stable platform file identity used to detect aliases and path replacement independently of content.
/// </summary>
public sealed class ArtifactFileIdentity : IEquatable<ArtifactFileIdentity>
{
    /// <summary>Initializes platform file identity.</summary>
    /// <param name="provider">The stable platform identity provider, such as a Windows file identifier.</param>
    /// <param name="volumeId">The stable volume or device identity.</param>
    /// <param name="fileId">The stable file identity within the volume.</param>
    /// <param name="linkCount">The observed hard-link count when available.</param>
    /// <exception cref="ArgumentException">Thrown when the provider, volume, or file identity is empty or whitespace.</exception>
    public ArtifactFileIdentity(string provider, string volumeId, string fileId, ulong? linkCount)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(provider);
        ArgumentException.ThrowIfNullOrWhiteSpace(volumeId);
        ArgumentException.ThrowIfNullOrWhiteSpace(fileId);
        Provider = provider;
        VolumeId = volumeId;
        FileId = fileId;
        LinkCount = linkCount;
    }

    /// <summary>Gets the stable platform identity provider.</summary>
    public string Provider { get; }

    /// <summary>Gets the stable volume or device identity.</summary>
    public string VolumeId { get; }

    /// <summary>Gets the stable file identity within the volume.</summary>
    public string FileId { get; }

    /// <summary>Gets the observed hard-link count, or <see langword="null"/> when unavailable.</summary>
    public ulong? LinkCount { get; }

    /// <inheritdoc />
    public bool Equals(ArtifactFileIdentity? other)
    {
        return other is not null
            && string.Equals(Provider, other.Provider, StringComparison.Ordinal)
            && string.Equals(VolumeId, other.VolumeId, StringComparison.Ordinal)
            && string.Equals(FileId, other.FileId, StringComparison.Ordinal)
            && LinkCount == other.LinkCount;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return Equals(obj as ArtifactFileIdentity);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(Provider, VolumeId, FileId, LinkCount);
    }
}
