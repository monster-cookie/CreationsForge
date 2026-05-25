using Mutagen.Bethesda.Plugins;
using NPoco;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Repositories.Interfaces;

namespace SFRecordCompareEngine.Core.Repositories;

public class RecordHeaderRepository : IRecordHeaderRepository
{
    public RecordHeaderDTO? GetCurrentByFormKey(IDatabase database, string formKey)
    {
        var row = database.FirstOrDefault<RecordHeaderRow>(
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

        return row is null ? null : MapRecordHeader(row);
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
                ModKey = recordHeader.ModKey.FileName,
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

    public void DeleteByModKey(IDatabase database, ModKey modKey)
    {
        database.Execute(
            """
            DELETE FROM RecordHeader
            WHERE ModKey = @ModKey COLLATE NOCASE;
            """,
            new { ModKey = modKey.FileName });
    }

    private static object DbValue(object? value)
    {
        return value ?? DBNull.Value;
    }

    private static RecordHeaderDTO MapRecordHeader(RecordHeaderRow row)
    {
        return new RecordHeaderDTO
        {
            ModKey = ModKey.FromFileName(row.ModKey),
            FormID = row.FormID,
            RecordType = row.RecordType,
            FormKey = row.FormKey,
            EditorID = row.EditorID,
            PluginFileName = row.PluginFileName,
            FormVersion = row.FormVersion,
            StarfieldMajorRecordFlags = row.StarfieldMajorRecordFlags,
            Version2 = row.Version2,
            VersionControl = row.VersionControl,
            ImportedAtUtc = row.ImportedAtUtc
        };
    }

    private sealed class RecordHeaderRow
    {
        public string ModKey { get; set; } = string.Empty;
        public string FormID { get; set; } = string.Empty;
        public string RecordType { get; set; } = string.Empty;
        public string FormKey { get; set; } = string.Empty;
        public string? EditorID { get; set; }
        public string PluginFileName { get; set; } = string.Empty;
        public int? FormVersion { get; set; }
        public int? StarfieldMajorRecordFlags { get; set; }
        public int? Version2 { get; set; }
        public string? VersionControl { get; set; }
        public string ImportedAtUtc { get; set; } = string.Empty;
    }
}
