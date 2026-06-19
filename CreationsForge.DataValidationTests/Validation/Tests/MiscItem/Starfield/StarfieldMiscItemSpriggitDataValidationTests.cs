using CreationsForge.Core.Enums;

namespace CreationsForge.DataValidationTests.Validation.Tests.MiscItem.Starfield;

public class StarfieldMiscItemSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "MISC")]
    public void Starfield_MISC_ShouldMatchSpriggitSamples()
    {
        ValidateScope(SupportedGame.Starfield, "MISC");
    }
}
