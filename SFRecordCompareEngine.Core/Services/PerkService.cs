using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Repositories.Interfaces;
using SFRecordCompareEngine.Core.Services.Interfaces;

namespace SFRecordCompareEngine.Core.Services;

public class PerkService : IPerkService
{
    private readonly IPerkRepository PerkRepository;

    public PerkService(IPerkRepository repository)
    {
        PerkRepository = repository;
    }

    public IList<PerkDTO> GetByModKey(ModKey modKey) => PerkRepository.GetByModKey(modKey);
    public IList<PerkDTO> GetByFormKeyID(uint formKeyID) => PerkRepository.GetByFormKeyID(formKeyID);
}
