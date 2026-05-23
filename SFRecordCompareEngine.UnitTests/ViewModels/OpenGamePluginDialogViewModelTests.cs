using Moq;
using Mutagen.Bethesda.Environments;
using Serilog;
using SFRecordCompareEngine.Core.Configuration.Interfaces;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.Services.Interfaces;
using SFRecordCompareEngine.ViewModels;
using Shouldly;

namespace SFRecordCompareEngine.UnitTests.ViewModels;

public class OpenGamePluginDialogViewModelTests
{
    [Fact]
    public void Constructor_WhenPluginsExist_LoadsPluginsAndSelectsFirst()
    {
        var gameConfigurationStore = CreateGameConfigurationStore();
        var pluginService = new Mock<IPluginService>();
        pluginService.Setup(service => service.GetPlugins()).Returns(["First.esm", "Second.esm"]);
        pluginService.Setup(service => service.SearchPlugins("First.esm")).Returns(["First.esm"]);

        var sut = new OpenGamePluginDialogViewModel(gameConfigurationStore.Object, pluginService.Object, Mock.Of<ILogger>());

        sut.SelectedGame.ShouldBe("Starfield");
        sut.PluginSearchText.ShouldBe("First.esm");
        sut.PluginItems.ShouldBe(["First.esm", "Second.esm"]);
        sut.StatusText.ShouldBe("Loaded 2 plugins.");
        gameConfigurationStore.Verify(store => store.SelectGame("Starfield"), Times.Once);
    }

    [Fact]
    public void PluginSearchText_WhenGameIsConfigured_FiltersPluginsAndClearsHeader()
    {
        var gameConfigurationStore = CreateGameConfigurationStore();
        var pluginService = new Mock<IPluginService>();
        pluginService.Setup(service => service.GetPlugins()).Returns(["First.esm", "Second.esm"]);
        var sut = new OpenGamePluginDialogViewModel(gameConfigurationStore.Object, pluginService.Object, Mock.Of<ILogger>());

        sut.PluginSearchText = "Sec";

        sut.PluginItems.ShouldBe(["Second.esm"]);
        sut.CanOpen.ShouldBeFalse();
        pluginService.Verify(service => service.SearchPlugins(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void PluginSearchText_WhenSearchMatchesMultiplePlugins_KeepsAllMatches()
    {
        var gameConfigurationStore = CreateGameConfigurationStore();
        var pluginService = new Mock<IPluginService>();
        pluginService.Setup(service => service.GetPlugins()).Returns([
            "Venworks-MyExperiments.esm",
            "Venworks-encountersoverhaul.esm",
            "Other.esm"
        ]);
        var sut = new OpenGamePluginDialogViewModel(gameConfigurationStore.Object, pluginService.Object, Mock.Of<ILogger>());

        sut.PluginSearchText = "venworks";

        sut.PluginItems.ShouldBe([
            "Venworks-MyExperiments.esm",
            "Venworks-encountersoverhaul.esm"
        ]);
    }

    [Fact]
    public void SelectPluginSearchResult_WhenFilteredPluginIsSelected_KeepsSelectedPluginTextAndFilteredList()
    {
        var gameConfigurationStore = CreateGameConfigurationStore();
        var pluginService = new Mock<IPluginService>();
        pluginService.Setup(service => service.GetPlugins()).Returns([
            "Venworks-MyExperiments.esm",
            "Venworks-encountersoverhaul.esm",
            "Other.esm"
        ]);
        var sut = new OpenGamePluginDialogViewModel(gameConfigurationStore.Object, pluginService.Object, Mock.Of<ILogger>());
        sut.PluginSearchText = "venworks";

        sut.SelectPluginSearchResult("Venworks-MyExperiments.esm");

        sut.PluginSearchText.ShouldBe("Venworks-MyExperiments.esm");
        sut.PluginItems.ShouldBe([
            "Venworks-MyExperiments.esm",
            "Venworks-encountersoverhaul.esm"
        ]);
        sut.CanOpen.ShouldBeFalse();
    }

    [Fact]
    public void LoadPluginHeaderCommand_WhenHeaderLoads_UpdatesHeaderStateAndAllowsOpen()
    {
        var gameConfigurationStore = CreateGameConfigurationStore();
        var pluginService = new Mock<IPluginService>();
        pluginService.Setup(service => service.GetPlugins()).Returns(["Example.esm"]);
        pluginService.Setup(service => service.SearchPlugins("Example.esm")).Returns(["Example.esm"]);
        pluginService.Setup(service => service.GetPluginHeader("Example.esm")).Returns(CreatePluginHeader());
        var sut = new OpenGamePluginDialogViewModel(gameConfigurationStore.Object, pluginService.Object, Mock.Of<ILogger>());

        sut.LoadPluginHeaderCommand.Execute(null);

        sut.SelectedPluginName.ShouldBe("Example.esm");
        sut.PluginName.ShouldBe("Example.esm");
        sut.PluginAuthor.ShouldBe("Author");
        sut.PluginVersion.ShouldBe("44");
        sut.PluginDescription.ShouldBeEmpty();
        sut.PluginMasters.ShouldBe("Starfield.esm");
        sut.CanOpen.ShouldBeTrue();
        sut.StatusText.ShouldBe("Loaded plugin header for Example.esm.");
    }

    [Fact]
    public void TryConfirmOpen_WhenHeaderIsMissing_ReturnsFalseAndReportsStatus()
    {
        var gameConfigurationStore = CreateGameConfigurationStore();
        var pluginService = new Mock<IPluginService>();
        pluginService.Setup(service => service.GetPlugins()).Returns(["Example.esm"]);
        pluginService.Setup(service => service.SearchPlugins("Example.esm")).Returns(["Example.esm"]);
        pluginService.Setup(service => service.GetPluginHeader("Example.esm")).Returns(null as PluginHeaderDTO);
        var sut = new OpenGamePluginDialogViewModel(gameConfigurationStore.Object, pluginService.Object, Mock.Of<ILogger>());

        var result = sut.TryConfirmOpen();

        result.ShouldBeFalse();
        sut.CanOpen.ShouldBeFalse();
        sut.StatusText.ShouldBe("Unable to load plugin header for Example.esm.");
    }

    private static Mock<IGameConfigurationStore> CreateGameConfigurationStore()
    {
        var gameConfigurationStore = new Mock<IGameConfigurationStore>();
        gameConfigurationStore.SetupProperty(store => store.SelectedGame);
        gameConfigurationStore.SetupGet(store => store.SupportedGames).Returns(["Starfield"]);
        gameConfigurationStore.SetupGet(store => store.Game).Returns(Mock.Of<IGameEnvironment>());
        gameConfigurationStore.Setup(store => store.SelectGame(It.IsAny<string?>()))
            .Callback<string?>(game => gameConfigurationStore.Object.SelectedGame = game);
        return gameConfigurationStore;
    }

    private static PluginHeaderDTO CreatePluginHeader()
    {
        return new PluginHeaderDTO(
            new PluginMetadataDTO
            {
                ModKey = "Example",
                GameRelease = "Starfield",
                PluginFileName = "Example.esm",
                FormVersion = 44,
                Author = "Author",
                LastCheckedUtc = DateTime.UtcNow.ToString("O")
            },
            [
                new PluginMasterReferenceDTO
                {
                    ModKey = "Example",
                    ParentModKey = "Starfield.esm",
                    MasterReferenceIndex = 0,
                    ImportedAtUtc = DateTime.UtcNow.ToString("O")
                }
            ]);
    }
}
