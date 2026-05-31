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
    public GlobalRepository(IDatabase database) { Database = database; }
    public IList<GlobalDTO> GetByModKey(ModKey modKey) => Database.Fetch<GlobalModel>("SELECT * FROM Global WHERE ModKey_Name = @ModKeyName AND ModKey_Type = @ModKeyType AND ModKey_FileName = @ModKeyFileName COLLATE NOCASE ORDER BY FormKey_ID;", new { ModKeyName = modKey.Name, ModKeyType = (int)modKey.Type, ModKeyFileName = modKey.FileName }).Select(x => new GlobalDTO(x)).ToList();
    public IList<GlobalDTO> GetByFormKeyID(uint formKeyID) => Database.Fetch<GlobalModel>("SELECT Global.* FROM Global INNER JOIN Plugins ON Plugins.ModKey_Name = Global.ModKey_Name AND Plugins.ModKey_Type = Global.ModKey_Type AND Plugins.ModKey_FileName = Global.ModKey_FileName WHERE Global.FormKey_ID = @FormKeyID AND Plugins.Enabled = 1 AND Plugins.ExistsOnDisk = 1 AND Plugins.ImportState = @ImportState ORDER BY Plugins.LoadOrderIndex;", new { FormKeyID = formKeyID, ImportState = nameof(PluginImportState.Current) }).Select(x => new GlobalDTO(x)).ToList();
    public void Save(GlobalDTO dto) { Database.Save(new GlobalModel(dto)); }
}
