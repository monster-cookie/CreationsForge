namespace CreationsForge.DataValidationTests.Validation.Specs;

public enum ValidationRuleKind
{
    Field,
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
    IgnoreDto,
    IgnoreDtoPrefix
}
