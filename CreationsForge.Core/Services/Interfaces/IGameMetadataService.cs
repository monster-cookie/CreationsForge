using CreationsForge.Core.DTOs.Games;
using CreationsForge.Core.Enums;

namespace CreationsForge.Core.Services.Interfaces;

public interface IGameMetadataService
{
    SupportedGame Game { get; }

    GameDTO GetGame();
}
