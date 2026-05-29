using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Moq;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
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
        reader.Setup(x => x.GetFormLists(plugin)).Returns(new List<FormListDTO>());
        reader.Setup(x => x.GetGameSettings(plugin)).Returns(new List<GameSettingDTO>());
        var sut = new RecordImportService(Array.Empty<ITypedRecordDetailImporter>(), reader.Object);

        var result = sut.ImportPluginRecords(plugin, null, 1, 1, CancellationToken.None);

        result.ModKey.ShouldBe(plugin.ModKey);
        result.RecordTypes.Count.ShouldBe(2);
        result.HeadersImported.ShouldBe(0);
    }

    [Fact]
    public void ImportPluginRecords_WhenFormListImporterIsMissing_DoesNotImportFormLists()
    {
        var plugin = CreatePluginDTO();
        var reader = new Mock<IStarfieldRecordReaderService>();
        reader.Setup(x => x.GetFormLists(plugin)).Returns(new List<FormListDTO> { CreateFormListDTO(plugin.ModKey, 123) });
        reader.Setup(x => x.GetGameSettings(plugin)).Returns(new List<GameSettingDTO>());
        var sut = new RecordImportService(Array.Empty<ITypedRecordDetailImporter>(), reader.Object);

        var result = sut.ImportPluginRecords(plugin, null, 1, 1, CancellationToken.None);

        result.ModKey.ShouldBe(plugin.ModKey);
        result.RecordTypes.Count.ShouldBe(2);
        result.UnsupportedRecordTypes.ShouldBe(2);
    }

    [Fact]
    public void ImportPluginRecords_WhenFormListImporterExists_ImportsEachFormList()
    {
        var plugin = CreatePluginDTO();
        var firstFormList = CreateFormListDTO(plugin.ModKey, 123);
        var secondFormList = CreateFormListDTO(plugin.ModKey, 456);
        var reader = new Mock<IStarfieldRecordReaderService>();
        reader.Setup(x => x.GetFormLists(plugin)).Returns(new List<FormListDTO> { firstFormList, secondFormList });
        reader.Setup(x => x.GetGameSettings(plugin)).Returns(new List<GameSettingDTO>());
        var importer = new Mock<ITypedRecordDetailImporter>();
        importer.SetupGet(x => x.GameRelease).Returns(GameRelease.Starfield);
        importer.SetupGet(x => x.RecordType).Returns(new RecordType("FLST"));
        var sut = new RecordImportService(new[] { importer.Object }, reader.Object);

        var result = sut.ImportPluginRecords(plugin, null, 1, 1, CancellationToken.None);

        var formListResult = result.RecordTypes.Single(x => x.RecordType == "FLST");
        importer.Verify(x => x.Import(firstFormList, formListResult), Times.Once);
        importer.Verify(x => x.Import(secondFormList, formListResult), Times.Once);
        formListResult.HeadersImported.ShouldBe(2);
    }

    [Fact]
    public void ImportPluginRecords_WhenGameSettingImporterExists_ImportsEachGameSetting()
    {
        var plugin = CreatePluginDTO();
        var firstGameSetting = CreateGameSettingDTO(plugin.ModKey, 123);
        var secondGameSetting = CreateGameSettingDTO(plugin.ModKey, 456);
        var reader = new Mock<IStarfieldRecordReaderService>();
        reader.Setup(x => x.GetFormLists(plugin)).Returns(new List<FormListDTO>());
        reader.Setup(x => x.GetGameSettings(plugin)).Returns(new List<GameSettingDTO> { firstGameSetting, secondGameSetting });
        var importer = new Mock<ITypedRecordDetailImporter>();
        importer.SetupGet(x => x.GameRelease).Returns(GameRelease.Starfield);
        importer.SetupGet(x => x.RecordType).Returns(new RecordType("GMST"));
        var sut = new RecordImportService(new[] { importer.Object }, reader.Object);

        var result = sut.ImportPluginRecords(plugin, null, 1, 1, CancellationToken.None);

        var gameSettingResult = result.RecordTypes.Single(x => x.RecordType == "GMST");
        importer.Verify(x => x.Import(firstGameSetting, gameSettingResult), Times.Once);
        importer.Verify(x => x.Import(secondGameSetting, gameSettingResult), Times.Once);
        gameSettingResult.HeadersImported.ShouldBe(2);
    }

    [Fact]
    public void ImportPluginRecords_ReportsRecordTypeProgress()
    {
        var plugin = CreatePluginDTO();
        var reader = new Mock<IStarfieldRecordReaderService>();
        reader.Setup(x => x.GetFormLists(plugin)).Returns(new List<FormListDTO> { CreateFormListDTO(plugin.ModKey, 123) });
        reader.Setup(x => x.GetGameSettings(plugin)).Returns(new List<GameSettingDTO>());
        var progress = new CapturingProgress();
        var sut = new RecordImportService(Array.Empty<ITypedRecordDetailImporter>(), reader.Object);

        sut.ImportPluginRecords(plugin, progress, 2, 5, CancellationToken.None);

        progress.Reports.ShouldContain(x => x.CurrentRecordType == "FLST" && x.StatusText.Contains("Discovering FLST", StringComparison.Ordinal));
        progress.Reports.ShouldContain(x => x.CurrentRecordType == "FLST" && x.RecordCount == 1);
        progress.Reports.ShouldAllBe(x => x.PluginIndex == 2 && x.PluginCount == 5);
    }

    private static PluginDTO CreatePluginDTO()
    {
        return new PluginDTO
        {
            ModKey = new ModKey("Example", ModType.Master)
        };
    }

    private static FormListDTO CreateFormListDTO(ModKey modKey, uint formId)
    {
        return new FormListDTO
        {
            ModKey = modKey,
            FormKey = new FormKey(modKey, formId),
            EditorID = "Editor",
            FormVersion = 44,
            StarfieldMajorRecordFlags = 0,
            Version2 = 0,
            VersionControl = 0,
            ImportedAtUTC = DateTime.UtcNow
        };
    }

    private static GameSettingDTO CreateGameSettingDTO(ModKey modKey, uint formId)
    {
        return new GameSettingDTO
        {
            ModKey = modKey,
            FormKey = new FormKey(modKey, formId),
            EditorID = "Editor",
            FormVersion = 44,
            StarfieldMajorRecordFlags = 0,
            Version2 = 0,
            VersionControl = 0,
            ImportedAtUTC = DateTime.UtcNow
        };
    }

    private sealed class CapturingProgress : IProgress<PluginImportProgressDTO>
    {
        public List<PluginImportProgressDTO> Reports { get; } = new();

        public void Report(PluginImportProgressDTO value)
        {
            Reports.Add(value);
        }
    }
}
