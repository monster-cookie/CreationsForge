using CreationsForge.Core.DTOs.Games;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Services.Interfaces;

namespace CreationsForge.PresentationTests.Support;

/// <summary>
/// Supplies and records the persisted active-game preference without touching machine configuration.
/// </summary>
internal sealed class FakeGameSelectionService : IGameSelectionService
{
    /// <summary>Gets or sets the supported games returned for presentation selection.</summary>
    public IReadOnlyList<SupportedGameDTO> SupportedGames { get; set; } = [];

    /// <summary>Gets or sets the game returned as the current preference.</summary>
    public SupportedGame? ActiveGame { get; set; }

    /// <summary>Gets the last game saved through this fake.</summary>
    public SupportedGame? SavedGame { get; private set; }

    /// <summary>Gets or sets whether saving the active game throws a simulated configuration failure.</summary>
    public bool ThrowOnSave { get; set; }

    /// <inheritdoc />
    public IReadOnlyList<SupportedGameDTO> GetSupportedGames()
    {
        return SupportedGames;
    }

    /// <inheritdoc />
    public SupportedGame? GetActiveGame()
    {
        return ActiveGame;
    }

    /// <inheritdoc />
    public void SetActiveGame(SupportedGame game)
    {
        if (ThrowOnSave)
        {
            throw new IOException("The test configuration could not be saved.");
        }

        SavedGame = game;
        ActiveGame = game;
    }
}
