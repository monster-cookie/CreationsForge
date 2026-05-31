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
    public MiscItemRepository(IDatabase database) { Database = database; }
    public IList<MiscItemDTO> GetByModKey(ModKey modKey) => Database.Fetch<MiscItem>("SELECT * FROM MiscItem WHERE ModKey_Name = @ModKeyName AND ModKey_Type = @ModKeyType AND ModKey_FileName = @ModKeyFileName COLLATE NOCASE ORDER BY FormKey_ID;", new { ModKeyName = modKey.Name, ModKeyType = (int)modKey.Type, ModKeyFileName = modKey.FileName }).Select(x => new MiscItemDTO(x)).ToList();
    public IList<MiscItemDTO> GetByFormKeyID(uint formKeyID) => Database.Fetch<MiscItem>("SELECT MiscItem.* FROM MiscItem INNER JOIN Plugins ON Plugins.ModKey_Name = MiscItem.ModKey_Name AND Plugins.ModKey_Type = MiscItem.ModKey_Type AND Plugins.ModKey_FileName = MiscItem.ModKey_FileName WHERE MiscItem.FormKey_ID = @FormKeyID AND Plugins.Enabled = 1 AND Plugins.ExistsOnDisk = 1 AND Plugins.ImportState = @ImportState ORDER BY Plugins.LoadOrderIndex;", new { FormKeyID = formKeyID, ImportState = nameof(PluginImportState.Current) }).Select(x => new MiscItemDTO(x)).ToList();
    public void Save(MiscItemDTO dto) { Database.Save(new MiscItem(dto)); }
}
