using Moq;
using Serilog;
using SFRecordCompareEngine.Core.Configuration.Interfaces;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Models.Records;
using SFRecordCompareEngine.Core.Services.Interfaces;
using SFRecordCompareEngine.ViewModels;
using Shouldly;

namespace SFRecordCompareEngine.UnitTests.ViewModels;

public class MainWindowViewModelTests
{
    [Fact]
    public void Constructor_SetsInitialEmptyState()
    {
        var sut = CreateSut();

        sut.LoadedGameText.ShouldBe("None");
        sut.LoadedPluginText.ShouldBe("None");
        sut.StatusText.ShouldBe("Use File > Open to choose a game and plugin.");
        sut.RecordTypeNodes.ShouldBeEmpty();
        sut.RecordsGridItems.ShouldBeNull();
        sut.IsComparisonMode.ShouldBeFalse();
        sut.CanUseApplication.ShouldBeTrue();
        sut.IsDatabaseImportRunning.ShouldBeFalse();
    }

    [Fact]
    public void LoadPlugin_LoadsRecordTypeNodesAndUpdatesStatus()
    {
        var pluginService = new Mock<IPluginService>();
        pluginService.Setup(service => service.GetRecordTypes()).Returns(["FormList", "Keyword"]);
        pluginService.Setup(service => service.GetRecords("Example.esm", "FormList")).Returns(
            [
                new RecordSummaryDTO
                {
                    RecordType = "FormList",
                    FormID = "Example.esm|800",
                    EditorID = "ExampleList"
                }
            ]);
        pluginService.Setup(service => service.GetRecords("Example.esm", "Keyword")).Returns([]);
        var sut = CreateSut(pluginService.Object);

        sut.LoadPlugin("Starfield", "Example.esm");

        sut.LoadedGameText.ShouldBe("Starfield");
        sut.LoadedPluginText.ShouldBe("Example.esm");
        sut.RecordTypeNodes.Count.ShouldBe(1);
        sut.RecordTypeNodes[0].Name.ShouldBe("FormList");
        sut.StatusText.ShouldBe("Loaded 1 record type.");
        sut.IsComparisonMode.ShouldBeFalse();
    }

    [Fact]
    public void SelectRecordTreeItem_WhenRecordTypeSelected_ShowsSummaries()
    {
        var records = new List<RecordSummaryDTO>
        {
            new()
            {
                RecordType = "FormList",
                FormID = "Example.esm|800",
                EditorID = "ExampleList"
            }
        };
        var sut = CreateSut();

        sut.SelectRecordTreeItem(new RecordTypeTreeNode
        {
            Name = "FormList",
            Records = records
        });

        sut.RecordsGridItems.ShouldBe(records);
        sut.StatusText.ShouldBe("Loaded 1 FormList records.");
        sut.IsComparisonMode.ShouldBeFalse();
    }

    [Fact]
    public void SelectRecordTreeItem_WhenRecordSelected_BuildsComparisonRows()
    {
        var pluginService = new Mock<IPluginService>();
        pluginService.Setup(service => service.GetRecordTypes()).Returns(["FormList"]);
        pluginService.Setup(service => service.GetRecords("Example.esm", "FormList")).Returns(
            [
                new RecordSummaryDTO
                {
                    RecordType = "FormList",
                    FormID = "Example.esm|800",
                    EditorID = "ExampleList"
                }
            ]);
        pluginService.Setup(service => service.GetRecordComparison("Example.esm", "FormList", "Example.esm|800"))
            .Returns(new RecordComparisonDTO
            {
                Plugins =
                [
                    new RecordComparisonPluginDTO
                    {
                        PluginName = "Starfield.esm"
                    },
                    new RecordComparisonPluginDTO
                    {
                        PluginName = "Example.esm"
                    }
                ],
                Fields =
                [
                    new RecordComparisonFieldDTO
                    {
                        FieldName = "EditorID",
                        DisplayKind = RecordComparisonFieldDisplayKind.Text,
                        ValuesByPlugin = new Dictionary<string, string?>
                        {
                            ["Starfield.esm"] = "BaseList",
                            ["Example.esm"] = "ExampleList"
                        }
                    }
                ]
            });
        var sut = CreateSut(pluginService.Object);
        sut.LoadPlugin("Starfield", "Example.esm");

        sut.SelectRecordTreeItem(new RecordSummaryDTO
        {
            RecordType = "FormList",
            FormID = "Example.esm|800",
            EditorID = "ExampleList"
        });

        sut.IsComparisonMode.ShouldBeTrue();
        sut.ComparisonPluginNames.ShouldBe(["Starfield.esm", "Example.esm"]);
        sut.RecordsGridItems.ShouldNotBeNull();
        var rows = sut.RecordsGridItems.ShouldBeAssignableTo<IList<RecordComparisonRowViewModel>>();
        rows.Count.ShouldBe(1);
        rows[0].FieldName.ShouldBe("EditorID");
        rows[0].Cells["Starfield.esm"].TextValue.ShouldBe("BaseList");
        rows[0].Cells["Example.esm"].TextValue.ShouldBe("ExampleList");
        sut.StatusText.ShouldBe("Loaded comparison for ExampleList.");
    }

