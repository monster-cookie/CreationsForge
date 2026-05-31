using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Helpers;
using SFRecordCompareEngine.Core.Models.Database;
using SFRecordCompareEngine.Core.Repositories.Interfaces;

namespace SFRecordCompareEngine.Core.Repositories;

public class NpcRepository : RecordHeaderRepository<Npc, NpcDTO>, INpcRepository
{
    public NpcRepository(IDatabase database)
        : base(database, RecordTypeCatalog.Npc.TableName)
    { }

    protected override NpcDTO CreateDTO(Npc model) => new(model);
    protected override Npc CreateModel(NpcDTO dto) => new(dto);
}
