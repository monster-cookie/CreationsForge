using CreationsForge.Core.Enums;

namespace CreationsForge.DataValidationTests.Validation.Tests.Container.Fallout4;

public class Fallout4ContainerSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "CONT")]
    public void Fallout4_CONT_ShouldMatchSpriggitSamples()
    {
        ValidateScope(SupportedGame.Fallout4, "CONT");
    }
}
