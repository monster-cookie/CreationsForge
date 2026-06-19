using CreationsForge.Core.Enums;

namespace CreationsForge.DataValidationTests.Validation.Tests.Faction.Skyrim;

public class SkyrimFactionSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "FACT")]
    public void Skyrim_FACT_ShouldMatchSpriggitSamples()
    {
        ValidateScope(SupportedGame.Skyrim, "FACT");
    }
}
