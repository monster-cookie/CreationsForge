using Mutagen.Bethesda.Plugins;
using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;
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

    /// <inheritdoc/>
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
    
    /// <inheritdoc/>
    public void Save(FormListDTO dto)
    {
        var model = new FormList(dto);
        Database.Save(model);
    }
}
