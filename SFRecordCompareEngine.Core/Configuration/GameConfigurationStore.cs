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

    /// <inheritdoc />
    public string? SelectedGame { get; set; }

    /// <inheritdoc />
    public IGameEnvironment? Game { get; set; }

    /// <inheritdoc />
    public GameRelease? Release { get; set; }

    /// <inheritdoc />
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
                Release = StarfieldRelease.Starfield.ToGameRelease();
                break;
            case "Skyrim":
                Logger.Warning("Skyrim is not currently supported. Please select Starfield");
                SelectedGame = "Skyrim";
                Game = GameEnvironment.Typical.Skyrim(SkyrimRelease.SkyrimSE);
                Release = SkyrimRelease.SkyrimSE.ToGameRelease();
                break;
            case "Fallout 4":
                Logger.Warning("Fallout 4 is not currently supported. Please select Starfield");
                SelectedGame = "Fallout 4";
                Game = GameEnvironment.Typical.Fallout4(Fallout4Release.Fallout4);
                Release = Fallout4Release.Fallout4.ToGameRelease();
                break;
            default:
                SelectedGame = null;
                Game = null;
                Release = null;
                Logger.Error("Game '{Game}' is not supported. Please select Starfield", game);
                break;
        }
    }

    public void ClearActiveGame()
    {
        SelectedGame = null;
        Game = null;
        Release = null;
    }
}