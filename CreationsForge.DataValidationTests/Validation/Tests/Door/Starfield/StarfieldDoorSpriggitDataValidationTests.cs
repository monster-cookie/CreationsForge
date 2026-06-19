using CreationsForge.Core.Enums;

namespace CreationsForge.DataValidationTests.Validation.Tests.Door.Starfield;

public class StarfieldDoorSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "DOOR")]
    public void Starfield_DOOR_ShouldMatchSpriggitSamples()
    {
        ValidateScope(SupportedGame.Starfield, "DOOR");
    }
}
