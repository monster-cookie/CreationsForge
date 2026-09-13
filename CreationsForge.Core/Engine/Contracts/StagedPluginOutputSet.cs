namespace CreationsForge.Core.Engine.Contracts;

/// <summary>
/// Carries an opaque staged plugin lifetime and every explicit staged-to-destination artifact mapping.
/// </summary>
public sealed class StagedPluginOutputSet
{
    /// <summary>Initializes a staged output set reopened through the game adapter.</summary>
    /// <param name="disposition">Whether plugin staging produced a complete changed artifact set or proved the existing output unchanged.</param>
    /// <param name="stagedOutput">The independently owned staged plugin lifetime when changes were written; otherwise <see langword="null"/>.</param>
    /// <param name="artifactMappings">Every prepared plugin and strings mapping, including intended destination removals.</param>
    /// <exception cref="ArgumentException">Thrown when the disposition disagrees with the staged lifetime or artifact mappings.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="artifactMappings"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="disposition"/> is undefined.</exception>
    public StagedPluginOutputSet(
        PluginWriteDisposition disposition,
        IStagedPluginOutputSet? stagedOutput,
        IReadOnlyList<StagedPluginArtifactMapping> artifactMappings)
    {
        ArgumentNullException.ThrowIfNull(artifactMappings);
        if (!Enum.IsDefined(disposition))
        {
            throw new ArgumentOutOfRangeException(nameof(disposition));
        }

        if (artifactMappings.Any(mapping => mapping is null))
        {
            throw new ArgumentException("A staged output set cannot contain a null artifact mapping.", nameof(artifactMappings));
        }

        if (disposition == PluginWriteDisposition.Unchanged
            && (stagedOutput is not null || artifactMappings.Count != 0))
        {
            throw new ArgumentException("An unchanged plugin write cannot carry a staged lifetime or artifact mappings.", nameof(disposition));
        }

        if (disposition == PluginWriteDisposition.StagedChanges
            && (stagedOutput is null || artifactMappings.Count == 0))
        {
            throw new ArgumentException("A changed plugin write requires a staged lifetime and explicit artifact mappings.", nameof(disposition));
        }

        var pathComparer = OperatingSystem.IsWindows() ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;
        if (artifactMappings.Select(mapping => mapping.StagedArtifact.Path).Distinct(pathComparer).Count() != artifactMappings.Count
            || artifactMappings.Select(mapping => mapping.DestinationPath).Distinct(pathComparer).Count() != artifactMappings.Count)
        {
            throw new ArgumentException("A staged output set cannot contain duplicate staged or destination paths.", nameof(artifactMappings));
        }

        Disposition = disposition;
        StagedOutput = stagedOutput;
        ArtifactMappings = Array.AsReadOnly(artifactMappings.ToArray());
    }

    /// <summary>Gets whether plugin staging produced changes or proved the complete existing output unchanged.</summary>
    public PluginWriteDisposition Disposition { get; }

    /// <summary>Gets the independently owned staged plugin lifetime when changes were written.</summary>
    public IStagedPluginOutputSet? StagedOutput { get; }

    /// <summary>Gets every prepared plugin and strings mapping, including intended destination removals.</summary>
    public IReadOnlyList<StagedPluginArtifactMapping> ArtifactMappings { get; }
}
