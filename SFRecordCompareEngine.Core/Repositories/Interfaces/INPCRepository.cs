using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Repositories.Interfaces;

public interface INPCRepository
{
    IList<NPCDTO> GetByModKey(ModKey modKey);
    IList<RecordTreeEntryDTO> GetRecordTreeEntriesByModKey(ModKey modKey);
    IList<NPCDTO> GetByFormKey(FormKey formKey);
    void Save(NPCDTO dto);
}
