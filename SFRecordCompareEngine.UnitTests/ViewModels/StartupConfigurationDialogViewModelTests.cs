using Moq;
using Mutagen.Bethesda.Environments;
using SFRecordCompareEngine.Core.Configuration.Interfaces;
using SFRecordCompareEngine.Core.Models.Configuration;
using SFRecordCompareEngine.ViewModels;
using Shouldly;

namespace SFRecordCompareEngine.UnitTests.ViewModels;

public class StartupConfigurationDialogViewModelTests
{
    [Fact]
    public void Constructor_WhenConfigurationIsMissing_UsesDefaults()
    {
        var applicationConfigurationStore = CreateApplicationConfigurationStore(new ApplicationConfiguration());
        var gameConfigurationStore = CreateGameConfigurationStore();

        var sut = new StartupConfigurationDialogViewModel(applicationConfigurationStore.Object, gameConfigurationStore.Object);

        sut.SelectedGame.ShouldBe("Starfield");
    }

    [Fact]
    public void TrySave_WhenConfigurationIsValid_SavesAndSelectsGame()
    {
        var applicationConfigurationStore = CreateApplicationConfigurationStore(new ApplicationConfiguration());
        var gameConfigurationStore = CreateGameConfigurationStore();
        var sut = new StartupConfigurationDialogViewModel(applicationConfigurationStore.Object, gameConfigurationStore.Object)
        {
            SelectedGame = "Starfield"
        };

        var result = sut.TrySave();

        result.ShouldBeTrue();
        gameConfigurationStore.Verify(store => store.SelectGame("Starfield"), Times.Once);
        applicationConfigurationStore.Verify(store => store.Save(It.Is<ApplicationConfiguration>(configuration =>
            configuration.SelectedGame == "Starfield")), Times.Once);
    }

    [Fact]
    public void TrySave_WhenGameIsMissing_DoesNotSave()
    {
        var applicationConfigurationStore = CreateApplicationConfigurationStore(new ApplicationConfiguration());
        var gameConfigurationStore = CreateGameConfigurationStore();
        var sut = new StartupConfigurationDialogViewModel(applicationConfigurationStore.Object, gameConfigurationStore.Object)
        {
            SelectedGame = null
        };

        var result = sut.TrySave();

        result.ShouldBeFalse();
        applicationConfigurationStore.Verify(store => store.Save(It.IsAny<ApplicationConfiguration>()), Times.Never);
        sut.StatusText.ShouldBe("Select a supported game.");
    }

    private static Mock<IApplicationConfigurationStore> CreateApplicationConfigurationStore(ApplicationConfiguration configuration)
    {
        var applicationConfigurationStore = new Mock<IApplicationConfigurationStore>();
        applicationConfigurationStore.SetupGet(store => store.Current).Returns(configuration);
        return applicationConfigurationStore;
    }

    private static Mock<IGameConfigurationStore> CreateGameConfigurationStore()
    {
        var gameConfigurationStore = new Mock<IGameConfigurationStore>();
        gameConfigurationStore.SetupProperty(store => store.SelectedGame);
        gameConfigurationStore.Object.SelectedGame = "Starfield";
        gameConfigurationStore.SetupGet(store => store.SupportedGames).Returns(["None", "Starfield"]);
        gameConfigurationStore.SetupGet(store => store.Game).Returns(Mock.Of<IGameEnvironment>());
        gameConfigurationStore.Setup(store => store.SelectGame(It.IsAny<string?>()))
            .Callback<string?>(game => gameConfigurationStore.Object.SelectedGame = game);
        return gameConfigurationStore;
    }
}
