namespace CreationsForge.Core.Engine.Contracts;

/// <summary>
/// Associates one canonical plugin file path with its role and observed baseline.
/// </summary>
public sealed class PluginArtifactAssociation
{
    /// <summary>Initializes a plugin artifact association.</summary>
    /// <param name="path">The canonical destination or source file path.</param>
    /// <param name="role">The file's plugin role.</param>
    /// <param name="language">The localized language identifier for a loose strings sidecar, or <see langword="null"/> for a plugin or strings archive.</param>
    /// <param name="fingerprint">The observed file baseline, including expected absence.</param>
    /// <param name="fileIdentity">Stable platform file identity used to detect aliases, or <see langword="null"/> when unavailable.</param>
    /// <exception cref="ArgumentException">Thrown when the path is empty or not absolute, the role and language disagree, or absent content has a file identity.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="fingerprint"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="role"/> is undefined.</exception>
    public PluginArtifactAssociation(
        string path,
        PluginArtifactRole role,
        string? language,
        PluginArtifactFingerprint fingerprint,
        ArtifactFileIdentity? fileIdentity = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentNullException.ThrowIfNull(fingerprint);
        if (!Enum.IsDefined(role))
        {
            throw new ArgumentOutOfRangeException(nameof(role));
        }

        if (!System.IO.Path.IsPathFullyQualified(path))
        {
            throw new ArgumentException("A plugin artifact requires a canonical absolute path.", nameof(path));
        }

        if (role is PluginArtifactRole.Plugin or PluginArtifactRole.StringsArchive && language is not null)
        {
            throw new ArgumentException("A plugin or strings archive artifact cannot have a localized language identifier.", nameof(language));
        }

        if (role is PluginArtifactRole.Strings or PluginArtifactRole.DlStrings or PluginArtifactRole.IlStrings
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

    /// <summary>Gets the canonical plugin file path.</summary>
    public string Path { get; }

    /// <summary>Gets the file's plugin role.</summary>
    public PluginArtifactRole Role { get; }

    /// <summary>Gets the localized language identifier, or <see langword="null"/> for a plugin or strings archive.</summary>
    public string? Language { get; }

    /// <summary>Gets the observed file baseline, including expected absence.</summary>
    public PluginArtifactFingerprint Fingerprint { get; }

    /// <summary>Gets stable platform file identity, or <see langword="null"/> when unavailable or absent.</summary>
    public ArtifactFileIdentity? FileIdentity { get; }
}
