namespace CreationsForge.Specification.Records;

/// <summary>
/// Identifies a comparison child-row strategy that can be selected by record-family metadata while the row-building
/// implementation remains owned by Core.
/// </summary>
public enum RecordComparisonChildGroupKind
{
    /// <summary>
    /// Indicates that persisted keyword mapping rows should be rendered as the shared <c>Keywords</c> child group.
    /// </summary>
    KeywordMappings,

    /// <summary>
    /// Indicates that persisted sound mapping rows should be rendered as the shared <c>Sounds</c> child group.
    /// </summary>
    SoundMappings
}
