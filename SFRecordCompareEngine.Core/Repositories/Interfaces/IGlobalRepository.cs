using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Repositories.Interfaces;

public interface IGlobalRepository
{
    IList<GlobalDTO> GetByModKey(ModKey modKey);
    IList<GlobalDTO> GetByFormKeyID(uint formKeyID);
    void Save(GlobalDTO dto);
}