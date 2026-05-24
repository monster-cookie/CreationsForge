using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Repositories.Interfaces;

public interface IMagicEffectRepository
{
    void Upsert(IDatabase database, MagicEffectDTO magicEffect);
    void ReplaceKeywords(IDatabase database, string modKey, string formId, IList<RecordKeywordDTO> keywords);
}
