namespace CreationsForge.DataValidationTests.Validation.Specs;

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
    /// Compare numeric values using a six-decimal invariant representation for fields that were previously asserted
    /// with floating-point tolerance.
    /// </summary>
    DecimalNumber
}
