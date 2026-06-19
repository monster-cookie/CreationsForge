using CreationsForge.Core.Enums;

namespace CreationsForge.DataValidationTests.Validation.Tests.Global.Fallout4;

public class Fallout4GlobalSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "GLOB")]
    public void Fallout4_GLOB_ShouldMatchSpriggitSamples()
    {
        ValidateScope(SupportedGame.Fallout4, "GLOB");
    }
}
