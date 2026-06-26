namespace CreationsForge.Specification.Records;

/// <summary>
/// Describes one comparison row that can be produced for a record family.
/// </summary>
public sealed class RecordComparisonFieldSpecification
{
    /// <summary>
    /// Gets the comparison row name that should appear in the Core comparison DTO.
    /// </summary>
    public required string FieldName { get; init; }

    /// <summary>
    /// Gets the DTO source path for the comparison value. Nested properties should use dotted path notation.
    /// </summary>
    public required string SourcePath { get; init; }

    /// <summary>
    /// Gets the broad value kind used to select comparison formatting behavior.
    /// </summary>
    public RecordFieldValueKind ValueKind { get; init; } = RecordFieldValueKind.Text;

    /// <summary>
    /// Gets a value indicating whether different plugin values should affect the row's comparison state.
    /// </summary>
    public bool IsComparable { get; init; } = true;

    /// <summary>
    /// Gets a value indicating whether the comparison field should resolve localized rows using the selected record
    /// text language.
    /// </summary>
    public bool UsesLocalizedDisplay { get; init; }

    /// <summary>
    /// Gets a concise explanation of strategy boundaries or formatting behavior for the comparison row.
    /// </summary>
    public string Description { get; init; } = string.Empty;
}
