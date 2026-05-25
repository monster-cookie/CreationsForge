using Mutagen.Bethesda.Plugins;
using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Repositories.Interfaces;

public interface IMiscItemRepository
{
    void Upsert(IDatabase database, MiscItemDTO miscItem);
    void ReplaceKeywords(IDatabase database, ModKey modKey, string formId, IList<RecordKeywordDTO> keywords);
}
