using CreationsForge.Core.Enums;

namespace CreationsForge.DataValidationTests.Validation.Tests.Keyword.Skyrim;

public class SkyrimKeywordSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "KYWD")]
    public void Skyrim_KYWD_ShouldMatchSpriggitSamples()
    {
        ValidateScope(SupportedGame.Skyrim, "KYWD");
    }
}
