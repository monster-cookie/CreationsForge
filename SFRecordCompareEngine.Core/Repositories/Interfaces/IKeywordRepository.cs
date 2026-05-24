using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Repositories.Interfaces;

public interface IKeywordRepository
{
    void Upsert(IDatabase database, KeywordDTO keyword);
}
