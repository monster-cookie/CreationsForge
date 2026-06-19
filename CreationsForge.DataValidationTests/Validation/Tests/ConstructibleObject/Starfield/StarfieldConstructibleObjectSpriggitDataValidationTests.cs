using CreationsForge.Core.Enums;

namespace CreationsForge.DataValidationTests.Validation.Tests.ConstructibleObject.Starfield;

public class StarfieldConstructibleObjectSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "COBJ")]
    public void Starfield_COBJ_ShouldMatchSpriggitSamples()
    {
        ValidateScope(SupportedGame.Starfield, "COBJ");
    }
}
