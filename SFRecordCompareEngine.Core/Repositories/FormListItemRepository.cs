using Mutagen.Bethesda.Plugins;
using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Models.Database;
using SFRecordCompareEngine.Core.Repositories.Interfaces;

namespace SFRecordCompareEngine.Core.Repositories;

public class FormListItemRepository : IFormListItemRepository
{
    private readonly IDatabase Database;

    public FormListItemRepository(IDatabase database)
    {
        Database = database;
    }

    /// <inheritdoc />
    public IList<FormListItemDTO> GetByFormList(ModKey modKey, FormKey formKey)
    {
        return Database.Fetch<FormListItem>(
                """
                SELECT *
                FROM FormListItems
                WHERE ModKey_Name = @ModKeyName AND ModKey_Type = @ModKeyType AND ModKey_FileName = @ModKeyFileName COLLATE NOCASE
                  AND FormKey_ModKey_Name = @FormKeyModKeyName
                  AND FormKey_ModKey_Type = @FormKeyModKeyType
                  AND FormKey_ModKey_FileName = @FormKeyModKeyFileName
                  AND FormKey_ID = @FormKeyID
                ORDER BY Item_Index;
                """,
                new { ModKeyName = modKey.Name, ModKeyType = (int)modKey.Type, ModKeyFileName = modKey.FileName, FormKeyModKeyName = formKey.ModKey.Name, FormKeyModKeyType = (int)formKey.ModKey.Type, FormKeyModKeyFileName = formKey.ModKey.FileName, FormKeyID = formKey.ID })
            .Select(formListItem => new FormListItemDTO(formListItem))
            .ToList();
    }

    /// <inheritdoc />
    public void DeleteByFormList(ModKey modKey, FormKey formKey)
    {
        Database.Delete<FormListItem>(
            """
            WHERE ModKey_Name = @ModKeyName AND ModKey_Type = @ModKeyType AND ModKey_FileName = @ModKeyFileName COLLATE NOCASE
              AND FormKey_ModKey_Name = @FormKeyModKeyName
              AND FormKey_ModKey_Type = @FormKeyModKeyType
              AND FormKey_ModKey_FileName = @FormKeyModKeyFileName
              AND FormKey_ID = @FormKeyID
            """,
            new { ModKeyName = modKey.Name, ModKeyType = (int)modKey.Type, ModKeyFileName = modKey.FileName, FormKeyModKeyName = formKey.ModKey.Name, FormKeyModKeyType = (int)formKey.ModKey.Type, FormKeyModKeyFileName = formKey.ModKey.FileName, FormKeyID = formKey.ID });
    }

    /// <inheritdoc />
    public void Save(FormListItemDTO dto)
    {
        var model = new FormListItem(dto);
        Database.Save(model);
    }
}
