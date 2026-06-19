using CreationsForge.Core.Enums;

namespace CreationsForge.DataValidationTests.Validation.Tests.GameSetting.Fallout4;

public class Fallout4GameSettingSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "GMST")]
    public void Fallout4_GMST_ShouldMatchSpriggitSamples()
    {
        ValidateScope(SupportedGame.Fallout4, "GMST");
    }
}
