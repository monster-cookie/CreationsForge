using CreationsForge.Core.Configuration.Interfaces;
using CreationsForge.Core.DTOs.Games;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Models.Configuration;
using CreationsForge.Core.Services.Interfaces;

namespace CreationsForge.Core.Services;

/// <summary>
/// Provides supported game choices and active-game persistence.
/// </summary>
public class GameSelectionService : IGameSelectionService
{
    private readonly IApplicationConfigurationStore ConfigurationStore;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameSelectionService"/> class.
    /// </summary>
    /// <param name="configurationStore">The configuration store used to read and save the active game.</param>
    public GameSelectionService(IApplicationConfigurationStore configurationStore)
    {
        ConfigurationStore = configurationStore;
    }

    /// <inheritdoc />
    public IReadOnlyList<SupportedGameDTO> GetSupportedGames()
    {
        return
        [
            CreateGameOption(SupportedGame.Starfield, "Starfield"),
            CreateGameOption(SupportedGame.Fallout4, "Fallout 4"),
            CreateGameOption(SupportedGame.Skyrim, "Skyrim Special Edition")
        ];
    }

    /// <inheritdoc />
    public SupportedGame? GetActiveGame()
    {
        return Enum.TryParse<SupportedGame>(ConfigurationStore.Current.ActiveGame, true, out var game)
            ? game
            : null;
    }

    /// <inheritdoc />
    public void SetActiveGame(SupportedGame game)
    {
        var configuration = new ApplicationConfiguration
        {
            ActiveGame = game.ToString(),
            ThemeFamily = ConfigurationStore.Current.ThemeFamily,
            ThemeMode = ConfigurationStore.Current.ThemeMode,
            RecordTextLanguage = ConfigurationStore.Current.RecordTextLanguage,
            NifSkopeExecutablePath = ConfigurationStore.Current.NifSkopeExecutablePath,
            PreferEspOverMatchingEsm = ConfigurationStore.Current.PreferEspOverMatchingEsm,
            ApplicationDataDirectory = ConfigurationStore.Current.ApplicationDataDirectory,
            DatabaseDirectory = ConfigurationStore.Current.DatabaseDirectory,
            LoggingDirectory = ConfigurationStore.Current.LoggingDirectory
        };
        ConfigurationStore.Save(configuration);
    }

    /// <summary>
    /// Creates a supported-game option for presentation and command-line selection surfaces.
    /// </summary>
    /// <param name="game">The supported game identifier.</param>
    /// <param name="displayName">The display label for the game.</param>
    /// <returns>The supported game DTO.</returns>
    private static SupportedGameDTO CreateGameOption(SupportedGame game, string displayName)
    {
        return new SupportedGameDTO
        {
            Game = game,
            Name = game.ToString(),
            DisplayName = displayName
        };
    }
}
