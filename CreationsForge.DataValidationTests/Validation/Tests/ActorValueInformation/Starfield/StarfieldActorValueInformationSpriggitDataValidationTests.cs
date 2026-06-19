using CreationsForge.Core.Enums;

namespace CreationsForge.DataValidationTests.Validation.Tests.ActorValueInformation.Starfield;

public class StarfieldActorValueInformationSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "AVIF")]
    public void Starfield_AVIF_ShouldMatchSpriggitSamples()
    {
        ValidateScope(SupportedGame.Starfield, "AVIF");
    }
}
