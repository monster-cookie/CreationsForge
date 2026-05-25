using Mutagen.Bethesda.Plugins;
using NPoco;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.Repositories.Interfaces;

namespace SFRecordCompareEngine.Core.Repositories;

public class PluginRepository : IPluginRepository
{
    public PluginMetadataDTO? GetByModKey(IDatabase database, ModKey modKey)
    {
        var row = database.FirstOrDefault<PluginMetadataRow>(
            "SELECT * FROM Plugins WHERE ModKey = @ModKey COLLATE NOCASE;",
            new { ModKey = modKey.FileName });

        return row is null ? null : MapPluginMetadata(row);
    }

    public IList<PluginMetadataDTO> GetAll(IDatabase database)
    {
        return database.Fetch<PluginMetadataRow>("SELECT * FROM Plugins;")
            .Select(MapPluginMetadata)
            .ToList();
    }

    public IList<PluginMetadataDTO> GetPlugins(IDatabase database)
    {
        return database.Fetch<PluginMetadataRow>(
            """
            SELECT *
            FROM Plugins
            WHERE Enabled = 1
              AND ExistsOnDisk = 1
              AND ImportState = @ImportState
              AND ModKey <> @BaseGameModKey COLLATE NOCASE
            ORDER BY LoadOrderIndex IS NULL, LoadOrderIndex ASC, PluginFileName COLLATE NOCASE ASC;
            """,
            new { ImportState = PluginImportState.Current.ToString(), BaseGameModKey = "Starfield.esm" })
            .Select(MapPluginMetadata)
            .ToList();
    }

    public IList<PluginMetadataDTO> GetOpenablePlugins(IDatabase database)
    {
        return database.Fetch<PluginMetadataRow>(
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
            })
            .Select(MapPluginMetadata)
            .ToList();
    }

    public IList<PluginMetadataDTO> SearchPlugins(IDatabase database, string searchText)
    {
        var searchPattern = $"%{searchText}%";
        return database.Fetch<PluginMetadataRow>(
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
            })
            .Select(MapPluginMetadata)
            .ToList();
    }

    public IList<PluginMetadataDTO> SearchOpenablePlugins(IDatabase database, string searchText)
    {
        var searchPattern = $"%{searchText}%";
        return database.Fetch<PluginMetadataRow>(
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
            })
            .Select(MapPluginMetadata)
            .ToList();
    }

    public IList<PluginMasterReferenceDTO> GetMasterReferences(IDatabase database, string modKey)
    {
        return database.Fetch<PluginMasterReferenceRow>(
            """
            SELECT *
            FROM PluginMasterReferences
            WHERE ModKey = @ModKey COLLATE NOCASE
            ORDER BY MasterReferenceIndex ASC;
            """,
            new { ModKey = modKey })
            .Select(MapPluginMasterReference)
            .ToList();
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
                ModKey = plugin.ModKey.FileName,
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

    public void ReplaceMasterReferences(IDatabase database, ModKey modKey, IList<PluginMasterReferenceDTO> masterReferences)
    {
        database.Execute("DELETE FROM PluginMasterReferences WHERE ModKey = @ModKey COLLATE NOCASE;", new { ModKey = modKey.FileName });

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
                    ModKey = masterReference.ModKey.FileName,
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

    public void MarkPluginsNotInLoadOrder(IDatabase database, HashSet<ModKey> currentModKeys, string checkedAtUtc)
    {
        var plugins = GetAll(database);
        var currentModKeysByCaseInsensitiveKey = currentModKeys.ToHashSet();
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
                new { ImportState = PluginImportState.Missing.ToString(), LastCheckedUtc = checkedAtUtc, ModKey = plugin.ModKey.FileName });
        }
    }

    private static PluginMetadataDTO MapPluginMetadata(PluginMetadataRow row)
    {
        return new PluginMetadataDTO
        {
            ModKey = ModKey.FromFileName(row.ModKey),
            GameRelease = row.GameRelease,
            LoadOrderIndex = row.LoadOrderIndex,
            PluginFileName = row.PluginFileName,
            PluginPath = row.PluginPath,
            Enabled = row.Enabled,
            ExistsOnDisk = row.ExistsOnDisk,
            ImportState = row.ImportState,
            HeaderFlags = row.HeaderFlags,
            FormVersion = row.FormVersion,
            Author = row.Author,
            Branch = row.Branch,
            InteriorCellCount = row.InteriorCellCount,
            SourceLastWriteUtcTicks = row.SourceLastWriteUtcTicks,
            SourceFileSizeBytes = row.SourceFileSizeBytes,
            LastCheckedUtc = row.LastCheckedUtc,
            LastImportedUtc = row.LastImportedUtc,
            InvalidatedAtUtc = row.InvalidatedAtUtc
        };
    }

    private static PluginMasterReferenceDTO MapPluginMasterReference(PluginMasterReferenceRow row)
    {
        return new PluginMasterReferenceDTO
        {
            ModKey = ModKey.FromFileName(row.ModKey),
            ParentModKey = row.ParentModKey,
            MasterReferenceIndex = row.MasterReferenceIndex,
            ParentLoadOrderIndex = row.ParentLoadOrderIndex,
            ImportedAtUtc = row.ImportedAtUtc
        };
    }

    private static object DbValue(object? value)
    {
        return value ?? DBNull.Value;
    }

    private sealed class PluginMetadataRow
    {
        public string ModKey { get; set; } = string.Empty;
        public string GameRelease { get; set; } = string.Empty;
        public int? LoadOrderIndex { get; set; }
        public string PluginFileName { get; set; } = string.Empty;
        public string? PluginPath { get; set; }
        public bool Enabled { get; set; } = true;
        public bool ExistsOnDisk { get; set; } = true;
        public string ImportState { get; set; } = PluginImportState.Current.ToString();
        public int? HeaderFlags { get; set; }
        public int? FormVersion { get; set; }
        public string? Author { get; set; }
        public string? Branch { get; set; }
        public int? InteriorCellCount { get; set; }
        public long? SourceLastWriteUtcTicks { get; set; }
        public long? SourceFileSizeBytes { get; set; }
        public string LastCheckedUtc { get; set; } = string.Empty;
        public string? LastImportedUtc { get; set; }
        public string? InvalidatedAtUtc { get; set; }
    }

    private sealed class PluginMasterReferenceRow
    {
        public string ModKey { get; set; } = string.Empty;
        public string ParentModKey { get; set; } = string.Empty;
        public int MasterReferenceIndex { get; set; }
        public int? ParentLoadOrderIndex { get; set; }
        public string ImportedAtUtc { get; set; } = string.Empty;
    }
}
