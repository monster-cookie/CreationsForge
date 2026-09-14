namespace CreationsForge.Core.Engine.Contracts;

/// <summary>
/// Carries an acquired plugin source lifetime and the deterministic baseline established while opening it.
/// </summary>
public sealed class PluginSourceOpenResult
{
    /// <summary>Initializes a plugin source-open result.</summary>
    /// <param name="sources">The independently owned plugin source lifetime.</param>
    /// <param name="baselineId">The deterministic identity derived from the complete observed source baseline.</param>
    /// <param name="artifacts">Every plugin and localized-string artifact actually considered, including expected absences.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="baselineId"/> is empty.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="sources"/> or <paramref name="artifacts"/> is <see langword="null"/>.</exception>
    public PluginSourceOpenResult(
        IPluginSourceSet sources,
        Guid baselineId,
        IReadOnlyList<PluginArtifactAssociation> artifacts)
    {
        if (baselineId == Guid.Empty)
        {
            throw new ArgumentException("A source-open result requires a deterministic non-empty baseline identifier.", nameof(baselineId));
        }

        ArgumentNullException.ThrowIfNull(sources);
        ArgumentNullException.ThrowIfNull(artifacts);
        Sources = sources;
        BaselineId = baselineId;
        Artifacts = Array.AsReadOnly(artifacts.ToArray());
    }

    /// <summary>Gets the independently owned plugin source lifetime.</summary>
    public IPluginSourceSet Sources { get; }

    /// <summary>Gets the deterministic identity of the complete observed source baseline.</summary>
    public Guid BaselineId { get; }

    /// <summary>Gets every plugin and localized-string artifact considered while opening.</summary>
    public IReadOnlyList<PluginArtifactAssociation> Artifacts { get; }
}
