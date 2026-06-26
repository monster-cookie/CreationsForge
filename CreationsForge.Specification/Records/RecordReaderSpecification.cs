namespace CreationsForge.Specification.Records;

/// <summary>
/// Describes reader-facing metadata for a record family without moving game-specific Mutagen mapping into the
/// specification project.
/// </summary>
public sealed class RecordReaderSpecification
{
    /// <summary>
    /// Gets the <c>PluginRecordSetDTO</c> collection property that receives mapped DTOs for this record family.
    /// </summary>
    public required string PluginRecordSetPropertyName { get; init; }

    /// <summary>
    /// Gets the default Mutagen mod collection property name that game adapters read for this record family.
    /// Game-specific support entries may still describe per-game collection differences when a future adapter needs
    /// them.
    /// </summary>
    public required string DefaultMutagenCollectionName { get; init; }

    /// <summary>
    /// Gets a value indicating whether current reader behavior still relies on game-adapter mapping code rather than
    /// purely declarative specification metadata.
    /// </summary>
    public bool UsesGameSpecificMapper { get; init; } = true;
}
