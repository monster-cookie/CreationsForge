using CreationsForge.Core.Enums;

namespace CreationsForge.DataValidationTests.Validation.Tests.MagicEffect.Fallout4;

public class Fallout4MagicEffectSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "MGEF")]
    public void Fallout4_MGEF_ShouldMatchSpriggitSamples()
    {
        ValidateScope(SupportedGame.Fallout4, "MGEF");
    }
}
