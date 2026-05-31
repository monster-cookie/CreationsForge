using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.Enums;
using Shouldly;

namespace SFRecordCompareEngine.UnitTests.DTOs.Plugins;

public class PluginListItemDTOTests
{
    [Theory]
    [InlineData("Failed", true)]
    [InlineData("failed", true)]
    [InlineData("Current", false)]
    public void IsFailed_ReturnsExpectedValue(string importState, bool expected)
    {
        var sut = new PluginListItemDTO
        {
            ModKey = "Example.esm",
            ImportState = importState
        };

        sut.IsFailed.ShouldBe(expected);
    }

    [Fact]
    public void Constructor_DefaultsImportStateToCurrent()
    {
        var sut = new PluginListItemDTO
        {
            ModKey = "Example.esm"
        };

        sut.ImportState.ShouldBe(nameof(PluginImportState.Current));
    }
}
