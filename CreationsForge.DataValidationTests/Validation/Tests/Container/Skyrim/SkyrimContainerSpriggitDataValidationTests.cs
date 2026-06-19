using CreationsForge.Core.Enums;

namespace CreationsForge.DataValidationTests.Validation.Tests.Container.Skyrim;

public class SkyrimContainerSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "CONT")]
    public void Skyrim_CONT_ShouldMatchSpriggitSamples()
    {
        ValidateScope(SupportedGame.Skyrim, "CONT");
    }
}
