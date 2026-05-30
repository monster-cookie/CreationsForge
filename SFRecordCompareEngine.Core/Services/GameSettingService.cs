using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Repositories.Interfaces;
using SFRecordCompareEngine.Core.Services.Interfaces;

namespace SFRecordCompareEngine.Core.Services;

public class GameSettingService : IGameSettingService
{
    private readonly IGameSettingRepository GameSettingRepository;

    public GameSettingService(IGameSettingRepository gameSettingRepository)
    {
        GameSettingRepository = gameSettingRepository;
    }

    public IList<GameSettingDTO> GetByModKey(ModKey modKey)
    {
        return GameSettingRepository.GetByModKey(modKey);
    }

    public IList<GameSettingDTO> GetByFormKeyID(uint formKeyID)
    {
        return GameSettingRepository.GetByFormKeyID(formKeyID);
    }
}