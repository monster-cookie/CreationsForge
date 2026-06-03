using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Repositories.Interfaces;

public interface IKeywordRepository
{
    IList<KeywordDTO> GetByModKey(ModKey modKey);
    IList<KeywordDTO> GetByFormKey(FormKey formKey);
    void Save(KeywordDTO dto);
}