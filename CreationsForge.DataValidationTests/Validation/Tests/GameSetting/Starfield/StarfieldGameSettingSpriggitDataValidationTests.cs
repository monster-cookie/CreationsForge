using CreationsForge.Core.Enums;

namespace CreationsForge.DataValidationTests.Validation.Tests.GameSetting.Starfield;

public class StarfieldGameSettingSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "GMST")]
    public void Starfield_GMST_ShouldMatchSpriggitSamples()
    {
        ValidateScope(SupportedGame.Starfield, "GMST");
    }
}
