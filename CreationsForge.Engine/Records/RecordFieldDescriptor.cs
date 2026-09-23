namespace CreationsForge.Engine.Records;

/// <summary>Describes one explicitly registered editable field path and its legal value operations.</summary>
public sealed class RecordFieldDescriptor
{
    /// <summary>Initializes a field descriptor.</summary>
    /// <param name="path">The stable discovered field path.</param>
    /// <param name="kind">The closed value kind.</param>
    /// <param name="isNullable">Whether an explicit null is legal.</param>
    /// <param name="isFlags">Whether enum choices may be combined as flags.</param>
    /// <param name="choices">Legal enum names, when applicable.</param>
    /// <param name="referenceTargets">Legal Mutagen getter target types, when applicable.</param>
    /// <param name="operations">Legal field operations.</param>
    /// <param name="alternatives">Legal nested or polymorphic alternative names, when applicable.</param>
    public RecordFieldDescriptor(
        string path,
        RecordValueKind kind,
        bool isNullable,
        bool isFlags,
        IEnumerable<string>? choices,
        IEnumerable<string>? referenceTargets,
        IEnumerable<RecordCollectionOperation> operations,
        IEnumerable<string>? alternatives = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentNullException.ThrowIfNull(operations);
        Path = path;
        Kind = kind;
        IsNullable = isNullable;
        IsFlags = isFlags;
        Choices = choices?.ToArray() ?? [];
        ReferenceTargets = referenceTargets?.ToArray() ?? [];
        Operations = operations.Distinct().ToArray();
        Alternatives = alternatives?.ToArray() ?? [];
        if (Operations.Count == 0)
        {
            throw new ArgumentException("An editable field must expose at least one legal operation.", nameof(operations));
        }
    }

    /// <summary>Gets the stable discovered field path.</summary>
    public string Path { get; }

    /// <summary>Gets the field's closed value kind.</summary>
    public RecordValueKind Kind { get; }

    /// <summary>Gets whether an explicit null is legal.</summary>
    public bool IsNullable { get; }

    /// <summary>Gets whether enum choices may be combined as flags.</summary>
    public bool IsFlags { get; }

    /// <summary>Gets the legal enum names.</summary>
    public IReadOnlyList<string> Choices { get; }

    /// <summary>Gets the legal Mutagen getter target type names for references.</summary>
    public IReadOnlyList<string> ReferenceTargets { get; }

    /// <summary>Gets the legal operations for the field.</summary>
    public IReadOnlyList<RecordCollectionOperation> Operations { get; }

    /// <summary>Gets the legal nested or polymorphic alternative names.</summary>
    public IReadOnlyList<string> Alternatives { get; }
}
