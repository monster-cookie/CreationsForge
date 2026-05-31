using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Repositories.Interfaces;
using SFRecordCompareEngine.Core.Services.Interfaces;

namespace SFRecordCompareEngine.Core.Services;

public class MagicEffectService : IMagicEffectService
{
    private readonly IMagicEffectRepository Repository;

    public MagicEffectService(IMagicEffectRepository repository)
    {
        Repository = repository;
    }

    public IList<MagicEffectDTO> GetByModKey(ModKey modKey)
    {
        return Repository.GetByModKey(modKey);
    }

    public IList<MagicEffectDTO> GetByFormKeyID(uint formKeyID)
    {
        return Repository.GetByFormKeyID(formKeyID);
    }
}