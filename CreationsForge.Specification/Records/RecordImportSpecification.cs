namespace CreationsForge.Specification.Records;

/// <summary>
/// Describes how a record family participates in the shared Core import dispatch workflow.
/// </summary>
public sealed class RecordImportSpecification
{
    /// <summary>
    /// Gets the <c>PluginRecordSetDTO</c> property name that contains the mapped DTOs for this record family.
    /// </summary>
    public required string PluginRecordSetPropertyName { get; init; }

    /// <summary>
    /// Gets a value indicating whether the record family should always produce an import result entry even when no
    /// rows are mapped and no typed detail importer is registered.
    /// </summary>
    public bool IsRequired { get; init; } = true;
}
