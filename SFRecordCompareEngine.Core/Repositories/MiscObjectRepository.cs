using Mutagen.Bethesda.Plugins;
using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Enums;
using SFRecordCompareEngine.Core.Models.Database;
using SFRecordCompareEngine.Core.Repositories.Interfaces;
namespace SFRecordCompareEngine.Core.Repositories;
public class MiscObjectRepository : IMiscObjectRepository
{
    private readonly IDatabase Database;
    public MiscObjectRepository(IDatabase database) { Database = database; }
    public IList<MiscObjectDTO> GetByModKey(ModKey modKey) => Database.Fetch<MiscObject>("SELECT * FROM MiscObject WHERE ModKey_Name = @ModKeyName AND ModKey_Type = @ModKeyType AND ModKey_FileName = @ModKeyFileName COLLATE NOCASE ORDER BY FormKey_ID;", new { ModKeyName = modKey.Name, ModKeyType = (int)modKey.Type, ModKeyFileName = modKey.FileName }).Select(x => new MiscObjectDTO(x)).ToList();
    public IList<MiscObjectDTO> GetByFormKeyID(uint formKeyID) => Database.Fetch<MiscObject>("SELECT MiscObject.* FROM MiscObject INNER JOIN Plugins ON Plugins.ModKey_Name = MiscObject.ModKey_Name AND Plugins.ModKey_Type = MiscObject.ModKey_Type AND Plugins.ModKey_FileName = MiscObject.ModKey_FileName WHERE MiscObject.FormKey_ID = @FormKeyID AND Plugins.Enabled = 1 AND Plugins.ExistsOnDisk = 1 AND Plugins.ImportState = @ImportState ORDER BY Plugins.LoadOrderIndex;", new { FormKeyID = formKeyID, ImportState = nameof(PluginImportState.Current) }).Select(x => new MiscObjectDTO(x)).ToList();
    public void Save(MiscObjectDTO dto) { Database.Save(new MiscObject(dto)); }
}
