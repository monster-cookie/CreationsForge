using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;
using SFRecordCompareEngine.Core.DTOs.Records;
using Shouldly;

namespace SFRecordCompareEngine.UnitTests.Models.Database;

public class GameSettingTests
{
    [Fact]
    public void Constructor_MapsDTO()
    {
        var modKey = new ModKey("Example", ModType.Master);
        var formKey = new FormKey(modKey, 123);
        var importedAtUTC = new DateTime(2026, 1, 2, 3, 4, 5, DateTimeKind.Utc);
        var dto = new GameSettingDTO
        {
            ModKey = modKey,
            FormKey = formKey,
            EditorID = "Editor",
            FormVersion = 44,
            StarfieldMajorRecordFlags = (StarfieldMajorRecord.StarfieldMajorRecordFlag)1,
            Version2 = 2,
            VersionControl = 3,
            ImportedAtUTC = importedAtUTC,
            SettingType = "GameSettingFloat",
            TitleString = "Title",
            Data = "1.5",
            RawData = 1.5,
            XALG = 4,
            IsCompressed = 0,
            IsDeleted = 1
        };

        var result = new SFRecordCompareEngine.Core.Models.Database.GameSetting(dto);

        result.ModKeyName.ShouldBe(modKey.Name);
        result.ModKeyType.ShouldBe((int)modKey.Type);
        result.ModKeyFileName.ShouldBe(modKey.FileName);
        result.FormKeyId.ShouldBe((int)formKey.ID);
        result.EditorId.ShouldBe("Editor");
        result.FormVersion.ShouldBe(44);
        result.StarfieldMajorRecordFlags.ShouldBe(1);
        result.Version2.ShouldBe(2);
        result.VersionControl.ShouldBe(3);
        result.ImportedAtUTC.ShouldBe(importedAtUTC);
        result.SettingType.ShouldBe("GameSettingFloat");
        result.TitleString.ShouldBe("Title");
        result.Data.ShouldBe("1.5");
        result.RawData.ShouldBe(1.5);
        result.XALG.ShouldBe(4);
        result.IsCompressed.ShouldBe(0);
        result.IsDeleted.ShouldBe(1);
    }
}
