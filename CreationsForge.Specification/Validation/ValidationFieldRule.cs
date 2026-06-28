namespace CreationsForge.Specification.Validation;

/// <summary>
/// Describes one Spriggit-to-DTO field relationship or coverage rule used by validation specs.
/// </summary>
public class ValidationFieldRule
{
    public ValidationRuleKind Kind { get; private init; }

    public string SpriggitPath { get; private init; } = string.Empty;

    public string DtoPath { get; private init; } = string.Empty;

    public string ExpectedValue { get; private init; } = string.Empty;

    public bool AllowEmptyExpectedValue { get; private init; }

    public string Reason { get; private init; } = string.Empty;

    public ValidationValueNormalizer Normalizer { get; private init; }

    public bool RequireAllTranslatedLanguages { get; private init; }

    public IReadOnlyDictionary<string, string> PathReplacements { get; private init; } =
        new Dictionary<string, string>(StringComparer.Ordinal);

    public static ValidationFieldRule Field(
        string spriggitPath,
        string dtoPath,
        ValidationValueNormalizer normalizer = ValidationValueNormalizer.None)
    {
        return new ValidationFieldRule
        {
            Kind = ValidationRuleKind.Field,
            SpriggitPath = spriggitPath,
            DtoPath = dtoPath,
            Normalizer = normalizer
        };
    }

    public static ValidationFieldRule OptionalField(
        string spriggitPath,
        string dtoPath,
        ValidationValueNormalizer normalizer = ValidationValueNormalizer.None)
    {
        return new ValidationFieldRule
        {
            Kind = ValidationRuleKind.OptionalField,
            SpriggitPath = spriggitPath,
            DtoPath = dtoPath,
            Normalizer = normalizer
        };
    }

    public static ValidationFieldRule FormKeyObjectField(string spriggitPath, string dtoPath)
    {
        return new ValidationFieldRule
        {
            Kind = ValidationRuleKind.FormKeyObjectField,
            SpriggitPath = spriggitPath,
            DtoPath = dtoPath
        };
    }

    public static ValidationFieldRule PathPrefix(
        string spriggitPath,
        string dtoPath,
        IReadOnlyDictionary<string, string> pathReplacements,
        ValidationValueNormalizer normalizer = ValidationValueNormalizer.None)
    {
        return new ValidationFieldRule
        {
            Kind = ValidationRuleKind.PathPrefix,
            SpriggitPath = spriggitPath,
            DtoPath = dtoPath,
            PathReplacements = pathReplacements,
            Normalizer = normalizer
        };
    }

    /// <summary>
    /// Creates a rule for child collections whose semantic order follows a canonical form-key sort rather than source
    /// row order.
    /// </summary>
    /// <param name="spriggitPath">The Spriggit collection root path.</param>
    /// <param name="dtoPath">The DTO collection root path.</param>
    /// <param name="pathReplacements">Path suffix replacements used to align nested Spriggit fields to DTO leaves.</param>
    /// <returns>A validation rule that compares each row by sorted form key and count.</returns>
    public static ValidationFieldRule CanonicalFormKeyCountList(
        string spriggitPath,
        string dtoPath,
        IReadOnlyDictionary<string, string> pathReplacements)
    {
        return new ValidationFieldRule
        {
            Kind = ValidationRuleKind.CanonicalFormKeyCountList,
            SpriggitPath = spriggitPath,
            DtoPath = dtoPath,
            PathReplacements = pathReplacements
        };
    }

    public static ValidationFieldRule FormKeyList(string spriggitPath, string dtoPath, string dtoLeafPath)
    {
        return new ValidationFieldRule
        {
            Kind = ValidationRuleKind.FormKeyList,
            SpriggitPath = spriggitPath,
            DtoPath = dtoPath,
            ExpectedValue = dtoLeafPath,
            AllowEmptyExpectedValue = string.IsNullOrWhiteSpace(dtoLeafPath)
        };
    }

    public static ValidationFieldRule ScalarList(
        string spriggitPath,
        string dtoPath,
        ValidationValueNormalizer normalizer = ValidationValueNormalizer.None)
    {
        return new ValidationFieldRule
        {
            Kind = ValidationRuleKind.ScalarList,
            SpriggitPath = spriggitPath,
            DtoPath = dtoPath,
            Normalizer = normalizer
        };
    }

    public static ValidationFieldRule TranslatedField(
        string spriggitPath,
        string dtoPath,
        ValidationValueNormalizer normalizer = ValidationValueNormalizer.None,
        bool requireAllLanguages = false)
    {
        return new ValidationFieldRule
        {
            Kind = ValidationRuleKind.TranslatedField,
            SpriggitPath = spriggitPath,
            DtoPath = dtoPath,
            Normalizer = normalizer,
            RequireAllTranslatedLanguages = requireAllLanguages
        };
    }

