using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Services.Interfaces;

public interface IGlobalService
{
    IList<GlobalDTO> GetByModKey(ModKey modKey);
    IList<RecordTreeEntryDTO> GetRecordTreeEntriesByModKey(ModKey modKey);
    IList<GlobalDTO> GetByFormKey(FormKey formKey);
}
