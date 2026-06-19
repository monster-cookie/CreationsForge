using CreationsForge.Core.Enums;

namespace CreationsForge.DataValidationTests.Validation.Tests.FormList.Skyrim;

public class SkyrimFormListSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "FLST")]
    public void Skyrim_FLST_ShouldMatchSpriggitSamples()
    {
        ValidateScope(SupportedGame.Skyrim, "FLST");
    }
}
