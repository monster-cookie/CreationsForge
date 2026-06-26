namespace CreationsForge.Specification.Records;

/// <summary>
/// Describes the broad value shape used by record specifications when they identify source, import, and comparison
/// fields.
/// </summary>
public enum RecordFieldValueKind
{
    /// <summary>
    /// Indicates that the field contains textual data that does not need special formatting beyond normal string
    /// display.
    /// </summary>
    Text,

    /// <summary>
    /// Indicates that the field contains numeric data whose display may require deterministic numeric formatting.
    /// </summary>
    Number,

    /// <summary>
    /// Indicates that the field contains a decomposable Bethesda FormKey reference.
    /// </summary>
    FormKey,

    /// <summary>
    /// Indicates that the field contains translation-table-backed text with language-specific values.
    /// </summary>
    LocalizedString,

    /// <summary>
    /// Indicates that the field contains a flag set or formatted flag payload.
    /// </summary>
    FlagSet,

    /// <summary>
    /// Indicates that the field contains an indexed or keyed child collection.
    /// </summary>
    Collection,

    /// <summary>
    /// Indicates that the field contains a structured object whose specific rows are handled by a strategy or child
    /// specification.
    /// </summary>
    Object
}

/// <summary>
/// Describes one canonical record field that a production record specification can expose for import, validation, or
/// comparison planning.
/// </summary>
public sealed class RecordFieldSpecification
{
    /// <summary>
    /// Gets the canonical CreationsForge DTO field name. This should align with Spriggit and Mutagen terminology
    /// whenever their names are stable.
    /// </summary>
    public required string FieldName { get; init; }

    /// <summary>
    /// Gets the Spriggit YAML path for the field when it differs from, or usefully clarifies, the canonical field
    /// name.
    /// </summary>
    public string? SpriggitPath { get; init; }

    /// <summary>
    /// Gets the broad value kind so shared import, validation, and comparison infrastructure can choose appropriate
    /// formatting and alignment behavior.
    /// </summary>
    public RecordFieldValueKind ValueKind { get; init; } = RecordFieldValueKind.Text;

    /// <summary>
    /// Gets a value indicating whether the field represents an indexed or keyed collection rather than a scalar
    /// property.
    /// </summary>
    public bool IsCollection { get; init; }

    /// <summary>
    /// Gets a value indicating whether the field should be resolved through localized string handling when display
    /// text is produced.
    /// </summary>
    public bool IsLocalized { get; init; }

    /// <summary>
    /// Gets a concise explanation of any important mapping behavior, nullable behavior, or strategy boundary for the
    /// field.
    /// </summary>
    public string Description { get; init; } = string.Empty;
}
