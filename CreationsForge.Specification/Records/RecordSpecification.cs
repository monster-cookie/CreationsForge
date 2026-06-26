namespace CreationsForge.Specification.Records;

/// <summary>
/// Describes one Bethesda record family as production metadata that can gradually drive import, validation, and
/// comparison behavior.
/// </summary>
public sealed class RecordSpecification
{
    /// <summary>
    /// Gets the four-character Bethesda major record identifier, such as <c>GLOB</c> or <c>FLST</c>.
    /// </summary>
    public required string RecordID { get; init; }

    /// <summary>
    /// Gets the canonical CreationsForge record type name.
    /// </summary>
    public required string RecordType { get; init; }

    /// <summary>
    /// Gets the current application table name used for typed detail persistence.
    /// </summary>
    public required string TableName { get; init; }

    /// <summary>
    /// Gets the user-facing friendly name for the record family.
    /// </summary>
    public required string FriendlyName { get; init; }

    /// <summary>
    /// Gets the game-specific read/import support entries for the record family.
    /// </summary>
    public IReadOnlyList<RecordGameSupportSpecification> GameSupport { get; init; } =
        new List<RecordGameSupportSpecification>();

    /// <summary>
    /// Gets the canonical fields that are intentionally exposed by the first production specification slice.
    /// </summary>
    public IReadOnlyList<RecordFieldSpecification> Fields { get; init; } =
        new List<RecordFieldSpecification>();

    /// <summary>
    /// Gets import dispatch metadata for the record family.
    /// </summary>
    public RecordImportSpecification Import { get; init; } = new()
    {
        PluginRecordSetPropertyName = string.Empty
    };

    /// <summary>
    /// Gets reader metadata that identifies the current DTO destination collection and default Mutagen collection
    /// used by game-specific reader services.
    /// </summary>
    public RecordReaderSpecification Reader { get; init; } = new()
    {
        PluginRecordSetPropertyName = string.Empty,
        DefaultMutagenCollectionName = string.Empty
    };

    /// <summary>
    /// Gets comparison metadata for the record family.
    /// </summary>
    public RecordComparisonSpecification Comparison { get; init; } = new();

    /// <summary>
    /// Gets a concise note describing whether the record family is fully spec-driven or still transitional.
    /// </summary>
    public string ImplementationNote { get; init; } = string.Empty;
}
