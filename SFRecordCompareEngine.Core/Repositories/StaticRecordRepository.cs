using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Repositories.Interfaces;

namespace SFRecordCompareEngine.Core.Repositories;

public class StaticRecordRepository : IStaticRecordRepository
{
    public void Upsert(IDatabase database, StaticRecordDTO staticRecord)
    {
        database.Execute(
            """
            INSERT INTO Static (ModKey, FormID, Name, ObjectBounds, Model, ImportedAtUtc)
            VALUES (@0, @1, @2, @3, @4, @5)
            ON CONFLICT(ModKey, FormID) DO UPDATE SET
                Name = excluded.Name,
                ObjectBounds = excluded.ObjectBounds,
                Model = excluded.Model,
                ImportedAtUtc = excluded.ImportedAtUtc;
            """,
            staticRecord.ModKey,
            staticRecord.FormID,
            DbValue(staticRecord.Name),
            DbValue(staticRecord.ObjectBounds),
            DbValue(staticRecord.Model),
            staticRecord.ImportedAtUtc);
    }

    private static object DbValue(object? value)
    {
        return value ?? DBNull.Value;
    }
}
