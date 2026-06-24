namespace CreationsForge.DataValidationTests.Validation.Specs;

public enum ValidationRuleKind
{
    Field,
    OptionalField,
    FormKeyObjectField,
    PathPrefix,
    FormKeyList,
    ScalarList,
    TranslatedField,
    SoundSlot,
    RawPayloadSlot,
    DtoExpectedValue,
    DtoDefaultWhenSpriggitAbsent,
    DtoNonEmpty,
    SpriggitAbsent,
    IgnoreSpriggit,
    IgnoreSpriggitPrefix,
    IgnoreDto,
    IgnoreDtoPrefix
}
