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
            WHERE rh.FormKey = @FormKey COLLATE NOCASE
              AND p.ImportState = @ImportState
              AND p.Enabled = 1
              AND p.ExistsOnDisk = 1
            ORDER BY
                p.LoadOrderIndex IS NULL,
                p.LoadOrderIndex DESC,
                rh.PluginFileName COLLATE NOCASE DESC;
            """,
            new
            {
                FormKey = formKey, 
                ImportState = nameof(PluginImportState.Current)
            });
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
            WHERE h.ChildModKey = @SelectedModKey COLLATE NOCASE
              AND p.ImportState = @ImportState
              AND (@FormId IS NULL OR rh.FormID = @FormId)
              AND (@EditorId IS NULL OR rh.EditorID = @EditorId COLLATE NOCASE)
              AND (@RecordType IS NULL OR rh.RecordType = @RecordType)
            ORDER BY
                h.HierarchyLoadOrderIndex IS NULL,
                h.HierarchyLoadOrderIndex ASC,
                h.IsChild ASC,
                rh.FormID ASC;
            """,
            new
            {
                SelectedModKey = selectedModKey,
                ImportState = nameof(PluginImportState.Current),
                FormId = DbValue(formId),
                EditorId = DbValue(editorId),
                RecordType = DbValue(recordType)
            });
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
            VALUES (@ModKey, @FormID, @RecordType, @FormKey, @EditorID, @PluginFileName, @FormVersion, @StarfieldMajorRecordFlags, @Version2, @VersionControl, @ImportedAtUtc)
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
            new
            {
                recordHeader.ModKey,
                recordHeader.FormID,
                recordHeader.RecordType,
                recordHeader.FormKey,
                EditorID = DbValue(recordHeader.EditorID),
                recordHeader.PluginFileName,
                FormVersion = DbValue(recordHeader.FormVersion),
                StarfieldMajorRecordFlags = DbValue(recordHeader.StarfieldMajorRecordFlags),
                Version2 = DbValue(recordHeader.Version2),
                VersionControl = DbValue(recordHeader.VersionControl),
                recordHeader.ImportedAtUtc
            });
    }

    public void DeleteByModKey(IDatabase database, string modKey)
    {
        database.Execute(
            """
            DELETE FROM RecordHeader
            WHERE ModKey = @ModKey COLLATE NOCASE;
            """,
            new { ModKey = modKey });
    }

    public void DeleteByModKeyAndRecordType(IDatabase database, string modKey, string recordType)
    {
        database.Execute(
            """
            DELETE FROM RecordHeader
            WHERE ModKey = @ModKey COLLATE NOCASE
              AND RecordType = @RecordType;
            """,
            new
            {
                ModKey = modKey, 
                RecordType = recordType
            });
    }

    private static object DbValue(object? value)
    {
        return value ?? DBNull.Value;
    }
}
