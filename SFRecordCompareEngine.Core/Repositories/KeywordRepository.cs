using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Repositories.Interfaces;

namespace SFRecordCompareEngine.Core.Repositories;

public class KeywordRepository : IKeywordRepository
{
    public void Upsert(IDatabase database, KeywordDTO keyword)
    {
        database.Execute(
            """
            INSERT INTO Keyword (
                ModKey,
                FormID,
                Name,
                Color,
                KeywordType,
                FNAM,
                ImportedAtUtc
            )
            VALUES (@0, @1, @2, @3, @4, @5, @6)
            ON CONFLICT(ModKey, FormID) DO UPDATE SET
                Name = excluded.Name,
                Color = excluded.Color,
                KeywordType = excluded.KeywordType,
                FNAM = excluded.FNAM,
                ImportedAtUtc = excluded.ImportedAtUtc;
            """,
            keyword.ModKey,
            keyword.FormID,
            DbValue(keyword.Name),
            DbValue(keyword.Color),
            DbValue(keyword.KeywordType),
            DbValue(keyword.FNAM),
            keyword.ImportedAtUtc);
    }

    private static object DbValue(object? value)
    {
        return value ?? DBNull.Value;
    }
}
