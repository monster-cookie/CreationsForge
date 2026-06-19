using CreationsForge.Core.Enums;

namespace CreationsForge.DataValidationTests.Validation.Tests.ConditionForm.Starfield;

public class StarfieldConditionFormSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "CNDF")]
    public void Starfield_CNDF_ShouldMatchSpriggitSamples()
    {
        ValidateScope(SupportedGame.Starfield, "CNDF");
    }
}
