using SFRecordCompareEngine.Core.Helpers;
using Shouldly;

namespace SFRecordCompareEngine.UnitTests.Helpers;

public class RecordTypeCatalogTests
{
    [Fact]
    public void FormList_ReturnsExpectedRecordTypeData()
    {
        RecordTypeCatalog.FormList.TableName.ShouldBe("FormList");
        RecordTypeCatalog.FormList.RecordType.ShouldBe("FormList");
        RecordTypeCatalog.FormList.RecordID.ShouldBe("FLST");
    }

    [Fact]
    public void GameSetting_ReturnsExpectedRecordTypeData()
    {
        RecordTypeCatalog.GameSetting.TableName.ShouldBe("GameSetting");
        RecordTypeCatalog.GameSetting.RecordType.ShouldBe("GameSetting");
        RecordTypeCatalog.GameSetting.RecordID.ShouldBe("GMST");
    }

    [Theory]
    [InlineData("Global", "Global", "GLOB")]
    [InlineData("MiscItem", "MiscItem", "MISC")]
    [InlineData("Keyword", "Keyword", "KYWD")]
    [InlineData("NPC", "NPC", "NPC_")]
    [InlineData("ActorValueInformation", "ActorValueInformation", "AVIF")]
    [InlineData("MagicEffect", "MagicEffect", "MGEF")]
    [InlineData("Perk", "Perk", "PERK")]
    public void AddedRecordType_ReturnsExpectedRecordTypeData(string propertyName, string expectedName, string expectedID)
    {
        var value = typeof(RecordTypeCatalog).GetField(propertyName)?.GetValue(null) as RecordTypeData;

        value.ShouldNotBeNull();
        value.TableName.ShouldBe(expectedName);
        value.RecordType.ShouldBe(expectedName);
        value.RecordID.ShouldBe(expectedID);
    }

}
