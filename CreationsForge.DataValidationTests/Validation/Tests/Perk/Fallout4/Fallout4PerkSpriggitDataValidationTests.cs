using CreationsForge.Core.Enums;

namespace CreationsForge.DataValidationTests.Validation.Tests.Perk.Fallout4;

public class Fallout4PerkSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "PERK")]
    public void Fallout4_PERK_ShouldMatchSpriggitSamples()
    {
        ValidateScope(SupportedGame.Fallout4, "PERK");
    }
}
