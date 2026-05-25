using Mutagen.Bethesda;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Starfield;
using Serilog;
using SFRecordCompareEngine.Core.Configuration.Interfaces;

namespace SFRecordCompareEngine.Core.Configuration;

public class GameConfigurationStore : IGameConfigurationStore
{
    private readonly ILogger Logger = Log.ForContext<GameConfigurationStore>();

    /// <summary>
    ///     The currently selected game.
    /// </summary>
    public string? SelectedGame { get; set; }

    /// <summary>
    ///     The game environment for the currently selected game.
    /// </summary>
    public IGameEnvironment? Game { get; set; }

    public string[] SupportedGames { get; set; } = ["None", "Starfield", "Skyrim", "Fallout 4"];

    public void SelectGame(string? game)
    {
        SelectedGame = game;
        switch (game)
        {
            case "None":
                SelectedGame = null;
                Game = null;
                break;

            case "Starfield":
                SelectedGame = "Starfield";
                Game = GameEnvironment.Typical.Starfield(StarfieldRelease.Starfield);
                break;
            case "Skyrim":
                Logger.Warning("Skyrim is not currently supported. Please select Starfield");
                SelectedGame = "Skyrim";
                Game = GameEnvironment.Typical.Skyrim(SkyrimRelease.SkyrimSE);
                break;
            case "Fallout 4":
                Logger.Warning("Fallout 4 is not currently supported. Please select Starfield");
                SelectedGame = "Fallout 4";
                Game = GameEnvironment.Typical.Fallout4(Fallout4Release.Fallout4);
                break;
            default:
                SelectedGame = null;
                Game = null;
                Logger.Error("Game '{Game}' is not supported. Please select Starfield", game);
                break;
        }
    }

    public void ClearActiveGame()
    {
        SelectedGame = null;
        Game = null;
    }
}