using CreationsForge.Core.Enums;

namespace CreationsForge.DataValidationTests.Validation.Tests.Static.Fallout4;

public class Fallout4StaticSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "STAT")]
    public void Fallout4_STAT_ShouldMatchSpriggitSamples()
    {
        ValidateScope(SupportedGame.Fallout4, "STAT");
    }
}
