namespace CreationsForge.Core.Engine.Contracts;

/// <summary>
/// Associates one canonical native file path with its role and observed baseline.
/// </summary>
public sealed class NativeArtifactAssociation
{
    /// <summary>Initializes a native artifact association.</summary>
    /// <param name="path">The canonical destination or source file path.</param>
    /// <param name="role">The file's native role.</param>
    /// <param name="language">The localized language identifier for a loose strings sidecar, or <see langword="null"/> for a plugin or strings archive.</param>
    /// <param name="fingerprint">The observed file baseline, including expected absence.</param>
    /// <param name="fileIdentity">Stable platform file identity used to detect aliases, or <see langword="null"/> when unavailable.</param>
    /// <exception cref="ArgumentException">Thrown when the path is empty or not absolute, the role and language disagree, or absent content has a file identity.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="fingerprint"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="role"/> is undefined.</exception>
    public NativeArtifactAssociation(
        string path,
        NativeArtifactRole role,
        string? language,
        NativeArtifactFingerprint fingerprint,
        NativeFileIdentity? fileIdentity = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentNullException.ThrowIfNull(fingerprint);
        if (!Enum.IsDefined(role))
        {
            throw new ArgumentOutOfRangeException(nameof(role));
        }

        if (!System.IO.Path.IsPathFullyQualified(path))
        {
            throw new ArgumentException("A native artifact requires a canonical absolute path.", nameof(path));
        }

        if (role is NativeArtifactRole.Plugin or NativeArtifactRole.StringsArchive && language is not null)
        {
            throw new ArgumentException("A plugin or strings archive artifact cannot have a localized language identifier.", nameof(language));
        }

        if (role is NativeArtifactRole.Strings or NativeArtifactRole.DlStrings or NativeArtifactRole.IlStrings
            && string.IsNullOrWhiteSpace(language))
        {
            throw new ArgumentException("A loose strings artifact requires a localized language identifier.", nameof(language));
        }

        if (!fingerprint.Exists && fileIdentity is not null)
        {
            throw new ArgumentException("An absent artifact cannot have an observed platform file identity.", nameof(fileIdentity));
        }

        Path = path;
        Role = role;
        Language = language;
        Fingerprint = fingerprint;
        FileIdentity = fileIdentity;
    }

    /// <summary>Gets the canonical native file path.</summary>
    public string Path { get; }

    /// <summary>Gets the file's native role.</summary>
    public NativeArtifactRole Role { get; }

    /// <summary>Gets the localized language identifier, or <see langword="null"/> for a plugin or strings archive.</summary>
    public string? Language { get; }

    /// <summary>Gets the observed file baseline, including expected absence.</summary>
    public NativeArtifactFingerprint Fingerprint { get; }

    /// <summary>Gets stable platform file identity, or <see langword="null"/> when unavailable or absent.</summary>
    public NativeFileIdentity? FileIdentity { get; }
}
