using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Repositories.Interfaces;

namespace SFRecordCompareEngine.Core.Repositories;

public class RecordHeaderRepository : IRecordHeaderRepository
{
    public RecordHeaderDTO? GetByFormKey(IDatabase database, string formKey)
    {
        return database.FirstOrDefault<RecordHeaderDTO>(
            """
            SELECT *
            FROM RecordHeader
            WHERE FormKey = @0 COLLATE NOCASE;
            """,
            formKey);
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
