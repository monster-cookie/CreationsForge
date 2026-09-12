using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Enums;
using Mutagen.Bethesda;

namespace CreationsForge.ViewModels;

/// <summary>Captures one revision-consistent detached review of every staged native FormList change.</summary>
public sealed class NativeWorkspaceChangeReview
{
    /// <summary>Initializes one immutable accepted workspace review.</summary>
    /// <param name="workspaceId">The exact live workspace identity.</param>
    /// <param name="game">The supported game selected by the workspace.</param>
    /// <param name="release">The exact native release selected by the workspace.</param>
    /// <param name="output">The exact selected output association.</param>
    /// <param name="outputBaseline">The exact complete selected-output baseline.</param>
    /// <param name="revision">The revision shared by the state and preview reads.</param>
    /// <param name="comparisons">Every detached engine comparison in engine order.</param>
    /// <param name="items">Every projected change item in comparison order.</param>
    /// <param name="unresolvedReferenceCount">The aggregate unresolved-reference count.</param>
    /// <param name="warnings">All state, preview, and comparison warnings retained for review.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="workspaceId"/> is empty or collection counts disagree.</exception>
    /// <exception cref="ArgumentNullException">Thrown when a required value is <see langword="null"/>.</exception>
    public NativeWorkspaceChangeReview(
        Guid workspaceId,
        SupportedGame game,
        GameRelease release,
        OutputAssociation output,
        OutputArtifactSetBaseline outputBaseline,
        WorkspaceRevision revision,
        IReadOnlyList<FormListComparison> comparisons,
        IReadOnlyList<NativeWorkspaceChangeItemViewModel> items,
        int unresolvedReferenceCount,
        IReadOnlyList<EngineWarning> warnings)
    {
        if (workspaceId == Guid.Empty)
        {
            throw new ArgumentException("A workspace review requires a non-empty workspace identifier.", nameof(workspaceId));
        }

        ArgumentNullException.ThrowIfNull(output);
        ArgumentNullException.ThrowIfNull(outputBaseline);
        ArgumentNullException.ThrowIfNull(comparisons);
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(warnings);
        if (comparisons.Count != items.Count)
        {
            throw new ArgumentException("A workspace review requires one projected item per comparison.", nameof(items));
        }

        ArgumentOutOfRangeException.ThrowIfNegative(unresolvedReferenceCount);
        WorkspaceId = workspaceId;
        Game = game;
        Release = release;
        Output = output;
        OutputBaseline = outputBaseline;
        Revision = revision;
        Comparisons = Array.AsReadOnly(comparisons.ToArray());
        Items = Array.AsReadOnly(items.ToArray());
        UnresolvedReferenceCount = unresolvedReferenceCount;
        Warnings = Array.AsReadOnly(warnings.ToArray());
    }

    /// <summary>Gets the exact workspace identity captured by the accepted state read.</summary>
    public Guid WorkspaceId { get; }

    /// <summary>Gets the supported game captured by the accepted state read.</summary>
    public SupportedGame Game { get; }

    /// <summary>Gets the exact native release captured by the accepted state read.</summary>
    public GameRelease Release { get; }

    /// <summary>Gets the exact selected output association captured by the accepted state read.</summary>
    public OutputAssociation Output { get; }

    /// <summary>Gets the exact complete output baseline captured by the accepted state read.</summary>
    public OutputArtifactSetBaseline OutputBaseline { get; }

    /// <summary>Gets the revision shared by the accepted state and preview reads.</summary>
    public WorkspaceRevision Revision { get; }

    /// <summary>Gets every detached engine comparison in engine order.</summary>
    public IReadOnlyList<FormListComparison> Comparisons { get; }

    /// <summary>Gets every projected dialog item in comparison order.</summary>
    public IReadOnlyList<NativeWorkspaceChangeItemViewModel> Items { get; }

    /// <summary>Gets the aggregate number of unresolved native references.</summary>
    public int UnresolvedReferenceCount { get; }

    /// <summary>Gets all warnings retained from the accepted review.</summary>
    public IReadOnlyList<EngineWarning> Warnings { get; }

    /// <summary>Gets whether the authoritative workspace preview contains staged comparisons.</summary>
    public bool HasStagedChanges => Comparisons.Count > 0;
}
