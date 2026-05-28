using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Repositories.Interfaces;

public interface IGameSettingRepository
{
    /// <summary>
    /// Saves a game setting record.
    /// </summary>
    void Save(GameSettingDTO dto);
}
