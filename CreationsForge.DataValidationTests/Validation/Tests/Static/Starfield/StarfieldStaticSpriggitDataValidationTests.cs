using CreationsForge.Core.Enums;

namespace CreationsForge.DataValidationTests.Validation.Tests.Static.Starfield;

public class StarfieldStaticSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "STAT")]
    public void Starfield_STAT_ShouldMatchSpriggitSamples()
    {
        ValidateScope(SupportedGame.Starfield, "STAT");
    }
}
