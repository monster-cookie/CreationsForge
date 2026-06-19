using CreationsForge.Core.Enums;

namespace CreationsForge.DataValidationTests.Validation.Tests.Global.Starfield;

public class StarfieldGlobalSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "GLOB")]
    public void Starfield_GLOB_ShouldMatchSpriggitSamples()
    {
        ValidateScope(SupportedGame.Starfield, "GLOB");
    }
}
