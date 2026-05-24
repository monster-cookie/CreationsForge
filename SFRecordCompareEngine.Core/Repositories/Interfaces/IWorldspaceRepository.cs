using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Repositories.Interfaces;

public interface IWorldspaceRepository
{
    void Upsert(IDatabase database, WorldspaceDTO worldspace);
}
