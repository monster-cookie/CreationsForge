using CreationsForge.Core.DTOs.Games;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Models.Configuration;
using Mutagen.Bethesda.Strings;

namespace CreationsForge.Core.Services.Interfaces;

public interface IGameSelectionService
{
    IReadOnlyList<SupportedGameDTO> GetSupportedGames();

    SupportedGame? GetActiveGame();

    ApplicationThemeMode GetThemeMode();

    ApplicationThemeFamily GetThemeFamily();

    IReadOnlyList<Language> GetRecordTextLanguages()
    {
        return [Language.English];
    }

    Language GetRecordTextLanguage()
    {
        return Language.English;
    }

    string? GetNifSkopeExecutablePath()
    {
        return null;
    }

    void SetActiveGame(SupportedGame game);

    void SetThemeMode(ApplicationThemeMode themeMode);

    void SetThemeFamily(ApplicationThemeFamily themeFamily);

    void SetActiveGameAndThemeMode(SupportedGame game, ApplicationThemeMode themeMode);

    void SetActiveGameAndTheme(SupportedGame game, ApplicationThemeFamily themeFamily, ApplicationThemeMode themeMode);

    void SetTheme(ApplicationThemeFamily themeFamily, ApplicationThemeMode themeMode);

    void SetThemeRecordTextLanguageAndNifSkopeExecutablePath(ApplicationThemeFamily themeFamily, ApplicationThemeMode themeMode, Language recordTextLanguage, string? nifSkopeExecutablePath)
    {
        SetTheme(themeFamily, themeMode);
    }

    void SetActiveGameThemeAndNifSkopeExecutablePath(SupportedGame game, ApplicationThemeFamily themeFamily, ApplicationThemeMode themeMode, string? nifSkopeExecutablePath)
    {
        SetActiveGameAndTheme(game, themeFamily, themeMode);
    }

    void SetActiveGameThemeRecordTextLanguageAndNifSkopeExecutablePath(SupportedGame game, ApplicationThemeFamily themeFamily, ApplicationThemeMode themeMode, Language recordTextLanguage, string? nifSkopeExecutablePath)
    {
        SetActiveGameAndTheme(game, themeFamily, themeMode);
    }

    void SetThemeAndNifSkopeExecutablePath(ApplicationThemeFamily themeFamily, ApplicationThemeMode themeMode, string? nifSkopeExecutablePath)
    {
        SetTheme(themeFamily, themeMode);
    }
}
