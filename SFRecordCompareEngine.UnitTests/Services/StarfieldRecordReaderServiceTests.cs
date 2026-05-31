using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.Services;
using Shouldly;

namespace SFRecordCompareEngine.UnitTests.Services;

public class StarfieldRecordReaderServiceTests
{
    [Fact]
    public void GetFormLists_WhenStarfieldEsmExists_ReturnsFormLists()
    {
        var sut = new StarfieldRecordReaderService();
        var plugin = CreateStarfieldPluginDTO();

        var result = sut.GetFormLists(plugin);

        result.ShouldNotBeEmpty();
        result.ShouldAllBe(formList => formList.FormKey.ModKey.FileName.String == "Starfield.esm");
    }

    [Fact]
    public void GetFormLists_WhenStarfieldEsmContainsFormList_ReturnsFormListData()
    {
        var sut = new StarfieldRecordReaderService();
        var plugin = CreateStarfieldPluginDTO();

        var result = sut.GetFormLists(plugin).First();

        result.ShouldNotBeNull();
        result.FormKey.ModKey.ShouldBe(plugin.ModKey);
        result.EditorID.ShouldNotBeNull();
        result.FormVersion.ShouldBeGreaterThanOrEqualTo(0);
        result.Items.ShouldNotBeNull();
    }

    [Fact]
    public void GetGameSettings_WhenStarfieldEsmExists_ReturnsGameSettings()
    {
        var sut = new StarfieldRecordReaderService();
        var plugin = CreateStarfieldPluginDTO();

        var result = sut.GetGameSettings(plugin);

        result.ShouldNotBeEmpty();
        result.ShouldAllBe(gameSetting => gameSetting.FormKey.ModKey.FileName.String == "Starfield.esm");
    }

    private static PluginDTO CreateStarfieldPluginDTO()
    {
        return new PluginDTO
        {
            ModKey = new ModKey("Starfield", ModType.Master)
        };
    }
}
