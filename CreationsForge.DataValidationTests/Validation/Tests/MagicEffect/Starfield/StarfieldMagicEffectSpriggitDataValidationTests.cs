using CreationsForge.Core.Enums;

namespace CreationsForge.DataValidationTests.Validation.Tests.MagicEffect.Starfield;

public class StarfieldMagicEffectSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "MGEF")]
    public void Starfield_MGEF_ShouldMatchSpriggitSamples()
    {
        ValidateScope(SupportedGame.Starfield, "MGEF");
    }
}
