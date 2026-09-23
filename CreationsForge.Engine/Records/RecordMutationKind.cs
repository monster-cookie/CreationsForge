namespace CreationsForge.Engine.Records;

/// <summary>Identifies how a record candidate enters the mutable output.</summary>
public enum RecordMutationKind
{
    /// <summary>Creates a new record identity using the output's Mutagen allocator.</summary>
    Create,

    /// <summary>Copies an exact selected record context into the output as an override.</summary>
    Override,
}
