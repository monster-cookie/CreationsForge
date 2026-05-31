using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Helpers;
using SFRecordCompareEngine.Core.Models.Database;
using SFRecordCompareEngine.Core.Repositories.Interfaces;

namespace SFRecordCompareEngine.Core.Repositories;

public class PerkRepository : RecordHeaderRepository<Perk, PerkDTO>, IPerkRepository
{
    public PerkRepository(IDatabase database)
        : base(database, RecordTypeCatalog.Perk.TableName)
    { }

    protected override PerkDTO CreateDTO(Perk model) => new(model);
    protected override Perk CreateModel(PerkDTO dto) => new(dto);
}
