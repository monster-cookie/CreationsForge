namespace CreationsForge.Core.Engine.Contracts;

/// <summary>
/// Identifies the exact current native baseline composition and a monotonically increasing in-memory workspace sequence.
/// </summary>
public readonly struct WorkspaceRevision : IEquatable<WorkspaceRevision>
{
    /// <summary>Initializes a workspace revision.</summary>
    /// <param name="baselineId">The non-empty identity of the current source and selected-output baseline composition.</param>
    /// <param name="sequence">The in-memory mutation sequence relative to that baseline.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="baselineId"/> is empty.</exception>
    public WorkspaceRevision(Guid baselineId, ulong sequence)
    {
        if (baselineId == Guid.Empty)
        {
            throw new ArgumentException("A workspace revision requires a non-empty baseline identifier.", nameof(baselineId));
        }

        BaselineId = baselineId;
        Sequence = sequence;
    }

    /// <summary>Gets the identity of the current source and selected-output native baseline composition.</summary>
    public Guid BaselineId { get; }

    /// <summary>Gets the monotonically increasing in-memory mutation sequence.</summary>
    public ulong Sequence { get; }

    /// <summary>Creates the next revision for the same native baseline composition.</summary>
    /// <returns>The next revision in the sequence.</returns>
    /// <exception cref="OverflowException">Thrown when the sequence has reached <see cref="ulong.MaxValue"/>.</exception>
    public WorkspaceRevision Next()
    {
        return new WorkspaceRevision(BaselineId, checked(Sequence + 1));
    }

    /// <inheritdoc />
    public bool Equals(WorkspaceRevision other)
    {
        return BaselineId == other.BaselineId && Sequence == other.Sequence;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is WorkspaceRevision other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(BaselineId, Sequence);
    }

    /// <summary>Determines whether two revisions identify the same baseline and sequence.</summary>
    public static bool operator ==(WorkspaceRevision left, WorkspaceRevision right)
    {
        return left.Equals(right);
    }

    /// <summary>Determines whether two revisions differ by baseline or sequence.</summary>
    public static bool operator !=(WorkspaceRevision left, WorkspaceRevision right)
    {
        return !left.Equals(right);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{BaselineId:N}:{Sequence}";
    }
}
