namespace CreationsForge.Specification.Records;

/// <summary>
/// Describes a strategy-backed child group that should be appended to comparison output for a record family.
/// </summary>
public sealed class RecordComparisonChildGroupSpecification
{
    /// <summary>
    /// Gets the child-row strategy that Core should execute for this group.
    /// </summary>
    public required RecordComparisonChildGroupKind GroupKind { get; init; }

    /// <summary>
    /// Gets the comparison group row name that should be produced by the selected strategy.
    /// </summary>
    public required string GroupName { get; init; }

    /// <summary>
    /// Gets a concise explanation of the child-row data source and current strategy boundary.
    /// </summary>
    public string Description { get; init; } = string.Empty;
}
