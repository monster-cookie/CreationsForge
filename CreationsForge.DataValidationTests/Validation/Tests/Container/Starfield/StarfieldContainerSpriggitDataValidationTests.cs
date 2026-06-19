using CreationsForge.Core.Enums;

namespace CreationsForge.DataValidationTests.Validation.Tests.Container.Starfield;

public class StarfieldContainerSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "CONT")]
    public void Starfield_CONT_ShouldMatchSpriggitSamples()
    {
        ValidateScope(SupportedGame.Starfield, "CONT");
    }
}
