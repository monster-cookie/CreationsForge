namespace CreationsForge.Specification.Records;

/// <summary>
/// Describes how a record family should contribute rows to the Core comparison DTO.
/// </summary>
public sealed class RecordComparisonSpecification
{
    /// <summary>
    /// Gets a value indicating whether the shared record header rows should be included before type-specific rows.
    /// </summary>
    public bool IncludeCommonFields { get; init; } = true;

    /// <summary>
    /// Gets the type-specific scalar or simple collection comparison fields for the record family.
    /// </summary>
    public IReadOnlyList<RecordComparisonFieldSpecification> Fields { get; init; } =
        new List<RecordComparisonFieldSpecification>();

    /// <summary>
    /// Gets strategy-backed child-row groups that should be appended after scalar comparison fields.
    /// </summary>
    public IReadOnlyList<RecordComparisonChildGroupSpecification> ChildGroups { get; init; } =
        new List<RecordComparisonChildGroupSpecification>();

    /// <summary>
    /// Gets strategy names for child-row groups that are expected to remain implemented by focused comparison helpers
    /// until their shapes are proven generic.
    /// </summary>
    public IReadOnlyList<string> ChildGroupStrategies { get; init; } = [];
}
