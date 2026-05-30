using Mutagen.Bethesda.Plugins;
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
    public IList<GameSettingDTO> GetByModKey(ModKey modKey)
    {
        return Database.Fetch<GameSetting>(
                """
                SELECT *
                FROM GameSetting
                WHERE ModKey_Name = @ModKeyName AND ModKey_Type = @ModKeyType AND ModKey_FileName = @ModKeyFileName COLLATE NOCASE
                ORDER BY FormKey_ID;
                """,
                new { ModKeyName = modKey.Name, ModKeyType = (int)modKey.Type, ModKeyFileName = modKey.FileName })
            .Select(gameSetting => new GameSettingDTO(gameSetting))
            .ToList();
    }

    /// <inheritdoc/>
    public void Save(GameSettingDTO dto)
    {
        var model = new GameSetting(dto);
        Database.Save(model);
    }
}
