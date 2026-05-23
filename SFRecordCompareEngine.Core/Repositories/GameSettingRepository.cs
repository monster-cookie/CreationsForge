using NPoco;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
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
            VALUES (@0, @1, @2, @3, @4, @5, @6, @7, @8, @9)
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
            gameSetting.ModKey,
            gameSetting.FormID,
            DbValue(gameSetting.SettingType),
            DbValue(gameSetting.TitleString),
            DbValue(gameSetting.Data),
            DbValue(gameSetting.RawData),
            DbValue(gameSetting.XALG),
            DbValue(gameSetting.IsCompressed),
            DbValue(gameSetting.IsDeleted),
            gameSetting.ImportedAtUtc);
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
            WHERE rh.ModKey = @0 COLLATE NOCASE
              AND rh.RecordType = @1
              AND p.ImportState = @2
              AND p.Enabled = 1
              AND p.ExistsOnDisk = 1
            ORDER BY rh.EditorID COLLATE NOCASE, rh.FormID;
            """,
            modKey,
            RecordTypeImportCatalog.GameSettingRecordType,
            PluginImportState.Current.ToString());
    }

    public IList<GameSettingComparisonRowDTO> GetByHierarchy(IDatabase database, string selectedModKey, string formId)
    {
        return database.Fetch<GameSettingComparisonRowDTO>(
            """
            SELECT
                h.HierarchyModKey AS ModKey,
                COALESCE(rh.FormID, @1) AS FormID,
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
               AND rh.FormID = @1
               AND rh.RecordType = @2
            LEFT JOIN GameSetting gs
                ON gs.ModKey = rh.ModKey COLLATE NOCASE
               AND gs.FormID = rh.FormID
            WHERE h.ChildModKey = @0 COLLATE NOCASE
              AND p.ImportState = @3
              AND p.Enabled = 1
              AND p.ExistsOnDisk = 1
            ORDER BY
                h.HierarchyLoadOrderIndex IS NULL,
                h.HierarchyLoadOrderIndex ASC,
                h.IsChild ASC;
            """,
            selectedModKey,
            formId,
            RecordTypeImportCatalog.GameSettingRecordType,
            PluginImportState.Current.ToString());
    }

    private static object DbValue(object? value)
    {
        return value ?? DBNull.Value;
    }
}