    [Fact]
    public void SelectRecordTreeItem_WhenRecordCannotBeLoaded_ReportsStatus()
    {
        var sut = CreateSut();

        sut.SelectRecordTreeItem(new RecordSummaryDTO
        {
            RecordType = "FormList",
            FormID = null,
            EditorID = "ExampleList"
        });

        sut.StatusText.ShouldBe("Unable to load comparison for the selected record.");
        sut.IsComparisonMode.ShouldBeFalse();
    }

    [Fact]
    public async Task InitializeDatabaseImportAsync_WhenImportSucceeds_ReportsProgress()
    {
        var gameConfigurationStore = new Mock<IGameConfigurationStore>();
        gameConfigurationStore.SetupGet(store => store.SelectedGame).Returns("Starfield");
        var pluginImportService = new Mock<IPluginImportService>();
        pluginImportService.Setup(service => service.InitializeAndImportAsync(
                It.IsAny<IProgress<PluginImportProgressDTO>>(),
                It.IsAny<CancellationToken>()))
            .Callback<IProgress<PluginImportProgressDTO>, CancellationToken>((progress, _) =>
            {
                progress.Report(new PluginImportProgressDTO
                {
                    CurrentPluginName = "Example.esm",
                    CurrentModKey = "Example.esm",
                    PluginIndex = 1,
                    PluginCount = 2,
                    StatusText = "Checking Example.esm (1 of 2)..."
                });
            })
            .ReturnsAsync(new PluginImportResultDTO
            {
                PluginsImported = 2
            });
        var sut = CreateSut(pluginImportService: pluginImportService.Object, gameConfigurationStore: gameConfigurationStore.Object);

        await sut.InitializeDatabaseImportAsync(CancellationToken.None);

        gameConfigurationStore.Verify(store => store.SelectGame(It.IsAny<string?>()), Times.Never);
        sut.IsDatabaseImportRunning.ShouldBeFalse();
        sut.CanUseApplication.ShouldBeTrue();
        sut.StatusText.ShouldBe("Plugin database import completed. Imported 2 plugins.");
    }

    [Fact]
    public async Task InitializeDatabaseImportAsync_WhenImportFails_ClearsRunningStateAndThrows()
    {
        var pluginImportService = new Mock<IPluginImportService>();
        pluginImportService.Setup(service => service.InitializeAndImportAsync(
                It.IsAny<IProgress<PluginImportProgressDTO>>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Import failed."));
        var sut = CreateSut(pluginImportService: pluginImportService.Object);

        await Should.ThrowAsync<InvalidOperationException>(() => sut.InitializeDatabaseImportAsync(CancellationToken.None));

        sut.IsDatabaseImportRunning.ShouldBeFalse();
        sut.CanUseApplication.ShouldBeTrue();
        sut.StatusText.ShouldBe("Unable to initialize the plugin database.");
    }

    [Fact]
    public async Task InitializeDatabaseImportAsync_WhenGameIsNotConfigured_Throws()
    {
        var gameConfigurationStore = new Mock<IGameConfigurationStore>();
        var pluginImportService = new Mock<IPluginImportService>();
        var sut = CreateSut(pluginImportService: pluginImportService.Object, gameConfigurationStore: gameConfigurationStore.Object);

        await Should.ThrowAsync<InvalidOperationException>(() => sut.InitializeDatabaseImportAsync(CancellationToken.None));

        pluginImportService.Verify(service => service.InitializeAndImportAsync(
            It.IsAny<IProgress<PluginImportProgressDTO>>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    private static MainWindowViewModel CreateSut(
        IPluginService? pluginService = null,
        IPluginImportService? pluginImportService = null,
        IGameConfigurationStore? gameConfigurationStore = null)
    {
        var defaultGameConfigurationStore = new Mock<IGameConfigurationStore>();
        defaultGameConfigurationStore.SetupGet(store => store.SelectedGame).Returns("Starfield");

        return new MainWindowViewModel(
            pluginService ?? Mock.Of<IPluginService>(),
            pluginImportService ?? Mock.Of<IPluginImportService>(),
            gameConfigurationStore ?? defaultGameConfigurationStore.Object,
            Mock.Of<ILogger>());
    }
}
