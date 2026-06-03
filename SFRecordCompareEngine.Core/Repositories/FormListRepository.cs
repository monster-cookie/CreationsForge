using Mutagen.Bethesda.Plugins;
using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Enums;
using SFRecordCompareEngine.Core.Models.Database;
using SFRecordCompareEngine.Core.Repositories.Interfaces;

namespace SFRecordCompareEngine.Core.Repositories;

public class FormListRepository : IFormListRepository
{
    private readonly IDatabase Database;

    public FormListRepository(IDatabase database)
    {
        Database = database;
    }

    /// <inheritdoc />
    public IList<FormListDTO> GetByModKey(ModKey modKey)
    {
        return Database.Fetch<FormList>(
                """
                SELECT *
                FROM FormList
                WHERE ModKey_Name = @ModKeyName AND ModKey_Type = @ModKeyType AND ModKey_FileName = @ModKeyFileName COLLATE NOCASE
                ORDER BY FormKey_ID;
                """,
                new { ModKeyName = modKey.Name, ModKeyType = (int)modKey.Type, ModKeyFileName = modKey.FileName })
            .Select(formList => new FormListDTO(formList))
            .ToList();
    }

    /// <inheritdoc />
    public IList<FormListDTO> GetByFormKey(FormKey formKey)
    {
        return Database.Fetch<FormList>(
                """
                SELECT FormList.*
                FROM FormList
                INNER JOIN Plugins
                    ON Plugins.ModKey_Name = FormList.ModKey_Name
                    AND Plugins.ModKey_Type = FormList.ModKey_Type
                    AND Plugins.ModKey_FileName = FormList.ModKey_FileName
                WHERE FormList.FormKey_ModKey_Name = @FormKeyModKeyName AND FormList.FormKey_ModKey_Type = @FormKeyModKeyType AND FormList.FormKey_ModKey_FileName = @FormKeyModKeyFileName AND FormList.FormKey_ID = @FormKeyID
                  AND Plugins.Enabled = 1
                  AND Plugins.ExistsOnDisk = 1
                  AND Plugins.ImportState = @ImportState
                ORDER BY Plugins.LoadOrderIndex;
                """,
                new { FormKeyModKeyName = formKey.ModKey.Name, FormKeyModKeyType = (int)formKey.ModKey.Type, FormKeyModKeyFileName = formKey.ModKey.FileName, FormKeyID = formKey.ID, ImportState = nameof(PluginImportState.Current) })
            .Select(formList => new FormListDTO(formList))
            .ToList();
    }

    /// <inheritdoc />
    public void Save(FormListDTO dto)
    {
        var model = new FormList(dto);
        Database.Save(model);
    }
}