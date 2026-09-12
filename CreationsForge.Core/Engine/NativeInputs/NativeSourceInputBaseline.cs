using CreationsForge.Core.Engine.Contracts;

namespace CreationsForge.Core.Engine.NativeInputs;

/// <summary>
/// Captures the immutable complete file baseline established after native source parsing finishes.
/// </summary>
public sealed class NativeSourceInputBaseline
{
    /// <summary>Initializes an immutable native source baseline.</summary>
    /// <param name="baselineId">The deterministic identity derived from every artifact observation.</param>
    /// <param name="artifacts">Every plugin, loose strings sidecar, expected absence, and selected strings archive.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="baselineId"/> is empty.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="artifacts"/> is <see langword="null"/>.</exception>
    public NativeSourceInputBaseline(Guid baselineId, IReadOnlyList<NativeArtifactAssociation> artifacts)
    {
        if (baselineId == Guid.Empty)
        {
            throw new ArgumentException("A native source baseline requires a non-empty deterministic identifier.", nameof(baselineId));
        }

        ArgumentNullException.ThrowIfNull(artifacts);
        BaselineId = baselineId;
        Artifacts = Array.AsReadOnly(artifacts.ToArray());
    }

    /// <summary>Gets the deterministic identity derived from the complete ordered artifact set.</summary>
    public Guid BaselineId { get; }

    /// <summary>Gets the immutable complete ordered artifact baseline.</summary>
    public IReadOnlyList<NativeArtifactAssociation> Artifacts { get; }
}
