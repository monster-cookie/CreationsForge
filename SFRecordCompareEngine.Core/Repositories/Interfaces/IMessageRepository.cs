using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Repositories.Interfaces;

public interface IMessageRepository
{
    void Upsert(IDatabase database, MessageDTO message);
}
