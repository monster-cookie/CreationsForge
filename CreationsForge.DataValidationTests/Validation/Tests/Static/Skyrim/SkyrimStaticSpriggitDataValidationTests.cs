using CreationsForge.Core.Enums;

namespace CreationsForge.DataValidationTests.Validation.Tests.Static.Skyrim;

public class SkyrimStaticSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "STAT")]
    public void Skyrim_STAT_ShouldMatchSpriggitSamples()
    {
        ValidateScope(SupportedGame.Skyrim, "STAT");
    }
}
