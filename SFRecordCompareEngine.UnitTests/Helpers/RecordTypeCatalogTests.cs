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

    [Fact]
    public void KnownMajorRecordTypes_ContainsSupportedAndUnsupportedTypesInOrdinalOrder()
    {
        var expected = RecordTypeCatalog.SupportedRecordTypes
            .Concat(RecordTypeCatalog.UnsupportedRecordTypes)
            .Distinct(StringComparer.Ordinal)
            .OrderBy(recordType => recordType, StringComparer.Ordinal)
            .ToList();

        RecordTypeCatalog.KnownMajorRecordTypes.ShouldBe(expected);
        RecordTypeCatalog.KnownMajorRecordTypes.ShouldContain("FormList");
        RecordTypeCatalog.KnownMajorRecordTypes.ShouldContain("GameSetting");
    }
}
