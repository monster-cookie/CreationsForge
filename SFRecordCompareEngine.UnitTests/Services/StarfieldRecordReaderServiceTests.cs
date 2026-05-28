using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.Services;
using Shouldly;

namespace SFRecordCompareEngine.UnitTests.Services;

public class StarfieldRecordReaderServiceTests
{
    [Fact]
    public void GetFormListFormKeys_WhenStarfieldEsmExists_ReturnsFormListKeys()
    {
        var sut = new StarfieldRecordReaderService();
        var plugin = CreateStarfieldPluginDTO();

        var result = sut.GetFormListFormKeys(plugin);

        result.ShouldNotBeEmpty();
        result.ShouldAllBe(formKey => formKey.ModKey.FileName.String == "Starfield.esm");
    }

    [Fact]
    public void GetFormList_WhenStarfieldEsmContainsFormList_ReturnsFormListData()
    {
        var sut = new StarfieldRecordReaderService();
        var plugin = CreateStarfieldPluginDTO();
        var formKey = sut.GetFormListFormKeys(plugin).First();

        var result = sut.GetFormList(plugin.ModKey, formKey);

        result.ShouldNotBeNull();
        result.FormKey.ShouldBe(formKey);
        result.EditorID.ShouldNotBeNull();
        result.FormVersion.ShouldBeGreaterThanOrEqualTo(0);
        result.Items.ShouldNotBeNull();
    }

    private static PluginDTO CreateStarfieldPluginDTO()
    {
        return new PluginDTO
        {
            ModKey = new ModKey("Starfield", ModType.Master)
        };
    }
}
