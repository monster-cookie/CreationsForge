using CreationsForge.Core.DTOs.Games;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Services.Interfaces;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Installs;
using Mutagen.Bethesda.Skyrim;

namespace CreationsForge.Skyrim;

public class SkyrimGameMetadataService : IGameMetadataService
{
    public SupportedGame Game => SupportedGame.Skyrim;

    public GameDTO GetGame()
    {
        var gameRelease = GameRelease.SkyrimSE;
        _ = SkyrimRelease.SkyrimSE;

        return new GameDTO
        {
            Game = Game,
            DisplayName = gameRelease.ToString(),
            InstallationFolder = GameLocations.GetGameFolder(gameRelease).Path,
            DataFolder = GameLocations.GetDataFolder(gameRelease).Path
        };
    }
}
