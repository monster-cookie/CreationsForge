using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Models.Database;
using SFRecordCompareEngine.Core.Repositories.Interfaces;

namespace SFRecordCompareEngine.Core.Repositories;

public class GameSettingRepository : IGameSettingRepository
{
    private readonly IDatabase Database;

    public GameSettingRepository(IDatabase database)
    {
        Database = database;
    }

    /// <inheritdoc/>
    public void Save(GameSettingDTO dto)
    {
        var model = new GameSetting(dto);
        Database.Save(model);
    }
}
