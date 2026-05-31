using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;
using SFRecordCompareEngine.Core.DTOs.Records;
using Shouldly;
using Global = SFRecordCompareEngine.Core.Models.Database.Global;
using Perk = SFRecordCompareEngine.Core.Models.Database.Perk;

namespace SFRecordCompareEngine.UnitTests.Models.Database;

public class RecordHeaderTests
{
    [Fact]
    public void GlobalConstructor_MapsRecordHeaderAndData()
    {
        var modKey = new ModKey("Example", ModType.Master);
        var importedAtUTC = new DateTime(2026, 1, 2, 3, 4, 5, DateTimeKind.Utc);
        var dto = new GlobalDTO
        {
            ModKey = modKey,
            FormKey = new FormKey(modKey, 123),
            EditorID = "Editor",
            FormVersion = 44,
            StarfieldMajorRecordFlags = (StarfieldMajorRecord.StarfieldMajorRecordFlag)1,
            Version2 = 2,
            VersionControl = 3,
            ImportedAtUTC = importedAtUTC,
            Data = "1.5"
        };

        var model = new Global(dto);
        var result = new GlobalDTO(model);

        result.ModKey.ShouldBe(modKey);
        result.FormKey.ShouldBe(dto.FormKey);
        result.EditorID.ShouldBe("Editor");
        result.FormVersion.ShouldBe(44);
        result.StarfieldMajorRecordFlags.ShouldBe((StarfieldMajorRecord.StarfieldMajorRecordFlag)1);
        result.Version2.ShouldBe(2);
        result.VersionControl.ShouldBe(3);
        result.ImportedAtUTC.ShouldBe(importedAtUTC);
        result.Data.ShouldBe("1.5");
    }

    [Fact]
    public void PerkConstructor_MapsRecordHeaderAndName()
    {
        var modKey = new ModKey("Example", ModType.Master);
        var dto = new PerkDTO
        {
            ModKey = modKey,
            FormKey = new FormKey(modKey, 123),
            EditorID = "Editor",
            FormVersion = 44,
            StarfieldMajorRecordFlags = 0,
            Version2 = 2,
            VersionControl = 3,
            ImportedAtUTC = DateTime.UtcNow,
            Name = "Perk Name"
        };

        var result = new PerkDTO(new Perk(dto));

        result.EditorID.ShouldBe("Editor");
        result.Name.ShouldBe("Perk Name");
    }
}
