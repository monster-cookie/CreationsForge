using CreationsForge.Core.Enums;

namespace CreationsForge.DataValidationTests.Validation.Tests.Perk.Starfield;

public class StarfieldPerkSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "PERK")]
    public void Starfield_PERK_ShouldMatchSpriggitSamples()
    {
        ValidateScope(SupportedGame.Starfield, "PERK");
    }
}
