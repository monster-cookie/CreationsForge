using CreationsForge.Core.Enums;

namespace CreationsForge.DataValidationTests.Validation.Tests.Faction.Fallout4;

public class Fallout4FactionSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "FACT")]
    public void Fallout4_FACT_ShouldMatchSpriggitSamples()
    {
        ValidateScope(SupportedGame.Fallout4, "FACT");
    }
}
