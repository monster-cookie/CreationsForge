using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.Models.Database;
using Shouldly;

namespace SFRecordCompareEngine.UnitTests.DTOs.Plugins;

public class PluginMasterReferenceDTOTests
{
    [Fact]
    public void Constructor_MapsModel()
    {
        var importedAtUTC = new DateTime(2026, 1, 2, 3, 4, 5, DateTimeKind.Utc);
        var model = new PluginMasterReference
        {
            ModKeyName = "Child",
            ModKeyType = (int)ModType.Master,
            ModKeyFileName = "Child.esm",
            ParentModKeyName = "Parent",
            ParentModKeyType = (int)ModType.Master,
            ParentModKeyFileName = "Parent.esm",
            MasterReferenceIndex = 1,
            ParentLoadOrderIndex = 2,
            ImportedAtUTC = importedAtUTC
        };

        var result = new PluginMasterReferenceDTO(model);

        result.ModKey.Name.ShouldBe("Child");
        result.ModKey.FileName.String.ShouldBe("Child.esm");
        result.ModKey.Type.ShouldBe(ModType.Master);
        result.ParentModKey.Name.ShouldBe("Parent");
        result.ParentModKey.FileName.String.ShouldBe("Parent.esm");
        result.ParentModKey.Type.ShouldBe(ModType.Master);
        result.MasterReferenceIndex.ShouldBe(1);
        result.ParentLoadOrderIndex.ShouldBe(2);
        result.ImportedAtUTC.ShouldBe(importedAtUTC);
    }

    [Fact]
    public void Constructor_WhenModKeyTypeIsInvalid_ThrowsArgumentOutOfRangeException()
    {
        var model = new PluginMasterReference
        {
            ModKeyType = -1,
            ParentModKeyType = (int)ModType.Master
        };

        Should.Throw<ArgumentOutOfRangeException>(() => new PluginMasterReferenceDTO(model));
    }

    [Fact]
    public void Constructor_WhenParentModKeyTypeIsInvalid_ThrowsArgumentOutOfRangeException()
    {
        var model = new PluginMasterReference
        {
            ModKeyType = (int)ModType.Master,
            ParentModKeyType = -1
        };

        Should.Throw<ArgumentOutOfRangeException>(() => new PluginMasterReferenceDTO(model));
    }
}
