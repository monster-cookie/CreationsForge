namespace CreationsForge.Engine.Records;

/// <summary>Contains two exact record snapshots and their registered field differences.</summary>
public sealed class RecordComparison
{
    /// <summary>Initializes a record comparison.</summary>
    /// <param name="left">The left exact record snapshot.</param>
    /// <param name="right">The right exact record snapshot.</param>
    /// <param name="differences">Registered field differences in descriptor order.</param>
    public RecordComparison(RecordSnapshot left, RecordSnapshot right, IReadOnlyList<RecordDifference> differences)
    {
        Left = left;
        Right = right;
        Differences = differences;
    }

    /// <summary>Gets the left exact record snapshot.</summary>
    public RecordSnapshot Left { get; }

    /// <summary>Gets the right exact record snapshot.</summary>
    public RecordSnapshot Right { get; }

    /// <summary>Gets the differing registered fields.</summary>
    public IReadOnlyList<RecordDifference> Differences { get; }
}
