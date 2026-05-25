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
            VALUES (@ModKey, @FormID, @Name, @Color, @KeywordType, @FNAM, @ImportedAtUtc)
            ON CONFLICT(ModKey, FormID) DO UPDATE SET
                Name = excluded.Name,
                Color = excluded.Color,
                KeywordType = excluded.KeywordType,
                FNAM = excluded.FNAM,
                ImportedAtUtc = excluded.ImportedAtUtc;
            """,
            new
            {
                ModKey = keyword.ModKey.FileName,
                keyword.FormID,
                Name = DbValue(keyword.Name),
                Color = DbValue(keyword.Color),
                KeywordType = DbValue(keyword.KeywordType),
                FNAM = DbValue(keyword.FNAM),
                keyword.ImportedAtUtc
            });
    }

    private static object DbValue(object? value)
    {
        return value ?? DBNull.Value;
    }
}
