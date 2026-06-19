using CreationsForge.Core.Enums;

namespace CreationsForge.DataValidationTests.Validation.Tests.ActorValueInformation.Fallout4;

public class Fallout4ActorValueInformationSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "AVIF")]
    public void Fallout4_AVIF_ShouldMatchSpriggitSamples()
    {
        ValidateScope(SupportedGame.Fallout4, "AVIF");
    }
}
