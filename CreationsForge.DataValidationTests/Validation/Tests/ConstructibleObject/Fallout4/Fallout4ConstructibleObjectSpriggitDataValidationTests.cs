using CreationsForge.Core.Enums;

namespace CreationsForge.DataValidationTests.Validation.Tests.ConstructibleObject.Fallout4;

public class Fallout4ConstructibleObjectSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "COBJ")]
    public void Fallout4_COBJ_ShouldMatchSpriggitSamples()
    {
        ValidateScope(SupportedGame.Fallout4, "COBJ");
    }
}
