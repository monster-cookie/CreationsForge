using CreationsForge.Specification.Records;

namespace CreationsForge.Core.Helpers;

/// <summary>
/// Describes record-family identity metadata in the Core boundary shape used by existing services and tests.
/// </summary>
public class RecordTypeData
{
    /// <summary>
    /// Gets or sets the typed detail table name for the record family.
    /// </summary>
    public string TableName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the canonical CreationsForge record type name.
    /// </summary>
    public string RecordType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Bethesda record identifier.
    /// </summary>
    public string RecordID { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user-facing record-family name.
    /// </summary>
    public string FriendlyName { get; set; } = string.Empty;

    /// <summary>
    /// Gets the standard display label used by record-type selectors and diagnostics.
    /// </summary>
    public string DisplayLabel => $"{FriendlyName} ({RecordID})";

    /// <summary>
    /// Creates a Core record-type shape from specification-owned record metadata.
    /// </summary>
    /// <param name="specification">The production record specification to adapt.</param>
    /// <returns>A Core-compatible record-type metadata object.</returns>
    public static RecordTypeData FromSpecification(RecordSpecification specification)
    {
        return new RecordTypeData
        {
            TableName = specification.TableName,
            RecordType = specification.RecordType,
            RecordID = specification.RecordID,
            FriendlyName = specification.FriendlyName
        };
    }
}
