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
    public PerkRepository(IDatabase database) { Database = database; }
    public IList<PerkDTO> GetByModKey(ModKey modKey) => Database.Fetch<Perk>("SELECT * FROM Perk WHERE ModKey_Name = @ModKeyName AND ModKey_Type = @ModKeyType AND ModKey_FileName = @ModKeyFileName COLLATE NOCASE ORDER BY FormKey_ID;", new { ModKeyName = modKey.Name, ModKeyType = (int)modKey.Type, ModKeyFileName = modKey.FileName }).Select(x => new PerkDTO(x)).ToList();
    public IList<PerkDTO> GetByFormKeyID(uint formKeyID) => Database.Fetch<Perk>("SELECT Perk.* FROM Perk INNER JOIN Plugins ON Plugins.ModKey_Name = Perk.ModKey_Name AND Plugins.ModKey_Type = Perk.ModKey_Type AND Plugins.ModKey_FileName = Perk.ModKey_FileName WHERE Perk.FormKey_ID = @FormKeyID AND Plugins.Enabled = 1 AND Plugins.ExistsOnDisk = 1 AND Plugins.ImportState = @ImportState ORDER BY Plugins.LoadOrderIndex;", new { FormKeyID = formKeyID, ImportState = nameof(PluginImportState.Current) }).Select(x => new PerkDTO(x)).ToList();
    public void Save(PerkDTO dto) { Database.Save(new Perk(dto)); }
}
