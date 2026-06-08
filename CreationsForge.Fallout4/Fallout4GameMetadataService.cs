using CreationsForge.Core.DTOs.Games;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Services.Interfaces;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Installs;
using Mutagen.Bethesda.Fallout4;

namespace CreationsForge.Fallout4;

public class Fallout4GameMetadataService : IGameMetadataService
{
    public SupportedGame Game => SupportedGame.Fallout4;

    public GameDTO GetGame()
    {
        var gameRelease = GameRelease.Fallout4;
        _ = Fallout4Release.Fallout4;

        return new GameDTO
        {
            Game = Game,
            DisplayName = gameRelease.ToString(),
            InstallationFolder = GameLocations.GetGameFolder(gameRelease).Path,
            DataFolder = GameLocations.GetDataFolder(gameRelease).Path
        };
    }
}
