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
            VALUES (@ModKey, @FormID, @Name, @ImportedAtUtc)
            ON CONFLICT(ModKey, FormID) DO UPDATE SET
                Name = excluded.Name,
                ImportedAtUtc = excluded.ImportedAtUtc;
            """,
            new
            {
                magicEffect.ModKey,
                magicEffect.FormID,
                Name = DbValue(magicEffect.Name),
                magicEffect.ImportedAtUtc
            });
    }

    public void ReplaceKeywords(IDatabase database, string modKey, string formId, IList<RecordKeywordDTO> keywords)
    {
        ReplaceKeywordRows(database, "MagicEffectKeyword", modKey, formId, keywords);
    }

    private static void ReplaceKeywordRows(IDatabase database, string tableName, string modKey, string formId, IList<RecordKeywordDTO> keywords)
    {
        database.Execute(
            $"DELETE FROM {tableName} WHERE ModKey = @ModKey COLLATE NOCASE AND FormID = @FormId;",
            new { ModKey = modKey, FormId = formId });

        foreach (var keyword in keywords)
        {
            database.Execute(
                $"""
                INSERT INTO {tableName} (ModKey, FormID, ItemIndex, KeywordFormKey, ImportedAtUtc)
                VALUES (@ModKey, @FormID, @ItemIndex, @KeywordFormKey, @ImportedAtUtc);
                """,
                new { keyword.ModKey, keyword.FormID, keyword.ItemIndex, keyword.KeywordFormKey, keyword.ImportedAtUtc });
        }
    }

    private static object DbValue(object? value)
    {
        return value ?? DBNull.Value;
    }
}
