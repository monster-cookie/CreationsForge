using Mutagen.Bethesda.Plugins;
using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Enums;
using SFRecordCompareEngine.Core.Repositories.Interfaces;
using GlobalModel = SFRecordCompareEngine.Core.Models.Database.Global;

namespace SFRecordCompareEngine.Core.Repositories;

public class GlobalRepository : IGlobalRepository
{
    private readonly IDatabase Database;

    public GlobalRepository(IDatabase database)
    {
        Database = database;
    }

    public IList<GlobalDTO> GetByModKey(ModKey modKey)
    {
        return Database.Fetch<GlobalModel>("SELECT * FROM Global WHERE ModKey_Name = @ModKeyName AND ModKey_Type = @ModKeyType AND ModKey_FileName = @ModKeyFileName COLLATE NOCASE ORDER BY FormKey_ID;", new { ModKeyName = modKey.Name, ModKeyType = (int)modKey.Type, ModKeyFileName = modKey.FileName }).Select(x => new GlobalDTO(x)).ToList();
    }

    public IList<RecordTreeEntryDTO> GetRecordTreeEntriesByModKey(ModKey modKey)
    {
        return Database.Fetch<GlobalModel>("SELECT FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, EditorID FROM Global WHERE ModKey_Name = @ModKeyName AND ModKey_Type = @ModKeyType AND ModKey_FileName = @ModKeyFileName COLLATE NOCASE ORDER BY FormKey_ID;", new { ModKeyName = modKey.Name, ModKeyType = (int)modKey.Type, ModKeyFileName = modKey.FileName })
            .Select(x => new RecordTreeEntryDTO
            {
                FormKey = new FormKey(new ModKey(x.FormKeyModKeyName, (ModType)x.FormKeyModKeyType), (uint)x.FormKeyId),
                EditorID = x.EditorId
            })
            .ToList();
    }

    public IList<GlobalDTO> GetByFormKey(FormKey formKey)
    {
        return Database.Fetch<GlobalModel>("SELECT Global.* FROM Global INNER JOIN Plugins ON Plugins.ModKey_Name = Global.ModKey_Name AND Plugins.ModKey_Type = Global.ModKey_Type AND Plugins.ModKey_FileName = Global.ModKey_FileName WHERE Global.FormKey_ModKey_Name = @FormKeyModKeyName AND Global.FormKey_ModKey_Type = @FormKeyModKeyType AND Global.FormKey_ModKey_FileName = @FormKeyModKeyFileName AND Global.FormKey_ID = @FormKeyID AND Plugins.Enabled = 1 AND Plugins.ExistsOnDisk = 1 AND Plugins.ImportState = @ImportState ORDER BY Plugins.LoadOrderIndex;",
            new { FormKeyModKeyName = formKey.ModKey.Name, FormKeyModKeyType = (int)formKey.ModKey.Type, FormKeyModKeyFileName = formKey.ModKey.FileName, FormKeyID = formKey.ID, ImportState = nameof(PluginImportState.Current) }).Select(x => new GlobalDTO(x)).ToList();
    }

    public void Save(GlobalDTO dto)
    {
        Database.Save(new GlobalModel(dto));
    }
}
