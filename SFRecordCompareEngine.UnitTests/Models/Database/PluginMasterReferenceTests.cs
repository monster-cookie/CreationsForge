using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.Models.Database;
using Shouldly;

namespace SFRecordCompareEngine.UnitTests.Models.Database;

public class PluginMasterReferenceTests
{
    [Fact]
    public void Constructor_MapsDTO()
    {
        var modKey = new ModKey("Child", ModType.Master);
        var parentModKey = new ModKey("Parent", ModType.Master);
        var importedAtUTC = new DateTime(2026, 1, 2, 3, 4, 5, DateTimeKind.Utc);
        var dto = new PluginMasterReferenceDTO
        {
            ModKey = modKey,
            ParentModKey = parentModKey,
            MasterReferenceIndex = 1,
            ParentLoadOrderIndex = 2,
            ImportedAtUTC = importedAtUTC
        };

        var result = new PluginMasterReference(dto);

        result.ModKeyName.ShouldBe(modKey.Name);
        result.ModKeyType.ShouldBe((int)modKey.Type);
        result.ModKeyFileName.ShouldBe(modKey.FileName);
        result.ParentModKeyName.ShouldBe(parentModKey.Name);
        result.ParentModKeyType.ShouldBe((int)parentModKey.Type);
        result.ParentModKeyFileName.ShouldBe(parentModKey.FileName);
        result.MasterReferenceIndex.ShouldBe(1);
        result.ParentLoadOrderIndex.ShouldBe(2);
        result.ImportedAtUTC.ShouldBe(importedAtUTC);
    }
}
