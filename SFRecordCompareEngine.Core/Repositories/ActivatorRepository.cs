using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Repositories.Interfaces;

namespace SFRecordCompareEngine.Core.Repositories;

public class ActivatorRepository : IActivatorRepository
{
    public void Upsert(IDatabase database, ActivatorDTO activator)
    {
        database.Execute(
            """
            INSERT INTO Activator (ModKey, FormID, Name, ObjectBounds, Model, Destructible, ImportedAtUtc)
            VALUES (@0, @1, @2, @3, @4, @5, @6)
            ON CONFLICT(ModKey, FormID) DO UPDATE SET
                Name = excluded.Name,
                ObjectBounds = excluded.ObjectBounds,
                Model = excluded.Model,
                Destructible = excluded.Destructible,
                ImportedAtUtc = excluded.ImportedAtUtc;
            """,
            activator.ModKey,
            activator.FormID,
            DbValue(activator.Name),
            DbValue(activator.ObjectBounds),
            DbValue(activator.Model),
            DbValue(activator.Destructible),
            activator.ImportedAtUtc);
    }

    public void ReplaceKeywords(IDatabase database, string modKey, string formId, IList<RecordKeywordDTO> keywords)
    {
        ReplaceKeywordRows(database, "ActivatorKeyword", modKey, formId, keywords);
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
