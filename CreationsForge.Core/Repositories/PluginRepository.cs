using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Models.Database;
using CreationsForge.Core.Repositories.Interfaces;
using NPoco;

namespace CreationsForge.Core.Repositories;

public class PluginRepository : IPluginRepository
{
    private readonly IDatabase Database;

    public PluginRepository(IDatabase database)
    {
        Database = database;
    }

    public int CountByGame(SupportedGame game)
    {
        return Database.ExecuteScalar<int>(
            """
            SELECT COUNT(*)
            FROM Plugins
            WHERE Game = @Game;
            """,
            new
            {
                Game = game.ToString()
            });
    }

    public long GetImportedRecordCountByGame(SupportedGame game)
    {
        return Database.ExecuteScalar<long>(
            """
            SELECT COALESCE(SUM(RecordCount), 0)
            FROM Plugins
            WHERE Game = @Game
              AND ExistsOnDisk = 1
              AND ImportState = @ImportState;
            """,
            new
            {
                Game = game.ToString(),
                ImportState = nameof(PluginImportState.Current)
            });
    }

    public PluginDTO? GetByModKey(SupportedGame game, ModKeyDTO modKey)
    {
        var plugin = Database.FirstOrDefault<Plugin>(
            """
            SELECT *
            FROM Plugins
            WHERE Game = @Game
              AND ModKey_Name = @ModKeyName COLLATE NOCASE
              AND ModKey_Type = @ModKeyType
              AND ModKey_FileName = @ModKeyFileName COLLATE NOCASE;
            """,
            new
            {
                Game = game.ToString(),
                ModKeyName = modKey.Name,
                ModKeyType = modKey.Type,
                ModKeyFileName = modKey.FileName
            });

        return plugin?.ToDTO();
    }

    public IReadOnlyList<PluginDTO> GetOpenablePlugins(SupportedGame game)
    {
        return Database.Fetch<Plugin>(
                """
                SELECT *
                FROM Plugins
                WHERE Game = @Game
                  AND ExistsOnDisk = 1
                  AND ImportState IN (@CurrentImportState, @FailedImportState)
                ORDER BY LoadOrderIndex IS NULL, LoadOrderIndex;
                """,
                new
                {
                    Game = game.ToString(),
                    CurrentImportState = nameof(PluginImportState.Current),
                    FailedImportState = nameof(PluginImportState.Failed)
                })
            .Select(plugin => plugin.ToDTO())
            .ToList();
    }

    public IReadOnlyList<PluginDTO> SearchOpenablePluginsByFilename(SupportedGame game, string searchFilename)
    {
        var searchPattern = $"%{searchFilename}%";
        return Database.Fetch<Plugin>(
                """
                SELECT *
                FROM Plugins
                WHERE Game = @Game
                  AND ExistsOnDisk = 1
                  AND ImportState IN (@CurrentImportState, @FailedImportState)
                  AND ModKey_FileName LIKE @SearchPattern COLLATE NOCASE
                ORDER BY LoadOrderIndex IS NULL, LoadOrderIndex;
                """,
                new
                {
                    Game = game.ToString(),
                    CurrentImportState = nameof(PluginImportState.Current),
                    FailedImportState = nameof(PluginImportState.Failed),
                    SearchPattern = searchPattern
                })
            .Select(plugin => plugin.ToDTO())
            .ToList();
    }

    public void Save(PluginDTO dto)
    {
        var model = new Plugin(dto);
        Database.Save(model);
    }
}
