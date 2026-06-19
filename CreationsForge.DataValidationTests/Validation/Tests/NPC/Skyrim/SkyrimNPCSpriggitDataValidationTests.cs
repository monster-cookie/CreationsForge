using CreationsForge.Core.Enums;

namespace CreationsForge.DataValidationTests.Validation.Tests.NPC.Skyrim;

public class SkyrimNPCSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "NPC_")]
    public void Skyrim_NPC_ShouldMatchSpriggitSamples()
    {
        ValidateScope(SupportedGame.Skyrim, "NPC_");
    }
}
