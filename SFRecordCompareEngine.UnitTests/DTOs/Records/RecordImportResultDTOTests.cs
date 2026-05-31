using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Results;
using Shouldly;

namespace SFRecordCompareEngine.UnitTests.DTOs.Records;

public class RecordImportResultDTOTests
{
    [Fact]
    public void Constructor_AssignsModKey()
    {
        var modKey = new ModKey("Example", ModType.Master);

        var sut = new RecordImportResultDTO
        {
            ModKey = modKey
        };

        sut.ModKey.ShouldBe(modKey);
        sut.RecordTypes.ShouldBeEmpty();
    }

    [Fact]
    public void AggregateProperties_ReturnSumsAndUnsupportedCount()
    {
        var sut = new RecordImportResultDTO
        {
            ModKey = new ModKey("Example", ModType.Master),
            RecordTypes = new List<RecordTypeImportResultDTO>
            {
                new()
                {
                    RecordType = "FLST",
                    HeaderImportSupported = true,
                    TypedDetailImportSupported = true,
                    HeadersImported = 1,
                    DetailRowsImported = 2,
                    FormListItemsImported = 3,
                    RecordsFailed = 4
                },
                new()
                {
                    RecordType = "GMST",
                    HeaderImportSupported = true,
                    TypedDetailImportSupported = false,
                    HeadersImported = 5,
                    DetailRowsImported = 6,
                    FormListItemsImported = 7,
                    RecordsFailed = 8
                }
            }
        };

        sut.HeadersImported.ShouldBe(6);
        sut.DetailRowsImported.ShouldBe(8);
        sut.FormListItemsImported.ShouldBe(10);
        sut.RecordsFailed.ShouldBe(12);
        sut.UnsupportedRecordTypes.ShouldBe(1);
    }
}