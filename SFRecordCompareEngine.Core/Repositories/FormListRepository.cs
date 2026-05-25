using Mutagen.Bethesda.Plugins;
using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Repositories.Interfaces;

namespace SFRecordCompareEngine.Core.Repositories;

public class FormListRepository : IFormListRepository
{
    public void UpsertFormList(IDatabase database, FormListDTO formList)
    {
        database.Execute(
            """
            INSERT INTO FormList (
                ModKey,
                FormID,
                AddToListFormKey,
                ImportedAtUtc
            )
            VALUES (@ModKey, @FormID, @AddToListFormKey, @ImportedAtUtc)
            ON CONFLICT(ModKey, FormID) DO UPDATE SET
                AddToListFormKey = excluded.AddToListFormKey,
                ImportedAtUtc = excluded.ImportedAtUtc;
            """,
            new
            {
                ModKey = formList.ModKey.FileName,
                formList.FormID,
                AddToListFormKey = DbValue(formList.AddToListFormKey),
                formList.ImportedAtUtc
            });
    }

    public void ReplaceItems(IDatabase database, ModKey modKey, string formId, IList<FormListItemDTO> items)
    {
        database.Execute(
            "DELETE FROM FormListItem WHERE ModKey = @ModKey COLLATE NOCASE AND FormID = @FormId;",
            new { ModKey = modKey.FileName, FormId = formId });

        foreach (var item in items)
        {
            database.Execute(
                """
                INSERT INTO FormListItem (
                    ModKey,
                    FormID,
                    ItemIndex,
                    ItemFormKey,
                    ImportedAtUtc
                )
                VALUES (@ModKey, @FormID, @ItemIndex, @ItemFormKey, @ImportedAtUtc);
                """,
                new { ModKey = item.ModKey.FileName, item.FormID, item.ItemIndex, item.ItemFormKey, item.ImportedAtUtc });
        }
    }

    private static object DbValue(object? value)
    {
        return value ?? DBNull.Value;
    }
}
