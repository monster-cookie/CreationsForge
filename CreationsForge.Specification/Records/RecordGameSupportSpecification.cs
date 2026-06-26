namespace CreationsForge.Specification.Records;

/// <summary>
/// Identifies a game in production record specifications without requiring the specification project to reference
/// CreationsForge.Core.
/// </summary>
public enum SpecificationGame
{
    /// <summary>
    /// Indicates the Starfield game adapter.
    /// </summary>
    Starfield,

    /// <summary>
    /// Indicates the Fallout 4 game adapter.
    /// </summary>
    Fallout4,

    /// <summary>
    /// Indicates the Skyrim game adapter.
    /// </summary>
    Skyrim
}

/// <summary>
/// Describes how one record family is supported for a specific game adapter.
/// </summary>
public sealed class RecordGameSupportSpecification
{
    /// <summary>
    /// Gets the game adapter that can read, import, or compare the record family.
    /// </summary>
    public required SpecificationGame Game { get; init; }

    /// <summary>
    /// Gets the Mutagen mod collection property name used by the game adapter when it discovers this record family.
    /// </summary>
    public required string MutagenCollectionName { get; init; }

    /// <summary>
    /// Gets the Spriggit record-directory or record-family name used when validation maps samples to this record
    /// family.
    /// </summary>
    public required string SpriggitRecordDirectoryName { get; init; }

    /// <summary>
    /// Gets a value indicating whether the current production import pipeline supports typed detail import for this
    /// game and record family.
    /// </summary>
    public bool IsImportSupported { get; init; } = true;

    /// <summary>
    /// Gets a value indicating whether the current production comparison pipeline supports this game and record
    /// family.
    /// </summary>
    public bool IsComparisonSupported { get; init; } = true;
}
