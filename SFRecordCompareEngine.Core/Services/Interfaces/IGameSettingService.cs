using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Services.Interfaces;

public interface IGameSettingService
{
    IList<GameSettingDTO> GetByModKey(ModKey modKey);
    IList<GameSettingDTO> GetByFormKeyID(uint formKeyID);
}