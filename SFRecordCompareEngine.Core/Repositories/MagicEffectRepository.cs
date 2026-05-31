using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Helpers;
using SFRecordCompareEngine.Core.Models.Database;
using SFRecordCompareEngine.Core.Repositories.Interfaces;

namespace SFRecordCompareEngine.Core.Repositories;

public class MagicEffectRepository : RecordHeaderRepository<MagicEffect, MagicEffectDTO>, IMagicEffectRepository
{
    public MagicEffectRepository(IDatabase database)
        : base(database, RecordTypeCatalog.MagicEffect.TableName)
    { }

    protected override MagicEffectDTO CreateDTO(MagicEffect model) => new(model);
    protected override MagicEffect CreateModel(MagicEffectDTO dto) => new(dto);
}
