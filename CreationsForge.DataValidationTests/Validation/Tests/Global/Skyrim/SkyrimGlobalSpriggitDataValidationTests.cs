using CreationsForge.Core.Enums;

namespace CreationsForge.DataValidationTests.Validation.Tests.Global.Skyrim;

public class SkyrimGlobalSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "GLOB")]
    public void Skyrim_GLOB_ShouldMatchSpriggitSamples()
    {
        ValidateScope(SupportedGame.Skyrim, "GLOB");
    }
}
