namespace CreationsForge.Engine.Records;

/// <summary>Groups record mutations that publish together with one workspace revision increment.</summary>
public sealed class RecordChangeSet
{
    /// <summary>Initializes an atomic change set.</summary>
    /// <param name="expectedRevision">The exact workspace revision on which the caller based the changes.</param>
    /// <param name="mutations">The record create and override mutations.</param>
    public RecordChangeSet(ulong expectedRevision, IEnumerable<RecordMutation> mutations)
    {
        ArgumentNullException.ThrowIfNull(mutations);
        ExpectedRevision = expectedRevision;
        Mutations = mutations.ToArray();
        if (Mutations.Count == 0)
        {
            throw new ArgumentException("A change set must contain at least one mutation.", nameof(mutations));
        }
    }

    /// <summary>Gets the exact workspace revision expected by the caller.</summary>
    public ulong ExpectedRevision { get; }

    /// <summary>Gets the mutations that must publish together.</summary>
    public IReadOnlyList<RecordMutation> Mutations { get; }
}
