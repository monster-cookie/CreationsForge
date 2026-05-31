using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;
using Moq;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.DTOs.Results;
using SFRecordCompareEngine.Core.Helpers;
using SFRecordCompareEngine.Core.Importers.Starfield;
using SFRecordCompareEngine.Core.Repositories.Interfaces;
using SFRecordCompareEngine.Core.Services.Interfaces;
using Shouldly;

namespace SFRecordCompareEngine.UnitTests.Importers.Starfield;

public class GameSettingImporterTests
{
    [Fact]
    public void Properties_ReturnGameSettingMetadata()
    {
        var sut = new GameSettingImporter(
            Mock.Of<IGameSettingRepository>());

        sut.GameRelease.ShouldBe(GameRelease.Starfield);
        sut.RecordType.ShouldBe(new RecordType(RecordTypeCatalog.GameSetting.RecordID));
        sut.TableName.ShouldBe(RecordTypeCatalog.GameSetting.TableName);
    }

    [Fact]
    public void Import_WhenGameSettingExists_SavesGameSetting()
    {
        var modKey = new ModKey("Example", ModType.Master);
        var formKey = new FormKey(modKey, 123);
        var repository = new Mock<IGameSettingRepository>();
        var record = new GameSettingDTO
        {
            ModKey = modKey,
            FormKey = formKey,
            EditorID = "Editor",
            FormVersion = 44,
            StarfieldMajorRecordFlags = (StarfieldMajorRecord.StarfieldMajorRecordFlag)1,
            Version2 = 2,
            VersionControl = 3,
            ImportedAtUTC = DateTime.UtcNow,
            SettingType = "GameSettingInt",
            Data = "60",
            RawData = 60,
            IsCompressed = 0,
            IsDeleted = 0
        };
        var sut = new GameSettingImporter(repository.Object);

        var result = new RecordTypeImportResultDTO
        {
            RecordType = "GMST",
            HeaderImportSupported = true,
            TypedDetailImportSupported = true
        };

        sut.Import(record, result);

        repository.Verify(x => x.Save(It.Is<GameSettingDTO>(dto =>
            dto.ModKey == modKey &&
            dto.FormKey == formKey &&
            dto.EditorID == "Editor" &&
            dto.FormVersion == 44 &&
            dto.StarfieldMajorRecordFlags == (StarfieldMajorRecord.StarfieldMajorRecordFlag)1 &&
            dto.Version2 == 2 &&
            dto.VersionControl == 3 &&
            dto.SettingType == "GameSettingInt" &&
            dto.Data == "60" &&
            dto.RawData == 60)), Times.Once);
        result.DetailRowsImported.ShouldBe(1);
    }

}
