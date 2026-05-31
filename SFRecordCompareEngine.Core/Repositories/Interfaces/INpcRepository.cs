using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Repositories.Interfaces;

public interface INpcRepository
{
    IList<NpcDTO> GetByModKey(ModKey modKey);
    IList<NpcDTO> GetByFormKeyID(uint formKeyID);
    void Save(NpcDTO dto);
}
