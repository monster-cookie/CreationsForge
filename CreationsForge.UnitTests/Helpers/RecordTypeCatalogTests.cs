using CreationsForge.Core.Helpers;
using Shouldly;

namespace CreationsForge.UnitTests.Helpers;

public class RecordTypeCatalogTests
{
    [Theory]
    [InlineData("AVIF", "Actor Value Information (AVIF)")]
    [InlineData("COBJ", "Constructible Object (COBJ)")]
    [InlineData("CONT", "Container (CONT)")]
    [InlineData("FLST", "Form List (FLST)")]
    [InlineData("GLOB", "Global (GLOB)")]
    [InlineData("GMST", "Game Setting (GMST)")]
    [InlineData("KYWD", "Keyword (KYWD)")]
    [InlineData("MGEF", "Magic Effect (MGEF)")]
    [InlineData("MISC", "Misc Item (MISC)")]
    [InlineData("NPC_", "NPC (NPC_)")]
    [InlineData("PERK", "Perk (PERK)")]
    [InlineData("STAT", "Static (STAT)")]
    [InlineData("CNDF", "Condition Form (CNDF)")]
    public void GetDisplayLabel_WhenRecordTypeIsKnown_ReturnsFriendlyNameAndSignature(string recordID, string expectedLabel)
    {
        RecordTypeCatalog.GetDisplayLabel(recordID).ShouldBe(expectedLabel);
    }

    [Fact]
    public void GetDisplayLabel_WhenRecordTypeIsUnknown_ReturnsRecordID()
    {
        RecordTypeCatalog.GetDisplayLabel("UNKN").ShouldBe("UNKN");
    }
}
