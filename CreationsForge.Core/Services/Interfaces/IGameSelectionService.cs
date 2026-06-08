using CreationsForge.Core.DTOs.Games;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Models.Configuration;

namespace CreationsForge.Core.Services.Interfaces;

public interface IGameSelectionService
{
    IReadOnlyList<SupportedGameDTO> GetSupportedGames();

    SupportedGame? GetActiveGame();

    ApplicationThemeMode GetThemeMode();

    ApplicationThemeFamily GetThemeFamily();

    void SetActiveGame(SupportedGame game);

    void SetThemeMode(ApplicationThemeMode themeMode);

    void SetThemeFamily(ApplicationThemeFamily themeFamily);

    void SetActiveGameAndThemeMode(SupportedGame game, ApplicationThemeMode themeMode);

    void SetActiveGameAndTheme(SupportedGame game, ApplicationThemeFamily themeFamily, ApplicationThemeMode themeMode);

    void SetTheme(ApplicationThemeFamily themeFamily, ApplicationThemeMode themeMode);
}
