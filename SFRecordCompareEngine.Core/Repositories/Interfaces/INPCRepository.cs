using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Repositories.Interfaces;

public interface INPCRepository
{
    IList<NPCDTO> GetByModKey(ModKey modKey);
    IList<NPCDTO> GetByFormKeyID(uint formKeyID);
    void Save(NPCDTO dto);
}