using System.Collections.Generic;
using System.Linq;
using Mutagen.Bethesda.Plugins;
using NPoco;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.Models.Database;
using SFRecordCompareEngine.Core.Repositories.Interfaces;

namespace SFRecordCompareEngine.Core.Repositories;

public class PluginMasterReferencesRepository : IPluginMasterReferencesRepository
{
    private readonly IDatabase Database;
    
    public PluginMasterReferencesRepository(IDatabase database)
    {
        Database = database;
    }
    
    public IList<PluginMasterReferenceDTO> GetMasterReferences(ModKey modKey)
    {
        return Database.Fetch<PluginMasterReference>(
                """
                SELECT *
                FROM PluginMasterReferences
                WHERE ModKey_Name = @ModKeyName AND ModKey_Type = @ModKeyType AND ModKey_FileName = @ModKeyFileName COLLATE NOCASE
                ORDER BY MasterReferenceIndex;
                """,
                new { ModKeyName = modKey.Name, ModKeyType = (int)modKey.Type, ModKeyFileName = modKey.FileName })
            .Select(pluginMasterReference => new PluginMasterReferenceDTO(pluginMasterReference))
            .ToList();
    }
}