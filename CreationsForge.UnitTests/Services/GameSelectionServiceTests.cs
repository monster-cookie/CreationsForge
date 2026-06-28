using CreationsForge.Core.Configuration.Interfaces;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Models.Configuration;
using CreationsForge.Core.Services;
using Shouldly;

namespace CreationsForge.UnitTests.Services;

/// <summary>
/// Tests supported-game and active-game behavior for <see cref="GameSelectionService"/>.
/// </summary>
public class GameSelectionServiceTests
{
    /// <summary>
    /// Verifies the supported game list exposes the approved CreationsForge games.
    /// </summary>
    [Fact]
    public void GetSupportedGames_ReturnsApprovedGames()
    {
        var store = new TestApplicationConfigurationStore();
        var service = new GameSelectionService(store);

        var games = service.GetSupportedGames();

        games.Select(game => game.Game).ShouldBe([SupportedGame.Starfield, SupportedGame.Fallout4, SupportedGame.Skyrim]);
        games.Single(game => game.Game == SupportedGame.Fallout4).DisplayName.ShouldBe("Fallout 4");
    }

    /// <summary>
    /// Verifies a configured supported game can be read back from configuration.
    /// </summary>
    [Fact]
    public void GetActiveGame_WithConfiguredGame_ReturnsGame()
    {
        var store = new TestApplicationConfigurationStore
        {
            Current = new ApplicationConfiguration { ActiveGame = "Skyrim" }
        };
        var service = new GameSelectionService(store);

        var game = service.GetActiveGame();

        game.ShouldBe(SupportedGame.Skyrim);
    }

    /// <summary>
    /// Verifies invalid active-game configuration values are ignored.
    /// </summary>
    [Fact]
    public void GetActiveGame_WithInvalidConfiguredGame_ReturnsNull()
    {
        var store = new TestApplicationConfigurationStore
        {
            Current = new ApplicationConfiguration { ActiveGame = "Oblivion" }
        };
        var service = new GameSelectionService(store);

        var game = service.GetActiveGame();

        game.ShouldBeNull();
    }

    /// <summary>
    /// Verifies active-game saves preserve unrelated application settings.
    /// </summary>
    [Fact]
    public void SetActiveGame_PreservesExistingSettingsAndPaths()
    {
        var store = new TestApplicationConfigurationStore
        {
            Current = new ApplicationConfiguration
            {
                ThemeFamily = ApplicationThemeFamily.Fluent,
                ThemeMode = ApplicationThemeMode.Light,
                RecordTextLanguage = "German",
                NifSkopeExecutablePath = "nifskope.exe",
                PreferEspOverMatchingEsm = false,
                ApplicationDataDirectory = "app-data",
                DatabaseDirectory = "database",
                LoggingDirectory = "logs"
            }
        };
        var service = new GameSelectionService(store);

        service.SetActiveGame(SupportedGame.Fallout4);

        store.Current.ActiveGame.ShouldBe("Fallout4");
        store.Current.ThemeFamily.ShouldBe(ApplicationThemeFamily.Fluent);
        store.Current.ThemeMode.ShouldBe(ApplicationThemeMode.Light);
        store.Current.RecordTextLanguage.ShouldBe("German");
        store.Current.NifSkopeExecutablePath.ShouldBe("nifskope.exe");
        store.Current.PreferEspOverMatchingEsm.ShouldBeFalse();
        store.Current.ApplicationDataDirectory.ShouldBe("app-data");
        store.Current.DatabaseDirectory.ShouldBe("database");
        store.Current.LoggingDirectory.ShouldBe("logs");
    }

    /// <summary>
    /// Provides in-memory configuration state for game-selection service tests.
    /// </summary>
    private sealed class TestApplicationConfigurationStore : IApplicationConfigurationStore
    {
        /// <inheritdoc />
        public string ConfigurationPath => "test.json";

        /// <inheritdoc />
        public ApplicationConfiguration Current { get; set; } = new();

        /// <inheritdoc />
        public void Load()
        { }

        /// <inheritdoc />
        public void Save(ApplicationConfiguration configuration)
        {
            Current = configuration;
        }
    }
}
