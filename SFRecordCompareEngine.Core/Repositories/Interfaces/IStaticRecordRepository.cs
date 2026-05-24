using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Repositories.Interfaces;

public interface IStaticRecordRepository
{
    void Upsert(IDatabase database, StaticRecordDTO staticRecord);
}
