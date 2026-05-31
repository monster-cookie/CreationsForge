using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Repositories.Interfaces;
using SFRecordCompareEngine.Core.Services.Interfaces;

namespace SFRecordCompareEngine.Core.Services;

public class MagicEffectService : IMagicEffectService
{
    private readonly IMagicEffectRepository MagicEffectRepository;

    public MagicEffectService(IMagicEffectRepository repository)
    {
        MagicEffectRepository = repository;
    }

    public IList<MagicEffectDTO> GetByModKey(ModKey modKey) => MagicEffectRepository.GetByModKey(modKey);
    public IList<MagicEffectDTO> GetByFormKeyID(uint formKeyID) => MagicEffectRepository.GetByFormKeyID(formKeyID);
}
