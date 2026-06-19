namespace CreationsForge.DataValidationTests.Validation.Models;

public enum SpriggitValidationCategory
{
    Match,
    EquivalentAfterNormalization,
    MissingInCreationsForge,
    MissingInSpriggit,
    ValueMismatch,
    TypeMismatch,
    CollectionCountMismatch,
    CollectionOrderMismatch,
    UnsupportedOrExcluded,
    ApprovedDifference,
    Error
}
