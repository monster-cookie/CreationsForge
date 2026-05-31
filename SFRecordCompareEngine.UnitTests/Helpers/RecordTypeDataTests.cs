using SFRecordCompareEngine.Core.Helpers;
using Shouldly;

namespace SFRecordCompareEngine.UnitTests.Helpers;

public class RecordTypeDataTests
{
    [Fact]
    public void Constructor_DefaultsStringsToEmpty()
    {
        var sut = new RecordTypeData();

        sut.TableName.ShouldBe(string.Empty);
        sut.RecordType.ShouldBe(string.Empty);
        sut.RecordID.ShouldBe(string.Empty);
    }

    [Fact]
    public void Properties_CanBeAssigned()
    {
        var sut = new RecordTypeData
        {
            TableName = "Table",
            RecordType = "Record",
            RecordID = "RCID"
        };

        sut.TableName.ShouldBe("Table");
        sut.RecordType.ShouldBe("Record");
        sut.RecordID.ShouldBe("RCID");
    }
}