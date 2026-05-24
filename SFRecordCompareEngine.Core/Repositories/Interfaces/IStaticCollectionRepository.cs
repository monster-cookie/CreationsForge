using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Repositories.Interfaces;

public interface IStaticCollectionRepository
{
    void Upsert(IDatabase database, StaticCollectionDTO staticCollection);
}
