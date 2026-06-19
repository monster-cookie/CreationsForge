using CreationsForge.Core.Enums;

namespace CreationsForge.DataValidationTests.Validation.Tests.Keyword.Starfield;

public class StarfieldKeywordSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "KYWD")]
    public void Starfield_KYWD_ShouldMatchSpriggitSamples()
    {
        ValidateScope(SupportedGame.Starfield, "KYWD");
    }
}
