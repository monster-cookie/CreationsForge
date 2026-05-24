using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Repositories.Interfaces;

namespace SFRecordCompareEngine.Core.Repositories;

public class MiscItemRepository : IMiscItemRepository
{
    public void Upsert(IDatabase database, MiscItemDTO miscItem)
    {
        database.Execute(
            """
            INSERT INTO MiscItem (ModKey, FormID, Name, ObjectBounds, Model, Destructible, ImportedAtUtc)
            VALUES (@0, @1, @2, @3, @4, @5, @6)
            ON CONFLICT(ModKey, FormID) DO UPDATE SET
                Name = excluded.Name,
                ObjectBounds = excluded.ObjectBounds,
                Model = excluded.Model,
                Destructible = excluded.Destructible,
                ImportedAtUtc = excluded.ImportedAtUtc;
            """,
            miscItem.ModKey,
            miscItem.FormID,
            DbValue(miscItem.Name),
            DbValue(miscItem.ObjectBounds),
            DbValue(miscItem.Model),
            DbValue(miscItem.Destructible),
            miscItem.ImportedAtUtc);
    }

    public void ReplaceKeywords(IDatabase database, string modKey, string formId, IList<RecordKeywordDTO> keywords)
    {
        ReplaceKeywordRows(database, "MiscItemKeyword", modKey, formId, keywords);
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
