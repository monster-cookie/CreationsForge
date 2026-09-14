namespace CreationsForge.Core.Engine.Contracts;

/// <summary>
/// Describes all staged FormList changes and save warnings without writing destination files.
/// </summary>
public sealed class WorkspacePreview
{
    /// <summary>Initializes an immutable workspace preview.</summary>
    /// <param name="comparisons">The per-record detached plugin comparisons.</param>
    /// <param name="unresolvedReferenceCount">The number of unresolved references.</param>
    /// <param name="warnings">Warnings that must be reviewed before save.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="comparisons"/> or <paramref name="warnings"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="unresolvedReferenceCount"/> is negative.</exception>
    public WorkspacePreview(
        IReadOnlyList<FormListComparison> comparisons,
        int unresolvedReferenceCount,
        IReadOnlyList<EngineWarning> warnings)
    {
        ArgumentNullException.ThrowIfNull(comparisons);
        ArgumentNullException.ThrowIfNull(warnings);
        ArgumentOutOfRangeException.ThrowIfNegative(unresolvedReferenceCount);
        Comparisons = Array.AsReadOnly(comparisons.ToArray());
        UnresolvedReferenceCount = unresolvedReferenceCount;
        Warnings = Array.AsReadOnly(warnings.ToArray());
    }

    /// <summary>Gets immutable per-record detached plugin comparisons.</summary>
    public IReadOnlyList<FormListComparison> Comparisons { get; }

    /// <summary>Gets the number of unresolved references.</summary>
    public int UnresolvedReferenceCount { get; }

    /// <summary>Gets immutable warnings that must be reviewed before save.</summary>
    public IReadOnlyList<EngineWarning> Warnings { get; }
}
