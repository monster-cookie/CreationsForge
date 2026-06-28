namespace CreationsForge.Specification.Validation;

/// <summary>
/// Opt-in normalization for equivalent source data that Spriggit and the reader DTO serialize differently.
/// </summary>
public enum ValidationValueNormalizer
{
    /// <summary>
    /// Compare the Spriggit and DTO values exactly as flattened.
    /// </summary>
    None,

    /// <summary>
    /// Compare escaped Spriggit book text against the DTO's decoded text value.
    /// </summary>
    BookText,

    /// <summary>
    /// Compare terminal body/menu text when Spriggit folded YAML scalars serialize single line breaks as spaces.
    /// </summary>
    TerminalText,

    /// <summary>
    /// Compare a Spriggit hexadecimal scalar, such as 0x20, against the DTO's decimal value, such as 32.
    /// </summary>
    HexInteger,

    /// <summary>
    /// Compare a Spriggit hexadecimal payload with a 0x prefix against the DTO payload without the prefix.
    /// </summary>
    HexPayload,

    /// <summary>
    /// Compare model paths when the reader DTO includes the implicit Meshes\ prefix that Spriggit omits.
    /// </summary>
    ModelFile,

    /// <summary>
    /// Compare Spriggit hexadecimal color values against the DTO's Mutagen color display string.
    /// </summary>
    Color,

    /// <summary>
    /// Compare Spriggit hexadecimal color values and decimal values under a mixed object tree.
    /// </summary>
    ColorOrDecimalNumber,

    /// <summary>
    /// Compare a decimal Spriggit FormID against the DTO's normalized hexadecimal FormKey string.
    /// </summary>
    DecimalFormKeyId,

    /// <summary>
    /// Compare numeric values using a six-decimal invariant representation for fields that were previously asserted
    /// with floating-point tolerance.
    /// </summary>
    DecimalNumber,

    /// <summary>
    /// Compare single-precision Mutagen readback values against Spriggit decimal scalars using stable round-trip text.
    /// </summary>
    FloatNumber,

    /// <summary>
    /// Compare structured JSON-like payloads when Spriggit folded YAML scalar formatting differs from DTO formatting.
    /// </summary>
    JsonWhitespace,

    /// <summary>
    /// Compare Starfield major record flag names when Spriggit uses game-specific names for the same stored bit.
    /// </summary>
    StarfieldMajorFlagName,

    /// <summary>
    /// Compare major flag lists against the combined integer flag value exposed by Mutagen readback.
    /// </summary>
    MajorFlagList,

    /// <summary>
    /// Compare Skyrim major flag lists against the combined integer flag value exposed by Mutagen readback.
    /// </summary>
    SkyrimMajorFlagList
}
