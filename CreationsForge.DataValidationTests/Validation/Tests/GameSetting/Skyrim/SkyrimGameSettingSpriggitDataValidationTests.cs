using CreationsForge.Core.Enums;

namespace CreationsForge.DataValidationTests.Validation.Tests.GameSetting.Skyrim;

public class SkyrimGameSettingSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "GMST")]
    public void Skyrim_GMST_ShouldMatchSpriggitSamples()
    {
        ValidateScope(SupportedGame.Skyrim, "GMST");
    }
}
