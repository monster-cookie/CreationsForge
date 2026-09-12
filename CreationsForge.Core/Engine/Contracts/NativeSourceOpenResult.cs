namespace CreationsForge.Core.Engine.Contracts;

/// <summary>
/// Carries an acquired native source lifetime and the deterministic baseline established while opening it.
/// </summary>
public sealed class NativeSourceOpenResult
{
    /// <summary>Initializes a native source-open result.</summary>
    /// <param name="sources">The independently owned native source lifetime.</param>
    /// <param name="baselineId">The deterministic identity derived from the complete observed source baseline.</param>
    /// <param name="artifacts">Every plugin and localized-string artifact actually considered, including expected absences.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="baselineId"/> is empty.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="sources"/> or <paramref name="artifacts"/> is <see langword="null"/>.</exception>
    public NativeSourceOpenResult(
        INativeSourceSet sources,
        Guid baselineId,
        IReadOnlyList<NativeArtifactAssociation> artifacts)
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

    /// <summary>Gets the independently owned native source lifetime.</summary>
    public INativeSourceSet Sources { get; }

    /// <summary>Gets the deterministic identity of the complete observed source baseline.</summary>
    public Guid BaselineId { get; }

    /// <summary>Gets every plugin and localized-string artifact considered while opening.</summary>
    public IReadOnlyList<NativeArtifactAssociation> Artifacts { get; }
}
