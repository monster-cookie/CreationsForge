using Mutagen.Bethesda.Plugins;
using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Models.Database;
using SFRecordCompareEngine.Core.Repositories.Interfaces;

namespace SFRecordCompareEngine.Core.Repositories;

public class ScriptingAdapterPropertyListItemRepository : IScriptingAdapterPropertyListItemRepository
{
    private readonly IDatabase Database;

    public ScriptingAdapterPropertyListItemRepository(IDatabase database)
    {
        Database = database;
    }

    public IList<ScriptingAdapterPropertyListItemDTO> GetByRecord(ModKey modKey, string recordType, FormKey formKey)
    {
        return Database.Fetch<ScriptingAdapterPropertyListItem>(
                """
                SELECT *
                FROM ScriptingAdapterPropertyListItems
                WHERE ModKey_Name = @ModKeyName AND ModKey_Type = @ModKeyType AND ModKey_FileName = @ModKeyFileName COLLATE NOCASE
                  AND RecordType = @RecordType
                  AND FormKey_ID = @FormKeyID
                ORDER BY ScriptingAdapter_Name, Property_Index, ListItem_Index;
                """,
                new { ModKeyName = modKey.Name, ModKeyType = (int)modKey.Type, ModKeyFileName = modKey.FileName, RecordType = recordType, FormKeyID = formKey.ID })
            .Select(model => new ScriptingAdapterPropertyListItemDTO(model))
            .ToList();
    }

    public void Save(ScriptingAdapterPropertyListItemDTO dto)
    {
        Database.Save(new ScriptingAdapterPropertyListItem(dto));
    }
}
