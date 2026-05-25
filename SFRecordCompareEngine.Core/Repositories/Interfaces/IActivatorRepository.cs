using Mutagen.Bethesda.Plugins;
using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Repositories.Interfaces;

public interface IActivatorRepository
{
    void Upsert(IDatabase database, ActivatorDTO activator);
    void ReplaceKeywords(IDatabase database, ModKey modKey, string formId, IList<RecordKeywordDTO> keywords);
}
