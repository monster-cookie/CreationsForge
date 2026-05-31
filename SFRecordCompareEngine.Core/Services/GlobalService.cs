using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Repositories.Interfaces;
using SFRecordCompareEngine.Core.Services.Interfaces;

namespace SFRecordCompareEngine.Core.Services;

public class GlobalService : IGlobalService
{
    private readonly IGlobalRepository GlobalRepository;

    public GlobalService(IGlobalRepository repository)
    {
        GlobalRepository = repository;
    }

    public IList<GlobalDTO> GetByModKey(ModKey modKey) => GlobalRepository.GetByModKey(modKey);
    public IList<GlobalDTO> GetByFormKeyID(uint formKeyID) => GlobalRepository.GetByFormKeyID(formKeyID);
}
