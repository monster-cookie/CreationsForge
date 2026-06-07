using CreationsForge.Core.DTOs.Games;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Services.Interfaces;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Installs;
using Mutagen.Bethesda.Starfield;

namespace CreationsForge.Starfield;

public class StarfieldGameMetadataService : IGameMetadataService
{
    public SupportedGame Game => SupportedGame.Starfield;

    public GameDTO GetGame()
    {
        var gameRelease = GameRelease.Starfield;
        _ = StarfieldRelease.Starfield;

        return new GameDTO
        {
            Game = Game,
            DisplayName = gameRelease.ToString(),
            InstallationFolder = GameLocations.GetGameFolder(gameRelease).Path,
            DataFolder = GameLocations.GetDataFolder(gameRelease).Path
        };
    }
}
