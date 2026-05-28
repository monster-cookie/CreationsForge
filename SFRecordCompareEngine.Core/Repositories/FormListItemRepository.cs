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
    
    /// <inheritdoc/>
    public void Save(FormListItemDTO dto)
    {
        var model = new FormListItem(dto);
        Database.Save(model);
    }
}