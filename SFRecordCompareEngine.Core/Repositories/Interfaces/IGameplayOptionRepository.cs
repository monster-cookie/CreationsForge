using Mutagen.Bethesda.Plugins;
using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Repositories.Interfaces;

public interface IGameplayOptionRepository
{
    void Upsert(IDatabase database, GameplayOptionDTO gameplayOption);
    void ReplaceKeywords(IDatabase database, ModKey modKey, string formId, IList<RecordKeywordDTO> keywords);
}
