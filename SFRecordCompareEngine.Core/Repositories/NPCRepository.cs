using Mutagen.Bethesda.Plugins;
using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Enums;
using SFRecordCompareEngine.Core.Models.Database;
using SFRecordCompareEngine.Core.Repositories.Interfaces;

namespace SFRecordCompareEngine.Core.Repositories;

public class NPCRepository : INPCRepository
{
    private readonly IDatabase Database;

    public NPCRepository(IDatabase database)
    {
        Database = database;
    }

    public IList<NPCDTO> GetByModKey(ModKey modKey)
    {
        return Database.Fetch<NPC>("SELECT * FROM NPC WHERE ModKey_Name = @ModKeyName AND ModKey_Type = @ModKeyType AND ModKey_FileName = @ModKeyFileName COLLATE NOCASE ORDER BY FormKey_ID;", new { ModKeyName = modKey.Name, ModKeyType = (int)modKey.Type, ModKeyFileName = modKey.FileName }).Select(x => new NPCDTO(x)).ToList();
    }

    public IList<NPCDTO> GetByFormKeyID(uint formKeyID)
    {
        return Database.Fetch<NPC>("SELECT NPC.* FROM NPC INNER JOIN Plugins ON Plugins.ModKey_Name = NPC.ModKey_Name AND Plugins.ModKey_Type = NPC.ModKey_Type AND Plugins.ModKey_FileName = NPC.ModKey_FileName WHERE NPC.FormKey_ID = @FormKeyID AND Plugins.Enabled = 1 AND Plugins.ExistsOnDisk = 1 AND Plugins.ImportState = @ImportState ORDER BY Plugins.LoadOrderIndex;",
            new { FormKeyID = formKeyID, ImportState = nameof(PluginImportState.Current) }).Select(x => new NPCDTO(x)).ToList();
    }

    public void Save(NPCDTO dto)
    {
        Database.Save(new NPC(dto));
    }
}