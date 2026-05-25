using Mutagen.Bethesda.Plugins;
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
                ModKey = miscItem.ModKey.FileName,
                miscItem.FormID,
                Name = DbValue(miscItem.Name),
                ObjectBounds = DbValue(miscItem.ObjectBounds),
                Model = DbValue(miscItem.Model),
                Destructible = DbValue(miscItem.Destructible),
                miscItem.ImportedAtUtc
            });
    }

    public void ReplaceKeywords(IDatabase database, ModKey modKey, string formId, IList<RecordKeywordDTO> keywords)
    {
        ReplaceKeywordRows(database, "MiscItemKeyword", modKey, formId, keywords);
    }

    private static void ReplaceKeywordRows(IDatabase database, string tableName, ModKey modKey, string formId, IList<RecordKeywordDTO> keywords)
    {
        database.Execute(
            $"DELETE FROM {tableName} WHERE ModKey = @ModKey COLLATE NOCASE AND FormID = @FormId;",
            new { ModKey = modKey.FileName, FormId = formId });

        foreach (var keyword in keywords)
        {
            database.Execute(
                $"""
                INSERT INTO {tableName} (ModKey, FormID, ItemIndex, KeywordFormKey, ImportedAtUtc)
                VALUES (@ModKey, @FormID, @ItemIndex, @KeywordFormKey, @ImportedAtUtc);
                """,
                new { ModKey = keyword.ModKey.FileName, keyword.FormID, keyword.ItemIndex, keyword.KeywordFormKey, keyword.ImportedAtUtc });
        }
    }

    private static object DbValue(object? value)
    {
        return value ?? DBNull.Value;
    }
}
