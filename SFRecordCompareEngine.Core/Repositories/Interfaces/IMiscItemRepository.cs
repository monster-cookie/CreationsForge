using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Repositories.Interfaces;

public interface IMiscItemRepository
{
    IList<MiscItemDTO> GetByModKey(ModKey modKey);
    IList<MiscItemDTO> GetByFormKeyID(uint formKeyID);
    void Save(MiscItemDTO dto);
}
