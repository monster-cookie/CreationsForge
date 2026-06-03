using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Services.Interfaces;

public interface IKeywordService
{
    IList<KeywordDTO> GetByModKey(ModKey modKey);
    IList<KeywordDTO> GetByFormKey(FormKey formKey);
}