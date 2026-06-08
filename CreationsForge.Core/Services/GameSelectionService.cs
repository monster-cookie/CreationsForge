using CreationsForge.Core.Configuration.Interfaces;
using CreationsForge.Core.DTOs.Games;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Models.Configuration;
using CreationsForge.Core.Services.Interfaces;

namespace CreationsForge.Core.Services;

public class GameSelectionService : IGameSelectionService
{
    private readonly IApplicationConfigurationStore ConfigurationStore;

    public GameSelectionService(IApplicationConfigurationStore configurationStore)
    {
        ConfigurationStore = configurationStore;
    }

    public IReadOnlyList<SupportedGameDTO> GetSupportedGames()
    {
        return
        [
            CreateGameOption(SupportedGame.Starfield, "Starfield"),
            CreateGameOption(SupportedGame.Fallout4, "Fallout 4"),
            CreateGameOption(SupportedGame.Skyrim, "Skyrim Special Edition")
        ];
    }

    public SupportedGame? GetActiveGame()
    {
        return Enum.TryParse<SupportedGame>(ConfigurationStore.Current.ActiveGame, true, out var game)
            ? game
            : null;
    }

    public ApplicationThemeMode GetThemeMode()
    {
        return ConfigurationStore.Current.ThemeMode;
    }

    public ApplicationThemeFamily GetThemeFamily()
    {
        return ConfigurationStore.Current.ThemeFamily;
    }

    public void SetActiveGame(SupportedGame game)
    {
        SaveConfiguration(game.ToString(), ConfigurationStore.Current.ThemeFamily, ConfigurationStore.Current.ThemeMode);
    }

    public void SetThemeMode(ApplicationThemeMode themeMode)
    {
        SaveConfiguration(ConfigurationStore.Current.ActiveGame, ConfigurationStore.Current.ThemeFamily, themeMode);
    }

    public void SetThemeFamily(ApplicationThemeFamily themeFamily)
    {
        SaveConfiguration(ConfigurationStore.Current.ActiveGame, themeFamily, ConfigurationStore.Current.ThemeMode);
    }

    public void SetActiveGameAndThemeMode(SupportedGame game, ApplicationThemeMode themeMode)
    {
        SaveConfiguration(game.ToString(), ConfigurationStore.Current.ThemeFamily, themeMode);
    }

    public void SetActiveGameAndTheme(SupportedGame game, ApplicationThemeFamily themeFamily, ApplicationThemeMode themeMode)
    {
        SaveConfiguration(game.ToString(), themeFamily, themeMode);
    }

    public void SetTheme(ApplicationThemeFamily themeFamily, ApplicationThemeMode themeMode)
    {
        SaveConfiguration(ConfigurationStore.Current.ActiveGame, themeFamily, themeMode);
    }

    private static SupportedGameDTO CreateGameOption(SupportedGame game, string displayName)
    {
        return new SupportedGameDTO
        {
            Game = game,
            Name = game.ToString(),
            DisplayName = displayName
        };
    }

    private void SaveConfiguration(string? activeGame, ApplicationThemeFamily themeFamily, ApplicationThemeMode themeMode)
    {
        var configuration = new ApplicationConfiguration
        {
            ActiveGame = activeGame,
            ThemeFamily = themeFamily,
            ThemeMode = themeMode,
            ApplicationDataDirectory = ConfigurationStore.Current.ApplicationDataDirectory,
            DatabaseDirectory = ConfigurationStore.Current.DatabaseDirectory,
            LoggingDirectory = ConfigurationStore.Current.LoggingDirectory
        };

        ConfigurationStore.Save(configuration);
    }
}
