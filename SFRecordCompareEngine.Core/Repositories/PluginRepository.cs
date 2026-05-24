using NPoco;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.Repositories.Interfaces;

namespace SFRecordCompareEngine.Core.Repositories;

public class PluginRepository : IPluginRepository
{
    public PluginMetadataDTO? GetByModKey(IDatabase database, string modKey)
    {
        return database.FirstOrDefault<PluginMetadataDTO>(
            "SELECT * FROM Plugins WHERE ModKey = @ModKey COLLATE NOCASE;",
            new { ModKey = modKey });
    }

    public IList<PluginMetadataDTO> GetAll(IDatabase database)
    {
        return database.Fetch<PluginMetadataDTO>("SELECT * FROM Plugins;");
    }

    public IList<PluginMetadataDTO> GetPlugins(IDatabase database)
    {
        return database.Fetch<PluginMetadataDTO>(
            """
            SELECT *
            FROM Plugins
            WHERE Enabled = 1
              AND ExistsOnDisk = 1
              AND ImportState = @ImportState
              AND ModKey <> @BaseGameModKey COLLATE NOCASE
            ORDER BY LoadOrderIndex IS NULL, LoadOrderIndex ASC, PluginFileName COLLATE NOCASE ASC;
            """,
            new { ImportState = PluginImportState.Current.ToString(), BaseGameModKey = "Starfield.esm" });
    }

    public IList<PluginMetadataDTO> GetOpenablePlugins(IDatabase database)
    {
        return database.Fetch<PluginMetadataDTO>(
            """
            SELECT *
            FROM Plugins
            WHERE ExistsOnDisk = 1
              AND ImportState IN (@CurrentImportState, @FailedImportState)
              AND ModKey <> @BaseGameModKey COLLATE NOCASE
            ORDER BY LoadOrderIndex IS NULL, LoadOrderIndex ASC, PluginFileName COLLATE NOCASE ASC;
            """,
            new
            {
                CurrentImportState = PluginImportState.Current.ToString(),
                FailedImportState = PluginImportState.Failed.ToString(),
                BaseGameModKey = "Starfield.esm"
            });
    }

    public IList<PluginMetadataDTO> SearchPlugins(IDatabase database, string searchText)
    {
        var searchPattern = $"%{searchText}%";
        return database.Fetch<PluginMetadataDTO>(
            """
            SELECT *
            FROM Plugins
            WHERE Enabled = 1
              AND ExistsOnDisk = 1
              AND ImportState = @ImportState
              AND ModKey <> @BaseGameModKey COLLATE NOCASE
              AND (PluginFileName LIKE @SearchPattern COLLATE NOCASE OR ModKey LIKE @SearchPattern COLLATE NOCASE)
            ORDER BY LoadOrderIndex IS NULL, LoadOrderIndex ASC, PluginFileName COLLATE NOCASE ASC;
            """,
            new
            {
                ImportState = PluginImportState.Current.ToString(),
                BaseGameModKey = "Starfield.esm",
                SearchPattern = searchPattern
            });
    }

    public IList<PluginMetadataDTO> SearchOpenablePlugins(IDatabase database, string searchText)
    {
        var searchPattern = $"%{searchText}%";
        return database.Fetch<PluginMetadataDTO>(
            """
            SELECT *
            FROM Plugins
            WHERE ExistsOnDisk = 1
              AND ImportState IN (@CurrentImportState, @FailedImportState)
              AND ModKey <> @BaseGameModKey COLLATE NOCASE
              AND (PluginFileName LIKE @SearchPattern COLLATE NOCASE OR ModKey LIKE @SearchPattern COLLATE NOCASE)
            ORDER BY LoadOrderIndex IS NULL, LoadOrderIndex ASC, PluginFileName COLLATE NOCASE ASC;
            """,
            new
            {
                CurrentImportState = PluginImportState.Current.ToString(),
                FailedImportState = PluginImportState.Failed.ToString(),
                BaseGameModKey = "Starfield.esm",
                SearchPattern = searchPattern
            });
    }

    public IList<PluginMasterReferenceDTO> GetMasterReferences(IDatabase database, string modKey)
    {
        return database.Fetch<PluginMasterReferenceDTO>(
            """
            SELECT *
            FROM PluginMasterReferences
            WHERE ModKey = @ModKey COLLATE NOCASE
            ORDER BY MasterReferenceIndex ASC;
            """,
            new { ModKey = modKey });
    }

    public IList<PluginResolutionHierarchyDTO> GetResolutionHierarchy(IDatabase database, string modKey)
    {
        return database.Fetch<PluginResolutionHierarchyDTO>(
            """
            SELECT *
            FROM PluginResolutionHierarchy
            WHERE ChildModKey = @ModKey COLLATE NOCASE
            ORDER BY
                HierarchyLoadOrderIndex IS NULL,
                HierarchyLoadOrderIndex ASC,
                IsChild ASC;
            """,
            new { ModKey = modKey });
    }

