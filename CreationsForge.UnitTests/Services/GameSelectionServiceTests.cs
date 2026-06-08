using CreationsForge.Core.Configuration.Interfaces;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Models.Configuration;
using CreationsForge.Core.Services;
using Shouldly;

namespace CreationsForge.UnitTests.Services;

public class GameSelectionServiceTests
{
    [Fact]
    public void GetSupportedGames_ReturnsApprovedGames()
    {
        var store = new TestApplicationConfigurationStore();
        var service = new GameSelectionService(store);

        var games = service.GetSupportedGames();

        games.Select(game => game.Game).ShouldBe([SupportedGame.Starfield, SupportedGame.Fallout4, SupportedGame.Skyrim]);
        games.Single(game => game.Game == SupportedGame.Fallout4).DisplayName.ShouldBe("Fallout 4");
    }

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

    [Fact]
    public void SetActiveGame_PreservesExistingPaths()
    {
        var store = new TestApplicationConfigurationStore
        {
            Current = new ApplicationConfiguration
            {
                ThemeFamily = ApplicationThemeFamily.Fluent,
                ThemeMode = ApplicationThemeMode.Light,
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
        store.Current.ApplicationDataDirectory.ShouldBe("app-data");
        store.Current.DatabaseDirectory.ShouldBe("database");
        store.Current.LoggingDirectory.ShouldBe("logs");
    }

    [Fact]
    public void GetThemeFamily_ReturnsConfiguredThemeFamily()
    {
        var store = new TestApplicationConfigurationStore
        {
            Current = new ApplicationConfiguration { ThemeFamily = ApplicationThemeFamily.Fluent }
        };
        var service = new GameSelectionService(store);

        var themeFamily = service.GetThemeFamily();

        themeFamily.ShouldBe(ApplicationThemeFamily.Fluent);
    }

    [Fact]
    public void GetThemeMode_ReturnsConfiguredThemeMode()
    {
        var store = new TestApplicationConfigurationStore
        {
            Current = new ApplicationConfiguration { ThemeMode = ApplicationThemeMode.Light }
        };
        var service = new GameSelectionService(store);

        var themeMode = service.GetThemeMode();

        themeMode.ShouldBe(ApplicationThemeMode.Light);
    }

    [Fact]
    public void SetThemeMode_PreservesActiveGameAndExistingPaths()
    {
        var store = new TestApplicationConfigurationStore
        {
            Current = new ApplicationConfiguration
            {
                ActiveGame = "Skyrim",
                ThemeFamily = ApplicationThemeFamily.Fluent,
                ApplicationDataDirectory = "app-data",
                DatabaseDirectory = "database",
                LoggingDirectory = "logs"
            }
        };
        var service = new GameSelectionService(store);

        service.SetThemeMode(ApplicationThemeMode.Light);

        store.Current.ActiveGame.ShouldBe("Skyrim");
        store.Current.ThemeFamily.ShouldBe(ApplicationThemeFamily.Fluent);
        store.Current.ThemeMode.ShouldBe(ApplicationThemeMode.Light);
        store.Current.ApplicationDataDirectory.ShouldBe("app-data");
        store.Current.DatabaseDirectory.ShouldBe("database");
        store.Current.LoggingDirectory.ShouldBe("logs");
    }

    [Fact]
    public void SetThemeFamily_PreservesActiveGameThemeModeAndExistingPaths()
    {
        var store = new TestApplicationConfigurationStore
        {
            Current = new ApplicationConfiguration
            {
                ActiveGame = "Skyrim",
                ThemeMode = ApplicationThemeMode.Light,
                ApplicationDataDirectory = "app-data",
                DatabaseDirectory = "database",
                LoggingDirectory = "logs"
            }
        };
        var service = new GameSelectionService(store);

        service.SetThemeFamily(ApplicationThemeFamily.Fluent);

        store.Current.ActiveGame.ShouldBe("Skyrim");
        store.Current.ThemeFamily.ShouldBe(ApplicationThemeFamily.Fluent);
        store.Current.ThemeMode.ShouldBe(ApplicationThemeMode.Light);
        store.Current.ApplicationDataDirectory.ShouldBe("app-data");
        store.Current.DatabaseDirectory.ShouldBe("database");
        store.Current.LoggingDirectory.ShouldBe("logs");
    }

    [Fact]
    public void SetActiveGameAndThemeMode_PreservesExistingPaths()
    {
        var store = new TestApplicationConfigurationStore
        {
            Current = new ApplicationConfiguration
            {
                ThemeFamily = ApplicationThemeFamily.Fluent,
                ApplicationDataDirectory = "app-data",
                DatabaseDirectory = "database",
                LoggingDirectory = "logs"
            }
        };
        var service = new GameSelectionService(store);

        service.SetActiveGameAndThemeMode(SupportedGame.Starfield, ApplicationThemeMode.Light);

        store.Current.ActiveGame.ShouldBe("Starfield");
        store.Current.ThemeFamily.ShouldBe(ApplicationThemeFamily.Fluent);
        store.Current.ThemeMode.ShouldBe(ApplicationThemeMode.Light);
        store.Current.ApplicationDataDirectory.ShouldBe("app-data");
        store.Current.DatabaseDirectory.ShouldBe("database");
        store.Current.LoggingDirectory.ShouldBe("logs");
    }

    [Fact]
    public void SetActiveGameAndTheme_PreservesExistingPaths()
    {
        var store = new TestApplicationConfigurationStore
        {
            Current = new ApplicationConfiguration
            {
                ApplicationDataDirectory = "app-data",
                DatabaseDirectory = "database",
                LoggingDirectory = "logs"
            }
        };
        var service = new GameSelectionService(store);

        service.SetActiveGameAndTheme(SupportedGame.Starfield, ApplicationThemeFamily.Fluent, ApplicationThemeMode.Light);

        store.Current.ActiveGame.ShouldBe("Starfield");
        store.Current.ThemeFamily.ShouldBe(ApplicationThemeFamily.Fluent);
        store.Current.ThemeMode.ShouldBe(ApplicationThemeMode.Light);
        store.Current.ApplicationDataDirectory.ShouldBe("app-data");
        store.Current.DatabaseDirectory.ShouldBe("database");
        store.Current.LoggingDirectory.ShouldBe("logs");
    }

    [Fact]
    public void SetTheme_PreservesActiveGameAndExistingPaths()
    {
        var store = new TestApplicationConfigurationStore
        {
            Current = new ApplicationConfiguration
            {
                ActiveGame = "Fallout4",
                ApplicationDataDirectory = "app-data",
                DatabaseDirectory = "database",
                LoggingDirectory = "logs"
            }
        };
        var service = new GameSelectionService(store);

        service.SetTheme(ApplicationThemeFamily.Fluent, ApplicationThemeMode.Light);

        store.Current.ActiveGame.ShouldBe("Fallout4");
        store.Current.ThemeFamily.ShouldBe(ApplicationThemeFamily.Fluent);
        store.Current.ThemeMode.ShouldBe(ApplicationThemeMode.Light);
        store.Current.ApplicationDataDirectory.ShouldBe("app-data");
        store.Current.DatabaseDirectory.ShouldBe("database");
        store.Current.LoggingDirectory.ShouldBe("logs");
    }

    private sealed class TestApplicationConfigurationStore : IApplicationConfigurationStore
    {
        public string ConfigurationPath => "test.json";

        public ApplicationConfiguration Current { get; set; } = new();

        public void Load()
        { }

        public void Save(ApplicationConfiguration configuration)
        {
            Current = configuration;
        }
    }
}
