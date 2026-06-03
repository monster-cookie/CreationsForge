using Mutagen.Bethesda.Plugins;
using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Enums;
using SFRecordCompareEngine.Core.Models.Database;
using SFRecordCompareEngine.Core.Repositories.Interfaces;

namespace SFRecordCompareEngine.Core.Repositories;

public class MiscItemRepository : IMiscItemRepository
{
    private readonly IDatabase Database;

    public MiscItemRepository(IDatabase database)
    {
        Database = database;
    }

    public IList<MiscItemDTO> GetByModKey(ModKey modKey)
    {
        return Database.Fetch<MiscItem>("SELECT * FROM MiscItem WHERE ModKey_Name = @ModKeyName AND ModKey_Type = @ModKeyType AND ModKey_FileName = @ModKeyFileName COLLATE NOCASE ORDER BY FormKey_ID;", new { ModKeyName = modKey.Name, ModKeyType = (int)modKey.Type, ModKeyFileName = modKey.FileName }).Select(x => new MiscItemDTO(x)).ToList();
    }

    public IList<RecordTreeEntryDTO> GetRecordTreeEntriesByModKey(ModKey modKey)
    {
        return Database.Fetch<MiscItem>("SELECT FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, EditorID FROM MiscItem WHERE ModKey_Name = @ModKeyName AND ModKey_Type = @ModKeyType AND ModKey_FileName = @ModKeyFileName COLLATE NOCASE ORDER BY FormKey_ID;", new { ModKeyName = modKey.Name, ModKeyType = (int)modKey.Type, ModKeyFileName = modKey.FileName })
            .Select(x => new RecordTreeEntryDTO
            {
                FormKey = new FormKey(new ModKey(x.FormKeyModKeyName, (ModType)x.FormKeyModKeyType), (uint)x.FormKeyId),
                EditorID = x.EditorId
            })
            .ToList();
    }

    public IList<MiscItemDTO> GetByFormKey(FormKey formKey)
    {
        return Database
            .Fetch<MiscItem>("SELECT MiscItem.* FROM MiscItem INNER JOIN Plugins ON Plugins.ModKey_Name = MiscItem.ModKey_Name AND Plugins.ModKey_Type = MiscItem.ModKey_Type AND Plugins.ModKey_FileName = MiscItem.ModKey_FileName WHERE MiscItem.FormKey_ModKey_Name = @FormKeyModKeyName AND MiscItem.FormKey_ModKey_Type = @FormKeyModKeyType AND MiscItem.FormKey_ModKey_FileName = @FormKeyModKeyFileName AND MiscItem.FormKey_ID = @FormKeyID AND Plugins.Enabled = 1 AND Plugins.ExistsOnDisk = 1 AND Plugins.ImportState = @ImportState ORDER BY Plugins.LoadOrderIndex;",
                new { FormKeyModKeyName = formKey.ModKey.Name, FormKeyModKeyType = (int)formKey.ModKey.Type, FormKeyModKeyFileName = formKey.ModKey.FileName, FormKeyID = formKey.ID, ImportState = nameof(PluginImportState.Current) }).Select(x => new MiscItemDTO(x)).ToList();
    }

    public void Save(MiscItemDTO dto)
    {
        Database.Save(new MiscItem(dto));
    }
}
