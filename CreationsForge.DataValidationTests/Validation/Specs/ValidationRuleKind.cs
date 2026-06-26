namespace CreationsForge.DataValidationTests.Validation.Specs;

public enum ValidationRuleKind
{
    Field,
    OptionalField,
    FormKeyObjectField,
    PathPrefix,
    CanonicalFormKeyCountList,
    FormKeyList,
    ScalarList,
    TranslatedField,
    SoundSlot,
    DtoExpectedValue,
    DtoDefaultWhenSpriggitAbsent,
    DtoNonEmpty,
    SpriggitAbsent,
    IgnoreSpriggit,
    IgnoreDto,
    IgnoreDtoPrefix
}
