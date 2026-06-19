using CreationsForge.Core.Enums;

namespace CreationsForge.DataValidationTests.Validation.Tests.Perk.Skyrim;

public class SkyrimPerkSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "PERK")]
    public void Skyrim_PERK_ShouldMatchSpriggitSamples()
    {
        ValidateScope(SupportedGame.Skyrim, "PERK");
    }
}
