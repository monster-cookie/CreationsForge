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
        var masterModKey = new ModKey("Master", ModType.Master);
        var pluginModKey = new ModKey("Plugin", ModType.Master);
        var importedAtUTC = new DateTime(2026, 1, 2, 3, 4, 5, DateTimeKind.Utc);
        var dto = new PluginMasterReferenceDTO
        {
            MasterModKey = masterModKey,
            PluginModKey = pluginModKey,
            ImportedAtUTC = importedAtUTC
        };

        var result = new PluginMasterReference(dto);

        result.MasterModKeyName.ShouldBe(masterModKey.Name);
        result.MasterModKeyType.ShouldBe((int)masterModKey.Type);
        result.MasterModKeyFileName.ShouldBe(masterModKey.FileName);
        result.PluginModKeyName.ShouldBe(pluginModKey.Name);
        result.PluginModKeyType.ShouldBe((int)pluginModKey.Type);
        result.PluginModKeyFileName.ShouldBe(pluginModKey.FileName);
        result.ImportedAtUTC.ShouldBe(importedAtUTC);
    }
}