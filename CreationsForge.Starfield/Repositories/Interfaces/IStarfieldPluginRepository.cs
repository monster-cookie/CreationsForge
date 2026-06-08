using CreationsForge.Starfield.DTOs;

namespace CreationsForge.Starfield.Repositories.Interfaces;

public interface IStarfieldPluginRepository
{
    void Save(StarfieldPluginDTO dto);
}
