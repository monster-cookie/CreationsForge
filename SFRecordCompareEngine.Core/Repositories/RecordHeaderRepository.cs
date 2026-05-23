using NPoco;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Repositories.Interfaces;

namespace SFRecordCompareEngine.Core.Repositories;

public class RecordHeaderRepository : IRecordHeaderRepository
{
    public RecordHeaderDTO? GetCurrentByFormKey(IDatabase database, string formKey)
    {
        return database.FirstOrDefault<RecordHeaderDTO>(
            """
            SELECT rh.*
            FROM RecordHeader rh
            INNER JOIN Plugins p
                ON p.ModKey = rh.ModKey COLLATE NOCASE
            WHERE rh.FormKey = @0 COLLATE NOCASE
              AND p.ImportState = @1
              AND p.Enabled = 1
              AND p.ExistsOnDisk = 1
            ORDER BY
                p.LoadOrderIndex IS NULL,
                p.LoadOrderIndex DESC,
                rh.PluginFileName COLLATE NOCASE DESC;
            """,
            formKey,
            PluginImportState.Current.ToString());
    }

    public IList<RecordHeaderDTO> GetByHierarchy(IDatabase database, string selectedModKey, string? formId, string? editorId, string? recordType)
    {
        return database.Fetch<RecordHeaderDTO>(
            """
            SELECT rh.*
            FROM PluginResolutionHierarchy h
            INNER JOIN Plugins p
                ON p.ModKey = h.HierarchyModKey COLLATE NOCASE
            INNER JOIN RecordHeader rh
                ON rh.ModKey = h.HierarchyModKey COLLATE NOCASE
            WHERE h.ChildModKey = @0 COLLATE NOCASE
              AND p.ImportState = @1
              AND (@2 IS NULL OR rh.FormID = @2)
              AND (@3 IS NULL OR rh.EditorID = @3 COLLATE NOCASE)
              AND (@4 IS NULL OR rh.RecordType = @4)
            ORDER BY
                h.HierarchyLoadOrderIndex IS NULL,
                h.HierarchyLoadOrderIndex ASC,
                h.IsChild ASC,
                rh.FormID ASC;
            """,
            selectedModKey,
            PluginImportState.Current.ToString(),
            DbValue(formId),
            DbValue(editorId),
            DbValue(recordType));
    }

    public RecordHeaderDTO? GetWinningOverride(IDatabase database, string selectedModKey, string? formId, string? editorId, string? recordType)
    {
        return GetByHierarchy(database, selectedModKey, formId, editorId, recordType).LastOrDefault();
    }

    public void Upsert(IDatabase database, RecordHeaderDTO recordHeader)
    {
        database.Execute(
            """
            INSERT INTO RecordHeader (
                ModKey,
                FormID,
                RecordType,
                FormKey,
                EditorID,
                PluginFileName,
                FormVersion,
                StarfieldMajorRecordFlags,
                Version2,
                VersionControl,
                ImportedAtUtc
            )
            VALUES (@0, @1, @2, @3, @4, @5, @6, @7, @8, @9, @10)
            ON CONFLICT(ModKey, FormID) DO UPDATE SET
                RecordType = excluded.RecordType,
                FormKey = excluded.FormKey,
                EditorID = excluded.EditorID,
                PluginFileName = excluded.PluginFileName,
                FormVersion = excluded.FormVersion,
                StarfieldMajorRecordFlags = excluded.StarfieldMajorRecordFlags,
                Version2 = excluded.Version2,
                VersionControl = excluded.VersionControl,
                ImportedAtUtc = excluded.ImportedAtUtc;
            """,
            recordHeader.ModKey,
            recordHeader.FormID,
            recordHeader.RecordType,
            recordHeader.FormKey,
            DbValue(recordHeader.EditorID),
            recordHeader.PluginFileName,
            DbValue(recordHeader.FormVersion),
            DbValue(recordHeader.StarfieldMajorRecordFlags),
            DbValue(recordHeader.Version2),
            DbValue(recordHeader.VersionControl),
            recordHeader.ImportedAtUtc);
    }

    public void DeleteByModKey(IDatabase database, string modKey)
    {
        database.Execute(
            """
            DELETE FROM RecordHeader
            WHERE ModKey = @0 COLLATE NOCASE;
            """,
            modKey);
    }

    public void DeleteByModKeyAndRecordType(IDatabase database, string modKey, string recordType)
    {
        database.Execute(
            """
            DELETE FROM RecordHeader
            WHERE ModKey = @0 COLLATE NOCASE
              AND RecordType = @1;
            """,
            modKey,
            recordType);
    }

    private static object DbValue(object? value)
    {
        return value ?? DBNull.Value;
    }
}
