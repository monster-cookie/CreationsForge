using CreationsForge.Skyrim.DTOs;

namespace CreationsForge.Skyrim.Repositories.Interfaces;

public interface ISkyrimPluginRepository
{
    void Save(SkyrimPluginDTO dto);
}
