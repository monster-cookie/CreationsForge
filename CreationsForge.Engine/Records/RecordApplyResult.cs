namespace CreationsForge.Engine.Records;

/// <summary>Reports the records published by one atomic change set and the resulting workspace revision.</summary>
public sealed class RecordApplyResult
{
    /// <summary>Initializes an apply result.</summary>
    /// <param name="revision">The workspace revision after the single successful commit.</param>
    /// <param name="records">Snapshots of the published output records.</param>
    public RecordApplyResult(ulong revision, IReadOnlyList<RecordSnapshot> records)
    {
        Revision = revision;
        Records = records;
    }

    /// <summary>Gets the workspace revision after the single successful commit.</summary>
    public ulong Revision { get; }

    /// <summary>Gets snapshots of the published output records.</summary>
    public IReadOnlyList<RecordSnapshot> Records { get; }
}
