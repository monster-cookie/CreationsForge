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
            MasterModKeyName = "Master",
            MasterModKeyType = (int)ModType.Master,
            MasterModKeyFileName = "Master.esm",
            PluginModKeyName = "Plugin",
            PluginModKeyType = (int)ModType.Master,
            PluginModKeyFileName = "Plugin.esm",
            ImportedAtUTC = importedAtUTC
        };

        var result = new PluginMasterReferenceDTO(model);

        result.MasterModKey.Name.ShouldBe("Master");
        result.MasterModKey.FileName.String.ShouldBe("Master.esm");
        result.MasterModKey.Type.ShouldBe(ModType.Master);
        result.PluginModKey.Name.ShouldBe("Plugin");
        result.PluginModKey.FileName.String.ShouldBe("Plugin.esm");
        result.PluginModKey.Type.ShouldBe(ModType.Master);
        result.ImportedAtUTC.ShouldBe(importedAtUTC);
    }

    [Fact]
    public void Constructor_WhenMasterModKeyTypeIsInvalid_ThrowsArgumentOutOfRangeException()
    {
        var model = new PluginMasterReference
        {
            MasterModKeyType = -1,
            PluginModKeyType = (int)ModType.Master
        };

        Should.Throw<ArgumentOutOfRangeException>(() => new PluginMasterReferenceDTO(model));
    }

    [Fact]
    public void Constructor_WhenPluginModKeyTypeIsInvalid_ThrowsArgumentOutOfRangeException()
    {
        var model = new PluginMasterReference
        {
            MasterModKeyType = (int)ModType.Master,
            PluginModKeyType = -1
        };

        Should.Throw<ArgumentOutOfRangeException>(() => new PluginMasterReferenceDTO(model));
    }
}