using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Repositories.Interfaces;

namespace SFRecordCompareEngine.Core.Repositories;

public class MagicEffectRepository : IMagicEffectRepository
{
    public void Upsert(IDatabase database, MagicEffectDTO magicEffect)
    {
        database.Execute(
            """
            INSERT INTO MagicEffect (ModKey, FormID, Name, ImportedAtUtc)
            VALUES (@0, @1, @2, @3)
            ON CONFLICT(ModKey, FormID) DO UPDATE SET
                Name = excluded.Name,
                ImportedAtUtc = excluded.ImportedAtUtc;
            """,
            magicEffect.ModKey,
            magicEffect.FormID,
            DbValue(magicEffect.Name),
            magicEffect.ImportedAtUtc);
    }

    public void ReplaceKeywords(IDatabase database, string modKey, string formId, IList<RecordKeywordDTO> keywords)
    {
        ReplaceKeywordRows(database, "MagicEffectKeyword", modKey, formId, keywords);
    }

    private static void ReplaceKeywordRows(IDatabase database, string tableName, string modKey, string formId, IList<RecordKeywordDTO> keywords)
    {
        database.Execute(
            $"DELETE FROM {tableName} WHERE ModKey = @0 COLLATE NOCASE AND FormID = @1;",
            modKey,
            formId);

        foreach (var keyword in keywords)
        {
            database.Execute(
                $"""
                INSERT INTO {tableName} (ModKey, FormID, ItemIndex, KeywordFormKey, ImportedAtUtc)
                VALUES (@0, @1, @2, @3, @4);
                """,
                keyword.ModKey,
                keyword.FormID,
                keyword.ItemIndex,
                keyword.KeywordFormKey,
                keyword.ImportedAtUtc);
        }
    }

    private static object DbValue(object? value)
    {
        return value ?? DBNull.Value;
    }
}
