using CreationsForge.Core.Enums;

namespace CreationsForge.Core.Services.Interfaces;

public interface IGameImportReadinessService
{
    bool HasImportedData(SupportedGame game);
}
