using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Repositories.Interfaces;

public interface IGameSettingRepository
{
    void Upsert(IDatabase database, GameSettingDTO gameSetting);
    IList<RecordSummaryDTO> GetSummaries(IDatabase database, string modKey);
    IList<GameSettingComparisonRowDTO> GetByHierarchy(IDatabase database, string selectedModKey, string formId);
}
