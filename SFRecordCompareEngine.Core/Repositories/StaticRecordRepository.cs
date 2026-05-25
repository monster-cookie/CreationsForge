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
            VALUES (@ModKey, @FormID, @Name, @ObjectBounds, @Model, @ImportedAtUtc)
            ON CONFLICT(ModKey, FormID) DO UPDATE SET
                Name = excluded.Name,
                ObjectBounds = excluded.ObjectBounds,
                Model = excluded.Model,
                ImportedAtUtc = excluded.ImportedAtUtc;
            """,
            new
            {
                ModKey = staticRecord.ModKey.FileName,
                staticRecord.FormID,
                Name = DbValue(staticRecord.Name),
                ObjectBounds = DbValue(staticRecord.ObjectBounds),
                Model = DbValue(staticRecord.Model),
                staticRecord.ImportedAtUtc
            });
    }

    private static object DbValue(object? value)
    {
        return value ?? DBNull.Value;
    }
}
