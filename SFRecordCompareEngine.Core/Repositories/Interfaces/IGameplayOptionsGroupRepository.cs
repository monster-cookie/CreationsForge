using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Repositories.Interfaces;

public interface IGameplayOptionsGroupRepository
{
    void Upsert(IDatabase database, GameplayOptionsGroupDTO gameplayOptionsGroup);
}
