using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Helpers;
using SFRecordCompareEngine.Core.Models.Database;
using SFRecordCompareEngine.Core.Repositories.Interfaces;

namespace SFRecordCompareEngine.Core.Repositories;

public class MiscItemRepository : RecordHeaderRepository<MiscItem, MiscItemDTO>, IMiscItemRepository
{
    public MiscItemRepository(IDatabase database)
        : base(database, RecordTypeCatalog.MiscItem.TableName)
    { }

    protected override MiscItemDTO CreateDTO(MiscItem model) => new(model);
    protected override MiscItem CreateModel(MiscItemDTO dto) => new(dto);
}