    public static ValidationFieldRule SoundSlot(
        string spriggitPath,
        string soundSlot,
        string dtoFieldName,
        ValidationValueNormalizer normalizer = ValidationValueNormalizer.None)
    {
        return new ValidationFieldRule
        {
            Kind = ValidationRuleKind.SoundSlot,
            SpriggitPath = spriggitPath,
            DtoPath = soundSlot,
            ExpectedValue = dtoFieldName,
            Normalizer = normalizer
        };
    }

    public static ValidationFieldRule DtoExpectedValue(string dtoPath, string expectedValue)
    {
        return new ValidationFieldRule
        {
            Kind = ValidationRuleKind.DtoExpectedValue,
            DtoPath = dtoPath,
            ExpectedValue = expectedValue
        };
    }

    /// <summary>
    /// Creates explicit field and metadata rules for a Spriggit component <c>REFL</c> field stored in the
    /// first-class <c>Reflections</c> DTO collection.
    /// </summary>
    /// <param name="componentIndex">The zero-based Spriggit component index that owns the <c>REFL</c> field.</param>
    /// <param name="reflectionIndex">The zero-based DTO reflection index for the same parent record.</param>
    /// <param name="reflectionCount">The expected number of reflection rows on the parent DTO.</param>
    /// <param name="componentType">The Spriggit component type name associated with the <c>REFL</c> field.</param>
    /// <returns>The validation rules that connect the Spriggit field to the typed reflection DTO row.</returns>
    public static IReadOnlyList<ValidationFieldRule> ComponentReflection(
        int componentIndex,
        int reflectionIndex,
        int reflectionCount,
        string componentType)
    {
        var componentPath = "Components[" + componentIndex.ToString(System.Globalization.CultureInfo.InvariantCulture) + "]";
        var reflectionPath = "Reflections[" + reflectionIndex.ToString(System.Globalization.CultureInfo.InvariantCulture) + "]";
        return new List<ValidationFieldRule>
        {
            Field(componentPath + ".REFL", reflectionPath + ".REFL", ValidationValueNormalizer.HexPayload),
            Field(componentPath + ".MutagenObjectType", reflectionPath + ".ComponentType"),
            OptionalField(componentPath + ".MutagenObjectType", componentPath + ".MutagenObjectType"),
            DtoExpectedValue("Reflections.Count", reflectionCount.ToString(System.Globalization.CultureInfo.InvariantCulture)),
            DtoExpectedValue(reflectionPath + ".ComponentType", componentType),
            DtoExpectedValue(reflectionPath + ".SourcePath", componentPath + ".REFL")
        };
    }

    public static ValidationFieldRule DtoDefaultWhenSpriggitAbsent(
        string spriggitPath,
        string dtoPath,
        string expectedValue,
        string reason,
        bool allowEmptyExpectedValue = false)
    {
        return new ValidationFieldRule
        {
            Kind = ValidationRuleKind.DtoDefaultWhenSpriggitAbsent,
            SpriggitPath = spriggitPath,
            DtoPath = dtoPath,
            ExpectedValue = expectedValue,
            AllowEmptyExpectedValue = allowEmptyExpectedValue,
            Reason = reason
        };
    }

    public static ValidationFieldRule DtoNonEmpty(string dtoPath)
    {
        return new ValidationFieldRule
        {
            Kind = ValidationRuleKind.DtoNonEmpty,
            DtoPath = dtoPath
        };
    }

    public static ValidationFieldRule DtoNonEmpty(string spriggitPath, string dtoPath)
    {
        return new ValidationFieldRule
        {
            Kind = ValidationRuleKind.DtoNonEmpty,
            SpriggitPath = spriggitPath,
            DtoPath = dtoPath
        };
    }

    public static ValidationFieldRule SpriggitAbsent(string spriggitPath)
    {
        return new ValidationFieldRule
        {
            Kind = ValidationRuleKind.SpriggitAbsent,
            SpriggitPath = spriggitPath
        };
    }

    public static ValidationFieldRule IgnoreSpriggit(string spriggitPath, string reason)
    {
        return new ValidationFieldRule
        {
            Kind = ValidationRuleKind.IgnoreSpriggit,
            SpriggitPath = spriggitPath,
            Reason = reason
        };
    }

    public static ValidationFieldRule IgnoreDto(string dtoPath, string reason)
    {
        return new ValidationFieldRule
        {
            Kind = ValidationRuleKind.IgnoreDto,
            DtoPath = dtoPath,
            Reason = reason
        };
    }

    public static ValidationFieldRule IgnoreDtoPrefix(string dtoPath, string reason)
    {
        return new ValidationFieldRule
        {
            Kind = ValidationRuleKind.IgnoreDtoPrefix,
            DtoPath = dtoPath,
            Reason = reason
        };
    }
}
