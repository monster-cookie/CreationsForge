using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using Shouldly;

namespace SFRecordCompareEngine.UnitTests.DTOs.Plugins;

public class PluginDataDTOTests
{
    [Fact]
    public void PluginLoadOrderEntryDTO_AssignsProperties()
    {
        var modKey = new ModKey("Example", ModType.Master);
        var sut = new PluginLoadOrderEntryDTO
        {
            ModKey = modKey,
            LoadOrderIndex = 3
        };

        sut.ModKey.ShouldBe(modKey);
        sut.LoadOrderIndex.ShouldBe(3);
        sut.Enabled.ShouldBeTrue();
    }

    [Fact]
    public void PluginImportProgressDTO_AssignsProperties()
    {
        var modKey = new ModKey("Example", ModType.Master);
        var sut = new PluginImportProgressDTO
        {
            CurrentPluginName = "Example.esm",
            CurrentModKey = modKey,
            PluginIndex = 1,
            PluginCount = 2,
            CurrentRecordType = "FLST",
            RecordIndex = 3,
            RecordCount = 4,
            StatusText = "Importing",
            IsIndeterminate = true
        };

        sut.CurrentPluginName.ShouldBe("Example.esm");
        sut.CurrentModKey.ShouldBe(modKey);
        sut.PluginIndex.ShouldBe(1);
        sut.PluginCount.ShouldBe(2);
        sut.CurrentRecordType.ShouldBe("FLST");
        sut.RecordIndex.ShouldBe(3);
        sut.RecordCount.ShouldBe(4);
        sut.StatusText.ShouldBe("Importing");
        sut.IsIndeterminate.ShouldBeTrue();
    }

    [Fact]
    public void PluginSourceInfoDTO_AssignsProperties()
    {
        var sut = new PluginSourceInfoDTO
        {
            Exists = true,
            LastWriteUTCTicks = 123,
            FileSizeBytes = 456
        };

        sut.Exists.ShouldBeTrue();
        sut.LastWriteUTCTicks.ShouldBe(123);
        sut.FileSizeBytes.ShouldBe(456);
    }

    [Fact]
    public void StarfieldPluginMetadataDTO_DefaultsAndAssignsProperties()
    {
        var modKey = new ModKey("Example", ModType.Master);
        var master = new ModKey("Master", ModType.Master);
        var sut = new StarfieldPluginMetadataDTO
        {
            ModKey = modKey,
            HeaderFlags = (StarfieldModHeader.HeaderFlag)7,
            FormVersion = 44,
            InteriorCellCount = 9,
            MasterReferences = new List<ModKey> { master }
        };

        sut.ModKey.ShouldBe(modKey);
        sut.HeaderFlags.ShouldBe((StarfieldModHeader.HeaderFlag)7);
        sut.FormVersion.ShouldBe(44);
        sut.Author.ShouldBe("Unknown");
        sut.InteriorCellCount.ShouldBe(9);
        sut.MasterReferences.ShouldBe(new List<ModKey> { master });
    }
}
