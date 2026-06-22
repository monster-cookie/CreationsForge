using CreationsForge.Core.Configuration.Interfaces;
using CreationsForge.Core.DTOs.Games;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Models.Configuration;
using CreationsForge.Core.Services.Interfaces;
using Mutagen.Bethesda.Strings;

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

    public IReadOnlyList<Language> GetRecordTextLanguages()
    {
        return Enum.GetValues<Language>().OrderBy(language => language.ToString(), StringComparer.OrdinalIgnoreCase).ToList();
    }

    public Language GetRecordTextLanguage()
    {
        return NormalizeRecordTextLanguage(ConfigurationStore.Current.RecordTextLanguage);
    }

    public string? GetNifSkopeExecutablePath()
    {
        return ConfigurationStore.Current.NifSkopeExecutablePath;
    }

    public void SetActiveGame(SupportedGame game)
    {
        SaveConfiguration(game.ToString(), ConfigurationStore.Current.ThemeFamily, ConfigurationStore.Current.ThemeMode, GetRecordTextLanguage().ToString(), ConfigurationStore.Current.NifSkopeExecutablePath);
    }

    public void SetThemeMode(ApplicationThemeMode themeMode)
    {
        SaveConfiguration(ConfigurationStore.Current.ActiveGame, ConfigurationStore.Current.ThemeFamily, themeMode, GetRecordTextLanguage().ToString(), ConfigurationStore.Current.NifSkopeExecutablePath);
    }

    public void SetThemeFamily(ApplicationThemeFamily themeFamily)
    {
        SaveConfiguration(ConfigurationStore.Current.ActiveGame, themeFamily, ConfigurationStore.Current.ThemeMode, GetRecordTextLanguage().ToString(), ConfigurationStore.Current.NifSkopeExecutablePath);
    }

    public void SetActiveGameAndThemeMode(SupportedGame game, ApplicationThemeMode themeMode)
    {
        SaveConfiguration(game.ToString(), ConfigurationStore.Current.ThemeFamily, themeMode, GetRecordTextLanguage().ToString(), ConfigurationStore.Current.NifSkopeExecutablePath);
    }

    public void SetActiveGameAndTheme(SupportedGame game, ApplicationThemeFamily themeFamily, ApplicationThemeMode themeMode)
    {
        SaveConfiguration(game.ToString(), themeFamily, themeMode, GetRecordTextLanguage().ToString(), ConfigurationStore.Current.NifSkopeExecutablePath);
    }

    public void SetTheme(ApplicationThemeFamily themeFamily, ApplicationThemeMode themeMode)
    {
        SaveConfiguration(ConfigurationStore.Current.ActiveGame, themeFamily, themeMode, GetRecordTextLanguage().ToString(), ConfigurationStore.Current.NifSkopeExecutablePath);
    }

    public void SetActiveGameThemeAndNifSkopeExecutablePath(SupportedGame game, ApplicationThemeFamily themeFamily, ApplicationThemeMode themeMode, string? nifSkopeExecutablePath)
    {
        SaveConfiguration(game.ToString(), themeFamily, themeMode, GetRecordTextLanguage().ToString(), NormalizeOptionalPath(nifSkopeExecutablePath));
    }

    public void SetThemeAndNifSkopeExecutablePath(ApplicationThemeFamily themeFamily, ApplicationThemeMode themeMode, string? nifSkopeExecutablePath)
    {
        SaveConfiguration(ConfigurationStore.Current.ActiveGame, themeFamily, themeMode, GetRecordTextLanguage().ToString(), NormalizeOptionalPath(nifSkopeExecutablePath));
    }

    public void SetThemeRecordTextLanguageAndNifSkopeExecutablePath(ApplicationThemeFamily themeFamily, ApplicationThemeMode themeMode, Language recordTextLanguage, string? nifSkopeExecutablePath)
    {
        SaveConfiguration(ConfigurationStore.Current.ActiveGame, themeFamily, themeMode, recordTextLanguage.ToString(), NormalizeOptionalPath(nifSkopeExecutablePath));
    }

    public void SetActiveGameThemeRecordTextLanguageAndNifSkopeExecutablePath(SupportedGame game, ApplicationThemeFamily themeFamily, ApplicationThemeMode themeMode, Language recordTextLanguage, string? nifSkopeExecutablePath)
    {
        SaveConfiguration(game.ToString(), themeFamily, themeMode, recordTextLanguage.ToString(), NormalizeOptionalPath(nifSkopeExecutablePath));
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

    private void SaveConfiguration(string? activeGame, ApplicationThemeFamily themeFamily, ApplicationThemeMode themeMode, string recordTextLanguage, string? nifSkopeExecutablePath)
    {
        var configuration = new ApplicationConfiguration
        {
            ActiveGame = activeGame,
            ThemeFamily = themeFamily,
            ThemeMode = themeMode,
            RecordTextLanguage = recordTextLanguage,
            NifSkopeExecutablePath = nifSkopeExecutablePath,
            ApplicationDataDirectory = ConfigurationStore.Current.ApplicationDataDirectory,
            DatabaseDirectory = ConfigurationStore.Current.DatabaseDirectory,
            LoggingDirectory = ConfigurationStore.Current.LoggingDirectory
        };

        ConfigurationStore.Save(configuration);
    }

    private static string? NormalizeOptionalPath(string? path)
    {
        return string.IsNullOrWhiteSpace(path)
            ? null
            : path.Trim();
    }

    private static Language NormalizeRecordTextLanguage(string? language)
    {
        return Enum.TryParse<Language>(language, true, out var parsedLanguage)
            ? parsedLanguage
            : Language.English;
    }
}
