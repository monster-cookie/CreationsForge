namespace CreationsForge.Core.Engine.Contracts;

/// <summary>
/// Captures the complete plugin-and-strings file set observed for a selected output.
/// </summary>
public sealed class OutputArtifactSetBaseline
{
    /// <summary>Initializes an immutable complete output-set baseline.</summary>
    /// <param name="baselineId">The non-empty identity of this exact file-set observation.</param>
    /// <param name="artifacts">Every expected present or absent plugin and strings artifact; the constructor snapshots and canonically orders them.</param>
    /// <exception cref="ArgumentException">Thrown when the identifier is empty or the artifact set is incomplete or ambiguous.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="artifacts"/> is <see langword="null"/>.</exception>
    public OutputArtifactSetBaseline(Guid baselineId, IReadOnlyList<PluginArtifactAssociation> artifacts)
    {
        if (baselineId == Guid.Empty)
        {
            throw new ArgumentException("An output baseline requires a non-empty identifier.", nameof(baselineId));
        }

        ArgumentNullException.ThrowIfNull(artifacts);
        var snapshot = artifacts.ToArray();
        if (snapshot.Count(artifact => artifact.Role == PluginArtifactRole.Plugin) != 1)
        {
            throw new ArgumentException("A complete output baseline requires exactly one plugin artifact.", nameof(artifacts));
        }

        if (snapshot.Any(artifact => artifact.Fingerprint.Exists && artifact.Fingerprint.Sha256 is null))
        {
            throw new ArgumentException("Every present output artifact requires a content digest.", nameof(artifacts));
        }

        var pathComparer = OperatingSystem.IsWindows() ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;
        if (snapshot.Select(artifact => artifact.Path).Distinct(pathComparer).Count() != snapshot.Length)
        {
            throw new ArgumentException("A complete output baseline cannot contain duplicate artifact paths.", nameof(artifacts));
        }

        snapshot = snapshot
            .OrderBy(artifact => artifact.Path, pathComparer)
            .ThenBy(artifact => artifact.Role)
            .ThenBy(artifact => artifact.Language, StringComparer.OrdinalIgnoreCase)
            .ToArray();
        BaselineId = baselineId;
        Artifacts = Array.AsReadOnly(snapshot);
    }

    /// <summary>Gets the identity of this exact file-set observation.</summary>
    public Guid BaselineId { get; }

    /// <summary>Gets every expected present or absent plugin and strings artifact in canonical path order.</summary>
    public IReadOnlyList<PluginArtifactAssociation> Artifacts { get; }
}
