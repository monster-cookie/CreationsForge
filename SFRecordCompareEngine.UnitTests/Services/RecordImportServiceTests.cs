using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Moq;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.DTOs.Results;
using SFRecordCompareEngine.Core.Importers.Interfaces;
using SFRecordCompareEngine.Core.Services;
using SFRecordCompareEngine.Core.Services.Interfaces;
using Shouldly;

namespace SFRecordCompareEngine.UnitTests.Services;

public class RecordImportServiceTests
{
    [Fact]
    public void ImportPluginRecords_WhenNoFormLists_ReturnsResultForPlugin()
    {
        var plugin = CreatePluginDTO();
        var reader = new Mock<IStarfieldRecordReaderService>();
        reader.Setup(x => x.GetFormListFormKeys(plugin)).Returns(new List<FormKey>());
        var sut = new RecordImportService(Array.Empty<ITypedRecordDetailImporter>(), reader.Object);

        var result = sut.ImportPluginRecords(plugin, CancellationToken.None);

        result.ModKey.ShouldBe(plugin.ModKey);
        result.RecordTypes.ShouldBeEmpty();
    }

    [Fact]
    public void ImportPluginRecords_WhenFormListImporterIsMissing_DoesNotImportFormLists()
    {
        var plugin = CreatePluginDTO();
        var formKey = new FormKey(plugin.ModKey, 123);
        var reader = new Mock<IStarfieldRecordReaderService>();
        reader.Setup(x => x.GetFormListFormKeys(plugin)).Returns(new List<FormKey> { formKey });
        var sut = new RecordImportService(Array.Empty<ITypedRecordDetailImporter>(), reader.Object);

        var result = sut.ImportPluginRecords(plugin, CancellationToken.None);

        result.ModKey.ShouldBe(plugin.ModKey);
        result.RecordTypes.ShouldBeEmpty();
    }

    [Fact]
    public void ImportPluginRecords_WhenFormListImporterExists_ImportsEachFormList()
    {
        var plugin = CreatePluginDTO();
        var firstFormKey = new FormKey(plugin.ModKey, 123);
        var secondFormKey = new FormKey(plugin.ModKey, 456);
        var reader = new Mock<IStarfieldRecordReaderService>();
        reader.Setup(x => x.GetFormListFormKeys(plugin)).Returns(new List<FormKey> { firstFormKey, secondFormKey });
        var importer = new Mock<ITypedRecordDetailImporter>();
        importer.SetupGet(x => x.GameRelease).Returns(GameRelease.Starfield);
        importer.SetupGet(x => x.RecordType).Returns(new RecordType("FLST"));
        var sut = new RecordImportService(new[] { importer.Object }, reader.Object);

        var result = sut.ImportPluginRecords(plugin, CancellationToken.None);

        importer.Verify(x => x.Import(plugin.ModKey, firstFormKey, result), Times.Once);
        importer.Verify(x => x.Import(plugin.ModKey, secondFormKey, result), Times.Once);
    }

    private static PluginDTO CreatePluginDTO()
    {
        return new PluginDTO
        {
            ModKey = new ModKey("Example", ModType.Master)
        };
    }
}
