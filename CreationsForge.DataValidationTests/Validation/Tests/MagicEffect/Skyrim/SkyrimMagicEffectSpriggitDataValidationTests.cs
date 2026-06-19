using CreationsForge.Core.Enums;

namespace CreationsForge.DataValidationTests.Validation.Tests.MagicEffect.Skyrim;

public class SkyrimMagicEffectSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "MGEF")]
    public void Skyrim_MGEF_ShouldMatchSpriggitSamples()
    {
        ValidateScope(SupportedGame.Skyrim, "MGEF");
    }
}
