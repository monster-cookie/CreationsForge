namespace CreationsForge.Core.Engine.Contracts;

/// <summary>
/// Maps one verified private staged artifact observation to its canonical destination path.
/// </summary>
public sealed class NativeStagedArtifactMapping
{
    /// <summary>Initializes an explicit staged-to-destination artifact mapping.</summary>
    /// <param name="stagedArtifact">The staged artifact observation, whose absent fingerprint represents an intended destination deletion.</param>
    /// <param name="destinationPath">The canonical absolute destination path.</param>
    /// <exception cref="ArgumentException">Thrown when the staged role cannot be an output, or the destination is empty, not fully qualified, or is the staged path.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="stagedArtifact"/> is <see langword="null"/>.</exception>
    public NativeStagedArtifactMapping(
        NativeArtifactAssociation stagedArtifact,
        string destinationPath)
    {
        ArgumentNullException.ThrowIfNull(stagedArtifact);
        ArgumentException.ThrowIfNullOrWhiteSpace(destinationPath);
        if (!Path.IsPathFullyQualified(destinationPath))
        {
            throw new ArgumentException("A staged artifact destination requires a canonical absolute path.", nameof(destinationPath));
        }

        if (stagedArtifact.Role == NativeArtifactRole.StringsArchive)
        {
            throw new ArgumentException("A localized strings input archive cannot be mapped as an output artifact.", nameof(stagedArtifact));
        }

        var pathComparison = OperatingSystem.IsWindows()
            ? StringComparison.OrdinalIgnoreCase
            : StringComparison.Ordinal;
        if (string.Equals(stagedArtifact.Path, destinationPath, pathComparison))
        {
            throw new ArgumentException("A private staged artifact path must differ from its destination path.", nameof(destinationPath));
        }

        StagedArtifact = stagedArtifact;
        DestinationPath = destinationPath;
    }

    /// <summary>Gets the verified staged artifact, including expected absence for an intended deletion.</summary>
    public NativeArtifactAssociation StagedArtifact { get; }

    /// <summary>Gets the canonical absolute path to mutate when the complete set commits.</summary>
    public string DestinationPath { get; }
}
