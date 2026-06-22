namespace CreationsForge.DataValidationTests.Validation.Specs;

public class ValidationFieldRule
{
    public ValidationRuleKind Kind { get; private init; }

    public string SpriggitPath { get; private init; } = string.Empty;

    public string DtoPath { get; private init; } = string.Empty;

    public string ExpectedValue { get; private init; } = string.Empty;

    public string Reason { get; private init; } = string.Empty;

    public ValidationValueNormalizer Normalizer { get; private init; }

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

    public static ValidationFieldRule PathPrefix(
        string spriggitPath,
        string dtoPath,
        IReadOnlyDictionary<string, string> pathReplacements)
    {
        return new ValidationFieldRule
        {
            Kind = ValidationRuleKind.PathPrefix,
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
            ExpectedValue = dtoLeafPath
        };
    }

    public static ValidationFieldRule TranslatedField(
        string spriggitPath,
        string dtoPath,
        ValidationValueNormalizer normalizer = ValidationValueNormalizer.None)
    {
        return new ValidationFieldRule
        {
            Kind = ValidationRuleKind.TranslatedField,
            SpriggitPath = spriggitPath,
            DtoPath = dtoPath,
            Normalizer = normalizer
        };
    }

    public static ValidationFieldRule SoundSlot(string spriggitPath, string soundSlot, string dtoFieldName)
    {
        return new ValidationFieldRule
        {
            Kind = ValidationRuleKind.SoundSlot,
            SpriggitPath = spriggitPath,
            DtoPath = soundSlot,
            ExpectedValue = dtoFieldName
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
}