    public void UpsertPlugin(IDatabase database, PluginMetadataDTO plugin)
    {
        database.Execute(
            """
            INSERT INTO Plugins (
                ModKey,
                GameRelease,
                LoadOrderIndex,
                PluginFileName,
                PluginPath,
                Enabled,
                ExistsOnDisk,
                ImportState,
                HeaderFlags,
                FormVersion,
                Author,
                Branch,
                InteriorCellCount,
                SourceLastWriteUtcTicks,
                SourceFileSizeBytes,
                LastCheckedUtc,
                LastImportedUtc,
                InvalidatedAtUtc
            )
            VALUES (@ModKey, @GameRelease, @LoadOrderIndex, @PluginFileName, @PluginPath, @Enabled, @ExistsOnDisk, @ImportState, @HeaderFlags, @FormVersion, @Author, @Branch, @InteriorCellCount, @SourceLastWriteUtcTicks, @SourceFileSizeBytes, @LastCheckedUtc, @LastImportedUtc, @InvalidatedAtUtc)
            ON CONFLICT(ModKey) DO UPDATE SET
                GameRelease = excluded.GameRelease,
                LoadOrderIndex = excluded.LoadOrderIndex,
                PluginFileName = excluded.PluginFileName,
                PluginPath = excluded.PluginPath,
                Enabled = excluded.Enabled,
                ExistsOnDisk = excluded.ExistsOnDisk,
                ImportState = excluded.ImportState,
                HeaderFlags = excluded.HeaderFlags,
                FormVersion = excluded.FormVersion,
                Author = excluded.Author,
                Branch = excluded.Branch,
                InteriorCellCount = excluded.InteriorCellCount,
                SourceLastWriteUtcTicks = excluded.SourceLastWriteUtcTicks,
                SourceFileSizeBytes = excluded.SourceFileSizeBytes,
                LastCheckedUtc = excluded.LastCheckedUtc,
                LastImportedUtc = excluded.LastImportedUtc,
                InvalidatedAtUtc = excluded.InvalidatedAtUtc;
            """,
            new
            {
                plugin.ModKey,
                plugin.GameRelease,
                LoadOrderIndex = DbValue(plugin.LoadOrderIndex),
                plugin.PluginFileName,
                PluginPath = DbValue(plugin.PluginPath),
                Enabled = plugin.Enabled ? 1 : 0,
                ExistsOnDisk = plugin.ExistsOnDisk ? 1 : 0,
                plugin.ImportState,
                HeaderFlags = DbValue(plugin.HeaderFlags),
                FormVersion = DbValue(plugin.FormVersion),
                Author = DbValue(plugin.Author),
                Branch = DbValue(plugin.Branch),
                InteriorCellCount = DbValue(plugin.InteriorCellCount),
                SourceLastWriteUtcTicks = DbValue(plugin.SourceLastWriteUtcTicks),
                SourceFileSizeBytes = DbValue(plugin.SourceFileSizeBytes),
                plugin.LastCheckedUtc,
                LastImportedUtc = DbValue(plugin.LastImportedUtc),
                InvalidatedAtUtc = DbValue(plugin.InvalidatedAtUtc)
            });
    }

    public void UpsertMissingPlaceholder(IDatabase database, string modKey, string checkedAtUtc)
    {
        database.Execute(
            """
            INSERT INTO Plugins (
                ModKey,
                GameRelease,
                LoadOrderIndex,
                PluginFileName,
                Enabled,
                ExistsOnDisk,
                ImportState,
                LastCheckedUtc
            )
            VALUES (@ModKey, @GameRelease, NULL, @PluginFileName, 0, 0, @ImportState, @LastCheckedUtc)
            ON CONFLICT(ModKey) DO NOTHING;
            """,
            new
            {
                ModKey = modKey,
                GameRelease = "Starfield",
                PluginFileName = modKey,
                ImportState = PluginImportState.Missing.ToString(),
                LastCheckedUtc = checkedAtUtc
            });
    }

    public void ReplaceMasterReferences(IDatabase database, string modKey, IList<PluginMasterReferenceDTO> masterReferences)
    {
        database.Execute("DELETE FROM PluginMasterReferences WHERE ModKey = @ModKey COLLATE NOCASE;", new { ModKey = modKey });

        foreach (var masterReference in masterReferences)
        {
            database.Execute(
                """
                INSERT INTO PluginMasterReferences (
                    ModKey,
                    ParentModKey,
                    MasterReferenceIndex,
                    ParentLoadOrderIndex,
                    ImportedAtUtc
                )
                VALUES (@ModKey, @ParentModKey, @MasterReferenceIndex, @ParentLoadOrderIndex, @ImportedAtUtc);
                """,
                new
                {
                    masterReference.ModKey,
                    masterReference.ParentModKey,
                    masterReference.MasterReferenceIndex,
                    ParentLoadOrderIndex = DbValue(masterReference.ParentLoadOrderIndex),
                    masterReference.ImportedAtUtc
                });
        }
    }

    public void RefreshParentLoadOrderIndexes(IDatabase database)
    {
        database.Execute(
            """
            UPDATE PluginMasterReferences
            SET ParentLoadOrderIndex = (
                SELECT Plugins.LoadOrderIndex
                FROM Plugins
                WHERE Plugins.ModKey = PluginMasterReferences.ParentModKey COLLATE NOCASE
            );
            """);
    }

    public void MarkPluginsNotInLoadOrder(IDatabase database, ISet<string> currentModKeys, string checkedAtUtc)
    {
        var plugins = GetAll(database);
        var currentModKeysByCaseInsensitiveKey = currentModKeys.ToHashSet(StringComparer.OrdinalIgnoreCase);
        foreach (var plugin in plugins.Where(plugin => !currentModKeysByCaseInsensitiveKey.Contains(plugin.ModKey)))
        {
            database.Execute(
                """
                UPDATE Plugins
                SET Enabled = 0,
                    ExistsOnDisk = 0,
                    ImportState = @ImportState,
                    LastCheckedUtc = @LastCheckedUtc
                WHERE ModKey = @ModKey;
                """,
                new { ImportState = PluginImportState.Missing.ToString(), LastCheckedUtc = checkedAtUtc, plugin.ModKey });
        }
    }

    private static object DbValue(object? value)
    {
        return value ?? DBNull.Value;
    }
}
