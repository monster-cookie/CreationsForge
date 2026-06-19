using CreationsForge.Core.Enums;

namespace CreationsForge.DataValidationTests.Validation.Tests.MiscItem.Fallout4;

public class Fallout4MiscItemSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "MISC")]
    public void Fallout4_MISC_ShouldMatchSpriggitSamples()
    {
        ValidateScope(SupportedGame.Fallout4, "MISC");
    }
}
