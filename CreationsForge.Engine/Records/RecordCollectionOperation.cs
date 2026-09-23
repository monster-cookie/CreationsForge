namespace CreationsForge.Engine.Records;

/// <summary>Identifies a legal operation on an editable field.</summary>
public enum RecordCollectionOperation
{
    /// <summary>Replaces a scalar, object, or complete collection value.</summary>
    Set,

    /// <summary>Appends one value to an ordered collection.</summary>
    Append,

    /// <summary>Inserts one value at a specified collection index.</summary>
    Insert,

    /// <summary>Removes the value at a specified collection index.</summary>
    RemoveAt,

    /// <summary>Removes every value from a collection.</summary>
    Clear,
}
