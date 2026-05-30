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
                SELECT pluginMasterReferences.*
                FROM PluginMasterReferences pluginMasterReferences
                INNER JOIN Plugins masterPlugin
                    ON masterPlugin.ModKey_Name = pluginMasterReferences.Master_ModKey_Name
                    AND masterPlugin.ModKey_Type = pluginMasterReferences.Master_ModKey_Type
                    AND masterPlugin.ModKey_FileName = pluginMasterReferences.Master_ModKey_FileName
                WHERE pluginMasterReferences.Plugin_ModKey_Name = @ModKeyName AND pluginMasterReferences.Plugin_ModKey_Type = @ModKeyType AND pluginMasterReferences.Plugin_ModKey_FileName = @ModKeyFileName COLLATE NOCASE
                ORDER BY masterPlugin.LoadOrderIndex;
                """,
                new { ModKeyName = modKey.Name, ModKeyType = (int)modKey.Type, ModKeyFileName = modKey.FileName })
            .Select(pluginMasterReference => new PluginMasterReferenceDTO(pluginMasterReference))
            .ToList();
    }

    /// <inheritdoc />
    public void Save(PluginMasterReferenceDTO dto)
    {
        var model = new PluginMasterReference(dto);
        Database.Save(model);
    }
}