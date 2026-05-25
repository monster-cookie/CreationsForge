using Mutagen.Bethesda;
using Mutagen.Bethesda.Environments;

namespace SFRecordCompareEngine.Core.Configuration.Interfaces;

public interface IGameConfigurationStore
{
    /// <summary>
    /// The active game environment.
    /// </summary>
    string? SelectedGame { get; set; }

    /// <summary>
    /// The game environment for the currently selected game.
    /// </summary>
    IGameEnvironment? Game { get; set; }

    /// <summary>
    /// The release marker for the currently selected game. 
    /// </summary>
    public GameRelease? Release { get; set; }
    
    /// <summary>
    /// The Bethesda games we support currently.
    /// </summary>
    public string[] SupportedGames { get; set; }

    /// <summary>
    /// Select the game to use for the engine.
    /// </summary>
    /// <param name="game">The selected game see </param>
    void SelectGame(string? game);

    /// <summary>
    /// Clear the active game.
    /// </summary>
    void ClearActiveGame();
}