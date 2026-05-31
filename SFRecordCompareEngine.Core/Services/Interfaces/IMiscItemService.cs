using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Services.Interfaces;

public interface IMiscItemService
{
    IList<MiscItemDTO> GetByModKey(ModKey modKey);
    IList<MiscItemDTO> GetByFormKeyID(uint formKeyID);
}
