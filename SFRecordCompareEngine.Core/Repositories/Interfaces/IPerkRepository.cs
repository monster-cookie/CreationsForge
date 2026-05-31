using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Repositories.Interfaces;

public interface IPerkRepository
{
    IList<PerkDTO> GetByModKey(ModKey modKey);
    IList<PerkDTO> GetByFormKeyID(uint formKeyID);
    void Save(PerkDTO dto);
}