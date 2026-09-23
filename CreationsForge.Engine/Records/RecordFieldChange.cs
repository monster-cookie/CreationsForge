namespace CreationsForge.Engine.Records;

/// <summary>Requests one legal operation on one registered field path.</summary>
public sealed class RecordFieldChange
{
    /// <summary>Initializes a field change.</summary>
    /// <param name="path">The registered field path.</param>
    /// <param name="operation">The requested operation.</param>
    /// <param name="value">The new or inserted value when required by the operation.</param>
    /// <param name="index">The collection index when required by the operation.</param>
    public RecordFieldChange(
        string path,
        RecordCollectionOperation operation,
        RecordValue? value = null,
        int? index = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        Path = path;
        Operation = operation;
        Value = value;
        Index = index;
    }

    /// <summary>Gets the registered field path.</summary>
    public string Path { get; }

    /// <summary>Gets the requested operation.</summary>
    public RecordCollectionOperation Operation { get; }

    /// <summary>Gets the new or inserted value, or <see langword="null"/> when the operation has no value operand.</summary>
    public RecordValue? Value { get; }

    /// <summary>Gets the collection index, or <see langword="null"/> when the operation has no index operand.</summary>
    public int? Index { get; }
}
