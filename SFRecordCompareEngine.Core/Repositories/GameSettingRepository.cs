using Mutagen.Bethesda.Plugins;
using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Enums;
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

    /// <inheritdoc />
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

    /// <inheritdoc />
    public IList<RecordTreeEntryDTO> GetRecordTreeEntriesByModKey(ModKey modKey)
    {
        return Database.Fetch<GameSetting>(
                """
                SELECT FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, EditorID
                FROM GameSetting
                WHERE ModKey_Name = @ModKeyName AND ModKey_Type = @ModKeyType AND ModKey_FileName = @ModKeyFileName COLLATE NOCASE
                ORDER BY FormKey_ID;
                """,
                new { ModKeyName = modKey.Name, ModKeyType = (int)modKey.Type, ModKeyFileName = modKey.FileName })
            .Select(gameSetting => new RecordTreeEntryDTO
            {
                FormKey = new FormKey(new ModKey(gameSetting.FormKeyModKeyName, (ModType)gameSetting.FormKeyModKeyType), (uint)gameSetting.FormKeyId),
                EditorID = gameSetting.EditorId
            })
            .ToList();
    }

    /// <inheritdoc />
    public IList<GameSettingDTO> GetByFormKey(FormKey formKey)
    {
        return Database.Fetch<GameSetting>(
                """
                SELECT GameSetting.*
                FROM GameSetting
                INNER JOIN Plugins
                    ON Plugins.ModKey_Name = GameSetting.ModKey_Name
                    AND Plugins.ModKey_Type = GameSetting.ModKey_Type
                    AND Plugins.ModKey_FileName = GameSetting.ModKey_FileName
                WHERE GameSetting.FormKey_ModKey_Name = @FormKeyModKeyName AND GameSetting.FormKey_ModKey_Type = @FormKeyModKeyType AND GameSetting.FormKey_ModKey_FileName = @FormKeyModKeyFileName AND GameSetting.FormKey_ID = @FormKeyID
                  AND Plugins.Enabled = 1
                  AND Plugins.ExistsOnDisk = 1
                  AND Plugins.ImportState = @ImportState
                ORDER BY Plugins.LoadOrderIndex;
                """,
                new { FormKeyModKeyName = formKey.ModKey.Name, FormKeyModKeyType = (int)formKey.ModKey.Type, FormKeyModKeyFileName = formKey.ModKey.FileName, FormKeyID = formKey.ID, ImportState = nameof(PluginImportState.Current) })
            .Select(gameSetting => new GameSettingDTO(gameSetting))
            .ToList();
    }

    /// <inheritdoc />
    public void Save(GameSettingDTO dto)
    {
        var model = new GameSetting(dto);
        Database.Save(model);
    }
}
