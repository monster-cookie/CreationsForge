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
            VALUES (@ModKey, @FormID, @Name, @ObjectBounds, @Model, @Destructible, @ImportedAtUtc)
            ON CONFLICT(ModKey, FormID) DO UPDATE SET
                Name = excluded.Name,
                ObjectBounds = excluded.ObjectBounds,
                Model = excluded.Model,
                Destructible = excluded.Destructible,
                ImportedAtUtc = excluded.ImportedAtUtc;
            """,
            new
            {
                activator.ModKey,
                activator.FormID,
                Name = DbValue(activator.Name),
                ObjectBounds = DbValue(activator.ObjectBounds),
                Model = DbValue(activator.Model),
                Destructible = DbValue(activator.Destructible),
                activator.ImportedAtUtc
            });
    }

    public void ReplaceKeywords(IDatabase database, string modKey, string formId, IList<RecordKeywordDTO> keywords)
    {
        ReplaceKeywordRows(database, "ActivatorKeyword", modKey, formId, keywords);
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
