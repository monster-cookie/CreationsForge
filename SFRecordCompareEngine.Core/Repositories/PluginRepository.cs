using NPoco;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.Repositories.Interfaces;

namespace SFRecordCompareEngine.Core.Repositories;

public class PluginRepository : IPluginRepository
{
    public PluginMetadataDTO? GetByModKey(IDatabase database, string modKey)
    {
        return database.FirstOrDefault<PluginMetadataDTO>(
            "SELECT * FROM Plugins WHERE ModKey = @0 COLLATE NOCASE;",
            modKey);
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
              AND ImportState = @0
              AND ModKey <> @1 COLLATE NOCASE
            ORDER BY LoadOrderIndex IS NULL, LoadOrderIndex ASC, PluginFileName COLLATE NOCASE ASC;
            """,
            PluginImportState.Current.ToString(),
            "Starfield.esm");
    }

    public IList<PluginMetadataDTO> GetOpenablePlugins(IDatabase database)
    {
        return database.Fetch<PluginMetadataDTO>(
            """
            SELECT *
            FROM Plugins
            WHERE ExistsOnDisk = 1
              AND ImportState IN (@0, @1)
              AND ModKey <> @2 COLLATE NOCASE
            ORDER BY LoadOrderIndex IS NULL, LoadOrderIndex ASC, PluginFileName COLLATE NOCASE ASC;
            """,
            PluginImportState.Current.ToString(),
            PluginImportState.Failed.ToString(),
            "Starfield.esm");
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
              AND ImportState = @0
              AND ModKey <> @1 COLLATE NOCASE
              AND (PluginFileName LIKE @2 COLLATE NOCASE OR ModKey LIKE @2 COLLATE NOCASE)
            ORDER BY LoadOrderIndex IS NULL, LoadOrderIndex ASC, PluginFileName COLLATE NOCASE ASC;
            """,
            PluginImportState.Current.ToString(),
            "Starfield.esm",
            searchPattern);
    }

    public IList<PluginMetadataDTO> SearchOpenablePlugins(IDatabase database, string searchText)
    {
        var searchPattern = $"%{searchText}%";
        return database.Fetch<PluginMetadataDTO>(
            """
            SELECT *
            FROM Plugins
            WHERE ExistsOnDisk = 1
              AND ImportState IN (@0, @1)
              AND ModKey <> @2 COLLATE NOCASE
              AND (PluginFileName LIKE @3 COLLATE NOCASE OR ModKey LIKE @3 COLLATE NOCASE)
            ORDER BY LoadOrderIndex IS NULL, LoadOrderIndex ASC, PluginFileName COLLATE NOCASE ASC;
            """,
            PluginImportState.Current.ToString(),
            PluginImportState.Failed.ToString(),
            "Starfield.esm",
            searchPattern);
    }

    public IList<PluginMasterReferenceDTO> GetMasterReferences(IDatabase database, string modKey)
    {
        return database.Fetch<PluginMasterReferenceDTO>(
            """
            SELECT *
            FROM PluginMasterReferences
            WHERE ModKey = @0 COLLATE NOCASE
            ORDER BY MasterReferenceIndex ASC;
            """,
            modKey);
    }

    public IList<PluginResolutionHierarchyDTO> GetResolutionHierarchy(IDatabase database, string modKey)
    {
        return database.Fetch<PluginResolutionHierarchyDTO>(
            """
            SELECT *
            FROM PluginResolutionHierarchy
            WHERE ChildModKey = @0 COLLATE NOCASE
            ORDER BY
                HierarchyLoadOrderIndex IS NULL,
                HierarchyLoadOrderIndex ASC,
                IsChild ASC;
            """,
            modKey);
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
            VALUES (@0, @1, @2, @3, @4, @5, @6, @7, @8, @9, @10, @11, @12, @13, @14, @15, @16, @17)
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
            plugin.ModKey,
            plugin.GameRelease,
            DbValue(plugin.LoadOrderIndex),
            plugin.PluginFileName,
            DbValue(plugin.PluginPath),
            plugin.Enabled ? 1 : 0,
            plugin.ExistsOnDisk ? 1 : 0,
            plugin.ImportState,
            DbValue(plugin.HeaderFlags),
            DbValue(plugin.FormVersion),
            DbValue(plugin.Author),
            DbValue(plugin.Branch),
            DbValue(plugin.InteriorCellCount),
            DbValue(plugin.SourceLastWriteUtcTicks),
            DbValue(plugin.SourceFileSizeBytes),
            plugin.LastCheckedUtc,
            DbValue(plugin.LastImportedUtc),
            DbValue(plugin.InvalidatedAtUtc));
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
            VALUES (@0, @1, NULL, @2, 0, 0, @3, @4)
            ON CONFLICT(ModKey) DO NOTHING;
            """,
            modKey,
            "Starfield",
            modKey,
            PluginImportState.Missing.ToString(),
            checkedAtUtc);
    }

    public void ReplaceMasterReferences(IDatabase database, string modKey, IList<PluginMasterReferenceDTO> masterReferences)
    {
        database.Execute("DELETE FROM PluginMasterReferences WHERE ModKey = @0 COLLATE NOCASE;", modKey);

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
                VALUES (@0, @1, @2, @3, @4);
                """,
                masterReference.ModKey,
                masterReference.ParentModKey,
                masterReference.MasterReferenceIndex,
                DbValue(masterReference.ParentLoadOrderIndex),
                masterReference.ImportedAtUtc);
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
                    ImportState = @0,
                    LastCheckedUtc = @1
                WHERE ModKey = @2;
                """,
                PluginImportState.Missing.ToString(),
                checkedAtUtc,
                plugin.ModKey);
        }
    }

    private static object DbValue(object? value)
    {
        return value ?? DBNull.Value;
    }
}
