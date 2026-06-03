using Mutagen.Bethesda.Plugins;
using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Enums;
using SFRecordCompareEngine.Core.Models.Database;
using SFRecordCompareEngine.Core.Repositories.Interfaces;

namespace SFRecordCompareEngine.Core.Repositories;

public class MagicEffectRepository : IMagicEffectRepository
{
    private readonly IDatabase Database;

    public MagicEffectRepository(IDatabase database)
    {
        Database = database;
    }

    public IList<MagicEffectDTO> GetByModKey(ModKey modKey)
    {
        return Database.Fetch<MagicEffect>("SELECT * FROM MagicEffect WHERE ModKey_Name = @ModKeyName AND ModKey_Type = @ModKeyType AND ModKey_FileName = @ModKeyFileName COLLATE NOCASE ORDER BY FormKey_ID;", new { ModKeyName = modKey.Name, ModKeyType = (int)modKey.Type, ModKeyFileName = modKey.FileName }).Select(x => new MagicEffectDTO(x)).ToList();
    }

    public IList<RecordTreeEntryDTO> GetRecordTreeEntriesByModKey(ModKey modKey)
    {
        return Database.Fetch<MagicEffect>("SELECT FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, EditorID FROM MagicEffect WHERE ModKey_Name = @ModKeyName AND ModKey_Type = @ModKeyType AND ModKey_FileName = @ModKeyFileName COLLATE NOCASE ORDER BY FormKey_ID;", new { ModKeyName = modKey.Name, ModKeyType = (int)modKey.Type, ModKeyFileName = modKey.FileName })
            .Select(x => new RecordTreeEntryDTO
            {
                FormKey = new FormKey(new ModKey(x.FormKeyModKeyName, (ModType)x.FormKeyModKeyType), (uint)x.FormKeyId),
                EditorID = x.EditorId
            })
            .ToList();
    }

    public IList<MagicEffectDTO> GetByFormKey(FormKey formKey)
    {
        return Database
            .Fetch<MagicEffect>(
                "SELECT MagicEffect.* FROM MagicEffect INNER JOIN Plugins ON Plugins.ModKey_Name = MagicEffect.ModKey_Name AND Plugins.ModKey_Type = MagicEffect.ModKey_Type AND Plugins.ModKey_FileName = MagicEffect.ModKey_FileName WHERE MagicEffect.FormKey_ModKey_Name = @FormKeyModKeyName AND MagicEffect.FormKey_ModKey_Type = @FormKeyModKeyType AND MagicEffect.FormKey_ModKey_FileName = @FormKeyModKeyFileName AND MagicEffect.FormKey_ID = @FormKeyID AND Plugins.Enabled = 1 AND Plugins.ExistsOnDisk = 1 AND Plugins.ImportState = @ImportState ORDER BY Plugins.LoadOrderIndex;",
                new { FormKeyModKeyName = formKey.ModKey.Name, FormKeyModKeyType = (int)formKey.ModKey.Type, FormKeyModKeyFileName = formKey.ModKey.FileName, FormKeyID = formKey.ID, ImportState = nameof(PluginImportState.Current) }).Select(x => new MagicEffectDTO(x)).ToList();
    }

    public void Save(MagicEffectDTO dto)
    {
        Database.Save(new MagicEffect(dto));
    }
}
