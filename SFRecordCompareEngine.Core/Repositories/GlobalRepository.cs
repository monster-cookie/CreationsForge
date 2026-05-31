using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Helpers;
using SFRecordCompareEngine.Core.Models.Database;
using SFRecordCompareEngine.Core.Repositories.Interfaces;

namespace SFRecordCompareEngine.Core.Repositories;

public class GlobalRepository : RecordHeaderRepository<Global, GlobalDTO>, IGlobalRepository
{
    public GlobalRepository(IDatabase database)
        : base(database, RecordTypeCatalog.Global.TableName)
    { }

    protected override GlobalDTO CreateDTO(Global model) => new(model);
    protected override Global CreateModel(GlobalDTO dto) => new(dto);
}
