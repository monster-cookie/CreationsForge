using CreationsForge.Core.DTOs.Games;

namespace CreationsForge.Core.Repositories.Interfaces;

public interface IGameRepository
{
    void Save(GameDTO dto);
}
