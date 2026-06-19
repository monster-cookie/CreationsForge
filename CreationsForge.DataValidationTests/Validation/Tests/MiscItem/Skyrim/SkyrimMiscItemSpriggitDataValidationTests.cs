using CreationsForge.Core.Enums;

namespace CreationsForge.DataValidationTests.Validation.Tests.MiscItem.Skyrim;

public class SkyrimMiscItemSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "MISC")]
    public void Skyrim_MISC_ShouldMatchSpriggitSamples()
    {
        ValidateScope(SupportedGame.Skyrim, "MISC");
    }
}
