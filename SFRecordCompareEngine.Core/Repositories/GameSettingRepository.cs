using Mutagen.Bethesda.Plugins;
using NPoco;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Helpers;
using SFRecordCompareEngine.Core.Repositories.Interfaces;
using SFRecordCompareEngine.Core.Services;

namespace SFRecordCompareEngine.Core.Repositories;

public class GameSettingRepository : IGameSettingRepository
{
    public void Upsert(IDatabase database, GameSettingDTO gameSetting)
    {
        database.Execute(
            """
            INSERT INTO GameSetting (
                ModKey,
                FormID,
                SettingType,
                TitleString,
                Data,
                RawData,
                XALG,
                IsCompressed,
                IsDeleted,
                ImportedAtUtc
            )
            VALUES (@ModKey, @FormID, @SettingType, @TitleString, @Data, @RawData, @XALG, @IsCompressed, @IsDeleted, @ImportedAtUtc)
            ON CONFLICT(ModKey, FormID) DO UPDATE SET
                SettingType = excluded.SettingType,
                TitleString = excluded.TitleString,
                Data = excluded.Data,
                RawData = excluded.RawData,
                XALG = excluded.XALG,
                IsCompressed = excluded.IsCompressed,
                IsDeleted = excluded.IsDeleted,
                ImportedAtUtc = excluded.ImportedAtUtc;
            """,
            new
            {
                ModKey = gameSetting.ModKey.FileName,
                gameSetting.FormID,
                SettingType = DbValue(gameSetting.SettingType),
                TitleString = DbValue(gameSetting.TitleString),
                Data = DbValue(gameSetting.Data),
                RawData = DbValue(gameSetting.RawData),
                XALG = DbValue(gameSetting.XALG),
                IsCompressed = DbValue(gameSetting.IsCompressed),
                IsDeleted = DbValue(gameSetting.IsDeleted),
                gameSetting.ImportedAtUtc
            });
    }

    public IList<RecordSummaryDTO> GetSummaries(IDatabase database, string modKey)
    {
        return database.Fetch<RecordSummaryDTO>(
            """
            SELECT
                rh.RecordType,
                rh.FormKey AS FormID,
                rh.EditorID
            FROM RecordHeader rh
            INNER JOIN GameSetting gs
                ON gs.ModKey = rh.ModKey COLLATE NOCASE
               AND gs.FormID = rh.FormID
            INNER JOIN Plugins p
                ON p.ModKey = rh.ModKey COLLATE NOCASE
            WHERE rh.ModKey = @ModKey COLLATE NOCASE
              AND rh.RecordType = @RecordType
              AND p.ImportState = @ImportState
              AND p.Enabled = 1
              AND p.ExistsOnDisk = 1
            ORDER BY rh.EditorID COLLATE NOCASE, rh.FormID;
            """,
            new
            {
                ModKey = modKey,
                RecordType = RecordTypeImportCatalog.GameSettingRecordType,
                ImportState = nameof(PluginImportState.Current)
            });
    }

    public IList<GameSettingComparisonRowDTO> GetByHierarchy(IDatabase database, ModKey selectedModKey, string formId)
    {
        return database.Fetch<GameSettingComparisonRow>(
            """
            SELECT
                h.HierarchyModKey AS ModKey,
                COALESCE(rh.FormID, @FormId) AS FormID,
                p.PluginFileName AS PluginName,
                h.HierarchyLoadOrderIndex,
                rh.FormKey,
                rh.EditorID,
                gs.SettingType,
                gs.TitleString,
                gs.Data,
                gs.RawData,
                gs.XALG,
                gs.IsCompressed,
                gs.IsDeleted,
                gs.ImportedAtUtc,
                CASE WHEN rh.FormID IS NULL THEN 0 ELSE 1 END AS HasRecord
            FROM PluginResolutionHierarchy h
            INNER JOIN Plugins p
                ON p.ModKey = h.HierarchyModKey COLLATE NOCASE
            LEFT JOIN RecordHeader rh
                ON rh.ModKey = h.HierarchyModKey COLLATE NOCASE
               AND rh.FormID = @FormId
               AND rh.RecordType = @RecordType
            LEFT JOIN GameSetting gs
                ON gs.ModKey = rh.ModKey COLLATE NOCASE
               AND gs.FormID = rh.FormID
            WHERE h.ChildModKey = @SelectedModKey COLLATE NOCASE
              AND p.ImportState = @ImportState
              AND p.Enabled = 1
              AND p.ExistsOnDisk = 1
            ORDER BY
                h.HierarchyLoadOrderIndex IS NULL,
                h.HierarchyLoadOrderIndex ASC,
                h.IsChild ASC;
            """,
            new
            {
                SelectedModKey = selectedModKey.FileName,
                FormId = formId,
                RecordType = RecordTypeImportCatalog.GameSettingRecordType,
                ImportState = nameof(PluginImportState.Current)
            })
            .Select(MapGameSettingComparisonRow)
            .ToList();
    }

    private static object DbValue(object? value)
    {
        return value ?? DBNull.Value;
    }

    private static GameSettingComparisonRowDTO MapGameSettingComparisonRow(GameSettingComparisonRow row)
    {
        return new GameSettingComparisonRowDTO
        {
            ModKey = ModKey.FromFileName(row.ModKey),
            FormID = row.FormID,
            PluginName = row.PluginName,
            HierarchyLoadOrderIndex = row.HierarchyLoadOrderIndex,
            FormKey = row.FormKey,
            EditorID = row.EditorID,
            SettingType = row.SettingType,
            TitleString = row.TitleString,
            Data = row.Data,
            RawData = row.RawData,
            XALG = row.XALG,
            IsCompressed = row.IsCompressed,
            IsDeleted = row.IsDeleted,
            ImportedAtUtc = row.ImportedAtUtc,
            HasRecord = row.HasRecord
        };
    }

    private sealed class GameSettingComparisonRow
    {
        public string ModKey { get; set; } = string.Empty;
        public string FormID { get; set; } = string.Empty;
        public string PluginName { get; set; } = string.Empty;
        public int? HierarchyLoadOrderIndex { get; set; }
        public string? FormKey { get; set; }
        public string? EditorID { get; set; }
        public string? SettingType { get; set; }
        public string? TitleString { get; set; }
        public string? Data { get; set; }
        public double? RawData { get; set; }
        public int? XALG { get; set; }
        public int? IsCompressed { get; set; }
        public int? IsDeleted { get; set; }
        public string ImportedAtUtc { get; set; } = string.Empty;
        public bool HasRecord { get; set; }
    }
}
