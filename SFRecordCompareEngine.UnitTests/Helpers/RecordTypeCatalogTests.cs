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

}
