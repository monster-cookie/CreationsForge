using CreationsForge.Core.Enums;

namespace CreationsForge.DataValidationTests.Validation.Tests.FormList.Fallout4;

public class Fallout4FormListSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "FLST")]
    public void Fallout4_FLST_ShouldMatchSpriggitSamples()
    {
        ValidateScope(SupportedGame.Fallout4, "FLST");
    }
}
