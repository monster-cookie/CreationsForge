using Mutagen.Bethesda.Plugins;
using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Enums;
using SFRecordCompareEngine.Core.Models.Database;
using SFRecordCompareEngine.Core.Repositories.Interfaces;

namespace SFRecordCompareEngine.Core.Repositories;

public class PerkRepository : IPerkRepository
{
    private readonly IDatabase Database;

    public PerkRepository(IDatabase database)
    {
        Database = database;
    }

    public IList<PerkDTO> GetByModKey(ModKey modKey)
    {
        return Database.Fetch<Perk>("SELECT * FROM Perk WHERE ModKey_Name = @ModKeyName AND ModKey_Type = @ModKeyType AND ModKey_FileName = @ModKeyFileName COLLATE NOCASE ORDER BY FormKey_ID;", new { ModKeyName = modKey.Name, ModKeyType = (int)modKey.Type, ModKeyFileName = modKey.FileName }).Select(x => new PerkDTO(x)).ToList();
    }

    public IList<PerkDTO> GetByFormKey(FormKey formKey)
    {
        return Database.Fetch<Perk>("SELECT Perk.* FROM Perk INNER JOIN Plugins ON Plugins.ModKey_Name = Perk.ModKey_Name AND Plugins.ModKey_Type = Perk.ModKey_Type AND Plugins.ModKey_FileName = Perk.ModKey_FileName WHERE Perk.FormKey_ModKey_Name = @FormKeyModKeyName AND Perk.FormKey_ModKey_Type = @FormKeyModKeyType AND Perk.FormKey_ModKey_FileName = @FormKeyModKeyFileName AND Perk.FormKey_ID = @FormKeyID AND Plugins.Enabled = 1 AND Plugins.ExistsOnDisk = 1 AND Plugins.ImportState = @ImportState ORDER BY Plugins.LoadOrderIndex;",
            new { FormKeyModKeyName = formKey.ModKey.Name, FormKeyModKeyType = (int)formKey.ModKey.Type, FormKeyModKeyFileName = formKey.ModKey.FileName, FormKeyID = formKey.ID, ImportState = nameof(PluginImportState.Current) }).Select(x => new PerkDTO(x)).ToList();
    }

    public void Save(PerkDTO dto)
    {
        Database.Save(new Perk(dto));
    }
}