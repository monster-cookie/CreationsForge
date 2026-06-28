using CreationsForge.Core.DTOs.Games;
using CreationsForge.Core.Enums;

namespace CreationsForge.Core.Services.Interfaces;

/// <summary>
/// Provides supported game choices and persisted active-game selection.
/// </summary>
public interface IGameSelectionService
{
    /// <summary>
    /// Gets the supported games that can be selected in CreationsForge workflows.
    /// </summary>
    /// <returns>The supported game options with display labels.</returns>
    IReadOnlyList<SupportedGameDTO> GetSupportedGames();

    /// <summary>
    /// Gets the currently configured active game when one is valid.
    /// </summary>
    /// <returns>The configured game, or <see langword="null"/> when no valid game is configured.</returns>
    SupportedGame? GetActiveGame();

    /// <summary>
    /// Persists the active game without changing other configuration settings.
    /// </summary>
    /// <param name="game">The game to store as active.</param>
    void SetActiveGame(SupportedGame game);
}
