using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Repositories.Interfaces;
using SFRecordCompareEngine.Core.Services.Interfaces;

namespace SFRecordCompareEngine.Core.Services;

public class NpcService : INpcService
{
    private readonly INpcRepository NpcRepository;

    public NpcService(INpcRepository repository)
    {
        NpcRepository = repository;
    }

    public IList<NpcDTO> GetByModKey(ModKey modKey) => NpcRepository.GetByModKey(modKey);
    public IList<NpcDTO> GetByFormKeyID(uint formKeyID) => NpcRepository.GetByFormKeyID(formKeyID);
}
