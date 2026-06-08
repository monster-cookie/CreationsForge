using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Models.Database;
using CreationsForge.Core.Repositories.Interfaces;
using NPoco;

namespace CreationsForge.Core.Repositories;

public class PluginMasterReferenceRepository : IPluginMasterReferenceRepository
{
    private readonly IDatabase Database;

    public PluginMasterReferenceRepository(IDatabase database)
    {
        Database = database;
    }

    public void Save(PluginMasterReferenceDTO dto)
    {
        var model = new PluginMasterReference(dto);
        Database.Save(model);
    }

    public void DeleteStaleByPlugin(SupportedGame game, ModKeyDTO pluginModKey, DateTime importedAtUTC)
    {
        Database.Execute(
            """
            DELETE FROM PluginMasterReferences
            WHERE Game = @Game
              AND Plugin_ModKey_Name = @PluginModKeyName
              AND Plugin_ModKey_Type = @PluginModKeyType
              AND Plugin_ModKey_FileName = @PluginModKeyFileName
              AND ImportedAtUTC <> @ImportedAtUTC;
            """,
            new
            {
                Game = game.ToString(),
                PluginModKeyName = pluginModKey.Name,
                PluginModKeyType = pluginModKey.Type,
                PluginModKeyFileName = pluginModKey.FileName,
                ImportedAtUTC = importedAtUTC
            });
    }
}
