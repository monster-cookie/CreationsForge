using CreationsForge.Core.Enums;

namespace CreationsForge.DataValidationTests.Validation.Tests.Class.Skyrim;

public class SkyrimClassSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "CLAS")]
    public void Skyrim_CLAS_ShouldMatchSpriggitSamples()
    {
        ValidateScope(SupportedGame.Skyrim, "CLAS");
    }
}
