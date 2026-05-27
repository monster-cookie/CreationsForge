using Mutagen.Bethesda.Plugins;
using NPoco;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.Enums;
using SFRecordCompareEngine.Core.Models.Database;
using SFRecordCompareEngine.Core.Repositories.Interfaces;

namespace SFRecordCompareEngine.Core.Repositories;

public class PluginRepository : IPluginRepository
{
    private readonly IDatabase Database;
    
    public PluginRepository(IDatabase database)
    {
        Database = database;
    }
    
    /// <inheritdoc />
    public PluginDTO? GetByModKey(ModKey modKey)
    {
        var plugin = Database.FirstOrDefault<Plugin>(
            "SELECT * FROM Plugins WHERE ModKey_Name = @ModKeyName AND ModKey_Type = @ModKeyType AND ModKey_FileName = @ModKeyFileName COLLATE NOCASE;", 
            new { ModKeyName = modKey.FileName, ModKeyType = (int)modKey.Type, ModKeyFileName = modKey.FileName });
        return plugin == null ? null : new PluginDTO(plugin);
    }

    /// <inheritdoc />
    public IList<PluginDTO> GetAll()
    {
        return Database.Fetch<Plugin>("SELECT * FROM Plugins;")
            .Select(plugin => new PluginDTO(plugin))
            .ToList();
    }

    /// <inheritdoc />
    public IList<PluginDTO> GetImportedPlugins()
    {
        return Database.Fetch<Plugin>(
            """
            SELECT *
            FROM Plugins
            WHERE Enabled = 1
              AND ExistsOnDisk = 1
              AND ImportState = @ImportState
            ORDER BY LoadOrderIndex IS NULL, LoadOrderIndex;
            """,
            new { ImportState = nameof(PluginImportState.Current) })
            .Select(plugin => new PluginDTO(plugin))
            .ToList();
    }

    /// <inheritdoc />
    public IList<PluginDTO> GetOpenablePlugins()
    {
        return Database.Fetch<Plugin>(
            """
            SELECT *
            FROM Plugins
            WHERE ExistsOnDisk = 1
              AND ImportState IN (@CurrentImportState, @FailedImportState)
            ORDER BY LoadOrderIndex IS NULL, LoadOrderIndex;
            """,
            new
            {
                CurrentImportState = nameof(PluginImportState.Current),
                FailedImportState = nameof(PluginImportState.Failed),
            })
            .Select(plugin => new PluginDTO(plugin))
            .ToList();
    }

    /// <inheritdoc />
    public IList<PluginDTO> SearchPluginsByFilename(string searchFilename)
    {
        var searchPattern = $"%{searchFilename}%";
        return Database.Fetch<Plugin>(
            """
            SELECT *
            FROM Plugins
            WHERE Enabled = 1
              AND ExistsOnDisk = 1
              AND ImportState = @ImportState
              AND (PluginFileName LIKE @SearchPattern COLLATE NOCASE OR ModKey LIKE @SearchPattern COLLATE NOCASE)
            ORDER BY LoadOrderIndex IS NULL, LoadOrderIndex;
            """,
            new
            {
                ImportState = nameof(PluginImportState.Current),
                SearchPattern = searchPattern
            })
            .Select(plugin => new PluginDTO(plugin))
            .ToList();
    }

    /// <inheritdoc />
    public IList<PluginDTO> SearchOpenablePluginsByFilename(string searchFilename)
    {
        var searchPattern = $"%{searchFilename}%";
        return Database.Fetch<Plugin>(
            """
            SELECT *
            FROM Plugins
            WHERE ExistsOnDisk = 1
              AND ImportState IN (@CurrentImportState, @FailedImportState)
              AND (PluginFileName LIKE @SearchPattern COLLATE NOCASE OR ModKey LIKE @SearchPattern COLLATE NOCASE)
            ORDER BY LoadOrderIndex IS NULL, LoadOrderIndex;
            """,
            new
            {
                CurrentImportState = nameof(PluginImportState.Current),
                FailedImportState = nameof(PluginImportState.Failed),
                SearchPattern = searchPattern
            })
            .Select(plugin => new PluginDTO(plugin))
            .ToList();
    }

    /// <inheritdoc />
    public void UpsertPlugin(PluginDTO dto)
    {
        var model = new Plugin(dto);
        Database.Save(model);
    }
}
