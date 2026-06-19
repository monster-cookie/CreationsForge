using CreationsForge.Core.Enums;

namespace CreationsForge.DataValidationTests.Validation.Tests.Keyword.Fallout4;

public class Fallout4KeywordSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "KYWD")]
    public void Fallout4_KYWD_ShouldMatchSpriggitSamples()
    {
        ValidateScope(SupportedGame.Fallout4, "KYWD");
    }
}
