using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Services.Interfaces;

public interface INpcService
{
    IList<NpcDTO> GetByModKey(ModKey modKey);
    IList<NpcDTO> GetByFormKeyID(uint formKeyID);
}
