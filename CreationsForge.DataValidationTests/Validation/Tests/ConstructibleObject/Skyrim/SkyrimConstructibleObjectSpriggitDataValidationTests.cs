using CreationsForge.Core.Enums;

namespace CreationsForge.DataValidationTests.Validation.Tests.ConstructibleObject.Skyrim;

public class SkyrimConstructibleObjectSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "COBJ")]
    public void Skyrim_COBJ_ShouldMatchSpriggitSamples()
    {
        ValidateScope(SupportedGame.Skyrim, "COBJ");
    }
}
