using SFRecordCompareEngine.Core.DTOs.Plugins;
using Shouldly;

namespace SFRecordCompareEngine.UnitTests.DTOs.Plugins;

public class PluginImportResultDTOTests
{
    [Fact]
    public void Constructor_DefaultsCountersToZero()
    {
        var sut = new PluginImportResultDTO();

        sut.PluginsDiscovered.ShouldBe(0);
        sut.PluginsUnchanged.ShouldBe(0);
        sut.PluginsChanged.ShouldBe(0);
        sut.PluginsImported.ShouldBe(0);
        sut.PluginsMissing.ShouldBe(0);
        sut.PluginsFailed.ShouldBe(0);
        sut.PluginsUnsupported.ShouldBe(0);
        sut.PluginsInvalidated.ShouldBe(0);
        sut.MasterReferencesImported.ShouldBe(0);
        sut.RecordHeadersImported.ShouldBe(0);
        sut.TypedRecordDetailRowsImported.ShouldBe(0);
        sut.FormListItemsImported.ShouldBe(0);
        sut.RecordImportFailures.ShouldBe(0);
        sut.UnsupportedRecordTypes.ShouldBe(0);
    }
}
