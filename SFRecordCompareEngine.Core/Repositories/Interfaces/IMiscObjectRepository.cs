using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Repositories.Interfaces;

public interface IMiscItemRepository
{
    IList<MiscItemDTO> GetByModKey(ModKey modKey);
    IList<RecordTreeEntryDTO> GetRecordTreeEntriesByModKey(ModKey modKey);
    IList<MiscItemDTO> GetByFormKey(FormKey formKey);
    void Save(MiscItemDTO dto);
}
