using CreationsForge.Core.Enums;

namespace CreationsForge.DataValidationTests.Validation.Tests.Faction.Starfield;

public class StarfieldFactionSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "FACT")]
    public void Starfield_FACT_ShouldMatchSpriggitSamples()
    {
        ValidateScope(SupportedGame.Starfield, "FACT");
    }
}
