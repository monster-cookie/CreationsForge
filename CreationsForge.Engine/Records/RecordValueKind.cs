namespace CreationsForge.Engine.Records;

/// <summary>Identifies the closed transport shape of one editable field value.</summary>
public enum RecordValueKind
{
    /// <summary>An explicit null value.</summary>
    Null,

    /// <summary>A Unicode string.</summary>
    String,

    /// <summary>A Boolean value.</summary>
    Boolean,

    /// <summary>A signed integer value.</summary>
    SignedInteger,

    /// <summary>An unsigned integer value.</summary>
    UnsignedInteger,

    /// <summary>A floating-point value.</summary>
    FloatingPoint,

    /// <summary>A named Mutagen enum or flags value.</summary>
    Enum,

    /// <summary>A FormKey reference.</summary>
    FormLink,

    /// <summary>An ordered collection whose duplicate values remain significant.</summary>
    List,

    /// <summary>A Mutagen translated string with one selected target language.</summary>
    TranslatedString,

    /// <summary>A bounded nested record object or polymorphic choice.</summary>
    Object,
}
