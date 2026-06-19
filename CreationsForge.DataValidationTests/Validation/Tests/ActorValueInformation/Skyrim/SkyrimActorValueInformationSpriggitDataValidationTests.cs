using CreationsForge.Core.Enums;

namespace CreationsForge.DataValidationTests.Validation.Tests.ActorValueInformation.Skyrim;

public class SkyrimActorValueInformationSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "AVIF")]
    public void Skyrim_AVIF_ShouldMatchSpriggitSamples()
    {
        ValidateScope(SupportedGame.Skyrim, "AVIF");
    }
}
