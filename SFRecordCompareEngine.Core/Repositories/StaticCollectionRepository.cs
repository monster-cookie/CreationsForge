using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Repositories.Interfaces;

namespace SFRecordCompareEngine.Core.Repositories;

public class StaticCollectionRepository : IStaticCollectionRepository
{
    public void Upsert(IDatabase database, StaticCollectionDTO staticCollection)
    {
        database.Execute(
            """
            INSERT INTO StaticCollection (ModKey, FormID, Name, ObjectBounds, Model, ImportedAtUtc)
            VALUES (@ModKey, @FormID, @Name, @ObjectBounds, @Model, @ImportedAtUtc)
            ON CONFLICT(ModKey, FormID) DO UPDATE SET
                Name = excluded.Name,
                ObjectBounds = excluded.ObjectBounds,
                Model = excluded.Model,
                ImportedAtUtc = excluded.ImportedAtUtc;
            """,
            new
            {
                staticCollection.ModKey,
                staticCollection.FormID,
                Name = DbValue(staticCollection.Name),
                ObjectBounds = DbValue(staticCollection.ObjectBounds),
                Model = DbValue(staticCollection.Model),
                staticCollection.ImportedAtUtc
            });
    }

    private static object DbValue(object? value)
    {
        return value ?? DBNull.Value;
    }
}
