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
    public void Save(FormListDTO dto)
    {
        var model = new FormList(dto);
        Database.Save(model);
    }
}
