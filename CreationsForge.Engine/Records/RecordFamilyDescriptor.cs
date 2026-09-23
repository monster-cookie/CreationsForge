namespace CreationsForge.Engine.Records;

/// <summary>Publishes one concrete editable Mutagen major-record family and its registered field paths.</summary>
public sealed class RecordFamilyDescriptor
{
    /// <summary>The descriptor schema version emitted by this implementation.</summary>
    public const int CurrentSchemaVersion = 1;

    /// <summary>Initializes a family descriptor.</summary>
    /// <param name="familyId">The stable family identifier.</param>
    /// <param name="getterType">The exact Mutagen getter type.</param>
    /// <param name="fields">The explicitly registered editable fields.</param>
    public RecordFamilyDescriptor(string familyId, Type getterType, IEnumerable<RecordFieldDescriptor> fields)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(familyId);
        ArgumentNullException.ThrowIfNull(getterType);
        ArgumentNullException.ThrowIfNull(fields);
        FamilyId = familyId;
        GetterType = getterType;
        Fields = fields.ToArray();
        if (Fields.Count == 0)
        {
            throw new ArgumentException("An editable family must expose at least one registered field.", nameof(fields));
        }

        if (Fields.Select(field => field.Path).Distinct(StringComparer.Ordinal).Count() != Fields.Count)
        {
            throw new ArgumentException($"Editable family '{familyId}' contains duplicate field paths.", nameof(fields));
        }
    }

    /// <summary>Gets the descriptor schema version.</summary>
    public int SchemaVersion => CurrentSchemaVersion;

    /// <summary>Gets the stable family identifier.</summary>
    public string FamilyId { get; }

    /// <summary>Gets the exact Mutagen getter type.</summary>
    public Type GetterType { get; }

    /// <summary>Gets the explicitly registered editable fields.</summary>
    public IReadOnlyList<RecordFieldDescriptor> Fields { get; }
}
