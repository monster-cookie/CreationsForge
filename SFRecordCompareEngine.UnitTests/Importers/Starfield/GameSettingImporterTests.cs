using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Results;
using SFRecordCompareEngine.Core.Helpers;
using SFRecordCompareEngine.Core.Importers.Starfield;
using Shouldly;

namespace SFRecordCompareEngine.UnitTests.Importers.Starfield;

public class GameSettingImporterTests
{
    [Fact]
    public void Properties_ReturnGameSettingMetadata()
    {
        var sut = new GameSettingImporter();

        sut.GameRelease.ShouldBe(GameRelease.Starfield);
        sut.RecordType.ShouldBe(new RecordType(RecordTypeCatalog.GameSetting.RecordID));
        sut.TableName.ShouldBe(RecordTypeCatalog.GameSetting.TableName);
    }

    [Fact]
    public void Import_ThrowsNotImplementedException()
    {
        var modKey = new ModKey("Example", ModType.Master);
        var formKey = new FormKey(modKey, 123);
        var sut = new GameSettingImporter();

        Should.Throw<NotImplementedException>(() => sut.Import(modKey, formKey, new RecordImportResultDTO
        {
            ModKey = modKey
        }));
    }
}
